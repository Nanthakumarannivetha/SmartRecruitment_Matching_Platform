let currentJobId = null;

document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.Employer)) return;

  const urlParams = new URLSearchParams(window.location.search);
  currentJobId = urlParams.get('jobId');

  if (!currentJobId) {
    window.location.href = 'vacancies.html';
    return;
  }

  await loadApplicants();
});

async function loadApplicants() {
  const container = document.getElementById('applicants-table-container');
  UI.showSpinner(container);

  try {
    const job = await API.get(`jobs/${currentJobId}`).catch(() => null);
    if (job) {
      document.getElementById('job-title-header').textContent = `Applicants for "${job.title}"`;
    }

    const applicants = await API.get(`jobs/${currentJobId}/applications`);

    if (!applicants || applicants.length === 0) {
      UI.showEmptyState(container, 'No candidates have applied for this job vacancy yet.', '👥');
      return;
    }

    container.innerHTML = `
      <div class="table-container">
        <table class="table">
          <thead>
            <tr>
              <th>Rank</th>
              <th>Candidate Name</th>
              <th>Match Score</th>
              <th>Experience</th>
              <th>Education</th>
              <th>Skills</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            ${applicants.map((app, idx) => {
              const appStatusVal = typeof app.status === 'number' ? app.status : CONFIG.ApplicationStatus[app.status] || 1;
              return `
                <tr>
                  <td class="font-bold text-muted">#${idx + 1}</td>
                  <td class="font-bold">${escapeHtml(app.fullName)}</td>
                  <td>${UI.renderMatchBadge(app.matchScore)}</td>
                  <td>${app.yearsOfExperience} yrs</td>
                  <td>${CONFIG.EducationLevel[app.educationLevel] || 'N/A'}</td>
                  <td>
                    <div class="skills-container" style="max-width:240px;">
                      ${(app.skills || []).map(s => `<span class="chip">${escapeHtml(s)}</span>`).join('')}
                    </div>
                  </td>
                  <td>${UI.renderStatusBadge(app.status)}</td>
                  <td>
                    <div class="flex gap-2" style="flex-wrap:wrap;">
                      <button class="btn btn-secondary btn-sm" onclick="downloadCv(${app.applicationId}, '${escapeHtml(app.fullName)}')">📄 Download CV</button>
                      <button class="btn btn-primary btn-sm" onclick="openStatusModal(${app.applicationId}, ${appStatusVal})">Update Status</button>
                      <button class="btn btn-success btn-sm" onclick="openContactModal(${app.applicationId}, '${escapeHtml(app.fullName)}')">✉️ Contact</button>
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
    UI.showEmptyState(container, 'Failed to load applicants for this vacancy.', '⚠️');
  }
}

async function downloadCv(applicationId, candidateName) {
  try {
    const blob = await API.get(`applications/${applicationId}/cv`);
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `${candidateName.replace(/\s+/g, '_')}_CV.pdf`;
    document.body.appendChild(a);
    a.click();
    a.remove();
  } catch (err) {
    UI.showToast(err.message || 'CV not available or access denied.', 'error');
  }
}

function openStatusModal(applicationId, currentStatusVal) {
  const modal = document.createElement('div');
  modal.className = 'modal-backdrop';
  modal.innerHTML = `
    <div class="modal-content">
      <div class="modal-header">
        <h3>Update Application Status</h3>
        <span style="cursor:pointer;font-size:1.5rem;" class="close-btn">&times;</span>
      </div>
      <div class="modal-body">
        <div class="form-group">
          <label class="form-label" for="status-select">Select New Status</label>
          <select id="status-select" class="form-control">
            <option value="1" ${currentStatusVal == 1 ? 'selected' : ''}>Applied</option>
            <option value="2" ${currentStatusVal == 2 ? 'selected' : ''}>Under Review</option>
            <option value="3" ${currentStatusVal == 3 ? 'selected' : ''}>Shortlisted</option>
            <option value="4" ${currentStatusVal == 4 ? 'selected' : ''}>Accepted</option>
            <option value="5" ${currentStatusVal == 5 ? 'selected' : ''}>Rejected</option>
          </select>
        </div>
      </div>
      <div class="modal-footer">
        <button class="btn btn-secondary cancel-btn">Cancel</button>
        <button class="btn btn-primary confirm-btn">Update Status</button>
      </div>
    </div>
  `;

  document.body.appendChild(modal);
  const closeModal = () => modal.remove();

  modal.querySelector('.close-btn').onclick = closeModal;
  modal.querySelector('.cancel-btn').onclick = closeModal;
  modal.querySelector('.confirm-btn').onclick = async () => {
    const statusVal = parseInt(document.getElementById('status-select').value);
    closeModal();
    try {
      await API.patch(`applications/${applicationId}/status`, { status: statusVal });
      UI.showToast('Application status updated & candidate notified!', 'success');
      await loadApplicants();
    } catch (err) {
      UI.showToast(err.message || 'Failed to update status.', 'error');
    }
  };
}

function openContactModal(applicationId, candidateName) {
  const modal = document.createElement('div');
  modal.className = 'modal-backdrop';
  modal.innerHTML = `
    <div class="modal-content">
      <div class="modal-header">
        <h3>Send Contact Request</h3>
        <span style="cursor:pointer;font-size:1.5rem;" class="close-btn">&times;</span>
      </div>
      <div class="modal-body">
        <p style="font-size:0.9rem; margin-bottom:1rem;">Send an interview/contact invitation to <strong>${escapeHtml(candidateName)}</strong>.</p>
        <div class="form-group">
          <label class="form-label" for="contact-msg">Optional Message</label>
          <textarea id="contact-msg" class="form-control" rows="3" placeholder="e.g. We were impressed by your match score and would like to schedule a technical interview..."></textarea>
        </div>
      </div>
      <div class="modal-footer">
        <button class="btn btn-secondary cancel-btn">Cancel</button>
        <button class="btn btn-success confirm-btn">Send Request</button>
      </div>
    </div>
  `;

  document.body.appendChild(modal);
  const closeModal = () => modal.remove();

  modal.querySelector('.close-btn').onclick = closeModal;
  modal.querySelector('.cancel-btn').onclick = closeModal;
  modal.querySelector('.confirm-btn').onclick = async () => {
    const msg = document.getElementById('contact-msg').value.trim();
    closeModal();
    try {
      await API.post('contact-requests', { applicationId, message: msg || null });
      UI.showToast('Contact request sent to candidate!', 'success');
    } catch (err) {
      UI.showToast(err.message || 'Failed to send contact request.', 'error');
    }
  };
}
