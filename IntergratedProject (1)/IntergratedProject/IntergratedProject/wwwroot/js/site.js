// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {
    const navbar = document.getElementById("main-navbar");
    window.addEventListener('scroll', function () {
        if (window.pageYOffset > 0) {
            navbar.classList.add("navbar-after-scroll");
        } else {
            navbar.classList.remove("navbar-after-scroll");
        }
    });
});