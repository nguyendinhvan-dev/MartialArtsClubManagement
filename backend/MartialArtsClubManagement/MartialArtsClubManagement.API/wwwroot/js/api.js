const API_BASE_URL = '/api';

/**
 * Hàm chung để gọi API với Authorization Header
 */
async function apiCall(endpoint, method = 'GET', body = null) {
    const token = localStorage.getItem('token');
    
    const headers = {
        'Content-Type': 'application/json',
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const config = {
        method: method,
        headers: headers
    };

    if (body) {
        config.body = JSON.stringify(body);
    }

    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, config);
        
        if (response.status === 401) {
            // Unauthorized, redirect to login
            localStorage.removeItem('token');
            localStorage.removeItem('role');
            window.location.href = '/login.html';
            return null;
        }

        const data = await response.json();
        return { success: response.ok, status: response.status, data };
    } catch (error) {
        console.error('API Call Error:', error);
        return { success: false, error: error.message };
    }
}
