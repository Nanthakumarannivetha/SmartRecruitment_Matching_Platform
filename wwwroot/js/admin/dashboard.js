document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.Administrator)) return;

  await loadAdminDashboard();
});

async function loadAdminDashboard() {
  try {
    const stats = await API.get('admin/dashboard');
    if (stats) {
      document.getElementById('stat-total-users').textContent = stats.totalUsers || 0;
      document.getElementById('stat-total-vacancies').textContent = stats.totalVacancies || 0;
      document.getElementById('stat-total-applications').textContent = stats.totalApplications || 0;
    }
  } catch (err) {
    UI.showToast('Failed to load admin dashboard statistics.', 'error');
  }
}
