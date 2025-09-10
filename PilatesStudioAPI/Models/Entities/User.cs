using Microsoft.AspNetCore.Identity;

namespace PilatesStudioAPI.Models.Entities;

public class User : IdentityUser<long>
{
    public string Role { get; set; } = "student";
    public bool IsActive { get; set; } = true;
    public DateTime? EmailVerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Instructor? Instructor { get; set; }
    public Student? Student { get; set; }
    public ICollection<Contact> AssignedContacts { get; set; } = new List<Contact>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}