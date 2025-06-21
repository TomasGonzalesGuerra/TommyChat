using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TommyChat.API.Data;
using TommyChat.Shared.Entities;

namespace TommyChat.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ConversationsController(DataContext dataContext) : ControllerBase
{
    private readonly DataContext _dataContext = dataContext;

    // GET: api/Conversations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Conversation>>> GetConversations()
    {
        return await _dataContext.Conversations.ToListAsync();
    }

    // GET: api/Conversations/#
    [HttpGet("{id}")]
    public async Task<ActionResult<Conversation>> GetConversation(int id)
    {
        var conversation = await _dataContext.Conversations.FindAsync(id);

        if (conversation == null)
        {
            return NotFound();
        }

        return conversation;
    }

    // PUT: api/Conversations/#
    [HttpPut("{id}")]
    public async Task<IActionResult> PutConversation(int id, Conversation conversation)
    {
        if (id != conversation.Id)
        {
            return BadRequest();
        }

        _dataContext.Entry(conversation).State = EntityState.Modified;

        try
        {
            await _dataContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ConversationExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // POST: api/Conversations
    [HttpPost]
    public async Task<ActionResult<Conversation>> PostConversation(Conversation conversation)
    {
        _dataContext.Conversations.Add(conversation);
        await _dataContext.SaveChangesAsync();

        return CreatedAtAction("GetConversation", new { id = conversation.Id }, conversation);
    }

    // DELETE: api/Conversations/#
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteConversation(int id)
    {
        var conversation = await _dataContext.Conversations.FindAsync(id);
        if (conversation == null)
        {
            return NotFound();
        }

        _dataContext.Conversations.Remove(conversation);
        await _dataContext.SaveChangesAsync();

        return NoContent();
    }

    private bool ConversationExists(int id)
    {
        return _dataContext.Conversations.Any(e => e.Id == id);
    }





}
