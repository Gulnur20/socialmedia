const API_BASE_URL = 'https://localhost:7241/api';

export async function getUserProfile(token, userId) {
  const response = await fetch(`${API_BASE_URL}/users/${userId}`, {
    headers: {
      'Authorization': `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    throw new Error('Profil yüklenemedi.');
  }

  return await response.json();
}

export async function getMyProfile(token) {
  const response = await fetch(`${API_BASE_URL}/users/me`, {
    headers: { 'Authorization': `Bearer ${token}` },
  });
  if (!response.ok) throw new Error('Profil yüklenemedi.');
  return await response.json();
}

export async function updateProfile(token, profileData) {
  const response = await fetch(`${API_BASE_URL}/users/profile`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
    body: JSON.stringify(profileData),
  });

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || 'Profil güncellenemedi.');
  }
}
export async function searchUsers(token, query) {
  const response = await fetch(`${API_BASE_URL}/users/search?query=${encodeURIComponent(query)}`, {
    headers: {
      'Authorization': `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    throw new Error('Arama yapılamadı.');
  }

  return await response.json();
}
export async function updateSettings(token, settingsData) {
  const response = await fetch(`${API_BASE_URL}/users/settings`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
    body: JSON.stringify(settingsData),
  });

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || 'Ayarlar güncellenemedi.');
  }
}

export async function freezeAccount(token) {
  const response = await fetch(`${API_BASE_URL}/users/freeze`, {
    method: 'POST',
    headers: { 'Authorization': `Bearer ${token}` },
  });
  if (!response.ok) throw new Error('Hesap dondurulamadı.');
}

export async function activateAccount(token) {
  const response = await fetch(`${API_BASE_URL}/users/activate`, {
    method: 'POST',
    headers: { 'Authorization': `Bearer ${token}` },
  });
  if (!response.ok) throw new Error('Hesap aktifleştirilemedi.');
}

export async function deleteAccount(token) {
  const response = await fetch(`${API_BASE_URL}/users/me`, {
    method: 'DELETE',
    headers: { 'Authorization': `Bearer ${token}` },
  });
  if (!response.ok) throw new Error('Hesap silinemedi.');
}

export async function changePassword(token, currentPassword, newPassword) {
  const response = await fetch(`${API_BASE_URL}/users/password`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`,
    },
    body: JSON.stringify({ currentPassword, newPassword }),
  });

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || 'Şifre değiştirilemedi.');
  }
}