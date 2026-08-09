namespace SmartRecruitment_Project.Models
{
    public class Skill
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string NormalizedName { get; set; } = string.Empty;

        // Navigation Properties
        public ICollection<JobSeekerSkill> JobSeekerSkills { get; set; }
            = new List<JobSeekerSkill>();

        public ICollection<JobVacancySkill> JobVacancySkills { get; set; }
            = new List<JobVacancySkill>();
    }
}