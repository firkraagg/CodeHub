window.themeHelper = {
    getTheme: function () {
        return localStorage.getItem('theme') || 'light';
    },
    setTheme: function (theme) {
        localStorage.setItem('theme', theme);
        document.body.classList.remove('light', 'dark');
        document.body.classList.add(theme);
    }
};