﻿// ------------------------------------------------------------------------------------
// Enum definitions
// ------------------------------------------------------------------------------------

// For CswImageButton
var CswImageButton_ButtonType = {
    None: -1,
    Add: 27,
    ArrowNorth: 28,
    ArrowEast: 29,
    ArrowSouth: 30,
    ArrowWest: 31,
    Calendar: 6,
    CheckboxFalse: 18,
    CheckboxNull: 19,
    CheckboxTrue: 20,
    Clear: 4,
    Clock: 10,
    ClockGrey: 11,
    Configure: 26,
    Delete: 4,
    Edit: 3,
    Fire: 5,
    PageFirst: 23,
    PagePrevious: 24,
    PageNext: 25,
    PageLast: 22,
    PinActive: 17,
    PinInactive: 15,
    Print: 2,
    Refresh: 9,
    SaveStatus: 13,
    Select: 32,
    ToggleActive: 1,
    ToggleInactive: 0,
    View: 8
}

// ------------------------------------------------------------------------------------
// Cookies
// ------------------------------------------------------------------------------------

//function SetSessionId(SessionId) {
//    $.cookie('CswSessionId', SessionId);
//}
function GetSessionId() {
    return $.cookie('CswSessionId');
}
function ClearSessionId() {
    $.cookie('CswSessionId', null);
}

function SetUsername(Username) {
    $.cookie('csw_username', Username);
}
function GetUsername() {
    return $.cookie('csw_username');
}
function ClearUsername() {
    $.cookie('csw_username', null);
}

function SetCurrentViewId(ViewId) {
    $.cookie('csw_currentviewid', ViewId);
}
function GetCurrentViewId() {
    return $.cookie('csw_currentviewid');
}
function ClearCurrentViewId() {
    $.cookie('csw_currentviewid', null);
}


// ------------------------------------------------------------------------------------
// Ajax
// ------------------------------------------------------------------------------------


function CswAjaxJSON(options) {
    var o = {
        url: '',
        data: '',
        success: function (result) { }
    };

    if (options) {
        $.extend(o, options);
    }

    var starttime = new Date();
    $.ajax({
        type: 'POST',
        url: o.url,
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        data: o.data,
        success: function (data, textStatus, XMLHttpRequest) {
            var endtime = new Date();
            $('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");

            o.success($.parseJSON(data.d));

        }, // success{}
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
        }
    });      // $.ajax({
} // CswAjaxXml()

function CswAjaxXml(options) {
    var o = {
        url: '',
        data: '',
        success: function ($xml) { }
    };

    if (options) {
        $.extend(o, options);
    }

    var starttime = new Date();
    $.ajax({
        type: 'POST',
        url: o.url,
        dataType: "xml",
        //contentType: 'application/json; charset=utf-8',
        data: o.data,     // should be 'field1=value&field2=value'
        success: function (data, textStatus, XMLHttpRequest) {

            var endtime = new Date();
            $('body').append("[" + endtime.getHours() + ":" + endtime.getMinutes() + ":" + endtime.getSeconds() + "] " + o.url + " time: " + (endtime - starttime) + "ms<br>");

            // this is IE compliant
            var $xml = $(XMLHttpRequest.responseXML);
            var $realxml = $xml.children().first();
            if ($realxml.first().get(0).nodeName == "error") {
                _handleAjaxError(XMLHttpRequest, $realxml.text(), '');
            }
            else {
                o.success($realxml);
            }

        }, // success{}
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            _handleAjaxError(XMLHttpRequest, textStatus, errorThrown);
        }
    });            // $.ajax({
} // CswAjaxXml()
        
function _handleAjaxError(XMLHttpRequest, textStatus, errorThrown) 
{
    ErrorMessage = "A WebServices Error Occurred: " + textStatus;
    if (null != errorThrown) {
        ErrorMessage += "; Exception: " + errorThrown.toString()
    }
    console.log(ErrorMessage);
} // _handleAjaxError()

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

function xmlToString($xmlnode) {
    var xmlstring = $xmlnode.get(0).xml; // IE
    if (!xmlstring) {            // FF, Chrome, Safari
        var s = new XMLSerializer();
        xmlstring = s.serializeToString($xmlnode.get(0));
    }
    if (!xmlstring) {
        $.error("Browser does not support XML operations necessary to convert to string");
    }
    return xmlstring;
}

// ------------------------------------------------------------------------------------
// Node interactions
// ------------------------------------------------------------------------------------

function deleteNode(nodeid, onSuccess) {
    CswAjaxJSON({
        url: '/NbtWebApp/wsNBT.asmx/DeleteNode',
        data: '{ "NodePk":"' + nodeid + '" }',
        success: function (result) {
            onSuccess('');  // returning '' will reselect the first node in the tree
        }
    });
}


// ------------------------------------------------------------------------------------
// Layout mechanics
// ------------------------------------------------------------------------------------

function makeTable(id) 
{
    return $('<table id="'+ id +'" cellpadding="0" cellspacing="0" border="0"><tr><td></td></tr></table>');
}

