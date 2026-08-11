let currentSkills = [];

document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.JobSeeker)) return;

  await loadProfile();
  setupSkillHandlers();
  setupCvUpload();
});

async function loadProfile() {
  try {
    const profile = await API.get('job-seekers/me');
    if (profile) {
      document.getElementById('full-name').value = profile.fullName || '';
      document.getElementById('location').value = profile.location || '';
      document.getElementById('experience-years').value = profile.yearsOfExperience || 0;
      document.getElementById('education-level').value = profile.educationLevel !== undefined ? profile.educationLevel : 0;
      document.getElementById('summary').value = profile.summary || '';

      currentSkills = Array.isArray(profile.skills) ? [...profile.skills] : [];
      renderSkills();

      renderCvInfo(profile.cv);
    }
  } catch (err) {
    UI.showToast('Failed to load profile details.', 'error');
  }
}

// Profile Form Submit
document.getElementById('profile-form').addEventListener('submit', async (e) => {
  e.preventDefault();
  const saveBtn = document.getElementById('save-profile-btn');

  const dto = {
    fullName: document.getElementById('full-name').value.trim(),
    location: document.getElementById('location').value.trim() || null,
    yearsOfExperience: parseInt(document.getElementById('experience-years').value) || 0,
    educationLevel: parseInt(document.getElementById('education-level').value) || 0,
    summary: document.getElementById('summary').value.trim() || null
  };

  saveBtn.disabled = true;
  saveBtn.textContent = 'Saving Profile...';

  try {
    const updated = await API.put('job-seekers/me', dto);
    UI.showToast('Profile information updated successfully!', 'success');
  } catch (err) {
    UI.showToast(err.message || 'Failed to update profile.', 'error');
  } finally {
    saveBtn.disabled = false;
    saveBtn.textContent = 'Save Profile Info';
  }
});

// Skills Handling
function setupSkillHandlers() {
  const addBtn = document.getElementById('add-skill-btn');
  const input = document.getElementById('new-skill-input');
  const saveSkillsBtn = document.getElementById('save-skills-btn');

  const addSkillAction = () => {
    const val = input.value.trim();
    if (!val) return;

    if (currentSkills.some(s => s.toLowerCase() === val.toLowerCase())) {
      UI.showToast('Skill is already added.', 'info');
      return;
    }

    currentSkills.push(val);
    input.value = '';
    renderSkills();
  };

  addBtn.addEventListener('click', (e) => {
    e.preventDefault();
    addSkillAction();
  });

  input.addEventListener('keypress', (e) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      addSkillAction();
    }
  });

  saveSkillsBtn.addEventListener('click', async () => {
    saveSkillsBtn.disabled = true;
    saveSkillsBtn.textContent = 'Saving Skills...';

    try {
      await API.put('job-seekers/me/skills', { skills: currentSkills });
      UI.showToast('Skills updated successfully!', 'success');
    } catch (err) {
      UI.showToast(err.message || 'Failed to update skills.', 'error');
    } finally {
      saveSkillsBtn.disabled = false;
      saveSkillsBtn.textContent = 'Save Skills List';
    }
  });
}

function renderSkills() {
  const container = document.getElementById('skills-list');
  if (currentSkills.length === 0) {
    container.innerHTML = `<span class="text-muted" style="font-size:0.9rem;">No skills added yet.</span>`;
    return;
  }

  container.innerHTML = currentSkills.map((skill, index) => `
    <span class="chip">
      ${escapeHtml(skill)}
      <span class="chip-remove" onclick="removeSkill(${index})">×</span>
    </span>
  `).join('');
}

function removeSkill(index) {
  currentSkills.splice(index, 1);
  renderSkills();
}

// CV Upload Handling
function renderCvInfo(cv) {
  const container = document.getElementById('current-cv-info');
  if (!cv) {
    container.innerHTML = `<p class="text-muted">No CV currently uploaded.</p>`;
    return;
  }

  const uploadedDate = new Date(cv.uploadedAt).toLocaleDateString();
  const sizeMb = (cv.fileSize / (1024 * 1024)).toFixed(2);

  container.innerHTML = `
    <div style="display:flex; justify-content:space-between; align-items:center; padding:1rem; border:1px solid var(--border-color); border-radius:var(--radius-md); background:var(--bg-surface);">
      <div>
        <div style="font-weight:600; color:var(--text-main);">📄 ${escapeHtml(cv.originalFileName)}</div>
        <div style="font-size:0.8rem; color:var(--text-muted);">Uploaded on ${uploadedDate} • ${sizeMb} MB</div>
      </div>
      <button id="download-cv-btn" class="btn btn-secondary btn-sm">Download Current CV</button>
    </div>
  `;

  document.getElementById('download-cv-btn').onclick = async () => {
    try {
      const blob = await API.get('job-seekers/me/cv');
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = cv.originalFileName || 'CV_Document.pdf';
      document.body.appendChild(a);
      a.click();
      a.remove();
    } catch (err) {
      UI.showToast('Failed to download CV file.', 'error');
    }
  };
}

function setupCvUpload() {
  const fileInput = document.getElementById('cv-file-input');
  const uploadBtn = document.getElementById('upload-cv-btn');
  const filenameLabel = document.getElementById('cv-selected-filename');

  fileInput.addEventListener('change', () => {
    if (fileInput.files.length > 0) {
      const file = fileInput.files[0];
      filenameLabel.textContent = `Selected: ${file.name} (${(file.size / 1024).toFixed(1)} KB)`;
      uploadBtn.classList.remove('hidden');
    }
  });

  uploadBtn.addEventListener('click', async () => {
    if (!fileInput.files || fileInput.files.length === 0) return;

    const file = fileInput.files[0];
    const formData = new FormData();
    formData.append('file', file);

    uploadBtn.disabled = true;
    uploadBtn.textContent = 'Uploading CV...';

    try {
      const result = await API.post('job-seekers/me/cv', formData);
      UI.showToast('CV uploaded successfully!', 'success');
      renderCvInfo(result);
      fileInput.value = '';
      filenameLabel.textContent = '';
      uploadBtn.classList.add('hidden');
    } catch (err) {
      UI.showToast(err.message || 'Failed to upload CV file.', 'error');
    } finally {
      uploadBtn.disabled = false;
      uploadBtn.textContent = 'Upload CV Document';
    }
  });
}
