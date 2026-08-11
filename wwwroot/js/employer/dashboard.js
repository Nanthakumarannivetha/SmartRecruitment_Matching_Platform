document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.Employer)) return;

  await loadEmployerDashboard();
});

async function loadEmployerDashboard() {
  try {
    // Company Profile
    const profile = await API.get('employers/me').catch(() => null);
    if (profile) {
      document.getElementById('welcome-company').textContent = `Welcome, ${escapeHtml(profile.companyName)}!`;
      document.getElementById('stat-company-name').textContent = escapeHtml(profile.companyName);
    }

    // Vacancies List
    const vacancies = await API.get('jobs/mine').catch(() => []);
    document.getElementById('stat-total-vacancies').textContent = vacancies.length;

    const openCount = vacancies.filter(v => v.status === CONFIG.JobStatus.Open || v.status === 'Open').length;
    document.getElementById('stat-open-vacancies').textContent = openCount;

    renderRecentVacancies(vacancies);
  } catch (err) {
    console.error('Failed to load employer dashboard:', err);
    UI.showToast('Failed to load dashboard statistics.', 'error');
  }
}

function renderRecentVacancies(vacancies) {
  const container = document.getElementById('my-vacancies-list');
  if (!vacancies || vacancies.length === 0) {
    UI.showEmptyState(container, 'No vacancies created yet. Click "Create New Vacancy" to start posting.', '📢');
    return;
  }

  container.innerHTML = `
    <div class="table-container">
      <table class="table">
        <thead>
          <tr>
            <th>Job Title</th>
            <th>Location</th>
            <th>Min Exp</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          ${vacancies.map(v => `
            <tr>
              <td class="font-bold">${escapeHtml(v.title)}</td>
              <td>📍 ${escapeHtml(v.location || 'Remote')}</td>
              <td>${v.minimumExperienceYears} yrs</td>
              <td>${UI.renderStatusBadge(v.status)}</td>
              <td>
                <a href="applicants.html?jobId=${v.id}" class="btn btn-primary btn-sm">View Applicants</a>
                <a href="edit-vacancy.html?id=${v.id}" class="btn btn-secondary btn-sm">Edit</a>
              </td>
            </tr>
          `).join('')}
        </tbody>
      </table>
    </div>
  `;
}
