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


    if ($('.wrap').hasClass('product')) {
        var navbar = $('.opt-wrap');
        var wrapper = $('.product-content-wrap');
        $(window).on('scroll', function () {
            var nsc = $(document).scrollTop();
            var bp1 = wrapper.offset().top;
            var bp2 = bp1 + wrapper.outerHeight() - navbar.height();
            var navbPos = navbar.offset().top;

            if (nsc > bp1) {
                navbar.addClass('fix-on');
            }
            else {
                navbar.removeClass('fix-on');
            }
            if (nsc > bp2) {
                navbar.removeClass('fix-on');
            }

        });
    }


    /*#####################    Карусель  ###############################*/
    $('.slick-slider .item-wrap').on('mouseover', function () {
        $('.slick-slider .item-wrap-full').fadeOut(250);
        $(this).parent().parent().css('position', 'static');
        $(this).next().fadeIn(250);
/*        $(this).next().on('mousedown', function () {
            $(this).prev().off('mouseover');
            $(this).fadeOut(250, function () {
                $(this).parent().parent().css('position', 'relative');
            });

        });*/
        $(this).next().on('mouseleave',function(){
            $(this).fadeOut(0);
        });
/*        $(this).on('mouseup', function () {
            $(this).on('mouseover', function () {
                $(this).parent().parent().css('position', 'static');
                $(this).next().fadeIn(250);
            });
        });*/
    });
    $('.slider-wrap').on('beforeChange', function(event, slick, currentSlide, nextSlide){
        $(this).next().off('mouseover');
    });
    $('.slider-wrap').on('afterChange', function(event, slick, currentSlide, nextSlide){
        $('.slick-slider .item-wrap').on('mouseover', function () {
            $(this).parent().parent().css('position', 'static');
            $(this).next().fadeIn(250);
        });
    });
    $('.slick-arrow').on('mouseover',function(){
        $('.slick-slider .item-wrap-full').fadeOut(250);
        setTimeout(function () {
            $('.slick-track').css('position', 'relative');
        }, 500);

    });


    /*#####################    Отображение подменю   ###############################*/

/*    var colors = ['#d6eaf1', '#71b1ce', '#c2e0ea', '#b8dbe7', '#aed6e3', '#a3d1df', '#99cbdc',
        '#90c6d9', '#85c1d5', '#7bbcd1', '#71b1ce'];
    var i = 0;
    $('#main-menu>li').each(function () {
        $(this).data('color', colors[i]);
        console.log(i);
        i++;
    });*/


    $('.side-menu-wrap ul li').on('click', function () {

        $(this).parent().find('li').each(function () {
            if($(this).hasClass('active')){
                $(this).removeClass('active');
            }
        });
        var color = rgb2hex($(this).css("background-color"));
        $('.side-second-menu-wrap').addClass('open');
        $('.side-menu-wrap').addClass('open');
        $(this).addClass('active');
        /*var curent_color = $(this).next().data('color');*/
        console.log($(this).next().data());

        var data = $(this);
        $('.side-second-menu-wrap .sub-menu').fadeOut(function () {
            $('.side-second-menu-wrap').css("background-color", color);
            $(this).html(data.find("div.sub-menu").html()).fadeIn(function () {

/*                $('.side-second-menu-wrap .sub-menu li').hover(function () {
                    $(this).css("background-color", curent_color);
                });*/
            });

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
        $(this).removeClass('active');
    });

    /*#####################    Слайдер для фильтров   ###############################*/
    $('.f-slider').each(function () {
        var data = $(this).data()
        $("#slider-" + data.filterNumber + "-r").slider({
            range: true,
            min: data.min,
            max: data.max,
            values: [data.min, data.max],
            slide: function (event, ui) {
                $("#slider-" + data.filterNumber + "-a1").val(ui.values[0]);
                $("#slider-" + data.filterNumber + "-a2").val(ui.values[1]);
            }
        });
        $("#slider-" + data.filterNumber + "-a1").val($("#slider-" + data.filterNumber + "-r").slider("values", 0));
        $("#slider-" + data.filterNumber + "-a2").val($("#slider-" + data.filterNumber + "-r").slider("values", 1));
    });

    $('.filters-wrap .vg-filter').on('click', function () {
        $('.filters-data').toggleClass('open');
        $('.f1').toggleClass('open');
        $('.prod-wrap').toggleClass('short');

    });


    /*#####################    Корзина   ###############################*/
    $('.cart .minus, .cart-popup-wrap .minus').on('click', function () {
        var count = +$(this).next().html();
        if (count > 1) {
            $(this).next().html(count -= 1);
            $(this).next().next().val(count -= 1);
        }
    });
    $('.cart .plus, .cart-popup-wrap .plus').on('click', function () {
        var count = +$(this).prev().prev().html();
        $(this).prev().prev().html(count += 1);
        $(this).prev().val(count -= 1);
    });

    /*#####################    Корзина (Скрытие pop-up)   ###############################*/
    $('.cart-popup-wrap .close-i, #pop-up-close, .back-shadow').on('click', function () {
        $('.cart-popup-wrap').removeClass('open');
        $('.back-shadow').fadeOut();
    });

    /*#####################    Корзина   (Отображение popup)###############################*/
    $('.item-wrap-full button').on('click', function () {
        $('.cart-popup-wrap').addClass('open');
        $('.back-shadow').fadeIn();
    });

    /*#####################    Корзина  (Редактирование личных данных и адреса) ###############################*/
    $('#edit-name, #edit-addres').on('click', function () {
        var el = $(this).next().next().children();
        var el2 = $(this).next().next().next().children();
        $(this).fadeOut(function () {
            $(this).next().fadeIn();
        });
        $(el[2]).fadeToggle(function () {
            $(el[0]).fadeToggle();
            $(el[1]).fadeToggle();
        });
        $(el[3]).fadeToggle();

        $(el2[2]).fadeToggle(function () {
            $(el2[0]).fadeToggle();
            $(el2[1]).fadeToggle();
        });
        $(el2[3]).fadeToggle();

        //$(this).next().$('label').fadeToggle();
    });

    $('#edit-name-close, #edit-addres-close').on('click', function () {
        var el = $(this).next().children();
        var el2 = $(this).next().next().children();
        $(this).fadeOut(function () {
            $(this).prev().fadeIn();
        });
        $(el[0]).fadeToggle(function () {
            $(el[2]).fadeToggle();
            $(el[3]).fadeToggle();
        });
        $(el[1]).fadeToggle()

        $(el2[0]).fadeToggle(function () {
            $(el2[2]).fadeToggle();
            $(el2[3]).fadeToggle();
        });
        $(el2[1]).fadeToggle()

        //$(this).next().$('label').fadeToggle();
    });

});
//Function to convert rgb color to hex format
var hexDigits = new Array
("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f");

function rgb2hex(rgb) {
    rgb = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    return "#" + hex(rgb[1]) + hex(rgb[2]) + hex(rgb[3]);
}

function hex(x) {
    return isNaN(x) ? "00" : hexDigits[(x - x % 16) / 16] + hexDigits[x % 16];
}


