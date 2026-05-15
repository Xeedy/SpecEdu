(function () {
    'use strict';

    var mobileQuery = window.matchMedia('(max-width: 767.98px)');
    var reducedMotionQuery = window.matchMedia('(prefers-reduced-motion: reduce)');

    function revealAll() {
        var nodes = document.querySelectorAll('.fade-up, .fade-in, .fade-up-stagger');
        for (var i = 0; i < nodes.length; i++) {
            nodes[i].classList.add('is-visible');
        }
    }

    function init() {
        // Skip scroll-triggered reveals on mobile, reduced motion, or older browsers.
        if (mobileQuery.matches || reducedMotionQuery.matches || !('IntersectionObserver' in window)) {
            revealAll();
            return;
        }

        var observer = new IntersectionObserver(function (entries) {
            for (var i = 0; i < entries.length; i++) {
                var entry = entries[i];
                if (entry.isIntersecting) {
                    entry.target.classList.add('is-visible');
                    observer.unobserve(entry.target);
                }
            }
        }, {
            threshold: 0.15,
            rootMargin: '0px 0px -40px 0px'
        });

        var targets = document.querySelectorAll('.fade-up, .fade-in, .fade-up-stagger');
        for (var j = 0; j < targets.length; j++) {
            observer.observe(targets[j]);
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
