﻿// for CswViewPropFilter
var ViewBuilder_CssClasses = {
    subfield_select: { name: 'csw_viewbuilder_subfield_select' },
    filter_select: { name: 'csw_viewbuilder_filter_select' },
    default_filter: { name: 'csw_viewbuilder_default_filter' },
    filter_value: { name: 'csw_viewbuilder_filter_value' },
    metadatatype_static: { name: 'csw_viewbuilder_metadatatype_static' }
};

;  (function ($) {
	
    var PluginName = "CswViewPropFilter";

    function makePropFilterId(ID, options)
    {
        var FilterId = '';
        var Delimiter = '_';
        var o = {
            'proparbitraryid': '',
            'filtarbitraryid': '',
            'viewbuilderpropid': '',
            'ID': ''
        };
        if(options) $.extend(o,options);
        
        if( o.filtarbitraryid !== '' && o.filtarbitraryid !== undefined )
        {
            FilterId = makeId({ 'ID': ID + Delimiter + 'filtarbitraryid', 
                                'prefix': o.ID, 
                                'suffix': o.filtarbitraryid });
        }
        else if( o.viewbuilderpropid !== '' && o.viewbuilderpropid !== undefined )
        {
            FilterId = makeId({ 'ID': ID + Delimiter + 'viewbuilderpropid', 
                                'prefix': o.ID, 
                                'suffix': o.viewbuilderpropid });
        }
        else if( o.proparbitraryid !== '' && o.proparbitraryid !== undefined )
        {
            FilterId = makeId({ 'ID': ID + Delimiter + 'proparbitraryid', 
                                'prefix': o.ID, 
                                'suffix': o.proparbitraryid });
        }
        else if( o.ID !== '' && o.ID !== undefined )
        {
            FilterId = makeId({ 'ID': ID, 
                                'prefix': o.ID });
        }
        else
        {
            FilterId = ID;
        }
        return FilterId;
    }

    var methods = {

        'init': function(options) 
		{
            var o = { 
                //URLs
                'getNewPropsUrl': '/NbtWebApp/wsNBT.asmx/getViewPropFilterUI',

                //options
			    'viewid': '',
                'viewxml': '',
                '$propsXml': '',
                'proparbitraryid': '',
                'filtarbitraryid': '',
                'viewbuilderpropid': '',
                'ID': '',
                'propRow': 1,
                'firstColumn': 3,
                'includePropertyName': false,
                'advancedIsHidden': false,

                'selectedSubfieldVal': '',
                'selectedFilterVal': '',

                'autoFocusInput': false
		    };
		
            if(options) $.extend(o, options);
        
            var $propFilterTable = $(this); //must call on a table
            
            var filtOpt = {
                'proparbitraryid': o.proparbitraryid,
                'filtarbitraryid': o.filtarbitraryid,
                'viewbuilderpropid': o.viewbuilderpropid,
                'ID': o.ID
            };
                       
            if( ( o.$propsXml === '' || o.$propsXml === undefined ) &&
                o.proparbitraryid !== '' && o.proparbitraryid !== undefined )
            {
                CswAjaxXml({ 
		            'url': o.getNewPropsUrl,
		            'data': "ViewXml=" + o.viewxml + "&PropArbitraryId=" + o.proparbitraryid,
                    'success': function($xml) { 
                                o.$propsXml = $xml.children('propertyfilters').children('property');
                                filtOpt.filtarbitraryid = o.$propsXml.CswAttrXml('filtarbitraryid');
                                renderPropFiltRow(filtOpt);
                    } //success
                }); //ajax
            }
            else
            {
                renderPropFiltRow(filtOpt);
            }

            function renderPropFiltRow(filtOpt)
            {
                var propertyId = o.$propsXml.CswAttrXml('viewbuilderpropid');
                var propertyName = o.$propsXml.CswAttrXml('propname');
                
                if( o.includePropertyName )
                {
                    //Row propRow, Column 3: property
                    var $propSelectCell = $propFilterTable.CswTable('cell', o.propRow, o.firstColumn) //3
                                                          .empty();
                    var propCellId = makePropFilterId(propertyName,filtOpt);
                    var $props = $propSelectCell.CswDOM('span',{ID: propCellId, value: propertyName});
                }
                
                var fieldtype = o.$propsXml.CswAttrXml('fieldtype');
                var $defaultFilter = o.$propsXml.children('defaultsubfield').CswAttrXml('filter');
                
                //Row propRow, Column 4: subfield default value (hidden) 
                var $subfieldCell = $propFilterTable.CswTable('cell', o.propRow, (o.firstColumn + 1)) //4
                                                    .empty();
                var defaultSubFieldId = makePropFilterId('default_filter', filtOpt);
                var $defaultSubField = $subfieldCell.CswDOM('span', {
                                                    ID: defaultSubFieldId,
                                                    value: $defaultFilter,
                                                    cssclass: ViewBuilder_CssClasses.default_filter.name })
                                                .CswAttrDom({align:"center"});
                if( !o.advancedIsHidden )
                {
                    $defaultSubField.hide();
                }

                //Row propRow, Column 4: subfield picklist 
                var subfieldOptionsId = makePropFilterId('subfield_select', filtOpt);
                var $subfieldsOptions = $(xmlToString(o.$propsXml.children('subfields').children('select')))
                                        .CswAttrDom('id', subfieldOptionsId)
                                        .CswAttrDom('name', subfieldOptionsId)
                                        .CswAttrDom('class',ViewBuilder_CssClasses.subfield_select.name)
                                        .change(function() {
                                            var $this = $(this);
                                            var r = {
                                                'selectedSubfieldVal': $this.val(),
                                                'selectedFilterVal': ''
                                            };
                                            $.extend(o,r);
                                            renderPropFiltRow(filtOpt) });

                if(o.selectedSubfieldVal !== '')
                {
                    $subfieldsOptions.val(o.selectedSubfieldVal).CswAttrDom('selected',true);
                }
                $subfieldCell.append($subfieldsOptions);
                if( o.advancedIsHidden )
                {
                    $subfieldsOptions.hide();
                }
                var subfield = $subfieldsOptions.find(':selected').val();
                var defaultValue = $subfieldsOptions.find(':selected').CswAttrDom('defaultvalue');

                //Row propRow, Column 5: filter picklist
                var $filtersCell = $propFilterTable.CswTable('cell', o.propRow, (o.firstColumn + 2)) //5
                                                   .empty();
                var filtersOptionsId = makePropFilterId('filter_select', filtOpt);
                var $filtersOptions =  $(xmlToString(o.$propsXml.children('propertyfilters').children('subfield[column=' + subfield + ']').children('select')))
                                        .CswAttrDom('id', filtersOptionsId)
                                        .CswAttrDom('name', filtersOptionsId)
                                        .CswAttrDom('class',ViewBuilder_CssClasses.filter_select.name)
                                        .change(function() {
                                            var $this = $(this);
                                            var r = {
                                                'selectedSubfieldVal': $subfieldsOptions.val(),
                                                'selectedFilterVal': $this.val()
                                            };
                                            $.extend(o,r);
                                            renderPropFiltRow(filtOpt) });

                if(o.selectedFilterVal !== '')
                {
                    $filtersOptions.val(o.selectedFilterVal).CswAttrDom('selected',true);
                }
                $filtersCell.append($filtersOptions);
                if( o.advancedIsHidden )
                {
                    $filtersOptions.hide();
                }
                //Row propRow, Column 6: filter input
                var $propFilterValueCell = $propFilterTable.CswTable('cell', o.propRow, (o.firstColumn + 3)) //6
                                                           .empty();
                
                var filtValInputId = makePropFilterId('propfilter_input', filtOpt);
                if( fieldtype === 'List' )
                {
                    $propFilterValueCell.append( $(xmlToString(o.$propsXml.children('filtersoptions').children('select'))) );
                }
                else if( fieldtype === 'Logical' )
                {
                    $propFilterValueCell.CswTristateCheckBox('init',{'ID': filtValInputId, 'Checked': defaultValue}); 
                }
                else
                {
                    var inputOpt = {
                        value: defaultValue,
                        placeholder: ''
                    };
                    if( inputOpt.value === '' || inputOpt.value === undefined )
                    {
                        o.placeholder = propertyName;
                        if(o.placeholder !== $subfieldsOptions.find(':selected').text() )
                        {
                            o.placeholder += "'s " +  $subfieldsOptions.find(':selected').text();
                        }  
                    }
                    var $filtValInput = $propFilterValueCell.CswInput('init', {ID: filtValInputId,
                                                                                type: CswInput_Types.text,
                                                                                cssclass: ViewBuilder_CssClasses.filter_value.name,
                                                                                value: inputOpt.value,
                                                                                placeholder: inputOpt.placeholder,
                                                                                width: "200px",
                                                                                autofocus: o.autoFocusInput,
                                                                                autocomplete: 'on'
                                                                       });
                }
            }
            return $propFilterTable;
        }, // 'init': function(options) {
        'getFilterJson': function(options)
        {
            var $thisProp = $(this);
            var o = {
                nodetypeorobjectclassid: '',
                relatedidtype: '',
                fieldtype: $thisProp.CswAttrXml('fieldtype'),
                ID: '',
                $parent: '',
                proparbitraryid: '',
                filtarbitraryid: $thisProp.CswAttrXml('filtarbitraryid'),
                viewbuilderpropid: ''
            };
            if(options) $.extend(o,options);

            var filtOpt = {
                'proparbitraryid': o.proparbitraryid,
                'filtarbitraryid': o.filtarbitraryid,
                'viewbuilderpropid': o.viewbuilderpropid,
                'ID': o.ID
            };

            var filtValInputId = makePropFilterId('propfilter_input', filtOpt);
            var filtValListId = makePropFilterId('filtersoptions_select',filtOpt);
            var subFieldId = makePropFilterId('subfield_select',filtOpt);
            var filterId = makePropFilterId('filter_select',filtOpt);

            var thisNodeProp = {}; //to return
            
            var $filtInput = o.$parent.CswInput('findandget',{ID: filtValInputId});
            var filterValue;
            switch( o.fieldtype )
            { 
                case 'Logical': 
                {
                    filterValue = $filtInput.CswTristateCheckBox('value');
                    break;
                }
                case 'List':
                {
                    var $filtList = o.$parent.CswDOM('findelement',{ID: filtValListId});
                    filterValue = $filtList.find(':selected').val();
                    break;
                }
                default:
                {
                    filterValue = $filtInput.val();
                    break;
                }
            }
            if(filterValue !== '')
            {
                var $subField = o.$parent.CswDOM('findelement',{ID: subFieldId});
				var subFieldText = $subField.find(':selected').text();

                var $filter = o.$parent.CswDOM('findelement',{ID: filterId});
                var filterText = $filter.find(':selected').val();

                var propType = $thisProp.CswAttrXml('proptype');
                                
                thisNodeProp = {
                    nodetypeorobjectclassid: o.nodetypeorobjectclassid, // for NodeType filters
                    relatedidtype: o.relatedidtype, // for NodeType filters
                    proptype: propType,
                    viewbuilderpropid: o.viewbuilderpropid,
                    filtarbitraryid: o.filtarbitraryid,
                    proparbitraryid: o.proparbitraryid,
                    relatedidtype: o.relatedidtype,
                    subfield: subFieldText,
                    filter: filterText,
                    filtervalue: filterValue  
                };
                
            } // if(filterValue !== '')
            return thisNodeProp;
        }, // 'getFilterJson': function(options) { 
        'makeFilter': function(options)
        {
            var o = {
                url: '/NbtWebApp/wsNBT.asmx/makeViewPropFilter',
                viewxml: '',
                filtJson: '',
                onSuccess: function($filterXml) {}
            };
            if(options) $.extend(o,options);

            //var $filterXml;

            CswAjaxXml({ 
			'url': o.url,
			'data': "ViewXml="  + o.viewxml + "&PropFiltJson=" + jsonToString(o.filtJson),
            'success': function($filter) { 
                    //$filterXml = $filter;
                    o.onSuccess($filter);
                }
            });

            //return $filterXml;
        } // 'makefilter': function(options)
    } // methods 
	 
    $.fn.CswViewPropFilter = function (method) {
		
		if ( methods[method] ) {
		  return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
		  return methods.init.apply( this, arguments );
		} else {
		  $.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    
  
	};
})(jQuery);


