// Cookie consent
function setCookieConsent(level) {
    document.cookie = "cookie_consent=" + level + ";path=/;max-age=31536000;SameSite=Lax";
    var banner = document.getElementById('cookieConsent');
    if (banner) banner.style.display = 'none';
}

// Sidebar toggle for mobile
document.addEventListener('DOMContentLoaded', function () {
    var sidebar = document.getElementById('sidebar');
    var overlay = document.getElementById('sidebarOverlay');
    var toggle = document.getElementById('sidebarToggle');
    var close = document.getElementById('sidebarClose');

    function openSidebar() {
        if (sidebar) sidebar.classList.add('show');
        if (overlay) overlay.classList.add('show');
    }

    function closeSidebar() {
        if (sidebar) sidebar.classList.remove('show');
        if (overlay) overlay.classList.remove('show');
    }

    if (toggle) toggle.addEventListener('click', openSidebar);
    if (close) close.addEventListener('click', closeSidebar);
    if (overlay) overlay.addEventListener('click', closeSidebar);

    // Show cookie consent if not yet accepted
    if (!document.cookie.split(';').some(function (c) { return c.trim().startsWith('cookie_consent='); })) {
        var banner = document.getElementById('cookieConsent');
        if (banner) banner.style.display = 'flex';
    }
});
