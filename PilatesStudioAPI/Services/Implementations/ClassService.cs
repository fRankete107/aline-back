using AutoMapper;
using PilatesStudioAPI.Models.DTOs;
using PilatesStudioAPI.Models.Entities;
using PilatesStudioAPI.Repositories.Interfaces;
using PilatesStudioAPI.Services.Interfaces;

namespace PilatesStudioAPI.Services.Implementations;

public class ClassService : IClassService
{
    private readonly IClassRepository _classRepository;
    private readonly IZoneRepository _zoneRepository;
    private readonly IInstructorRepository _instructorRepository;
    private readonly IMapper _mapper;

    public ClassService(
        IClassRepository classRepository,
        IZoneRepository zoneRepository,
        IInstructorRepository instructorRepository,
        IMapper mapper)
    {
        _classRepository = classRepository;
        _zoneRepository = zoneRepository;
        _instructorRepository = instructorRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ClassDto>> GetAllClassesAsync()
    {
        var classes = await _classRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ClassDto>>(classes);
    }

    public async Task<ClassDto?> GetClassByIdAsync(long id)
    {
        var classEntity = await _classRepository.GetByIdAsync(id);
        return classEntity == null ? null : _mapper.Map<ClassDto>(classEntity);
    }

    public async Task<ClassDto> CreateClassAsync(CreateClassDto createClassDto)
    {
        await ValidateClassCreationAsync(createClassDto);

        var classEntity = _mapper.Map<Class>(createClassDto);
        var createdClass = await _classRepository.CreateAsync(classEntity);
        return _mapper.Map<ClassDto>(createdClass);
    }

    public async Task<ClassDto?> UpdateClassAsync(long id, UpdateClassDto updateClassDto)
    {
        var existingClass = await _classRepository.GetByIdAsync(id);
        if (existingClass == null)
            return null;

        await ValidateClassUpdateAsync(id, updateClassDto, existingClass);

        _mapper.Map(updateClassDto, existingClass);
        var updatedClass = await _classRepository.UpdateAsync(id, existingClass);
        return updatedClass == null ? null : _mapper.Map<ClassDto>(updatedClass);
    }

    public async Task<bool> DeleteClassAsync(long id)
    {
        if (!await _classRepository.ExistsAsync(id))
            return false;

        return await _classRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ClassDto>> GetFilteredClassesAsync(ClassFilterDto filter)
    {
        var classes = await _classRepository.GetFilteredAsync(filter);
        return _mapper.Map<IEnumerable<ClassDto>>(classes);
    }

    public async Task<IEnumerable<ClassDto>> GetClassesByDateRangeAsync(DateOnly startDate, DateOnly endDate)
    {
        if (startDate > endDate)
            throw new ArgumentException("Start date must be before or equal to end date.");

        var classes = await _classRepository.GetByDateRangeAsync(startDate, endDate);
        return _mapper.Map<IEnumerable<ClassDto>>(classes);
    }

    public async Task<IEnumerable<ClassDto>> GetClassesByInstructorAsync(long instructorId)
    {
        if (!await _instructorRepository.ExistsAsync(instructorId))
            throw new ArgumentException($"Instructor with ID {instructorId} not found.");

        var classes = await _classRepository.GetByInstructorAsync(instructorId);
        return _mapper.Map<IEnumerable<ClassDto>>(classes);
    }

    public async Task<IEnumerable<ClassDto>> GetClassesByZoneAsync(long zoneId)
    {
        if (!await _zoneRepository.ExistsAsync(zoneId))
            throw new ArgumentException($"Zone with ID {zoneId} not found.");

        var classes = await _classRepository.GetByZoneAsync(zoneId);
        return _mapper.Map<IEnumerable<ClassDto>>(classes);
    }

    public async Task<IEnumerable<ClassDto>> GetAvailableClassesAsync()
    {
        var classes = await _classRepository.GetAvailableClassesAsync();
        return _mapper.Map<IEnumerable<ClassDto>>(classes);
    }

    public async Task<bool> ClassExistsAsync(long id)
    {
        return await _classRepository.ExistsAsync(id);
    }

    public async Task<bool> HasScheduleConflictAsync(long instructorId, DateOnly date, TimeOnly startTime, TimeOnly endTime, long? excludeClassId = null)
    {
        return await _classRepository.HasScheduleConflictAsync(instructorId, date, startTime, endTime, excludeClassId);
    }

    public async Task<bool> HasZoneConflictAsync(long zoneId, DateOnly date, TimeOnly startTime, TimeOnly endTime, long? excludeClassId = null)
    {
        return await _classRepository.HasZoneConflictAsync(zoneId, date, startTime, endTime, excludeClassId);
    }

    private async Task ValidateClassCreationAsync(CreateClassDto createClassDto)
    {
        if (!await _instructorRepository.ExistsAsync(createClassDto.InstructorId))
            throw new ArgumentException($"Instructor with ID {createClassDto.InstructorId} not found.");

        if (!await _zoneRepository.ExistsAsync(createClassDto.ZoneId))
            throw new ArgumentException($"Zone with ID {createClassDto.ZoneId} not found.");

        var zone = await _zoneRepository.GetByIdAsync(createClassDto.ZoneId);
        if (zone != null && createClassDto.CapacityLimit > zone.Capacity)
            throw new ArgumentException($"Class capacity ({createClassDto.CapacityLimit}) cannot exceed zone capacity ({zone.Capacity}).");

        if (await HasScheduleConflictAsync(createClassDto.InstructorId, createClassDto.ClassDate, createClassDto.StartTime, createClassDto.EndTime))
            throw new InvalidOperationException("The instructor already has a class scheduled at this time.");

        if (await HasZoneConflictAsync(createClassDto.ZoneId, createClassDto.ClassDate, createClassDto.StartTime, createClassDto.EndTime))
            throw new InvalidOperationException("The zone is already booked at this time.");
    }

    private async Task ValidateClassUpdateAsync(long classId, UpdateClassDto updateClassDto, Class existingClass)
    {
        if (updateClassDto.InstructorId.HasValue && !await _instructorRepository.ExistsAsync(updateClassDto.InstructorId.Value))
            throw new ArgumentException($"Instructor with ID {updateClassDto.InstructorId.Value} not found.");

        if (updateClassDto.ZoneId.HasValue && !await _zoneRepository.ExistsAsync(updateClassDto.ZoneId.Value))
            throw new ArgumentException($"Zone with ID {updateClassDto.ZoneId.Value} not found.");

        var zoneId = updateClassDto.ZoneId ?? existingClass.ZoneId;
        var capacityLimit = updateClassDto.CapacityLimit ?? existingClass.CapacityLimit;
        
        var zone = await _zoneRepository.GetByIdAsync(zoneId);
        if (zone != null && capacityLimit > zone.Capacity)
            throw new ArgumentException($"Class capacity ({capacityLimit}) cannot exceed zone capacity ({zone.Capacity}).");

        var reservedSpots = await _classRepository.GetReservedSpotsCountAsync(classId);
        if (capacityLimit < reservedSpots)
            throw new InvalidOperationException($"Cannot reduce capacity below current reservations ({reservedSpots}).");

        var instructorId = updateClassDto.InstructorId ?? existingClass.InstructorId;
        var classDate = updateClassDto.ClassDate ?? existingClass.ClassDate;
        var startTime = updateClassDto.StartTime ?? existingClass.StartTime;
        var endTime = updateClassDto.EndTime ?? existingClass.EndTime;

        if (await HasScheduleConflictAsync(instructorId, classDate, startTime, endTime, classId))
            throw new InvalidOperationException("The instructor already has a class scheduled at this time.");

        if (await HasZoneConflictAsync(zoneId, classDate, startTime, endTime, classId))
            throw new InvalidOperationException("The zone is already booked at this time.");
    }
}