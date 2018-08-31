//浏览器判断是否低过IE9 
var navigatorType = navigator.appName;

if (navigatorType == "Microsoft Internet Explorer"){
    var navigatorVersion = navigator.appVersion .split(";")[1].replace(/[ ]/g,"");
    if( navigatorVersion == "MSIE6.0" || navigatorVersion == "MSIE7.0" || navigatorVersion == "MSIE8.0" ){
        location.href = "/manager/company/obsolete/obsolete";
    }
}
function cp_downdom(obj){
    // console.log(1)
    var downdom = $(".down_trigger");
    var downswitch = downdom.length;

    if ( downswitch != 0 ){
        downdom.hover(function(event){
            downdom.find(".down_box").hide();
            $(this).find(".downswitch").addClass("hover");
            $(this).find(".down_box").show();

        },function(){
            downdom.find(".downswitch").removeClass("hover");
            downdom.find(".down_box").hide();
        }) 
    }
    //通用表格 进度条大于30%切换样式
    var leng = $(".Listevaluation .table .progress").length;
    for ( var i=0; i<leng; i++ ){
        var progress = $(".Listevaluation .table .progress").eq(i);
        var barw = progress.find(".progress-bar").width();
        if ( barw < progress.width()*0.3 ) {
            
            progress.find(".progress-bar").find("span").css({ "position": "absolute", "top": "0px", "left": barw+3, "color": "#777" })
        }
    }
    
    //提示信息
    $('[data-toggle="popover"]').popover({html : true});
    $('.icon-a, .progress, .btn-down').tooltip(true);
    
}

window.onload = function(){
    
    //头部用户信息下拉
    $(".d-userInfo").hover(function(){
        $(this).find($(".dataBox-dropdown")).show();
    },function(){
        $(this).find($(".dataBox-dropdown")).hide();
    })
    
    //首页列表自适应
    var layout_header = $("#header").outerHeight();
    var layout_side = $("#side-menu").outerHeight();
    var layout_main = $("#layoutmain").outerHeight();
    
    var domH = $(window).outerHeight();

    if ( layout_main < domH - layout_header ) {
        $("#layoutmain").css("min-height",(domH - layout_header));
        layout_main = $("#layoutmain").outerHeight();
    }
    if ( layout_side < layout_main ) {
        $("#side-menu").css("min-height",layout_main)
        $("#layoutmain").css("min-height",layout_main)
    } else {
        $("#layoutmain").css("min-height",layout_side)
    }
 
    
 
 
    //部门树自适应
    if( $(".departent_tree").length ) {
        if ( $(".departent_tree").outerHeight() < $(".list-position-block").outerHeight() ){
            $(".departent_tree").css("min-height",$(".list-position-block").outerHeight())
        }
    }
    
    
    //表格下拉
    cp_downdom(); 
    
    
    
    
}



