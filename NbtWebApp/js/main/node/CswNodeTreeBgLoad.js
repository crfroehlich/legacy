/// <reference path="/js/../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />

(function ($) { /// <param name="$" type="jQuery" />

    var pluginName = 'CswNodeTree';

    var methods = {
        'init': function (options)     // options are defined in _getTreeContent()
        {
            var o = {
                ID: '',
                RunTreeUrl: '/NbtWebApp/wsNBT.asmx/runTree',
                fetchTreeFirstLevelUrl: '/NbtWebApp/wsNBT.asmx/fetchTreeFirstLevel',
                fetchTreeLevelUrl: '/NbtWebApp/wsNBT.asmx/fetchTreeLevel',
                NodeTreeUrl: '/NbtWebApp/wsNBT.asmx/getTreeOfNode',
                viewid: '',       // loads an arbitrary view
                viewmode: '',
                showempty: false, // if true, shows an empty tree (primarily for search)
                forsearch: false, // if true, used to override default behavior of list views
                nodeid: '',       // if viewid are not supplied, loads a view of this node
                cswnbtnodekey: '',
                IncludeNodeRequired: false,
                UsePaging: true,
                UseScrollbars: true,
                onSelectNode: null, // function(optSelect) { var o =  { nodeid: '',  nodename: '', iconurl: '', cswnbtnodekey: '', viewid: '' }; return o; },
                onInitialSelectNode: undefined,
                onViewChange: null, // function(newviewid, newviewmode) {},    // if the server returns a different view than what we asked for (e.g. case 21262)
                SelectFirstChild: true,
                ShowCheckboxes: false,
                IncludeInQuickLaunch: true
            };
            if (options) $.extend(o, options);

            //            if (o.onInitialSelectNode === undefined) {
            //                o.onInitialSelectNode = o.onSelectNode;
            //            }

            var idPrefix = o.ID + '_';
            var $treediv = $('<div id="' + idPrefix + '" />')
                                .appendTo($(this));
            if (o.UseScrollbars) {
                $treediv.addClass('treediv');
            } else {
                $treediv.addClass('treediv_noscroll');
            }

            var url = o.RunTreeUrl;
            var dataParam = {
                //                UsePaging: o.UsePaging,
                ViewId: o.viewid,
                IdPrefix: tryParseString(idPrefix),
                //                IsFirstLoad: true,
                //                ParentNodeKey: '',
                IncludeNodeRequired: o.IncludeNodeRequired,
                IncludeNodeKey: tryParseString(o.cswnbtnodekey),
                //                ShowEmpty: o.showempty,
                //                ForSearch: o.forsearch,
                //                NodePk: tryParseString(o.nodeid),
                IncludeInQuickLaunch: o.IncludeInQuickLaunch
            };

            if (isNullOrEmpty(o.viewid)) {
                url = o.NodeTreeUrl;
            }


            function getFirstLevel($treediv, pagesize, pageno) {
                var realpagesize = tryParseNumber(pagesize, 0);
                var realpageno = tryParseNumber(pageno, 0);

                CswAjaxJson({
                    url: o.fetchTreeFirstLevelUrl,
                    data: {
                        ViewId: o.viewid,
                        IdPrefix: tryParseString(idPrefix),
                        PageSize: realpagesize,
                        PageNo: realpageno,
                        ForSearch: o.forsearch
                    },
                    stringify: false,
                    success: function (data) {
                        // this page
                        recurseNodes($treediv, data.tree);

                        // next page
                        if (isTrue(data.more)) {
                            getFirstLevel($treediv, realpagesize, realpageno + 1);
                        }

                        // children
                        // Note: root is level 1
                        //       "first" level is actually level 2
                        //       so the next level is level 3
                        getLevel($treediv, 3, data.nodecountstart, data.nodecountend);

                    } // success
                }); // ajax
            } // getNextPage()

            function getLevel($treediv, level, parentstart, parentend) {
                var reallevel = tryParseNumber(level, 0);
                var realparentstart = tryParseNumber(parentstart, 0);
                var realparentend = tryParseNumber(parentend, 0);

                CswAjaxJson({
                    url: o.fetchTreeLevelUrl,
                    data: {
                        ViewId: o.viewid,
                        IdPrefix: tryParseString(idPrefix),
                        Level: reallevel,
                        ParentRangeStart: realparentstart,
                        ParentRangeEnd: realparentend,
                        ForSearch: o.forsearch
                    },
                    stringify: false,
                    success: function (data) {
                        // this level
                        recurseNodes($treediv, data.tree);
                        
                        // children
                        if (tryParseNumber(data.nodecountend, -1) > 0) {
                            getLevel($treediv, reallevel + 1, data.nodecountstart, data.nodecountend);
                        }
                    } // success
                }); // ajax
            } // getNextPage()

            function recurseNodes($treediv, nodescoll) {
                var parent = false;
                each(nodescoll, function (childObj, childKey, thisObj, value) {
                    if (false === isNullOrEmpty(childObj)) {
                        if (false === isNullOrEmpty(childObj.attr.parentkey)) {
                            parent = findParent($treediv, childObj.attr.parentkey);
                        }
                        addNodeToTree($treediv, parent, childObj);
                        if (false === isNullOrEmpty(childObj.children) && childObj.children.length > 0) {
                            recurseNodes($treediv, childObj.children);
                        }
                    }
                }); // each
            } // recurseNodes()

            function findParent($treediv, parentkey) {
                // Using attribute selector $('li[cswnbtnodekey=""]') doesn't seem to work, so we'll do it manually
                var ret = false;
                $.each($treediv.find('li'), function (childkey, value) {
                    childObj = $(value);
                    if (childObj.attr('cswnbtnodekey') === parentkey) {
                        ret = childObj;
                    }
                });
                return ret;
            }

            var rootnode = false;
            function addNodeToTree($treediv, parentnode, childjs) {
                if (isNullOrEmpty(parentnode) || parentnode === false) {
                    parentnode = rootnode;
                }
                var newnode = $treediv.jstree("create", parentnode, "last", childjs, false, true);
                if (rootnode === false) {
                    rootnode = newnode;
                }
            }
            function removeNodeFromTree($treediv, treenode) {
                return $treediv.jstree("remove", treenode);
            }

            CswAjaxJson({
                url: url,
                data: dataParam,
                stringify: false,
                success: function (data) {
                    if (isTrue(data.result)) {
                        var treePlugins = ["themes", "ui", "types", "crrm"];
                        var jsonTypes = data.types;

                        var treeThemes;
                        if (o.viewmode === 'list') {
                            treeThemes = { "dots": false };
                        } else {
                            treeThemes = { "dots": true };
                        }

                        $treediv.jstree({
                            "ui": {
                                "select_limit": 1//,
                                //"initially_select": selectid
                            },
                            "themes": treeThemes,
                            "types": {
                                "types": jsonTypes,
                                "max_children": -2,
                                "max_depth": -2
                            },
                            "plugins": treePlugins
                        }); // jstree()

                        //$treediv.jstree("create", false, "first", data.root, false, true);
                        addNodeToTree($treediv, false, data.root);

                        getFirstLevel($treediv, 2, 0);

                        removeNodeFromTree($treediv, $('.jstree-loading'));

                    } // if (isTrue(data.result)) {
                } // success
            }); // ajax

            //                    var idToSelect = '';
            //                    //var treePlugins = ["themes", "xml_data", "ui", "types", "crrm"];
            //                    var treePlugins = ["themes", "html_data", "ui", "types", "crrm"];
            //                    var treeThemes;
            //                    if (false === isNullOrEmpty(o.nodeid)) {
            //                        idToSelect = idPrefix + o.nodeid;
            //                    }

            //                    var newviewid = data.viewid;
            //                    if (false === isNullOrEmpty(newviewid) && o.viewid !== newviewid) {
            //                        o.viewid = newviewid;
            //                        if (isFunction(o.onViewChange)) {
            //                            o.onViewChange(o.viewid, 'tree');
            //                        }
            //                    }

            //                    var treeData = data.tree;
            //                    var jsonTypes = data.types;

            //                    //var $selecteditem = data.find('item[id="'+ selectid + '"]');
            //                    var selectLevel = -1;
            //                    if (o.SelectFirstChild) {
            //                        if (o.viewmode === 'list') {
            //                            selectLevel = 1;
            //                            treeThemes = { "dots": false };
            //                        } else {
            //                            selectLevel = 2;
            //                            treeThemes = { "dots": true };
            //                        }
            //                    }
            //                    else {
            //                        if (isNullOrEmpty(idToSelect)) {
            //                            idToSelect = idPrefix + 'root';
            //                        }
            //                    }

            //                    var hasNodes = false;
            //                    function checkHasNodes(json) {
            //                        var ret = false;
            //                        if ('No Results' !== json &&
            //                            contains(json, 'attr') &&
            //                            (false === contains(json, 'children') ||
            //                           (json.children.length > 1 ||
            //                             'No Results' !== json.children[0]))) {
            //                            ret = true;
            //                        }
            //                        return ret;
            //                    }

            //var selectid = '';
            //            function treeJsonToHtml(json, level) {
            //                hasNodes = checkHasNodes(json);
            //                var treestr = '';
            //                if (hasNodes) {
            //                    var id = tryParseString(json.attr.id),
            //			                                nodeid = tryParseString(id.substring(idPrefix.length)),
            //			                                nodename = tryParseString(json.data),
            //			                                nbtnodekey = tryParseString(json.attr.cswnbtnodekey),
            //			                                rel = tryParseString(json.attr.rel),
            //			                                species = tryParseString(json.attr.species),
            //			                                state = tryParseString(json.attr.state, 'open');

            //                    if (idToSelect === id || (level === selectLevel && isNullOrEmpty(selectid))) {
            //                        selectid = id;
            //                    }

            //                    var locked = isTrue(json.attr.locked);
            //                    treestr += '<li id="' + id + '" rel="' + rel + '" species="' + species + '" class="jstree-' + state + '" ';
            //                    if (!isNullOrEmpty(nbtnodekey)) {
            //                        treestr += '    cswnbtnodekey="' + nbtnodekey.replace(/"/g, '&quot;') + '"';
            //                    }
            //                    treestr += '>';
            //                    if (o.ShowCheckboxes) {
            //                        treestr += '<input type="checkbox" class="' + idPrefix + 'check" id="check_' + nodeid + '" rel="' + rel + '" nodeid="' + nodeid + '" nodename="' + nodename + '"></input>';
            //                    }
            //                    treestr += '  <a href="#">' + nodename + '</a>';
            //                    var children = json.children;
            //                    for (var child in children) {
            //                        // recurse
            //                        if (children.hasOwnProperty(child)) {
            //                            var childNode = children[child];
            //                            treestr += '<ul>';
            //                            treestr += treeJsonToHtml(childNode, 2);
            //                            treestr += '</ul>';
            //                        }
            //                    }
            //                    if (locked) {
            //                        treestr += '<img src="Images/quota/lock.gif" title="Quota exceeded" />';
            //                    }
            //                    treestr += '</li>';
            //                } else {
            //                    treestr += '<li id="' + Int32MinVal + '" rel="leaf" class="jstree-leaf">No Results</li>';
            //                }
            //                return treestr;
            //            } // _treeXmlToHtml()

            //                    var treehtmlstring = '<ul>';
            //                    for (var parent in treeData) {
            //                        if (treeData.hasOwnProperty(parent)) {
            //                            var parentNode = treeData[parent];
            //                            treehtmlstring += treeJsonToHtml(parentNode, 1);
            //                        }
            //                    }
            //                    treehtmlstring += '</ul>';

            //                    if (hasNodes) {
            //                        $treediv.bind('init_done.jstree', function () {

            //                            // initially_open and initially_select cause multiple event triggers and race conditions.
            //                            // So we'll do it ourselves instead.
            //                            // Open
            //                            if (!isNullOrEmpty(selectid)) {
            //                                var $selecteditem = $treediv.find('#' + selectid);
            //                                var $itemparents = $selecteditem.parents().andSelf();
            //                                $itemparents.each(function () {
            //                                    $treediv.jstree('open_node', '#' + $(this).CswAttrXml('id'));
            //                                });

            //                                // Select
            //                                $treediv.jstree('select_node', '#' + selectid);
            //                            }
            //                        }).bind('load_node.jstree', function () {
            //                            $('.' + idPrefix + 'check').unbind('click');
            //                            $('.' + idPrefix + 'check').click(function () { return handleCheck($treediv, $(this)); });

            //                        }).bind('select_node.jstree', function (e, newData) {
            //                            return firstSelectNode({
            //                                e: e,
            //                                data: newData,
            //                                url: url,
            //                                $treediv: $treediv,
            //                                IdPrefix: idPrefix,
            //                                onSelectNode: o.onSelectNode,
            //                                onInitialSelectNode: o.onInitialSelectNode,
            //                                viewid: o.viewid,
            //                                UsePaging: o.UsePaging,
            //                                forsearch: o.forsearch
            //                            });

            //                        }).bind('hover_node.jstree', function (e, data) {
            //                            var $hoverLI = $(data.rslt.obj[0]);
            //                            var nodeid = $hoverLI.CswAttrDom('id').substring(idPrefix.length);
            //                            var cswnbtnodekey = $hoverLI.CswAttrDom('cswnbtnodekey');
            //                            nodeHoverIn(data.args[1], nodeid, cswnbtnodekey);

            //                        }).bind('dehover_node.jstree', function (e, data) {
            //                            var selected = jsTreeGetSelected($treediv);
            //                            nodeHoverOut();

            //                        }).jstree({
            //                            "html_data":
            //                                    {
            //                                        "data": treehtmlstring,
            //                                        "ajax":
            //                                            {
            //                                                type: 'POST',
            //                                                url: url,
            //                                                dataType: "json",
            //                                                contentType: 'application/json; charset=utf-8',
            //                                                data: function ($nodeOpening) {
            //                                                    var nodekey = $nodeOpening.CswAttrXml('cswnbtnodekey');
            //                                                    var retDataParam = {
            //                                                        UsePaging: o.UsePaging,
            //                                                        ViewId: o.viewid,
            //                                                        IdPrefix: idPrefix,
            //                                                        IsFirstLoad: false,
            //                                                        ParentNodeKey: nodekey,
            //                                                        IncludeNodeRequired: false,
            //                                                        IncludeNodeKey: '',
            //                                                        ShowEmpty: false,
            //                                                        ForSearch: o.forsearch,
            //                                                        NodePk: tryParseString(o.nodeid),
            //                                                        IncludeInQuickLaunch: false
            //                                                    };
            //                                                    return JSON.stringify(retDataParam);
            //                                                },
            //                                                success: function (rawData) {
            //                                                    var newData = JSON.parse(rawData.d);
            //                                                    var nodeData = newData.tree;
            //                                                    var childhtmlstr = '';
            //                                                    for (var nodeItem in nodeData) {
            //                                                        if (nodeData.hasOwnProperty(nodeItem)) {
            //                                                            var thisNode = nodeData[nodeItem];
            //                                                            childhtmlstr += treeJsonToHtml(thisNode);
            //                                                        }
            //                                                    }
            //                                                    return childhtmlstr;
            //                                                }
            //                                            }
            //                                    },
            //                            "ui": {
            //                                "select_limit": 1//,
            //                                //"initially_select": selectid
            //                            },
            //                            "themes": treeThemes,
            //                            "core": {
            //                                //"initially_open": initiallyOpen
            //                            },
            //                            "types": {
            //                                "types": jsonTypes,
            //                                "max_children": -2,
            //                                "max_depth": -2
            //                            },
            //                            "plugins": treePlugins
            //                        });

            //                        // DO NOT define an onSuccess() function here that interacts with the tree.
            //                        // The tree has initalization events that appear to happen asynchronously,
            //                        // and thus having an onSuccess() function that changes the selected node will
            //                        // cause a race condition.

            //                        $('.' + idPrefix + 'check').click(function () { return handleCheck($treediv, $(this)); });

            //                        // case 21424 - Manufacture unique IDs on the expand <ins> for automated testing
            //                        $treediv.find('li').each(function () {
            //                            var $li = $(this);
            //                            $li.children('ins').CswAttrDom('id', $li.CswAttrDom('id') + '_expand');
            //                        });

            //                    } else {
            //                        $treediv.append('No Results');
            //                        o.onInitialSelectNode({ viewid: o.viewid });
            //                    }

            //                } // success{}
            //            }); // ajax

            return $treediv;
        },

        'selectNode': function (optSelect) {
            var o = {
                newnodeid: '',
                newcswnbtnodekey: ''
            };
            if (optSelect) {
                $.extend(o, optSelect);
            }
            var $treediv = $(this).children('.treediv');
            var idPrefix = $treediv.CswAttrDom('id');
            $treediv.jstree('select_node', '#' + idPrefix + o.newnodeid);
        }
    };

    function firstSelectNode(myoptions) {
        var m = {
            e: '',
            data: '',
            url: '',
            $treediv: '',
            IdPrefix: '',
            onSelectNode: null, //function() {},
            onInitialSelectNode: null, //function() {},
            viewid: '',
            UsePaging: '',
            forsearch: ''
        };
        if (myoptions) $.extend(m, myoptions);

        // case 21715 - don't trigger onSelectNode event on first event
        var m2 = {};
        $.extend(m2, m);
        m2.onSelectNode = m.onInitialSelectNode;
        handleSelectNode(m2);

        // rebind event for next select
        m.$treediv.unbind('select_node.jstree');
        m.$treediv.bind('select_node.jstree', function () { return handleSelectNode(m); });
    }

    function handleSelectNode(myoptions) {
        var m = {
            e: '',
            data: '',
            url: '',
            $treediv: '',
            IdPrefix: '',
            onSelectNode: function () { },
            viewid: '',
            UsePaging: '',
            forsearch: ''
        };
        if (myoptions) $.extend(m, myoptions);

        var selected = jsTreeGetSelected(m.$treediv);
        var optSelect = {
            nodeid: selected.id,
            nodename: selected.text,
            iconurl: selected.iconurl,
            cswnbtnodekey: selected.$item.CswAttrDom('cswnbtnodekey'),
            nodespecies: selected.$item.CswAttrDom('species'),
            viewid: m.viewid
        };

        if (optSelect.nodespecies === "More") {
            var parentNodeKey = '';
            var parent = m.data.inst._get_parent(m.data.rslt.obj);
            if (parent !== -1) {
                parentNodeKey = tryParseString(parent.CswAttrDom('cswnbtnodekey'), '');
            }

            var nextDataParam = {
                UsePaging: m.UsePaging,
                ViewId: m.viewid,
                IdPrefix: m.IdPrefix,
                IsFirstLoad: false,
                ParentNodeKey: parentNodeKey,
                IncludeNodeRequired: false,
                IncludeNodeKey: optSelect.cswnbtnodekey,
                ShowEmpty: false,
                ForSearch: m.forsearch,
                NodePk: selected.id,
                IncludeInQuickLaunch: false
            };

            // get next page of nodes
            CswAjaxJson({
                url: m.url,
                data: nextDataParam,
                success: function (data) {
                    var afterNodeId = m.IdPrefix + optSelect.nodeid;
                    var itemJson = data.tree;

                    // we have to do these one at a time in successive OnSuccess callbacks, 
                    // or else they won't end up in the right place on the tree
                    doContinue(0);

                    function doContinue(index) {
                        var thisItem;
                        if (contains(itemJson, index)) {
                            thisItem = itemJson[index];
                            m.$treediv.jstree('create',
                                              '#' + afterNodeId,
                                              'after',
                                              thisItem,
                                                function () {
                                                    // remove 'More' node
                                                    if (afterNodeId === thisItem.attr.id) {
                                                        m.$treediv.jstree('remove', '#' + m.IdPrefix + optSelect.nodeid + '[species="More"]');
                                                    }

                                                    afterNodeId = thisItem.attr.id;
                                                    index += 1;
                                                    doContinue(index);
                                                },
                                               true,
                                               true);

                        } // if($itemxml.length > 0)
                    } // _continue()

                } // success
            }); // ajax
        }
        else {
            clearChecks(m.IdPrefix);
            m.onSelectNode(optSelect);
        }
    }

    function handleCheck($treediv, $checkbox) {
        var $selected = jsTreeGetSelected($treediv);
        return ($selected.$item.CswAttrDom('rel') === $checkbox.CswAttrDom('rel'));
    }

    function clearChecks(IdPrefix) {
        $('.' + IdPrefix + 'check').CswAttrDom('checked', '');
    }

    // Method calling logic
    $.fn.CswNodeTree = function (method) {

        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof method === 'object' || !method) {
            return methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + method + ' does not exist on ' + pluginName);
        }

    };

})(jQuery);

