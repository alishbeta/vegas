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
    /*#####################    Мобильно меню  ###############################*/

    $('.hanburger').on('click', function () {
        $('.side-menu-wrap, html, .header-wrap, body').toggleClass('open');
        $('.back-shadow').fadeToggle();
    });

    /*#####################    Карусель  ###############################*/

    if ($(window).width() >= 575) {
        $('.outer-wrapp.scroll').hover(function () {
            if ($(this).find(".item-wrap-full").length) {
                var b = $(this).find(".item-wrap-full")
                    , c = $(this).position()
                    , d = $(this).parents(".scrollWrapper");
                (c.left >= 0 && c.left + $(this).find(".item-wrap-full").width() - Math.abs($(this).width() - $(this).find(".item-wrap-full").width()) / 2 <= $(this).parents(".scrollWrapper").width() || 0 == $(this).parents(".scrollWrapper").length) && $(this).find(".item-wrap-full").css({
                    left: c.left,
                    top: c.top
                }).stop().fadeIn(750)
            }
        }, function () {
            $(this).find(".item-wrap-full").hide()
        });
        $('.scrollWrapper').on('scroll', function () {
            $(".item-wrap-full").hide();
        });
    }
    /*    $('.outer-wrapp.scroll .item-wrap').on('mouseover', function () {
            $(this).next().fadeIn();


            console.log($(this).position().left);

        }).next().on('mousedown', function () {
            var curentPosition = $(this).position().left;
            console.log(scrollerOffset);
        }).on('mouseup', function () {
            var scrollerOffset = $(".scrollComtent").smoothDivScroll("getScrollerOffset");
            var newPosition = curentPosition
        });*/

    setTimeout(function () {
        if ($(window).width() >= 575) {
            $(".scrollComtent").smoothDivScroll({
                hotSpotScrolling: true,
                touchScrolling: true
            });
        }
    }, 300);

    /*#####################    Отображение подменю   ###############################*/

	$('.header-wrap #non-authorized .user_icon').on('click', function () {
        $('.back-shadow, .login-form-wrap').fadeIn();
    });
    $('.login-form-wrap .close-i').on('click', function () {
        $('.back-shadow, .login-form-wrap').fadeOut();
    });


    $('.side-menu-wrap ul li').on('click', function () {
        $(this).parent().find('li').each(function () {
            if ($(this).hasClass('active')) {
                $(this).removeClass('active');
            }
        });
        var color = rgb2hex($(this).css("background-color"));
        $('.side-second-menu-wrap').addClass('open');
        $('.side-menu-wrap').addClass('open');
        $(this).addClass('active');
        /*var curent_color = $(this).next().data('color');*/
        
        var data = $(this);
        $('.side-second-menu-wrap .sub-menu').fadeOut(function () {
            $('.side-second-menu-wrap').css("background-color", color);
            $(this).html(data.find("div.sub-menu").html()).fadeIn();

        });
        $('.side-second-menu-wrap .title').fadeOut(function () {
            console.log(data.find('span').text());
            $(this).html(data.find('span').text()).fadeIn();
        });
    });

    $('.search-wrap').on('click', function() {
        $('.side-menu-wrap').addClass('open');
    });

    $('.content').on('click', function(){
        $('.side-menu-wrap').removeClass('open');
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

    /*#####################    Личный кабинет   ###############################*/
    $('.cabinet .order-wrap a').on('click', function () {
        $(this).children().toggleClass('open');
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
    $('.item-wrap-full button:not(.arived), .item-wrap button:not(.arived)').on('click', function () {
        //UpdateCart();
        $('.cart-popup-wrap').addClass('open');
		$('.back-shadow').fadeIn();
		
    });

    /*#####################    Корзина  (Редактирование личных данных и адреса) ###############################*/
    $('#edit-name, #edit-addres, #edit-pass').on('click', function () {
        var el = $(this).next().next().children();
        var el2 = $(this).next().next().next().children();
        $(this).fadeOut(function () {
            $(this).next().fadeIn();
        });
        if (!!el[2]) {
            console.log(1);
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
        } else {
            $(el[0]).fadeToggle();
            $(el[1]).fadeToggle();
        }
    });

    $('#edit-name-close, #edit-addres-close, #edit-pass-close').on('click', function () {
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


