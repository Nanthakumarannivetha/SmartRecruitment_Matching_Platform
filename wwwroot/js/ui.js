// Reusable Vanilla JS UI Components & Helpers
const UI = {
  showToast(message, type = 'info') {
    let container = document.getElementById('toast-container');
    if (!container) {
      container = document.createElement('div');
      container.id = 'toast-container';
      document.body.appendChild(container);
    }

    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `
      <span>${escapeHtml(message)}</span>
      <span style="cursor:pointer;margin-left:1rem;" onclick="this.parentElement.remove()">×</span>
    `;

    container.appendChild(toast);

    setTimeout(() => {
      if (toast.parentElement) toast.remove();
    }, 4000);
  },

  showSpinner(containerId) {
    const el = typeof containerId === 'string' ? document.getElementById(containerId) : containerId;
    if (el) {
      el.innerHTML = `<div class="spinner"></div>`;
    }
  },

  showEmptyState(containerId, message, icon = '📭') {
    const el = typeof containerId === 'string' ? document.getElementById(containerId) : containerId;
    if (el) {
      el.innerHTML = `
        <div class="empty-state">
          <div class="empty-icon">${icon}</div>
          <p>${escapeHtml(message)}</p>
        </div>
      `;
    }
  },

  showConfirmModal({ title, message, confirmText = 'Confirm', confirmClass = 'btn-primary', onConfirm }) {
    const modal = document.createElement('div');
    modal.className = 'modal-backdrop';
    modal.innerHTML = `
      <div class="modal-content">
        <div class="modal-header">
          <h3>${escapeHtml(title)}</h3>
          <span style="cursor:pointer;font-size:1.5rem;" class="close-btn">&times;</span>
        </div>
        <div class="modal-body">
          <p>${escapeHtml(message)}</p>
        </div>
        <div class="modal-footer">
          <button class="btn btn-secondary cancel-btn">Cancel</button>
          <button class="btn ${confirmClass} confirm-btn">${escapeHtml(confirmText)}</button>
        </div>
      </div>
    `;

    document.body.appendChild(modal);

    const closeModal = () => modal.remove();

    modal.querySelector('.close-btn').onclick = closeModal;
    modal.querySelector('.cancel-btn').onclick = closeModal;
    modal.querySelector('.confirm-btn').onclick = () => {
      closeModal();
      if (onConfirm) onConfirm();
    };
  },

  renderMatchBadge(score) {
    const num = parseFloat(score) || 0;
    let cls = 'score-low';
    if (num >= 70) cls = 'score-high';
    else if (num >= 40) cls = 'score-medium';

    return `<span class="match-score-badge ${cls}">🎯 ${num.toFixed(1)}% Match</span>`;
  },

  renderStatusBadge(status) {
    const statusMap = {
      1: { label: 'Applied', class: 'badge-applied' },
      2: { label: 'Under Review', class: 'badge-underreview' },
      3: { label: 'Shortlisted', class: 'badge-shortlisted' },
      4: { label: 'Accepted', class: 'badge-accepted' },
      5: { label: 'Rejected', class: 'badge-rejected' },
      
      // Contact requests
      Pending: { label: 'Pending', class: 'badge-pending' },
      Declined: { label: 'Declined', class: 'badge-declined' },
      
      // Vacancy status
      Open: { label: 'Open', class: 'badge-open' },
      Closed: { label: 'Closed', class: 'badge-closed' }
    };

    const info = statusMap[status] || { label: status, class: 'badge-applied' };
    return `<span class="badge ${info.class}">${escapeHtml(info.label)}</span>`;
  }
};

function escapeHtml(str) {
  if (!str) return '';
  return String(str)
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;');
}
