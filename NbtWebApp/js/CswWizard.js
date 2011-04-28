﻿; (function ($) {
	var PluginName = "CswWizard";

	var methods = {
		'init': function(options) 
			{
				var o = {
					ID: '',
					Title: 'A Wizard',
					StepCount: 1,
					Steps: { 1: 'Default' },
					SelectedStep: 1,
					FinishText: 'Finish',
					onNext: function (newstepno) { },
					onPrevious: function (newstepno) { },
					onBeforeNext: function(stepno) { return true; },
					onBeforePrevious: function(stepno) { return true; },
					onFinish: function () { },
					onCancel: function() {}
				};
				if(options) $.extend(o, options);
				
				var $parent = $(this);
				
				var $table = $parent.CswTable({ ID: o.ID, TableCssClass: 'CswWizard_WizardTable' });
				$table.attr("stepcount", o.StepCount);
				
				var $titlecell = $table.CswTable('cell', 1, 1)
									.addClass('CswWizard_TitleCell')
									.attr('colspan', 2)
									.append(o.Title);

				var $indexcell = $table.CswTable('cell', 2, 1)
									.attr('rowspan', 2)
									.addClass('CswWizard_IndexCell');
				
				var $stepscell = $table.CswTable('cell', 2, 2)
									.addClass('CswWizard_StepsCell');

				var $buttonscell = $table.CswTable('cell', 3, 1)
									.addClass('CswWizard_ButtonsCell');

				for(var s = 1; s <= o.StepCount; s++)
				{
					var steptitle = o.Steps[s];

//					$('<div class="CswWizard_StepLinkDiv" stepno="' + s + '"><a href="#">' + s + '.&nbsp;' + steptitle + '</a></div>')
//										.appendTo($indexcell)
//										.children('a')
//										.click( function(stepno) { return function() { _selectStep($table, stepno); return false; }; }(s) );
					$('<div class="CswWizard_StepLinkDiv" stepno="' + s + '">' + s + '.&nbsp;' + steptitle + '</div>')
										.appendTo($indexcell);

					$('<div class="CswWizard_StepDiv" id="' + o.ID + '_' + s + '" stepno="' + s + '" ><span class="CswWizard_StepTitle">'+ steptitle +'</span><br/></br><div id="' + o.ID + '_' + s + '_content"></div></div>')
										.appendTo($stepscell);
				}

				var $buttontbl = $buttonscell.CswTable({ ID: o.ID + '_btntbl', width: '100%' });
				var $bcell11 = $buttontbl.CswTable('cell', 1, 1)
							.attr('align', 'right')
							.attr('width', '65%');
				var $bcell12 = $buttontbl.CswTable('cell', 1, 2)
							.attr('align', 'right')
							.attr('width', '35%');

				var $prevbtn = $bcell11.CswButton('init', { 'ID': o.ID + '_prev',
															'enabledText': '< Previous',
															'disableOnClick': false,
															'onclick': function() { 
																	var currentStepNo = _getCurrentStepNo($table);
																	if(o.onBeforePrevious(currentStepNo))
																	{
																		_selectStep($table, currentStepNo - 1);
																		o.onPrevious(currentStepNo - 1);
																	}
																}
															});
				var $nextbtn = $bcell11.CswButton('init', { 'ID': o.ID + '_next',
															'enabledText': 'Next >',
															'disableOnClick': false,
															'onclick': function() { 
																	var currentStepNo = _getCurrentStepNo($table);
																	if(o.onBeforeNext(currentStepNo))
																	{
																		_selectStep($table, currentStepNo + 1);
																		o.onNext(currentStepNo + 1);
																	}
																}
															});
				var $finishbtn = $bcell11.CswButton('init', { 'ID': o.ID + '_finish',
															'enabledText': o.FinishText,
															'onclick': o.onFinish
															});
				var $cancelbtn = $bcell12.CswButton('init', { 'ID': o.ID + '_cancel',
															'enabledText': 'Cancel',
															'onclick': o.onCancel
															});

				_selectStep($table, o.SelectedStep);

				return $table;
			}, // init()

		'div': function (stepno)
			{
				var $table = $(this);
				return $table.find('.CswWizard_StepDiv[stepno=' + stepno + '] div');
			},

		// e.g. $wizard.CswWizard('button', 'next', 'disable');
		'button': function(button, action) {
				var $table = $(this);
				switch(button)
				{
					case 'previous':
						$('#' + $table.attr('id') + '_prev').CswButton(action);
						break;
					case 'next':
						$('#' + $table.attr('id') + '_next').CswButton(action);
						break;
					case 'finish':
						$('#' + $table.attr('id') + '_finish').CswButton(action);
						break;
					case 'cancel':
						$('#' + $table.attr('id') + '_cancel').CswButton(action);
						break;
				}
			}
		};


	function _getCurrentStepNo($table)
	{
		return parseInt($table.find('.CswWizard_StepLinkDivSelected').attr('stepno'));
	}
	
	function _selectStep($table, stepno)
	{
		var stepcount = $table.attr("stepcount");
		if(stepno > 0 && stepno <= stepcount)
		{
			$table.find('.CswWizard_StepLinkDiv').removeClass('CswWizard_StepLinkDivSelected');
			$table.find('.CswWizard_StepLinkDiv[stepno='+ stepno + ']').addClass('CswWizard_StepLinkDivSelected');

			$table.find('.CswWizard_StepDiv').hide();
			$table.find('.CswWizard_StepDiv[stepno=' + stepno + ']').show();

			var $prevbtn = $('#' + $table.attr('id') + '_prev');
			if(stepno === 1) 
				$prevbtn.CswButton('disable');
			else
				$prevbtn.CswButton('enable');

			var $nextbtn = $('#' + $table.attr('id') + '_next');
			if(stepno >= stepcount)
				$nextbtn.CswButton('disable');
			else
				$nextbtn.CswButton('enable');

		} // if(stepno <= stepcount)
	} // _selectStep()
	
	// Method calling logic
	$.fn.CswWizard = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};

}) (jQuery);

