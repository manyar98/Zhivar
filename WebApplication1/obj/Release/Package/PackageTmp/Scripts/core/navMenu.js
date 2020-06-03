function loadMenuAccordion() {
    var dropdown = document.querySelectorAll('.dropdown');
    var dropdownArray = Array.prototype.slice.call(dropdown, 0);
    dropdownArray.forEach(function (el) {
        var button = el.querySelector('a[data-toggle="dropdown"]'),
          menu = el.querySelector('.accordion-menu'),
          arrow = button.querySelector('i.icon-arrow');
        button.onclick = function (event) {
            removeClass(arrow, event);
            if (!menu.hasClass('show')) {
                menu.classList.add('show');
                menu.classList.remove('hide');
                arrow.classList.add('open');
                arrow.classList.remove('close');
                event.preventDefault();
            } else {
                menu.classList.remove('show');
                menu.classList.add('hide');
                arrow.classList.remove('open');
                arrow.classList.add('close');
                event.preventDefault();
            }
        };
    })

    function removeClass(arrow, event) {
        dropdownArray.forEach(function (el) {
            var button = el.querySelector('a[data-toggle="dropdown"]');
            var menu = el.querySelector('.accordion-menu');
            menu.classList.add('hide');
            menu.classList.remove('show');
            arrow = button.querySelector('i.icon-arrow');
            arrow.classList.remove('open');
            arrow.classList.add('close');
        })
    }

    Element.prototype.hasClass = function (className) {
        return this.className && new RegExp("(^|\\s)" + className + "(\\s|$)").test(this.className);
    };
}