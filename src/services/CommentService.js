const API_BASE_URL = 'https://localhost:7241/api';

export async function getComments(token, postId) {
    const response = await fetch(`${API_BASE_URL}/Comment/post/${postId}`, {
        headers: { 'Authorization': `Bearer ${token}` },
    });
    if (!response.ok) throw new Error('Yorumlar yüklenemedi.');
    return await response.json();
}

export async function addComment(token, postId, commentText, parentCommentId = null) {
    const response = await fetch(`${API_BASE_URL}/Comment/${postId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${token}`,
        },
        body: JSON.stringify({
            commentText,
            parentCommentID: parentCommentId,
        }),
    });
    if (!response.ok) throw new Error('Yorum eklenemedi.');
    return await response.json();
}

export async function toggleCommentLike(token, commentId) {
    const response = await fetch(`${API_BASE_URL}/comment/${commentId}/like`, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${token}` },
    });
    if (!response.ok) throw new Error('Beğeni işlemi başarısız.');
    return await response.json();
}