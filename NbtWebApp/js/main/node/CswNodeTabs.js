﻿/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../fieldtypes/_CswFieldTypeFactory.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	$.fn.CswNodeTabs = function (options)
	{

		var o = {
			ID: '',
			TabsUrl: '/NbtWebApp/wsNBT.asmx/getTabs',
			SinglePropUrl: '/NbtWebApp/wsNBT.asmx/getSingleProp',
			PropsUrl: '/NbtWebApp/wsNBT.asmx/getProps',
			MovePropUrl: '/NbtWebApp/wsNBT.asmx/moveProp',
			SavePropUrl: '/NbtWebApp/wsNBT.asmx/saveProps',
			CopyPropValuesUrl: '/NbtWebApp/wsNBT.asmx/copyPropValues',
			NodePreviewUrl: '/NbtWebApp/wsNBT.asmx/getNodePreview',
			nodeid: '',               
			relatednodeid: '',
			tabid: '',                
			cswnbtnodekey: '',        
			nodetypeid: '',           
			filterToPropId: '',       
			title: '',
			date: '',      // for audit records
			EditMode: EditMode.Edit.name, // Edit, AddInPopup, EditInPopup, Demo, PrintReport, DefaultValue, NodePreview
			onSave: null, // function (nodeid, cswnbtnodekey, tabcount) { },
			onBeforeTabSelect: null, // function (tabid) { return true; },
			onTabSelect: null, // function (tabid) { },
			onPropertyChange: null, // function(propid, propname) { },
			onInitFinish: null, // function() { },
			ShowCheckboxes: false,
			ShowAsReport: true,
			NodeCheckTreeId: '',
			onEditView: null, // function(viewid) { }
			Config: false
		};

		if (options)
		{
			$.extend(o, options);
		}
		var $parent = $(this);

		var $outertabdiv = $('<div id="' + o.ID + '_tabdiv" />')
						.appendTo($parent);
		
		var tabcnt = 0;

		getTabs(o);

		if(o.EditMode !== EditMode.PrintReport.name)
		{
			var $linkdiv = $('<div id="' + o.ID + '_linkdiv" align="right"/>')
							.appendTo($parent);
			if(o.ShowAsReport)
			{
				var $AsReportLink = $('<a href="#">As Report</a>')
								.appendTo($linkdiv)
								.click(function() { 
									openPopup('NodeReport.html?nodeid=' + o.nodeid + '&cswnbtnodekey=' + o.cswnbtnodekey, 600, 800); 
								});
			}
		}

		function clearTabs()
		{
			$outertabdiv.contents().remove();
		}

		function getTabs()
		{
			var jsonData = {
				EditMode: o.EditMode,
				NodeId: o.nodeid,
				SafeNodeKey: o.cswnbtnodekey,
				NodeTypeId: o.nodetypeid,
				Date: o.date,
				filterToPropId: o.filterToPropId
			};
			CswAjaxJson({
				url: o.TabsUrl,
				data: jsonData,
				success: function (data)
				{
				    clearTabs();
					var tabdivs = [];
					var selectedtabno = 0;
					var tabno = 0;

					for (var tabId in data) {
					    if (data.hasOwnProperty(tabId)) {
					        var thisTab = data[tabId];
					        var thisTabId = thisTab.id;
					        if (o.EditMode === 'PrintReport' || tabdivs.length === 0)
					        {
					            // For PrintReports, we're going to make a separate tabstrip for each tab
					            tabdivs[tabdivs.length] = $("<div><ul></ul></div>").appendTo($outertabdiv);
					        }
					        var $tabdiv = tabdivs[tabdivs.length - 1];
					        $tabdiv.children('ul').append('<li><a href="#' + thisTabId + '">' + thisTab.name + '</a></li>');
					        var $tabcontentdiv = $('<div id="' + thisTabId + '"><form onsubmit="return false;" id="' + thisTabId + '_form" /></div>')
    					        .appendTo($tabdiv);
					        $tabcontentdiv.data('canEditLayout', thisTab.canEditLayout);
					        if (thisTabId === o.tabid)
					        {
					            selectedtabno = tabno;
					        }
					        tabno++;
					    }
					} // for (var tabId in data) {

					tabcnt = tabno;

					for(var t in tabdivs)
					{
						var $tabdiv = tabdivs[t];
						$tabdiv.tabs({
							'selected': selectedtabno,
							'select': function (event, ui)
							{
							    if (isFunction(o.onBeforeTabSelect) && o.onBeforeTabSelect(tabid)) {
									var $tabcontentdiv = $($tabdiv.children('div')[ui.index]);
									var tabid = $tabcontentdiv.CswAttrDom('id');
									getProps($tabcontentdiv, tabid);
                                    if (isFunction(o.onTabSelect)) {
                                        o.onTabSelect(tabid);
                                    }
							    } else {
									return false;
								}
							}
						});
						var $tabcontentdiv = $($tabdiv.children('div')[$tabdiv.tabs('option', 'selected')]);
						var selectedtabid = $tabcontentdiv.CswAttrDom('id');
						getProps($tabcontentdiv, selectedtabid);
						if (isFunction(o.onTabSelect)) o.onTabSelect(selectedtabid);
					} // for(var t in tabdivs)

				} // success{}
			}); // ajax
		} // getTabs()

		function getProps($tabcontentdiv, tabid)
		{
			var jsonData = {
				EditMode: o.EditMode,
				NodeId: o.nodeid,
				TabId: tabid, 
				SafeNodeKey: o.cswnbtnodekey,
				NodeTypeId: o.nodetypeid,
				Date: o.date
			};

			CswAjaxJson({
				url: o.PropsUrl,
				data: jsonData,
				success: function (data)
				{
				    var $form = $tabcontentdiv.children('form');
					$form.contents().remove();

					if(o.title !== '')
						$form.append(o.title);

					var $formtbl = $form.CswTable('init', { ID: o.ID + '_formtbl', width: '100%' });
					var $formtblcell11 = $formtbl.CswTable('cell', 1, 1);
					var $formtblcell12 = $formtbl.CswTable('cell', 1, 2);

					var $savetab;
					var $layouttable = $formtblcell11.CswLayoutTable('init', {
						ID: o.ID + '_props',
						OddCellRightAlign: true,
						ReadOnly: (o.EditMode === 'PrintReport'),
						cellset: {
							rows: 1,
							columns: 2
						},
						onSwap: function (e, onSwapData)
						{
							onSwap(onSwapData);
						},
						showConfigButton: false,
						onConfigOn: function() { 
							for (var prop in data) {
							    if (data.hasOwnProperty(prop)) {
							        var thisProp = data[prop];
							        var propId = thisProp.id;
							        var $subtable = $layouttable.find('#' + propId + '_subproptable');
							        var $parentcell = $subtable.parent().parent();
							        var $cellset = $layouttable.CswLayoutTable('cellset', $parentcell.CswAttrDom('row'), $parentcell.CswAttrDom('column'));
							        var $propcell = _getPropertyCell($cellset);

							        if ($subtable.length > 0)
							        {
							            var fieldOpt = {
							                fieldtype: thisProp.fieldtype,
							                nodeid: o.nodeid,
							                relatednodeid: o.relatednodeid,
							                propid: propId,
							                $propdiv: $propcell.children('div'),
							                propData: thisProp,
							                onchange: function() { },
							                onReload: function() { getProps($tabcontentdiv, tabid); },
							                cswnbtnodekey: o.cswnbtnodekey
							            };

							            _updateSubProps(fieldOpt, o.SinglePropUrl, o.EditMode, o.cswnbtnodekey, propId, o.nodetypeid, thisProp, $propcell, $tabcontentdiv, tabid, true, $savetab);
							        }
							    }
							} // for (var prop in data) {
						}, // onConfigOn
						onConfigOff: function() { 
							for (var prop in data) {
							    if (data.hasOwnProperty(prop)) {
							        var thisProp = data[prop];
							        var propId = thisProp.id;
							        var $subtable = $layouttable.find('#' + propId + '_subproptable');
							        var $parentcell = $subtable.parent().parent();
							        var $cellset = $layouttable.CswLayoutTable('cellset', $parentcell.CswAttrDom('row'), $parentcell.CswAttrDom('column'));
							        var $propcell = _getPropertyCell($cellset);

							        if ($subtable.length > 0)
							        {
							            var fieldOpt = {
							                fieldtype: thisProp.fieldtype,
							                nodeid: o.nodeid,
							                relatednodeid: o.relatednodeid,
							                propid: propId,
							                $propdiv: $propcell.children('div'),
							                propData: thisProp,
							                onchange: function() { },
							                onReload: function() { getProps($tabcontentdiv, tabid); },
							                cswnbtnodekey: o.cswnbtnodekey
							            };

							            _updateSubProps(fieldOpt, o.SinglePropUrl, o.EditMode, o.cswnbtnodekey, propId, o.nodetypeid, thisProp, $propcell, $tabcontentdiv, tabid, false, $savetab);
							        }
						        }
							} // for (var prop in data) {
						} // onConfigOff

					}); // CswLayoutTable()

					var i = 0;

					if(o.EditMode !== EditMode.PrintReport.Name)
					{
						$savetab = $formtblcell11.CswButton({ID: 'SaveTab', 
												enabledText: 'Save Changes', 
												disabledText: 'Saving...', 
												onclick: function () { Save($form, $layouttable, data, $savetab, tabid); }
												});
					}
					var AtLeastOneSaveable = _handleProps($layouttable, data, $tabcontentdiv, tabid, false, $savetab);

					// Validation
					$form.validate({
						highlight: function (element)
						{
							var $elm = $(element);
							$elm.CswAttrDom('csw_invalid', '1');
							$elm.animate({ backgroundColor: '#ff6666' });
						},
						unhighlight: function (element)
						{
							var $elm = $(element);
							if($elm.CswAttrDom('csw_invalid') === '1')  // only unhighlight where we highlighted
							{
								$elm.css('background-color', '#66ff66');
							    $elm.CswAttrDom('csw_invalid', '0');
								setTimeout(function () { $elm.animate({ backgroundColor: 'transparent' }); }, 500);
							}
						}
					}); // validate()

					if(isTrue(o.Config))
					{
						$layouttable.CswLayoutTable('ConfigOn');
					} 
					else if(!o.Config && 
						isNullOrEmpty(o.date) && 
						o.filterToPropId === '' && 
						isTrue($tabcontentdiv.data('canEditLayout')))
					{
						// Show the 'fake' config button to open the dialog
						$formtblcell12.CswImageButton({
													ButtonType: CswImageButton_ButtonType.Configure,
													AlternateText: 'Configure',
													ID: o.ID + 'configbtn',
													onClick: function ($ImageDiv) 
													{ 
														$.CswDialog('EditLayoutDialog', o);
														return CswImageButton_ButtonType.None; 
													}
												});
					}


					// case 8494
					if (!AtLeastOneSaveable && o.EditMode == EditMode.AddInPopup.name) {
						Save($form, $layouttable, data, $savetab, tabid);
					} else if (isFunction(o.onInitFinish)) {
						o.onInitFinish();
					}
				} // success{}
			}); // ajax
		} // getProps()

		function onSwap(onSwapData)
		{
			_moveProp(_getPropertyCell(onSwapData.cellset).children('div').first(), onSwapData.swaprow, onSwapData.swapcolumn);
			_moveProp(_getPropertyCell(onSwapData.swapcellset).children('div').first(), onSwapData.row, onSwapData.column);
		} // onSwap()

		function _moveProp($propdiv, newrow, newcolumn)
		{
			if ($propdiv.length > 0)
			{
				var propid = $propdiv.CswAttrDom('propid');

				var dataJson = { 
					PropId: propid, 
					NewRow: newrow, 
					NewColumn: newcolumn, 
					EditMode: o.EditMode
				};

				CswAjaxJson({
					url: o.MovePropUrl,
					data: dataJson,
					success: function (result)
					{

					}
				});
			}
		} // _moveProp()

		function _getLabelCell($cellset)
		{
			return $cellset[1][1].children('div');
		}
		function _getPropertyCell($cellset)
		{
			return $cellset[1][2].children('div');
		}

		function _handleProps($layouttable, data, $tabcontentdiv, tabid, configMode, $savebtn)
		{
			var AtLeastOneSaveable = false;
			for (var prop in data) {
				if (data.hasOwnProperty(prop)) {
                    var thisProp = data[prop];
				    var propid = thisProp.id;
				    var fieldtype = thisProp.fieldtype;
				    var $cellset = $layouttable.CswLayoutTable('cellset', thisProp.displayrow, thisProp.displaycol);

				    if ((isTrue(thisProp.display, true) || configMode) &&
    				    fieldtype !== 'Image' &&
        				    fieldtype !== 'Grid' &&
            				    (o.filterToPropId === '' || o.filterToPropId === propid))
				    {
				        var $labelcell = _getLabelCell($cellset);
				        $labelcell.addClass('propertylabel');

				        if (isTrue(thisProp.highlight))
				        {
				            $labelcell.addClass('ui-state-highlight');
				        }

				        var helpText = tryParseString(thisProp.helptext);
				        var propName = tryParseString(thisProp.name);
				        if (!isNullOrEmpty(helpText)) {
				            $('<a href="#" class="cswprop_helplink" title="' + helpText + '" onclick="return false;">' + propName + '</a>')
    				            .appendTo($labelcell);
				        } else {
				            $labelcell.append(propName);
				        }

				        if (!isTrue(thisProp.readonly))
				        {
				            AtLeastOneSaveable = true;
				            if (o.ShowCheckboxes && isTrue(thisProp.copyable)) {
				                var $propcheck = $labelcell.CswInput('init', {ID: 'check_' + propid,
				                    type: CswInput_Types.checkbox,
				                    value: false, // Value --not defined?,
				                    cssclass: o.ID + '_check'
				                });
				                $propcheck.CswAttrDom('propid', propid);
				            }
				        }
				    }

				    var $propcell = _getPropertyCell($cellset);
				    $propcell.addClass('propertyvaluecell');

				    if (isTrue(thisProp.highlight)) {
				        $propcell.addClass('ui-state-highlight');
				    }
				    _makeProp($propcell, thisProp, $tabcontentdiv, tabid, configMode, $savebtn);
				}
			}

			if(AtLeastOneSaveable === false && o.EditMode != EditMode.AddInPopup.name) {
				$savebtn.hide();
			} else {
				$savebtn.show();
			}
			return AtLeastOneSaveable;
		} // _handleProps()

		function _makeProp($propcell, propData, $tabcontentdiv, tabid, configMode, $savebtn)
		{
			$propcell.empty();
			if ((isTrue(propData.display, true) || configMode ) &&
				(o.filterToPropId === '' || o.filterToPropId === propData.id)) {

			    var propId = propData.id;
			    var propName = propData.name;
			    
			    var fieldOpt = {
					fieldtype: propData.fieldtype,
					nodeid: o.nodeid,
					relatednodeid: o.relatednodeid,
					propid: propId,
					$propdiv: $('<div/>').appendTo($propcell),
					$savebtn: $savebtn,
					propData: propData,
					onchange: function() { },
					onReload: function() { getProps($tabcontentdiv, tabid); },
					cswnbtnodekey: o.cswnbtnodekey,
					EditMode: o.EditMode,
					onEditView: o.onEditView,
					ReadOnly: isTrue(propData.readonly)
				};
				fieldOpt.$propdiv.CswAttrDom('nodeid', fieldOpt.nodeid);
				fieldOpt.$propdiv.CswAttrDom('propid', fieldOpt.propid);
				fieldOpt.$propdiv.CswAttrDom('cswnbtnodekey', fieldOpt.cswnbtnodekey);

				fieldOpt.onchange = function () { if(isFunction(o.onPropertyChange)) o.onPropertyChange(fieldOpt.propid, propName); };
				if (isTrue(propData.hassubprops)) {
					fieldOpt.onchange = function ()
					{
						_updateSubProps(fieldOpt, o.SinglePropUrl, o.EditMode, o.cswnbtnodekey, propId, o.nodetypeid, propData, $propcell, $tabcontentdiv, tabid, false, $savebtn);
						if(isFunction(o.onPropertyChange)) o.onPropertyChange(fieldOpt.propid, propName);
					};
				} // if (propData.hassubprops === "true")

				$.CswFieldTypeFactory('make', fieldOpt);

				// recurse on sub-props
				var subProps = propData.subprops;

				var $subtable = $propcell.CswLayoutTable('init', {
					ID: fieldOpt.propid + '_subproptable',
					OddCellRightAlign: true,
					ReadOnly: (o.EditMode === 'PrintReport'),
					cellset: {
						rows: 1,
						columns: 2
					},
					onSwap: function (e, onSwapData)
					{
						onSwap(onSwapData);
					},
					showConfigButton: false
				});

				if ((!isNullOrEmpty(subProps) && isTrue(subProps.display)) || configMode)
				{
					_handleProps($subtable, subProps, $tabcontentdiv, tabid, configMode, $savebtn);
					if (configMode) {
						$subtable.CswLayoutTable('ConfigOn');
					} else {
						$subtable.CswLayoutTable('ConfigOff');
					}
				}
			} // if (propData.display != 'false' || ConfigMode )
		} // _makeProp()

		function _updateSubProps(fieldOpt, singlePropUrl, editMode, cswnbtnodekey, propId, nodetypeid, propData, $propcell, $tabcontentdiv, tabid, configMode, $savebtn)
		{
			// do a fake 'save' to update the xml with the current value
			$.CswFieldTypeFactory('save', fieldOpt);

			// update the propxml from the server
			var jsonData = {
				EditMode: editMode,
				NodeId: o.nodeid,
				SafeNodeKey: cswnbtnodekey,
				PropId: propId,
				NodeTypeId: nodetypeid,
				NewPropJson: JSON.stringify(propData)
			};

			CswAjaxJson({
				url: singlePropUrl,
				data: jsonData,
				success: function (data)
				{
				    _makeProp($propcell, data["prop_" + propId], $tabcontentdiv, tabid, configMode, $savebtn );
				}
			});
		} // _updateSubProps()

		function Save($form, $layouttable, propsData, $savebtn, tabid)
		{
			if($form.valid())
			{
				_updatePropJsonFromForm($layouttable, propsData);
				var data = {
					EditMode: o.EditMode,
					NodeId: o.nodeid,
					SafeNodeKey: o.cswnbtnodekey,
					TabId: tabid,
					NodeTypeId: o.nodetypeid,
					NewPropsJson: JSON.stringify(propsData),
					ViewId: $.CswCookie('get', CswCookieName.CurrentViewId)
				   };

				CswAjaxJson({
					url: o.SavePropUrl,
					//data: "{ EditMode: '" + o.EditMode + "', SafeNodeKey: '" + o.cswnbtnodekey + "', NodeTypeId: '" + o.nodetypeid + "', ViewId: '"+ $.CswCookie('get', CswCookieName.CurrentView.ViewId) +"', NewPropsXml: '" + safeJsonParam(xmlToString($propsxml)) + "' }",
					data: data,
					success: function (data)
					{
                        if (debugOn()) {
                            log('CswNodeTabs_Save()');
                            log(data);
                        }
					    var doSave = true;
						if(o.ShowCheckboxes)
						{
							// apply the newly saved checked property values on this node to the checked nodes
							var $nodechecks = $('.' + o.NodeCheckTreeId + '_check:checked');
							var $propchecks = $('.' + o.ID + '_check:checked');
							if($nodechecks.length > 0 && $propchecks.length > 0)
							{
								var dataJson = {
									SourceNodeKey: o.cswnbtnodekey,
									CopyNodeIds: [],
									PropIds: []
								};
								
								$nodechecks.each(function() { 
									var nodeid = $(this).CswAttrDom('nodeid');
									dataJson.CopyNodeIds.push(nodeid); 
								});

								$propchecks.each(function() { 
									var propid = $(this).CswAttrDom('propid');
									dataJson.PropIds.push(propid);
								});

								CswAjaxJson({
									url: o.CopyPropValuesUrl,
									data: dataJson
								}); // ajax
							} // if($nodechecks.length > 0 && $propchecks.length > 0)
							else
							{
								doSave = false;
								alert('You have not selected any properties to save.');
							}
						} // if(o.ShowCheckboxes)
						if (isFunction(o.onSave) && doSave) o.onSave(data.nodeid, data.cswnbtnodekey, tabcnt);
						$savebtn.CswButton('enable');
					}, // success
					error: function()
					{
						$savebtn.CswButton('enable');
					}
				}); // ajax
			} // if($form.valid())
			else 
			{
				$savebtn.CswButton('enable');
			}
		} // Save()

		function _updatePropJsonFromForm($layouttable, propData)
		{
			for (var prop in propData) {
				if (propData.hasOwnProperty(prop)) {
				    var thisProp = propData[prop];
				    var propOpt = {
				        propData: thisProp,
				        $propdiv: '',
				        $propCell: '',
				        fieldtype: thisProp.fieldtype,
				        nodeid: o.nodeid,
				        cswnbtnodekey: o.cswnbtnodekey
				    };
				    
				    var $cellset = $layouttable.CswLayoutTable('cellset', thisProp.displayrow, thisProp.displaycol);
				    propOpt.$propcell = _getPropertyCell($cellset);
				    propOpt.$propdiv = propOpt.$propcell.children('div').first();

				    $.CswFieldTypeFactory('save', propOpt);

				    // recurse on subprops
				    if ( isTrue(thisProp.hassubprops)) {
				        var subProps = thisProp.subprops;
				        if (!isNullOrEmpty(subProps)) { //&& $subprops.children('[display != "false"]').length > 0)
				            var $subtable = propOpt.$propcell.children('#' + thisProp.id + '_subproptable').first();
				            if ($subtable.length > 0)
				            {
				                _updatePropJsonFromForm($subtable, subProps);
				            }
				        }
				    }
				}
			} // each()
		} // _updatePropXmlFromForm()

		// For proper chaining support
		return this;

	}; // function(options) {
})(jQuery);

