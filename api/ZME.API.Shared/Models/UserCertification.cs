using ZME.API.Shared.Enums;

namespace ZME.API.Shared.Models;

public class UserCertification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public int CertificationId { get; set; }
    public Certification? Certification { get; set; }
    public CertificationLevel Level { get; set; }
    public DateTimeOffset Timestamp { get; } = DateTimeOffset.UtcNow;
}
