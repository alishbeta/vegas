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

    if ($('.wrap').hasClass('product')){
        var navbar =  $('.opt-wrap');
        var wrapper = $('.product-content-wrap');
        $(window).on('scroll',function(){
            var nsc = $(document).scrollTop();
            var bp1 = wrapper.offset().top;
            var bp2 = bp1 + wrapper.outerHeight() - navbar.height();
            var navbPos = navbar.offset().top;

            if (nsc>bp1) {  navbar.addClass('fix-on'); }
            else {navbar.removeClass('fix-on');  }
            if (nsc>bp2) { navbar.removeClass('fix-on'); }

        });
    }


    $('.side-menu-wrap ul li').on('click',function () {
        var color = rgb2hex($(this).css("background-color"));
        $('.side-second-menu-wrap').addClass('open');
        $('.side-menu-wrap').addClass('open');
        var data = $(this);
        $('.side-second-menu-wrap .sub-menu').fadeOut(function () {
            $('.side-second-menu-wrap').css("background-color", color);
            $(this).html("<ul> " +data.find("ul.sub-menu").html() + "</ul>").fadeIn();
        });
        $('.side-second-menu-wrap .title').fadeOut(function () {
            $(this).html(data.find("a .hint").text()).fadeIn();
        });
    });


    $('.side-second-menu-wrap .close-i').on('click', function () {
        $('.side-second-menu-wrap').removeClass('open');
        $('.side-menu-wrap').removeClass('open');
        $('.side-second-menu-wrap .sub-menu').html("");
        $('.side-second-menu-wrap .title').html("");
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

    /*#####################    Корзина   ###############################*/
    $('.cart .minus').on('click', function () {
        var count = +$(this).next().html();
        if (count > 1){
            $(this).next().html(count -= 1);
            $(this).next().next().val(count -= 1);
        }
    });
    $('.cart .plus').on('click', function () {
        var count = +$(this).prev().prev().html();
            $(this).prev().prev().html(count += 1);
            $(this).prev().val(count -= 1);
    });
});
//Function to convert rgb color to hex format
var hexDigits = new Array
("0","1","2","3","4","5","6","7","8","9","a","b","c","d","e","f");

function rgb2hex(rgb) {
    rgb = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    return "#" + hex(rgb[1]) + hex(rgb[2]) + hex(rgb[3]);
}

function hex(x) {
    return isNaN(x) ? "00" : hexDigits[(x - x % 16) / 16] + hexDigits[x % 16];
}


