/// <reference path="../../../Scripts/jquery-1.6.4-vsdoc.js" />
/// <reference path="../controls/ICswMobileWebControls.js" />
/// <reference path="../controls/CswMobilePageHeader.js" />
/// <reference path="../controls/CswMobilePageFooter.js" />
/// <reference path="../controls/CswMobileMenuButton.js" />
/// <reference path="../CswMobile.js" />
/// <reference path="CswMobilePageFactory.js" />
/// <reference path="../../main/tools/CswCookie.js" />
/// <reference path="../../globals/Global.js" />
/// <reference path="../../globals/CswGlobalTools.js" />
/// <reference path="../../globals/CswEnums.js" />
/// <reference path="../../main/tools/CswArray.js" />
/// <reference path="../globals/CswMobileTools.js" />
/// <reference path="../globals/CswMobileEnums.js" />

//#region CswMobilePageLogin

function CswMobilePageLogin(loginDef,$page,mobileStorage,loginSuccess) {
    /// <summary>
    ///   Login Page class. Responsible for generating a Mobile login page.
    /// </summary>
    /// <param name="loginDef" type="Object">Login definitional data.</param>
    /// <param name="$page" type="jQuery">Parent page element to attach to.</param>
    /// <param name="mobileStorage" type="CswMobileClientDbResources">Client DB Resources</param>
    /// <param name="loginSuccess" type="Function">Function to execute on login success.</param>
    /// <returns type="CswMobilePageLogin">Instance of itself. Must instance with 'new' keyword.</returns>

    //#region private

    var pageDef = { };
    var id, title, contentDivId, $contentPage, $content,
        divSuffix = '_login';
    
    //ctor
    (function () {

        var p = {
            level: -1,
            DivId: '',
            title: '',
            theme: CswMobileGlobal_Config.theme
        };
        if (loginDef) {
            $.extend(p, loginDef);
        }

        id = tryParseString(p.DivId, CswMobilePage_Type.login.id);
        contentDivId = id + divSuffix;
        title = tryParseString(p.title, CswMobilePage_Type.login.title);
        $contentPage = $page.find('div:jqmData(role="content")');
        $content = (isNullOrEmpty($contentPage) || $contentPage.length === 0) ? null : $contentPage.find('#' + contentDivId);

        getContent();
    })();  //ctor
    
    function getContent() {
        $content = ensureContent($content, contentDivId);
        
        $content.append('<p style="text-align: center;">Login to Mobile Inspection Manager</p><br/>');
        var loginFailure = mobileStorage.getItem('loginFailure');
        if (loginFailure)
        {
            $content.append('<span class="error">' + loginFailure + '</span><br/>');
        }
        var $customerId = $('<input type="text" id="login_customerid" placeholder="Customer Id" />')
                            .appendTo($content);
        $content.append('<br/>');
        var $username = $('<input type="text" id="login_username" placeholder="User Name" />')
                            .appendTo($content);
        $content.append('<br/>');
        var $password = $('<input type="password" id="login_password" placeholder="Password" />')
                            .appendTo($content);
        $content.append('<br/>');
        var $loginBtn = $('<a id="loginsubmit" data-role="button" data-identity="loginsubmit" data-url="loginsubmit" href="javascript:void(0);">Continue</a>')
                            .appendTo($content)
                            .unbind('click')
                            .bind('click', function() {
                                return startLoadingMsg(function() { onLoginSubmit(); });
                            });
        
        if (false === isNullOrEmpty($contentPage) && $contentPage.length > 0 ) {
            $contentPage.append($content);
        }
        
        $customerId.clickOnEnter($loginBtn);
        $username.clickOnEnter($loginBtn);
        $password.clickOnEnter($loginBtn);

        function onLoginSubmit() {
            var authenticateUrl = '/NbtWebApp/wsNBT.asmx/authenticate';
            if (mobileStorage.amOnline()) {
                var userName = $username.val();
                var accessId = $customerId.val();

                var ajaxData = {
                    'AccessId': accessId, //We're displaying "Customer ID" but processing "AccessID"
                    'UserName': userName,
                    'Password': $password.val(),
                    ForMobile: true
                };

                CswAjaxJson({
                        formobile: true,
                        //async: false,
                        url: authenticateUrl,
                        data: ajaxData,
                        onloginfail: function(text) {
                            onLoginFail(text, mobileStorage);
                        },
                        success: function(data) {
                            doSuccess(loginSuccess, data, userName, accessId);
                        },
                        error: function() {
                            onError();
                        }
                    });
            }

        } //onLoginSubmit() 
        return $content;
    }
    
    //#endregion private
    
    //#region public, priveleged

    return {
        $content: $content,
        contentDivId: contentDivId,
        pageDef: pageDef,
        id: id,
        title: title,
        getContent: getContent
    };
    
    //#endregion public, priveleged
}

//#endregion CswMobilePageLogin