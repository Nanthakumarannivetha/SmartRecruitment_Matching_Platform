using Microsoft.AspNetCore.Identity;
using SmartRecruitment_Project.Models;
using SmartRecruitment_Project.Models.Enums;

namespace SmartRecruitment_Project.Data
{
    /// <summary>
    /// Adds a complete, repeatable demo dataset for university evaluation.
    /// It runs only when the main demo employer account is not already present.
    ///
    /// Demo password for all active demo accounts: Demo@123
    /// CV files are intentionally NOT seeded because the real CV workflow stores
    /// both a physical file and database metadata. Upload a CV through the UI.
    /// </summary>
    public static class DbSeeder
    {
        public const string DemoPassword = "Demo@123";

        public static void Seed(AppDbContext db)
        {
            const string employerEmail = "employer.demo@smart.local";

            // Prevent duplicate demo data on every application startup.
            if (db.Users.Any(x => x.Email == employerEmail))
            {
                return;
            }

            using var transaction = db.Database.BeginTransaction();

            try
            {
                var now = DateTime.UtcNow;
                var passwordHasher = new PasswordHasher<User>();

                User CreateUser(
                    string email,
                    UserRole role,
                    bool isActive = true,
                    int createdDaysAgo = 0)
                {
                    var user = new User
                    {
                        Email = email.Trim().ToLowerInvariant(),
                        Role = role,
                        IsActive = isActive,
                        CreatedAt = now.AddDays(-createdDaysAgo)
                    };

                    user.PasswordHash =
                        passwordHasher.HashPassword(
                            user,
                            DemoPassword);

                    return user;
                }

                // ============================================================
                // 1. DEMO USERS
                // ============================================================

                var employerUser = CreateUser(
                    employerEmail,
                    UserRole.Employer,
                    true,
                    30);

                var arunUser = CreateUser(
                    "jobseeker.demo@smart.local",
                    UserRole.JobSeeker,
                    true,
                    25);

                var nimalUser = CreateUser(
                    "jobseeker2.demo@smart.local",
                    UserRole.JobSeeker,
                    true,
                    20);

                var saraUser = CreateUser(
                    "jobseeker3.demo@smart.local",
                    UserRole.JobSeeker,
                    true,
                    18);

                var adminUser = CreateUser(
                    "admin.demo@smart.local",
                    UserRole.Administrator,
                    true,
                    35);

                var inactiveUser = CreateUser(
                    "inactive.demo@smart.local",
                    UserRole.JobSeeker,
                    false,
                    10);

                db.Users.AddRange(
                    employerUser,
                    arunUser,
                    nimalUser,
                    saraUser,
                    adminUser,
                    inactiveUser);

                db.SaveChanges();

                // ============================================================
                // 2. EMPLOYER PROFILE
                // ============================================================

                var employerProfile = new EmployerProfile
                {
                    UserId = employerUser.Id,
                    CompanyName = "TechNova Solutions (Pvt) Ltd",
                    Location = "Colombo",
                    Website = "https://www.technova-demo.com",
                    Description =
                        "TechNova Solutions is a software development and " +
                        "technology services company specializing in web " +
                        "applications, enterprise systems, cloud solutions " +
                        "and digital transformation. The company recruits " +
                        "software developers, QA engineers, database " +
                        "specialists and cloud professionals.",
                    UpdatedAt = now.AddDays(-14)
                };

                db.EmployerProfiles.Add(employerProfile);

                // ============================================================
                // 3. JOB SEEKER PROFILES
                // ============================================================

                var arunProfile = new JobSeekerProfile
                {
                    UserId = arunUser.Id,
                    FullName = "Arun Kumar",
                    Location = "Colombo",
                    YearsOfExperience = 3,
                    EducationLevel = EducationLevel.Bachelor,
                    Summary =
                        "Software developer with 3 years of experience in " +
                        "ASP.NET Core web application development. Skilled " +
                        "in C#, Entity Framework Core, SQL Server, REST API " +
                        "development and frontend technologies. Experienced " +
                        "in Git-based team development and interested in " +
                        "full-stack and backend software engineering roles.",
                    UpdatedAt = now.AddDays(-5)
                };

                var nimalProfile = new JobSeekerProfile
                {
                    UserId = nimalUser.Id,
                    FullName = "Nimal Perera",
                    Location = "Colombo",
                    YearsOfExperience = 1,
                    EducationLevel = EducationLevel.Diploma,
                    Summary =
                        "Junior web developer with practical experience in " +
                        "C#, ASP.NET Core, HTML, CSS and JavaScript. " +
                        "Interested in growing into a full-stack .NET role.",
                    UpdatedAt = now.AddDays(-4)
                };

                var saraProfile = new JobSeekerProfile
                {
                    UserId = saraUser.Id,
                    FullName = "Sara Fernando",
                    Location = "Kandy",
                    YearsOfExperience = 5,
                    EducationLevel = EducationLevel.Master,
                    Summary =
                        "Experienced backend and cloud-focused software " +
                        "engineer with strong .NET, SQL Server, REST API, " +
                        "Git, Azure and Docker skills. Comfortable with " +
                        "enterprise development and technical leadership.",
                    UpdatedAt = now.AddDays(-3)
                };

                var inactiveProfile = new JobSeekerProfile
                {
                    UserId = inactiveUser.Id,
                    FullName = "Inactive Demo User",
                    Location = "Colombo",
                    YearsOfExperience = 0,
                    EducationLevel = EducationLevel.Diploma,
                    Summary =
                        "Demo account used to test Administrator activate " +
                        "and deactivate functionality.",
                    UpdatedAt = now.AddDays(-2)
                };

                db.JobSeekerProfiles.AddRange(
                    arunProfile,
                    nimalProfile,
                    saraProfile,
                    inactiveProfile);

                db.SaveChanges();

                // ============================================================
                // 4. MASTER SKILLS
                // ============================================================

                var requiredSkillNames = new[]
                {
                    "C#",
                    "ASP.NET Core",
                    "Entity Framework Core",
                    "SQL Server",
                    "HTML",
                    "CSS",
                    "JavaScript",
                    "Git",
                    "REST API",
                    "JWT",
                    "React",
                    "T-SQL",
                    "Stored Procedures",
                    "Database Design",
                    "Docker",
                    "Azure",
                    "Selenium",
                    "Postman",
                    "API Testing",
                    "CI/CD",
                    "Linux",
                    "Python",
                    "Power BI",
                    "Excel",
                    ".NET MAUI",
                    "Kubernetes"
                };

                // Reuse any pre-existing normalized skill rows.
                var skillsByNormalizedName =
                    db.Skills
                        .ToList()
                        .ToDictionary(
                            x => x.NormalizedName,
                            StringComparer.OrdinalIgnoreCase);

                foreach (var skillName in requiredSkillNames)
                {
                    var normalized =
                        skillName.Trim().ToUpperInvariant();

                    if (!skillsByNormalizedName.ContainsKey(normalized))
                    {
                        var skill = new Skill
                        {
                            Name = skillName.Trim(),
                            NormalizedName = normalized
                        };

                        db.Skills.Add(skill);
                        skillsByNormalizedName[normalized] = skill;
                    }
                }

                db.SaveChanges();

                Skill SkillOf(string name)
                {
                    return skillsByNormalizedName[
                        name.Trim().ToUpperInvariant()];
                }

                // ============================================================
                // 5. JOB SEEKER SKILLS
                // ============================================================

                void AddJobSeekerSkills(
                    JobSeekerProfile profile,
                    params string[] skillNames)
                {
                    foreach (var skillName in skillNames)
                    {
                        db.JobSeekerSkills.Add(
                            new JobSeekerSkill
                            {
                                JobSeekerProfileId = profile.Id,
                                SkillId = SkillOf(skillName).Id
                            });
                    }
                }

                AddJobSeekerSkills(
                    arunProfile,
                    "C#",
                    "ASP.NET Core",
                    "Entity Framework Core",
                    "SQL Server",
                    "HTML",
                    "CSS",
                    "JavaScript",
                    "Git",
                    "REST API");

                AddJobSeekerSkills(
                    nimalProfile,
                    "C#",
                    "ASP.NET Core",
                    "HTML",
                    "CSS",
                    "JavaScript");

                AddJobSeekerSkills(
                    saraProfile,
                    "C#",
                    "ASP.NET Core",
                    "Entity Framework Core",
                    "SQL Server",
                    "Git",
                    "REST API",
                    "Azure",
                    "Docker");

                db.SaveChanges();

                // ============================================================
                // 6. TWELVE JOB VACANCIES
                // ============================================================

                JobVacancy CreateJob(
                    string title,
                    string description,
                    string location,
                    int minimumExperience,
                    EducationLevel education,
                    JobStatus status,
                    int createdDaysAgo)
                {
                    return new JobVacancy
                    {
                        EmployerProfileId = employerProfile.Id,
                        Title = title,
                        Description = description,
                        Location = location,
                        MinimumExperienceYears = minimumExperience,
                        RequiredEducationLevel = education,
                        Status = status,
                        CreatedAt = now.AddDays(-createdDaysAgo),
                        UpdatedAt =
                            status == JobStatus.Closed
                                ? now.AddDays(-1)
                                : null
                    };
                }

                var juniorDotNet = CreateJob(
                    "Junior .NET Developer",
                    "We are looking for a Junior .NET Developer to develop " +
                    "and maintain web applications using C# and ASP.NET Core. " +
                    "The candidate will work with Entity Framework Core, " +
                    "SQL Server and REST-based backend services as part of " +
                    "an agile development team.",
                    "Colombo",
                    2,
                    EducationLevel.Bachelor,
                    JobStatus.Open,
                    12);

                var fullStack = CreateJob(
                    "Full Stack .NET Developer",
                    "Develop full-stack business applications using " +
                    "ASP.NET Core, C#, SQL Server and frontend technologies. " +
                    "The role includes API integration, responsive user " +
                    "interfaces and database-driven application development.",
                    "Colombo",
                    3,
                    EducationLevel.Bachelor,
                    JobStatus.Open,
                    11);

                var backendApi = CreateJob(
                    "Backend API Developer",
                    "Design and implement secure RESTful backend APIs using " +
                    "ASP.NET Core and C#. Responsibilities include database " +
                    "integration, API security, authentication and scalable " +
                    "backend service development.",
                    "Colombo",
                    3,
                    EducationLevel.Bachelor,
                    JobStatus.Open,
                    10);

                var frontend = CreateJob(
                    "Frontend Web Developer",
                    "Build responsive and user-friendly web interfaces using " +
                    "HTML, CSS and JavaScript. Experience with React and " +
                    "component-based frontend development is preferred.",
                    "Colombo",
                    2,
                    EducationLevel.Diploma,
                    JobStatus.Open,
                    9);

                var database = CreateJob(
                    "Database Developer",
                    "Design and maintain SQL Server databases, write optimized " +
                    "queries and stored procedures, and support database-driven " +
                    "enterprise applications.",
                    "Colombo",
                    2,
                    EducationLevel.Diploma,
                    JobStatus.Open,
                    8);

                var softwareEngineer = CreateJob(
                    "Software Engineer - .NET",
                    "Develop enterprise .NET applications and participate in " +
                    "source control, deployment and cloud-based development. " +
                    "Experience with Docker and Microsoft Azure is desirable.",
                    "Colombo",
                    4,
                    EducationLevel.Bachelor,
                    JobStatus.Open,
                    7);

                var seniorDotNet = CreateJob(
                    "Senior .NET Developer",
                    "Lead the development of scalable .NET applications using " +
                    "ASP.NET Core, Entity Framework Core and SQL Server. The " +
                    "role requires strong backend development experience and " +
                    "familiarity with Azure cloud services.",
                    "Colombo",
                    5,
                    EducationLevel.Bachelor,
                    JobStatus.Open,
                    6);

                var qaAutomation = CreateJob(
                    "QA Automation Engineer",
                    "Develop automated tests for web and API-based " +
                    "applications. Perform functional and regression testing " +
                    "using Selenium, Postman and automation technologies.",
                    "Colombo",
                    2,
                    EducationLevel.Diploma,
                    JobStatus.Open,
                    5);

                var devOps = CreateJob(
                    "DevOps Engineer",
                    "Manage application deployment pipelines, cloud " +
                    "infrastructure and development operations. Experience " +
                    "with Git, Docker, Azure, CI/CD pipelines and Linux is " +
                    "required.",
                    "Kandy",
                    3,
                    EducationLevel.Bachelor,
                    JobStatus.Open,
                    4);

                var dataAnalyst = CreateJob(
                    "Data Analyst",
                    "Analyze organizational data and prepare business reports " +
                    "and dashboards. The role requires SQL knowledge together " +
                    "with Python, Power BI and Excel skills.",
                    "Colombo",
                    1,
                    EducationLevel.Bachelor,
                    JobStatus.Open,
                    3);

                var mobileDeveloper = CreateJob(
                    ".NET Mobile Developer",
                    "Develop cross-platform mobile applications using .NET " +
                    "technologies. The candidate will integrate REST APIs and " +
                    "participate in Git-based collaborative development.",
                    "Jaffna",
                    2,
                    EducationLevel.Bachelor,
                    JobStatus.Open,
                    2);

                var cloudEngineer = CreateJob(
                    "Cloud Engineer",
                    "Design, deploy and maintain cloud infrastructure and " +
                    "containerized services. The role requires hands-on " +
                    "experience with Azure, Docker, Kubernetes, CI/CD and " +
                    "Linux systems.",
                    "Colombo",
                    4,
                    EducationLevel.Bachelor,
                    JobStatus.Closed,
                    15);

                db.JobVacancies.AddRange(
                    juniorDotNet,
                    fullStack,
                    backendApi,
                    frontend,
                    database,
                    softwareEngineer,
                    seniorDotNet,
                    qaAutomation,
                    devOps,
                    dataAnalyst,
                    mobileDeveloper,
                    cloudEngineer);

                db.SaveChanges();

                // ============================================================
                // 7. REQUIRED SKILLS FOR VACANCIES
                // ============================================================

                void AddJobSkills(
                    JobVacancy vacancy,
                    params string[] skillNames)
                {
                    foreach (var skillName in skillNames)
                    {
                        db.JobVacancySkills.Add(
                            new JobVacancySkill
                            {
                                JobVacancyId = vacancy.Id,
                                SkillId = SkillOf(skillName).Id
                            });
                    }
                }

                AddJobSkills(
                    juniorDotNet,
                    "C#",
                    "ASP.NET Core",
                    "Entity Framework Core",
                    "SQL Server");

                AddJobSkills(
                    fullStack,
                    "C#",
                    "ASP.NET Core",
                    "JavaScript",
                    "HTML",
                    "CSS",
                    "SQL Server");

                AddJobSkills(
                    backendApi,
                    "C#",
                    "ASP.NET Core",
                    "REST API",
                    "SQL Server",
                    "JWT");

                AddJobSkills(
                    frontend,
                    "HTML",
                    "CSS",
                    "JavaScript",
                    "React");

                AddJobSkills(
                    database,
                    "SQL Server",
                    "T-SQL",
                    "Stored Procedures",
                    "Database Design");

                AddJobSkills(
                    softwareEngineer,
                    "C#",
                    "ASP.NET Core",
                    "Git",
                    "Docker",
                    "Azure");

                AddJobSkills(
                    seniorDotNet,
                    "C#",
                    "ASP.NET Core",
                    "Entity Framework Core",
                    "SQL Server",
                    "Azure");

                AddJobSkills(
                    qaAutomation,
                    "Selenium",
                    "C#",
                    "JavaScript",
                    "Postman",
                    "API Testing");

                AddJobSkills(
                    devOps,
                    "Git",
                    "Docker",
                    "Azure",
                    "CI/CD",
                    "Linux");

                AddJobSkills(
                    dataAnalyst,
                    "SQL Server",
                    "Python",
                    "Power BI",
                    "Excel");

                AddJobSkills(
                    mobileDeveloper,
                    "C#",
                    ".NET MAUI",
                    "REST API",
                    "Git");

                AddJobSkills(
                    cloudEngineer,
                    "Azure",
                    "Docker",
                    "Kubernetes",
                    "CI/CD",
                    "Linux");

                db.SaveChanges();

                // ============================================================
                // 8. DEMO APPLICATIONS
                // Match scores below follow the project's actual algorithm:
                // Skills 60%, Experience 20%, Education 10%, Location 10%.
                // ============================================================

                var arunJuniorApplication = new Application
                {
                    JobVacancyId = juniorDotNet.Id,
                    JobSeekerProfileId = arunProfile.Id,
                    Status = ApplicationStatus.Shortlisted,
                    MatchScore = 100.00m,
                    AppliedAt = now.AddDays(-7),
                    UpdatedAt = now.AddDays(-2)
                };

                var nimalJuniorApplication = new Application
                {
                    JobVacancyId = juniorDotNet.Id,
                    JobSeekerProfileId = nimalProfile.Id,
                    Status = ApplicationStatus.UnderReview,
                    MatchScore = 50.00m,
                    AppliedAt = now.AddDays(-6),
                    UpdatedAt = now.AddDays(-1)
                };

                var saraJuniorApplication = new Application
                {
                    JobVacancyId = juniorDotNet.Id,
                    JobSeekerProfileId = saraProfile.Id,
                    Status = ApplicationStatus.Accepted,
                    MatchScore = 90.00m,
                    AppliedAt = now.AddDays(-5),
                    UpdatedAt = now.AddHours(-10)
                };

                var arunBackendApplication = new Application
                {
                    JobVacancyId = backendApi.Id,
                    JobSeekerProfileId = arunProfile.Id,
                    Status = ApplicationStatus.Applied,
                    MatchScore = 88.00m,
                    AppliedAt = now.AddDays(-4)
                };

                var arunSoftwareApplication = new Application
                {
                    JobVacancyId = softwareEngineer.Id,
                    JobSeekerProfileId = arunProfile.Id,
                    Status = ApplicationStatus.UnderReview,
                    MatchScore = 71.00m,
                    AppliedAt = now.AddDays(-3),
                    UpdatedAt = now.AddHours(-18)
                };

                var arunDevOpsApplication = new Application
                {
                    JobVacancyId = devOps.Id,
                    JobSeekerProfileId = arunProfile.Id,
                    Status = ApplicationStatus.Rejected,
                    MatchScore = 42.00m,
                    AppliedAt = now.AddDays(-2),
                    UpdatedAt = now.AddHours(-12)
                };

                var nimalFullStackApplication = new Application
                {
                    JobVacancyId = fullStack.Id,
                    JobSeekerProfileId = nimalProfile.Id,
                    Status = ApplicationStatus.Applied,
                    MatchScore = 66.67m,
                    AppliedAt = now.AddDays(-2)
                };

                var saraSeniorApplication = new Application
                {
                    JobVacancyId = seniorDotNet.Id,
                    JobSeekerProfileId = saraProfile.Id,
                    Status = ApplicationStatus.Shortlisted,
                    MatchScore = 90.00m,
                    AppliedAt = now.AddDays(-2),
                    UpdatedAt = now.AddHours(-8)
                };

                db.Applications.AddRange(
                    arunJuniorApplication,
                    nimalJuniorApplication,
                    saraJuniorApplication,
                    arunBackendApplication,
                    arunSoftwareApplication,
                    arunDevOpsApplication,
                    nimalFullStackApplication,
                    saraSeniorApplication);

                db.SaveChanges();

                // ============================================================
                // 9. CONTACT REQUESTS
                // ============================================================

                db.ContactRequests.AddRange(
                    new ContactRequest
                    {
                        ApplicationId = arunJuniorApplication.Id,
                        Message =
                            "Hello Arun, we reviewed your application and " +
                            "would like to discuss your experience and " +
                            "availability for the Junior .NET Developer position.",
                        Status = ContactRequestStatus.Accepted,
                        CreatedAt = now.AddDays(-2),
                        RespondedAt = now.AddDays(-1)
                    },
                    new ContactRequest
                    {
                        ApplicationId = nimalJuniorApplication.Id,
                        Message =
                            "Hello Nimal, we would like to discuss your " +
                            "Junior .NET Developer application and future " +
                            "learning opportunities.",
                        Status = ContactRequestStatus.Declined,
                        CreatedAt = now.AddDays(-2),
                        RespondedAt = now.AddHours(-16)
                    },
                    new ContactRequest
                    {
                        ApplicationId = saraJuniorApplication.Id,
                        Message =
                            "Hello Sara, your profile is a strong match. " +
                            "Please confirm whether you would like to continue " +
                            "with the recruitment discussion.",
                        Status = ContactRequestStatus.Pending,
                        CreatedAt = now.AddHours(-6)
                    });

                // ============================================================
                // 10. IN-APP NOTIFICATIONS
                // ============================================================

                db.Notifications.AddRange(
                    new Notification
                    {
                        UserId = arunUser.Id,
                        Type = NotificationType.ApplicationStatusChanged,
                        Title = "Application Status Updated",
                        Message =
                            "Your application for Junior .NET Developer " +
                            "has been shortlisted.",
                        IsRead = false,
                        CreatedAt = now.AddDays(-2)
                    },
                    new Notification
                    {
                        UserId = nimalUser.Id,
                        Type = NotificationType.ApplicationStatusChanged,
                        Title = "Application Status Updated",
                        Message =
                            "Your application for Junior .NET Developer " +
                            "is under review.",
                        IsRead = false,
                        CreatedAt = now.AddDays(-1)
                    },
                    new Notification
                    {
                        UserId = saraUser.Id,
                        Type = NotificationType.ApplicationStatusChanged,
                        Title = "Application Status Updated",
                        Message =
                            "Your application for Junior .NET Developer " +
                            "has been accepted.",
                        IsRead = false,
                        CreatedAt = now.AddHours(-10)
                    },
                    new Notification
                    {
                        UserId = arunUser.Id,
                        Type = NotificationType.ApplicationStatusChanged,
                        Title = "Application Status Updated",
                        Message =
                            "Your application for Software Engineer - .NET " +
                            "is under review.",
                        IsRead = true,
                        CreatedAt = now.AddHours(-18)
                    },
                    new Notification
                    {
                        UserId = arunUser.Id,
                        Type = NotificationType.ApplicationStatusChanged,
                        Title = "Application Status Updated",
                        Message =
                            "Your application for DevOps Engineer was not selected.",
                        IsRead = false,
                        CreatedAt = now.AddHours(-12)
                    },
                    new Notification
                    {
                        UserId = arunUser.Id,
                        Type = NotificationType.ContactRequestReceived,
                        Title = "Contact Request Received",
                        Message =
                            "An employer has sent you a contact request.",
                        IsRead = true,
                        CreatedAt = now.AddDays(-2)
                    },
                    new Notification
                    {
                        UserId = nimalUser.Id,
                        Type = NotificationType.ContactRequestReceived,
                        Title = "Contact Request Received",
                        Message =
                            "An employer has sent you a contact request.",
                        IsRead = true,
                        CreatedAt = now.AddDays(-2)
                    },
                    new Notification
                    {
                        UserId = saraUser.Id,
                        Type = NotificationType.ContactRequestReceived,
                        Title = "Contact Request Received",
                        Message =
                            "An employer has sent you a contact request.",
                        IsRead = false,
                        CreatedAt = now.AddHours(-6)
                    },
                    new Notification
                    {
                        UserId = employerUser.Id,
                        Type = NotificationType.ContactRequestResponded,
                        Title = "Contact Request Responded",
                        Message =
                            "Arun Kumar has accepted your contact request.",
                        IsRead = false,
                        CreatedAt = now.AddDays(-1)
                    },
                    new Notification
                    {
                        UserId = employerUser.Id,
                        Type = NotificationType.ContactRequestResponded,
                        Title = "Contact Request Responded",
                        Message =
                            "Nimal Perera has declined your contact request.",
                        IsRead = false,
                        CreatedAt = now.AddHours(-16)
                    });

                db.SaveChanges();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
