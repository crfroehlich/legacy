///// <reference path="../js/thirdparty/jquery/core/jquery-1.6.1-vsdoc.js" />
///// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/linq-vsdoc.js" />
///// <reference path="../js/thirdparty/js/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
///// <reference path="_Global.js" />


//#region CswString
function CswString(string)
{
    var value = string;
    this.val = function (string)
    {
        if (arguments.length === 1)
        {
            value = string;
            return this; //for chaining
        }
        else
        {
            return value;
        }
    };
    this.contains = function (string) { return value.indexOf(string) !== -1; };
}

CswString.prototype.toString = function () { return this.value; };

//#endregion CswString

//#region CswArray
function CswArray()
{
    var ctor = [];
    ctor.push.apply(arr, arguments);
    ctor.__proto__ = CswArray.prototype;
    return ctor;
}
CswArray.prototype = new Array;
CswArray.prototype = {
    last: function ()
    {
        return this[this.length - 1];
    },
    contains: function (key)
    {
        this.indexOf(key) !== -1
    }
};

//#endregion CswArray

//#region CswLocalStorage
function CswStorage(nativeStorage) //, serializer)
{
    //abstracts storage of 
    this.storage = nativeStorage;
    this.serializer = new ONEGEEK.GSerializer();
    this.keys = new Array;
}

CswStorage.prototype = {

    clear: function ()
    {
        //only clear the storage consumed by this instance
        for (var key in this.keys)
        {
            var keyName = this.keys[key];
            this.storage.removeItem(keyName);
        }
        return (this);
    },
    purgeAllStorage: function ()
    {
        //nuke the entire storage collection
        this.storage.clear();
        return this;
    },
    getItem: function (key, defaultValue)
    {
        var ret;
        var value = this.storage.getItem(key);
        if ( isNullOrEmpty(value) )
        {
            ret = (!isNullOrEmpty(defaultValue)) ? defaultValue : null;
        }
        else
        {
            ret = this.serializer.deserialize(value);
        }
        return ret;
    },
    getKeys: function ()
    {
        var ret = this.keys;
        return ret;
    },
    hasItem: function (key)
    {
        ret = ( !isNullOrEmpty( this.storage.getItem(key) );
        return ret;
    },
    removeItem: function (key)
    {
        this.storage.removeItem(key);
        return (this);
    },
    setItem: function (key, value)
    {
        if (this.keys.indexOf(key) === -1)
        {
            this.keys.push(key);
        }
        this.storage.setItem(key, this.serializer.serialize(value));
        return (this);
    }
};
//#endregion CswLocalStorage