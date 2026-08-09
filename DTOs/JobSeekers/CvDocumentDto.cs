namespace SmartRecruitment_Project.DTOs.JobSeekers
{
    public class CvDocumentDto
    {
        public string OriginalFileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; }
    }
}