$(document).ready(function () {
    // 行動版選單切換
    $(document).on('click', '#mobileMenuBtn', function () {
        const sidebar = $('#sidebar');
        const overlay = $('#sidebarOverlay');

        if (window.innerWidth <= 768) {
            sidebar.toggleClass('show');
            overlay.toggleClass('show');
        }
    });
    // 手機版關閉按鈕
    $(document).on('click', '#mobileCloseBtn', function () {
        closeMobileMenu();
    });

    // 點擊遮罩關閉選單
    $(document).on('click', '#sidebarOverlay', function () {
        closeMobileMenu();
    });

    // 點擊選單項目後關閉選單（手機版）
    $(document).on('click', '.nav-submenu .nav-link', function () {
        if (window.innerWidth <= 768) {
            setTimeout(closeMobileMenu, 300);
        }
    });

    // 阻止側邊欄內容點擊時關閉選單
    //$('#sidebar').on('click', function (e) {
    //    e.stopPropagation();
    //});

    // 側邊欄內容點擊不關閉
    $(document).on('click', '#sidebar', function (e) {
        e.stopPropagation();
    });


    // 視窗大小改變處理
    $(window).on('resize', function () {
        if (window.innerWidth > 768) {
            closeMobileMenu();
        }
    });

    // ESC鍵關閉選單
    $(document).on('keydown', function (e) {
        if (e.key === 'Escape' && window.innerWidth <= 768) {
            closeMobileMenu();
        }
    });

    // 關閉手機版選單的統一函數
    function closeMobileMenu() {
        $('#sidebar').removeClass('show');
        $('#sidebarOverlay').removeClass('show');
    }

    // 側邊欄選單項目點擊處理
    $('.nav-link').on('click', function (e) {
        if ($(this).hasClass('nav-link') && !$(this).attr('data-bs-toggle')) {
            $('.nav-submenu .nav-link').removeClass('active');
            $(this).addClass('active');
        }
    });
});
