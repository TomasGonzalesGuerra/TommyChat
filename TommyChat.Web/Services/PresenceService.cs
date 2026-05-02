using Microsoft.AspNetCore.SignalR.Client;

namespace TommyChat.Web.Services;

public class PresenceService : IAsyncDisposable
{
    private HubConnection? _hub;
    private readonly ITokenService _tokenService;    // tu servicio que devuelve el JWT guardado
    private readonly string _hubUrl;

    // ── Estado público ────────────────────────────────────────────────────
    public List<OnlineUserDto> OnlineUsers { get; private set; } = new();
    public List<ChatSession> OpenSessions { get; private set; } = new();

    // Notifica a los componentes que deben re-renderizar
    public event Action? OnChange;

    public PresenceService(IConfiguration config, ITokenService tokenService)
    {
        _tokenService = tokenService;
        _hubUrl = config["ApiBaseUrl"] + "/hubs/presence";
    }

    // ── Conectar (llama esto tras hacer login) ────────────────────────────
    public async Task ConnectAsync()
    {
        if (_hub is { State: HubConnectionState.Connected }) return;

        var token = await _tokenService.GetTokenAsync();

        _hub = new HubConnectionBuilder()
            .WithUrl(_hubUrl, options =>
            {
                // Manda el JWT en la query string para WebSockets
                options.AccessTokenProvider = () => Task.FromResult<string?>(token);
            })
            .WithAutomaticReconnect()           // reconecta automáticamente
            .Build();

        // ── Handlers de eventos del servidor ─────────────────────────────

        // Lista inicial al conectarse
        _hub.On<List<OnlineUserDto>>("OnlineUsers", users =>
        {
            OnlineUsers = users;
            Notify();
        });

        // Alguien se conectó
        _hub.On<string, string>("UserOnline", (userId, userName) =>
        {
            if (!OnlineUsers.Any(u => u.UserId == userId))
                OnlineUsers.Add(new OnlineUserDto(userId, userName));
            Notify();
        });

        // Alguien se desconectó
        _hub.On<string>("UserOffline", userId =>
        {
            OnlineUsers.RemoveAll(u => u.UserId == userId);
            // Si tenía chat abierto, lo marca como offline pero lo mantiene
            var session = OpenSessions.FirstOrDefault(s => s.PeerId == userId);
            if (session != null) session.PeerOnline = false;
            Notify();
        });

        // Mensaje entrante
        _hub.On<MessageDto>("ReceiveMessage", msg =>
        {
            var session = GetOrCreateSession(msg.FromId, msg.FromName);
            session.Messages.Add(new ChatMessage
            {
                Text = msg.Text,
                FromMe = false,
                Timestamp = msg.Timestamp
            });
            session.Unread++;
            Notify();
        });

        // Confirmación de mensaje enviado (echo)
        _hub.On<SentMessageDto>("MessageSent", msg =>
        {
            var session = OpenSessions.FirstOrDefault(s => s.PeerId == msg.ToId);
            if (session != null)
            {
                // Marca el último mensaje pendiente como entregado
                var pending = session.Messages.LastOrDefault(m => m.FromMe && m.Pending);
                if (pending != null) pending.Pending = false;
                Notify();
            }
        });

        await _hub.StartAsync();
    }

    // ── Desconectar ───────────────────────────────────────────────────────
    public async Task DisconnectAsync()
    {
        if (_hub != null)
            await _hub.StopAsync();
    }

    // ── Abrir chat con un peer ────────────────────────────────────────────
    public void OpenChat(string peerId, string peerName)
    {
        if (OpenSessions.Any(s => s.PeerId == peerId)) return;
        OpenSessions.Add(new ChatSession
        {
            PeerId = peerId,
            PeerName = peerName,
            PeerOnline = OnlineUsers.Any(u => u.UserId == peerId)
        });
        Notify();
    }

    public void CloseChat(string peerId)
    {
        OpenSessions.RemoveAll(s => s.PeerId == peerId);
        Notify();
    }

    public void MarkRead(string peerId)
    {
        var session = OpenSessions.FirstOrDefault(s => s.PeerId == peerId);
        if (session != null) { session.Unread = 0; Notify(); }
    }

    // ── Enviar mensaje ────────────────────────────────────────────────────
    public async Task SendMessageAsync(string toUserId, string text)
    {
        if (_hub is not { State: HubConnectionState.Connected }) return;

        var session = OpenSessions.First(s => s.PeerId == toUserId);
        // Agrega optimistamente (Pending=true hasta el echo)
        session.Messages.Add(new ChatMessage
        {
            Text = text,
            FromMe = true,
            Pending = true,
            Timestamp = DateTime.UtcNow
        });
        Notify();

        await _hub.InvokeAsync("SendMessage", toUserId, text);
    }

    // ── Helpers ───────────────────────────────────────────────────────────
    private ChatSession GetOrCreateSession(string peerId, string peerName)
    {
        var session = OpenSessions.FirstOrDefault(s => s.PeerId == peerId);
        if (session == null)
        {
            session = new ChatSession
            {
                PeerId = peerId,
                PeerName = peerName,
                PeerOnline = true
            };
            OpenSessions.Add(session);
        }
        return session;
    }

    private void Notify() => OnChange?.Invoke();

    public async ValueTask DisposeAsync()
    {
        if (_hub != null)
            await _hub.DisposeAsync();
    }
}

// ── DTOs ──────────────────────────────────────────────────────────────────
public record OnlineUserDto(string UserId, string UserName);
public record MessageDto(string FromId, string FromName, string Text, DateTime Timestamp);
public record SentMessageDto(string ToId, string Text, DateTime Timestamp);

// ── Modelos de sesión de chat ─────────────────────────────────────────────
public class ChatSession
{
    public string PeerId { get; set; } = "";
    public string PeerName { get; set; } = "";
    public bool PeerOnline { get; set; }
    public bool Minimized { get; set; }
    public int Unread { get; set; }
    public List<ChatMessage> Messages { get; set; } = new();
}

public class ChatMessage
{
    public string Text { get; set; } = "";
    public bool FromMe { get; set; }
    public bool Pending { get; set; }
    public DateTime Timestamp { get; set; }
}