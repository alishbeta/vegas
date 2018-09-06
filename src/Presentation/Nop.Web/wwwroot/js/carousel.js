/*//Обработка клика на стрелку вправо
$(document).on('click', ".carousel-button-right",function(){
    var carusel = $(this).parents('.carousel');
    right_carusel(carusel);
    return false;
});
//Обработка клика на стрелку влево
$(document).on('click',".carousel-button-left",function(){
    var carusel = $(this).parents('.carousel');
    left_carusel(carusel);
    return false;
});*/
$(document).ready(function () {

    $.fn.loopingAnimation = function(props, dur, eas)
    {
        if (this.data('loop') == true)
        {
            this.animate( props, dur, eas, function() {
                if( $(this).data('loop') == true ) $(this).loopingAnimation(props, dur, eas);
            });
        }
        return this; // Don't break the chain
    }

    $(".carousel-button-left").hover(function(){

        var wrap = $(this).next().next().children();
        console.log(wrap);
        $(wrap).data('loop', true).stop().loopingAnimation({ left: "-10px"}, 300);
    }, function(){
        $(this).next().next().children().data('loop', false);
        // Now our animation will stop after fully completing its last cycle
    });
});
function left_carusel(carusel){
    var block_width = $(carusel).find('.outer-wrapp').outerWidth();
    $(carusel).find(".carousel-items .outer-wrapp").eq(-1).clone().prependTo($(carusel).find(".carousel-items"));
    $(carusel).find(".carousel-items").css({"left":"-"+block_width+"px"});
    $(carusel).find(".carousel-items .outer-wrapp").eq(-1).remove();
    $(carusel).find(".carousel-items").animate({left: "0px"}, 200);

}
function right_carusel(carusel){
    var block_width = $(carusel).find('.outer-wrapp').outerWidth();
    console.log(block_width);
    $(carusel).find(".carousel-items").animate({left: "-"+ block_width +"px"}, 200, function(){
        //$(carusel).find(".carousel-items .outer-wrapp").eq(0).clone().appendTo($(carusel).find(".carousel-items"));
        //$(carusel).find(".carousel-items .outer-wrapp").eq(0).remove();
        //$(carusel).find(".carousel-items").css({"left":"0px"});
    });

}