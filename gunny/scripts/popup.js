/*
pbinchen@2007
*/
var popupList = [];
var _popup = Class.create();

function popup(text, options) {
    if (typeof jsbase == "undefined" || typeof Prototype == "undefined")
        return alert("base.js, prototype.js or dragdrop.js not found. All must be present for popup to work");
    var p = new _popup(text, options);
    popupList = popupList.each(function(d) {
        if (d.name == p.name) d.close();
    });
    popupList.push(p);
    return p;
}

_popup.prototype = {
    initialize: function(text, options) {
        this.id = 'popup' + randnum();
        this.name = this.id;
        this.width = 270;
        this.height = 70;
        this.title = '';
        this.desc = '';
        this.content = text;
        this.footer = '';
        this.position = {
            left: 0, top: 0,
            right: 0, bottom: 0
        };
        this.closeBtn = null;
        this.closeBtnImageUrl = 'scripts/popup/images/close.gif';

        this.divShadow = null;
        this.divCon = null;
        this.divTitle = null;
        this.divDesc = null;
        this.divContent = null;
        this.divText = null;
        this.divFooter = null;
        this.divMask = null;

        this.hasCloseBtn = true;
        this.hasMask = false;
        this.hasShadow = false;
        this.movable = false;

        Object.extend(this, options || {});
        ensure({ css: "scripts/popup/style/popup.css" }, (function() {
            this.create();
        }).bind(this));
    },
    create: function() {
        if (this.hasMask) this.showMask();

        this.divShadow = document.createElement('div');
        this.divShadow.className = 'divShadow';
        document.body.appendChild(this.divShadow);

        this.divCon = document.createElement('div');
        this.divCon.className = 'divCon';
        this.divShadow.appendChild(this.divCon);

        if (this.hasShadow) this.setShadow(-4, -5);

        if (this.hasCloseBtn) {
            this.closeBtn = new Image();
            this.closeBtn.className = 'closeBtn';
            this.closeBtn.src = this.closeBtnImageUrl;
            this.closeBtn.onclick = this.closeBtnClick.bind(this);
            this.divCon.appendChild(this.closeBtn);
        }

        this.setTitle(this.title);
        this.setDesc(this.desc);
        this.setContent(this.content);
        this.setFooter(this.footer);
        this.setPosition(this.position);

        if (this.movable) {
            ensure({ js: ["scripts/effects/effects.js", "scripts/effects/dragdrop.js"] }, (function() {
                if (this.divTitle) {
                    this.setDraggable(this.divTitle);
                } else {
                    this.setDraggable(this.divContent);
                }
            }).bind(this));
        }
        if (typeof this.filledCallback == 'function') this.filledCallback();

        Event.observe(window, 'resize', this.toResize.bind(this));
        Event.observe(window, 'scroll', this.toResize.bind(this));
    },
    setShadow: function(top, left) {
        this.divCon.style.top = top + 'px';
        this.divCon.style.left = left + 'px';
    },
    showMask: function() {
        if (this.divMask == null) {
            this.divMask = document.createElement('div');
            this.divMask.className = 'divMask';
            document.body.appendChild(this.divMask);
            this.setMaskPosition();
        }
    },
    toResize: function() {
        this.setMaskPosition();
    },
    setMaskPosition: function() {
        if (this.divMask) {
            var docSize = getDocumentSize();
            var winSize = getWindowSize();
            if (docSize.height < winSize.height)
                docSize.height = winSize.height;
            this.divMask.setAttribute('style', 'height:' + docSize.height + 'px; width:' + docSize.width + 'px;');
        }
    },
    setTitle: function(title) {
        this.title = title;
        if (this.title != '') {
            if (this.divTitle == null) {
                this.divTitle = document.createElement('div');
                this.divTitle.className = 'divTitle';
                this.divCon.appendChild(this.divTitle);
            }
            this.divTitle.innerHTML = this.title;
        } else if (this.divTitle) {
            this.divCon.removeChild(this.divTitle);
            this.divTitle = null;
        }
    },
    setDesc: function(desc) {
        this.desc = desc;
        if (this.desc != '') {
            if (this.divDesc == null) {
                this.divDesc = document.createElement('div');
                this.divDesc.className = 'divDesc';
                this.divCon.appendChild(this.divDesc);
            }
            this.divDesc.innerHTML = this.desc;
        } else if (this.divDesc) {
            this.divCon.removeChild(this.divDesc);
            this.divDesc = null;
        }
    },
    setContent: function(content) {
        this.content = content;
        if (this.divContent == null) {
            this.divContent = document.createElement('div');
            this.setSize(this.width, this.height);
            this.divContent.className = 'divContent';
            this.divCon.appendChild(this.divContent);
        }
        this.divText = document.createElement('div');
        this.divText.className = 'text';
        this.divText.innerHTML = this.content;
        this.divContent.appendChild(this.divText);
    },
    setFooter: function(footer) {
        this.footer = footer;
        if (this.footer != '') {
            if (this.divFooter == null) {
                this.divFooter = document.createElement('div');
                this.divFooter.className = 'divFooter';
                this.divCon.appendChild(this.divFooter);
            }
            this.divFooter.innerHTML = this.footer;
        } else if (this.divFooter) {
            this.divCon.removeChild(this.divFooter);
            this.divFooter = null;
        }
    },
    setPosition: function(position) {
        if (this.divShadow) {
            var c1 = '';
            if (position.left) {
                this.position.left = position.left;
                c1 += ' left:' + position.left + 'px;';
            }
            if (position.top) {
                this.position.top = position.top;
                c1 += ' top:' + position.top + 'px;';
            }
            if (position.right) {
                this.position.right = position.right;
                c1 += ' right:' + position.right + 'px;';
            }
            if (position.bottom) {
                this.position.bottom = position.bottom;
                c1 += ' bottom:' + position.bottom + 'px;';
            }
            if (c1 == '') {
                var ws = getWindowSize();
                var sc = getWindowScroll(window);
                var h = this.height;
                if (this.divShadow.offsetHeight > h)
                    h = this.divShadow.offsetHeight;

                var left = (ws.width - this.width) / 2 + sc.left;
                var top = (ws.height - h) / 2 + sc.top;

                if (ws.width < this.width) left = 2;
                if (ws.height < h) top = 10;

                c1 = 'top:' + top + 'px; left:' + left + 'px;';
            }
            $(this.divShadow).setStyle({'top':top+'px', 'left':left+'px'});
            $(this.divShadow).setAttribute('style', c1);
        }
    },
    setSize: function(width, height) {
        this.width = width;
        this.height = height;
        this.divContent.style.width = width + 'px';
        this.divContent.style.height = height + 'px';
    },
    setDraggable: function(handle) {
        if (handle) {
            setTimeout((function() {
                if (this.dragHandle && this.dragHandle == handle) {
                    this.dg.destroy();
                }
                this.dragHandle = handle;
                this.dg = new Draggable(this.divShadow, { 'handle': handle });
            }).bind(this), 500);
        }
    },
    close: function() {
        if (this.divShadow) {
            document.body.removeChild(this.divShadow);
            this.divShadow = null;
            var t = this;
            popupList.each(function(d) {
                if (d.name == t.name) {
                    popupList = popupList.without(d);
                }
            });
        }
        if (this.divMask) {
            document.body.removeChild(this.divMask);
            this.divMask = null;
        }
        if (typeof this.closedCallback == 'function') this.closedCallback();

        Event.stopObserving(window, 'resize', this.toResize);
        Event.stopObserving(window, 'scroll', this.toResize);
    },
    closeBtnClick: function() {
        this.close();
    },
    filledCallback: function() { },
    closedCallback: function() { }
};