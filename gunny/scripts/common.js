// JavaScript Document
jQuery(document).ready(function () {
    jQuery('.Dangky').bind('mouseover', function () {
        jQuery(this).removeClass('Dangky');
        jQuery(this).addClass('Dangky-o');
    })
    jQuery('.Dangky').bind('mouseout', function () {
        jQuery(this).removeClass('Dangky-o');
        jQuery(this).addClass('Dangky');
    })
    jQuery('.quickReg > a').bind('mouseover', function () {
        jQuery(this).removeClass('quick');
        jQuery(this).addClass('quick-o');
    })
    jQuery('.quickReg > a').bind('mouseout', function () {
        jQuery(this).removeClass('quick-o');
        jQuery(this).addClass('quick');
    })
    if (jQuery("#img").length > 0) {
        new FadeGallery(jQuery("#img"), {
            control_event: "mouseover",
            auto_play: true,
            control: jQuery("ul#imgControl"),
            delay: 2
        });
    }
    if (jQuery('p.flashwarning').length > 0) {
        var color = ['black', 'red'];
        var i = 0;

        var change_c = setInterval(function () {
            if (i == color.length) {
                i = 0;
            }
            jQuery('p.flashwarning > a').css('color', color[i])
            i = i + 1;

        }, 500);
    }
});

function checklogin()
	 {
		var err=0;
		if(document.getElementById("username").value == '')
		{
		    $("#lbError").html('Vui lòng nhập tên tài khoản!');
		  err++;
		}
		else if(document.getElementById("password").value == '')
		{
		    $("#lbError").html('Vui lòng nhập mật khẩu!');
		 err++;
		}

		if (err == 0)
		    xmlhttpPost("createLogin.ashx");
			return false;
	 }
