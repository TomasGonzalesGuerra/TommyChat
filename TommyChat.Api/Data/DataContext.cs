using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TommyChat.Shared.Entities;

namespace TommyChat.API.Data
{
    public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Conversation>()
            .HasOne(c => c.ParticipantA)
            .WithMany(u => u.ConversationsAsParticipantA)
            .HasForeignKey(c => c.ParticipantAId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>()
            .HasOne(c => c.ParticipantB)
            .WithMany(u => u.ConversationsAsParticipantB)
            .HasForeignKey(c => c.ParticipantBId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
            .HasOne(m => m.Sender)
            .WithMany(u => u.MessagesSent)
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
