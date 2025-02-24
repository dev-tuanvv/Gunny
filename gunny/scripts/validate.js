/*
* validate information
*/

$(document).ready(function(){
	//global vars
	var form = $("#frmLogin");
	var formchooseserver = $("#frmChooseServer");
	var name = $("#username");
	var nameInfo = $("#usernameInfo");
	var pass = $("#password");
	var passInfo = $("#passwordInfo");
	var server = $("#server");
	var serverinfo = $("#serverinfo");
	
	//On blur
	//name.blur(validateName);
	name.blur(validateName2);
	pass.blur(validatePass1);

	//On key press
	//name.keyup(validateName);
	name.keyup(validateName2);
	pass.keyup(validatePass1);
	
	//On Submitting
	form.submit(function(){
		if(validateName2() & validatePass1() )
			return true;
		else
			return false;
	});
	/*
	formchooseserver.submit(function(){
									 
			if(validateoption())
			  return true;
			else
			  return false;
			});
	
	function validateoption()
	{
		if(server.val().length < 1)
		{
			serverinfo.addClass("error");
			serverinfo.text("Vui lòng chọn máy chủ");
			return false;
		}
		else
		{
			serverinfo.text("");
			return true;
		}
	}
	/*
	function validateoption()
	{
		var info = ($('input[@name=server]:checked').val());
		
	 	 if(!info)
	 	 {
		 	serverinfo.addClass("error");
			serverinfo.text("Vui lòng chọn máy chủ");
			//nameInfo.addClass("error");
			return false;
		}
		else
		{
			
			serverinfo.text("");
			//nameInfo.addClass("error");
			return true;
		}
	  
	}
	*/
	function validateName(){
		//if it's NOT valid
		if(name.val().length < 6 || name.val().length >16){
			name.addClass("error");
			nameInfo.text("Tên đăng nhập phải từ 6-16 ký tự!");
			//nameInfo.addClass("error");
			return false;
		}
		else
		{
			//name.removeClass("error");
			nameInfo.text("");	
			return true;
		}
		return true;
	}
	function validateName2()
	{
		
			var a = $("#username").val();
			var filter = /^([aA-zZ0-9]){4,32}$/;
		//if it's valid email
			if(filter.test(a)){
			//	name.removeClass("error");
				nameInfo.text("");
			//	nameInfo.removeClass("error");
				return true;
			}
		//if it's NOT valid
			else{
				//name.addClass("error");
				nameInfo.text("Tên đăng nhập không đúng định dạng!");
				//nameInfo.addClass("error");
				return false;
			}
	}
	function validatePass1(){
		var a = $("#password");
	

		//it's NOT valid
		if(pass.val().length <4){
			//pass.addClass("error");
			passInfo.text("Mật khẩu phải từ 4 ký tự");
			//passInfo.addClass("error");
			return false;
		}
		else
		{
		//	pass.removeClass("error");
			passInfo.text("");	
			return true;
		}
		//it's valid
		return true;
	}
	
});