let requiredSkills = [];
let editingVacancyId = null;

document.addEventListener('DOMContentLoaded', async () => {
  if (!Auth.requireRole(CONFIG.Roles.Employer)) return;

  const urlParams = new URLSearchParams(window.location.search);
  editingVacancyId = urlParams.get('id');

  setupSkillHandlers();

  if (editingVacancyId) {
    document.getElementById('page-form-title').textContent = 'Edit Job Vacancy';
    document.getElementById('save-btn').textContent = 'Update Vacancy';
    await loadVacancyData(editingVacancyId);
  }

  setupFormSubmit();
});

function setupSkillHandlers() {
  const input = document.getElementById('skill-input');
  const addBtn = document.getElementById('add-skill-btn');

  const addAction = () => {
    const val = input.value.trim();
    if (!val) return;
    if (requiredSkills.some(s => s.toLowerCase() === val.toLowerCase())) return;

    requiredSkills.push(val);
    input.value = '';
    renderSkills();
  };

  addBtn.addEventListener('click', (e) => {
    e.preventDefault();
    addAction();
  });

  input.addEventListener('keypress', (e) => {
    if (e.key === 'Enter') {
      e.preventDefault();
      addAction();
    }
  });
}

function renderSkills() {
  const container = document.getElementById('skills-list');
  if (requiredSkills.length === 0) {
    container.innerHTML = `<span class="text-muted" style="font-size:0.85rem;">No required skills added.</span>`;
    return;
  }

  container.innerHTML = requiredSkills.map((s, idx) => `
    <span class="chip">
      ${escapeHtml(s)}
      <span class="chip-remove" onclick="removeSkillTag(${idx})">×</span>
    </span>
  `).join('');
}

function removeSkillTag(idx) {
  requiredSkills.splice(idx, 1);
  renderSkills();
}

async function loadVacancyData(id) {
  try {
    const job = await API.get(`jobs/${id}`);
    if (job) {
      document.getElementById('title').value = job.title || '';
      document.getElementById('location').value = job.location || '';
      document.getElementById('min-experience').value = job.minimumExperienceYears || 0;
      document.getElementById('education-level').value = job.requiredEducationLevel !== undefined ? job.requiredEducationLevel : 0;
      document.getElementById('description').value = job.description || '';

      requiredSkills = Array.isArray(job.requiredSkills) ? [...job.requiredSkills] : [];
      renderSkills();
    }
  } catch (err) {
    UI.showToast('Failed to load vacancy data.', 'error');
  }
}

function setupFormSubmit() {
  const form = document.getElementById('vacancy-form');
  const saveBtn = document.getElementById('save-btn');

  form.addEventListener('submit', async (e) => {
    e.preventDefault();

    if (requiredSkills.length === 0) {
      UI.showToast('Please add at least one required skill tag.', 'info');
      return;
    }

    const dto = {
      title: document.getElementById('title').value.trim(),
      description: document.getElementById('description').value.trim(),
      location: document.getElementById('location').value.trim() || null,
      minimumExperienceYears: parseInt(document.getElementById('min-experience').value) || 0,
      requiredEducationLevel: parseInt(document.getElementById('education-level').value) || 0,
      requiredSkills: requiredSkills
    };

    saveBtn.disabled = true;
    saveBtn.textContent = editingVacancyId ? 'Updating...' : 'Publishing...';

    try {
      if (editingVacancyId) {
        await API.put(`jobs/${editingVacancyId}`, dto);
        UI.showToast('Job vacancy updated successfully!', 'success');
      } else {
        await API.post('jobs', dto);
        UI.showToast('Job vacancy published successfully!', 'success');
      }

      setTimeout(() => {
        window.location.href = 'vacancies.html';
      }, 700);
    } catch (err) {
      UI.showToast(err.message || 'Failed to save vacancy.', 'error');
      saveBtn.disabled = false;
      saveBtn.textContent = editingVacancyId ? 'Update Vacancy' : 'Publish Job Vacancy';
    }
  });
}
