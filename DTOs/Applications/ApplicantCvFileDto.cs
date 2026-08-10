namespace SmartRecruitment.API.DTOs.Applications;

public class ApplicantCvFileDto
{
    public byte[] FileBytes { get; set; } = Array.Empty<byte>();

    public string ContentType { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;
}