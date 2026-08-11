document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.Administrator)) return;

  await loadUsers();
});

async function loadUsers() {
  const container = document.getElementById('users-table-container');
  UI.showSpinner(container);

  try {
    const users = await API.get('admin/users');

    if (!users || users.length === 0) {
      UI.showEmptyState(container, 'No user accounts found.', '👥');
      return;
    }

    container.innerHTML = `
      <div class="table-container">
        <table class="table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Email Address</th>
              <th>System Role</th>
              <th>Status</th>
              <th>Created Date</th>
              <th>Action</th>
            </tr>
          </thead>
          <tbody>
            ${users.map(u => {
              const regDate = u.createdAt ? new Date(u.createdAt).toLocaleDateString() : 'N/A';
              const roleName = typeof u.role === 'number' ? (u.role === 1 ? 'JobSeeker' : u.role === 2 ? 'Employer' : 'Administrator') : u.role;
              return `
                <tr>
                  <td>#${u.id}</td>
                  <td class="font-bold">${escapeHtml(u.email)}</td>
                  <td><span class="chip">${escapeHtml(roleName)}</span></td>
                  <td>
                    ${u.isActive ? '<span class="badge badge-accepted">Active</span>' : '<span class="badge badge-rejected">Inactive</span>'}
                  </td>
                  <td>${regDate}</td>
                  <td>
                    ${u.isActive ? `
                      <button class="btn btn-danger btn-sm" onclick="toggleUserStatus(${u.id}, false)">Deactivate</button>
                    ` : `
                      <button class="btn btn-success btn-sm" onclick="toggleUserStatus(${u.id}, true)">Activate</button>
                    `}
                  </td>
                </tr>
              `;
            }).join('')}
          </tbody>
        </table>
      </div>
    `;
  } catch (err) {
    UI.showEmptyState(container, 'Failed to load user accounts.', '⚠️');
  }
}

async function toggleUserStatus(userId, activate) {
  const actionName = activate ? 'Activate' : 'Deactivate';
  
  UI.showConfirmModal({
    title: `${actionName} User Account`,
    message: `Are you sure you want to ${actionName.toLowerCase()} User #${userId}?`,
    confirmText: actionName,
    confirmClass: activate ? 'btn-success' : 'btn-danger',
    onConfirm: async () => {
      try {
        const endpoint = activate ? `admin/users/${userId}/activate` : `admin/users/${userId}/deactivate`;
        await API.patch(endpoint);
        UI.showToast(`User account #${userId} ${actionName.toLowerCase()}d successfully!`, 'success');
        await loadUsers();
      } catch (err) {
        UI.showToast(err.message || `Failed to ${actionName.toLowerCase()} user.`, 'error');
      }
    }
  });
}
