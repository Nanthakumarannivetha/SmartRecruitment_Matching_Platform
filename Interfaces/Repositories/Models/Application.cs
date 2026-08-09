using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.Models
{
    public class Application
    {
        public int Id { get; set; }

        public int JobVacancyId { get; set; }

        public int JobSeekerProfileId { get; set; }

        public ApplicationStatus Status { get; set; }
            = ApplicationStatus.Applied;

        public decimal MatchScore { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public JobVacancy JobVacancy { get; set; } = null!;

        public JobSeekerProfile JobSeekerProfile { get; set; } = null!;

        public ICollection<ContactRequest> ContactRequests { get; set; }
            = new List<ContactRequest>();
    }
}