// row and col are 1-based
function getTableCell($table, row, col) {
    var $cell = null;
    if ($table.length > 0 &&
         row != undefined && row != '' &&
         col != undefined && col != '') 
    {
        if (row <= 0) {
            console.log("error: row must be greater than 1, got: " + row);
            row = 1;
        }
        if (col <= 0) {
            console.log("error: col must be greater than 1, got: " + col);
            col = 1;
        }

        while (row > $table.children('tbody').children('tr').length) {
            $table.append('<tr></tr>');
        }
        var $row = $($table.children('tbody').children('tr')[row-1]);
        while (col > $row.children('td').length) {
            $row.append('<td></td>');
        }
        var $cell = $($row.children('td')[col-1]);
    }
    return $cell;
}

// ------------------------------------------------------------------------------------
// jsTree
// ------------------------------------------------------------------------------------

function jsTreeGetSelected($treediv, IDPrefix) 
{
    $SelectedItem = $treediv.jstree('get_selected');
    return { 
        'SelectedIconUrl': $SelectedItem.children('a').children('ins').css('background-image'),
        'SelectedId': $SelectedItem.attr('id').substring(IDPrefix.length),
        'SelectedText': $SelectedItem.children('a').first().text().trim()
    };
}


// ------------------------------------------------------------------------------------
// Menu
// ------------------------------------------------------------------------------------

function GoHome() 
{
    ClearCurrentViewId();
    window.location = "NewMain.html";
}

function HandleMenuItem($ul, $this, onLogout, onAlterNode) {
    var $li;
    if ($this.attr('href') != undefined && $this.attr('href') != '') {
        $li = $('<li><a href="' + $this.attr('href') + '">' + $this.attr('text') + '</a></li>')
                        .appendTo($ul)
    }
    else if ($this.attr('popup') != undefined && $this.attr('popup') != '') {
        $li = $('<li class="headermenu_dialog">' + $this.attr('text') + '</li>')
                        .appendTo($ul)
                        .click(function () { OpenDialog($this.attr('text'), $this.attr('popup')); });
    }
    else if ($this.attr('action') != undefined && $this.attr('action') != '') {
        $li = $('<li><a href="#">' + $this.attr('text') + '</a></li>')
                        .appendTo($ul);
        var $a = $li.children('a');
        switch ($this.attr('action')) {

            case 'About':
                $a.click(function () { $.CswDialog('AboutDialog'); return false; });
                break;

            case 'AddNode':
                $a.click(function () { $.CswDialog('AddNodeDialog', $this.attr('nodetypeid'), onAlterNode); return false; });
                break;

            case 'DeleteNode':
                $a.click(function () { $.CswDialog('DeleteNodeDialog', $this.attr('nodename'), $this.attr('nodeid'), onAlterNode); return false; });
                break;

            case 'Home':
                $a.click(function () { GoHome(); return false; });
                break;

            case 'Logout':
                $a.click(function () { onLogout(); return false; });
                break;
        }
    }
    else {
        $li = $('<li>' + $this.attr('text') + '</li>')
                        .appendTo($ul)
    }
    return $li;
}


// ------------------------------------------------------------------------------------
// Validation
// ------------------------------------------------------------------------------------

function validateFloatMinValue(value, minvalue) {
    var nValue = parseFloat(value);
    var nMinValue = parseFloat(minvalue);
    var isValid = true;

    if (nMinValue != undefined)
    {
        if (nValue == undefined || nValue < nMinValue) {
            isValid = false;
        }
    }
    return isValid; 
} // validateFloatMinValue()

function validateFloatMaxValue(value, maxvalue) {
    var nValue = parseFloat(value);
    var nMaxValue = parseFloat(maxvalue);
    var isValid = true;

    if (nMaxValue != undefined) {
        if (nValue == undefined || nValue > nMaxValue) {
            isValid = false;
        }
    }
    return isValid; 
} // validateFloatMaxValue()

function validateFloatPrecision(value, precision) {
    var isValid = true;

    var regex;
    if (precision > 0) {
        // Allow any valid number -- we'll round later
        regex = /^\-?\d*\.?\d*$/g;
    }
    else {
        // Integers Only
        regex = /^\-?\d*$/g;
    }
    if (isValid && !regex.test(value)) {
        isValid = false;
    }

    return isValid;
} // validateFloatPrecision()

function validateInteger(value) {
    // Integers Only
    var regex = /^\-?\d*$/g;
    return (regex.test(value));
} // validateInteger()

// ------------------------------------------------------------------------------------
// strings
// ------------------------------------------------------------------------------------

function startsWith(source, search) 
{
    return (source.substr(0, search.length) == search);
}

// ------------------------------------------------------------------------------------
// for debug
// ------------------------------------------------------------------------------------
function iterate(obj) {
    var str;
    for (var x in obj) {
        str = str + x + "=" + obj[x] + "<br><br>";
    }
    var popup = window.open("", "popup");
    if (popup != null)
        popup.document.write(str);
    else
        console.log("iterate() error: No popup!");
}



