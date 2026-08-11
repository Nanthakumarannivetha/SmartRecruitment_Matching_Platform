document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireAuth()) return;

  await loadNotifications();
});

async function loadNotifications() {
  const container = document.getElementById('notifications-container');
  UI.showSpinner(container);

  try {
    const notifications = await API.get('notifications');

    if (!notifications || notifications.length === 0) {
      UI.showEmptyState(container, 'No notifications found.', '🔔');
      return;
    }

    container.innerHTML = `
      <div style="display:flex; flex-direction:column; gap:0.85rem;">
        ${notifications.map(n => {
          const dateStr = new Date(n.createdAt).toLocaleString();
          return `
            <div class="card" style="padding:1.15rem; ${!n.isRead ? 'border-left:4px solid var(--primary); background:var(--primary-light);' : ''}">
              <div style="display:flex; justify-content:space-between; align-items:flex-start;">
                <div>
                  <h4 style="margin-bottom:0.25rem;">${escapeHtml(n.title)}</h4>
                  <p style="font-size:0.9rem; color:var(--text-main);">${escapeHtml(n.message)}</p>
                  <div class="text-subtle" style="font-size:0.78rem; margin-top:0.5rem;">${dateStr}</div>
                </div>
                ${!n.isRead ? `
                  <button class="btn btn-secondary btn-sm" onclick="markRead(${n.id})">Mark Read</button>
                ` : `
                  <span class="badge badge-closed">Read</span>
                `}
              </div>
            </div>
          `;
        }).join('')}
      </div>
    `;
  } catch (err) {
    UI.showEmptyState(container, 'Failed to load notifications.', '⚠️');
  }
}

async function markRead(id) {
  try {
    await API.patch(`notifications/${id}/read`);
    UI.showToast('Notification marked as read.', 'success');
    await loadNotifications();
    fetchUnreadNotificationCount();
  } catch (err) {
    UI.showToast(err.message || 'Failed to update notification status.', 'error');
  }
}
