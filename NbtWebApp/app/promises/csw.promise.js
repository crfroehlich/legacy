/// <reference path="~/app/CswApp-vsdoc.js" />


(function () {
    'use strict';

    Csw.register('promises', Csw.makeNameSpace());

    /**
     * A promise wrapper around AJAX requests.
    */
    Csw.promises.register('ajax', function(ajax) {
        var promise = Q.when(ajax);
        
        //Hybrid compatability with jQuery's XHR object.
        promise.abort = ajax.abort;
        promise.readyState = ajax.readyState;
                
        return promise;
    });

    var ajaxCount = 0;
    var spinning = false;
    
    //Fires when all jQuery AJAX requests have completed
    $(document).ajaxStop(function () {
        ajaxCount = 0;
        if (true === spinning) {
            Csw.main.ajaxImage.hide();
            Csw.main.ajaxSpacer.show();
        }
    });

    //Fires when any jQuery AJAX request starts
    $(document).ajaxSend(function (event, jqXHR, ajaxOptions) {
        if (ajaxOptions.watchGlobal) {
            ajaxCount += 1;

            if (ajaxCount > 0 && spinning === false) {
                spinning = true;

                Csw.main.ajaxImage.show();
                Csw.main.ajaxSpacer.hide();
            }
        }
    });

    //Fires when any jQuery AJAX request ends
    $(document).ajaxComplete(function (event, jqXHR, ajaxOptions) {
        if (ajaxOptions.watchGlobal) {
            ajaxCount -= 1;

            if (ajaxCount === 0 && spinning === true) {
                spinning = false;

                Csw.main.ajaxImage.hide();
                Csw.main.ajaxSpacer.show();
            }
        }
    });

} ());
