// Shared App Layout & Header Render
document.addEventListener('DOMContentLoaded', () => {
  renderAppShell();
  setupMobileToggle();
  fetchUnreadNotificationCount();
});

function renderAppShell() {
  const root = getAppRoot();
  const user = Auth.getUser();
  if (!user) return;

  // Set header user name
  const userNameEl = document.getElementById('header-user-name');
  if (userNameEl) userNameEl.textContent = user.email ? user.email.split('@')[0] : 'User';

  const userAvatarEl = document.getElementById('header-user-avatar');
  if (userAvatarEl) userAvatarEl.textContent = (user.email || 'U')[0].toUpperCase();

  const userRoleEl = document.getElementById('header-user-role');
  if (userRoleEl) userRoleEl.textContent = user.role;

  // Mark current nav item as active
  const currentPath = window.location.pathname;
  document.querySelectorAll('.nav-item').forEach(item => {
    const href = item.getAttribute('href');
    if (href && currentPath.includes(href.replace('../', ''))) {
      item.classList.add('active');
    }
  });

  // Logout listener
  const logoutBtn = document.getElementById('logout-btn');
  if (logoutBtn) {
    logoutBtn.addEventListener('click', (e) => {
      e.preventDefault();
      UI.showConfirmModal({
        title: 'Logout',
        message: 'Are you sure you want to log out of your account?',
        confirmText: 'Logout',
        confirmClass: 'btn-danger',
        onConfirm: () => Auth.logout()
      });
    });
  }
}

function setupMobileToggle() {
  const toggleBtn = document.getElementById('mobile-toggle');
  const sidebar = document.querySelector('.app-sidebar');

  if (toggleBtn && sidebar) {
    toggleBtn.addEventListener('click', () => {
      sidebar.classList.toggle('open');
    });
  }
}

async function fetchUnreadNotificationCount() {
  const countBadge = document.getElementById('notification-badge');
  if (!countBadge || !Auth.isAuthenticated()) return;

  try {
    const notifications = await API.get('notifications');
    if (Array.isArray(notifications)) {
      const unreadCount = notifications.filter(n => !n.isRead).length;
      if (unreadCount > 0) {
        countBadge.textContent = unreadCount > 9 ? '9+' : unreadCount;
        countBadge.classList.remove('hidden');
      } else {
        countBadge.classList.add('hidden');
      }
    }
  } catch (err) {
    // Ignore notification badge fetch error silently
  }
}
