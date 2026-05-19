function setCookieConsent(level) {
    document.cookie = "cookie_consent=" + level + ";path=/;max-age=31536000;SameSite=Lax";
    document.getElementById('cookieConsent').style.display = 'none';
}
(function () {
    if (!document.cookie.split(';').some(function (c) { return c.trim().startsWith('cookie_consent='); })) {
        document.getElementById('cookieConsent').style.display = 'flex';
    }
})();