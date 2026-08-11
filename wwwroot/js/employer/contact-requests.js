document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.Employer)) return;

  await loadEmployerContactRequests();
});

async function loadEmployerContactRequests() {
  const container = document.getElementById('employer-contact-requests-container');
  UI.showSpinner(container);

  try {
    const requests = await API.get('contact-requests/mine');

    if (!requests || requests.length === 0) {
      UI.showEmptyState(container, 'No contact requests sent yet.', '✉️');
      return;
    }

    container.innerHTML = `
      <div style="display:flex; flex-direction:column; gap:1rem;">
        ${requests.map(req => {
          const sentDate = new Date(req.createdAt).toLocaleDateString();
          return `
            <div class="card">
              <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:0.5rem;">
                <div>
                  <h4 style="margin-bottom:0.2rem;">Contact Request #${req.id}</h4>
                  <div class="text-subtle" style="font-size:0.8rem;">Sent on ${sentDate}</div>
                </div>
                <div>${UI.renderStatusBadge(req.status)}</div>
              </div>
              ${req.message ? `<p style="font-size:0.875rem; color:var(--text-muted); background:var(--bg-main); padding:0.75rem; border-radius:var(--radius-md);">"${escapeHtml(req.message)}"</p>` : ''}
            </div>
          `;
        }).join('')}
      </div>
    `;
  } catch (err) {
    UI.showEmptyState(container, 'Failed to load contact requests.', '⚠️');
  }
}
