/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) { /// <param name="$" type="jQuery" />
        
    $.fn.CswFieldTypeRelationship = function (method) {

        var pluginName = 'CswFieldTypeRelationship';

        var methods = {
            init: function(o) { //nodepk = o.nodeid, o.propData = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly , cswnbtnodekey
            
                var $Div = $(this);

                var propVals = o.propData.values;
                
                var selectedNodeId = tryParseString(propVals.nodeid).trim();
                if (!isNullOrEmpty(o.relatednodeid) && isNullOrEmpty(selectedNodeId)) {
                    selectedNodeId = o.relatednodeid;
                }
                var selectedName = tryParseString(propVals.name).trim();
                var nodeTypeId = tryParseString(propVals.nodetypeid).trim();
                var allowAdd = isTrue(propVals.allowadd);
                var options = propVals.options;

                if(o.ReadOnly) {
                    $Div.append(selectedName);
                } 
				else 
				{
                    var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });

                    var $selectcell = $table.CswTable('cell', 1, 1);
                    var $SelectBox = $('<select id="'+ o.ID +'" name="'+ o.ID +'" class="selectinput" />"' )
                                        .appendTo($selectcell)
                                        .change(o.onchange);

                    for (var optId in options) {
                        if (options.hasOwnProperty(optId)) {
                            var optVal = options[optId];
                            $SelectBox.append('<option value="' + optId + '">' + optVal + '</option>');
                        }
                    }

                    $SelectBox.val( selectedNodeId );
                    
                    if (!isNullOrEmpty(nodeTypeId) && allowAdd) {
						var $addcell = $table.CswTable('cell', 1, 2);
						var $AddButton = $('<div />').appendTo($addcell);
						$AddButton.CswImageButton({ ButtonType: CswImageButton_ButtonType.Add, 
													AlternateText: "Add New",
													onClick: function($ImageDiv) { 
															$.CswDialog('AddNodeDialog', {
																							'nodetypeid': nodeTypeId, 
																							'onAddNode': function(nodeid, cswnbtnodekey) { o.onReload(); }
																						});
															return CswImageButton_ButtonType.None;
														}
													});
					}

                    if (o.Required) {
                        $SelectBox.addClass("required");
                    }
                }

				var $nodepreview = undefined;
				$Div.hover(function(event) { onHoverIn(event, selectedNodeId); }, onHoverOut);
				
				function onHoverIn(event, selectedNodeId) 
				{
					$nodepreview = $.CswNodePreview('open', {
									ID: selectedNodeId + "_preview",
									nodeid: selectedNodeId, 
									eventArg: event
								});
				}
				function onHoverOut(event) 
				{
					if($nodepreview !== undefined)
					{
						$nodepreview.CswNodePreview('close');
						$nodepreview = undefined;
					}
				}

            },
            save: function(o) {
                    var $SelectBox = o.$propdiv.find('select');
                    o.propData.values.nodeid = $SelectBox.val();
                }
        };
    
        // Method calling logic
        if ( methods[method] ) {
            return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
        } else if ( typeof method === 'object' || ! method ) {
            return methods.init.apply( this, arguments );
        } else {
            $.error( 'Method ' +  method + ' does not exist on ' + pluginName );
        }    
  
    };
})(jQuery);
