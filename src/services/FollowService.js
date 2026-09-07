const API_BASE_URL = 'https://localhost:7241/api';

export async function followUser(token, followingId) {
    const response = await fetch(`${API_BASE_URL}/follow/${followingId}`, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
        },
    });

    if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Takip işlemi başarısız.');
    }

    return await response.json();
}

export async function unfollowUser(token, followingId) {
    const response = await fetch(`${API_BASE_URL}/follow/${followingId}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` },
    });

    if (!response.ok) {
        throw new Error('Takipten çıkma işlemi başarısız.');
    }
}

export async function getFollowers(token, userId) {
    const response = await fetch(`${API_BASE_URL}/follow/followers/${userId}`, {
        headers: { 'Authorization': `Bearer ${token}` },
    });
    if (!response.ok) throw new Error('Takipçiler yüklenemedi.');
    return await response.json();
}

export async function getFollowing(token, userId) {
    const response = await fetch(`${API_BASE_URL}/follow/following/${userId}`, {
        headers: { 'Authorization': `Bearer ${token}` },
    });
    if (!response.ok) throw new Error('Takip edilenler yüklenemedi.');
    return await response.json();
}
export async function getFollowRequests(token) {
    const response = await fetch(`${API_BASE_URL}/follow/requests`, {
        headers: { 'Authorization': `Bearer ${token}` },
    });
    if (!response.ok) throw new Error('Takip istekleri yüklenemedi.');
    return await response.json();
}

export async function acceptFollowRequest(token, requestId) {
    const response = await fetch(`${API_BASE_URL}/follow/requests/${requestId}/accept`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${token}` },
    });
    if (!response.ok) throw new Error('İstek kabul edilemedi.');
}

export async function rejectFollowRequest(token, requestId) {
    const response = await fetch(`${API_BASE_URL}/follow/requests/${requestId}/reject`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${token}` },
    });
    if (!response.ok) throw new Error('İstek reddedilemedi.');
}