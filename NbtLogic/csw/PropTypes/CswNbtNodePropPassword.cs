using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.Encryption;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.PropTypes
{
    public class CswNbtNodePropPassword : CswNbtNodeProp
    {
        private CswEncryption _CswEncryption;

        public static implicit operator CswNbtNodePropPassword( CswNbtNodePropWrapper PropWrapper )
        {
            return PropWrapper.AsPassword;
        }

        public CswNbtNodePropPassword( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _FieldTypeRule = (CswNbtFieldTypeRulePassword) CswNbtMetaDataNodeTypeProp.getFieldTypeRule();
            _EncryptedPasswordSubField = _FieldTypeRule.EncryptedPasswordSubField;
            _ChangedDateSubField = _FieldTypeRule.ChangedDateSubField;

            _CswEncryption = new CswEncryption( CswNbtResources.MD5Seed );
        }
        private CswNbtFieldTypeRulePassword _FieldTypeRule;
        private CswNbtSubField _EncryptedPasswordSubField;
        private CswNbtSubField _ChangedDateSubField;

        override public bool Empty
        {
            get
            {
                return ( 0 == Gestalt.Length );
            }//
        }


        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }//

        }//Gestalt

        public string EncryptedPassword
        {
            get
            {
                return _CswNbtNodePropData.GetPropRowValue( _EncryptedPasswordSubField.Column );
            }
            set
            {
                string UserName = string.Empty;
                CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass.UserClass );
                CswNbtMetaDataObjectClassProp UserPassword = UserOC.getObjectClassProp( CswNbtObjClassUser.PropertyName.Password );

                if( this.ObjectClassPropId == UserPassword.ObjectClassPropId )
                {
                    //CswNbtNode UserNode = _CswNbtResources.Nodes.GetNode( this.NodeId );
                    if( //null != UserNode &&
                        false == (
                                    _CswNbtResources.Permit.canNode( CswNbtPermit.NodeTypePermission.Edit, NodeTypeProp.getNodeType(), this.NodeId ) ) &&
                                    _CswNbtResources.Permit.canProp( CswNbtPermit.NodeTypePermission.Edit, NodeTypeProp, null )
                                 )
                    {
                        throw new CswDniException( ErrorType.Warning, "User does not have permission to edit this password", "Permit.can() returned false for UserNode '" + this.NodeId + "'." );
                    }
                }

                _CswNbtNodePropData.SetPropRowValue( _EncryptedPasswordSubField.Column, value );
                _CswNbtNodePropData.Gestalt = value;
                ChangedDate = DateTime.Now;
            }
        }

        public DateTime ChangedDate
        {
            get
            {
                //string StringValue = _CswNbtNodePropData.GetPropRowValue( _ChangedDateSubField.Column );
                //DateTime ReturnVal = DateTime.MinValue;
                //if( StringValue != string.Empty )
                //    ReturnVal = Convert.ToDateTime( StringValue );
                //return ( ReturnVal.Date );
                return _CswNbtNodePropData.GetPropRowValueDate( _ChangedDateSubField.Column );
            }

            set
            {
                if( DateTime.MinValue != value )
                    _CswNbtNodePropData.SetPropRowValue( _ChangedDateSubField.Column, value );
                else
                    _CswNbtNodePropData.SetPropRowValue( _ChangedDateSubField.Column, DBNull.Value );
            }
        }

        public string Password
        {
            set
            {
                EncryptedPassword = _CswEncryption.getMd5Hash( value );
                ChangedDate = DateTime.Now;
            }
        }

        public bool IsExpired
        {
            get
            {
                Int32 PasswordExpiryDays = CswConvert.ToInt32( _CswNbtResources.ConfigVbls.getConfigVariableValue( "passwordexpiry_days" ) );
                return ( ChangedDate == DateTime.MinValue ||
                         ChangedDate.AddDays( PasswordExpiryDays ).Date <= DateTime.Now.Date );
            }
        }

        public override void ToXml( XmlNode ParentNode )
        {
            CswXmlDocument.AppendXmlNode( ParentNode, _EncryptedPasswordSubField.ToXmlNodeName(), EncryptedPassword );

            // We don't provide the raw password, but we do provide a node in which someone can set a new password for saving
            CswXmlDocument.AppendXmlNode( ParentNode, "newpassword", "" );
            CswXmlDocument.AppendXmlNode( ParentNode, "isexpired", IsExpired.ToString().ToLower() );
            CswXmlDocument.AppendXmlNode( ParentNode, "isadmin", _CswNbtResources.CurrentNbtUser.IsAdministrator().ToString().ToLower() );
        }

        public override void ToXElement( XElement ParentNode )
        {
            ParentNode.Add( new XElement( _EncryptedPasswordSubField.ToXmlNodeName( true ), EncryptedPassword ),
                new XElement( "newpassword" ) );
        }

        public override void ToJSON( JObject ParentObject )
        {
            ParentObject[_EncryptedPasswordSubField.ToXmlNodeName( true )] = EncryptedPassword;
            ParentObject["passwordcomplexity"] = _CswNbtResources.ConfigVbls.getConfigVariableValue( ChemSW.Config.CswConfigurationVariables.ConfigurationVariableNames.Password_Complexity );
            ParentObject["passwordlength"] = _CswNbtResources.ConfigVbls.getConfigVariableValue( ChemSW.Config.CswConfigurationVariables.ConfigurationVariableNames.Password_Length );
            ParentObject["newpassword"] = string.Empty;
            ParentObject["isexpired"] = IsExpired.ToString().ToLower();
            ParentObject["isadmin"] = _CswNbtResources.CurrentNbtUser.IsAdministrator().ToString().ToLower();
        }

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            EncryptedPassword = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _EncryptedPasswordSubField.ToXmlNodeName() );

            bool inIsExpired = CswXmlDocument.ChildXmlNodeValueAsBoolean( XmlNode, "isexpired" );
            if( inIsExpired && !IsExpired )
            {
                ChangedDate = DateTime.MinValue;
            }

            _saveProp( CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, "newpassword" ) );
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            if( null != XmlNode.Element( _EncryptedPasswordSubField.ToXmlNodeName( true ) ) )
            {
                EncryptedPassword = XmlNode.Element( _EncryptedPasswordSubField.ToXmlNodeName( true ) ).Value;
            }
            if( null != XmlNode.Element( "newpassword" ) )
            {
                _saveProp( XmlNode.Element( "newpassword" ).Value );
            }
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            EncryptedPassword = CswTools.XmlRealAttributeName( PropRow[_EncryptedPasswordSubField.ToXmlNodeName()].ToString() );
        }

        public override void ReadJSON( JObject JObject, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            if( null != JObject[_EncryptedPasswordSubField.ToXmlNodeName( true )] )
            {
                EncryptedPassword = JObject[_EncryptedPasswordSubField.ToXmlNodeName( true )].ToString();
            }
            if( null != JObject["newpassword"] )
            {
                _saveProp( JObject["newpassword"].ToString() );
            }
            if( null != JObject["isexpired"] )
            {
                bool inIsExpired = CswConvert.ToBoolean( JObject["isexpired"].ToString() );
                if( inIsExpired && !IsExpired )
                {
                    ChangedDate = DateTime.MinValue;
                }
            }
        }

        private void _saveProp( string NewPassword )
        {
            if( NewPassword != string.Empty )
            {
                Password = NewPassword;
            }
        }
    }//CswNbtNodePropPassword

}//namespace ChemSW.Nbt.PropTypes
