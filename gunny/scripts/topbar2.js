document.write("<div class='tb_div'>");
document.write("<div class='tb_div_bg'>");
document.write("<div class='tb_div_bg_logo'><a href='http://www.hithere.com/' title='HiThere'><img src='http://ddt.hithere.com/public/images/topbar/logo.jpg' alt='HiThere' /></a></div>");
document.write("<div class='tb_div_bg_link'>");
document.write("<ul>");
if(user_info.status == true){
document.write("<li class='tb_div_bg_link_text'>|　<a href='http://www.hithere.com/login.php?do=Logout' title='Logout'>Log out</a> title='Login'>Login</a>　</li>");
}
else{
	document.write("<li class='tb_div_bg_link_text'>|　<a href='http://www.hithere.com/reg.php' title='Sign Up'>Sign Up</a>　|　<a href='http://www.hithere.com/gamelogin.php?g=ddt' title='Login'>Login</a>　</li>");
}
document.write("<li id='button' class='tb_div_bg_link_button'>");
document.write("<div class='tb_div_bg_link_button_icon'><img src='http://ddt.hithere.com/public/images/topbar/icon_ddt.png' /></div>");
document.write("<div class='tb_div_bg_link_button_text'>DDTank</div>");
document.write("</li>");
document.write("</ul>");
document.write("</div>");
document.write("</div>");
document.write("</div>");
document.write("<div id='topbar' class='tb_show'>");
document.write("<div class='tb_show_top'></div>");
document.write("<div class='tb_show_center'>");
document.write("<ul>");
document.write("<li>");
document.write("<a href='http://ddt.hithere.com' title='DDTank (DDT)'><img src='http://ddt.hithere.com/public/images/topbar/game_03.png' alt='DDTank (DDT)' /></a>");
document.write("<a href='http://ddt.hithere.com' title='DDTank (DDT)'>DDTank (DDT)</a>");
document.write("</li>");
document.write("<li>");
document.write("<a href='http://wf.hithere.com' title='WarFlow (WF)'><img src='http://ddt.hithere.com/public/images/topbar/game_01.png' alt='WarFlow (WF)' /></a>");
document.write("<a href='http://wf.hithere.com' title='WarFlow (WF)'>WarFlow (WF)</a>");
document.write("</li>");
document.write("<li>");
document.write("<a href='http://wg.hithere.com' title='WinningGoal (WG)'><img src='http://ddt.hithere.com/public/images/topbar/game_02.png' alt='WinningGoal (WG)' /></a>");
document.write("<a href='http://wg.hithere.com' title='WinningGoal (WG)'>WinningGoal (WG)</a>");
document.write("</li>");
document.write("<li>");
document.write("<a href='http://ec.hithere.com' title='EmpireCraft (EC)'><img src='http://ddt.hithere.com/public/images/topbar/game_04.png' alt='EmpireCraft (EC)' /></a>");
document.write("<a href='http://ec.hithere.com' title='EmpireCraft (EC)'>EmpireCraft (EC)</a>");
document.write("</li>");
document.write("</ul>");
document.write("</div>");
document.write("<div class='tb_show_down'></div>");
document.write("</div>");

//IE6 PNG透明
$(document).pngFix();

var oRect=document.getElementById("button").getBoundingClientRect();
	//判断IE
	if(document.all){
		var left=oRect.left-2+'px';
	}else{ 
		var left=oRect.left+'px';
	}
	$("#button").toggle(function(){
		$(this).addClass("tb_div_bg_link_button_o");
		$("#topbar").css({"left":left});
		$("#topbar").css({"top":"48px"});
		$("#topbar").hide();
		$("#topbar").show(250);
	},function(){
		$(this).removeClass("tb_div_bg_link_button_o");
			$("#topbar").hide(500);
	});