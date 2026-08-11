// Central API Configuration
const CONFIG = {
  // Replace with API port when running locally or via ASP.NET Core hosting
  API_BASE_URL: window.location.origin.includes('7286') || window.location.origin.includes('5000') || window.location.origin.includes('5001') || window.location.origin.includes('5271')
    ? `${window.location.origin}/api`
    : 'https://localhost:7286/api',
    
  TOKEN_KEY: 'smart_recruitment_token',
  USER_KEY: 'smart_recruitment_user',
  
  // Real Backend Enums
  Roles: {
    JobSeeker: 'JobSeeker',
    Employer: 'Employer',
    Administrator: 'Administrator'
  },
  
  JobStatus: {
    Open: 1,
    Closed: 2
  },
  
  ApplicationStatus: {
    Applied: 1,
    UnderReview: 2,
    Shortlisted: 3,
    Accepted: 4,
    Rejected: 5
  },

  ContactRequestStatus: {
    Pending: 1,
    Accepted: 2,
    Declined: 3
  },
  
  EducationLevel: {
    0: 'Not Specified',
    1: 'O/Level',
    2: 'A/Level',
    3: 'Diploma',
    4: 'Bachelor',
    5: 'Master',
    6: 'Doctorate'
  }
};
