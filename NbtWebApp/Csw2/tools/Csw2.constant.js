/* jshint undef: true, unused: true */
/* global Csw2:true, window:true, Ext:true, $: true */

(function() {

    /**
     * Create a new object with constant properties.
     * @param props {Object} an object represent the enun members
    */
    var Constant = function(props) {
        var that = null;
        var keys = [];

        if (props) {
            that = this;

            Object.defineProperties(that, {
                has: {
                    /**
                     * Assert that the provided key is a member of the enum
                     * @param key {String} enum property name
                    */
                    value: function(key) {
                        return keys.indexOf(key) !== -1;
                    }
                }
            });

            Csw2.each(props, function(propVal, propName) {
                keys.push(propName);
                Object.defineProperty(that, propName, {
                    value: propVal
                });
            });
        }
        return that;
    };

    /**
     * Create a new enum on the constants namespace.
     * Enums are objects consisting of read-only, non-configurable, non-enumerable properties.
     * @param name {String} the name of the enum
     * @param props {Object} the properties of the enum
    */
    Csw2.lift('constant', function(name, props) {
        var ret = new Constant(props);
        if(ret && name) {
            Csw2.constants.lift(name, ret);
            Object.seal(ret);
            Object.freeze(ret);
        }
        return ret;
    });

    }());