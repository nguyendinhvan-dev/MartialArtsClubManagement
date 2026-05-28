document.addEventListener('DOMContentLoaded', () => {
    // Check authorization first - allow Admin, QuanTri, or QuanTriVien roles
    if (!checkAuth(['Admin', 'QuanTri', 'QuanTriVien'])) {
        return;
    }

    // Load dashboard stats if we are on the dashboard
    if (window.location.pathname.includes('/admin/dashboard.html')) {
        loadDashboardStats();
    }
});

async function loadDashboardStats() {
    try {
        const result = await apiCall('/admin/dashboard/stats', 'GET');
        
        if (result && result.success && result.data) {
            const stats = result.data;
            
            // Find stat elements (assuming they are in order: HocVien, HLV, Lop, DoanhThu)
            const statElements = document.querySelectorAll('.stat-num');
            if (statElements.length >= 4) {
                // Animate or set text
                statElements[0].textContent = stats.tongHocVien || stats.TongHocVien;
                statElements[1].textContent = stats.tongHuanLuyenVien || stats.TongHuanLuyenVien;
                statElements[2].textContent = stats.tongLopHoc || stats.TongLopHoc;
                
                let revenue = stats.doanhThuThangNay || stats.DoanhThuThangNay || 0;
                let revenueText = revenue.toLocaleString('vi-VN') + ' đ';
                if (revenue >= 1000000) {
                    revenueText = (revenue / 1000000).toFixed(1) + 'M';
                }
                statElements[3].textContent = revenueText;
            }
        } else {
            console.error('Failed to load dashboard stats:', result?.error || 'Unknown error');
        }
    } catch (error) {
        console.error('Error loading stats:', error);
    }
}
