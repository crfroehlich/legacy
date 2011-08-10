﻿/// <reference path="../../_Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../jquery/common/CswAttr.js" />
/// <reference path="../CswMobileTools.js" />
/// <reference path="../../CswEnums.js" />

//#region CswMobileFieldTypeTime

function CswMobileFieldTypeTime(ftDef) {
	/// <summary>
	///   Time field type. Responsible for generating prop according to Field Type rules.
	/// </summary>
    /// <param name="ftDef" type="Object">Field Type definitional data.</param>
	/// <returns type="CswMobileFieldTypeTime">Instance of itself. Must instance with 'new' keyword.</returns>

	//#region private

    var divSuffix = '_propdiv';
    var propSuffix = '_input';
    var $content, contentDivId, elementId, propId, propName, subfields, value, gestalt;
    
    //ctor
    (function () {
        var p = { 
            propId: '',
            propName: '',
            gestalt: '',
            value: ''
        };
        if (ftDef) $.extend(p, ftDef);

        propId = p.propId;
        propName = p.propName;
        contentDivId = propId + divSuffix;
        elementId = propId + propSuffix;
        
        subfields = CswFieldTypes.Time.subfields;
        value = tryParseString(p[subfields.Value.subfield.name]);
        gestalt = tryParseString(p.gestalt, '');
        
        $content = ensureContent(contentDivId);
        $content.CswInput('init', { type: CswInput_Types.time, ID: elementId, value: value });
    })(); //ctor
        
    function applyFieldTypeLogicToContent($control) {
        
    }
    
    function updatePropValue(json,id,newValue) {
        json = modifyPropJson(json, subfields.Value.subfield.name, newValue);
        return json;
    }    
    
	//#endregion private
    
    //#region public, priveleged

    this.$content = $content;
    this.applyFieldTypeLogicToContent = applyFieldTypeLogicToContent;
    this.updatePropValue = updatePropValue;
    this.value = value;
    this.gestalt = gestalt;
    this.contentDivId = contentDivId;
    this.propId = propId;
    this.propName = propName;
    this.subfields = subfields;
    this.fieldType = CswFieldTypes.Time;
    
    //#endregion public, priveleged
}

//#endregion CswMobileFieldTypeTime