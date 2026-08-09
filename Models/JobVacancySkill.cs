namespace SmartRecruitment_Project.Models
{
    public class JobVacancySkill
    {
        public int JobVacancyId { get; set; }

        public int SkillId { get; set; }

        // Navigation Properties
        public JobVacancy JobVacancy { get; set; } = null!;

        public Skill Skill { get; set; } = null!;
    }
}