namespace PilatesStudioAPI.Models.DTOs.Users;

public class UserDto
{
    public long Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool EmailVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Navigation properties
    public InstructorDto? Instructor { get; set; }
    public StudentDto? Student { get; set; }
}

public class UserCreateDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = "student";
    public bool IsActive { get; set; } = true;
}

public class UserUpdateDto
{
    public string? Email { get; set; }
    public string? Role { get; set; }
    public bool? IsActive { get; set; }
}