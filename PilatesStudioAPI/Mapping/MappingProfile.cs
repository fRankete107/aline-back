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
    }
}