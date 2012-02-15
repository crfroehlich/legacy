/// <reference path="~/Scripts/jquery-1.7.1-vsdoc.js" />
/// <reference path="~/csw.js/ChemSW-vsdoc.js" />

(function _cswLink() {
    'use strict';

    function link(options) {
        /// <summary> Create or extend an HTML <a /> and return a Csw.link object
        ///     &#10;1 - link(options)
        ///</summary>
        /// <param name="options" type="Object">
        /// <para>A JSON Object</para>
        /// <para>options.$parent: An element to attach to.</para>
        /// <para>options.ID: An ID for the input.</para>
        /// <para>options.cssclass: CSS class to asign</para>
        /// <para>options.text: Text to display</para>
        /// </param>
        /// <returns type="link">A link object</returns>
        var internal = {
            $parent: '',
            ID: '',
            cssclass: '',
            text: '',
            href: '',
            type: '',
            title: '',
            rel: '',
            media: '',
            target: '_blank',
            onClick: null //function () {}
        };
        var external = {};

        (function () {
            var html = '',
                style = Csw.controls.dom.style(),
                attr = Csw.controls.dom.attributes();
            var $link;

            if (options) {
                $.extend(internal, options);
            }

            internal.ID = Csw.string(internal.ID, internal.name);

            html += '<a ';
            attr.add('id', internal.ID);
            attr.add('class', internal.cssclass);
            if (false == Csw.isNullOrEmpty(internal.href)) {
                attr.add('href', internal.href);
            } else {
                attr.add('href', '#');
            }
            attr.add('type', internal.type);
            attr.add('title', internal.title);
            attr.add('rel', internal.rel);
            attr.add('media', internal.media);
            attr.add('target', internal.target);

            html += attr.get();
            html += style.get();

            html += '>';

            html += Csw.string(internal.text, internal.value);

            html += '</a>';
            $link = $(html);

            Csw.controls.factory($link, external);
            if (Csw.isFunction(internal.onClick)) {
                external.bind('click', internal.onClick);
            }

            internal.$parent.append(external.$);

        } ());

        external.click = function (func) {
            if (Csw.isFunction(func)) {
                external.bind('click', func);
            }
        };

        return external;
    }
    Csw.controls.register('link', link);
    Csw.controls.link = Csw.controls.link || link;

} ());

