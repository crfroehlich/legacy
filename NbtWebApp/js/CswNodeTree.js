﻿; (function ($) {
	
	var PluginName = 'CswNodeTree';

	var methods = {
		'init': function(options)     // options are defined in _getTreeContent()
			{    
				var o = {
					ID: '',
					ViewTreeUrl: '/NbtWebApp/wsNBT.asmx/getTreeOfView',
					NodeTreeUrl: '/NbtWebApp/wsNBT.asmx/getTreeOfNode',
					viewid: '',       // loads an arbitrary view
					viewmode: '',
					nodeid: '',       // if viewid is not supplied, loads a view of this node
					cswnbtnodekey: '',
					onSelectNode: function(optSelect) {
											var o =  {
												nodeid: '', 
												nodename: '', 
												iconurl: '', 
												cswnbtnodekey: '',
												viewid: ''
											};
									},
					SelectFirstChild: true
				};
				if(options) $.extend(o, options);

				var IDPrefix = o.ID + '_';
				var $treediv = $('<div id="'+ IDPrefix + '" class="treediv" />')
								.appendTo($(this));

				var url = o.ViewTreeUrl;
				var data = 'ViewNum=' + o.viewid + '&IDPrefix=' + IDPrefix + '&ParentNodeKey=';
				if(o.viewid == '' || o.viewid == undefined)
				{
					url = o.NodeTreeUrl;
					data = 'IDPrefix=' + IDPrefix + '&NodePk=' + o.nodeid;
				}

				CswAjaxXml({
					url: url,
					data: data,
					success: function ($xml) {
						var selectid = '';
						var treePlugins = ["themes", "xml_data", "ui", "types"];
						var treeThemes;
						if(o.nodeid != undefined && o.nodeid != '') 
						{
							selectid = IDPrefix + o.nodeid;
						}

						var $selecteditem = $xml.find('item[id="'+ selectid + '"]');
						if(selectid == '' || $selecteditem.length == 0)
						{
							if(o.SelectFirstChild)
							{	
								if(o.viewmode == 'list' )
								{
									selectid = $xml.find('item').first().attr('id');
									treeThemes = {"dots": false};
								}
								else
								{
									selectid = $xml.find('item').first().find('item').first().attr('id');
									treeThemes = {"dots": true};
								}
							}
							else
							{
								selectid = IDPrefix + 'root';
							}
						}

						// make sure selected item is visible
						var $selecteditem = $xml.find('item[id="'+ selectid + '"]');
						var $itemparents = $selecteditem.parents('item').andSelf();
						var initiallyOpen = new Array();
						var i = 0;
						$itemparents.each(function() { initiallyOpen[i] = $(this).attr('id'); i++; });

						var strTypes = $xml.find('types').text();
						var jsonTypes = $.parseJSON(strTypes);
						var $treexml = $xml.find('tree').children('root')
						if($treexml.length > 0)
						{
							var treexmlstring = xmlToString($treexml);
				
							$treediv.jstree({
								"xml_data": 
									{
										"data": treexmlstring,
										"xsl": "nest",
										"ajax": 
											{
												"type": 'POST',
												"url": url,
												"dataType": "xml",
												"data": function($nodeOpening) 
													{
														var nodekey = $nodeOpening.attr('cswnbtnodekey');
														return data + nodekey;
													}
											}
									},
								"ui": {
									"select_limit": 1,
									"initially_select": selectid
								},
								"themes": treeThemes,
								"core": {
									"initially_open": initiallyOpen
								},
								"types": {
									"types": jsonTypes
								},
								"plugins": treePlugins
							}).bind('select_node.jstree', 
											function (e, data) {
												var Selected = jsTreeGetSelected($treediv, IDPrefix);
												var optSelect =  {
													nodeid: Selected.id, 
													nodename: Selected.text, 
													iconurl: Selected.iconurl, 
													cswnbtnodekey: Selected.$item.attr('cswnbtnodekey'),
													viewid: o.viewid
												};

												if(optSelect.nodename == "More...")
												{
													// get next page of nodes

												}
												else 
												{
													o.onSelectNode(optSelect);
												}
											});
							// DO NOT define an onSuccess() function here that interacts with the tree.
							// The tree has initalization events that appear to happen asynchronously,
							// and thus having an onSuccess() function that changes the selected node will
							// cause a race condition.
						} else {
							$treediv.append('No Results');
						}

					} // success{}
				}); // ajax

				return $treediv;
			},

		'selectNode': function(optSelect) 
			{
				var o = {
					newnodeid: '', 
					newcswnbtnodekey: ''
				}
				if (optSelect) {
					$.extend(o, optSelect);
				}
				var $treediv = $(this).children('.treediv')
				var IDPrefix = $treediv.attr('id');
				$treediv.jstree('select_node', '#' + IDPrefix + o.newnodeid);
			}
	};

	// Method calling logic
	$.fn.CswNodeTree = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};

})(jQuery);

