/// <reference path="/js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../controls/CswGrid.js" />

; (function ($) { /// <param name="$" type="jQuery" />
	
    var pluginName = 'CswNodeGrid';
    
	var methods = {
	
		'init': function (optJqGrid) {

			var o = {
				GridUrl: '/NbtWebApp/wsNBT.asmx/getGrid',
				viewid: '',
				showempty: false,
				ID: '',
				nodeid: '',
				cswnbtnodekey: '',
				reinit: false,
				EditMode: EditMode.Edit.name,
				//onAddNode: function(nodeid,cswnbtnodekey){},
				onEditNode: null, //function(nodeid,cswnbtnodekey){},
				onDeleteNode: null //function(nodeid,cswnbtnodekey){}
			};
		
			if (optJqGrid) {
				$.extend(o, optJqGrid);
			}
			
			var $parent = $(this);
			if (o.reinit) $parent.empty();
			
			var dataJson = {ViewId: o.viewid, SafeNodeKey: o.cswnbtnodekey, ShowEmpty: o.showempty };
		    var ret;
		    
			CswAjaxJson({
				url: o.GridUrl,
				data: dataJson,
				success: function (gridJson) {

				    var jqGridOpt = gridJson.jqGridOpt;

				    var g = {
				        canEdit: isTrue(jqGridOpt.CanEdit),
				        canDelete: isTrue(jqGridOpt.CanDelete),
				        hasPager: true,
				        gridOpts: {
				            toppager: (jqGridOpt.rowNum >= 50)
				        },
				        optNav: { },
				        optSearch: { },
				        optNavEdit: { },
				        optNavDelete: { }
				    };
				    $.extend(g.gridOpts, jqGridOpt);

				    if (isNullOrEmpty(g.gridOpts.width)) {
				        g.gridOpts.width = '650px';
				    }

				    if (o.EditMode === EditMode.PrintReport.name)
				    {
				        g.gridOpts.caption = '';
				        g.hasPager = false;
				    }
				    else
				    {
				        g.optNavEdit =
    				    {
    				        editfunc: function(rowid)
    				        {
    				            var editOpt = {
    				                cswnbtnodekey: '',
    				                nodeid: '',
    				                onEditNode: o.onEditNode
    				            };
    				            if (rowid !== null)
    				            {
    				                editOpt.cswnbtnodekey = ret.getCell(rowid, 'cswnbtnodekey');
    				                editOpt.nodeid = ret.getCell(rowid, 'nodeidstr');
    				                $.CswDialog('EditNodeDialog', editOpt);
    				            }
    				            else
    				            {
    				                alert('Please select a row to edit');
    				            }
    				            return editOpt.CswNbtNodeKey;
    				        }
    				    };
						
				        g.optNavDelete = {
						    delfunc: function(rowid)
						    {
						        var delOpt = {
						            cswnbtnodekey: '',
						            nodeid: '',
						            nodename: '',
						            onDeleteNode: o.onDeleteNode
						        };
						        if (rowid !== null) {
						            delOpt.cswnbtnodekey = ret.getCell(rowid, 'cswnbtnodekey');
						            delOpt.nodename = ret.getCell(rowid, 'nodename');
						            $.CswDialog('DeleteNodeDialog', delOpt);
						        }
						        else
						        {
						            alert('Please select a row to delete');
						        }
						        return delOpt.cswnbtnodekey;
						    }
						};

					} // else
				    ret = new CswGrid(g, $parent);
				} // success{} 
			}); // ajax
			return ret;
		} // 'init'
	}; // methods

    $.fn.CswNodeGrid = function(method) {
		// Method calling logic
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }
    };

})(jQuery);

