document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.JobSeeker)) return;

  await loadJobs();

  const searchForm = document.getElementById('search-form');
  searchForm.addEventListener('submit', (e) => {
    e.preventDefault();
    loadJobs();
  });

  document.getElementById('reset-search-btn').addEventListener('click', () => {
    document.getElementById('search-keyword').value = '';
    document.getElementById('search-location').value = '';
    loadJobs();
  });
});

async function loadJobs() {
  const container = document.getElementById('jobs-container');
  UI.showSpinner(container);

  const keyword = document.getElementById('search-keyword').value.trim();
  const location = document.getElementById('search-location').value.trim();

  let query = 'jobs/discover';
  const params = [];
  if (keyword) params.push(`search=${encodeURIComponent(keyword)}`);
  if (location) params.push(`location=${encodeURIComponent(location)}`);
  if (params.length > 0) query += '?' + params.join('&');

  try {
    const jobs = await API.get(query);
    renderJobsList(jobs);
  } catch (err) {
    UI.showEmptyState(container, 'Failed to fetch job vacancies.', '⚠️');
  }
}

function renderJobsList(jobs) {
  const container = document.getElementById('jobs-container');
  if (!jobs || jobs.length === 0) {
    UI.showEmptyState(container, 'No job vacancies found matching your search.', '🔍');
    return;
  }

  container.innerHTML = jobs.map(job => `
    <div class="job-card">
      <div>
        <div class="job-header">
          <span class="job-title">${escapeHtml(job.title)}</span>
          ${UI.renderMatchBadge(job.matchScore)}
        </div>
        <div class="company-name">🏢 ${escapeHtml(job.companyName || 'Employer')}</div>
        <div class="job-meta">
          <span>📍 ${escapeHtml(job.location || 'Remote')}</span>
          <span>⏳ ${job.minimumExperienceYears}+ yrs exp</span>
          <span>🎓 ${CONFIG.EducationLevel[job.requiredEducationLevel] || 'Education'}</span>
        </div>

        <p class="text-muted" style="font-size:0.875rem; margin-bottom:1rem; display:-webkit-box; -webkit-line-clamp:2; -webkit-box-orient:vertical; overflow:hidden;">
          ${escapeHtml(job.description)}
        </p>

        <div style="margin-bottom:1rem;">
          <div style="font-size:0.78rem; font-weight:600; color:var(--text-muted); margin-bottom:0.35rem;">REQUIRED SKILLS</div>
          <div class="skills-container">
            ${(job.requiredSkills || []).map(skill => `
              <span class="chip ${(job.matchedSkills || []).includes(skill) ? 'chip-matched' : 'chip-missing'}">
                ${escapeHtml(skill)}
              </span>
            `).join('')}
          </div>
        </div>
      </div>

      <div style="border-top:1px solid var(--border-color); padding-top:1rem; margin-top:1rem; display:flex; justify-content:space-between; align-items:center;">
        <span class="text-subtle" style="font-size:0.8rem;">Click view details for full score breakdown</span>
        <a href="job-details.html?id=${job.jobId}" class="btn btn-primary btn-sm">View Job Details</a>
      </div>
    </div>
  `).join('');
}
