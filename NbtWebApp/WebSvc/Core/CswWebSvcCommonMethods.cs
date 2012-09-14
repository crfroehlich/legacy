using System;
using ChemSW.Config;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Log;
using ChemSW.Nbt;
using ChemSW.Security;
using NbtWebApp.WebSvc.Returns;
using Newtonsoft.Json.Linq;

namespace ChemSW.WebSvc
{
    public class CswWebSvcCommonMethods
    {
        /// <summary>
        /// </summary>
        public static CswWebSvcReturnBase.ErrorMessage wError( CswNbtResources CswNbtResources, Exception ex )
        {
            CswWebSvcReturnBase.ErrorMessage Ret = new CswWebSvcReturnBase.ErrorMessage();
            string Message, Detail;
            ErrorType Type;
            bool Display;
            error( CswNbtResources, ex, out Type, out Message, out Detail, out Display );

            Ret.ShowError = Display;
            Ret.Type = Type.ToString();
            Ret.Message = Message;
            Ret.Detail = Detail;

            return Ret;

        }//jError() 

        /// <summary>
        /// </summary>
        public static JObject jError( CswNbtResources CswNbtResources, Exception ex )
        {
            JObject Ret = new JObject();
            string Message, Detail;
            ErrorType Type;
            bool Display;
            error( CswNbtResources, ex, out Type, out Message, out Detail, out Display );

            Ret["success"] = "false";
            Ret["error"] = new JObject();
            Ret["error"]["display"] = Display.ToString().ToLower();
            Ret["error"]["type"] = Type.ToString();
            Ret["error"]["message"] = Message;
            Ret["error"]["detail"] = Detail;
            return Ret;
        }//jError() 

        public static void error( CswNbtResources CswNbtResources, Exception ex, out ErrorType Type, out string Message, out string Detail, out bool Display )
        {
            if( CswNbtResources != null )
            {
                CswNbtResources.CswLogger.reportError( ex );
                CswNbtResources.Rollback();
            }

            CswDniException newEx = null;
            if( ex is CswDniException )
            {
                newEx = (CswDniException) ex;
            }
            else
            {
                newEx = new CswDniException( ex.Message, ex );
            }

            Display = true;
            if( CswNbtResources != null )
            {
                if( newEx.Type == ErrorType.Warning )
                {
                    Display = ( CswNbtResources.ConfigVbls.getConfigVariableValue( "displaywarningsinui" ) != "0" );
                }
                else
                {
                    Display = ( CswNbtResources.ConfigVbls.getConfigVariableValue( "displayerrorsinui" ) != "0" );
                }
            }

            Type = newEx.Type;
            Message = newEx.MsgFriendly;
            Detail = newEx.MsgEscoteric + "; " + ex.StackTrace;

        } // _error()

        public static CswNbtView getView( CswNbtResources CswNbResources, string ViewId )
        {
            CswNbtView ReturnVal = null;

            if( CswNbtViewId.isViewIdString( ViewId ) )
            {
                CswNbtViewId realViewid = new CswNbtViewId( ViewId );
                ReturnVal = CswNbResources.ViewSelect.restoreView( realViewid );
            }
            else if( CswNbtSessionDataId.isSessionDataIdString( ViewId ) )
            {
                CswNbtSessionDataId SessionViewid = new CswNbtSessionDataId( ViewId );
                ReturnVal = CswNbResources.ViewSelect.getSessionView( SessionViewid );
            }

            return ( ReturnVal );
        } // _getView()

