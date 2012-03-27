/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function () {

    Csw.controls.moreDiv = Csw.controls.moreDiv ||
        Csw.controls.register('moreDiv', function (cswParent, options) {
            'use strict';
            var internal = {
                ID: '',
                $parent: '',
                moretext: 'more',
                lesstext: 'less'
            };
            if (options) {
                $.extend(internal, options);
            }

            var external = {};

            external.shownDiv = cswParent.div({
                ID: Csw.makeId(internal.ID, '', '_shwn')
            });

            external.hiddenDiv = cswParent.div({
                ID: Csw.makeId(internal.ID, '', '_hddn')
            }).hide();

            external.moreLink = cswParent.link({
                ID: Csw.makeId(internal.ID, '', '_more'),
                text: internal.moretext,
                cssclass: 'morelink',
                onClick: function () {
                    if (external.moreLink.toggleState === Csw.enums.toggleState.on) {
                        external.moreLink.text(internal.lesstext);
                        external.hiddenDiv.show();
                    } else {
                        external.moreLink.text(internal.moretext);
                        external.hiddenDiv.hide();
                    }
                    return false;
                } // onClick()
            });

            return external;
        });

} ());
