namespace SmartRecruitment_Project.Models
{
    public class CvDocument
    {
        public int Id { get; set; }

        public int JobSeekerProfileId { get; set; }

        public string OriginalFileName { get; set; } = string.Empty;

        public string StoredFileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public JobSeekerProfile JobSeekerProfile { get; set; } = null!;
    }
}