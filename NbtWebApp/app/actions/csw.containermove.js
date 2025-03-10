/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {

    Csw.actions.register('containerMove', function (cswParent, options) {
        'use strict';
        var cswPublic = {};
        var cswPrivate = {
            name: 'CswContainerMove',
            onSubmit: null,
            onCancel: null,
            gridOpts: {},
            requestitemid: '',
            location: ''
        };

        if (Csw.isNullOrEmpty(cswParent)) {
            Csw.error.throwException('Cannot create a Request action without a valid Csw Parent object.', 'Csw.actions.containerMove', 'csw.containerMove.js', 14);
        }

        (function _preCtor() {
            Csw.extend(cswPrivate, options, true);
            cswParent.empty();
            var state;
            if (!cswPrivate.requestitemid) {
                state = Csw.clientDb.getItem(cswPrivate.name + '_clientDb');
                cswPrivate.requestitemid = state.requestitemid;
                cswPrivate.location = state.location;
            } else {
                Csw.clientDb.setItem(cswPrivate.name + '_clientDb', {
                    requestitemid: cswPrivate.requestitemid,
                    location: cswPrivate.location
                });
            }
        }());

        cswPrivate.submitRequest = function () {
            if (cswPrivate.containerGrid &&
                cswPrivate.containerGrid.grid &&
                cswPrivate.containerGrid.grid.getSelectedRowsVals('nodeid').length > 0) {
                Csw.ajaxWcf.post({
                    urlMethod: 'Requests/fulfill',
                    data: {
                        RequestItemId: cswPrivate.requestitemid,
                        ContainerIds: cswPrivate.containerGrid.grid.getSelectedRowsVals('nodeid')
                    },
                    success: function (json) {
                        if (json.Succeeded) {
                            Csw.clientDb.removeItem(cswPrivate.name + '_clientDb');
                            Csw.tryExec(cswPrivate.onSubmit);
                        } else {
                            cswPrivate.containerGrid.grid.reload();
                        }
                    }
                });
            } else {
                cswPrivate.action.finish.quickTip({ html: 'No containers selected for move.' });
            }
        };

        cswPrivate.initGrid = function () {

            Csw.ajax.deprecatedWsNbt({
                urlMethod: 'getDispenseContainerView',
                data: {
                    RequestItemId: cswPrivate.requestitemid
                },
                success: function (data) {
                    if (Csw.isNullOrEmpty(data.viewid)) {
                        Csw.error.throwException(Csw.error.exception('Could not get a grid of containers for this request item.', '', 'csw.containerMove.js', 54));
                    }
                    cswPrivate.containerGrid = Csw.wizard.nodeGrid(cswPublic.gridParent, {
                        hasMenu: false,
                        viewid: data.viewid,
                        forceFit: false,
                        readonly: true,
                        showCheckboxes: true
                    });
                }
            });
        }; // initGrid()

        (function _postCtor() {

            cswPrivate.action = Csw.layouts.action(cswParent, {
                title: cswPrivate.title || 'Move Containers',
                finishText: 'Fulfill Request',
                onFinish: cswPrivate.submitRequest,
                onCancel: function () {
                    Csw.clientDb.removeItem(cswPrivate.name + '_clientDb');
                    Csw.tryExec(cswPrivate.onCancel);
                }
            });

            cswPrivate.actionTbl = cswPrivate.action.actionDiv.table({
                name: cswPrivate.name + '_tbl',
                align: 'center'
            }).css('width', '95%');

            cswPrivate.actionTbl.cell(1, 1)
                .css('text-align', 'left')
                .span({ text: 'Select the containers to move to: ' + cswPrivate.location });

            cswPrivate.actionTbl.cell(3, 1).br({ number: 2 });
            cswPrivate.gridId = cswPrivate.name + '_csw_requestGrid_outer';
            cswPublic.gridParent = cswPrivate.actionTbl.cell(4, 1).div({
                name: cswPrivate.gridId
            });

            cswPrivate.initGrid();

        }());

        return cswPublic;
    });
}());

