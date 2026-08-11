document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.JobSeeker)) return;

  await loadDashboardData();
});

async function loadDashboardData() {
  try {
    // 1. Profile Data & Stats
    const profile = await API.get('job-seekers/me').catch(() => null);
    if (profile) {
      document.getElementById('welcome-name').textContent = `Welcome back, ${escapeHtml(profile.fullName)}!`;
      document.getElementById('stat-skills-count').textContent = (profile.skills || []).length;
      document.getElementById('stat-cv-status').textContent = profile.cv ? 'Uploaded ✓' : 'Not Uploaded';
    }

    // 2. Applications Stats
    const applications = await API.get('applications/mine').catch(() => []);
    document.getElementById('stat-applications-count').textContent = applications.length;

    renderRecentApplications(applications.slice(0, 3));

    // 3. Contact Requests Stats
    const contactRequests = await API.get('contact-requests/mine').catch(() => []);
    const pendingRequests = contactRequests.filter(r => r.status === CONFIG.ContactRequestStatus.Pending || r.status === 'Pending').length;
    document.getElementById('stat-requests-count').textContent = pendingRequests;

    // 4. Discover Jobs
    const jobs = await API.get('jobs/discover').catch(() => []);
    renderRecommendedJobs(jobs.slice(0, 3));

  } catch (err) {
    console.error('Failed to load dashboard data:', err);
    UI.showToast('Failed to load dashboard data', 'error');
  }
}

function renderRecentApplications(apps) {
  const container = document.getElementById('recent-applications-list');
  if (!apps || apps.length === 0) {
    UI.showEmptyState(container, 'No applications submitted yet.', '📄');
    return;
  }

  container.innerHTML = `
    <div style="display:flex; flex-direction:column; gap:0.75rem;">
      ${apps.map(app => `
        <div style="padding:0.85rem; border:1px solid var(--border-color); border-radius:var(--radius-md); display:flex; justify-content:space-between; align-items:center;">
          <div>
            <div style="font-weight:600;">${escapeHtml(app.jobTitle)}</div>
            <div style="font-size:0.8rem; color:var(--text-muted);">${escapeHtml(app.companyName)}</div>
          </div>
          <div>
            ${UI.renderStatusBadge(app.status)}
          </div>
        </div>
      `).join('')}
    </div>
  `;
}

function renderRecommendedJobs(jobs) {
  const container = document.getElementById('recommended-jobs-list');
  if (!jobs || jobs.length === 0) {
    UI.showEmptyState(container, 'No open jobs available at the moment.', '🔍');
    return;
  }

  container.innerHTML = `
    <div style="display:flex; flex-direction:column; gap:0.75rem;">
      ${jobs.map(job => `
        <div style="padding:0.85rem; border:1px solid var(--border-color); border-radius:var(--radius-md); display:flex; justify-content:space-between; align-items:center;">
          <div>
            <div style="font-weight:600;">${escapeHtml(job.title)}</div>
            <div style="font-size:0.8rem; color:var(--text-muted);">${escapeHtml(job.companyName)} • ${escapeHtml(job.location || 'Remote')}</div>
          </div>
          <div style="display:flex; items-center; gap:0.75rem;">
            ${UI.renderMatchBadge(job.matchScore)}
            <a href="job-details.html?id=${job.jobId}" class="btn btn-secondary btn-sm">View</a>
          </div>
        </div>
      `).join('')}
    </div>
  `;
}
