public interface IEnrollmentService
{
    Task<EnrollmentRecord> EnrollAsync(string courseCode, string studentId);
    Task<EnrollmentRecord?> GetByIdAsync(string id);
    Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync();

    Task<bool> DeleteAsync(string id);
   
}

public class EnrollmentService : IEnrollmentService
{
    private readonly Dictionary<string, EnrollmentRecord> _store=new();

    private readonly ILogger<EnrollmentService> _logger;

    public EnrollmentService(ILogger<EnrollmentService> logger)
    {
        _logger = logger;
    }

    public Task<EnrollmentRecord> EnrollAsync(string studentId, string courseCode)
    {
        var existing = _store.Values.FirstOrDefault(e => e.StudentId == studentId && e.CourseCode == courseCode);

        if (existing is not null)
        {
            _logger.LogWarning("duplicate enrollment attempt {StudentId} in course {CourseCode} (record {EnrollmentId})", studentId, courseCode, existing.Id);
            return Task.FromResult(existing);
        }

        var id = Guid.NewGuid().ToString("N")[..8];

        var record = new EnrollmentRecord(id,studentId,courseCode,DateTime.UtcNow);
        _store[id] = record;

        _logger.LogInformation("Enrolled student {StudentId} in course {CourseCode} with record id {RecordId}", studentId, courseCode, id);

        return Task.FromResult(record);
        
    }

    public Task<EnrollmentRecord?> GetByIdAsync(string id)
    {
        _store.TryGetValue(id, out var record);
        if(record is null)
        {
            _logger.LogWarning("Enrollment record {RecordId} not found", id);
        }
        return Task.FromResult(record);
    }

    public Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync()
    {
      
       IReadOnlyList<EnrollmentRecord> records = _store.Values.ToList();
        return Task.FromResult(records);
    }
    

    public Task<bool> DeleteAsync(string id)
    {
        var removed = _store.Remove(id);
        if (removed)
        {
            _logger.LogInformation("Enrollment record {RecordId} deleted", id);
        }
        else
        {
            _logger.LogWarning("Enrollment record {RecordId} not found for deletion", id);
        }
        return Task.FromResult(removed);
    }

   


}

 public class TmsDatabaseException(string message) : Exception(message);
