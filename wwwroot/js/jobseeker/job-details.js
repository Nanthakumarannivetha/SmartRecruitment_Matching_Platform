let currentJobId = null;

document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.JobSeeker)) return;

  const urlParams = new URLSearchParams(window.location.search);
  currentJobId = urlParams.get('id');

  if (!currentJobId) {
    window.location.href = 'jobs.html';
    return;
  }

  await loadJobDetails();
});

async function loadJobDetails() {
  const container = document.getElementById('job-details-container');
  UI.showSpinner(container);

  try {
    const jobMatch = await API.get(`jobs/${currentJobId}/match`);
    const myApplications = await API.get('applications/mine').catch(() => []);
    const existingApp = myApplications.find(a => a.jobVacancyId == currentJobId);

    renderJobMatchDetails(jobMatch, existingApp);
  } catch (err) {
    UI.showEmptyState(container, err.message || 'Failed to load job details.', '⚠️');
  }
}

function renderJobMatchDetails(job, existingApp) {
  const container = document.getElementById('job-details-container');
  const score = parseFloat(job.matchScore) || 0;

  container.innerHTML = `
    <!-- Top Card Header -->
    <div class="card" style="margin-bottom:1.5rem;">
      <div style="display:flex; justify-content:space-between; align-items:flex-start; margin-bottom:1rem;">
        <div>
          <h1 style="font-size:1.75rem; margin-bottom:0.25rem;">${escapeHtml(job.title)}</h1>
          <div style="font-size:1.1rem; color:var(--primary); font-weight:600;">🏢 ${escapeHtml(job.companyName)}</div>
          <div style="display:flex; gap:1.25rem; color:var(--text-muted); font-size:0.9rem; margin-top:0.5rem;">
            <span>📍 ${escapeHtml(job.location || 'Remote')}</span>
            <span>⏳ Min ${job.minimumExperienceYears} years experience</span>
            <span>🎓 ${CONFIG.EducationLevel[job.requiredEducationLevel] || 'Education'}</span>
          </div>
        </div>

        <div style="text-align:right;">
          ${UI.renderMatchBadge(score)}
          <div style="margin-top:0.75rem;">
            ${existingApp ? `
              <span class="badge badge-shortlisted">Already Applied (${UI.renderStatusBadge(existingApp.status)})</span>
            ` : `
              <button id="apply-btn" class="btn btn-primary btn-lg">Apply for Job</button>
            `}
          </div>
        </div>
      </div>

      <!-- Match Score Breakdown Bars -->
      <div style="background:var(--bg-main); border:1px solid var(--border-color); border-radius:var(--radius-md); padding:1.25rem; margin-top:1.5rem;">
        <h4 style="margin-bottom:0.75rem;">Automated Match Score Breakdown</h4>
        
        <div style="margin-bottom:0.6rem;">
          <div style="display:flex; justify-content:space-between; font-size:0.85rem; font-weight:600;">
            <span>Skills Alignment (60% Weight)</span>
            <span>${job.skillsScore || 0}%</span>
          </div>
          <div class="match-progress-bar"><div class="match-progress-fill" style="width:${job.skillsScore || 0}%;"></div></div>
        </div>

        <div style="margin-bottom:0.6rem;">
          <div style="display:flex; justify-content:space-between; font-size:0.85rem; font-weight:600;">
            <span>Experience Threshold (20% Weight)</span>
            <span>${job.experienceScore || 0}%</span>
          </div>
          <div class="match-progress-bar"><div class="match-progress-fill" style="width:${job.experienceScore || 0}%;"></div></div>
        </div>

        <div style="margin-bottom:0.6rem;">
          <div style="display:flex; justify-content:space-between; font-size:0.85rem; font-weight:600;">
            <span>Education Alignment (10% Weight)</span>
            <span>${job.educationScore || 0}%</span>
          </div>
          <div class="match-progress-bar"><div class="match-progress-fill" style="width:${job.educationScore || 0}%;"></div></div>
        </div>

        <div>
          <div style="display:flex; justify-content:space-between; font-size:0.85rem; font-weight:600;">
            <span>Location Proximity (10% Weight)</span>
            <span>${job.locationScore || 0}%</span>
          </div>
          <div class="match-progress-bar"><div class="match-progress-fill" style="width:${job.locationScore || 0}%;"></div></div>
        </div>
      </div>
    </div>

    <!-- Description & Skills Analysis -->
    <div class="grid grid-cols-2">
      <div class="card">
        <h3>Job Description</h3>
        <p style="white-space:pre-line; color:var(--text-main); font-size:0.95rem; line-height:1.6; margin-top:0.75rem;">
          ${escapeHtml(job.description)}
        </p>
      </div>

      <div class="card">
        <h3>Required Skills Analysis</h3>
        <div style="margin-top:1rem;">
          <h4 class="text-success" style="font-size:0.9rem; margin-bottom:0.5rem;">✓ Matched Skills</h4>
          <div class="skills-container" style="margin-bottom:1.25rem;">
            ${(job.matchedSkills && job.matchedSkills.length > 0) ? 
              job.matchedSkills.map(s => `<span class="chip chip-matched">${escapeHtml(s)}</span>`).join('') :
              '<span class="text-muted" style="font-size:0.85rem;">None matched.</span>'
            }
          </div>

          <h4 class="text-danger" style="font-size:0.9rem; margin-bottom:0.5rem;">✕ Missing Skills</h4>
          <div class="skills-container">
            ${(job.missingSkills && job.missingSkills.length > 0) ? 
              job.missingSkills.map(s => `<span class="chip chip-missing">${escapeHtml(s)}</span>`).join('') :
              '<span class="text-success font-semibold" style="font-size:0.85rem;">You possess all required skills!</span>'
            }
          </div>
        </div>
      </div>
    </div>
  `;

  // Attach Apply Listener
  const applyBtn = document.getElementById('apply-btn');
  if (applyBtn) {
    applyBtn.onclick = () => {
      UI.showConfirmModal({
        title: 'Confirm Job Application',
        message: `Are you sure you want to apply for "${job.title}" at ${job.companyName}? Your match score (${score.toFixed(1)}%) will be submitted to the employer.`,
        confirmText: 'Submit Application',
        confirmClass: 'btn-primary',
        onConfirm: async () => {
          applyBtn.disabled = true;
          applyBtn.textContent = 'Submitting...';

          try {
            await API.post(`jobs/${currentJobId}/apply`);
            UI.showToast('Application submitted successfully!', 'success');
            setTimeout(() => {
              window.location.href = 'applications.html';
            }, 800);
          } catch (err) {
            UI.showToast(err.message || 'Failed to submit application.', 'error');
            applyBtn.disabled = false;
            applyBtn.textContent = 'Apply for Job';
          }
        }
      });
    };
  }
}
