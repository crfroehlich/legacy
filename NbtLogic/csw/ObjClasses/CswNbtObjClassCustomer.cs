using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using ChemSW.Security;
using System;
using System.Collections.ObjectModel;


namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassCustomer : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string IPFilterRegex = "IP Filter Regex";
            public const string SubscriptionExpirationDate = "Subscription Expiration Date";
            public const string Deactivated = "Deactivated";
            public const string CompanyID = "Company ID";
            public const string UserCount = "User Count";
            public const string ModulesEnabled = "Modules Enabled";
            public const string Login = "Login";
            public const string SchemaName = "Schema Name";
            public const string SchemaVersion = "Schema Version";
            public const string PendingFeedbackCount = "Pending Feedback Count";
        }

        public CswNbtObjClassCustomer( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.CustomerClass ); }
        }

        private bool _CompanyIDDefined()
        {
            return ( CompanyID.Text != string.Empty && _CswNbtResources.CswDbCfgInfo.ConfigurationExists( CompanyID.Text ) );
        }

        private CswNbtResources makeOtherResources()
        {
            CswNbtResources OtherResources = CswNbtResourcesFactory.makeCswNbtResources( _CswNbtResources );
            OtherResources.AccessId = CompanyID.Text;
            OtherResources.InitCurrentUser = InitUser;
            return OtherResources;
        }
        public ICswUser InitUser( ICswResources Resources )
        {
            ICswUser RetUser = new CswNbtSystemUser( _CswNbtResources, CswEnumSystemUserNames.SysUsr_ObjClassCustomer );
            return RetUser;
        }

        private void finalizeOtherResources( CswNbtResources OtherResources )
        {
            OtherResources.finalize();
            OtherResources.release();
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassCustomer
        /// </summary>
        public static implicit operator CswNbtObjClassCustomer( CswNbtNode Node )
        {
            CswNbtObjClassCustomer ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.CustomerClass ) )
            {
                ret = (CswNbtObjClassCustomer) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void beforeWriteNodeLogic( bool Creating, bool OverrideUniqueValidation )
        {
            _checkForConfigFileUpdate();
        }

        protected override void afterWriteNodeLogic()
        {
            _doConfigFileUpdate();
        } // afterWriteNode()

        bool UpdateConfigFile = false;
        private void _checkForConfigFileUpdate()
        {
            if( ( Deactivated.wasAnySubFieldModified() || IPFilterRegex.wasAnySubFieldModified() || UserCount.wasAnySubFieldModified() ) && _CompanyIDDefined() )
            {
                UpdateConfigFile = true;
            }
        }

        private void _doConfigFileUpdate()
        {
            if( UpdateConfigFile )
            {
                // Update the value of Deactivated and IP Filter Regex in the DbConfig file
                _CswNbtResources.CswDbCfgInfo.makeConfigurationCurrent( CompanyID.Text );
                _CswNbtResources.CswDbCfgInfo.CurrentDeactivated = ( Deactivated.Checked == CswEnumTristate.True );
                _CswNbtResources.CswDbCfgInfo.CurrentIPFilterRegex = IPFilterRegex.Text;
                if( !Double.IsNaN( UserCount.Value ) )   // BZ 7822
                    _CswNbtResources.CswDbCfgInfo.CurrentUserCount = UserCount.Value.ToString();
                UpdateConfigFile = false;
            }
        }

        /// <summary>
        /// Set property values according to the value in the DbConfig file or from the target schema
        /// </summary>
        public void syncCustomerInfo()
        {
            if( _CompanyIDDefined() )
            {
                // get data from DbConfig file
                _CswNbtResources.CswDbCfgInfo.makeConfigurationCurrent( CompanyID.Text );
                if( _CswNbtResources.CswDbCfgInfo.CurrentDeactivated )
                    Deactivated.Checked = CswEnumTristate.True;
                else
                    Deactivated.Checked = CswEnumTristate.False;
                IPFilterRegex.Text = _CswNbtResources.CswDbCfgInfo.CurrentIPFilterRegex;
                if( CswTools.IsInteger( _CswNbtResources.CswDbCfgInfo.CurrentUserCount ) )
                    UserCount.Value = CswConvert.ToInt32( _CswNbtResources.CswDbCfgInfo.CurrentUserCount );
                else
                    UserCount.Value = Double.NaN;

                // case 25960
                this.SchemaName.StaticText = _CswNbtResources.CswDbCfgInfo.CurrentUserName;

                CswCommaDelimitedString YValues = new CswCommaDelimitedString();
                foreach( CswEnumNbtModuleName ModuleName in CswEnumNbtModuleName.All )
                {
                    if( CswEnumNbtModuleName.Unknown != ModuleName )
                    {
                        YValues.Add( ModuleName.ToString() );
                    }
                }
                ModulesEnabled.YValues = YValues;
                ModulesEnabled.XValues = new CswCommaDelimitedString() { ModulesEnabledXValue };

                // get data from target schema
                //string OriginalAccessId = _CswNbtResources.AccessId;
                //_CswNbtResources.AccessId = CompanyID.Text;
                CswNbtResources OtherResources = makeOtherResources();

                Collection<CswEnumNbtModuleName> Modules = new Collection<CswEnumNbtModuleName>();
                foreach( CswEnumNbtModuleName Module in OtherResources.Modules.ModulesEnabled() )
                {
                    Modules.Add( Module );
                }

                // case 25960
                string OtherSchemaVersion = OtherResources.ConfigVbls.getConfigVariableValue( CswEnumNbtConfigurationVariables.schemaversion.ToString() );

                //case 28079 - count the number of pending feedback nodes
                int count = 0;
                //Sourced from case 29852. If this schema is out of date, it's very possible that touching MetaData will generate an exception. 
                //Per Steve, we're not doing anything about this now.
                CswNbtMetaDataObjectClass feedbackOC = OtherResources.MetaData.getObjectClass( CswEnumNbtObjectClass.FeedbackClass );
                foreach( CswNbtObjClassFeedback feedbackNode in feedbackOC.getNodes( false, false ) )
                {
                    if( feedbackNode.Status.Value.Equals( CswNbtObjClassFeedback.Statuses.PendingReview ) )
                    {
                        count++;
                    }
                }
                PendingFeedbackCount.Value = count;

                // reconnect to original schema
                //_CswNbtResources.AccessId = OriginalAccessId;
                finalizeOtherResources( OtherResources );

                foreach( CswEnumNbtModuleName ModuleName in CswEnumNbtModuleName.All )
                {
                    if( CswEnumNbtModuleName.Unknown != ModuleName )
                    {
                        ModulesEnabled.SetValue( ModulesEnabledXValue, ModuleName.ToString(), Modules.Contains( ModuleName ) );
                    }
                }

                this.SchemaVersion.StaticText = OtherSchemaVersion;

                //case 29751
                if( CompanyID.Text == _CswNbtResources.AccessId )
                {
                    Login.setHidden( true, false );
                }
            }
        } // syncCustomerInfo()

        protected override void afterPopulateProps()
        {
            SchemaVersion.SetOnBeforeRender( delegate( CswNbtNodeProp Prop )
                {
                    syncCustomerInfo();
                } );

            CompanyID.SetOnPropChange( OnCompanyIdPropChange );
        }//afterPopulateProps()

        public static string ModulesEnabledXValue = "Enabled";

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp )
            {
                //Remember: Save is an OCP too
                if( PropertyName.Login == ButtonData.NodeTypeProp.getObjectClassPropName() )
                {
                    ButtonData.Action = CswEnumNbtButtonAction.reauthenticate;
                }
            }
            return true;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText IPFilterRegex
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.IPFilterRegex] );
            }
        }
        public CswNbtNodePropDateTime SubscriptionExpirationDate
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.SubscriptionExpirationDate] );
            }
        }
        public CswNbtNodePropLogical Deactivated
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Deactivated] );
            }
        }
        public CswNbtNodePropText CompanyID
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.CompanyID] );
            }
        }
        private void OnCompanyIdPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if ( false == CswTools.IsValidUsername( CompanyID.Text ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "The suppplied CustomerId is not in a valid format.", "CustomerId: {" + CompanyID.Text + "} is not well-formed." );
            }
        }

        public CswNbtNodePropNumber UserCount
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.UserCount] );
            }
        }
        public CswNbtNodePropLogicalSet ModulesEnabled
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.ModulesEnabled] );
            }
        }
        public CswNbtNodePropButton Login
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.Login] );
            }
        }
        public CswNbtNodePropStatic SchemaVersion
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.SchemaVersion] );
            }
        }
        public CswNbtNodePropStatic SchemaName
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.SchemaName] );
            }
        }
        public CswNbtNodePropNumber PendingFeedbackCount
        {
            get
            {
                return ( _CswNbtNode.Properties[PropertyName.PendingFeedbackCount] );
            }
        }

        #endregion

    }//CswNbtObjClassCustomer

}//namespace ChemSW.Nbt.ObjClasses