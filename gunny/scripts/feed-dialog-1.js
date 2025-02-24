var zmf = new function(){
	var BASE_URL = "http://feed.me.zing.vn",
		ua = navigator.userAgent.toLowerCase(),
		browser = (ua.match(/(^|\s)(firefox|safari|opera|msie|chrome)[\/:\s]([\d\.]+)/) || ['', '', '', '0.0'])[2];
		
    function ui(options){	
        var uid_to = options.uid_to;
        var object_id = encodeURIComponent(options.object_id);
		var actid = options.action_id;
		// attachment
		if(options.attach_name.length>80){
			options.attach_name = options.attach_name.substring(0, 80);
		}
        var attach_name = encodeURIComponent(options.attach_name);

		if(options.attach_href.length>150){
			options.attach_href = options.attach_href.substring(0, 150);
		}
		var attach_href = encodeURIComponent(options.attach_href);

		if(options.attach_caption.length>30){
			options.attach_caption = options.attach_caption.substring(0, 30);
		}
		var attach_caption = encodeURIComponent(options.attach_caption);

		if(options.attach_des.length>200){
			options.attach_des = options.attach_des.substring(0, 200);
		}
		var attach_des= encodeURIComponent(options.attach_des);

		// media
        var media_type = encodeURIComponent(options.media_type);

		if(options.media_img.length>150){
			options.media_img = options.media_img.substring(0, 150);
		}
        var media_img = encodeURIComponent(options.media_img);

		if(options.media_src.length>150){
			options.media_src = options.media_src.substring(0, 150);
		}
        var media_src = encodeURIComponent(options.media_src);

		//action link
		if(options.actlink_text.length>20){
			options.actlink_text = options.actlink_text.substring(0, 20);
		}
        var actlink_text = encodeURIComponent(options.actlink_text);

		if(options.actlink_href.length>150){
			options.actlink_href = actlink_href.substring(0, 150);
		}
        var actlink_href = encodeURIComponent(options.actlink_href);
		var callback = (options.callback)? options.callback : "" ;
		var suggestion = (options.suggestion)? options.suggestion : "" ;
		var strSug = "";
		if(suggestion!=""){
			var sugLength = suggestion.length;			
			for(i =0; i< sugLength; i++)
			{
				if(i!= sugLength - 1)
					strSug += escape(suggestion[i]) + "|$|";
				else
					strSug += escape(suggestion[i]);
			}
		}

        var tpl_id = options.tpl_id;
		var pubkey = options.pub_key;
		var signkey = options.sign_key;
		var inIframe = (window.top==window)? 0 : 1;
		var domain  = window.location.host;	
		var rand = Math.floor(Math.random() * 1000);
		
		// build url
		var url =  BASE_URL + "/fd3/stream.public?ut=" + uid_to + "&ob=" + object_id +"&ac=" +actid + "&atn=" + attach_name 
				+ "&atc=" + attach_caption + "&atd=" + attach_des + "&ath=" + attach_href + "&amt=" + media_type + "&ami=" + media_img 
				+ "&ams=" + media_src + "&lt=" + actlink_text + "&lh=" + actlink_href + "&tpl=" + tpl_id + "&dm=" + domain 
				+ "&pk=" + pubkey + "&sk=" + signkey + "&inf=" + inIframe + "&cb=" + callback + "&rand=" + rand + "#sug=" + strSug;

        showDialog(url);
    }

	function xFunction(obj){
		
		if (obj.message.indexOf('hide') >= 0) {
			hide(obj);
		}	
		else if (obj.message.indexOf('resize') >= 0) {
			resize(obj);
		}
		else if (obj.message.indexOf('callback') >= 0) {
			callback(obj);
		}
	}
    function showDialog(ifUrl){
		if (!document.getElementById('zmfeedboxy')) {
			var e = document.createElement('iframe');
			e.id = 'zmfeedboxy';
			e.src = ifUrl;
			e.frameBorder = '0';
			e.style.width = '602px';
			e.style.height = '200px';
			e.style.top = '-9999px';
			e.style.left = '0px';
			e.style.zIndex = '999999';
			e.style.position = browser == 'msie' ? 'absolute' : 'fixed';
			document.body.appendChild(e);
        }
        else{
			document.getElementById('zmfeedboxy').src = ifUrl;
		}
    }
	
	function hide(obj){
		var elem = document.getElementById('zmfeedboxy');
		elem.style.top = '-9999px';
		elem.style.left = '0px';
	}
	function resize(obj){
	      
		var frame = document.getElementById('zmfeedboxy');
		var p = obj.message.split('_');
		if (p[1] && p[2]){
			frame.style.height = p[2] + 'px';
		}
		var topWSize, iframeOffset;
		if (p[3] && p[4] && p[5] && p[6])
			topWSize = {width: p[3], height: p[4], offsetX: p[5], offsetY: p[6]};
		if (p[7] && p[8])
			iframeOffset = {pageXOffset: p[7], pageYOffset: p[8]};
		center(topWSize, iframeOffset);
	}
	function callback(obj){
		var p = obj.message.split('_');
		if (p[1] && p[2]){
			window[p[1]](p[2]);
		}
	}
	function center(topWSize, iframeOffset) {
		var viewport = topWSize ? topWSize : getViewport(),
			vHeight = viewport.height,
			vWidth = getViewport().width,
			boxy = document.getElementById('zmfeedboxy'),
			width = boxy.clientWidth,
			height = boxy.clientHeight,
			top, left;
		if (browser == 'msie') {
			top = document.body.scrollTop + Math.round( ((vHeight - height < 0) ? 0 : (vHeight - height)) / 2 );
			left = document.body.scrollLeft + Math.round( ((vWidth - width < 0) ? 0 : (vWidth - width)) / 2 );
		} else {
			top = Math.round((vHeight - height) / 2);
			left = Math.round((vWidth - width) / 2);
		}

		top += iframeOffset ? parseInt(iframeOffset.pageYOffset) : 0;
		boxy.style.top = top + 'px';
		boxy.style.left = left + 'px';
	}
	
	function checkBrowser() {
		var ua = navigator.userAgent.toLowerCase();
		return ua.match(/(^|\s)(firefox|safari|opera|msie|chrome)[\/:\s]([\d\.]+)/) || ['', '', '0.0'];
	}
	function getViewport() {
		var result = {}, docElem = document.documentElement, body = document.body;
		if (window.innerWidth) {
			result.width = window.innerWidth;
			result.height = window.innerHeight;
		}
		else {
			result.width = docElem.clientWidth;
			if (result.width == 0) {
				result.width = body.clientWidth;
				result.height = body.clientHeight;
			}
			else
				result.height = docElem.clientHeight;
		}
		if (typeof window.pageYOffset === 'number') {
			result.offsetY = window.pageYOffset;
			result.offsetX = window.pageXOffset;
		}
		else if ( docElem && (docElem.scrollLeft || docElem.scrollTop)) {
			result.offsetY = docElem.scrollTop;
			result.offsetX = docElem.scrollLeft;
		}
		else if (body && (typeof body.scrollLeft === 'number')) {
			result.offsetY = body.scrollTop;
			result.offsetX = body.scrollLeft;
		}
		return result;
	}

	function xRegister(){
		if(typeof(zmXCall) != 'undefined'){
			zmXCall.register("xFunction", xFunction);
		}
		else{
			setTimeout(xRegister, 500);
		}
	}
    this.ui = ui;
	this.xFunction = xFunction;

	if(typeof(zmXCall) != 'undefined'){
		zmXCall.register("xFunction", xFunction);
	}
	else{
		var xScript = document.createElement('script');
		xScript.type = 'text/javascript';
		xScript.src = 'http://static.me.zing.vn/v3/js/zm.xcall-1.00.js';
		document.getElementsByTagName("head")[0].appendChild(xScript);

		setTimeout(xRegister, 500);
	}	
}();
