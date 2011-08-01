﻿/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../_Global.js" />
/// <reference path="../../_CswPrototypeExtensions.js" />
/// <reference path="CswMobileMenuButton.js" />
/// <reference path="ICswMobileWebControls.js" />

//#region CswMobilePageHeader

function CswMobilePageHeader(headerDef, $parent) {
    /// <summary>
    ///   Header class. Responsible for generating a Mobile page header.
    /// </summary>
    /// <param name="headerDef" type="Object">JSON definition of header to display</param>
    /// <param name="$parent" type="jQuery">Parent element to attach to.</param>
    /// <returns type="CswMobilePageFooter">Instance of itself. Must instance with 'new' keyword.</returns>
    
    ICswMobileWebControls.call(this);
    
    //#region private
    
    var o = {
        buttons: { button1: { ID: '', text: '', 'data-icon': '', cssclass: '' } },
        ID: '',
        text: '',
        dataId: 'csw_header',
        dataTheme: 'b'
    };
    
    if(headerDef) $.extend(o, headerDef);

    var button1, button2;
    var headerId = o.ID + '_header';

    var $header = $parent.find('div:jqmData(role="header")');

    if( isNullOrEmpty($header) || $header.length === 0 )
    {
        $header = $parent.CswDiv('init', { ID: headerId });
    } 
    $header.CswAttrXml({
                'data-role': 'header',
                'data-position': 'fixed',
                'data-id': o.dataId,
                'data-theme': o.dataTheme
            });

    _pageHeader(o.text);
    
    function _pageHeader(text) {
        var ret;
        var $headerText = $header.find('#' + headerId + '_text');
        if( isNullOrEmpty($headerText) || $headerText.length === 0) {
            $headerText = $('<h1 id="' + headerId + '_text"></h1>')
                            .appendTo($header);
        }
        if(arguments.length === 1) {
            ret = $headerText.text(text);
        } else {
            ret = $headerText.text();
        }
        return ret;
    }
    
    //#endregion private
    
    //#region sheol
    
    //let's make these buttons accessible by name
    var buttonCnt = 0;
    for( var button in o.buttons) {
        buttonCnt++;

        var thisButton = o.buttons[button];
        
        switch(buttonCnt) {
            case 1:
                button1 = new CswMobileMenuButton(thisButton, $parent);
                this[button] = button1;
                break;
            case 2:
                button2 = new CswMobileMenuButton(thisButton, $parent);
                this[button] = button2;
                break;
        } 
    }
    
    //#endregion sheol
    
    //#region public, priveleged
    this.ID = o.ID;
    this.$content = $header;
    
    //in case we don't know the button names, we can fetch and query
    this.button1 = button1;
    this.button2 = button2;

    this.pageHeader = _pageHeader;
    
    //#endregion public, priveleged
    
}

//#endregion CswMobilePageHeader