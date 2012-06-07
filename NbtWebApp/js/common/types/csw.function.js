﻿/// <reference path="~/js/CswCommon-vsdoc.js" />
/// <reference path="~/js/CswNbt-vsdoc.js" />

(function _cswFunction() {
    'use strict';

    Csw.isFunction = Csw.isFunction ||
        Csw.register('isFunction', function (obj) {
            'use strict';
            /// <summary> Returns true if the object is a function</summary>
            /// <param name="obj" type="Object"> Object to test</param>
            /// <returns type="Boolean" />
            var ret = ($.isFunction(obj));
            return ret;
        });

    Csw.tryExec = Csw.tryExec ||
        Csw.register('tryExec', function (func) {
            'use strict';
            /// <summary> If the supplied argument is a function, execute it. </summary>
            /// <param name="func" type="Function"> Function to evaluate </param>
            /// <returns type="undefined" />
            try {
                if (Csw.isFunction(func)) {
                    return func.apply(this, Array.prototype.slice.call(arguments, 1));
                }
            } catch (exception) {
                Csw.error.catchException(exception);
            }
        });

    Csw.tryJqExec = Csw.tryJqExec ||
        Csw.register('tryJqExec', function (cswObj, method) {
            'use strict';
            /// <summary> If the supplied argument is a function, execute it. </summary>
            /// <param name="func" type="Function"> Function to evaluate </param>
            /// <returns type="undefined" />
            try {
                var args = arguments[2];
                return cswObj.$[method].apply(cswObj.$, args);
            } catch (exception) {
                Csw.error.catchException(exception);
            }
        });

} ());
