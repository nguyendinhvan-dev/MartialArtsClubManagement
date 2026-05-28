// API Configuration
const API_BASE_URL = 'https://localhost:5001/api';

// Get JWT token from localStorage
function getToken() {
    return localStorage.getItem('token');
}

// Get Role from localStorage
function getRole() {
    return localStorage.getItem('role');
}

// Get Username from localStorage
function getUsername() {
    return localStorage.getItem('username');
}

// Common API call function
async function apiCall(endpoint, options = {}) {
    const token = getToken();
    const headers = {
        'Content-Type': 'application/json',
        ...options.headers
    };

    if (token) {
        headers['Authorization'] = `Bearer ${token}`;
    }

    const config = {
        ...options,
        headers
    };

    try {
        const response = await fetch(`${API_BASE_URL}${endpoint}`, config);
        
        if (!response.ok) {
            if (response.status === 401) {
                // Token expired or invalid
                localStorage.removeItem('token');
                localStorage.removeItem('role');
                localStorage.removeItem('username');
                window.location.href = '../login.html';
                throw new Error('Unauthorized');
            }
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error('API call error:', error);
        throw error;
    }
}

// GET request
async function get(endpoint) {
    return apiCall(endpoint, { method: 'GET' });
}

// POST request
async function post(endpoint, data) {
    return apiCall(endpoint, {
        method: 'POST',
        body: JSON.stringify(data)
    });
}

// PUT request
async function put(endpoint, data) {
    return apiCall(endpoint, {
        method: 'PUT',
        body: JSON.stringify(data)
    });
}

// DELETE request
async function del(endpoint) {
    return apiCall(endpoint, { method: 'DELETE' });
}

// Authentication API
async function login(username, password) {
    return post('/auth/login', { username, password });
}

async function logout() {
    try {
        await post('/auth/logout');
    } catch (error) {
        console.error('Logout error:', error);
    } finally {
        localStorage.removeItem('token');
        localStorage.removeItem('role');
        localStorage.removeItem('username');
        window.location.href = '../login.html';
    }
}

// Huan LuyenVien Portal API
async function getHLVDashboard() {
    return get('/HuanLuyenVienPortal/dashboard');
}

async function getHLVProfile() {
    return get('/HuanLuyenVienPortal/profile');
}

async function updateHLVProfile(data) {
    return put('/HuanLuyenVienPortal/profile', data);
}

async function getHLVSchedule() {
    return get('/HuanLuyenVienPortal/schedule');
}

async function getHLVStudents(classId = null) {
    const endpoint = classId ? `/HuanLuyenVienPortal/students?classId=${classId}` : '/HuanLuyenVienPortal/students';
    return get(endpoint);
}

async function getAttendance(classId, date) {
    return get(`/HuanLuyenVienPortal/attendance?classId=${classId}&date=${date}`);
}

async function saveAttendance(data) {
    return post('/HuanLuyenVienPortal/attendance', data);
}

async function getExams() {
    return get('/HuanLuyenVienPortal/exams');
}

async function saveExamResults(data) {
    return post('/HuanLuyenVienPortal/exams/results', data);
}

async function getNotifications() {
    return get('/HuanLuyenVienPortal/notifications');
}

async function changePassword(data) {
    return post('/HuanLuyenVienPortal/change-password', data);
}
