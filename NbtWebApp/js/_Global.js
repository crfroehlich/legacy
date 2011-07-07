﻿/// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../js/thirdparty/jquery/core/jquery.mobile/jquery.mobile-1.0b1.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="../js/thirdparty/jquery/plugins/jquery-validate-1.8/jquery.validate.js" />
/// <reference path="CswClasses.js" />

// ------------------------------------------------------------------------------------
// Enums
// ------------------------------------------------------------------------------------

var EditMode = {
	Edit: { name: 'Edit' },
	AddInPopup: { name: 'AddInPopup' },
	EditInPopup: { name: 'EditInPopup' },
	Demo: { name: 'Demo' },
	PrintReport: { name: 'PrintReport' },
	DefaultValue: { name: 'DefaultValue' },
	AuditHistoryInPopup: { name: 'AuditHistoryInPopup' }
};

// for CswInput
var CswInput_Types = {
    button: { id: 0, name: 'button', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
    checkbox: { id: 1, name: 'checkbox', placeholder: false, autocomplete: false, value: { required: true, allowed: true }, defaultwidth: '' },
    color: { id: 2, name: 'color', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
    date: { id: 3, name: 'date', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    datetime: { id: 4, name: 'datetime', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '200px' },
    'datetime-local': { value: 5, name: 'datetime-local', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    email: { id: 6, name: 'email', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    file: { id: 7, name: 'file', placeholder: false, autocomplete: false, value: { required: false, allowed: false }, defaultwidth: '' },
    hidden: { id: 8, name: 'hidden', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
    image: { id: 9, name: 'image', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
    month: { id: 10, name: 'month', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
    number: { id: 11, name: 'number', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '200px' },
    password: { id: 12, name: 'password', placeholder: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    radio: { id: 13, name: 'radio', placeholder: false, autocomplete: false, value: { required: true, allowed: true }, defaultwidth: '' },
    range: { id: 14, name: 'range', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
    reset: { id: 15, name: 'reset', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
    search: { id: 16, name: 'search', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
    submit: { id: 17, name: 'submit', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' },
    tel: { id: 18, name: 'button', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '' },
    text: { id: 19, name: 'text', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    time: { id: 20, name: 'time', placeholder: false, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    url: { id: 21, name: 'url', placeholder: true, autocomplete: true, value: { required: false, allowed: true }, defaultwidth: '200px' },
    week: { id: 22, name: 'week', placeholder: false, autocomplete: false, value: { required: false, allowed: true }, defaultwidth: '' }
};

var CswAppMode = {
    mode: 'full'     
};

// ------------------------------------------------------------------------------------
// Globals (yuck)
// ------------------------------------------------------------------------------------

var homeUrl = 'NewMain.html';

var timeout = '';
function getTimeout()
{
	return timeout;
}


// ------------------------------------------------------------------------------------
// Current State
// ------------------------------------------------------------------------------------

function setCurrentView(viewid, viewmode)
{
	clearCurrent();
	$.CswCookie('set', CswCookieName.CurrentViewId, viewid);
	$.CswCookie('set', CswCookieName.CurrentViewMode, viewmode);
}

function setCurrentAction(actionname, actionurl)
{
	clearCurrent();
	$.CswCookie('set', CswCookieName.CurrentActionName, actionname);
	$.CswCookie('set', CswCookieName.CurrentActionUrl, actionurl);
}

function clearCurrent()
{
	$.CswCookie('clear', CswCookieName.CurrentViewId);
	$.CswCookie('clear', CswCookieName.CurrentViewMode);
	$.CswCookie('clear', CswCookieName.CurrentActionName);
	$.CswCookie('clear', CswCookieName.CurrentActionUrl);
}

function getCurrent()
{
	return {
		'viewid': $.CswCookie('get', CswCookieName.CurrentViewId),
		'viewmode': $.CswCookie('get', CswCookieName.CurrentViewMode),
		'actionname': $.CswCookie('get', CswCookieName.CurrentActionName),
		'actionurl': $.CswCookie('get', CswCookieName.CurrentActionUrl)
	};
}

// ------------------------------------------------------------------------------------
// Ajax
// ------------------------------------------------------------------------------------

var _ajaxCount = 0;
function ajaxInProgress()
{
	return (_ajaxCount > 0);
}

function CswAjaxJSON(options)
{ /// <param name="$" type="jQuery" />
    /// <summary>
    ///   Executes Async webservice request for JSON
    /// </summary>
    /// <param name="options" type="Object">
    ///     A JSON Object
    ///     &#10;1 - options.url: WebService URL
    ///     &#10;2 - options.data: {field1: value, field2: value}
    ///     &#10;3 - options.success: function() {}
    ///     &#10;4 - options.error: function() {}
    /// </param>
	var o = {
		url: '',
		data: {},
		onloginfail: function () { _finishLogout(); },
		success: function (result) { },
		error: function () { },
        formobile: false,
        async: true
	};

    if (options) $.extend(o, options);
    //var starttime = new Date();
    _ajaxCount++;
    $.ajax({
        type: 'POST',
        async: o.async,
        url: o.url,
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(o.data),
        success: function (data, textStatus, XMLHttpRequest)
        {
        	_ajaxCount--;
        	//var endtime = new Date();
            //$('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");
            var result = $.parseJSON(data.d);

            if (result.error !== undefined)
            {
                _handleAjaxError(XMLHttpRequest, { 'message': result.error.message, 'detail': result.error.detail }, '');
                o.error(XMLHttpRequest, textStatus, errorThrown);
            }
            else
            {

                var auth = tryParseString(result.AuthenticationStatus, 'Unknown');
                timeout = tryParseString(result.timeout, '');

                _handleAuthenticationStatus({
                    status: auth,
                    success: function () { o.success(result); },
                    failure: o.onloginfail,
                    usernodeid: result.nodeid,
                    usernodekey: result.cswnbtnodekey,
                    passwordpropid: result.passwordpropid,
                    ForMobile: o.formobile
                });
            }
        }, // success{}
        error: function (XMLHttpRequest, textStatus, errorThrown)
        {
        	_ajaxCount--;
        	//_handleAjaxError(XMLHttpRequest, { 'message': 'A Webservices Error Occurred', 'detail': textStatus }, errorThrown);
            log("Webservice Request (" + o.url + ") Failed: " + textStatus);
            o.error(XMLHttpRequest, textStatus, errorThrown);
        }
    });           // $.ajax({
} // CswAjaxXml()

function CswAjaxXml(options)
{
    /// <summary>
    ///   Executes Async webservice request for XML
    /// </summary>
    /// <param name="options" type="Object">
    ///     A JSON Object
    ///     &#10;1 - options.url: WebService URL
    ///     &#10;2 - options.data: {field1: value, field2: value}
    ///     &#10;3 - options.success: function() {}
    ///     &#10;4 - options.error: function() {}
    ///     &#10;5 - options.formobile: false
    /// </param>

    var o = {
        url: '',
        data: {},
        stringify: false, //in case we need to conditionally apply $.param() instead of JSON.stringify() (or both)
        onloginfail: function () { _finishLogout(); },
        success: function ($xml) { },
        error: function () { },
        formobile: false,
        async: true
	};

    if (options) $.extend(o, options);
    
    if (!isNullOrEmpty(o.url))
    {
    	_ajaxCount++;
    	$.ajax({
    		type: 'POST',
    		async: o.async,
    		url: o.url,
    		dataType: "text",
    		//contentType: 'application/json; charset=utf-8',
    		data: $.param(o.data),     // should be 'field1=value&field2=value'
    		success: function (data, textStatus, XMLHttpRequest)
    		{
    			_ajaxCount--;
    			//var endtime = new Date();
    			//$('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");

    			var $realxml;
    			if ($.browser.msie)
    			{
    				// We have to use third-party jquery.xml.js for Internet Explorer to handle non-DOM XML content
    				$realxml = $.xml(data);
    			}
    			else
    			{
					$realxml = $(XMLHttpRequest.responseXML).children().first();    			
				}

    			if ($realxml.first().get(0).nodeName === "error")
    			{
    				_handleAjaxError(XMLHttpRequest, { 'message': $realxml.CswAttrXml('message'), 'detail': $realxml.CswAttrXml('detail') }, '');
    				o.error();
    			}
    			else
    			{
    				var auth = tryParseString($realxml.CswAttrXml('authenticationstatus'), 'Unknown');
    				timeout = $realxml.CswAttrXml('timeout');

    				_handleAuthenticationStatus({
    					status: auth,
    					success: function () { o.success($realxml) },
    					failure: o.onloginfail,
    					usernodeid: tryParseString($realxml.CswAttrXml('nodeid'), ''),
    					usernodekey: tryParseString($realxml.CswAttrXml('cswnbtnodekey'), ''),
    					passwordpropid: tryParseString($realxml.CswAttrXml('passwordpropid'), ''),
                        ForMobile: o.formobile
    				});
    			}

    		}, // success{}
    		error: function (XMLHttpRequest, textStatus, errorThrown)
    		{
    			_ajaxCount--;
    			//_handleAjaxError(XMLHttpRequest, { 'message': 'A Webservices Error Occurred', 'detail': textStatus }, errorThrown);
    			log("Webservice Request (" + o.url + ") Failed: " + textStatus);
    			o.error();
    		}
    	});                             // $.ajax({
    } // if(o.url != '')
} // CswAjaxXml()

function _handleAjaxError(XMLHttpRequest, errorJson, errorThrown)
{
	/// <param name="$" type="jQuery" />
    //	ErrorMessage = "A WebServices Error Occurred: " + textStatus;
    //	if (null != errorThrown) {
    //		ErrorMessage += "; Exception: " + errorThrown.toString()
    //	}
    var $errorsdiv = $('#ErrorDiv');
    if ($errorsdiv.length > 0)
    {
        $errorsdiv.CswErrorMessage({ 'message': errorJson.message, 'detail': errorJson.detail });
    } else
    {
        log(errorJson.message + '; ' + errorJson.detail);
    }
} // _handleAjaxError()

function _handleAuthenticationStatus(options)
{
 	var o = {
		status: '',
		success: function () { },
		failure: function () { },
		usernodeid: '',
		usernodekey: '',
		passwordpropid: '',
        ForMobile: false
	};
	if(options) $.extend(o, options);

	var txt = '';
    var GoodEnoughForMobile = false; //Ignore password expirery and license accept for Mobile for now
	switch (o.status)
	{
		case 'Authenticated': o.success(); break;
		case 'Deauthenticated': o.success(); break;  // yes, o.success() is intentional here.
		case 'Failed': txt = "Invalid login."; break;
		case 'Locked': txt = "Your account is locked.  Please see your account administrator."; break;
		case 'Deactivated': txt = "Your account is deactivated.  Please see your account administrator."; break;
		case 'TooManyUsers': txt = "Too many users are currently connected.  Try again later."; break;
		case 'NonExistentAccessId': txt = "Invalid login."; break;
		case 'NonExistentSession': txt = "Your session has timed out.  Please login again."; break;
		case 'Unknown': txt = "An Unknown Error Occurred"; break;
		case 'TimedOut': 
            GoodEnoughForMobile = true;
            txt = "Your session has timed out.  Please login again."; 
            break;
		case 'ExpiredPassword':
            GoodEnoughForMobile = true;
            if( !o.ForMobile ) {
                $.CswDialog('EditNodeDialog', {
				    'nodeid': o.usernodeid,
				    'cswnbtnodekey': o.usernodekey,
				    'filterToPropId': o.passwordpropid,
				    'title': 'Your password has expired.  Please change it now:',
				    'onEditNode': function (nodeid, nodekey) { o.success(); }
			    });
            }
			break;
		case 'ShowLicense':
            GoodEnoughForMobile = true;
            if( !o.ForMobile ) {
                $.CswDialog('ShowLicenseDialog', {
				    'onAccept': function () { o.success(); },
				    'onDecline': function () { o.failure('You must accept the license agreement to use this application'); }
			    });
            }
			break;
	}

    if( o.ForMobile &&   
        ( o.status !== 'Authenticated' && GoodEnoughForMobile ) ) {
        o.success();
    }
    else if (!isNullOrEmpty(txt) && o.status !== 'Authenticated' )
	{
		o.failure(txt,o.status);
	}
} // _handleAuthenticationStatus()


function Logout(options)
{
	var o = {
		DeauthenticateUrl: '/NbtWebApp/wsNBT.asmx/deauthenticate',
		onDeauthenticate: function () { }
	};

	if (options)
	{
		$.extend(o, options);
	}

	CswAjaxJSON({
		url: o.DeauthenticateUrl,
		data: {},
		success: function (data)
		{
			_finishLogout();
			o.onDeauthenticate();
		} // success{}
	});
} // logout

function _finishLogout()
{
	$.CswCookie('clearAll');
	window.location = homeUrl;
}


//function extractCDataValue($node) {
//    // default
//    ret = $node.text();

//    // for some reason, CDATA fields come through from the webservice like this:
//    // <node><!--[CDATA[some text]]--></node>
//    var cdataval = $node.html();
//    if (cdataval != undefined && cdataval != '') {
//        var prefix = '<!--[CDATA[';
//        var suffix = ']]-->';

//        if (cdataval.substr(0, prefix.length) == prefix) {
//            ret = cdataval.substr(prefix.length, cdataval.length - prefix.length - suffix.length);
//        }
//    }
//    return ret;
//}

function xmlToString($xmlnode)
{ /// <param name="$" type="jQuery" />
    var xmlstring = '';
    if (!($xmlnode instanceof jQuery))
    {
        $xmlnode = $($xmlnode);
    }
    if (!isNullOrEmpty($xmlnode))
    {
        xmlstring = $xmlnode.get(0).xml; // IE
        if (!xmlstring)
        {            // FF, Chrome, Safari
            var s = new XMLSerializer();
            xmlstring = s.serializeToString($xmlnode.get(0));
        }
        if (!xmlstring)
        {
            $.error("Browser does not support XML operations necessary to convert to string");
        }
    }
    return xmlstring;
}

function jsonToString(j)
{
    /// <summary>
    ///   Thin wrapper around JSON.stringify()
    /// </summary>
    /// <param name="j" type="Object">A JSON Object</param>
    /// <returns type="String" />
    //    if(typeof j === "object")
    //	{
    //		var ret = "{";
    //	 	var first = true;
    //		for (var property in j)
    //		{
    //			if (j.hasOwnProperty(property))
    //			{
    //				if (!first)
    //					ret += ",";
    //				ret += " '" + property + "': ";
    //				ret += jsonToString(j[property]);
    //				first = false;
    //			}
    //		}
    //		ret += "}";
    //	} 
    //	else
    //	{
    //		ret = "'" + safeJsonParam(j) + "'";
    //	}
    return JSON.stringify(j);
} // jsonToString

//function safeJsonParam(obj) {
//    /// <summary>
//    ///   Converts an object toString and returns a regex parsed, safe-for-JSON string
//    /// </summary>
//    /// <param name="options" type="Object">A JavaScript Object representing a string to parse</param>
//    /// <returns type="String" />
//    var ret = '';
//    if (obj !== undefined)
//    {
//        var str = obj.toString();
//        ret = str.replace(/'/g, "\\'");
//    }
//	return ret;
//}

// ------------------------------------------------------------------------------------
// Check Changes
// ------------------------------------------------------------------------------------

var changed = new Number(0);
var checkChangesEnabled = true;

function setChanged()
{
    if (checkChangesEnabled)
    {
        changed = 1;
        //        var statusimage = getMainStatusImage();
        //var savebutton = $('#SaveTab');
        //        if (statusimage != null) {
        //            statusimage.style.backgroundPosition = "0px -210px";
        //            statusimage.onmouseover = function() { this.style.backgroundPosition = "-15px -210px"; }
        //            statusimage.onmouseout = function() { this.style.backgroundPosition = "0px -210px"; }
        //            statusimage.title = "There are unsaved changes";
        //        } 
        //		if (savebutton != null)
        //		{
        //			savebutton.value = "Save Changes";
        //			savebutton.disabled = false;
        //		}
    }
}

function unsetChanged()
{
    if (checkChangesEnabled)
    {
        //        var statusimage = getMainStatusImage();
        //        if(statusimage != null)
        //            statusimage.style.backgroundPosition = "0px -195px";
        //        statusimage.onmouseover = function() { this.style.backgroundPosition = "-15px -195px"; }
        //        statusimage.onmouseout = function() { this.style.backgroundPosition = "0px -195px"; }
        //        statusimage.title = "There are no changes";
        //		var savebutton = $('#SaveTab');
        //		if (savebutton != null)
        //		{
        //			if (changed != 0)
        //				savebutton.value = "Changes Saved";
        //			savebutton.disabled = true;
        //		}
        changed = 0;
    }
}

function checkChanges()
{
    if (checkChangesEnabled && changed === 1)
    {
        return 'If you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.';
    }
}

function manuallyCheckChanges()
{
    var ret = true;
    if (checkChangesEnabled && changed === 1)
    {
        ret = confirm('Are you sure you want to navigate away from this page?\n\nIf you continue, you will lose any changes made on this page.  To save your changes, click Cancel and then click the Save button.\n\nPress OK to continue, or Cancel to stay on the current page.');

        // this serves several purposes:
        // 1. after you've been prompted to lose this change, you won't be prompted again for the same change later
        // 2. multiple calls to manuallyCheckChanges() in the same event won't prompt more than once
        if (ret)
        {
            changed = 0;
        }
    }
    return ret;
}

function initCheckChanges()
{
    // Assign the checkchanges event to happen onbeforeunload
    if (!isNullOrEmpty(window.onbeforeunload))
    {
        window.onbeforeunload = function ()
        {
            var f = window.onbeforeunload;
            var ret = f();
            if (ret)
            {
                return checkChanges();
            } else
            {
                return false;
            }
        };
    } else
    {
        window.onbeforeunload = function ()
        {
            return checkChanges();
        };
    }

    //	// IE6 has this annoying habit of throwing unspecified errors if we prevent
    //	// the navigation with onbeforeunload after clicking a button.
    //	// So we're going to trap this error and prevent it from being shown.
    //	window.onerror = function (strError, uri, line)
    //	{
    //		if (strError.toLowerCase().indexOf('unspecified error') >= 0)
    //		{
    //			window.event.returnValue = true;
    //		} else
    //		{
    //			window.event.returnValue = false;
    //		}
    //	}
}

if (CswAppMode.mode === 'full')
{
    if (!isNullOrEmpty(window.onload))
    {
        window.onload = new Function('initCheckChanges(); var f=' + window.onload + '; return f();');
    } else
    {
        window.onload = function() { initCheckChanges(); };
    }
}


// ------------------------------------------------------------------------------------
// User permissions
// ------------------------------------------------------------------------------------

function IsAdministrator(options)
{
    var o = {
        'Yes': function () { },
        'No': function () { }
    };
    if (options) $.extend(o, options);

	CswAjaxJSON({
		url: '/NbtWebApp/wsNBT.asmx/isAdministrator',
		success: function (data)
			{
				if (data.Administrator === "true")
				{
					o.Yes();
				} else
				{
					o.No();
				}
			}
	});
} // IsAdministrator()

// ------------------------------------------------------------------------------------
// Node interactions
// ------------------------------------------------------------------------------------
function copyNode(options)
{
    var o = {
        'nodeid': '',
        'nodekey': '',
        'onSuccess': function (nodeid, nodekey) { },
        'onError': function () { }
    };
    if (options)
    {
        $.extend(o, options);
    }

    var dataJson = {
        NodePk: o.nodeid
    };

    CswAjaxJSON({
        url: '/NbtWebApp/wsNBT.asmx/CopyNode',
        data: dataJson,
        success: function (result)
        {
            o.onSuccess(result.NewNodeId, '');
        },
        error: o.onError
    });
}

function deleteNodes(options)
{ /// <param name="$" type="jQuery" />
    var o = {
        'nodeids': [],
        'nodekeys': [],
        'onSuccess': function (nodeid, nodekey) { },
        'onError': function () { }
    };
    if (options)
    {
        $.extend(o, options);
    }

    var jData = {
        NodePks: o.nodeids,
        NodeKeys: o.nodekeys
    };

    CswAjaxJSON({
        url: '/NbtWebApp/wsNBT.asmx/DeleteNodes',
        data: jData,
        success: function (result)
        {
        	// clear selected node cookies
			o.nodeid = $.CswCookie('clear', CswCookieName.CurrentNodeId);
        	o.cswnbtnodekey = $.CswCookie('clear', CswCookieName.CurrentNodeKey);
			// returning '' will reselect the first node in the tree
        	o.onSuccess('', '');
        },
        error: o.onError
    });
}


// ------------------------------------------------------------------------------------
// jsTree
// ------------------------------------------------------------------------------------

function jsTreeGetSelected($treediv)
{ /// <param name="$" type="jQuery" />
    var IDPrefix = $treediv.CswAttrDom('id');
    $SelectedItem = $treediv.jstree('get_selected');
    var ret = {
        'iconurl': $SelectedItem.children('a').children('ins').css('background-image'),
        'id': $SelectedItem.CswAttrDom('id').substring(IDPrefix.length),
        'text': $SelectedItem.children('a').first().text().trim(),
        '$item': $SelectedItem
    };
    return ret;
}


// ------------------------------------------------------------------------------------
// Menu
// ------------------------------------------------------------------------------------

function GoHome()
{ /// <param name="$" type="jQuery" />
	clearCurrent();
	window.location = homeUrl;
}

function HandleMenuItem(options)
{ /// <param name="$" type="jQuery" />
    var o = {
        '$ul': '',
        '$itemxml': '',
        'onLogout': function () { },
        'onAlterNode': function (nodeid, nodekey) { },
        'onSearch': { onViewSearch: function () { }, onGenericSearch: function () { } },
        'onMultiEdit': function () { },
        'onEditView': function (viewid) { },
        'onSaveView': function (newviewid) { },
        'Multi': false,
        'NodeCheckTreeId': ''
    };
    if (options)
    {
        $.extend(o, options);
    }
    var $li;
    if (o.$itemxml.CswAttrXml('href') !== undefined && o.$itemxml.CswAttrXml('href') !== '')
    {
        $li = $('<li><a href="' + o.$itemxml.CswAttrXml('href') + '">' + o.$itemxml.CswAttrXml('text') + '</a></li>')
						.appendTo(o.$ul)
    }
    else if (o.$itemxml.CswAttrXml('popup') !== undefined && o.$itemxml.CswAttrXml('popup') !== '')
    {
        $li = $('<li class="headermenu_dialog"><a href="' + o.$itemxml.CswAttrXml('popup') + '" target="_blank">' + o.$itemxml.CswAttrXml('text') + '</a></li>')
						.appendTo(o.$ul)
//						.click(function ()
//						{
//						    $.CswDialog('OpenDialog', o.$itemxml.CswAttrXml('text'), o.$itemxml.CswAttrXml('popup'));
//						    return false;
//						})
						;
    }
    else if (o.$itemxml.CswAttrXml('action') !== undefined && o.$itemxml.CswAttrXml('action') !== '')
    {
        $li = $('<li><a href="#">' + o.$itemxml.CswAttrXml('text') + '</a></li>')
						.appendTo(o.$ul);
        var $a = $li.children('a');
        switch (o.$itemxml.CswAttrXml('action'))
        {

            case 'About':
                $a.click(function () { $.CswDialog('AboutDialog'); return false; });
                break;

            case 'AddNode':
                $a.click(function ()
                {
                    $.CswDialog('AddNodeDialog', {
                        'nodetypeid': o.$itemxml.CswAttrXml('nodetypeid'),
                        'relatednodeid': o.$itemxml.CswAttrXml('relatednodeid'), //for Grid Props
                        'onAddNode': o.onAlterNode
                    });
                    return false;
                });
                break;

            case 'DeleteNode':
                $a.click(function ()
                {
                    $.CswDialog('DeleteNodeDialog', {
                        'nodename': o.$itemxml.CswAttrXml('nodename'),
                        'nodeid': o.$itemxml.CswAttrXml('nodeid'),
                        'onDeleteNode': o.onAlterNode,
                        'NodeCheckTreeId': o.NodeCheckTreeId,
                        'Multi': o.Multi
                    });
                    return false;
                });
                break;

            case 'editview':
                $a.click(function () { o.onEditView(o.$itemxml.CswAttrXml('viewid')); return false; });
                break;

            case 'CopyNode':
                $a.click(function ()
                {
                    $.CswDialog('CopyNodeDialog', {
                        'nodename': o.$itemxml.CswAttrXml('nodename'),
                        'nodeid': o.$itemxml.CswAttrXml('nodeid'),
                        'onCopyNode': o.onAlterNode
                    });
                    return false;
                });
                break;

            case 'PrintLabel':
                $a.click(function ()
                {
                    $.CswDialog('PrintLabelDialog', {
                        'nodeid': o.$itemxml.CswAttrXml('nodeid'),
                        'propid': o.$itemxml.CswAttrXml('propid')
                    });
                    return false;
                });
                break;

            case 'Home':
                $a.click(function () { GoHome(); return false; });
                break;

            case 'Logout':
                $a.click(function () { o.onLogout(); return false; });
                break;

            case 'Profile':
                $a.click(function ()
                {
                    $.CswDialog('EditNodeDialog', {
                        'nodeid': o.$itemxml.CswAttrXml('userid'),
                        'cswnbtnodekey': '',
                        'filterToPropId': '',
                        'title': 'User Profile',
                        'onEditNode': function (nodeid, nodekey) { }
                    });
                    return false;
                });
                break;

            case 'ViewSearch':
                $a.click(function ()
                {
                    o.onSearch.onViewSearch();
                });
                break;

            case 'GenericSearch':
                $a.click(function ()
                {
                    o.onSearch.onGenericSearch();
                });
                break;

            case 'multiedit':
               	$a.click(o.onMultiEdit);
               	break;
            
			case 'SaveViewAs':
				$a.click(function ()
				{
					$.CswDialog('AddViewDialog', {
						'viewid': o.$itemxml.CswAttrXml('viewid'),
						'viewmode': o.$itemxml.CswAttrXml('viewmode'),
						'onAddView': o.onSaveView
					});
					return false;
				});
				break;

        }
    }
    else
    {
        $li = $('<li>' + o.$itemxml.CswAttrXml('text') + '</li>')
						.appendTo(o.$ul)
    }
    return $li;
}




// Used by CswDialog and CswViewEditor
function makeViewVisibilitySelect($table, rownum, label)
{
	var $visibilityselect;
	var $visroleselect;
	var $visuserselect;

	IsAdministrator({
		'Yes': function ()
		{

			$table.CswTable('cell', rownum, 1).append(label);
			var $parent = $table.CswTable('cell', rownum, 2);
			var id = $table.CswAttrDom('id');

			$visibilityselect = $('<select id="' + id + '_vissel" />')
													.appendTo($parent);
			$visibilityselect.append('<option value="User">User:</option>');
			$visibilityselect.append('<option value="Role">Role:</option>');
			$visibilityselect.append('<option value="Global">Global</option>');

			$visroleselect = $parent.CswNodeSelect('init', {
				'ID': id + '_visrolesel',
				'objectclass': 'RoleClass'
			}).hide();
			$visuserselect = $parent.CswNodeSelect('init', {
				'ID': id + '_visusersel',
				'objectclass': 'UserClass'
			})

			$visibilityselect.change(function ()
			{
				var val = $visibilityselect.val();
				if (val === 'Role')
				{
					$visroleselect.show();
					$visuserselect.hide();
				}
				else if (val === 'User')
				{
					$visroleselect.hide();
					$visuserselect.show();
				}
				else
				{
					$visroleselect.hide();
					$visuserselect.hide();
				}
			}); // change
		} // yes
	}); // IsAdministrator

	return {
		'getvisibilityselect': function () { return $visibilityselect; },
		'getvisroleselect': function () { return $visroleselect; },
		'getvisuserselect': function () { return $visuserselect; }
	};

} // makeViewVisibilitySelect()



// ------------------------------------------------------------------------------------
// Popups
// ------------------------------------------------------------------------------------

function openPopup(url, height, width)
{
    var popup = window.open(url, null, 'height=' + height + ', width=' + width + ', status=no, resizable=yes, scrollbars=yes, toolbar=yes, location=no, menubar=yes');
    popup.focus();
    return popup;
}


// ------------------------------------------------------------------------------------
// Validation
// ------------------------------------------------------------------------------------

function validateTime(value)
{
    var isValid = true;
    var regex = /^(\d?\d):(\d\d)\s?([APap][Mm])?$/g;
    var match = regex.exec(value);
    if (match === null)
    {
        isValid = false;
    }
    else
    {
        var hour = parseInt(match[1]);
        var minute = parseInt(match[2]);
        if (hour < 0 || hour >= 24 || minute < 0 || minute >= 60)
        {
            isValid = false;
        }
    }
    return isValid;
} // validateTime()

function validateFloatMinValue(value, minvalue)
{
    var nValue = parseFloat(value);
    var nMinValue = parseFloat(minvalue);
    var isValid = true;

    if (nMinValue !== undefined)
    {
        if (nValue === undefined || nValue < nMinValue)
        {
            isValid = false;
        }
    }
    return isValid;
} // validateFloatMinValue()

function validateFloatMaxValue(value, maxvalue)
{
    var nValue = parseFloat(value);
    var nMaxValue = parseFloat(maxvalue);
    var isValid = true;

    if (nMaxValue !== undefined)
    {
        if (nValue === undefined || nValue > nMaxValue)
        {
            isValid = false;
        }
    }
    return isValid;
} // validateFloatMaxValue()

function validateFloatPrecision(value, precision)
{
    var isValid = true;

    var regex;
    if (precision > 0)
    {
        // Allow any valid number -- we'll round later
        regex = /^\-?\d*\.?\d*$/g;
    }
    else
    {
        // Integers Only
        regex = /^\-?\d*$/g;
    }
    if (isValid && !regex.test(value))
    {
        isValid = false;
    }

    return isValid;
} // validateFloatPrecision()

function validateInteger(value)
{
    // Integers Only
    var regex = /^\-?\d*$/g;
    return (regex.test(value));
} // validateInteger()

// ------------------------------------------------------------------------------------
// strings
// ------------------------------------------------------------------------------------

function startsWith(source, search)
{
    return (source.substr(0, search.length) === search);
}

function getTimeString(date)
{
    var ret = '';
    var hours = date.getHours()
    var minutes = date.getMinutes()
    if (minutes < 10)
    {
        minutes = "0" + minutes
    }
    ret = (hours % 12) + ":" + minutes + " ";
    if (hours > 11)
    {
        ret += "PM";
    } else
    {
        ret += "AM";
    }
    return ret;
}

function makeId(options)
{
    /// <summary>
    ///   Generates an ID for DOM assignment
    /// </summary>
    /// <param name="options" type="Object">
    ///     A JSON Object
    ///     &#10;1 - options.ID: Base ID string
    ///     &#10;2 - options.prefix: String prefix to prepend
    ///     &#10;3 - options.suffix: String suffix to append
    ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
    /// </param>
    /// <returns type="String>A concatenated string of provided values</returns>
    var o = {
        'ID': '',
        'prefix': '',
        'suffix': '',
        'Delimiter': '_'
    };
    if (options) $.extend(o, options);

    var elementId = o.ID;

    if (!isNullOrEmpty(o.prefix) && !isNullOrEmpty(elementId))
    {
        elementId = o.prefix + o.Delimiter + elementId;
    }
    if (!isNullOrEmpty(o.suffix) && !isNullOrEmpty(elementId))
    {
        elementId += o.Delimiter + o.suffix;
    }
    return elementId;
}

function makeSafeId(options)
{
    /// <summary>
    ///   Generates a "safe" ID for DOM assignment
    /// </summary>
    /// <param name="options" type="Object">
    ///     A JSON Object
    ///     &#10;1 - options.ID: Base ID string
    ///     &#10;2 - options.prefix: String prefix to prepend
    ///     &#10;3 - options.suffix: String suffix to append
    ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
    /// </param>
    /// <returns type="String>A concatenated string of provided values</returns>
    var o = {
        'ID': '',
        'prefix': '',
        'suffix': '',
        'Delimiter': '_'
    };
    if (options) $.extend(o, options);

    var elementId = o.ID;
    var toReplace = [/'/gi, /\//g];

    if (!isNullOrEmpty(o.prefix) && !isNullOrEmpty(elementId))
    {
        elementId = o.prefix + o.Delimiter + elementId;
    }
    if (!isNullOrEmpty(o.suffix) && !isNullOrEmpty(elementId))
    {
        elementId += o.Delimiter + o.suffix;
    }
    for (var i = 0; i < toReplace.length; i++)
    {
        if (!isNullOrEmpty(elementId))
        {
            elementId = elementId.replace(toReplace[i], '');
        }
    }

    return elementId;
}

function isNullOrEmpty(obj)
{
    /// <summary> Returns true if the input is null, undefined, or ''</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = false;
    if (!isFunction(obj))
    {
    	ret = $.isPlainObject(obj) && $.isEmptyObject(obj);
        if (!ret && isGeneric(obj))
        {
            ret = (trim(obj) === '');
        }
    }
    return ret;
}

function isGeneric(obj)
{
    /// <summary> Returns true if the object is not a function, array, jQuery or JSON object</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = (!isFunction(obj) && !isArray(obj) && !isJQuery(obj) && !isJson(obj));
    return ret;
}

function isFunction(obj)
{
    /// <summary> Returns true if the object is a function</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = ($.isFunction(obj));
    return ret;
}

function isArray(obj)
{
    /// <summary> Returns true if the object is an array</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = ($.isArray(obj));
    return ret;
}

function isJson(obj)
{
    /// <summary> 
    ///    Returns true if the object is a JSON object.
    ///     &#10; isJson(CswInput_Types.text) === true 
    ///     &#10; isJson(CswInput_Types.text.name) === false
    /// </summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = ($.isPlainObject(obj));
    return ret;
}

function isJQuery(obj)
{
    ret = (obj instanceof jQuery);
    return ret;
}

function isNumeric(obj)
{
    /// <summary>
    ///   Returns true if the input is a number
    /// </summary>
    /// <param name="str" type="Object">
    ///     String or object to test
    /// </param>
    var ret = false;
    if (!isNullOrEmpty(obj))
    {
        var num = parseInt(obj);
        if (num !== NaN)
        {
            ret = true;
        }
    }
    return ret;
}

function isTrue(str)
{
    /// <summary>
    ///   Returns true if the input is true, 'true', '1' or 1.
    ///   &#10;1 Returns false if the input is false, 'false', '0' or 0.
    ///   &#10;2 Otherwise returns false and (if debug) writes an error to the log.
    /// </summary>
    /// <param name="str" type="Object">
    ///     String or object to test
    /// </param>
    /// <returns type="Bool" />

    var ret;
    if (str === 'true' || str === true || str === '1' || str === 1)
    {
        ret = true;
    }
    else if (str === 'false' || str === false || str === '0' || str === 0)
    {
        ret = false;
    }
    else
    {
        ret = false;
        //if(debug) log('isTrue() was called on ' + str + ', which is not a boolean.',false);
    }
    return ret;
}

function tryParseString(inputStr, defaultStr)
{
    /// <summary>
    ///   Returns the inputStr if !isNullOrEmpty, else returns the defaultStr
    /// </summary>
    /// <param name="inputStr" type="String"> String to parse </param>
    /// <param name="defaultStr" type="String"> Default value if null or empty </param>
    /// <returns type="String" />
    var ret = defaultStr;
    if (!isNullOrEmpty(inputStr))
    {
        ret = inputStr;
    }
    return ret;
}

var Int32MinVal = new Number(-2147483648);
function tryParseNumber(inputNum, defaultNum)
{
    /// <summary>
    ///   Returns the inputNum if !NaN, else returns the defaultNum
    /// </summary>
    /// <param name="inputNum" type="Number"> String to parse to number </param>
    /// <param name="defaultNum" type="Number"> Default value if not a number </param>
    /// <returns type="Number" />
    var ret = new Number(defaultNum);
    var tryRet = new Number(inputNum);
    if (tryRet.toString() !== "NaN" && tryRet !== Int32MinVal)
    {
        ret = tryRet;
    }
    return ret;
}

function tryParseElement(elementId, $context)
{
    /// <summary>
    ///   Attempts to fetch an element from the DOM first through jQuery, then through JavaScript
    /// </summary>
    /// <param name="elementId" type="String"> ElementId to find </param>
    /// <param name="$context" type="jQuery"> Optional context to limit the search </param>
    /// <returns type="jQuery">jQuery object, empty if no match found.</returns>
    var $ret = $('');
    if (!isNullOrEmpty(elementId))
    {
        if (arguments.length == 2 && !isNullOrEmpty($context))
        {
            $ret = $('#' + elementId, $context);
        }
        else
        {
            $ret = $('#' + elementId);
        }

        if ($ret.length === 0)
        {
            $ret = $(document.getElementById(elementId));
        }
        if ($ret.length === 0)
        {
            $ret = $(document.getElementsByName(elementId));
        }
    }
    return $ret;
}

function trim(str)
{
    /// <summary>Returns a string without left and right whitespace</summary>
    /// <param name="str" type="String"> String to parse </param>
    /// <returns type="String">Parsed string</returns>
    return $.trim(str);
}

// ------------------------------------------------------------------------------------
// for debug
// ------------------------------------------------------------------------------------
function iterate(obj)
{
    var str;
    for (var x in obj)
    {
        str = str + x + "=" + obj[x] + "<br><br>";
    }
    var popup = window.open("", "popup");
    if (popup !== null)
        popup.document.write(str);
    else
        console.log("iterate() error: No popup!");
}

// because IE 8 doesn't support console.log unless the console is open (*duh*)
function log(s, includeCallStack)
{
    /// <summary>Outputs a message to the console log(Webkit,FF) or an alert(IE)</summary>
    /// <param name="s" type="String"> String to output </param>
    /// <param name="includeCallStack" type="Boolean"> If true, include the callStack </param>
    var extendedLog = '';
    if (isTrue(includeCallStack))
    {
        extendedLog = getCallStack();
    }

    try
    {
    	if (!isNullOrEmpty(extendedLog))
    		console.log(s, extendedLog);
    	else
    		console.log(s);
    } catch (e)
    {
        alert(s);
        if (!isNullOrEmpty(extendedLog)) alert(extendedLog);
    }
}

function getCallStack()
{
    var stack = '';
    var callername = arguments.callee.caller.name;
    var caller = arguments.callee.caller;
    while (!isNullOrEmpty(callername))
    {
        if (callername != 'log')
        {
            stack += "Called by function " + callername + "() \n";
        }
        caller = caller.caller;
        callername = (!isNullOrEmpty(caller)) ? caller.name : '';
    }

    return stack;
}

function errorHandler(error, includeCallStack, includeLocalStorage, toCswStorage)
{
    if( hasWebStorage() && includeLocalStorage) log(localStorage);
    log('Error: ' + error.message + ' (Code ' + error.code + ')', includeCallStack, toCswStorage);
}

//#region Persistent Logging

function doLogging(value)
{
    var ret = undefined;
    if (hasWebStorage())
    {
        if (arguments.length === 1)
        {
            localStorage['doLogging'] = isTrue(value);
        }
        ret = isTrue(localStorage['doLogging']);
    }
    return ret;
}

function debugOn(value)
{
    var ret = undefined;
    if (hasWebStorage())
    {
        if (arguments.length === 1)
        {
            localStorage['debugOn'] = isTrue(value);
        }
        ret = isTrue(localStorage['debugOn']);
    }
    return ret;
}

function cacheLogInfo(logger, includeCallStack)
{
    if ( doLogging() || debug )
    {
        if (hasWebStorage())
        {
            if (undefined !== logger.setEnded) logger.setEnded();
            var logStorage = new CswStorage(sessionStorage,JSON,true);
            var log = logStorage.getItem('debuglog');
            log += logger.toHtml();

            var extendedLog = '';
            if (isTrue(includeCallStack)) {
                extendedLog = getCallStack();
            }
            if (!isNullOrEmpty(extendedLog)) {
                log += ',' + extendedLog;
            }
            logStorage.setItem('debuglog', log);
        }
    }
}

function purgeLogInfo()
{
    if (hasWebStorage())
    {
        window.sessionStorage.clear();
    }
}

//#endregion Persistent Logging

// ------------------------------------------------------------------------------------
// Browser Compatibility
// ------------------------------------------------------------------------------------

// for IE 8
if (typeof String.prototype.trim !== 'function')
{
    String.prototype.trim = function ()
    {
        return this.replace(/^\s+|\s+$/g, '');
    }
}

function hasWebStorage(localOnly)
{
    var ret = (Modernizr.localstorage && (localOnly || Modernizr.sessionstorage)); 
    return ret;
}