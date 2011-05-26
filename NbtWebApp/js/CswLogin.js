﻿/// <reference path="../jquery/jquery-1.6-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/linq-vsdoc.js" />
/// <reference path="../jquery/linq.js_ver2.2.0.2/jquery.linq-vsdoc.js" />
/// <reference path="_Global.js" />

; (function ($) { /// <param name="$" type="jQuery" />

	$.fn.CswLogin = function (method) {
		var PluginName = 'CswLogin';

		var methods = {
			'login': function (options) {
						var o = {
							AuthenticateUrl: '/NbtWebApp/wsNBT.asmx/authenticate',
							onAuthenticate: function(Username) {}
						};
						if (options) $.extend(o, options);

						var ThisSessionId = $.CswCookie('get', CswCookieName.SessionId);
                        if( !isNullOrEmpty(ThisSessionId) )
						{
							o.onAuthenticate( $.CswCookie('get', CswCookieName.Username) );
						}
						else 
						{
							var $LoginDiv = $( '<div id="logindiv" align="center">' +
												'  <form>' +  
												'    <table>' +
												'    <tr>' +
                                                '      <td align="right"></td>' +
                                                '      <td id="loginmsg" style="display: none;"></td>' +
                                                '    </tr>' +
                                                '    <tr>' +
												'      <td align="right">Customer ID:</td>' +
												'      <td><input type="text" name="login_accessid" id="login_accessid" /></td>' +
												'    </tr>' +
												'    <tr>' +
												'      <td align="right">User Name:</td>' +
												'      <td><input type="text" name="login_username" id="login_username" /></td>' +
												'    </tr>' +
												'    <tr>' +
												'      <td align="right">Password:</td>' +
												'      <td><input type="password" name="login_password" id="login_password" /></td>' +
												'    </tr>' +
												'    <tr>' +
												'      <td align="right"></td>' +
												'      <td id="login_button_cell"> '+ //<input type="submit" id="login_button" name="Login" value="Login" />' +
												'          </td>' +
												'    </tr>' +
												'    <tr>' +
												'      <td></td>' +
												'      <td></td>' +
												'    </tr>' + 
                                                '  </table>' +
												' </form>' +
												' </div>' +
												' <br/><br/><br/>' +
												' <div id="assemblydiv" width="100%" align="right"></div>')
                                            .appendTo($(this));

							$('#assemblydiv').load('_Assembly.txt');
							
							$('#login_accessid').focus();

							var $loginbutton = $('#login_button_cell').CswButton({ID: 'login_button', 
														enabledText: 'Login', 
														disabledText: 'Logging in...', 
														onclick: function() {
                                                            $('#loginmsg').hide()
																	.children().remove();

															var AccessId = $('#login_accessid').val();
															var UserName = $('#login_username').val();
															var Password = $('#login_password').val();

															function _handleAuthenticated(UserName)
															{
																$.CswCookie('set', CswCookieName.Username, UserName);
																$LoginDiv.remove();
																o.onAuthenticate(UserName);
															}

                                                            var dataJson = {
                                                                AccessId: AccessId, 
                                                                UserName: UserName, 
                                                                Password: Password
                                                            };

															CswAjaxJSON({
																		url: o.AuthenticateUrl,
																		data: dataJson,
																		success: function (data) {
																			var auth = data.AuthenticationStatus;
                                                                            if(auth === 'Authenticated')
																			{
																				_handleAuthenticated(UserName);
																			}
																			else 
																			{
																				_handleAuthenticationStatus({
																					status: auth,
																					success: function() { _handleAuthenticated(UserName); },
																					failure: function(txt) {
																						$('#loginmsg').CswErrorMessage({'message': txt });
																						$('#login_password').val('');   // case 21303
																						$loginbutton.CswButton('enable');
																					},
																					usernodeid: data.nodeid,
																					usernodekey: data.cswnbtnodekey,
																					passwordpropid: data.passwordpropid
																				});
                                                                            }
                                                                        },  // success{}
																		error: function() {
																			$loginbutton.CswButton('enable');
																		}
															}); // ajax
											} // onclick
							}); // button

                            $('#login_accessid').clickOnEnter($loginbutton);
                            $('#login_username').clickOnEnter($loginbutton);
                            $('#login_password').clickOnEnter($loginbutton);

                        } // if-else(ThisSessionId !== null)
					}  // login
		};



		// Method calling logic
		if ( methods[method] ) {
			return methods[ method ].apply( this, Array.prototype.slice.call( arguments, 1 ));
		} else if ( typeof method === 'object' || ! method ) {
			return methods.init.apply( this, arguments );
		} else {
			$.error( 'Method ' +  method + ' does not exist on ' + PluginName );
		}    

	}; // function(options) {
})(jQuery);

