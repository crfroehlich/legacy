/// <reference path="~/app/CswApp-vsdoc.js" />
/// <reference path="~/vendor/extjs-4.1.0/ext-all-debug.js" />

(function ($) {
    "use strict";

    Csw.controls.nodeButton = Csw.controls.nodeButton ||
        Csw.controls.register('nodeButton', function (cswParent, options) {

            var cswPublic = {};
            var cswPrivate = {};

            Csw.tryExec(function () {

                (function _pre() {
                    cswPrivate = {
                        ID: 'nodebutton' + window.Ext.id(),
                        div: {},
                        value: '',
                        mode: 'button',
                        useToolTip: true,
                        messageDiv: {},
                        state: '',
                        confirmmessage: '',
                        table: {},
                        btnCell: {},
                        size: 'medium',
                        propId: '',
                        onClickSuccess: null,
                        onClickAction: null
                    };
                    Csw.extend(cswPrivate, options);
                    cswPrivate.div = cswParent.div();
                    cswPrivate.div.empty();

                    cswPrivate.table = cswPrivate.div.table({
                        ID: Csw.makeId(cswPrivate.ID, 'tbl')
                    });
                } ());

                cswPrivate.onButtonClick = function () {

                    cswPublic.button.disable();
                    if (Csw.isNullOrEmpty(cswPrivate.propId)) {
                        Csw.error.showError(Csw.error.makeErrorObj(Csw.enums.errorType.warning.name, 'Cannot execute a property\'s button click event without a valid property.', 'Attempted to click a property button with a null or empty propid.'));
                        cswPublic.button.enable();
                    } else {
                        // Case 27263: prompt to save instead
                        if (Csw.clientChanges.manuallyCheckChanges()) {
                            var performOnObjectClassButtonClick = function() {
                                Csw.ajax.post({
                                    urlMethod: 'onObjectClassButtonClick',
                                    data: {
                                        NodeTypePropAttr: cswPrivate.propId,
                                        SelectedText: Csw.string(cswPublic.button.selectedOption, Csw.string(cswPrivate.value))
                                    },
                                    success: function(data) {
                                        cswPublic.button.enable();

                                        var actionData = {
                                            data: data,
                                            propid: cswPrivate.propId,
                                            button: cswPublic.button,
                                            selectedOption: Csw.string(cswPublic.button.selectedOption),
                                            messagediv: cswPrivate.messageDiv,
                                            context: cswPrivate,
                                            onSuccess: cswPrivate.onAfterButtonClick
                                        };

                                        if (false === Csw.isNullOrEmpty(data.message)) {
                                            if (false === cswPrivate.useToolTip) {
                                                cswPublic.messageDiv.text(data.message);
                                            } else {
                                                window.Ext.create('Ext.tip.ToolTip', {
                                                    target: Csw.makeId(cswPrivate.ID, 'tbl'),
                                                    html: data.message,
                                                    autoShow: true,
                                                    focusOnToFront: true,
                                                    autoHide: false,
                                                    closable: true,
                                                    anchor: 'left',
                                                    bodyStyle: {
                                                        background: '#ffff00'
                                                    }
                                                });
                                            }
                                        }
                                        var continueToPub = false === Csw.isFunction(cswPrivate.onClickSuccess) || Csw.tryExec(cswPrivate.onClickSuccess, data);
                                        if (Csw.bool(data.success) && continueToPub) {
                                            Csw.publish(Csw.enums.events.objectClassButtonClick, actionData);
                                        }
                                    }, // ajax success()
                                    error: function() {
                                        cswPublic.button.enable();
                                    }
                                }); // ajax.post()
                            };

                            if (false === Csw.isNullOrEmpty(cswPrivate.confirmmessage)) {
                                $.CswDialog('GenericDialog', {
                                    ID: Csw.makeSafeId('ButtonConfirmationDialog'),
                                    title: 'Confirm ' + Csw.string(cswPrivate.value),
                                    height: 150,
                                    width: 400,
                                    div: Csw.literals.div({ text: cswPrivate.confirmmessage, align: 'center' }),
                                    onOk: function (selectedOption) {
                                        performOnObjectClassButtonClick();
                                    },
                                    onCancel: function () {
                                        cswPublic.button.enable();
                                    },
                                    onClose: function () {
                                        cswPublic.button.enable();
                                    }
                                });
                            } else {
                                performOnObjectClassButtonClick();
                            }

                        } // if (Csw.clientChanges.manuallyCheckChanges()) {
                    } // if-else (Csw.isNullOrEmpty(propAttr)) {
                }; // onButtonClick()

                (function _post() {
                    cswPrivate.btnCell = cswPrivate.table.cell(1, 1).div({ ID: Csw.makeSafeId(cswPrivate.ID, window.Ext.id()) });
                    cswPrivate.buttonId = Csw.makeSafeId(cswPrivate.ID, window.Ext.id());
                    switch (cswPrivate.mode) {
                        case 'button':
                            cswPublic.button = cswPrivate.btnCell.buttonExt({
                                ID: cswPrivate.buttonId,
                                size: cswPrivate.size,
                                enabledText: cswPrivate.value,
                                disabledText: cswPrivate.value,
                                disableOnClick: true,
                                onClick: cswPrivate.onButtonClick
                            });
                            break;
                        case 'menu':
                            cswPublic.button = cswPrivate.btnCell.menuButton({
                                ID: cswPrivate.buttonId,
                                selectedText: cswPrivate.selectedText,
                                menuOptions: cswPrivate.menuOptions,
                                size: cswPrivate.size,
                                state: Csw.string(cswPrivate.state, cswPrivate.value),
                                onClick: function (selectedOption) {
                                    cswPrivate.selectedText = selectedOption;
                                    cswPrivate.onButtonClick();
                                }
                            });
                            break;
                        //case 'link':           
                        //this is a fallthrough case           
                        default:
                            cswPublic.button = cswPrivate.btnCell.a({
                                ID: cswPrivate.buttonId,
                                value: cswPrivate.value,
                                onClick: cswPrivate.onButtonClick
                            });
                            break;
                    }

                    if (Csw.bool(cswPrivate.ReadOnly)) {
                        cswPublic.button.disable();
                    }

                    cswPublic.messageDiv = cswPrivate.table.cell(1, 2).div({
                        ID: Csw.makeId(cswPrivate.buttonId, 'msg'),
                        cssclass: 'buttonmessage'
                    });
                    
                    if (cswPrivate.Required) {
                        cswPublic.button.addClass('required');
                    }
                } ());
            });
            return cswPublic;
        });

    function onObjectClassButtonClick(eventOj, opts) {
        Csw.debug.assert(false === Csw.isNullOrEmpty(opts.data), 'opts.data is null.');
        var actionJson = opts.data.actionData;
        Csw.publish(Csw.enums.events.afterObjectClassButtonClick, opts.data.action);
        switch (Csw.string(opts.data.action).toLowerCase()) {
            case Csw.enums.nbtButtonAction.refresh:
                Csw.publish(Csw.enums.events.main.refreshSelected, actionJson);
                break;
            case Csw.enums.nbtButtonAction.nothing:
                //Do nothing
                break;
            case Csw.enums.nbtButtonAction.dispense:
                Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                actionJson.actionname = 'DispenseContainer';
                Csw.publish(Csw.enums.events.main.handleAction, actionJson);
                break;
            case Csw.enums.nbtButtonAction.editprop:
                $.CswDialog('EditNodeDialog', {
                    nodeids: [Csw.string(actionJson.nodeid)],
                    filterToPropId: Csw.string(actionJson.propidattr),
                    title: Csw.string(actionJson.title),
                    onEditNode: function (nodeid, nodekey, close) {
                        Csw.tryExec(close);
                    }
                });
                break;

            case Csw.enums.nbtButtonAction.loadView:
                Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                Csw.debug.assert(false === Csw.isNullOrEmpty(actionJson), 'actionJson is null.');
                Csw.publish(Csw.enums.events.RestoreViewContext, actionJson);
                break;

            case Csw.enums.nbtButtonAction.popup:
                Csw.debug.assert(false === Csw.isNullOrEmpty(actionJson), 'actionJson is null.');
                Csw.openPopup(actionJson.url);
                break;

            case Csw.enums.nbtButtonAction.reauthenticate:
                Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                /* case 24669 */
                Csw.cookie.clearAll();
                Csw.ajax.post({
                    urlMethod: 'reauthenticate',
                    data: { PropId: Csw.string(opts.propid) },
                    success: function () {
                        Csw.clientChanges.unsetChanged();
                        Csw.window.location('Main.html');
                    }
                });

                break;

            case Csw.enums.nbtButtonAction.receive:
                Csw.publish(Csw.enums.events.main.clear, { centertop: true, centerbottom: true });
                actionJson.actionname = 'Receiving';
                Csw.publish(Csw.enums.events.main.handleAction, actionJson);
                break;

            case Csw.enums.nbtButtonAction.request:
                Csw.debug.assert(false === Csw.isNullOrEmpty(actionJson), 'actionJson is null.');
                switch (actionJson.requestaction) {
                    case 'Dispose':
                        Csw.publish(Csw.enums.events.main.refreshHeader);
                        break;
                    default:
                        $.CswDialog('AddNodeDialog', {
                            nodetypeid: actionJson.requestItemNodeTypeId,
                            propertyData: actionJson.requestItemProps,
                            text: actionJson.titleText,
                            onSaveImmediate: function () {
                                Csw.publish(Csw.enums.events.main.refreshHeader);
                            }
                        });
                        break;
                }
                break;

            default:
                Csw.debug.error('No event has been defined for button click ' + opts.data.action);
                break;
        }
    }
    Csw.subscribe(Csw.enums.events.objectClassButtonClick, onObjectClassButtonClick);

})(jQuery);