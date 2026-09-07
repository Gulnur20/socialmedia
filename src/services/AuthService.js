const API_BASE_URL = 'https://localhost:7241/api'

export async function login(username, password) {
    const response = await fetch(`${API_BASE_URL}/auth/login`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ username, password })
    })
    if (!response.ok) {
        throw new Error('Kullanıcı adı veya şifre hatalı.')
    }
    const data = await response.json()
    return data
}
export async function register(userData) {
    const response = await fetch(`${API_BASE_URL}/auth/register`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(userData),
    });

    if (!response.ok) {

        const errorData = await response.json();
        throw new Error(errorData.message || 'Kayıt sırasında bir hata oluştu.');
    }

    const data = await response.json();
    return data;
}