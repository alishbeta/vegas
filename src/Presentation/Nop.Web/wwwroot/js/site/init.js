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

    if ($(window).width() >= 769) {
        if ($('.container-fluid').hasClass('product')) {
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

    setTimeout(function () {
        if ($(window).width() >= 575) {
            $(".scrollComtent").smoothDivScroll({
                hotSpotScrolling: true,
                touchScrolling: true
            });
        }

        if ($(window).width() >= 769) {
            $(".scrollM").smoothDivScroll({
                hotSpotScrolling: false,
                touchScrolling: true
            });
        }else{
            $('.scrollM .scrollableArea').slick({
                dots: false,
                slidesToShow: 1,
                slidesToScroll: 1,
                variableWidth: true,
                verticalSwiping:false,
            });
        }

    }, 200);

    /*#####################    Отображение подменю   ###############################*/

    /*    var colors = ['#d6eaf1', '#71b1ce', '#c2e0ea', '#b8dbe7', '#aed6e3', '#a3d1df', '#99cbdc',
            '#90c6d9', '#85c1d5', '#7bbcd1', '#71b1ce'];
        var i = 0;
        $('#main-menu>li').each(function () {
            $(this).data('color', colors[i]);
            console.log(i);
            i++;
        });*/

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


    $('.filters-wrap .btn-display').on('click', function () {
        $('.filters-data, .f1').toggleClass('open');
        $('.prod-wrap').toggleClass('short');
    });

    $('button.vg-filter').on('click', function () {
        $(this).toggleClass('open').next().on('click', function () {
            $(this).prev().removeClass('open');
        });
    });

    $(document).on('click', '.dropdown-menu', function (e) {
        $(this).hasClass('keep_open') && e.stopPropagation(); // This replace if conditional.
    });

    $('#clr-filters').on('click', function () {
        $('.filters-data form')[0].reset();
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
    $('.item-wrap-full button:not(.arived), .item-wrap button:not(.arived), .product .price-wrap .btn-blue').on('click', function () {
        $('.cart-popup-wrap').addClass('open');
        $('.back-shadow').fadeIn();
    });

    /*#####################    Табы   ###############################*/
    tabControl();

    var resizeTimer;
    $(window).on('resize', function(e) {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function() {
            tabControl();
        }, 250);
    });

    function tabControl() {
        var tabs = $('.tabbed-content').find('.tabs');
        if(tabs.is(':visible')) {
            tabs.find('a').on('click', function(event) {
                event.preventDefault();
                var target = $(this).attr('href'),
                    tabs = $(this).parents('.tabs'),
                    buttons = tabs.find('a'),
                    item = tabs.parents('.tabbed-content').find('.item');
                buttons.removeClass('active');
                item.removeClass('active');
                $(this).addClass('active');
                $(target).addClass('active');
            });
        } else {
            $('.item').on('click', function() {
                var container = $(this).parents('.tabbed-content'),
                    currId = $(this).attr('id'),
                    items = container.find('.item');
                container.find('.tabs a').removeClass('active');
                items.removeClass('active');
                $(this).addClass('active');
                container.find('.tabs a[href$="#'+ currId +'"]').addClass('active');
            });
        }
    }

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