        public static void jAddAuthenticationStatus( CswNbtResources CswNbtResources, CswSessionResourcesNbt CswSessionResources, JObject SvcReturn, AuthenticationStatus AuthenticationStatusIn )
        {
            if( SvcReturn != null )
            {
                SvcReturn["AuthenticationStatus"] = AuthenticationStatusIn.ToString();
                if( ( null != CswNbtResources ) && ( null != CswNbtResources.CswSessionManager ) )
                {
                    SvcReturn["timeout"] = CswDateTime.ToClientAsJavascriptString( CswNbtResources.CswSessionManager.TimeoutDate );
                }
                SvcReturn["server"] = Environment.MachineName;
                SvcReturn["timer"] = new JObject();
                SvcReturn["timer"]["serverinit"] = Math.Round( CswNbtResources.ServerInitTime, 3 );
                if( null != CswNbtResources )
                {
                    LogLevels LogLevel = CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.Logging_Level );
                    if( LogLevel == CswNbtResources.UnknownEnum )
                    {
                        LogLevel = LogLevels.Error;
                    }
                    SvcReturn["LogLevel"] = LogLevel.ToString().ToLower();

                    SvcReturn["timer"]["customerid"] = CswNbtResources.AccessId;
                    SvcReturn["timer"]["dbinit"] = Math.Round( CswNbtResources.CswLogger.DbInitTime, 3 );
                    SvcReturn["timer"]["dbquery"] = Math.Round( CswNbtResources.CswLogger.DbQueryTime, 3 );
                    SvcReturn["timer"]["dbcommit"] = Math.Round( CswNbtResources.CswLogger.DbCommitTime, 3 );
                    SvcReturn["timer"]["dbdeinit"] = Math.Round( CswNbtResources.CswLogger.DbDeInitTime, 3 );
                    SvcReturn["timer"]["treeloadersql"] = Math.Round( CswNbtResources.CswLogger.TreeLoaderSQLTime, 3 );

                }
                SvcReturn["timer"]["servertotal"] = Math.Round( CswNbtResources.TotalServerTime, 3 );
                SvcReturn["AuthenticationStatus"] = AuthenticationStatusIn.ToString();
            }
        }

        public static void wAddAuthenticationStatus( CswNbtResources CswNbtResources, CswSessionResourcesNbt CswSessionResources, CswWebSvcReturn SvcReturn, AuthenticationStatus AuthenticationStatusIn )
        {
            // ******************************************
            // IT IS VERY IMPORTANT for this function not to require the use of database resources, 
            // since it occurs AFTER the call to _deInitResources(), and thus will leak Oracle connections 
            // (see case 26273)
            // ******************************************
            if( null != SvcReturn )
            {
                CswWebSvcReturnBase.Authentication Session = SvcReturn.Authentication ?? new CswWebSvcReturnBase.Authentication();

                Session.AuthenticationStatus = AuthenticationStatusIn.ToString();

                if( ( null != CswNbtResources ) && ( null != CswNbtResources.CswSessionManager ) )
                {
                    Session.TimeOut = CswDateTime.ToClientAsJavascriptString( CswNbtResources.CswSessionManager.TimeoutDate );
                }

                CswWebSvcReturnBase.Performance Perf = SvcReturn.Performance ?? new CswWebSvcReturnBase.Performance();

                Perf.ServerInit = Math.Round( CswNbtResources.ServerInitTime, 3 );
                if( null != CswNbtResources )
                {
                    Perf.DbDeinit = Math.Round( CswNbtResources.CswLogger.DbInitTime, 3 );
                    Perf.DbQuery = Math.Round( CswNbtResources.CswLogger.DbQueryTime, 3 );
                    Perf.DbCommit = Math.Round( CswNbtResources.CswLogger.DbCommitTime, 3 );
                    Perf.DbDeinit = Math.Round( CswNbtResources.CswLogger.DbDeInitTime, 3 );
                    Perf.TreeLoaderSql = Math.Round( CswNbtResources.CswLogger.TreeLoaderSQLTime, 3 );
                }
                Perf.ServerTotal = Math.Round( CswNbtResources.TotalServerTime, 3 );

                CswWebSvcReturnBase.Logging Logging = SvcReturn.Logging ?? new CswWebSvcReturnBase.Logging();
                Logging.CustomerId = CswNbtResources.AccessId;
                Logging.Server = Environment.MachineName;
                LogLevels LogLevel = CswNbtResources.ConfigVbls.getConfigVariableValue( CswConfigurationVariables.ConfigurationVariableNames.Logging_Level );
                if( LogLevel == CswNbtResources.UnknownEnum )
                {
                    LogLevel = LogLevels.Error;
                }
                Logging.LogLevel = LogLevel;
            }
        }//_jAuthenticationStatus()


    } // class CswWebSvcCommonMethods

} // namespace ChemSW.Nbt.WebServices