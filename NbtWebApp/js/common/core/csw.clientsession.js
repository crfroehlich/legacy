﻿/// <reference path="~/js/CswNbt-vsdoc.js" />
/// <reference path="~/js/CswCommon-vsdoc.js" />

(function _cswClientSession() {
    'use strict';

    var internal = {
        AccessId: '',
        UserName: '',
        Password: '',
        ForMobile: false,
        onAuthenticate: null, // function (UserName) {} 
        onFail: null, // function (errormessage) {} 
        logoutpath: '',
        authenticateUrl: '/NbtWebApp/wsNBT.asmx/authenticate',
        DeauthenticateUrl: '/NbtWebApp/wsNBT.asmx/deauthenticate',
        expiretimeInterval: '',
        expiretime: '',
        expiredInterval: ''
    };

    internal.checkExpired = function () {
        var now = new Date();
        if (Date.parse(internal.expiretime) - Date.parse(now) < 0) {
            window.clearInterval(internal.expiredInterval);
            Csw.clientSession.logout();
        }
    };

    internal.checkExpireTime = function () {
        var now = new Date();
        if (Date.parse(internal.expiretime) - Date.parse(now) < 180000) { // 3 minutes until timeout
            window.clearInterval(internal.expiretimeInterval);
            $.CswDialog('ExpireDialog', {
                'onYes': function () {
                    Csw.ajax.post({
                        'url': '/NbtWebApp/wsNBT.asmx/RenewSession',
                        'success': function () {
                        }
                    });
                }
            });
        }
    };

    internal.setExpireTimeInterval = function () {
        window.clearInterval(internal.expiretimeInterval);
        window.clearInterval(internal.expiredInterval);
        internal.expiretimeInterval = window.setInterval(function () {
            internal.checkExpireTime();
        }, 60000);
        internal.expiredInterval = window.setInterval(function () {
            internal.checkExpired();
        }, 60000);
    };

    Csw.clientSession = Csw.clientSession ||
        Csw.register('clientSession', Csw.makeNameSpace());

    Csw.clientSession.finishLogout = Csw.clientSession.finishLogout ||
        Csw.clientSession.register('finishLogout', function () {
            ///<summary>Complete the logout. Nuke any lingering client-side data.</summary>
            internal.logoutpath = Csw.cookie.get(Csw.cookie.cookieNames.LogoutPath);
            Csw.clientDb.clear();
            Csw.cookie.clearAll();
            if (false === Csw.isNullOrEmpty(internal.logoutpath)) {
                window.location = internal.logoutpath;
            } else {
                window.location = Csw.getGlobalProp('homeUrl');
            }
        });

    Csw.clientSession.login = Csw.clientSession.login ||
        Csw.clientSession.register('login', function (loginopts) {
            ///<summary>Attempt a login.</summary>
            if (loginopts) {
                $.extend(internal, loginopts);
            }

            Csw.ajax.post({
                url: internal.authenticateUrl,
                data: {
                    AccessId: internal.AccessId,
                    UserName: internal.UserName,
                    Password: internal.Password,
                    ForMobile: internal.ForMobile
                },
                success: function () {
                    Csw.cookie.set(Csw.cookie.cookieNames.Username, internal.UserName);
                    Csw.cookie.set(Csw.cookie.cookieNames.LogoutPath, internal.logoutpath);
                    Csw.tryExec(internal.onAuthenticate, internal.UserName);
                },
                onloginfail: function (txt) {
                    Csw.tryExec(internal.onFail, txt);
                },
                error: function () {
                    Csw.tryExec(internal.onFail, 'Webservice Error');
                }
            }); // ajax
        });

    Csw.clientSession.logout = Csw.clientSession.logout ||
        Csw.clientSession.register('logout', function (options) {
            ///<summary>End the current session.</summary>
            if (options) {
                $.extend(internal, options);
            }

            Csw.ajax.post({
                url: internal.DeauthenticateUrl,
                data: {},
                success: function () {
                    Csw.clientSession.finishLogout();
                }
            });
        });

    Csw.clientSession.getExpireTime = Csw.clientSession.getExpireTime ||
        Csw.clientSession.register('getExpireTime', function () {
            ///<summary>Get the expiration time.</summary>
            return internal.expiretime;
        });

    Csw.clientSession.setExpireTime = Csw.clientSession.setExpireTime ||
        Csw.clientSession.register('setExpireTime', function (value) {
            ///<summary>Define the expiration time.</summary>
            internal.expiretime = value;
            internal.setExpireTimeInterval();
        });

    Csw.clientSession.handleAuthenticationStatus = Csw.clientSession.handleAuthenticationStatus ||
        Csw.clientSession.register('handleAuthenticationStatus', function (options) {
            ///<summary>Each web request will authenticate. Depending on the current authentication status, ajax onSuccess methods may need to change.</summary>
            var o = {
                status: '',
                success: function () {
                },
                failure: function () {
                },
                usernodeid: '',
                usernodekey: '',
                passwordpropid: '',
                ForMobile: false
            };
            if (options) {
                $.extend(o, options);
            }

            var txt = '';
            var goodEnoughForMobile = false; //Ignore password expirery and license accept for Mobile for now
            switch (o.status) {
                case 'Authenticated':
                    o.success();
                    break;
                case 'Deauthenticated':
                    o.success(); // yes, o.success() is intentional here.
                    break;
                case 'Failed':
                    txt = 'Invalid login.';
                    break;
                case 'Locked':
                    txt = 'Your account is locked.  Please see your account administrator.';
                    break;
                case 'Deactivated':
                    txt = 'Your account is deactivated.  Please see your account administrator.';
                    break;
                case 'ModuleNotEnabled':
                    txt = 'This feature is not enabled.  Please see your account administrator.';
                    break;
                case 'TooManyUsers':
                    txt = 'Too many users are currently connected.  Try again later.';
                    break;
                case 'NonExistentAccessId':
                    txt = 'Invalid login.';
                    break;
                case 'NonExistentSession':
                    txt = 'Your session has timed out.  Please login again.';
                    break;
                case 'Unknown':
                    txt = 'An Unknown Error Occurred';
                    break;
                case 'TimedOut':
                    goodEnoughForMobile = true;
                    txt = 'Your session has timed out.  Please login again.';
                    break;
                case 'ExpiredPassword':
                    goodEnoughForMobile = true;
                    if (!o.ForMobile) {
                        $.CswDialog('EditNodeDialog', {
                            nodeids: [o.usernodeid],
                            nodekeys: [o.usernodekey],
                            filterToPropId: o.passwordpropid,
                            title: 'Your password has expired.  Please change it now:',
                            onEditNode: function () {
                                o.success();
                            }
                        });
                    }
                    break;
                case 'ShowLicense':
                    goodEnoughForMobile = true;
                    if (!o.ForMobile) {
                        $.CswDialog('ShowLicenseDialog', {
                            'onAccept': function () {
                                o.success();
                            },
                            'onDecline': function () {
                                o.failure('You must accept the license agreement to use this application');
                            }
                        });
                    }
                    break;
            }

            if (o.ForMobile &&
                (o.status !== 'Authenticated' && goodEnoughForMobile)) {
                o.success();
            } else if (false === Csw.isNullOrEmpty(txt) && o.status !== 'Authenticated') {
                o.failure(txt, o.status);
            }
        });

    Csw.clientSession.isAdministrator = Csw.clientSession.isAdministrator ||
        Csw.clientSession.register('isAdministrator', function (options) {
            ///<returns type="bool">True if the current session has Administrative priveleges</returns>
            var o = {
                'Yes': null,
                'No': null
            };
            if (options) {
                $.extend(o, options);
            }

            Csw.ajax.post({
                url: '/NbtWebApp/wsNBT.asmx/isAdministrator',
                success: function (data) {
                    if (Csw.bool(data.Administrator)) {
                        Csw.tryExec(o.Yes);
                    } else {
                        Csw.tryExec(o.No);
                    }
                }
            });
        });

} ());