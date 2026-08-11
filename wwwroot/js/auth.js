// Authentication & Authorization Helper
const Auth = {
  saveAuth(authResponse) {
    if (!authResponse || !authResponse.token) return;
    localStorage.setItem(CONFIG.TOKEN_KEY, authResponse.token);
    
    const userObj = {
      userId: authResponse.userId,
      email: authResponse.email,
      role: authResponse.role,
      expiresAt: authResponse.expiresAt
    };
    localStorage.setItem(CONFIG.USER_KEY, JSON.stringify(userObj));
  },

  getToken() {
    return localStorage.getItem(CONFIG.TOKEN_KEY);
  },

  getUser() {
    const raw = localStorage.getItem(CONFIG.USER_KEY);
    return raw ? JSON.parse(raw) : null;
  },

  getUserRole() {
    const user = this.getUser();
    return user ? user.role : null;
  },

  isAuthenticated() {
    return !!this.getToken();
  },

  logout() {
    localStorage.removeItem(CONFIG.TOKEN_KEY);
    localStorage.removeItem(CONFIG.USER_KEY);
    window.location.href = getAppRoot() + 'pages/auth/login.html';
  },

  requireAuth() {
    if (!this.isAuthenticated()) {
      window.location.href = getAppRoot() + 'pages/auth/login.html';
      return false;
    }
    return true;
  },

  requireRole(allowedRole) {
    if (!this.requireAuth()) return false;
    
    const role = this.getUserRole();
    if (role !== allowedRole) {
      this.redirectToDashboard(role);
      return false;
    }
    return true;
  },

  redirectToDashboard(role) {
    const targetRole = role || this.getUserRole();
    const root = getAppRoot();
    if (targetRole === CONFIG.Roles.JobSeeker) {
      window.location.href = root + 'pages/jobseeker/dashboard.html';
    } else if (targetRole === CONFIG.Roles.Employer) {
      window.location.href = root + 'pages/employer/dashboard.html';
    } else if (targetRole === CONFIG.Roles.Administrator) {
      window.location.href = root + 'pages/admin/dashboard.html';
    } else {
      window.location.href = root + 'pages/auth/login.html';
    }
  }
};

// Returns path prefix depending on current page depth
function getAppRoot() {
  const path = window.location.pathname;
  if (path.includes('/pages/jobseeker/') || path.includes('/pages/employer/') || path.includes('/pages/admin/')) {
    return '../../';
  }
  if (path.includes('/pages/auth/')) {
    return '../../';
  }
  return './';
}
