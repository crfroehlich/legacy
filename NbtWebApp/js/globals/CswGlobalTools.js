function isArray(obj) {
    /// <summary> Returns true if the object is an array</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = ($.isArray(obj));
    return ret;
}

function isJQuery(obj) {
    var ret = (obj instanceof jQuery);
    return ret;
}

function hasLength(obj) {
    /// <summary> Returns true if the object is an Array or jQuery</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = (isArray(obj) || isJQuery(obj));
    return ret;
}

var dateTimeMinValue = new Date('1/1/0001 12:00:00 AM');
function isDate(obj) {
    /// <summary> Returns true if the object is a Date</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = (obj instanceof Date);
    return ret;
}

function isNumber(obj) {
    /// <summary> Returns true if the object is typeof number</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = (typeof obj === 'number');
    return ret;
}

function isFunction(obj) {
    /// <summary> Returns true if the object is a function</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = ($.isFunction(obj));
    return ret;
}

function isPlainObject(obj) {
    /// <summary> 
    ///    Returns true if the object is a JavaScript object.
    ///     &#10; isPlainObject(CswInput_Types) === true 
    ///     &#10; isPlainObject('CswInput_Types') === false
    /// </summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = ($.isPlainObject(obj));
    return ret;
}

function isGeneric(obj) {
    /// <summary> Returns true if the object is not a function, array, jQuery or JSON object</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = (false === isFunction(obj) && false === hasLength(obj) && false === isPlainObject(obj));
    return ret;
}

function isNullOrEmpty(obj, checkLength) {
    /// <summary> Returns true if the input is null, undefined, or ''</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = isNullOrUndefined(obj);
    if (false === ret && isGeneric(obj)) {
        ret = ((trim(obj) === '') ||(isDate(obj) && obj === dateTimeMinValue) ||(isNumber(obj) && obj === Int32MinVal));
    }
    else if (checkLength && hasLength(obj)) {
        ret = (obj.length === 0);
    }
    return ret;
}

function isNullOrUndefined(obj) {
    /// <summary> Returns true if the input is null or undefined</summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = false;
    if (false === isFunction(obj)) {
        ret = obj === null || obj === undefined || ($.isPlainObject(obj) && $.isEmptyObject(obj));
    }
    return ret;
}

function makeId(options) {
    /// <summary>
    ///   Generates an ID for DOM assignment
    /// </summary>
    /// <param name="options" type="Object">
    ///     A JSON Object
    ///     &#10;1 - options.ID: Base ID string
    ///     &#10;2 - options.prefix: String prefix to prepend
    ///     &#10;3 - options.suffix: String suffix to append
    ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
    /// </param>
    /// <returns type="String>A concatenated string of provided values</returns>
    var o = {
        'ID': '',
        'prefix': '',
        'suffix': '',
        'Delimiter': '_'
    };
    if (options) $.extend(o, options);
    var elementId = o.ID;
    if (false === isNullOrEmpty(o.prefix) && false === isNullOrEmpty(elementId)) {
        elementId = o.prefix + o.Delimiter + elementId;
    }
    if (false === isNullOrEmpty(o.suffix) && false === isNullOrEmpty(elementId)) {
        elementId += o.Delimiter + o.suffix;
    }
    return elementId;
}

