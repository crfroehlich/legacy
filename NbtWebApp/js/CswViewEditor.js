﻿; (function ($) {

	$.fn.CswViewEditor = function (options) 
	{
		var o = {
			ViewGridUrl: '/NbtWebApp/wsNBT.asmx/getViewGrid',
			CopyViewUrl: '/NbtWebApp/wsNBT.asmx/copyView',
			DeleteViewUrl: '/NbtWebApp/wsNBT.asmx/deleteView',
			viewid: '',
			ID: 'vieweditor'
		};
		if(options) $.extend(o, options);

		var $parent = $(this);
		var $div = $('<div></div>')
					.appendTo($parent);

		var $wizard = $div.CswWizard('init', { 
				'ID': o.ID + '_wizard',
				'Title': 'Edit View',
				'StepCount': 5,
				'Steps': 
				{ 
					1: 'Choose a View',
					2: 'Edit View Attributes',
					3: 'Add Relationships',
					4: 'Select Properties',
					5: 'Set Filters'
				},
				'FinishText': 'Save and Finish'
			});

		// Step 1 - Choose a View
		var $div1 = $wizard.CswWizard('div', 1);
		var instructions = "A <em>View</em> controls the arrangement of information you see in a tree or grid.  "+
							"Views are useful for defining a user's workflow or for creating elaborate search criteria. "+
							"This wizard will take you step by step through the process of creating a new View or "+
							"editing an existing View.<br/><br/>";
		$div1.append(instructions);
		$div1.append('Select a View to Edit:');

		var $viewgrid_div = $('<div></div>').appendTo($div1);
		var gridwidth = 550;
		var $viewgrid;
		function onViewGridSuccess($vg) { 
			$viewgrid = $vg; 
		}
		_getViewsGrid(onViewGridSuccess, o.viewid);

		var $div1_btntbl = $div1.CswTable({ ID: o.ID + '_1_btntbl', width: gridwidth+'px' });
		var $div1_btntbl_cell11 = $div1_btntbl.CswTable('cell', 1, 1)
		var $div1_btntbl_cell12 = $div1_btntbl.CswTable('cell', 1, 2)
		$div1_btntbl_cell12.attr('align', 'right');
		var $allcheck_div = $('<div></div>').appendTo($div1_btntbl_cell12);

		IsAdministrator({
			'Yes': function() {
				$('<input type="checkbox" id="'+ o.ID + '_all">Show Other Roles/Users</input>')
					.appendTo($allcheck_div)
					.click(function() { 
						_getViewsGrid(onViewGridSuccess); 
					});
			}
		});

		var $copyviewbtn = $div1_btntbl_cell11.CswButton({
			'ID': o.ID + '_copyview',
			'enabledText': 'Copy View',
			'disableOnClick': true,
			'onclick': function() {
				var viewid = _getSelectedRowValue($viewgrid, 'NODEVIEWID');
				if(viewid != '' && viewid != undefined)
				{
					CswAjaxJSON({
						url: o.CopyViewUrl,
						data: "{ ViewId: "+ viewid +" }",
						success: function (gridJson) {
							_getViewsGrid(onViewGridSuccess, gridJson.copyviewid); 
						}
					});
				} // if(viewid != '' && viewid != undefined)
			} // onclick
		}); // copy button
		$copyviewbtn.CswButton('disable');

		var $deleteviewbtn = $div1_btntbl_cell11.CswButton({
			'ID': o.ID + '_deleteview',
			'enabledText': 'Delete View',
			'disableOnClick': true,
			'onclick': function() {
			    var viewid = _getSelectedRowValue($viewgrid, 'NODEVIEWID');
				if(viewid != '' && viewid != undefined)
				{
					if(confirm("Are you sure you want to delete: " + _getSelectedRowValue($viewgrid, 'VIEWNAME')))
					{
						CswAjaxJSON({
							url: o.DeleteViewUrl,
							data: "{ ViewId: "+ viewid +" }",
							success: function (gridJson) {
								_getViewsGrid(onViewGridSuccess); 
								$copyviewbtn.CswButton('disable');
							}
						});
					}
				}
			} // onclick
		}); // delete button
		$deleteviewbtn.CswButton('disable');

		var $newviewbtn = $div1_btntbl_cell11.CswButton({
			'ID': o.ID + '_newview',
			'enabledText': 'Create New View',
			'disableOnClick': false,
			'onclick': function() {
				$.CswDialog('AddViewDialog', { 
					'onAddView': function(newviewid) {
						$viewgrid = _getViewsGrid(onViewGridSuccess, newviewid); 
					},  // onAddView
					'onClose': function() {
						$newviewbtn.CswButton('enable');
					}  // onClose
				}); // CswDialog
			} // onclick
		})

		function _getViewsGrid(onSuccess, selectedrowpk)
		{
			var all = false;
			if($('#'+ o.ID + '_all:checked').length > 0)
				all = true;

			CswAjaxJSON({
				url: o.ViewGridUrl,
				data: "{ All: "+ all +" }",
				success: function (gridJson) {

					$viewgrid_div.empty();
					var $gridPager = $('<div id="' + o.ID + '_gp" style="width:100%; height:20px;" />')
									 .appendTo($viewgrid_div);
					var $viewgrid = $('<table id="'+ o.ID + '" />')
										.appendTo($viewgrid_div);

					var mygridopts = {
						'width': gridwidth,
						'height': 200,
						'onSelectRow': function(id, selected) {
							if(selected) 
							{
								$copyviewbtn.CswButton('enable');
								$deleteviewbtn.CswButton('enable');
							}
							else 
							{
								$copyviewbtn.CswButton('disable');
								$deleteviewbtn.CswButton('disable');
							}
						},
						//'toppager': true,
						'pager': $gridPager
					};
					$.extend(gridJson, mygridopts);

					$viewgrid.jqGrid(gridJson)
								.hideCol('NODEVIEWID');

					if(selectedrowpk != undefined)
					{
						$viewgrid.setSelection(_getRowForPk($viewgrid, selectedrowpk));
					}
					onSuccess($viewgrid);
				} // success
			}); // ajax
		} // _getViewsGrid()
		
		function _getSelectedRowValue($viewgrid, columnname)
		{
			var rowid = $viewgrid.jqGrid('getGridParam', 'selrow');
			var ret = $viewgrid.jqGrid('getCell', rowid, columnname);
			return ret;
		}
		function _getRowForPk($viewgrid, selectedpk)
		{
			var pks = $viewgrid.jqGrid('getCol', 'NODEVIEWID', true);
			var rowid = 0;
			for(var i in pks)
			{
				if(pks[i].value == selectedpk)
					rowid = pks[i].id;
			}
			return rowid;
		}

		return $div;

	} // $.fn.CswViewEditor
}) (jQuery);

