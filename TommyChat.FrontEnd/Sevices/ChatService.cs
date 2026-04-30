using TommyChat.FrontEnd.Models;

namespace TommyChat.FrontEnd.Sevices;
/// <summary>
/// Servicio singleton que centraliza el estado de todos los chats abiertos.
/// Los componentes se suscriben a OnChange para re-renderizarse.
/// </summary>
public class ChatService
{
    // ── Estado ──────────────────────────────────────────────────────────────
    private readonly Dictionary<string, ChatSession> _sessions = new();

    // ── Eventos ─────────────────────────────────────────────────────────────
    public event Action? OnChange;

    // ── Consultas ───────────────────────────────────────────────────────────
    public IReadOnlyList<ChatSession> OpenSessions => _sessions.Values.ToList().AsReadOnly();

    public bool IsOpen(string peerId) => _sessions.ContainsKey(peerId);

    public int TotalUnread => _sessions.Values.Sum(s => s.UnreadCount);

    // ── Comandos ────────────────────────────────────────────────────────────

    /// Abre o restaura el chat con el compañero indicado.
    public void OpenChat(Peer peer, IEnumerable<ChatMessage>? seedMessages = null)
    {
        if (_sessions.TryGetValue(peer.Id, out var existing))
        {
            // Si estaba minimizado, lo restauramos
            existing.IsMinimized = false;
            Notify();
            return;
        }

        var session = new ChatSession { Peer = peer };
        if (seedMessages is not null)
            session.Messages.AddRange(seedMessages);

        _sessions[peer.Id] = session;
        Notify();
    }

    /// Cierra el chat.
    public void CloseChat(string peerId)
    {
        _sessions.Remove(peerId);
        Notify();
    }

    /// Alterna minimizado / expandido.
    public void ToggleMinimize(string peerId)
    {
        if (!_sessions.TryGetValue(peerId, out var s)) return;
        s.IsMinimized = !s.IsMinimized;

        // Al expandir, marcar como leído
        if (!s.IsMinimized)
            s.UnreadCount = 0;

        Notify();
    }

    /// Envía un mensaje "mío".
    public void SendMessage(string peerId, string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        if (!_sessions.TryGetValue(peerId, out var s)) return;

        s.Messages.Add(new ChatMessage
        {
            Text = text,
            IsMe = true,
            Time = DateTime.Now.ToString("HH:mm")
        });
        Notify();

        // Simular respuesta del compañero (indicador typing → mensaje)
        SimulateReply(peerId);
    }

    /// Marca los mensajes como leídos cuando el usuario expande el chat.
    public void MarkAsRead(string peerId)
    {
        if (!_sessions.TryGetValue(peerId, out var s)) return;
        if (s.UnreadCount == 0) return;
        s.UnreadCount = 0;
        Notify();
    }

    // ── Simulación compañero ────────────────────────────────────────────────
    private void SimulateReply(string peerId)
    {
        // Esperar 1 segundo, mostrar "typing"
        Task.Run(async () =>
        {
            await Task.Delay(1000);
            SetTyping(peerId, true);

            // Simular tiempo de escritura
            await Task.Delay(new Random().Next(1200, 2200));
            SetTyping(peerId, false);

            var replies = new[]
            {
                "Sí, ya lo vi!",
                "Dale, te aviso luego 👍",
                "Che, ¿tenés apuntes?",
                "Entendí todo, gracias!",
                "Espera que pregunto también.",
                "Ok! 🙌"
            };
            var text = replies[new Random().Next(replies.Length)];
            AddIncoming(peerId, text);
        });
    }

    /// Agrega un mensaje entrante (del compañero) con notificación de no leído.
    public void AddIncoming(string peerId, string text)
    {
        if (!_sessions.TryGetValue(peerId, out var s)) return;

        s.Messages.Add(new ChatMessage
        {
            Text = text,
            IsMe = false,
            Time = DateTime.Now.ToString("HH:mm")
        });

        // Solo suma no-leído si el chat está minimizado
        if (s.IsMinimized)
            s.UnreadCount++;

        Notify();
    }

    private void SetTyping(string peerId, bool value)
    {
        if (!_sessions.TryGetValue(peerId, out var s)) return;
        s.IsTyping = value;
        Notify();
    }

    private void Notify() => OnChange?.Invoke();
}