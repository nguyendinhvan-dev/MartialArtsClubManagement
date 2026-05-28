// Huấn Luyện Viên Portal - Specific JavaScript Functions

// This file contains trainer-specific logic that can be used across all trainer pages

// Load trainer dashboard data
async function loadDashboardData() {
    try {
        const data = await getHLVDashboard();
        if (data.success) {
            return data.data;
        }
        return null;
    } catch (error) {
        console.error('Error loading dashboard data:', error);
        return null;
    }
}

// Load trainer profile
async function loadTrainerProfile() {
    try {
        const data = await getHLVProfile();
        if (data.success) {
            return data.data;
        }
        return null;
    } catch (error) {
        console.error('Error loading trainer profile:', error);
        return null;
    }
}

// Load trainer schedule
async function loadTrainerSchedule() {
    try {
        const data = await getHLVSchedule();
        if (data.success) {
            return data.data;
        }
        return null;
    } catch (error) {
        console.error('Error loading trainer schedule:', error);
        return null;
    }
}

// Load trainer students
async function loadTrainerStudents(classId = null) {
    try {
        const data = await getHLVStudents(classId);
        if (data.success) {
            return data.data;
        }
        return null;
    } catch (error) {
        console.error('Error loading trainer students:', error);
        return null;
    }
}

// Load attendance data
async function loadAttendanceData(classId, date) {
    try {
        const data = await getAttendance(classId, date);
        if (data.success) {
            return data.data;
        }
        return null;
    } catch (error) {
        console.error('Error loading attendance data:', error);
        return null;
    }
}

// Save attendance data
async function saveAttendanceData(classId, date, records) {
    try {
        const data = await saveAttendance({
            maLop: classId,
            ngayDay: date,
            chiTiet: records
        });
        return data.success;
    } catch (error) {
        console.error('Error saving attendance data:', error);
        return false;
    }
}

// Load exams
async function loadExams() {
    try {
        const data = await getExams();
        if (data.success) {
            return data.data;
        }
        return null;
    } catch (error) {
        console.error('Error loading exams:', error);
        return null;
    }
}

// Save exam results
async function saveExamResultsData(examId, results) {
    try {
        const data = await saveExamResults({
            maKyThi: examId,
            ketQua: results
        });
        return data.success;
    } catch (error) {
        console.error('Error saving exam results:', error);
        return false;
    }
}

// Load notifications
async function loadTrainerNotifications() {
    try {
        const data = await getNotifications();
        if (data.success) {
            return data.data;
        }
        return null;
    } catch (error) {
        console.error('Error loading notifications:', error);
        return null;
    }
}

// Update trainer profile
async function updateTrainerProfile(profileData) {
    try {
        const data = await updateHLVProfile(profileData);
        return data.success;
    } catch (error) {
        console.error('Error updating trainer profile:', error);
        return false;
    }
}

// Change password
async function changeTrainerPassword(passwordData) {
    try {
        const data = await changePassword(passwordData);
        return data.success;
    } catch (error) {
        console.error('Error changing password:', error);
        return false;
    }
}

// Utility function to get current trainer info from localStorage
function getCurrentTrainerInfo() {
    return {
        username: getUsername(),
        role: getRole(),
        token: getToken()
    };
}

// Check if user is trainer
function isTrainer() {
    return hasRole('HuanLuyenVien');
}

// Redirect to login if not authenticated as trainer
function requireTrainerAuth() {
    if (!isTrainer()) {
        window.location.href = '../login.html';
    }
}

// Format attendance status for display
function formatAttendanceStatus(status) {
    const statusMap = {
        'present': 'Có mặt',
        'absent': 'Vắng',
        'late': 'Muộn'
    };
    return statusMap[status] || status;
}

// Get attendance status color class
function getAttendanceStatusClass(status) {
    const classMap = {
        'present': 'text-success',
        'absent': 'text-danger',
        'late': 'text-warning'
    };
    return classMap[status] || '';
}

// Format exam result for display
function formatExamResult(result) {
    const resultMap = {
        'Đạt': '<span class="text-success">Đạt</span>',
        'Không đạt': '<span class="text-danger">Không đạt</span>'
    };
    return resultMap[result] || result;
}

// Calculate attendance rate
function calculateAttendanceRate(present, total) {
    if (total === 0) return 0;
    return Math.round((present / total) * 100);
}

