document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.JobSeeker)) return;

  await loadContactRequests();
});

async function loadContactRequests() {
  const container = document.getElementById('contact-requests-container');
  UI.showSpinner(container);

  try {
    const requests = await API.get('contact-requests/mine');

    if (!requests || requests.length === 0) {
      UI.showEmptyState(container, 'No employer contact requests received yet.', '✉️');
      return;
    }

    container.innerHTML = `
      <div style="display:flex; flex-direction:column; gap:1.25rem;">
        ${requests.map(req => {
          const reqDate = new Date(req.createdAt).toLocaleDateString();
          const isPending = req.status === CONFIG.ContactRequestStatus.Pending || req.status === 'Pending';
          
          return `
            <div class="card">
              <div style="display:flex; justify-content:space-between; align-items:flex-start; margin-bottom:0.75rem;">
                <div>
                  <h3 style="margin-bottom:0.25rem;">Contact Request from Employer</h3>
                  <div class="text-muted" style="font-size:0.85rem;">Received on ${reqDate}</div>
                </div>
                <div>${UI.renderStatusBadge(req.status)}</div>
              </div>

              ${req.message ? `
                <div style="background:var(--bg-main); border:1px solid var(--border-color); padding:1rem; border-radius:var(--radius-md); font-size:0.9rem; margin-bottom:1rem;">
                  "${escapeHtml(req.message)}"
                </div>
              ` : ''}

              <div style="display:flex; justify-content:flex-end; gap:0.75rem;">
                ${isPending ? `
                  <button class="btn btn-secondary btn-sm" onclick="respondRequest(${req.id}, 3)">Decline</button>
                  <button class="btn btn-success btn-sm" onclick="respondRequest(${req.id}, 2)">Accept Request</button>
                ` : `
                  <span class="text-muted" style="font-size:0.85rem;">Responded on ${req.respondedAt ? new Date(req.respondedAt).toLocaleDateString() : 'recently'}</span>
                `}
              </div>
            </div>
          `;
        }).join('')}
      </div>
    `;
  } catch (err) {
    UI.showEmptyState(container, 'Failed to load contact requests.', '⚠️');
  }
}

async function respondRequest(requestId, statusValue) {
  const actionText = statusValue === 2 ? 'Accept' : 'Decline';
  
  UI.showConfirmModal({
    title: `${actionText} Contact Request`,
    message: `Are you sure you want to ${actionText.toLowerCase()} this contact request?`,
    confirmText: actionText,
    confirmClass: statusValue === 2 ? 'btn-success' : 'btn-danger',
    onConfirm: async () => {
      try {
        await API.patch(`contact-requests/${requestId}/respond`, { status: statusValue });
        UI.showToast(`Contact request ${actionText.toLowerCase()}ed!`, 'success');
        await loadContactRequests();
      } catch (err) {
        UI.showToast(err.message || 'Failed to respond to request.', 'error');
      }
    }
  });
}
