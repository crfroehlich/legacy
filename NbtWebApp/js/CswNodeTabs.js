﻿; (function ($) {
    $.fn.CswNodeTabs = function (options) {

        var o = {
            TabsUrl: '/NbtWebApp/wsNBT.asmx/getTabs',
            PropsUrl: '/NbtWebApp/wsNBT.asmx/getProps',
            nodeid: '',
            onSave: function() {}
        };

        if (options) {
            $.extend(o, options);
        }

        var $outertabdiv = $('<div id="tabdiv" />')
                        .appendTo($(this));

        getTabs(o.nodeid);


        function clearTabs()
        {
            $outertabdiv.children().remove();
        }

        function getTabs()
        {
            CswAjaxXml({
                url: o.TabsUrl,
                data: 'NodePk=' + o.nodeid,
                success: function ($xml) {
                            clearTabs();
                            var $tabdiv = $("<div><ul></ul></div>");
                            $outertabdiv.append($tabdiv);
                            //var firsttabid = null;
                            $xml.children().each(function() { 
                                $this = $(this);
                                $tabdiv.children('ul').append('<li><a href="#'+ $this.attr('id') +'">'+ $this.attr('name') +'</a></li>');
                                $tabdiv.append('<div id="'+ $this.attr('id') +'"><form id="'+ $this.attr('id') +'_form"></div>');
                                //if(null == firsttabid) 
                                //    firsttabid = $this.attr('id');
                            });
                            $tabdiv.tabs({
                                select: function(event, ui) {
                                            getProps($($tabdiv.children('div')[ui.index]).attr('id'));
                                        }
                            });
                            getProps($($tabdiv.children('div')[$tabdiv.tabs('option', 'selected')]).attr('id'));
                        } // success{}
            });
        } // getTabs()
            
        function getProps(tabid)
        {
            CswAjaxXml({
                url: o.PropsUrl,
                data: 'NodePk=' + o.nodeid + '&TabId=' + tabid,
                success: function ($xml) {
                            $div = $("#" + tabid);
                            $form = $div.children('form');
                            $form.children().remove();
                            
                            var $table = makeTable('proptable').appendTo($form);
                            
                            var i = 0;

                            $xml.children().each(function() { 
                                var $this = $(this);
                                var fieldtype = $this.attr('fieldtype');

                                if( fieldtype != 'Image' && 
                                    fieldtype != 'Grid' )
                                {
                                    var $labelcell = getTableCell($table, $this.attr('displayrow'), ($this.attr('displaycol') * 2 ) - 1);
                                    $labelcell.addClass('propertylabel');
                                    $labelcell.append($this.attr('name'));
                                }

                                var $propcell = getTableCell($table, $this.attr('displayrow'), ($this.attr('displaycol') * 2));
                                $propcell.addClass('propertyvaluecell');
                                var $propdiv = $('<div/>').appendTo($propcell); 

                                makePropControl($propdiv, fieldtype, $this);
                                
                            });

                            $table.append('<tr><td><input type="button" id="SaveTab" name="SaveTab" value="Save"/></td></tr>')
                                  .find('#SaveTab')
                                  .click(function() { Save($table, $xml) });

                            // Validation
                            $form.validate({ 
                                             highlight: function(element, errorClass) {
                                                 var $elm = $(element);
                                                 $elm.animate({ backgroundColor: '#ff6666'});
                                             },
                                             unhighlight: function(element, errorClass) {
                                                 var $elm = $(element);
                                                 $elm.css('background-color', '#66ff66');
                                                 setTimeout(function() { $elm.animate({ backgroundColor: 'transparent'}); }, 500);
                                             }
                                           });
                        } // success{}
            }); 
        } // getProps()

        function makePropControl($propdiv, fieldtype, $propxml)
        {
            switch(fieldtype)
            {
                case "Barcode":
                    $propdiv.CswFieldTypeBarcode('init', o.nodeid, $propxml);
                    break;
                case "Date":
                    $propdiv.CswFieldTypeDate('init', o.nodeid, $propxml);
                    break;
                case "Image":
                    $propdiv.CswFieldTypeImage('init', o.nodeid, $propxml);
                    break;
                case "List":
                    $propdiv.CswFieldTypeList( 'init', o.nodeid, $propxml );
                    break;
                case "Logical":
                    $propdiv.CswFieldTypeLogical( 'init', o.nodeid, $propxml );
                    break;
                case "Memo":
                    $propdiv.CswFieldTypeMemo('init', o.nodeid, $propxml);
                    break;
                case "Number":
                    $propdiv.CswFieldTypeNumber( 'init', o.nodeid, $propxml );
                    break;
                case "PropertyReference":
                    $propdiv.CswFieldTypePropertyReference('init', o.nodeid, $propxml);
                    break;
                case "Relationship":
                    $propdiv.CswFieldTypeRelationship('init', o.nodeid, $propxml);
                    break;
                case "Sequence":
                    $propdiv.CswFieldTypeSequence('init', o.nodeid, $propxml);
                    break;
                case "Static":
                    $propdiv.CswFieldTypeStatic( 'init', o.nodeid, $propxml );
                    break;
                case "Text":
                    $propdiv.CswFieldTypeText( 'init', o.nodeid, $propxml );
                    break;
                default:
                    $propdiv.append($propxml.attr('gestalt'));
                    break;
            }
        } // makePropControl()

        function Save($table, $propsxml)
        {
            $propsxml.children().each(function() { 
                var $propxml = $(this);
                var $propcell = getTableCell($table, $propxml.attr('displayrow'), ($propxml.attr('displaycol') * 2));
                var fieldtype = $propxml.attr('fieldtype');
                var $propdiv = $propcell.children('div');
                                
                switch(fieldtype)
                {
                    case "Barcode":
                        $propdiv.CswFieldTypeBarcode( 'save', $propdiv, $propxml );
                        break;
                    case "Date":
                        $propdiv.CswFieldTypeDate( 'save', $propdiv, $propxml );
                        break;
                    case "Image":
                        $propdiv.CswFieldTypeImage( 'save', $propdiv, $propxml );
                        break;
                    case "List":
                        $propdiv.CswFieldTypeList( 'save', $propdiv, $propxml );
                        break;
                    case "Logical":
                        $propdiv.CswFieldTypeLogical( 'save', $propdiv, $propxml );
                        break;
                    case "Memo":
                        $propdiv.CswFieldTypeMemo( 'save', $propdiv, $propxml );
                        break;
                    case "Number":
                        $propdiv.CswFieldTypeNumber( 'save', $propdiv, $propxml );
                        break;
                    case "PropertyReference":
                        $propdiv.CswFieldTypePropertyReference( 'save', $propdiv, $propxml );
                        break;                    
                    case "Relationship":
                        $propdiv.CswFieldTypeRelationship( 'save', $propdiv, $propxml );
                        break;
                    case "Sequence":
                        $propdiv.CswFieldTypeSequence( 'save', $propdiv, $propxml );
                        break;
                    case "Static":
                        $propdiv.CswFieldTypeStatic( 'save', $propdiv, $propxml );
                        break;
                    case "Text":
                        $propdiv.CswFieldTypeText( 'save', $propdiv, $propxml );
                        break;
                    default:
                        break;
                } // switch
            }); // each()

            CswAjaxJSON({
                url: '/NbtWebApp/wsNBT.asmx/SaveProps',
                data: "{ NodePk: '" + o.nodeid + "', NewPropsXml: '" + xmlToString($propsxml) + "' }",
                success: o.onSave 
            });

        } // Save()

        // For proper chaining support
        return this;

    }; // function(options) {
}) (jQuery);

