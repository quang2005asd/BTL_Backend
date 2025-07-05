document.addEventListener('DOMContentLoaded', function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
});
function updateNavbarCartCount() {
    $.get('/Cart/GetCartItemCount', function (data) {
        $('.cart-count').text(data.count);
    });
}

$(document).ready(function () {
    // Lần đầu load
    requestAnimationFrame(updateNavbarCartCount);

    // Khi sự kiện cartUpdated được trigger (sau khi thêm món thành công)
    $(window).on('cartUpdated', function () {
        requestAnimationFrame(updateNavbarCartCount);
    });
});