function makeSafeId(options) {
    /// <summary>   Generates a "safe" ID for DOM assignment </summary>
    /// <param name="options" type="Object">
    ///     A JSON Object
    ///     &#10;1 - options.ID: Base ID string
    ///     &#10;2 - options.prefix: String prefix to prepend
    ///     &#10;3 - options.suffix: String suffix to append
    ///     &#10;4 - options.Delimiter: String to use as delimiter for concatenation
    /// </param>
    /// <returns type="String>A concatenated string of provided values</returns>
    var o = {
        'ID': '',
        'prefix': '',
        'suffix': '',
        'Delimiter': '_'
    };
    if (options) {
        $.extend(o, options);
    }
    var elementId = o.ID;
    var toReplace = [/'/gi, / /gi, /\//g];
    if (false === isNullOrEmpty(o.prefix) && false === isNullOrEmpty(elementId)) {
        elementId = o.prefix + o.Delimiter + elementId;
    }
    if (false === isNullOrEmpty(o.suffix) && false === isNullOrEmpty(elementId)) {
        elementId += o.Delimiter + o.suffix;
    }
    for (var i = 0; i < toReplace.length; i++) {
        if (toReplace.hasOwnProperty(i)) {
            if (!isNullOrEmpty(elementId)) {
                elementId = elementId.replace(toReplace[i], '');
            }
        }
    }
    return elementId;
}

function isString(obj) {
    /// <summary> Returns true if the object is a String object. </summary>
    /// <param name="obj" type="Object"> Object to test</param>
    /// <returns type="Boolean" />
    var ret = typeof obj === 'string';
    return ret;
}


function isNumeric(obj) {
    /// <summary> Returns true if the input can be parsed as a Number </summary>
    /// <param name="str" type="Object"> String or object to test </param>
    /// <returns type="Boolean" />
    var ret = false;
    if (isNumber(obj) && false === isNullOrEmpty(obj)) {
        var num = parseInt(obj);
        if (false === isNaN(num)) {
            ret = true;
        }
    }
    return ret;
}
function isTrue(str, isTrueIfNull) {
    /// <summary>
    ///   Returns true if the input is true, 'true', '1' or 1.
    ///   &#10;1 Returns false if the input is false, 'false', '0' or 0.
    ///   &#10;2 Otherwise returns false and (if debug) writes an error to the log.
    /// </summary>
    /// <param name="str" type="Object">
    ///     String or object to test
    /// </param>
    /// <returns type="Bool" />
    var ret;
    var truthy = tryParseString(str).toString().toLowerCase().trim();
    if (truthy === 'true' || truthy === '1') {
        ret = true;
    }
    else if (truthy === 'false' || truthy === '0') {
        ret = false;
    }
    else if (isTrueIfNull && isNullOrEmpty(str)) {
        ret = true;
    } else {
        ret = false;
    }
    return ret;
}
function tryParseString(inputStr, defaultStr) {
    /// <summary>
    ///   Returns the inputStr if !isNullOrEmpty, else returns the defaultStr
    /// </summary>
    /// <param name="inputStr" type="String"> String to parse </param>
    /// <param name="defaultStr" type="String"> Default value if null or empty </param>
    /// <returns type="String" />
    var ret = '';
    if (false === isPlainObject(inputStr) &&
        false === isFunction(inputStr) && 
        false === isNullOrEmpty(inputStr)) {
        ret = inputStr.toString();
    } 
    else if (false === isPlainObject(defaultStr) && 
        false === isFunction(defaultStr) &&
        false === isNullOrEmpty(defaultStr)) {
        ret = defaultStr.toString();
    }
    return ret;
}
var Int32MinVal = new Number(-2147483648);
function tryParseNumber(inputNum, defaultNum) {
    /// <summary>
    ///   Returns the inputNum if !NaN, else returns the defaultNum
    /// </summary>
    /// <param name="inputNum" type="Number"> String to parse to number </param>
    /// <param name="defaultNum" type="Number"> Default value if not a number </param>
    /// <returns type="Number" />
    var ret = new Number(defaultNum);
    var tryRet = new Number(inputNum);
    if (false === isNaN(tryRet) && tryRet !== Int32MinVal) {
        ret = tryRet;
    }
    return ret;
}
function cswIndexOf(object, property) {
    /// <summary>
    ///   Because IE 8 doesn't implement indexOf on the Array prototype.
    /// </summary>
    var ret = -1,i,len = object.length;
    if (isFunction(object.indexOf)) {
        ret = object.indexOf(property);
    }
    else if (hasLength(object) && len > 0) {
        for (i = 0; i < len; i++) {
            if (object[i] === property) {
                ret = i;
                break;
            }
        }
    }
    return ret;
}
function contains(object, index) {
    /// <summary>Determines whether an object or an array contains a property or index. Null-safe.</summary>
    /// <param name="object" type="Object"> An object to evaluate </param>
    /// <param name="index" type="String"> An index or property to find </param>
    /// <returns type="Boolean" />
    var ret = false;
    if (false === isNullOrUndefined(object)) {
        if (isArray(object)) {
            ret = cswIndexOf(object, index) !== -1;
        }
        if (false === ret && object.hasOwnProperty(index)) {
            ret = true;
        }
    }
    return ret;
}
function tryParseObjByIdx(object, index, defaultStr) {
    /// <summary> Attempts to fetch the value at an array index. Null-safe.</summary>
    /// <param name="object" type="Object"> Object or array to parse </param>
    /// <param name="index" type="String"> Index or property to find </param>
    /// <param name="defaultStr" type="String"> Optional. String to use instead of '' if index does not exist. </param>
    /// <returns type="String">Parsed string</returns>
    var ret = '';
    if (false === isNullOrEmpty(defaultStr)) {
        ret = defaultStr;
    }
    if (contains(object, index)) {
        if (false === isNullOrUndefined(object[index])) {
            ret = object[index];
        }
    }
    return ret;
}
function tryParseElement(elementId, $context) {
    /// <summary>Attempts to fetch an element from the DOM first through jQuery, then through JavaScript.</summary>
    /// <param name="elementId" type="String"> ElementId to find </param>
    /// <param name="$context" type="jQuery"> Optional context to limit the search </param>
    /// <returns type="jQuery">jQuery object, empty if no match found.</returns>
    var $ret = $('');
    if (!isNullOrEmpty(elementId)) {
        if (arguments.length == 2 && !isNullOrEmpty($context)) {
            $ret = $('#' + elementId, $context);
        } else {
            $ret = $('#' + elementId);
        }
        if ($ret.length === 0) {
            $ret = $(document.getElementById(elementId));
        }
        if ($ret.length === 0) {
            $ret = $(document.getElementsByName(elementId));
        }
    }
    return $ret;
}
function renameProperty(obj, oldName, newName) {
    if (false === isNullOrUndefined(obj) && contains(obj, oldName)) {
        obj[newName] = obj[oldName];
        delete obj[oldName];
    }
    return obj;
}
function trim(str) {
    /// <summary>Returns a string without left and right whitespace</summary>
    /// <param name="str" type="String"> String to parse </param>
    /// <returns type="String">Parsed string</returns>
    return $.trim(str);
}
function makeDelegate(method, options) {
    /// <summary>
    /// Returns a function with the argument parameter of the value of the current instance of the object.
    /// <para>For example, in a "for(i=0;i<10;i++)" loop, makeDelegate will capture the value of "i" for a given function.</para>
    /// </summary>
    /// <param name="method" type="Function"> A function to delegate. </param>
    /// <param name="options" type="Object"> A single parameter to hand the delegate function.</param>
    /// <returns type="Function">A delegate function: function(options)</returns>
    return function () { method(options); };
}
function makeEventDelegate(method, options) {
    /// <summary>
    /// Returns a function with the event object as the first parameter, and the current value of options as the second parameter.
    /// </summary>
    /// <param name="method" type="Function"> A function to delegate. </param>
    /// <param name="options" type="Object"> A single parameter to hand the delegate function.</param>
    /// <returns type="Function">A delegate function: function(eventObj, options)</returns>
    return function (eventObj) { method(eventObj, options); };
}
function foundMatch(anObj, prop, value) {
    var ret = false;
    if (false === isNullOrEmpty(anObj) &&contains(anObj, prop) &&anObj[prop] === value) {
        ret = true;
    }
    return ret;
}
function ObjectHelper(obj) {
    /// <summary>Find an object in a JSON object.</summary>
    /// <param name="obj" type="Object"> Object to search </param>
    /// <returns type="ObjectHelper"></returns>
    var thisObj = obj;
    var currentObj = null;
    var parentObj = thisObj;
    var currentKey = null;
    var parentKey = null;
    function find(key, value) {
        /// <summary>Find a property's parent</summary>
        /// <param name="key" type="String"> Property name to match. </param>
        /// <param name="value" type="Object"> Property value to match </param>
        /// <returns type="Object"> Returns the value of the 'property' property which contains a matching key/value prop. </returns>
        var ret = false;
        if (foundMatch(thisObj, key, value)) {
            ret = thisObj;
            currentObj = ret;
            parentObj = ret;
            currentKey = key;
        }
        if (false === ret) {
            var onSuccess = function (childObj, childKey, parObj) {
                var found = false;
                if (foundMatch(childObj, key, value)) {
                    ret = childObj;
                    currentObj = ret;
                    parentObj = parObj;
                    currentKey = childKey;
                    found = true;
                }
                return found;
            };
            crawlObject(thisObj, onSuccess, true);
        }
        return ret;
    }
    function remove(key, value) {
        var onSuccess = function (childObj, childKey, parObj) {
            var deleted = false;
            if (foundMatch(childObj, key, value)) {
                deleted = true;
                delete parObj[childKey];
                currentKey = null;
                currentObj = null;
                parentObj = parentObj;
            }
            return deleted;
        };
        return crawlObject(thisObj, onSuccess, true);
    }
    this.find = find;
    this.remove = remove;
    this.obj = thisObj;
    this.parentObj = parentObj;
    this.currentObj = currentObj;
    this.currentKey = currentObj;
}
//http://stackoverflow.com/questions/7356835/jquery-each-fumbles-if-non-array-object-has-length-property
function each(thisObj, onSuccess) {
    /// <summary>Iterates an Object or an Array and handles length property</summary>
    /// <param name="thisObj" type="Object"> An object to crawl </param>
    /// <param name="onSuccess" type="Function"> A function to execute on finding a property, which should return true to continue</param>
    /// <returns type="Object">Returns the return of onSuccess</returns>
    var ret = false;
    if (isFunction(onSuccess)) {
        if (isArray(thisObj) || (isPlainObject(thisObj) && false === thisObj.hasOwnProperty('length'))) {
            $.each(thisObj, function (childKey, value) {
                var childObj = thisObj[childKey];
                ret = onSuccess(childObj, childKey, thisObj, value);
                if (ret) {
                    return false; //false signals break
                }
            });
        }
        else if (isPlainObject(thisObj)) {
            for (var childKey in thisObj) {
                if (contains(thisObj, childKey)) {
                    var childObj = thisObj[childKey];
                    ret = onSuccess(childObj, childKey, thisObj);
                    if (ret) {
                        break;
                    }
                }
            }
        }
    }
    return ret;
} // each()
//borrowed from http://code.google.com/p/shadejs
function crawlObject(thisObj, onSuccess, doRecursion) {
    /// <summary>Iterates (optionally recursively) an object and executes a function on each of its properties.</summary>
    /// <param name="thisObj" type="Object"> An object to crawl </param>
    /// <param name="onSuccess" type="Function"> A function to execute on finding a property. To force iteration to stop, onSuccess should return false. </param>
    /// <param name="doRecursion" type="Boolean"> If true, recurse on all properties. Recursion will stop if onSuccess returns false. </param>
    /// <returns type="Object">Returns the return of onSuccess</returns>
    var stopCrawling = false;
    var onEach = function (childObj, childKey, parentObj, value) {
        if (false === stopCrawling) {
            stopCrawling = isTrue(onSuccess(childObj, childKey, parentObj, value));
        }
        if (false === stopCrawling && doRecursion) {
            stopCrawling = isTrue(crawlObject(childObj, onSuccess, doRecursion));
        }
        return stopCrawling;
    };
    stopCrawling = each(thisObj, onEach);
    return stopCrawling;
}
// because IE 8 doesn't support console.log unless the console is open (*duh*)
function log(s, includeCallStack) {
    /// <summary>Outputs a message to the console log(Webkit,FF) or an alert(IE)</summary>
    /// <param name="s" type="String"> String to output </param>
    /// <param name="includeCallStack" type="Boolean"> If true, include the callStack </param>
    var extendedLog = '';
    if (isTrue(includeCallStack)) {
        extendedLog = console.trace();
    }
    try {
        if (!isNullOrEmpty(extendedLog))
            console.log(s, extendedLog);
        else
            console.log(s);
    } catch (e) {
        alert(s);
        if (!isNullOrEmpty(extendedLog)) alert(extendedLog);
    }
}