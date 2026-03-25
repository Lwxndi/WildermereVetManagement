document.addEventListener("DOMContentLoaded", function() {
    const carousel = document.querySelector(".carousel-cards");
    const prevBtn = document.querySelector(".prev-btn");
    const nextBtn = document.querySelector(".next-btn");

    const cardWidth = carousel.querySelector(".animal-card").offsetWidth + 30; // card width + gap

    prevBtn.addEventListener("click", () => {
        carousel.scrollBy({
            left: -cardWidth,
            behavior: "smooth"
        });
    });

    nextBtn.addEventListener("click", () => {
        carousel.scrollBy({
            left: cardWidth,
            behavior: "smooth"
        });
    });

    // Optional: Hide arrows if scroll start/end reached
    function updateArrows() {
        prevBtn.style.display = carousel.scrollLeft <= 0 ? "none" : "flex";
        nextBtn.style.display = carousel.scrollLeft >= carousel.scrollWidth - carousel.clientWidth ? "none" : "flex";
    }

    carousel.addEventListener("scroll", updateArrows);
    updateArrows();
});
