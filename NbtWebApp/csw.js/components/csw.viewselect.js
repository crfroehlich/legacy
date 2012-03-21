/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function () {
    "use strict";

    function viewSelect(params) {
        var internal = {
            viewurl: '/NbtWebApp/wsNBT.asmx/getViewSelect',
            recenturl: '/NbtWebApp/wsNBT.asmx/getViewSelectRecent',
            ID: 'viewselect',
            $parent: null,
            onSelect: null,
            onSuccess: null,
            ClickDelay: 300,
            issearchable: false,
            //usesession: true,
            hidethreshold: 5,

            div: null
        };
        if (params) $.extend(internal, params);

        var external = {};

        internal.addCategory = function (catobj) {

            var fieldsetid = Csw.controls.dom.makeId(internal.ID, '', catobj.category + '_fs', '', false);
            var $fieldset = internal.vsdiv.$.find('#' + fieldsetid);
            if ($fieldset.length === 0) {
                $fieldset = $('<fieldset id="' + fieldsetid + '" class="viewselectfieldset"></fieldset>')
                                .appendTo(internal.vsdiv.$);
            }
            
            $fieldset.contents().remove();
            $fieldset.append('<legend class="viewselectlegend">' + catobj.category + '</legend>');

            var morediv = Csw.controls.moreDiv({
                ID: Csw.controls.dom.makeId(internal.ID, '', catobj.category + '_morediv'),
                $parent: $fieldset
            });

            var showntbl = morediv.shownDiv.table({ cellpadding: '2px', width: '100%' });
            var hiddentbl = morediv.hiddenDiv.table({ cellpadding: '2px', width: '100%' });
            var row = 1;
            var tbl = showntbl;
            morediv.moreLink.hide();

            Csw.each(catobj.items, function (itemobj, itemname) {
                if (row > internal.hidethreshold) {
                    row = 1;
                    tbl = hiddentbl;
                    morediv.moreLink.show();
                }

                var iconcell = tbl.cell(row, 1).addClass('viewselecticoncell');
                iconcell.img({ src: itemobj.iconurl });

                var linkcell = tbl.cell(row, 2).addClass('viewselectitemcell');
                linkcell.text(itemobj.name);

                iconcell.$.click(function () { internal.handleSelect(itemobj); });
                linkcell.$.click(function () { internal.handleSelect(itemobj); });

                linkcell.$.hover(
                    function () {
                        iconcell.addClass('viewselectitemhover');
                        linkcell.addClass('viewselectitemhover');
                    },
                    function () {
                        iconcell.removeClass('viewselectitemhover');
                        linkcell.removeClass('viewselectitemhover');
                    });

                row += 1;
            }); // Csw.each() items
        }; // addCategory()

        internal.handleSelect = function (itemobj) {

            var $newTopContent = $('<div></div>');
            var table = Csw.controls.table({
                $parent: $newTopContent,
                ID: internal.ID + 'selectedtbl'
            });
            var iconDiv = table.cell(1, 1).div();

            iconDiv.css('background-image', itemobj.iconurl);
            iconDiv.css('width', '16px');
            iconDiv.css('height', '16px');

            table.cell(1, 2).text(Csw.string(itemobj.name).substr(0, 30));

            internal.div.$.CswComboBox('TopContent', $newTopContent);
            internal.div.$.CswAttrNonDom('selectedType', itemobj.type);
            switch (itemobj.type.toLowerCase()) {
                case 'view':
                    internal.div.propNonDom('selectedValue', itemobj.viewid);
                    break;
                case 'action':
                    internal.div.propNonDom('selectedValue', itemobj.actionid);
                    break;
                case 'report':
                    internal.div.propNonDom('selectedValue', itemobj.reportid);
                    break;
            }

            setTimeout(function () { internal.div.$.CswComboBox('toggle'); }, internal.ClickDelay);
            Csw.tryExec(internal.onSelect, itemobj);
        }; // internal.handleSelect()


        // Constructor
        (function () {
            internal.div = Csw.controls.div({ $parent: internal.$parent });
            internal.vsdiv = Csw.controls.div({ ID: Csw.controls.dom.makeId(internal.ID, '', 'vsdiv') });
            internal.div.$.CswComboBox('init', {
                ID: internal.ID + '_combo',
                TopContent: 'Select a View',
                SelectContent: internal.vsdiv.$,
                Width: '266px'
            });

            Csw.ajax.post({
                url: internal.viewurl,
                data: { IsSearchable: internal.issearchable },
                stringify: false,
                success: function (data) {
                    Csw.each(data.viewselectitems, internal.addCategory);
                    Csw.tryExec(internal.onSuccess);
                }
            });
        })();


        external.value = function () {
            return {
                type: internal.div.propNonDom('selectedType'),
                value: internal.div.propNonDom('selectedValue')
            };
        };

        external.refreshRecent = function () {
            Csw.ajax.post({
                url: internal.recenturl,
                success: function (data) {
                    Csw.each(data.viewselectitems, internal.addCategory);
                }
            });
        };

        return external;
    }

    Csw.controls.register('viewSelect', viewSelect);
    Csw.controls.viewSelect = Csw.controls.viewSelect || viewSelect;
})();