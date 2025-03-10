/// <reference path="~/app/CswApp-vsdoc.js" />


(function ($) {
    "use strict";

    // Create private collection of errors that are stored here
    // When we call the function below, first check to see if the error message already exists here
    //  and if it does, then we don't display it again
    var cswPrivate = {
        errors: {}
    };

    $.fn.CswErrorMessage = function (options) {

        var o = {
            name: '',
            type: '',   // Warning, Error 
            message: '',
            detail: ''
        };
        Csw.extend(o, options);

        var errorDiv = null;

        // If the error doesn't already exist, we create it and show it
        if (Csw.isNullOrUndefined(cswPrivate.errors[o.name])) {

            var $parentdiv = $(this);
            var parent = Csw.literals.factory($parentdiv);
            parent.show();

            var date = new Date();
            var id;
            if (Csw.isNullOrEmpty(o.name)) {
                id = ".error_" + date.getTime();
            } else {
                id = o.name;
            }

            errorDiv = parent.div({
                name: id,
                cssclass: 'CswErrorMessage_Message'
            });

            if (o.type.toLowerCase() === "warning") {
                errorDiv.addClass('CswErrorMessage_Warning');
            } else if (o.type.toLowerCase() === 'js') {
                errorDiv.addClass('CswErrorMessage_JS');
            } else {
                errorDiv.addClass('CswErrorMessage_Error');
            }

            var table = errorDiv.table({
                name: 'tbl',
                width: '100%'
            });

            var cell11 = table.cell(1, 1);
            var cell12 = table.cell(1, 2);
            var cell13 = table.cell(1, 3);
            var cell22 = table.cell(2, 2);

            // Look for node references in the error message
            cell12.nodeLink({ text: o.message });

            cell13.css({ width: '18px' });
            cell13.icon({
                name: 'hidebtn',
                iconType: Csw.enums.iconType.x,
                hovertext: 'Hide',
                isButton: true,
                size: 16,
                onClick: function () {
                    errorDiv.remove();
                    if ($parentdiv.children().length === 0) {
                        $parentdiv.hide();
                    }
                    // Remove from list of errors
                    delete cswPrivate.errors[id];
                }
            });

            if (false === Csw.isNullOrEmpty(o.detail)) {
                cell11.css({ width: '18px' });
                var togglebtn = cell11.imageButton({
                    ButtonType: Csw.enums.imageButton_ButtonType.ToggleInactive,
                    AlternateText: 'Expand',
                    name: 'expandbtn',
                    onClick: function () {
                        cell22.$.toggle();
                        if (togglebtn.getButtonType() == Csw.enums.imageButton_ButtonType.ToggleActive) {
                            togglebtn.setButtonType(Csw.enums.imageButton_ButtonType.ToggleInactive);
                        } else {
                            togglebtn.setButtonType(Csw.enums.imageButton_ButtonType.ToggleActive);
                        }
                    }
                });

                cell22.append(o.detail);
                cell22.hide();
            }

            $('html, body').animate({ scrollTop: 0 }, 0);

            //case 23675
            var dialog = parent.parent();
            if (dialog.$.hasClass('ui-dialog-content')) {
                dialog.$.animate({ scrollTop: 0 }, 0);
            }

            // Add to the list of errors
            cswPrivate.errors[id] = errorDiv;
        }

        return errorDiv;

    }; // function (options) {
})(jQuery);
