const API_BASE_URL = 'https://localhost:7241/api';

export async function getTimeline(token, page = 1, pageSize = 20) {
    const response = await fetch(`${API_BASE_URL}/post/timeline?page=${page}&pageSize=${pageSize}`, {
        headers: {
            'Authorization': `Bearer ${token}`,
        },
    });

    if (!response.ok) {
        throw new Error('Postlar yüklenemedi.');
    }

    return await response.json();
}

export async function createPost(token, postData) {
    const response = await fetch(`${API_BASE_URL}/post`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify(postData),
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Post oluşturulamadı.');
    }

    return await response.json();
}

export async function toggleLike(token, postId) {
    const response = await fetch(`${API_BASE_URL}/post/${postId}/like`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
        },
    });

    if (!response.ok) {
        throw new Error('Beğeni işlemi başarısız.');
    }

    return await response.json();
}

export async function deletePost(token, postId) {
    const response = await fetch(`${API_BASE_URL}/post/${postId}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` },
    });

    if (!response.ok) {
        throw new Error('Post silinemedi.');
    }
}

export async function getPostById(token, postId) {
    const response = await fetch(`${API_BASE_URL}/post/${postId}`, {
        headers: token ? { 'Authorization': `Bearer ${token}` } : {},
    });
    if (!response.ok) throw new Error('Post yüklenemedi.');
    return await response.json();
}

export async function getPostsByUserId(token, userId) {
    const response = await fetch(`${API_BASE_URL}/post/user/${userId}`, {
        headers: {
            'Authorization': `Bearer ${token}`,
        },
    });

    if (!response.ok) {
        throw new Error('Postlar yüklenemedi.');
    }

    return await response.json();
}

export async function getPostLikes(token, postId) {
    const response = await fetch(`${API_BASE_URL}/post/${postId}/likes`, {
        headers: { 'Authorization': `Bearer ${token}` },
    });
    if (!response.ok) throw new Error('Beğeniler yüklenemedi.');
    return await response.json();
}

export async function updatePost(token, postId, caption) {
    const response = await fetch(`${API_BASE_URL}/post/${postId}`, {
        method: 'PUT',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify({ caption }),
    });
    if (!response.ok) throw new Error('Post güncellenemedi.');
    return await response.json();
}