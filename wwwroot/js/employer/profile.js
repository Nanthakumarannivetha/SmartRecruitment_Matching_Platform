document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.Employer)) return;

  await loadEmployerProfile();
});

async function loadEmployerProfile() {
  try {
    const profile = await API.get('employers/me');
    if (profile) {
      document.getElementById('company-name').value = profile.companyName || '';
      document.getElementById('location').value = profile.location || '';
      document.getElementById('website').value = profile.website || '';
      document.getElementById('description').value = profile.description || '';
    }
  } catch (err) {
    UI.showToast('Failed to load company profile.', 'error');
  }
}

document.getElementById('employer-profile-form').addEventListener('submit', async (e) => {
  e.preventDefault();

  const saveBtn = document.getElementById('save-btn');
  const dto = {
    companyName: document.getElementById('company-name').value.trim(),
    location: document.getElementById('location').value.trim() || null,
    website: document.getElementById('website').value.trim() || null,
    description: document.getElementById('description').value.trim() || null
  };

  saveBtn.disabled = true;
  saveBtn.textContent = 'Saving Profile...';

  try {
    await API.put('employers/me', dto);
    UI.showToast('Company profile saved successfully!', 'success');
  } catch (err) {
    UI.showToast(err.message || 'Failed to save company profile.', 'error');
  } finally {
    saveBtn.disabled = false;
    saveBtn.textContent = 'Save Company Profile';
  }
});
