namespace SmartRecruitment.API.DTOs.Applications;

public class ApplicantCvDto
{
    public int ApplicationId { get; set; }

    public int JobSeekerProfileId { get; set; }

    public string OriginalFileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; }
}