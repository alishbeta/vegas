$(document).ready(function () {

    $(window).on('scroll', function () {
        var top = $(window).scrollTop();
        if (top > 400)
            $('.top-wraper').fadeIn();
        if (top < 400)
            $('.top-wraper').fadeOut();
    });
    $('.top-wraper').on('click', function () {
        $('html, body').animate({
            scrollTop: $('head').position().top
        }, 2000);
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
            } else {
                navbar.removeClass('fix-on');
            }
            if (nsc > bp2) {
                navbar.removeClass('fix-on');
            }

        });
    }

    /*#####################    Карусель  ###############################*/

    if ($(window).width() >= 575) {
        $('ul li.d-sm-none').remove();
        $('.outer-wrapp.scroll').hover(function () {
            console.log();
            if ($(this).find(".item-wrap-full").length) {
                var b = $(this).find(".item-wrap-full"),
                    c = $(this).position(),
                    d = $(this).parents(".scrollWrapper");
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
        if ($(window).width() >= 770) {
            $(".scrollComtent, .scrollM").smoothDivScroll({
                hotSpotScrolling: true,
                touchScrolling: true
            });
        } else {
            $(".scrollM .scrollableArea").slick({
                slidesToShow: 1,
                slidesToScroll: 1,
                responsive: [
                    {
                        breakpoint: 770,
                        settings: {
                            slidesToShow: 3,
                            slidesToScroll: 1,
                        }
                    },
                    {
                        breakpoint: 440,
                        settings: {
                            slidesToShow: 1,
                            slidesToScroll: 1,
                        }
                    },
                ]
            });
        }
    }, 100);

    /*#####################    Мобильно меню  ###############################*/

    $('.humburger-wrap').on('click', function () {
        console.log(5);
        $('.side-menu-wrap, html, .header-wrap, body').toggleClass('open');
        $('.back-shadow').fadeToggle();
        $(this).toggleClass('op');
    });

    if ($(window).width() <= 800) {
        $('#main-menu li').on('click', function () {
            $(this).find('.sub-menu').slideToggle();
        });
    }

    /*#####################    Отображение подменю   ###############################*/

    $('.header-wrap #non-authorized .user_icon').on('click', function () {
        $('.back-shadow, .login-form-wrap').fadeIn();
    });
    $('.login-form-wrap .close-i, .back-shadow').on('click', function () {
        $('.back-shadow, .login-form-wrap').fadeOut();
    });

    if ($(window).width() >= 575) {
        $('.side-menu-wrap ul li').on('click', function () {
            $(this).parent().find('li').each(function () {
                if ($(this).hasClass('active')) {
                    if ($(this).parent().parent().hasClass('sub-menu') == false) {
                        $(this).removeClass('active');
                    }
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
                $(this).html(data.find('span').text()).fadeIn();
            });
        });

        if ($('.side-menu-wrap ul li').hasClass('active')) {
            var color_active = rgb2hex($('.side-menu-wrap ul li.active').css("background-color"));
            $('.side-second-menu-wrap').css("background-color", color_active);
        }
    }


    //Анимация подменю и открытие по клику
    $('.side-second-menu-wrap').on('mouseover', function () {
        $(this).addClass('half-open');
    }).on('mouseleave', function () {
        $(this).removeClass('half-open');
    }).on('click', function (event) {
        if (!!!$(event.target).hasClass('close-i') && $(event.target).hasClass('side-second-menu-wrap')) {
            var data = $(this);
            $('.side-menu-wrap ul li').each(function () {
                if ($(this).hasClass('active')) {
                    var curentItem = $(this);
                    data.addClass('open').find('.sub-menu').fadeOut('fast', function () {
                        $(this).html(curentItem.find('div.sub-menu').html()).fadeIn('fast');
                    });
                    data.find('.title').fadeOut('fast', function () {
                        $(this).html(curentItem.find('span').text()).fadeIn('fast');
                    });
                }
            });
            $('.side-menu-wrap').addClass('open');
        }
    });


    $('.search-wrap').on('click', function () {
        $('.side-menu-wrap').addClass('open');
    });

    // if ($(window).width() >= 768) {
    //     $('.content').on('click', function () {
    //         $('.side-menu-wrap, .side-second-menu-wrap').removeClass('open');
    //     });
    // }

    //Закрываем водменю при клике на крестик.
    $('.side-second-menu-wrap .close-i').on('click', function () {
        $('.side-second-menu-wrap').removeClass('open');
        $('.side-menu-wrap').removeClass('open');
        $('.side-second-menu-wrap .sub-menu').html("");
        $('.side-second-menu-wrap .title').html("");
        $(this).removeClass('active');
    });

    //Скролинг в полной новости.
    $('#similar-button').on('click', function () {
        let pos = $('#similar-destination').offset().top;
        $('html').animate({ scrollTop: pos }, 1000);
    }
    );


    /*#####################    Слайдер для фильтров   ###############################*/
    $('.f-slider').each(function () {
        var data = $(this).data()
        $("#slider-" + data.filterNumber + "-r").slider({
            range: true,
            min: data.min,
            max: parseFloat(data.max),
            values: [data.min, parseFloat(data.max)],
            slide: function (event, ui) {
                $("#slider-" + data.filterNumber + "-a1").val(ui.values[0]).trigger("change");
                $("#slider-" + data.filterNumber + "-a2").val(ui.values[1]);
            }
        });
        $("#slider-" + data.filterNumber + "-a1").val($("#slider-" + data.filterNumber + "-r").slider("values", 0));
        $("#slider-" + data.filterNumber + "-a2").val($("#slider-" + data.filterNumber + "-r").slider("values", 1));
    });
    // $('.filters-data input').on("change", function() {
    //     console.log($(this).val());
    // });
    $('.filters-wrap .btn-display').on('click', function () {
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

    /*#####################    Закрываем фенсибох по тапу   ###############################*/
    $('.fancybox-slide').bind('touchstart', function () {
        $.fancybox.close();
    });

    /*#####################    Корзина (Скрытие pop-up)   ###############################*/
    $('.cart-popup-wrap .close-i, #pop-up-close, .back-shadow').on('click', function () {
        $('.cart-popup-wrap, .side-menu-wrap, .header-wrap, .side-second-menu-wrap').removeClass('open');
        $('.back-shadow').fadeOut();
        $('.humburger-wrap').removeClass('op');
    });

    /*#####################    Корзина   (Отображение popup)###############################*/
    // $('.item-wrap-full button:not(.arived), .item-wrap button:not(.arived), .product .btn-blue:not(.arived), .favorit.shopping_bag_icon').on('click', function () {
    $('.buy-btn').on('click', function () {
        //UpdateCart();
        $('.cart-popup-wrap').addClass('open');
        $('.back-shadow').fadeIn();

    });

    /*#####################   Комментарии отображаем  ###############################*/
    $('#comment-activate').on('click', function () {
        $(this).parent().next().slideToggle();
    });

    $('#left-feedback').on('click', function () {
        $('.form-block').slideToggle();
        $('.line').fadeToggle();
    })

    $('.comment-items .more').on('click', function () {
        $(this).parent().parent().parent().next().slideToggle();
    })

    /*#####################   Диаграмма ###############################*/
    $('.total-diagram .item-line-wrapp').each(function () {
        $(this).find('.item-line').each(function (indx) {
            let persent = $(this).text().split('%')[0].trim();
            if (parseInt(persent) <= 5)
                $(this).text('');
            $(this).animate({ width: persent + '%' });
            if (indx == 0) {
                let left = $(this).next().text().split('%')[0].trim();
                $(this).animate({ left: left + '%' });
            }
        })
    });

    /*#####################   Popover ###############################*/

    $('.item-line-wrapp').on('mouseover', function(){
        let p_arr = $(this).data('persent');
        if (p_arr)
            $('.persent .gren-b>div').text(p_arr[0]);
        
        let data = $('.persent').html();
        $(this).popover({
            trigger: 'manual',
            content: '<div class="persent-block">'+
            '<div class="gren-b"></div>'+
            '<div  class="persent-data">'+ p_arr[0] +'</div>'+
            '<div class="grey-b"></div>'+
            '<div  class="persent-data">'+ p_arr[1] +'</div>'+
            '<div class="red-b"></div>'+
            '<div  class="persent-data">'+ p_arr[2] +'</div>'+
        '</div>',
            html: true,
            placement: 'bottom'
        });

        $(this).popover('show');
    }).on('mouseleave', function(){
        $(this).popover('hide');
    })

    /*#####################   Города  ###############################*/
    $('.city-wrapp li').on('click', function () {
        $('.head-menu .dropdown-menu').removeClass('show');
        let city = $(this).find('a').html();
        $('#your-city').html(city);
        localStorage.City = city;
    });

    $('.confirm-city .btn-blue').on('click', function () {
        $(this).parent().parent().addClass('d-none');
    }).next().on('click', function () {
        $(this).parent().parent().hide();
        $('.head-menu .dropdown-menu').addClass('show');
    });

    /*#####################    Кабинет  (Редактирование личных данных и адреса) ###############################*/
    $('#edit-addres').on('click', function () {
        $(this).hide().next().show();
        $('#secondZone input:not([type="radio"], #old-newpost-department), #secondZone select').attr('disabled', false);
    });
    $('#edit-addres-close').on('click', function () {
        $(this).hide().prev().show();
        $('#secondZone input:not([type="radio"], #old-newpost-department), #secondZone select').attr('disabled', true);
    });

    $('#edit-name').on('click', function () {
        $(this).hide().next().show();
        $('#firstZone input').attr('disabled', false);
    });

    $('#edit-name-close').on('click', function () {
        $(this).hide().prev().show();
        $('#firstZone input').attr('disabled', true);
    });

    $('#edit-pass').on('click', function () {
        $(this).hide().next().show();
        $('#thirdZone input').attr('disabled', false);
    });

    $('#edit-pass-close').on('click', function () {
        $(this).hide().prev().show();
        $('#thirdZone input').attr('disabled', true);
    });

    tabControl();

});
//Function to convert rgb color to hex format
var hexDigits = new Array("0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f");
        
/*#####################   Попап на фильтрах  ###############################*/
        function showPopover(text, btn){
            let elem = $(this).parent().parent().next().popover({
                trigger: 'manual',
                content: '<div class = "filter-title">'+ text +' </div><button onclick = "$(\'#submit-filters\').click()" class = "btn btn-blue popover-submit">'+btn+'</button>',
                html: true,
            });

            elem.popover('show');
              $('.ui-slider-handle').on('mouseenter', function(){
                elem.popover('hide');
              })
              $('.prod-wrap').on('mouseenter', function(){
                elem.popover('hide');
              })
              $('input').on('click', function(){
                elem.popover('hide');
              })
        };

function rgb2hex(rgb) {
    rgb = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/);
    return "#" + hex(rgb[1]) + hex(rgb[2]) + hex(rgb[3]);
}

function hex(x) {
    return isNaN(x) ? "00" : hexDigits[(x - x % 16) / 16] + hexDigits[x % 16];
}

function tabControl() {
    var tabs = $('.tabbed-content').find('.tabs');
    if (tabs.is(':visible')) {
        tabs.find('a').on('click', function (event) {
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
        $('.item').on('click', function () {
            var container = $(this).parents('.tabbed-content'),
                currId = $(this).attr('id'),
                items = container.find('.item');
            container.find('.tabs a').removeClass('active');
            items.removeClass('active');
            $(this).addClass('active');
            container.find('.tabs a[href$="#' + currId + '"]').addClass('active');
        });
    }
}
