/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../../Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="../controls/CswSelect.js" />

(function ($) { /// <param name="$" type="jQuery" />
    "use strict";
    $.fn.CswFieldTypeRelationship = function (method) {

        var pluginName = 'CswFieldTypeRelationship';

        var methods = {
            init: function (o) {

                var $Div = $(this),
                    propVals = o.propData.values,
                    selectedNodeId = (false === o.Multi) ? tryParseString(propVals.nodeid).trim() : CswMultiEditDefaultValue,
                    selectedName = (false === o.Multi) ? tryParseString(propVals.name).trim() : CswMultiEditDefaultValue,
                    nodeTypeId = tryParseString(propVals.nodetypeid).trim(),
                    allowAdd = isTrue(propVals.allowadd),
                    options = propVals.options,
                    relationships = [];

                if (false === isNullOrEmpty(o.relatednodeid) && isNullOrEmpty(selectedNodeId) && false === o.Multi) {
                    selectedNodeId = o.relatednodeid;
                }

                if (o.Multi) {
                    relationships.push({ value: CswMultiEditDefaultValue, display: CswMultiEditDefaultValue });
                }
                crawlObject(options, function (relatedObj) {
                    relationships.push({ value: relatedObj.id, display: relatedObj.value });
                }, false);

                if (o.ReadOnly) {
                    $Div.append(selectedName);
                    $Div.hover(function(event) { nodeHoverIn(event, selectedNodeId); }, nodeHoverOut);
                } else {
                    var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });

                    var $selectcell = $table.CswTable('cell', 1, 1);

                    var $SelectBox = $selectcell.CswSelect('init', {
                        ID: o.ID,
                        cssclass: 'selectinput',
                        onChange: o.onchange,
                        values: relationships,
                        selected: selectedNodeId
                    });

                    if (false === isNullOrEmpty(nodeTypeId) && allowAdd) {
                        var $addcell = $table.CswTable('cell', 1, 2);
                        var $AddButton = $('<div />').appendTo($addcell);
                        $AddButton.CswImageButton({ ButtonType: CswImageButton_ButtonType.Add,
                            AlternateText: "Add New",
                            onClick: function () {
                                $.CswDialog('AddNodeDialog', {
                                    'nodetypeid': nodeTypeId,
                                    'onAddNode': function () { o.onReload(); }
                                });
                                return CswImageButton_ButtonType.None;
                            }
                        });
                    }

                    if (o.Required) {
                        $SelectBox.addClass("required");
                    }

                    $Div.hover(function(event) { nodeHoverIn(event, $SelectBox.val()); }, nodeHoverOut);
                }
            },
            save: function (o) {
                var attributes = {
                    nodeid: null
                };
                var $nodeid = o.$propdiv.find('select');
                if (false === isNullOrEmpty($nodeid)) {
                    attributes.nodeid = $nodeid.val();
                }
                preparePropJsonForSave(o.Multi, o.propData, attributes);
            }
        };

        // Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName); return false;
        }

    };
})(jQuery);
