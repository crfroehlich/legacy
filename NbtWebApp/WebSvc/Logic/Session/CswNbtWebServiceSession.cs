using System;
using System.Runtime.Serialization;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Session;
using NbtWebApp.WebSvc.Returns;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceSession
    {
        /// <summary>
        /// View Select Return Object
        /// </summary>
        [DataContract]
        public class CswNbtSessionReturn : CswWebSvcReturn
        {
            public CswNbtSessionReturn()
            {
                Data = new CswWebSvcReturnBase.Data();
            }
            [DataMember]
            public CswWebSvcReturnBase.Data Data;
        }

        /// <summary>
        /// WCF wrapper around resetPassword
        /// </summary>
        public static void resetPassword( ICswResources CswResources, CswNbtSessionReturn Ret, CswNbtSessionAuthenticateData.Authentication.Response.Expired Request )
        {
            Ret.Data.Succeeded = false;
            try
            {
                if( false == string.IsNullOrEmpty( Request.NewPassword ) )
                {
                    CswNbtResources Resources = (CswNbtResources) CswResources;
                    CswNbtObjClassUser User = Resources.Nodes[Request.UserId];
                    if( null != User )
                    {
                        User.PasswordProperty.Password = Request.NewPassword;
                        User.postChanges( ForceUpdate: false );
                        Ret.Data.Succeeded = true;
                    }
                }
            }
            catch( Exception Ex )
            {
                Ret.addException( Ex );
            }
        }
    } // class CswNbtWebServiceSession

} // namespace ChemSW.Nbt.WebServices