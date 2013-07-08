/// <reference path="~/app/CswApp-vsdoc.js" />
/* globals Csw:false, $:false  */

(function () {
    'use strict';
    Csw.properties.list = Csw.properties.register('list',
        function (nodeProperty) {
            'use strict';

            //The render function to be executed as a callback
            var render = function () {
                'use strict';
                var cswPrivate = Csw.object();
                var comboBoxDefaultWidth = 150;

                cswPrivate.text = nodeProperty.propData.values.text;
                cswPrivate.value = nodeProperty.propData.values.value;
                cswPrivate.options = nodeProperty.propData.values.options;
                cswPrivate.propid = nodeProperty.propData.id;
                //cswPrivate.fieldtype = nodeProperty.propData.fieldtype;

                nodeProperty.onPropChangeBroadcast(function (val) {
                    if (cswPrivate.text !== val) {
                        cswPrivate.text = val;
                        if (cswPrivate.select) {
                            cswPrivate.select.setValue(text);
                        }
                        if (span) {
                            span.remove();
                            span = nodeProperty.propDiv.span({ text: cswPrivate.text });
                        }
                    }
                });//nodeProperty.onPropChangeBroadcast()

                if (nodeProperty.isReadOnly()) {
                    var span = nodeProperty.propDiv.span({ text: cswPrivate.text });
                } else {

                    // Create the Store
                    cswPrivate.listOptionsStore = new Ext.data.Store({
                        fields: ['Text', 'Value'],
                        autoLoad: false
                    });

                    // Create the Ext JS ComboBox
                    cswPrivate.select = Ext.create('Ext.form.field.ComboBox', {
                        name: nodeProperty.name,
                        renderTo: nodeProperty.propDiv.div().getId(),
                        displayField: 'Text',
                        store: cswPrivate.listOptionsStore,
                        queryMode: 'local',
                        value: cswPrivate.value,
                        listeners: {
                            select: function (combo, records, eOpts) {
                                var text = records[0].get('Text');
                                cswPrivate.text = text;
                                nodeProperty.propData.values.text = text;

                                var val = records[0].get('Value');
                                cswPrivate.value = val;
                                nodeProperty.propData.values.value = val;

                                nodeProperty.broadcastPropChange(text);
                            }
                        },
                        listConfig: {
                            width: 'auto'
                        },
                        tpl: new Ext.XTemplate('<tpl for=".">' + '<li style="height:22px;" class="x-boundlist-item" role="option">' + '{Text}' + '</li></tpl>'),
                        queryDelay: 2000


                    });

                    /*
                     * To search or not to search?
                     *
                     * If the server returns search === true, then the number of options exceeded
                     * the relationshipoptionlimit configuration variable. When the number of
                     * options exceeds this variable, the user is forced to search the options.
                     *
                     */
                    if (nodeProperty.propData.values.search === false) {
                        cswPrivate.listOptionsStore.loadData(cswPrivate.options);
                    } else {
                        // Create a proxy to call the searchListOptions web service method
                        cswPrivate.proxy = new Ext.data.proxy.Ajax({
                            noCache: false,
                            pageParam: undefined,
                            headers: {
                                'Accept': 'application/json',
                                'Content-Type': 'application/json; charset=utf-8'
                            },
                            startParam: undefined,
                            limitParam: undefined,
                            actionMethods: {
                                read: 'POST'
                            },
                            url: 'Services/Search/searchListOptions', //MUST INCLUDE "Services/" or it doesn't work
                            reader: {
                                type: 'json',
                                root: 'Data.Options',
                                getResponseData: function (response) {
                                    // This function allows us to intercept the data before the reader
                                    // reads it so that we can convert it into an array of objects the 
                                    // store will accept.
                                    var json = Ext.decode(response.responseText);

                                    //Set the width of the combobox to match the longest string returned
                                    if (json.Data.Options.length > 0) {
                                        var longestOption = json.Data.Options.sort(function (a, b) { return b.Text.length - a.Text.length; })[0];
                                        var newWidth = (longestOption.Text.length * 7);
                                        if (newWidth > comboBoxDefaultWidth) {
                                            cswPrivate.select.setWidth(newWidth);
                                        }
                                    }

                                    return this.readRecords(json);
                                }
                            }
                        });
                        // Set the store's proxy to the one created above
                        cswPrivate.listOptionsStore.setProxy(cswPrivate.proxy);

                        // Add the appropriate listeners for the remotely populated combobox
                        cswPrivate.listOptionsStore.on({
                            beforeload: function (store, operation) {

                                // Clear the store before filling it with new data
                                cswPrivate.listOptionsStore.loadData([], false);

                                //Set the parameter object to be sent
                                var CswNbtSearchRequest = {};
                                CswNbtSearchRequest.NodeTypePropId = cswPrivate.propid;
                                CswNbtSearchRequest.SearchTerm = cswPrivate.select.getValue();

                                operation.params = Csw.serialize(CswNbtSearchRequest);
                            }
                        });

                        // Set the properties on the combobox that are needed for querying remotely
                        cswPrivate.select.queryMode = 'remote';
                        cswPrivate.select.queryParam = false;
                        cswPrivate.select.minChars = 1;
                        cswPrivate.select.triggerAction = 'query';

                    }//if (cswPrivate.options.length > 0)

                }//if (nodeProperty.isReadOnly())

            };//render()

            //Bind the callback to the render event
            nodeProperty.bindRender(render);

            //Bind an unrender callback to terminate any outstanding ajax requests, if any. See propTypeGrid.
            //nodeProperty.unBindRender();

            return true;
        });
}());
