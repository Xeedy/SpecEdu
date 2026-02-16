document.addEventListener("visibilitychange", function () {
    const el = document.getElementById("specHero");
    if (!el || !window.bootstrap) return;

    const carousel = bootstrap.Carousel.getOrCreateInstance(el);

    if (document.hidden) carousel.pause();
    else carousel.cycle();
});