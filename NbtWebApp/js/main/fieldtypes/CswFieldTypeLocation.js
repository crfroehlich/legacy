/// <reference path="_CswFieldTypeFactory.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />

; (function ($) {
		
	$.fn.CswFieldTypeLocation = function (method) {

		var pluginName = 'CswFieldTypeLocation';

		var methods = {
			init: function(o) { //nodepk = o.nodeid, $xml = o.propData, onchange = o.onchange, ID = o.ID, Required = o.Required, ReadOnly = o.ReadOnly 
			
				var $Div = $(this);
				$Div.contents().remove();

				var nodeId = tryParseString(o.propData.nodeid).trim();
				var nodeKey = tryParseString(o.propData.nodekey).trim();
				var name = tryParseString(o.propData.name).trim();
				var path = tryParseString(o.propData.path).trim();
				var viewId = tryParseString(o.propData.viewid).trim();

				if(o.ReadOnly)
				{
					$Div.append(path);
				}
				else 
				{
                    var $table = $Div.CswTable('init', { ID: o.ID + '_tbl' });

					var $pathcell = $table.CswTable('cell', 1, 1);
					$pathcell.CswAttrDom('colspan', '2');
					$pathcell.append(path + "<br/>");

					var $selectcell = $table.CswTable('cell', 2, 1);
					var $selectdiv = $('<div class="locationselect" value="'+ nodeId +'"/>' )
										.appendTo($selectcell);

					var $locationtree = $('<div />').CswNodeTree('init', { 'ID': o.ID,
																			viewid: viewId,
																			nodeid: nodeId,
																			cswnbtnodekey: nodeKey,
																			onSelectNode: function (optSelect)
																			{
																				onTreeSelect($selectdiv, optSelect.nodeid, optSelect.nodename, optSelect.iconurl, o.onchange);
																			},
																			onInitialSelectNode: function(optSelect)
																			{
																				onTreeSelect($selectdiv, optSelect.nodeid, optSelect.nodename, optSelect.iconurl, function() {}); 
																			}, 
																			SelectFirstChild: false,
																			UsePaging: false,
																			IncludeInQuickLaunch: false
																		});
	
					$selectdiv.CswComboBox( 'init', {	'ID': o.ID + '_combo', 
														'TopContent': name,
														'SelectContent': $locationtree,
														'Width': '266px' 
													});

//					var $addcell = $table.CswTable('cell', 2, 2);
//					var $AddButton = $('<div />').appendTo($addcell);
//					$AddButton.CswImageButton({ ButtonType: CswImageButton_ButtonType.Add, 
//												AlternateText: "Add New",
//												onClick: function($ImageDiv) { 
//														onAdd();
//														return CswImageButton_ButtonType.None;
//													}
//												});

//                        if(o.Required)
//                        {
//                            $SelectBox.addClass("required");
//                        }
				}

			},
			save: function(o) { //($propdiv, $xml
					var $selectdiv = o.$propdiv.find('.locationselect');
					o.propData.nodeid = $selectdiv.val();
				}
		};
	
	
		function onTreeSelect($selectdiv, itemid, text, iconurl, onchange)
		{
			if(itemid === 'root') itemid = '';   // case 21046
			$selectdiv.CswComboBox( 'TopContent', text );
			if($selectdiv.val() !== itemid)
			{
				$selectdiv.val(itemid);
				onchange();
			}
			setTimeout(function() { $selectdiv.CswComboBox( 'close'); }, 300);
		}
		
//		function onAdd()
//		{
//			alert('This function has not been implemented yet.');
//		}

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
