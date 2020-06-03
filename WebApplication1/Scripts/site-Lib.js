/**************************************** Menu ***************************************************/
$(function () {

    $('.navbar-collapse.collapse ul.navbar-nav').on('click', 'li.dropdown a', function (v) {
        var e = $(this);
        if (e.parents('.collapse.in').length == 1 && !e.hasClass('dropdown-toggle')) {
            e.parents('.collapse.in').removeClass('in');
        }
    });

    $('.navbar-collapse.collapse > ul.navbar-nav').on('click', 'li.dropdown > ul > li.dropdown-submenu a', function (v) {
        var e = $(this).closest('li.dropdown-submenu');
        if (e.length == 1) {
            if (!e.hasClass('open-submenu')) {
                if (e.siblings('.open-submenu').length == 1)
                    e.siblings('.open-submenu').first().removeClass('open-submenu');

                e.addClass('open-submenu');
                e.closest('ul.dropdown-menu').css('display', 'block');
            }
            //else if (e.hasClass('open-submenu')) {
            //    e.removeClass('open-submenu');
            //    e.closest('ul.dropdown-menu').attr('style', '');
            //}
        }
    });

    $('.navbar-collapse.collapse > ul.navbar-nav').on('click', 'li.dropdown > ul > li.dropdown-submenu > ul > li > a', function (v) {
        var e = $(this).closest('li.open-submenu');
        if (e.length == 1)
            e.removeClass('open-submenu').removeClass('open');

        var eParent = e.closest('ul.dropdown-menu');
        if (eParent.length == 1)
            eParent.attr('style', '');
    });

    $('.navbar-collapse.collapse > ul.navbar-nav').on('click', 'li.dropdown > ul > li.dropdown-submenu > ul > li.dropdown-submenu > ul > li > a', function (v) {
        var e = $(this).closest('li.open-submenu');
        if (e.length == 1)
            e.removeClass('open-submenu').removeClass('open');

        var eParent = e.closest('ul.dropdown-menu');
        if (eParent.length == 1)
            eParent.attr('style', '');

        e = eParent.closest('li.open-submenu');
        if (e.length == 1)
            e.removeClass('open-submenu').removeClass('open');

        eParent = e.closest('ul.dropdown-menu');
        if (eParent.length == 1)
            eParent.attr('style', '');
    });

    $('body').click(function (event) {
        var e = $(event.target);

        if (e.parents("ul.navbar-nav >li.dropdown > ul > li.dropdown-submenu").length == 0) {
            var allLi = $('ul.navbar-nav li.dropdown-submenu.open-submenu');
            allLi.each(function () {
                var e = $(this);
                e.removeClass('open-submenu');
                e.closest('ul.dropdown-menu').attr('style', '');
            });
        }
    });

})

/****************************************Drawer Menu********************************************/
function ShowDrawerMenu() {
    $('#drawerMenuShadow').show();
    $('#drawerMenuWrapper').css('right', '-250px');
    $('#drawerMenuWrapper').show();
    $('body').css('overflow', 'hidden');
    $('#drawerMenuWrapper').animate({ 'right': '0px' }, 200, function () {
        _drawerMenuIsReady = true;
    });
}
function HideDrawerMenu() {
    $('#drawerMenuWrapper').animate({ 'right': '-250px' }, 200, function () {
        _drawerMenuIsReady = true;
        $('#drawerMenuWrapper').hide();
        $('#drawerMenuShadow').hide();
        $('body').css('overflow', 'auto');
    });
}

function ExpandableDrawerMenuItemClick(e) {
    e = $(e);
    var emenu = e.parent().parent();
    var emenuParent = emenu.parent();
    if (emenu.hasClass('drawer-menu-exp-item-visible')) {
        emenu.removeClass('drawer-menu-exp-item-visible');
        emenu.addClass('drawer-menu-exp-item-hidden');
        var expandIcon = e.find('.drawer-menu-expand-icon > .fa-caret-down');
        expandIcon.removeClass('fa-caret-down');
        expandIcon.addClass('fa-caret-left');
    }
    else {
        emenuParent.find('> .drawer-menu-item').each(function () {
            $(this).removeClass('drawer-menu-exp-item-visible');
            $(this).addClass('drawer-menu-exp-item-hidden');
            var expandIcon = $(this).find('.drawer-menu-expand-icon > .fa-caret-down');
            expandIcon.removeClass('fa-caret-down');
            expandIcon.addClass('fa-caret-left');
        });

        emenu.removeClass('drawer-menu-exp-item-hidden');
        emenu.addClass('drawer-menu-exp-item-visible');
        var expandIcon = e.find('.drawer-menu-expand-icon > .fa-caret-left');
        expandIcon.removeClass('fa-caret-left');
        expandIcon.addClass('fa-caret-down');
    }
}

$(window).on('resize', function () {
    if ($(window).width() >= 768) {
        HideDrawerMenu();
    }
})