namespace SmartRecruitment_Project.Models
{
    public class JobSeekerSkill
    {
        public int JobSeekerProfileId { get; set; }

        public int SkillId { get; set; }

        // Navigation Properties
        public JobSeekerProfile JobSeekerProfile { get; set; } = null!;

        public Skill Skill { get; set; } = null!;
    }
}