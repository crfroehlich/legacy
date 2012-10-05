/// <reference path="~/app/CswApp-vsdoc.js" />

(function () {
    'use strict';
    Csw.properties.mol = Csw.properties.mol ||
        Csw.properties.register('mol',
            Csw.method(function (propertyOption) {
                'use strict';
                var cswPrivate = {};
                var cswPublic = {
                    data: propertyOption
                };

                var render = function () {
                    'use strict';
                    cswPublic.data = cswPublic.data || Csw.nbt.propertyOption(propertyOption);

                    cswPrivate.propVals = cswPublic.data.propData.values;
                    cswPrivate.parent = cswPublic.data.propDiv;
                    cswPrivate.width = 200; //Csw.string(propVals.width);
                    cswPrivate.mol = Csw.string(cswPrivate.propVals.mol).trim();

                    cswPublic.control = cswPrivate.parent.table({
                        ID: Csw.makeId(cswPublic.data.ID, 'tbl')
                    });

                    cswPrivate.cell11 = cswPublic.control.cell(1, 1).propDom('colspan', '3');

                    cswPublic.control.cell(2, 1).css('width', cswPrivate.width - 36);

                    cswPrivate.cell22 = cswPublic.control.cell(2, 2).css('textAlign', 'right');
                    cswPrivate.cell23 = cswPublic.control.cell(2, 3).css('textAlign', 'right');

                    cswPrivate.href = Csw.string(cswPrivate.propVals.href);
                    cswPrivate.href += '&usenodetypeasplaceholder=false';     // case 27596

                    cswPrivate.initMol = (function () {
                        function init(molData) {
                            cswPrivate.cell11.empty();
                            if (molData) {
                                if (cswPublic.data.isDisabled()) {
                                    cswPrivate.cell11.img({
                                        src: molData.href + '&uid=' + window.Ext.id(), //case 27492 - FF and IE cache URLs, so we have to make it unique to get new content to display
                                        height: cswPrivate.propVals.height,
                                        width: cswPrivate.width
                                    });
                                } else {
                                    cswPrivate.cell11.a({
                                        href: molData.href,
                                        target: '_blank'
                                    }).img({
                                        src: molData.href + '&foo=' + window.Ext.id(),
                                        height: cswPrivate.propVals.height,
                                        width: cswPrivate.width
                                    });
                                }
                            }
                        }

                        init(cswPrivate);
                        return init;
                    } ());

                    if (false === Csw.bool(cswPublic.data.isReadOnly())) {
                        /* Edit Button */
                        cswPrivate.cell22.div()
                            .icon({
                                ID: cswPublic.data.ID + '_edit',
                                iconType: Csw.enums.iconType.pencil,
                                hovertext: 'Edit',
                                size: 16,
                                isButton: true,
                                onClick: function () {
                                    $.CswDialog('EditMolDialog', {
                                        TextUrl: 'saveMolPropText',
                                        FileUrl: 'saveMolPropFile',
                                        PropId: cswPublic.data.propData.id,
                                        molData: cswPrivate.mol,
                                        onSuccess: function (data) {
                                            var val = data;
                                            cswPrivate.initMol(data);
                                            cswPublic.data.onPropChange({ href: data.href, mol: data.mol });
                                        }
                                    });
                                }
                            });

                        /* Clear Button */
                        cswPrivate.cell23.div()
                            .icon({
                                ID: cswPublic.data.ID + '_clr',
                                iconType: Csw.enums.iconType.trash,
                                hovertext: 'Clear Mol',
                                size: 16,
                                isButton: true,
                                onClick: function () {
                                    /* remember: confirm is globally blocking call */
                                    if (confirm("Are you sure you want to clear this structure?")) {
                                        var dataJson = {
                                            PropId: cswPublic.data.propData.id,
                                            IncludeBlob: true
                                        };

                                        Csw.ajax.post({
                                            urlMethod: 'clearProp',
                                            data: dataJson,
                                            success: function () {
                                                cswPrivate.initMol();
                                                cswPublic.data.onPropChange({ href: '', mol: '' });
                                            }
                                        });
                                    }
                                }
                            });

                    }
                };

                cswPublic.data.bindRender(render);
                return cswPublic;
            }));

} ());


