const API_BASE_URL = 'https://localhost:7241/api';

export async function uploadFile(token, file, uploadType = 'post') {
    const formData = new FormData();
    formData.append('file', file);

    const response = await fetch(`${API_BASE_URL}/upload?uploadType=${uploadType}`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
        },
        body: formData,
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Dosya yüklenemedi.');
    }

    const result = await response.json();
    return result.url;
}