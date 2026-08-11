// Form Validation Utilities
const Validation = {
  isValidEmail(email) {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
  },

  isValidPassword(password) {
    // Min 8 chars, 1 upper, 1 lower, 1 digit, 1 special char
    return /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,100}$/.test(password);
  },

  showError(inputId, message) {
    const input = typeof inputId === 'string' ? document.getElementById(inputId) : inputId;
    if (!input) return;
    
    input.style.borderColor = 'var(--danger)';
    
    let errorEl = input.parentElement.querySelector('.error-msg');
    if (!errorEl) {
      errorEl = document.createElement('span');
      errorEl.className = 'error-msg text-danger';
      errorEl.style.fontSize = '0.78rem';
      errorEl.style.marginTop = '0.2rem';
      input.parentElement.appendChild(errorEl);
    }
    errorEl.textContent = message;
  },

  clearError(inputId) {
    const input = typeof inputId === 'string' ? document.getElementById(inputId) : inputId;
    if (!input) return;
    
    input.style.borderColor = '';
    const errorEl = input.parentElement.querySelector('.error-msg');
    if (errorEl) errorEl.remove();
  }
};
