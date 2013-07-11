using System.Web;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Nbt;
using ChemSW.Nbt.Actions;
using ChemSW.Security;

namespace ChemSW.WebSvc
{
    public class CswWebSvcResourceInitializerNbt: ICswWebSvcResourceInitializer
    {
        private CswTimer _Timer = new CswTimer();
        private HttpContext _HttpContext = null;
        private CswWebSvcSessionAuthenticateData.Authentication.Request _AuthenticationRequest;
        private delegate void _OnDeInitDelegate();
        private _OnDeInitDelegate _OnDeInit;

        private void _setHttpContextOnRequest()
        {
            if( null != _HttpContext.Request.Cookies["csw_currentviewid"] )
            {
                _AuthenticationRequest.CurrentViewId = _HttpContext.Request.Cookies["csw_currentviewid"].Value;
            }
            if( null != _HttpContext.Request.Cookies["csw_currentactionname"] )
            {
                _AuthenticationRequest.CurrentActionName = _HttpContext.Request.Cookies["csw_currentactionname"].Value;
            }
            _AuthenticationRequest.IpAddress = CswWebSvcCommonMethods.getIpAddress();
        }

        public CswWebSvcResourceInitializerNbt( HttpContext HttpContext, CswWebSvcSessionAuthenticateData.Authentication.Request AuthenticationRequest ) //TODO: add Username/Password
        {
            _HttpContext = HttpContext;
            _AuthenticationRequest = AuthenticationRequest ?? new CswWebSvcSessionAuthenticateData.Authentication.Request();
            _setHttpContextOnRequest();
        }

        private CswSessionResourcesNbt _CswSessionResourcesNbt = null;

        private CswNbtResources _CswNbtResources = null;
        public CswNbtResources CswNbtResources
        {
            get
            {
                return ( _CswNbtResources );
            }
        }

        private CswNbtSessionAuthenticate _SessionAuthenticate = null;

        public ICswResources initResources()
        {
            _CswSessionResourcesNbt = new CswSessionResourcesNbt( _HttpContext.Application, _HttpContext.Request, _HttpContext.Response, _HttpContext, string.Empty, CswEnumSetupMode.NbtWeb );
            _CswNbtResources = _CswSessionResourcesNbt.CswNbtResources;
            _CswNbtResources.beginTransaction();
            _SessionAuthenticate = new CswNbtSessionAuthenticate( _CswNbtResources, _CswSessionResourcesNbt.CswSessionManager, _AuthenticationRequest );
            _OnDeInit = new _OnDeInitDelegate( _deInitResources );
            return ( _CswNbtResources );

        }//_initResources() 

        public CswEnumAuthenticationStatus authenticate()
        {
            CswEnumAuthenticationStatus Ret = CswEnumAuthenticationStatus.Unknown;
            //We're keeping this logic here, because we don't want to contaminate NbtLogic with the necessary web libraries required to support CswSessionResourcesNbt
            if( null != _AuthenticationRequest && _AuthenticationRequest.IsValid() )
            {
                Ret = _SessionAuthenticate.authenticate();
            }
            else
            {
                Ret = _CswSessionResourcesNbt.attemptRefresh();
            }

            //Set audit context
            if( Ret == CswEnumAuthenticationStatus.Authenticated && null != _CswNbtResources.CurrentNbtUser.Cookies )
            {
                string ContextViewId = string.Empty;
                string ContextActionName = string.Empty;
                if( _CswNbtResources.CurrentNbtUser.Cookies.ContainsKey( "csw_currentviewid" ) && null != _CswNbtResources.CurrentNbtUser.Cookies["csw_currentviewid"] )
                {
                    ContextViewId = _CswNbtResources.CurrentNbtUser.Cookies["csw_currentviewid"];
                }
                if( _CswNbtResources.CurrentNbtUser.Cookies.ContainsKey( "csw_currentactionname" ) && null != _CswNbtResources.CurrentNbtUser.Cookies["csw_currentactionname"] )
                {
                    ContextActionName = _CswNbtResources.CurrentNbtUser.Cookies["csw_currentactionname"];
                }

                if( string.Empty != ContextViewId )
                {
                    CswNbtView ContextView = null;
                    if( CswNbtViewId.isViewIdString( ContextViewId ) )
                    {
                        CswNbtViewId realViewid = new CswNbtViewId( ContextViewId );
                        ContextView = _CswNbtResources.ViewSelect.restoreView( realViewid );
                    }
                    else if( CswNbtSessionDataId.isSessionDataIdString( ContextViewId ) )
                    {
                        CswNbtSessionDataId SessionViewid = new CswNbtSessionDataId( ContextViewId );
                        ContextView = _CswNbtResources.ViewSelect.getSessionView( SessionViewid );
                    }
                    if( null != ContextView )
                    {
                        _CswNbtResources.AuditContext = ContextView.ViewName + " (" + ContextView.ViewId.ToString() + ")";
                    }
                }
                else if( string.Empty != ContextActionName )
                {
                    CswNbtAction ContextAction = _CswNbtResources.Actions[CswNbtAction.ActionNameStringToEnum( ContextActionName )];
                    if( null != ContextAction )
                    {
                        _CswNbtResources.AuditContext = CswNbtAction.ActionNameEnumToString( ContextAction.Name ) + " (Action_" + ContextAction.ActionId.ToString() + ")";
                    }
                }
            }

            _CswNbtResources.ServerInitTime = _Timer.ElapsedDurationInMilliseconds;

            return ( Ret );

        }//autheticate

        public void deauthenticate()
        {
            _SessionAuthenticate.deauthenticate();
        }//autheticate

        private void _deInitResources()
        {
            if( _CswSessionResourcesNbt != null )
            {
                _CswSessionResourcesNbt.endSession();

                _CswSessionResourcesNbt.finalize();
                _CswSessionResourcesNbt.release();
            }
        }

        public void deInitResources()
        {
            if( null != _OnDeInit )
            {
                //for example, Oracle is down and we never finished Init
                _OnDeInit.BeginInvoke( null, null );
            }
            if( null != _CswNbtResources )
            {
                _CswNbtResources.TotalServerTime = _Timer.ElapsedDurationInMilliseconds;
            }
        } // _deInitResources()

    } // class CswWebSvcResourceInitializerCommon

} // namespace ChemSW.Nbt.WebServices
