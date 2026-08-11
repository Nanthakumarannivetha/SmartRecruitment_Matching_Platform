document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.Employer)) return;

  await loadVacancies();
});

async function loadVacancies() {
  const container = document.getElementById('vacancies-table-container');
  UI.showSpinner(container);

  try {
    const vacancies = await API.get('jobs/mine');

    if (!vacancies || vacancies.length === 0) {
      UI.showEmptyState(container, 'No vacancies created yet.', '📢');
      return;
    }

    container.innerHTML = `
      <div class="table-container">
        <table class="table">
          <thead>
            <tr>
              <th>Title</th>
              <th>Location</th>
              <th>Min Experience</th>
              <th>Required Skills</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            ${vacancies.map(v => {
              const isOpen = v.status === CONFIG.JobStatus.Open || v.status === 'Open';
              return `
                <tr>
                  <td class="font-bold">${escapeHtml(v.title)}</td>
                  <td>📍 ${escapeHtml(v.location || 'Remote')}</td>
                  <td>${v.minimumExperienceYears} yrs</td>
                  <td>
                    <div class="skills-container">
                      ${(v.requiredSkills || []).map(s => `<span class="chip">${escapeHtml(s)}</span>`).join('')}
                    </div>
                  </td>
                  <td>${UI.renderStatusBadge(v.status)}</td>
                  <td>
                    <div class="flex gap-2">
                      <a href="applicants.html?jobId=${v.id}" class="btn btn-primary btn-sm">Ranked Applicants</a>
                      <a href="edit-vacancy.html?id=${v.id}" class="btn btn-secondary btn-sm">Edit</a>
                      ${isOpen ? `
                        <button class="btn btn-danger btn-sm" onclick="closeVacancy(${v.id}, '${escapeHtml(v.title)}')">Close</button>
                      ` : ''}
                    </div>
                  </td>
                </tr>
              `;
            }).join('')}
          </tbody>
        </table>
      </div>
    `;
  } catch (err) {
    UI.showEmptyState(container, 'Failed to load vacancies.', '⚠️');
  }
}

async function closeVacancy(jobId, title) {
  UI.showConfirmModal({
    title: 'Close Job Vacancy',
    message: `Are you sure you want to close "${title}"? Candidates will no longer be able to submit new applications.`,
    confirmText: 'Close Vacancy',
    confirmClass: 'btn-danger',
    onConfirm: async () => {
      try {
        await API.patch(`jobs/${jobId}/close`);
        UI.showToast('Vacancy closed successfully.', 'success');
        await loadVacancies();
      } catch (err) {
        UI.showToast(err.message || 'Failed to close vacancy.', 'error');
      }
    }
  });
}
