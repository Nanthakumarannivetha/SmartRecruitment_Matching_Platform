document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.JobSeeker)) return;

  await loadApplications();
});

async function loadApplications() {
  const tbody = document.getElementById('applications-table-body');

  try {
    const apps = await API.get('applications/mine');
    if (!apps || apps.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="6">
            <div class="empty-state">
              <div class="empty-icon">📄</div>
              <p>You haven't submitted any job applications yet.</p>
              <a href="jobs.html" class="btn btn-primary btn-sm" style="margin-top:1rem;">Discover Open Jobs</a>
            </div>
          </td>
        </tr>
      `;
      return;
    }

    tbody.innerHTML = apps.map(app => {
      const appliedDate = new Date(app.appliedAt).toLocaleDateString();
      return `
        <tr>
          <td class="font-bold">${escapeHtml(app.jobTitle)}</td>
          <td>🏢 ${escapeHtml(app.companyName)}</td>
          <td>${appliedDate}</td>
          <td>${UI.renderMatchBadge(app.matchScore)}</td>
          <td>${UI.renderStatusBadge(app.status)}</td>
          <td>
            <a href="job-details.html?id=${app.jobVacancyId}" class="btn btn-secondary btn-sm">View Vacancy</a>
          </td>
        </tr>
      `;
    }).join('');
  } catch (err) {
    tbody.innerHTML = `
      <tr>
        <td colspan="6" class="text-center text-danger">Failed to load applications. ${escapeHtml(err.message)}</td>
      </tr>
    `;
  }
}
