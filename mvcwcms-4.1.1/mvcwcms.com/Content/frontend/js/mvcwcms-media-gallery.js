jQuery(document).ready(function ($) {

    if ($(".galleria").length) {
        Galleria.loadTheme("/Content/frontend/js/galleria/themes/classicmod/galleria.classicmod.min.js");
        Galleria.configure({
            transitionSpeed: 2000
        });
        Galleria.run(".galleria");
    }

});