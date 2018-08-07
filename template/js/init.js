$(document).ready(function () {
    $(window).on('scroll', function () {
        var top = $(window).scrollTop();
        if (top > 400)
            $('.top-wraper').fadeIn();
        if (top < 400)
            $('.top-wraper').fadeOut();
    });
    $('.top-wraper a').on('click', function () {
        $('html, body').animate({scrollTop: $('head').position().top}, 2000);
    });


    var navbar =  $('.opt-wrap');  // navigation block
    var wrapper = $('.product-content-wrap');        // may be: navbar.parent();

    $(window).on('scroll',function(){
        var nsc = $(document).scrollTop();
        var bp1 = wrapper.offset().top;
        var bp2 = bp1 + wrapper.outerHeight() - navbar.height();
        var navbPos = navbar.offset().top;

        if (nsc>bp1) {  navbar.addClass('fix-on'); }
        else {navbar.removeClass('fix-on');  }
        if (nsc>bp2) { navbar.removeClass('fix-on'); }

    });
    $('.side-second-menu-wrap').on('click',function () {

    });
    /*range slider*/
    $('.f-slider').each(function () {
        var data = $(this).data()
        $("#slider-"+data.filterNumber +"-r").slider({
            range: true,
            min: data.min,
            max: data.max,
            values: [data.min, data.max],
            slide: function (event, ui) {
                $("#slider-"+data.filterNumber+"-a1").val(ui.values[0]);
                $("#slider-"+data.filterNumber+"-a2").val(ui.values[1]);
            }
        });
        $("#slider-"+data.filterNumber+"-a1").val($("#slider-"+data.filterNumber +"-r").slider("values", 0));
        $("#slider-"+data.filterNumber+"-a2").val($("#slider-"+data.filterNumber +"-r").slider("values", 1));
    });

    $('.filters-wrap .vg-filter').on('click', function () {
        $('.filters-data').toggleClass('open');
        $('.f1').toggleClass('open');
        $('.prod-wrap').toggleClass('short');

    });
    $('.slider').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: false,
        fade: true,
        asNavFor: '.slider-nav'
    });
    $('.slider-nav').slick({
        slidesToShow: 3,
        slidesToScroll: 1,
        asNavFor: '.slider',
        dots: false,
        centerMode: false,
        focusOnSelect: true,
        vertical:true,
        verticalSwiping:true,
        prevArrow:'<div class="prev-arrow"></div>',
        nextArrow:'<div class="next-arrow"></div>',
    });
});


