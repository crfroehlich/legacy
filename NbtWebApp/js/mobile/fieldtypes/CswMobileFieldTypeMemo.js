﻿/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />

//#region CswMobileFieldTypeMemo

function CswMobileFieldTypeMemo(ftDef) {
	/// <summary>
	///   Memo field type. Responsible for generating prop according to Field Type rules.
	/// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
	/// <returns type="CswMobileFieldTypeMemo">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var divSuffix = '_propdiv';
    var propSuffix = '_input';
    var $content, contentDivId, elementId, propId, propName, subfields, value, gestalt;
    
    //ctor
    (function () {
        var p = { 
            propid: '',
            propname: '',
            gestalt: '',
            text: ''
        };
        if (ftDef) $.extend(p, ftDef);

        contentDivId = p.nodekey + divSuffix;
        elementId = p.propId + propSuffix;
        value = tryParseString(p.text);
        gestalt = tryParseString(p.gestalt, '');
        propId = p.propid;
        propName = p.propname;
        subfields = '';
        
        $content = ensureContent(contentDivId);
        $content.append($('<textarea name="' + elementId + '">' + value + '</textarea>'));
    })(); //ctor
        
    function applyFieldTypeLogicToContent($control) {
        
    }
    
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.applyFieldTypeLogicToContent = applyFieldTypeLogicToContent;
    this.value = value;
    this.gestalt = gestalt;
    this.contentDivId = contentDivId;
    this.propId = propId;
    this.propName = propName;
    this.subfields = subfields;
    this.fieldType = CswFieldTypes.Memo;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeMemo