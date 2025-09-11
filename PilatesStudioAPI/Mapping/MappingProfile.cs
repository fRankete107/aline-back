using AutoMapper;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.DTOs.Auth;
using PilatesStudioAPI.Models.DTOs.Users;
using PilatesStudioAPI.Models.Entities;

namespace PilatesStudioAPI.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // User Mappings
        CreateMap<User, UserInfoDto>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => 
                src.Instructor != null ? src.Instructor.FirstName :
                src.Student != null ? src.Student.FirstName : string.Empty))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => 
                src.Instructor != null ? src.Instructor.LastName :
                src.Student != null ? src.Student.LastName : string.Empty));

        CreateMap<RegisterRequestDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Instructor Mappings
        CreateMap<Instructor, InstructorDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty));

        CreateMap<InstructorCreateDto, Instructor>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<InstructorUpdateDto, Instructor>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<RegisterRequestDto, Instructor>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Student Mappings
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty));

        CreateMap<StudentCreateDto, Student>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<StudentUpdateDto, Student>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<RegisterRequestDto, Student>()
            .ForMember(dest => dest.BirthDate, opt => opt.MapFrom(src => 
                src.BirthDate.HasValue ? DateOnly.FromDateTime(src.BirthDate.Value) : (DateOnly?)null))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        // Zone Mappings
        CreateMap<Zone, ZoneDto>();
        CreateMap<CreateZoneDto, Zone>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateZoneDto, Zone>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Class Mappings
        CreateMap<Class, ClassDto>()
            .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => 
                src.Instructor != null && src.Instructor.User != null 
                    ? $"{src.Instructor.FirstName} {src.Instructor.LastName}" 
                    : string.Empty))
            .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => 
                src.Zone != null ? src.Zone.Name : string.Empty))
            .ForMember(dest => dest.ReservedSpots, opt => opt.MapFrom(src => 
                src.Reservations.Count(r => r.Status == "confirmed")))
            .ForMember(dest => dest.AvailableSpots, opt => opt.MapFrom(src => 
                src.CapacityLimit - src.Reservations.Count(r => r.Status == "confirmed")));

        CreateMap<CreateClassDto, Class>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "scheduled"))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateClassDto, Class>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Plan Mappings
        CreateMap<Plan, PlanDto>()
            .ForMember(dest => dest.ActiveSubscriptionsCount, opt => opt.MapFrom(src => 
                src.Subscriptions.Count(s => s.Status == "active")));

        CreateMap<CreatePlanDto, Plan>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdatePlanDto, Plan>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Subscription Mappings
        CreateMap<Subscription, SubscriptionDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => 
                src.Student != null ? $"{src.Student.FirstName} {src.Student.LastName}" : string.Empty))
            .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => 
                src.Student != null && src.Student.User != null ? src.Student.User.Email : string.Empty))
            .ForMember(dest => dest.PlanTitle, opt => opt.MapFrom(src => 
                src.Plan != null ? src.Plan.Title : string.Empty))
            .ForMember(dest => dest.PlanPrice, opt => opt.MapFrom(src => 
                src.Plan != null ? src.Plan.Price : 0))
            .ForMember(dest => dest.TotalClasses, opt => opt.MapFrom(src => 
                src.Plan != null ? src.Plan.MonthlyClasses : 0))
            .ForMember(dest => dest.UsedClasses, opt => opt.MapFrom(src => 
                src.Plan != null ? src.Plan.MonthlyClasses - src.ClassesRemaining : 0))
            .ForMember(dest => dest.DaysRemaining, opt => opt.MapFrom(src => 
                Math.Max(0, src.ExpiryDate.DayNumber - DateOnly.FromDateTime(DateTime.Today).DayNumber)))
            .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => 
                src.ExpiryDate < DateOnly.FromDateTime(DateTime.Today) || src.Status == "expired"))
            .ForMember(dest => dest.IsExpiringSoon, opt => opt.MapFrom(src => 
                src.Status == "active" && src.ExpiryDate <= DateOnly.FromDateTime(DateTime.Today.AddDays(7)) && 
                src.ExpiryDate >= DateOnly.FromDateTime(DateTime.Today)));

        CreateMap<CreateSubscriptionDto, Subscription>()
            .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => 
                src.StartDate ?? DateOnly.FromDateTime(DateTime.Today)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "active"))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateSubscriptionDto, Subscription>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Reservation Mappings
        CreateMap<Reservation, ReservationDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => 
                src.Student != null ? $"{src.Student.FirstName} {src.Student.LastName}" : string.Empty))
            .ForMember(dest => dest.StudentEmail, opt => opt.MapFrom(src => 
                src.Student != null && src.Student.User != null ? src.Student.User.Email : string.Empty))
            .ForMember(dest => dest.ClassDate, opt => opt.MapFrom(src => 
                src.Class != null ? src.Class.ClassDate : default))
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => 
                src.Class != null ? src.Class.StartTime : default))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => 
                src.Class != null ? src.Class.EndTime : default))
            .ForMember(dest => dest.ClassType, opt => opt.MapFrom(src => 
                src.Class != null ? src.Class.ClassType : null))
            .ForMember(dest => dest.ClassLevel, opt => opt.MapFrom(src => 
                src.Class != null ? src.Class.DifficultyLevel : string.Empty))
            .ForMember(dest => dest.InstructorName, opt => opt.MapFrom(src => 
                src.Class != null && src.Class.Instructor != null 
                    ? $"{src.Class.Instructor.FirstName} {src.Class.Instructor.LastName}" 
                    : string.Empty))
            .ForMember(dest => dest.ZoneName, opt => opt.MapFrom(src => 
                src.Class != null && src.Class.Zone != null ? src.Class.Zone.Name : string.Empty))
            .ForMember(dest => dest.CanCancel, opt => opt.MapFrom(src => 
                src.Status == "confirmed" && src.Class != null &&
                DateTime.Now < src.Class.ClassDate.ToDateTime(src.Class.StartTime).AddHours(-2)))
            .ForMember(dest => dest.IsUpcoming, opt => opt.MapFrom(src => 
                src.Status == "confirmed" && src.Class != null &&
                (src.Class.ClassDate > DateOnly.FromDateTime(DateTime.Today) ||
                 (src.Class.ClassDate == DateOnly.FromDateTime(DateTime.Today) && 
                  src.Class.StartTime > TimeOnly.FromDateTime(DateTime.Now)))))
            .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => 
                src.Status == "completed"))
            .ForMember(dest => dest.HoursUntilClass, opt => opt.MapFrom(src => 
                src.Class != null 
                    ? Math.Max(0, (int)(src.Class.ClassDate.ToDateTime(src.Class.StartTime) - DateTime.Now).TotalHours)
                    : 0));

        CreateMap<CreateReservationDto, Reservation>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "confirmed"))
            .ForMember(dest => dest.ReservationDate, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdateReservationDto, Reservation>()
            .ForMember(dest => dest.CancelledAt, opt => opt.MapFrom((src, dest) => 
                src.Status == "cancelled" ? DateTime.UtcNow : dest.CancelledAt))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        // Payment Mappings
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => 
                src.Student != null ? $"{src.Student.FirstName} {src.Student.LastName}" : string.Empty))
            .ForMember(dest => dest.PlanTitle, opt => opt.MapFrom(src => 
                src.Plan != null ? src.Plan.Title : string.Empty))
            .ForMember(dest => dest.IsRefundable, opt => opt.MapFrom(src => 
                src.Status == "completed" && src.PaymentDate > DateTime.UtcNow.AddDays(-30)))
            .ForMember(dest => dest.DaysSincePayment, opt => opt.MapFrom(src => 
                Math.Max(0, (int)(DateTime.UtcNow - src.PaymentDate).TotalDays)));

        CreateMap<CreatePaymentDto, Payment>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => "pending"))
            .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => 
                src.PaymentDate ?? DateTime.UtcNow))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<UpdatePaymentDto, Payment>()
            .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => 
                src.PaymentDate ?? DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}