// Get belt color for display
function getBeltColor(belt) {
    const colorMap = {
        'Trắng': '#FFFFFF',
        'Vàng': '#FFD700',
        'Xanh lá': '#32CD32',
        'Xanh dương': '#1E90FF',
        'Đỏ': '#FF0000',
        'Nâu': '#8B4513',
        'Đen': '#000000'
    };
    return colorMap[belt] || '#CCCCCC';
}

// Validate attendance form
function validateAttendanceForm(records) {
    if (!records || records.length === 0) {
        return { valid: false, message: 'Vui lòng điểm danh cho ít nhất một học viên' };
    }
    
    const hasStatus = records.some(r => r.trangThai);
    if (!hasStatus) {
        return { valid: false, message: 'Vui lòng chọn trạng thái điểm danh' };
    }
    
    return { valid: true };
}

// Validate exam results form
function validateExamResultsForm(results) {
    if (!results || results.length === 0) {
        return { valid: false, message: 'Vui lòng nhập kết quả cho ít nhất một học viên' };
    }
    
    const hasResult = results.some(r => r.ketQua);
    if (!hasResult) {
        return { valid: false, message: 'Vui lòng chọn kết quả thi' };
    }
    
    return { valid: true };
}

// Export data to CSV (for attendance or exam results)
function exportDataToCSV(data, filename) {
    if (!data || data.length === 0) {
        showToast('Không có dữ liệu để xuất', 'warning');
        return;
    }
    
    const headers = Object.keys(data[0]);
    const csvContent = [
        headers.join(','),
        ...data.map(row => headers.map(header => `"${row[header] || ''}"`).join(','))
    ].join('\n');
    
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    
    link.setAttribute('href', url);
    link.setAttribute('download', filename);
    link.style.visibility = 'hidden';
    
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    showToast('Đã xuất dữ liệu thành công', 'success');
}

// Print attendance sheet
function printAttendanceSheet(classId, date) {
    const printContent = document.getElementById('attendanceSheet').innerHTML;
    const printWindow = window.open('', '', 'height=600,width=800');
    
    printWindow.document.write('<html><head><title>Bảng điểm danh</title>');
    printWindow.document.write('<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css">');
    printWindow.document.write('<style>body { padding: 20px; } table { width: 100%; border-collapse: collapse; } th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }</style>');
    printWindow.document.write('</head><body>');
    printWindow.document.write(`<h3>Bảng điểm danh - Lớp: ${classId} - Ngày: ${date}</h3>`);
    printWindow.document.write(printContent);
    printWindow.document.write('</body></html>');
    
    printWindow.document.close();
    printWindow.print();
}

// Show confirmation dialog before critical actions
function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

// Auto-refresh data at specified interval
function setupAutoRefresh(callback, intervalMinutes = 5) {
    const intervalMs = intervalMinutes * 60 * 1000;
    return setInterval(callback, intervalMs);
}

// Clear auto-refresh interval
function clearAutoRefresh(intervalId) {
    if (intervalId) {
        clearInterval(intervalId);
    }
}

// Initialize trainer-specific features
function initTrainerFeatures() {
    requireTrainerAuth();
    
    // Set up auto-refresh for notifications every 5 minutes
    const refreshInterval = setupAutoRefresh(async () => {
        // Refresh notifications in background
        const notifications = await loadTrainerNotifications();
        if (notifications) {
            updateNotificationBadge(notifications.length);
        }
    }, 5);
    
    return refreshInterval;
}

// Update notification badge
function updateNotificationBadge(count) {
    const badge = document.getElementById('notifCount');
    if (badge) {
        badge.textContent = count;
    }
}

// Clean up on page unload
window.addEventListener('beforeunload', function() {
    // Clear any intervals or perform cleanup
});

// Make functions available globally
window.loadDashboardData = loadDashboardData;
window.loadTrainerProfile = loadTrainerProfile;
window.loadTrainerSchedule = loadTrainerSchedule;
window.loadTrainerStudents = loadTrainerStudents;
window.loadAttendanceData = loadAttendanceData;
window.saveAttendanceData = saveAttendanceData;
window.loadExams = loadExams;
window.saveExamResultsData = saveExamResultsData;
window.loadTrainerNotifications = loadTrainerNotifications;
window.updateTrainerProfile = updateTrainerProfile;
window.changeTrainerPassword = changeTrainerPassword;
