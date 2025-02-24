var $SGNToolTip = null;
document.domain = 'zing.vn';
if ($SGNToolTip == null) {
    var head = document.getElementsByTagName("head")[0];
    var body = document.getElementsByTagName("body")[0];
    var tt, currentId;
    var items = [];
    var cacheItems = [];
    var itemDiv;
    var enableCookie = 1;
    var tooltipType = 'none';
    var isLoading = null;
    var unionUrl = 'http://union.zing.vn/';
    var unionCss = 0;
    if(typeof module != 'undefined'){
    //var unionCss = 'http://img.zing.vn/union/clientscripts/css/' + module + '/unionpower.css';
    } else {
        unionCss = 'http://img.zing.vn/union/clientscripts/unionpower/unionpower.css';
    }
    var jqueryUrl = 'http://img.zing.vn/union/clientscripts/js/jquery/jQuery.js';
    var jsonUrl = "http://union.zing.vn/clientscripts/json.js";
    var ajaxLoader = 'http://img.zing.vn/union/clientscripts/image/icn_loading_animated.gif';
    var url = '';
    var domainName = 'union.zing.vn';
    var subDomainName = 'union';

    function createAkzElement(type, p)
    {
        // type = html element type (ie: link, a, p)
        // p = array of attributes for type
        var newelement = document.createElement(type);
        if (p) {
            createAkzObject(newelement, p);
        }
        return newelement;
    }

    function addAkzElement(p, element)
    {
        return p.appendChild(element);
    }

    function addAkzEvent(z, y, x)
    {
        if (window.attachEvent) {
            z.attachEvent("on" + y, x);
        } else {
            z.addEventListener(y, x, false);
        }
    }

    function createAkzObject(ele, s)
    {
        for ( var p in s) {
            if (typeof s[p] == "object") {
                if (!ele[p]) {
                    ele[p] = {};
                }
                createAkzObject(ele[p], s[p]);
            } else {
                ele[p] = s[p];
            }
        }
    }

    function $E(e)
    {

        if (!e) {
            e = event;
        }
        if (!e.button) {
            e._button = e.which ? e.which : e.button;
            e._target = e.target ? e.target : e.srcElement;
        }
        return e;
    }

    function onMouseOver(e)
    {
        e = $E(e);
        var t = e._target;

        if (t.nodeName != "A") {

            if (t.parentNode && t.parentNode.nodeName == "A") {
                t = t.parentNode;
            } else if (t.parentNode.parentNode && t.parentNode.parentNode.nodeName == "A") {
                t = t.parentNode.parentNode;
            } else if(t.attributes.href){
                t.href = t.attributes.href.nodeValue;
            } else if(t.parentNode.attributes && t.parentNode.attributes.href){
                t = t.parentNode;
                t.href = t.attributes.href.nodeValue;
            } else {
                if (t.nodeName == "embed" || t.nodeName == "EMBED" || t.nodeName == "img" || t.nodeName == "IMG") {
                    t = t.parentNode;
                }
                if (t.children.length > 0) {
                    var staticTooltipContent = '';
                    for (i = 0; i < t.children.length; ++i) {
                        if (t.children[i].className == 'tooltip') {
                            staticTooltipContent = t.children[i].innerHTML;
                            break;
                        }
                    }
                    if (staticTooltipContent) {
                        if (!t.onmouseover) {
                            t.onmouseout = onMouseOut;
                            t.onmousemove = onMouseMove;
                        }
                        showToolTip(staticTooltipContent);
                    }
                }
                return;
            }
        }

        var m = [];
        if (t.className)
            m['class'] = t.className;
        var site;
        var v;
        var valid = 0;
        var thref = t.href.replace('%3A', ':');

        var isZAM = 0;
        if (document.URL.match(/^http:\/\/.*\.(zing\.vn|dev.vinagame\.vn)\/.*/)) {
            isZAM = 1;
        }

        if (jQuery(t).attr('ref') || jQuery(t).attr('data-tooltip')) {
            m['ref'] = jQuery(t).attr('ref') || jQuery(t).attr('data-tooltip');
        }
        if (m['ref'] == 'notooltip') {
            return;
        }
        //refer
        if(v = thref.match(/[\?&]ref=([^&]+)/i)){
            m['refer'] = v[1];
        }else{
            m['refer'] = '';
        }
        if (jQuery(t).attr('data-item'))
            m['data-item'] = jQuery(t).attr('data-item');
        if (v = thref.match(/^http:\/\/(.+)(\.zing\.vn)\/(.+)\/(character)\/(display)\/(.+)\/(\d+)\/?/i)) {
            m['host'] = v[1];
            // m['box'] = v[1];
            m['domain'] = v[2];
            m['product'] = v[3];
            m['controller'] = v[4];
            m['action'] = v[5];
            m['qs'] = v[6];
            m['id'] = v[7];
            valid = 1;
        } else if (v = thref.match(/^http:\/\/(.+)(\.zing\.vn)\/(.+)\/(mapinstance)\/(display)\/(.+)\/(\d+)\/?/i)) {
            m['host'] = v[1];
            // m['box'] = v[1];
            m['domain'] = v[2];
            m['product'] = v[3];
            m['controller'] = v[4];
            m['action'] = v[5];
            m['qs'] = v[6];
            m['id'] = v[7];
            valid = 1;
        } else if (v = thref.match(/^http:\/\/(.+)(\.zing\.vn)\/(.+)\/(.+)\/(.+)\/(.+)\/(.\d+)[\?#]?(.*)/i)) {
            m['host'] = v[1];
            // m['box'] = v[1];
            m['domain'] = v[2];
            m['product'] = v[3];
            m['controller'] = v[4];
            m['action'] = v[5];
            m['idName'] = v[6];
            m['id'] = v[7];
            m['qs'] = v[8];
            valid = 1;
            if (m['product'] != 'gunny' || m['controller'] == 'guild') {
                return;
            }
        } else if (v = thref.match(/^http:\/\/(.+)(\.zing\.vn)\/(.+)\/(.+)\/(.+)\/(.+)\/(\d+)\??(.+)/i)) {
            m['host'] = v[1];
            m['box'] = v[2];
            m['domain'] = v[3];
            var cat = v[4];
            valid = 1;

            if (cat.match(/eq2[_ ]item/i)) {
                m['qs'] = 'eq2itemname=' + v[5];
                m['name'] = 'eq2itemname' + v[5];
            } else if (cat.match(/rift[_ ]recipe/i)) {
                m['qs'] = 'riftrecipe=' + v[5];
                m['name'] = v[5];
                m['path'] = 'en';
            } else if (cat.match(/rift[_ ]collection/i)) {
                m['qs'] = 'riftcollection=' + v[5];
                m['name'] = v[5];
                m['path'] = 'en';
            } else {
                valid = 0;
            }
        } else if (v = thref.match(/^http:\/\/([^\.]+)(\..+)?\.(zing\.vn)\/(.+)\/(.+)\/(.+)\/(.+)\/(\d+)/i)) {
            m['host'] = v[1];
            m['box'] = v[2];
            m['domain'] = v[3];
            m['path'] = v[4];
            m['file'] = v[4];
            m['id'] = v[5];
            m['qs'] = v[5] + '.1.' + 'tooltip.html';
            if (m['file'] == 'achievement') {
                m['qs'] = v[5] + '.' + 'tooltip.1.html';
            }
            valid = 1;
        } else if (v = thref.match(/^http:\/\/([^\.]+)(\..+)?\.(zing\.vn|dev.vinagame\.vn)\/(.+)\/(\d+)\.(.+).html$/i)) { // old links compatibility
            m['action'] = 'display';
            m['class'] = v[2];
            m['controller'] = 'item';
            m['domain'] = '.zing.vn';
            m['host'] = subDomainName;
            m['id'] = v[5];
            m['idName'] = 'itemid';
            m['product'] = 'gunny';
            valid = 1;
        } else if (v = thref.match(/^http:\/\/([^\.]+)(\..+)?\.(dev\.vinagame\.vn|zing\.vn)\/(.+)\/([^\?]+)\?(.*)/i)) {
            m['host'] = v[1];
            m['box'] = v[2];
            m['domain'] = v[3];
            m['path'] = v[4];
            m['file'] = v[5];
            m['qs'] = v[6];

            if (m['host'] == 'union' && m['file'] == 'Item.php') { // gis.dev.vinagame.vn/item.html
                if (z = m['qs'].match(/id=(\d+)/)) {
                    if (! m['qs'].match(/setcookie=1/)) {
                        m['id'] = z[1];
                        valid = 1;
                    }
                }
            } else if (m['host'] == 'union' && m['file'] == 'Quest.php') {
                if (z = m['qs'].match(/id=(\d+)/)) {
                    if (! m['qs'].match(/setcookie=1/)) {
                        m['id'] = z[1];
                        valid = 1;
                    }
                }
            } else if ((m['host'] == 'union' || (m['host'] + m['box']) == 'dev.sgn') && m['path'] == 'character') {
                m['source'] = m['path'];
                if(m['qs'].length > 0){
                    var params = m['qs'].split("&");
                    if(params.length > 0){
                        for(var i=0; i<params.length; ++i){
                            if(params[i].search(/user=/i) === 0 || params[i].search(/acc=/i) === 0){
                                var subparams = params[i].split("=");
                                m['id'] = subparams[1];
                                break;
                            }
                        }
                    }
                }
                m['action'] = 'display';
                m['controller'] = 'character';
                m['domain'] = '.zing.vn';
                m['host'] = subDomainName;
                m['product'] = 'gunny';
                var serverArray = m['file'].split('.');
                m['qs'] = "server/" + serverArray[0] + "/user";
                valid = 1;
            }
        // ends old link compatibility
        }

        if (v && valid == 1) {
            t.style.cursor = 'pointer';
            t.title = ''; // remove the title attribute from items in the
            // forums

            if (!t.onmouseover) {
                t.onmousemove = onMouseMove;
                t.onmouseout = onMouseOut;
            }
            displayToolTip(m, isZAM);
        }
    }

    function onMouseMove(e)
    {
        e = $E(e);
        showAtCursor(e);
    }

    function onMouseOut(e)
    {
        if (top.location != document.location) {
            if (typeof top.$SGNToolTip != 'undefined') {
                top.onMouseOut(e);
                return;
            }
        }
        tt = null;
        itemDiv.style.display = 'none';
    }

    function displayToolTip(m, isZAM)
    {
        if (top.location != document.location) {
            if (typeof top.$SGNToolTip != 'undefined') {
                top.displayToolTip(m, isZAM);
                return;
            }
        }
        tt = 1;

        if (m['id']) {
            currentId = m['id'];
        } else {
            currentId = decodeURI(m['name']);
        }

        var key = m['site'] + currentId + m['locale'] + m['source'];

        if (typeof items[key] == "object") { // If it's already in the items
            showToolTip(items[key].tooltip);
        } else {
            if (!items[key]) {
                requestToolTip(m, isZAM);
            } else {
        //showLoading();
        }
        }
    }
    var staticTooltipWraper = '<div class="tooltip-content" id="tooltipcontent"><div class="ui-tooltip wiki-tooltip"><div class="tooltips">{staticContent}</div></div></div>';

    function showToolTip(itemstr)
    {
        if (typeof _gaq != 'undefined') {
            _gaq.push(['thirdTracker._setAccount', 'UA-23401728-2'],
                ['thirdTracker._trackPageview'],
                ['thirdTracker._trackEvent', 'SGN Tooltip', 'Hover Tooltip', tooltipType]);
        }
        if (top.location != document.location) {
            if (typeof top.$SGNToolTip != 'undefined') {

                top.showToolTip(itemstr);
                return;
            }
        }
        itemDiv.style.display = "block";
        if(itemstr.indexOf('<div class="tooltip-content"')==-1 && itemstr.indexOf('<div class="tooltips"')==-1){
            itemstr=staticTooltipWraper.replace('{staticContent}',itemstr);
        }

        itemDiv.innerHTML = itemstr;
        itemDiv.onmouseover = function()
        {
            this.style.display = 'block';
        };
        itemDiv.onmouseout = function()
        {
            this.style.display = 'none';
        };
    }

    function requestToolTip(m, isZAM)
    {
        if (top.location != document.location) {
            if (typeof top.$SGNToolTip != 'undefined') {

                top.requestToolTip(m, isZAM);
                return;
            }
        }
        tooltipType = m['controller'];
        url = '';
        if (m['controller'] == 'item' || m['controller'] == 'achievement' || m['controller'] == 'quest'
            || m['controller'] == 'map' || m['controller'] == 'map2' || m['controller'] == 'npc'
            ) {
            url = "http://" + m['host'] + m['domain'] + "/" + m['product'] + "/" + m['controller'] + "/" + m['action'] + "tooltip/" + m['idName'] + "/" + m['id'] + "?" + m['qs'] + '&' + m['data-item'];

            switch (m['ref']) {
                case 'hideimage' :
                    url = url + '&hideimage=1';
                    break;
            }
        } else if (m['controller'] == 'character') {
            url = "http://" + m['host'] + m['domain'] + "/" + m['product'] + "/" + m['controller'] + "/" + m['action'] + "tooltip/" + m['qs'] + "/" + m['id'];
        } else if (m['controller'] == 'mapinstance') {
            url = "http://" + m['host'] + m['domain'] + "/" + m['product'] + "/" + m['controller'] + "/" + m['action'] + "tooltip/" + m['qs'] + "/" + m['id'];
        }
        if (!url) {
            return;
        }
        if(m['refer']!=''){
            if(url.indexOf('?')>-1){
                url += '&ref=' + m['refer'];
            }else{
                url += '?ref=' + m['refer'];
            }
        }

        if (isZAM == 0) {
            url += '&remote=1';
        }
        getToolTip(url, m);
    }

    function getToolTip(url, m)
    {
        if (top.location != document.location) {
            if (typeof top.$SGNToolTip != 'undefined') {
                top.getToolTip(url, m);
                //hideLoading();
                return;
            }
        }
        obj = cacheItems[url];
        if (obj) {
            $SGNToolTip.registerItem(obj);
            // hideLoading();
            return;
        }
        showLoading();
        newElement= addAkzElement(head, createAkzElement("script", {
            type : "text/javascript",
            src : url
        }));
        //quannda add for callback function
        // most browsers, include IE 8,9
        // newElement.onload = hideLoading;
        newElement.onerror = hideLoading;

        // IE 6 & 7
        newElement.onreadystatechange = function() {
            if (this.readyState == 'complete') {
                hideLoading();
            }
        }
    }
    var isLoading = false;
    function showLoading() {
        if(top.location!=document.location){
            if(typeof top.$SGNToolTip !='undefined'){
                top.showLoading();
                return;
            }
        }
        $(itemDiv).html('<div class="tooltip-content" id="tooltipcontent"><div class="ui-tooltip wiki-tooltip"><div class="tooltips"><font color="FF9900">Đang đọc...</font></div></div></div>');
        $(itemDiv).show();
        loadingStatus = true;
    }
    function hideLoading()
    {
        if (top.location != document.location) {
            if (typeof top.$SGNToolTip != 'undefined') {
                top.hideLoading();
                return;
            }
        }
        $('#loadingimg').remove();
        isLoading = false;
    }
    var iframeIds = [];
    function showAtCursor(e, iframe)
    {
        if (top.location != document.location) {
            if (typeof top.$SGNToolTip != 'undefined') {
                top.showAtCursor(e, location.href);
                return;
            }
        }
        if (typeof iframe == 'undefined') {
            callFromIframe = 0;
        }
        var obj = itemDiv;
        var maxX;
        var maxY;
        obj.style.position = "absolute";
        obj.style.display = "block";
        if (document.all && !window.opera) {
            if (document.documentElement && typeof document.documentElement.scrollTop != undefined) {
                maxX = document.documentElement.clientWidth + document.documentElement.scrollLeft;
                maxY = document.documentElement.clientHeight + document.documentElement.scrollTop;
                y = e.clientY + document.documentElement.scrollTop;// +
                // document.documentElement.scrollTop;
                x = e.clientX + document.documentElement.scrollLeft;// +
            // document.documentElement.scrollLeft;
            } else {
                y = event.clientY + document.body.scrollTop;
                x = event.clientX + document.body.scrollLeft;
            }
        } else {
            if (document.body.scrollTop) {
                maxX = window.innerWidth + document.body.scrollLeft;
                maxY = window.innerHeight + document.body.scrollTop;
            } else {
                maxX = window.innerWidth + document.documentElement.scrollLeft;
                maxY = window.innerHeight + document.documentElement.scrollTop;
            }
            y = e.pageY;
            x = e.pageX;
        }

        // var divW = parseInt(obj.offsetWidth);
        var divW = 0;
        if (typeof obj.children[0] != 'undefined' && typeof obj.children[0].children[0] != 'undefined' && typeof obj.children[0].children[1] != 'undefined') {
            divW = obj.children[0].children[0].offsetWidth + obj.children[0].children[1].offsetWidth;
        } else {
            divW = parseInt(obj.offsetWidth);
        }
        var divH = parseInt(obj.offsetHeight);
        divW = divW ? divW : 400;
        divH = divH ? divH : 100;

        if (iframe) {
            var curleft = 0;
            var curtop = 0;
            if (typeof iframeIds[iframe] == 'undefined'
                || iframeIds[iframe] == '') {
                // var a= window.body.childNodes;
                var a = document.body.getElementsByTagName("iframe");
                for (i = 0; i < a.length; i++) {
                    var c = a[i];
                    if (c.localName == "iframe" || c.nodeName == "IFRAME") {
                        var iframehref = '';
                        try {
                            //try to get current url of iframe.
                            // can throw security exception if document inframe does not same domain with top document
                            iframehref = c.contentWindow.location.href;
                        } catch (Exception) {
                            iframehref = c.src;
                        }

                        if (iframe.indexOf(iframehref) != -1) {
                            do {
                                curleft += c.offsetLeft;
                                curtop += c.offsetTop;
                            } while (c = c.offsetParent);
                            iframeIds[iframe] = [];
                            iframeIds[iframe]['left'] = curleft;
                            iframeIds[iframe]['top'] = curtop;
                            break;
                        }
                    }
                }
            } else {
                // b = document.getElementById(iframeIds[iframe]);
                curleft = iframeIds[iframe]['left'];
                curtop = iframeIds[iframe]['top'];
            }
            x += curleft;
            y += curtop;
        }

        if (maxX && maxY) {
            var xr = 10;
            while (x + divW > (maxX - 10) && x > 0) {
                x = x - divW - xr;
                xr = 0;
            }

            var yr = 25;
            while (y + divH > (maxY - 25) && y > 0) {
                y = y - divH - yr;
                yr = 0;
            }
        }

        if (document.body.style.marginTop)
            y = y - parseInt(document.body.style.marginTop.replace('px', ''));

        if (x + 15 < 0) {
            x = -15;
        }
        if (y + 15 < 0) {
            y = -15;
        }
        obj.style.left = x + 15 + "px";
        obj.style.top = y + 15 + "px";
    }

    function onPageShow(e)
    {
        if (top.location != document.location) {
            try {
                if (typeof top.$SGNToolTip != 'undefined') {

                    top.onPageShow(e);
                    return;
                }
            } catch (ex) {
            // in case unionpower on top page does not load completely
            }
        }
        if (e.persisted) {
            tt = null;
            itemDiv.style.display = 'none';
        }
    }

    function init()
    {
        if (!document.getElementById('tmpItemFrm')) {
            addAkzElement(body, createAkzElement("div", {
                id : 'tmpItemFrm'
            }));
            document.getElementById('tmpItemFrm').style.display = 'none';
        }

        itemDiv = document.getElementById('tmpItemFrm');
        if (window.location.hostname != domainName) {
            if (unionCss != 0) {
                addAkzElement(head, createAkzElement("link", {
                    type : "text/css",
                    href : unionCss,
                    rel : "stylesheet"
                }));
            }
        }
        if (typeof jQuery == 'undefined') {
            addAkzElement(head, createAkzElement("script", {
                type : "text/javascript",
                src : jqueryUrl
            }));
        }
        addAkzEvent(document, "mouseover", onMouseOver);
        addAkzEvent(window, "pageshow", onPageShow);
    }
    $SGNToolTip = new function() {
        this.registerItem = function(obj) {
            if (top.location != document.location) {
                if (typeof top.$SGNToolTip != 'undefined') {
                    top.$SGNToolTip.registerItem(obj);
                    return;
                }
            }
            var site = obj.site;
            var id;

            if (obj.key) {
                id = obj.key;
            } else if (obj.id) {
                id = obj.id;
            } else {
                id = obj.name;
                id = id.replace(/\+/g, '%2B');
            }
            if (obj.cookie == 10) {
                addAkzElement(head, createAkzElement("script", {
                    type : "text/javascript",
                    src : jsonUrl
                }));
                prefs.data = obj;
                prefs.save(obj.type + obj.id);
            }
            var locale = typeof obj.locale != 'undefined' ? obj.locale : 'enUS';
            var source = typeof obj.source != 'undefined' ? obj.source : 'live';
            var key = site + id + locale + source;
            items[key] = obj;
            if (tt == 1 && id == currentId) {
                cacheItems[url] = obj;
                if (typeof (documentReady) != 'undefined')
                    documentReady();
                showToolTip(items[key].tooltip);
            }
        }
        init();
    }
}

if (!UnionEmbed) {
    var UnionEmbed = {
        ids : [],
        comment_ids : [],
        initialized : false,
        init : function()
        {
            var __self = this, scripts, readers;
            if (this.initialized)
                return;
            this.initialized = true;
            if (((readers = document.getElementsByTagName('union:character')) && readers.length > 0) || ((readers = document.getElementsByTagName('character:embed')) && readers.length > 0)) {
                do {
                    __self.reader(readers[0]);
                } while (((readers = document.getElementsByTagName('union:character')) && readers.length > 0) || ((readers = document.getElementsByTagName('character:embed')) && readers.length > 0))
            }
            if ((readers = document.getElementsByTagName('union:comment')) && readers.length > 0) {
                do {
                    __self.readerComment(readers[0]);
                } while ((readers = document.getElementsByTagName('union:comment')) && readers.length > 0)
            }

        },
        reader : function(elem)
        {
            var iframe, container, characterSrc;
            container = elem.parentNode;
            var unique_id = Math.floor(Math.random() * 14);
            while (typeof this.ids[unique_id] != 'undefined') {
                unique_id = Math.floor(Math.random() * 14);
            }

            if (!elem.attributes.href) {
                if (elem.attributes.server && elem.attributes.character) {
                    characterSrc = unionUrl + '/gunny/character/server/' + elem.attributes.server.nodeValue + '/character/' + elem.attributes.character.nodeValue + '/embed/1?id=' + unique_id + ''
                }
            } else {
                characterSrc = elem.attributes.href.nodeValue;
                var server = '';
                var user = '';

                if(characterSrc.indexOf('.html') > -1){//version1 link
                    var tmp = characterSrc.split('.html?');
                    var tmp2 = tmp[0].split('/');
                    if (tmp2.length > 0) {
                        server = tmp2[tmp2.length - 1];
                    }
                    if (typeof tmp[1] != 'undefined') {
                        tmp2 = tmp[1].split('&');
                        for (i = 0; i < tmp2.length; i++) {
                            var tmp3 = tmp2[i].split('=');
                            if (tmp3[0] == 'user') {
                                user = tmp3[1];
                            }
                        }
                    }
                }else{
                    //find server & char id param from link. then re-build link
                    var tmp = characterSrc.split('/');
                    for(i=0; i<tmp.length;i++){
                        if(tmp[i]=='server'){
                            i++;
                            if(typeof tmp[i] != 'undefined'){
                                server= tmp[i];
                            }else{
                                return;
                            }
                        }else if(tmp[i]=='user'){
                            i++;
                            if(typeof tmp[i] != 'undefined'){
                                user= tmp[i];
                            }else{
                                return;
                            }
                        }
                    }
                }
                if (server != '' && user != '') {
                    characterSrc = unionUrl + '/gunny/character/display/server/' +  server + '/user/' + user + '/embed/1?id=' + unique_id + ''
                } else {
                    characterSrc = '';
                }

            }
            if (characterSrc) {
                iframe = document.createElement('iframe');
                // iframe.style.display = 'none';
                iframe.width = 450;
                iframe.height = 470;
                iframe.style.border = "none";
                iframe.frameborder = "0";
                iframe.style.backUnionEmbedound = "url(" + ajaxLoader + ") center center no-repeat";
                iframe.allowTransparency = true;
                iframe.scrolling = 'no';
                iframe.style.overflow = 'hidden';

                iframe.id = unique_id;
                this.ids[unique_id] = unique_id;
                iframe.src = characterSrc;
                tmp_container = document.createElement('div');
                tmp_container.appendChild(iframe);
                if (elem.attributes.align) {
                    tmp_container.style.textAlign = elem.attributes.align.nodeValue;
                }
                if (elem.getAttribute('class')) {
                    tmp_container.className = elem.getAttribute('class');
                }
                if (elem.attributes.id) {
                    tmp_container.id = elem.attributes.id.nodeValue;
                }
                container.replaceChild(tmp_container, elem);

            // iframe.style.display = 'block';
            } else {
                container.removeChild(elem);
            }
        },
        readerComment : function(elem)
        {
            var iframe, container, commentSrc;
            container = elem.parentNode;
            var unique_id = Math.floor(Math.random() * 14);
            while (typeof this.comment_ids[unique_id] != 'undefined') {
                unique_id = Math.floor(Math.random() * 14);
            }

            commentSrc = '';
            if (elem.attributes.site && elem.attributes.feature && elem.attributes.object) {
                commentSrc = 'http://union.zing.vn/comment/index.html';
                commentSrc += '?site=' + elem.attributes.site.nodeValue;
                commentSrc += '&feature=' + elem.attributes.feature.nodeValue;
                commentSrc += '&object=' + elem.attributes.object.nodeValue;
                commentSrc += '&frame_id=' + unique_id;
                if (elem.attributes.param1) {
                    commentSrc += '&param1=' + elem.attributes.param1.nodeValue;
                }
                if (elem.attributes.param2) {
                    commentSrc += '&param2=' + elem.attributes.param2.nodeValue;
                }
                if (elem.attributes.param3) {
                    commentSrc += '&param3=' + elem.attributes.param3.nodeValue;
                }
                if (elem.attributes.number) {
                    commentSrc += '&number=' + elem.attributes.number.nodeValue;
                }
                iframe = document.createElement('iframe');
                // iframe.style.display = 'none';
                iframe.width = (elem.attributes.width) ? elem.attributes.width.nodeValue : 450;
                iframe.height = (elem.attributes.height) ? elem.attributes.height.nodeValue : 460;

                iframe.style.border = "none";
                iframe.frameborder = "0";
                iframe.style.backUnionEmbedound = "url(" + ajaxLoader + ") center center no-repeat";
                iframe.allowTransparency = true;
                iframe.scrolling = 'no';
                iframe.style.overflow = 'hidden';

                iframe.id = unique_id;
                this.comment_ids[unique_id] = unique_id;
                iframe.src = commentSrc;
                container.replaceChild(iframe, elem);

            } else {
                container.removeChild(elem);
            }
        }
    };
    if(typeof unionSearch =='undefined'){
        var unionSearch = {
            unionUrl:'http://union.zing.vn/', // base id of div contain main site output
            serviceUrl:'http://beta.union.zing.vn/', // base id of div contain main site output
            tabId:'boxTab',
            currentKeyword:'',
            tabHeaderId:'tabHeader',
            activeTabCssClass:'ui-tabs-selected',
            searchKeyInput:'Keyword',
            mainsiteResult:'mainsite_result',
            loadingTimeout:5000,
            tabs:[],
            titles:[],//title of result type, will be fill by request search from union.
            currentTab:'',
            loadTab:function(tabId){
                jQuery('#'+this.tabs[this.currentTab]).hide();
                jQuery('#'+this.tabs[tabId]).show();
                jQuery('#'+this.currentTab).removeClass(this.activeTabCssClass);
                jQuery('#'+tabId).addClass(this.activeTabCssClass);
                this.currentTab = tabId;
                //check if loaded content
                if(!jQuery('#'+this.tabs[tabId]).hasClass('loaded') && !jQuery('#'+this.tabs[tabId]).hasClass('loading')){
                    var type='';
                    if(tabId !='character'){
                        //  gom chung misc load 1 lan 
                        type='misc';
                        for(tab in this.tabs){
                            if(tab!='character' && !jQuery('#'+this.tabs[tab]).hasClass('loaded')&& !jQuery('#'+this.tabs[tab]).hasClass('loading')){
                                jQuery('#'+this.tabs[tab]).addClass('loading');
                            }
                        }
                    }else{
                        type = 'character';
                        jQuery('#'+this.tabs[tabId]).addClass('loading');
                    }
                    var head = document.getElementsByTagName("head")[0];
                    addAkzElement(head, createAkzElement("script", {
                        type : "text/javascript",
                        src : this.serviceUrl + 'gunny/search/result?keyword='+this.currentKeyword +'&type='+type
                    }));
                    setTimeout('unionSearch.checkTimeout("'+type+'")',this.loadingTimeout);
                }
            },
            checkTimeout:function(type){
                if(type !='character'){
                    //  gom chung misc load 1 lan 
                    for(tab in this.tabs){
                        if(tab!='character' && !jQuery('#'+this.tabs[tab]).hasClass('loaded')&& !jQuery('#'+this.tabs[tab]).hasClass('loading')){
                            jQuery('#'+this.tabs[tab]).addClass('loaded');
                            jQuery('#'+this.tabs[tab]+'_content').html('Có lỗi trong quá trình tải dữ liệu');
                        }
                    }
                }else{
                    if(!jQuery('#'+this.tabs[type]).hasClass('loaded')){
                        jQuery('#'+this.tabs[type]).addClass('loaded');
                        jQuery('#'+this.tabs[type]+'_content').html('Có lỗi trong quá trình tải dữ liệu');
                    }
                }
            },
            init:function(){
                //depend on union power
                var head = document.getElementsByTagName("head")[0];
                this.currentKeyword = jQuery('#'+this.searchKeyInput).val();
                
                //init tab
                var tmp =jQuery('#frmNewsSearch');
                //create tabheader for all result
                jQuery('<div id="'+this.tabId+'"><ul id="'+this.tabHeaderId+'"></ul></div>')
                .insertAfter(tmp);

                // create div for mainsite search result
                jQuery('<div id="'+this.mainsiteResult+'"></div>').insertAfter(jQuery('#'+this.tabId));

                // move sep0erate element of mainsite result to inside of mainsite search result
                var contentInfo = jQuery('#search_info');
                var mainsiteTab = jQuery('#'+this.mainsiteResult);
                mainsiteTab.append(contentInfo.clone());
                contentInfo.remove();

                var searchResult = jQuery('#search_result');
                mainsiteTab.append(searchResult.clone());
                searchResult.remove();
                var tmp = jQuery('div.PagingWrapper');
                if(tmp){
                    var newPaginator = tmp.first().clone();
                    mainsiteTab.prepend(newPaginator.clone());
                    mainsiteTab.append(newPaginator);
                    tmp.remove();
                }

                this.insertTab('tat-ca','Bài viết', this.mainsiteResult, jQuery('#search_info').html().replace(/[^0-9]/g,''));
                // tab created
                this.insertTab('sgn_loading','<img src="'+ajaxLoader+'">', '', 0);
                              
                
                addAkzElement(head, createAkzElement("script", {
                    type : "text/javascript",
                    src : this.serviceUrl + 'gunny/search/service?keyword='+this.currentKeyword +'&ref=gn-news-search'
                }));
            },
            insertTab:function(tabId, tabTitle, containId, totalResult){
                
                if(tabId =='sgn_loading'){
                    jQuery('#'+this.tabHeaderId).append(jQuery('<li id="'+tabId+'">' +
                    '<a href="javascript:void(0)"><span>'+tabTitle +'</span></a>' +
                    '</li>'));
                    return;
                }
                
                if(typeof containId == 'undefined'){
                    containId ='conent-'+tabId;
                }
                this.tabs[tabId]=containId;
                var cssClass='';
                if(this.currentTab==''){
                    cssClass=this.activeTabCssClass;
                    this.currentTab = tabId;
                    jQuery('#'+this.tabs[tabId]).addClass('loaded')
                }
                if(totalResult>0){
                    tabTitle += '('+totalResult+')';
                }
                jQuery('#'+this.tabHeaderId).append(jQuery('<li id="'+tabId+'" class="'+cssClass+'">' +
                    '<a href="#'+tabId+'" onclick="unionSearch.loadTab(\''+tabId+'\');"><span>'+tabTitle +'</span></a>' +
                    '</li>'));
                
                jQuery('<div id="'+this.tabs[tabId]+'" style="display:none"><b>Có '+totalResult+' kết quả được tìm thấy.</b>'+
                    '<div id="'+this.tabs[tabId]+'_content"><img align="center" style="margin:20px;" src="'+ajaxLoader+'"/></div></div>').insertAfter(jQuery('#'+this.mainsiteResult));                
                
            },
            result:function(data){
                // remove or relace contentInfoId & paginator is specify for gunny,
                // if implement for another, pls using inheritance
                if(data){
                    for (resultType in data['result']) {
                        this.insertTab(resultType,this.titles[resultType],resultType+'_content', data['result'][resultType]);
                    //this.showUnionData(resultType,data[resultType]);
                    };
                    jQuery('#tabHeader').css('height','auto');
                }
                jQuery('#sgn_loading').remove();
            },
            showResult:function(data){
                for (resultType in data) {
                    this.showUnionData(resultType,data[resultType]);
                };
                
            }
        };
        
        //specify inherit implement for each game
        //
        //this will be call in script request from union (like the way as implement tooltip)
        unionSearch.showUnionData = function(type, data){
            //generate div for contain info
            var html='';
            if(type =='character'){
                html +=  this.showCharacter(data);
            }else if(type =='achievement'){
                html += this.showAchivement(data);
            }else if(type =='item'){
                html += this.showItem(data);
            }else if(type =='quest'){
                html += this.showQuest(data);
            }else if(type =='npc'){
                html += this.showNpc(data);
            }else if(type =='guild'){
                html += this.showGuild(data);
            }
            
            jQuery('#'+this.tabs[type]+'_content').html(html);
            jQuery('#'+this.tabs[type]).addClass('loaded');
            
        };
        unionSearch.showCharacter=function(data){
            var html ='<table class="MyTable" width="98%" cellpadding="0" cellspacing="0">'+
            '<thead>'+
            '<tr>'+
            '<th width="auto">Nhân vật</th>'+
            '<th width="17%">Máy chủ</th>'+
            '<th width="12%">Cấp</th>'+
            '</tr>'+
            '</thead>'+
            '<tbody>';
            var className='';
            for(i=0; i<data.length; i++){
                className = (i%2==0)?'OddRow':'EvenRow';
                html += '<tr><td class="'+className+'"><a href="'+this.unionUrl+'gunny/character/display/server/'+data[i].server + '/user/'+data[i].UserID + '">'+
                data[i].NickName + ' - ' + data[i].serverName + ' - '+ data[i].Grade + '</a></td>';
                html += '<td class="'+className+'">' + data[i].serverName + '</td>';
                html += '<td class="'+className+'">' + data[i].Grade + '</a></td></tr>';
            }
            return html+'</tbody></table>';
        }
        unionSearch.showAchivement=function(data){
            var html ='<table class="MyTable" width="98%" cellpadding="0" cellspacing="0">'+
            '<thead>'+
            '<tr>'+
            '<th width="auto">Thành tựu</th>'+
            '<th width="17%">Mô tả</th>'+
            '<th width="12%">Điểm</th>'+
            '</tr>'+
            '</thead>'+
            '<tbody>';
            var className='';
            for(i=0; i<data.length; i++){
                className = (i%2==0)?'OddRow':'EvenRow';
                html += '<tr><td class="'+className+'"><a href="'+this.unionUrl+'gunny/achievement/display/achievementid/'+data[i].ID +'?ref=gn-news-search'+ '">'+
                data[i].Title + '</td>';
                html += '<td class="'+className+'">' + data[i].Detail +'</td>';
                html += '<td class="'+className+'">' + data[i].AchievementPoint +'</td></tr>';
            }
            return html+'</tbody></table>';

        },
        unionSearch.showItem=function(data){
            var html ='<table class="MyTable" width="98%" cellpadding="0" cellspacing="0">'+
            '<thead>'+
            '<tr>'+
            '<th width="auto">Vật phẩm</th>'+
            '<th width="17%">Cấp</th>'+
            '<th width="17%">Giới tính</th>'+
            '<th width="17%">Loại</th>'+
            '<th width="17%">Phẩm chất</th>'+
            '</tr>'+
            '</thead>'+
            '<tbody>';
            var className='';
            var sex='',itemType='',itemColor='',quality='';
            for(i=0; i<data.length; i++){
                className = (i%2==0)?'OddRow':'EvenRow';
                sex = (data[i].NeedSex==0)?'Cả hai':(data[i].NeedSex==1)?'Name':'Nữ';
                itemType = (typeof this.itemCat[data[i].CategoryID] !='undefined')?this.itemCat[data[i].CategoryID]:'';
                itemColor = (typeof this.itemColor[data[i].Quality] !='undefined')?this.itemColor[data[i].Quality]:'';
                quality = (typeof this.itemQuality[data[i].Quality] !='undefined')?this.itemQuality[data[i].Quality]:'';
                html += '<tr style="color:'+itemColor+'"><td class="'+className+'"><a href="'+this.unionUrl+'gunny/item/display/itemid/'+data[i].TemplateID + '">'+
                data[i].Name + '</a></td>';
                html += '<td class="'+className+'">'+data[i].Level + '</td>';
                html += '<td class="'+className+'">'+sex + '</td>';
                html += '<td class="'+className+'">'+itemType+ '</td>';
                html += '<td class="'+className+'">'+quality + '</td></tr>';
            }
            return html+'</tbody></table>';
        }
        unionSearch.showQuest=function(data){
            var html ='<table class="MyTable" width="98%" cellpadding="0" cellspacing="0">'+
            '<thead>'+
            '<tr>'+
            '<th width="auto">Nhiệm vụ</th>'+
            '</tr>'+
            '</thead>'+
            '<tbody>';
            var className='';
            for(i=0; i<data.length; i++){
                className = (i%2==0)?'OddRow':'EvenRow';
                html += '<tr><td class="'+className+'"><a href="'+this.unionUrl+'gunny/quest/display/questid/'+data[i].ID +'?ref=gn-news-search'+ '">'+
                data[i].Title + '</a></td></tr>';
            }
            return html+'</tbody></table>';
            
        } 
        unionSearch.showNpc=function(data){
            var html ='<table class="MyTable" width="98%" cellpadding="0" cellspacing="0">'+
            '<thead>'+
            '<tr>'+
            '<th width="auto">NPC</th>'+
            '<th width="auto">Cấp</th>'+
            '<th width="auto">Máu</th>'+
            '</tr>'+
            '</thead>'+
            '<tbody>';
            var className='';
            for(i=0; i<data.length; i++){
                className = (i%2==0)?'OddRow':'EvenRow';
                html += '<tr><td class="'+className+'"><a href="'+this.unionUrl+'gunny/npc/display/npcid/'+data[i].ID +'?ref=gn-news-search'+ '">'+
                data[i].Name + '</a></td><td class="'+className+'">'+
                data[i].Level +'</td><td class="'+className+'">'+
                data[i].Blood + '</td></tr>';
            }
            return html+'</tbody></table>';
        }
        unionSearch.showGuild=function(data){
            var html ='<table class="MyTable" width="98%" cellpadding="0" cellspacing="0">'+
            '<thead>'+
            '<tr>'+
            '<th width="auto">Tên</th>'+
            '<th width="25%">Cấp</th>'+
            '<th width="25%">Người tạo</th>'+
            '</tr>'+
            '</thead>'+
            '<tbody>';
            var className='';
            for(i=0; i<data.length; i++){
                className = (i%2==0)?'OddRow':'EvenRow';
                html += '<tr><td class="'+className+'"><a href="'+this.unionUrl+'gunny/guild/display/server/'+
                data[i].server+'/guildid/'+data[i].ConsortiaID +'?ref=gn-news-search'+ '">'+
                data[i].ConsortiaName + '</a></td><td class="'+className+'">'+
                data[i].Level +'</td><td class="'+className+'"><a href="'+this.unionUrl+'gunny/character/display/server/'+data[i].server + '/user/'+data[i].ChairmanID + '">'+
                data[i].ChairmanName + '</a></td></tr>';
            }
            return html+'</tbody></table>';
        }
    }

    if (document.readyState == 'complete' || document.readyState == 'loaded') {
        UnionEmbed.init();
    } else {
        window.onload = function()
        {
            UnionEmbed.init();
            if(getCookie('unionSearch')){
                unionSearch.init();
            }
        }
    }

}
function getCookie(c_name)
{
    var i,x,y,ARRcookies=document.cookie.split(";");
    for (i=0;i<ARRcookies.length;i++)
    {
        x=ARRcookies[i].substr(0,ARRcookies[i].indexOf("="));
        y=ARRcookies[i].substr(ARRcookies[i].indexOf("=")+1);
        x=x.replace(/^\s+|\s+$/g,"");
        if (x==c_name)
        {
            return unescape(y);
        }
    }
    return false;
}