using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    /// <summary>
    /// Permission Property Set
    /// </summary>
    public abstract class CswNbtPropertySetPermission : CswNbtObjClass
    {
        #region Enums

        /// <summary>
        /// Object Class property names
        /// </summary>
        public new class PropertyName : CswNbtObjClass.PropertyName
        {
            /// <summary>
            /// The Group with which to apply permissions
            /// </summary>
            public const string PermissionGroup = "Permission Group";
            /// <summary>
            /// The Work Unit to which this permission applies
            /// </summary>
            public const string WorkUnit = "WorkUnit";
            /// <summary>
            /// When checked, these permissions will apply to all Work Units by default.
            /// Permissions defined elsewhere that apply to specific Work Units will take precedence.
            /// </summary>
            public const string ApplyToAllWorkUnits = "Apply to all Work Units";
            /// <summary>
            /// The Role to which this permission applies
            /// </summary>
            public const string Role = "Role";
            /// <summary>
            /// When checked, these permissions will apply to all Roles by default.
            /// Permissions defined elsewhere that apply to specific Roles will take precedence.
            /// </summary>
            public const string ApplyToAllRoles = "Apply to all Roles";
            /// <summary>
            /// Permission to view the Target
            /// </summary>
            public const string View = "View";
            /// <summary>
            /// Permission to edit the Target
            /// </summary>
            public const string Edit = "Edit";
        }

        #endregion Enums

        #region Base

        /// <summary>
        /// Property Set ctor
        /// </summary>
        protected CswNbtPropertySetPermission( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.GenericClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtPropertySetPermission
        /// </summary>
        public static implicit operator CswNbtPropertySetPermission( CswNbtNode Node )
        {
            CswNbtPropertySetPermission ret = null;
            if( null != Node && Members().Contains( Node.ObjClass.ObjectClass.ObjectClass ) )
            {
                ret = (CswNbtPropertySetPermission) Node.ObjClass;
            }
            return ret;
        }

        public static Collection<CswEnumNbtObjectClass> Members()
        {
            Collection<CswEnumNbtObjectClass> Ret = new Collection<CswEnumNbtObjectClass>
            {
                CswEnumNbtObjectClass.InventoryGroupPermissionClass,
                CswEnumNbtObjectClass.ReportGroupPermissionClass,
                CswEnumNbtObjectClass.MailReportGroupPermissionClass
            };
            return Ret;
        }

        #endregion Base

        #region Abstract Methods

        /// <summary>
        /// Before write node event for derived classes to implement
        /// </summary>
        public virtual void beforePropertySetWriteNode() {}

        /// <summary>
        /// After write node event for derived classes to implement
        /// </summary>
        public virtual void afterPropertySetWriteNode() {}

        /// <summary>
        /// Before delete node event for derived classes to implement
        /// </summary>
        public virtual void beforePropertySetDeleteNode() {}

        /// <summary>
        /// After delete node event for derived classes to implement
        /// </summary>
        public virtual void afterPropertySetDeleteNode() {}

        /// <summary>
        /// Populate props event for derived classes to implement
        /// </summary>
        public virtual void afterPropertySetPopulateProps() {}

        /// <summary>
        /// Button click event for derived classes to implement
        /// </summary>
        public virtual bool onPropertySetButtonClick( NbtButtonData ButtonData ) { return true; }

        /// <summary>
        /// Mechanism to add default filters in derived classes
        /// </summary>
        public virtual void onPropertySetAddDefaultViewFilters( CswNbtViewRelationship ParentRelationship ) {}

        /// <summary>
        /// ObjectClass-specific logic for determining Action Permissions
        /// </summary>
        public virtual bool canAction( CswNbtAction Action ) { return true; }

        /// <summary>
        /// ObjectClass-specific logic for setting default wildcard permissions on a new Permission Group
        /// </summary>
        public virtual void setWildCardValues() {}

        #endregion Abstract Methods

        #region Inherited Events

        protected override void beforeWriteNodeLogic( bool Creating, bool OverrideUniqueValidation )
        {
            beforePropertySetWriteNode();
            if( false == OverrideUniqueValidation )
            {
                _validateCompoundUniqueness();
            }
        }

        protected override void afterWriteNodeLogic()
        {
            afterPropertySetWriteNode();
        }

        protected override void beforeDeleteNodeLogic()
        {
            beforePropertySetDeleteNode();
        }

        protected override void afterDeleteNodeLogic()
        {
            afterPropertySetDeleteNode();
        }

        protected override void afterPopulateProps()
        {
            afterPropertySetPopulateProps();
            WorkUnit.SetOnPropChange( OnWorkUnitPropChange );
            ApplyToAllWorkUnits.SetOnPropChange( OnApplyToAllWorkUnitsPropChange );
            Role.SetOnPropChange( OnRolePropChange );
            ApplyToAllRoles.SetOnPropChange( OnApplyToAllRolesPropChange );
        }

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            onPropertySetAddDefaultViewFilters( ParentRelationship );
        }

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp ) { /*Do Something*/ }
            return true;
        }

        #endregion Inherited Events

        #region Custom Logic

        private void _validateCompoundUniqueness()
        {
            _validateWorkUnit();
            _validateRole();
            if( false == IsTemp )
            {
                CswNbtView MatchingPermissionsView = new CswNbtView( _CswNbtResources );
                CswNbtMetaDataObjectClassProp ApplyToAllWorkUnitsOCP = ObjectClass.getObjectClassProp( ApplyToAllWorkUnits.ObjectClassPropId );
                CswNbtMetaDataObjectClassProp WorkUnitOCP = ObjectClass.getObjectClassProp( WorkUnit.ObjectClassPropId );
                CswNbtMetaDataObjectClassProp ApplyToAllRolesOCP = ObjectClass.getObjectClassProp( ApplyToAllRoles.ObjectClassPropId );
                CswNbtMetaDataObjectClassProp RoleOCP = ObjectClass.getObjectClassProp( Role.ObjectClassPropId );
                CswNbtMetaDataObjectClassProp GroupOCP = ObjectClass.getObjectClassProp( PermissionGroup.ObjectClassPropId );

                CswNbtViewRelationship parent = MatchingPermissionsView.AddViewRelationship( ObjectClass, false );
                if( ApplyToAllWorkUnits.Checked == CswEnumTristate.True )
                {
                    MatchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: ApplyToAllWorkUnitsOCP,
                    Value: CswEnumTristate.True.ToString(),
                    SubFieldName: CswNbtFieldTypeRuleLogical.SubFieldName.Checked,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                }
                else
                {
                    MatchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: WorkUnitOCP,
                    Value: WorkUnit.RelatedNodeId.PrimaryKey.ToString(),
                    SubFieldName: CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                }
                if( ApplyToAllRoles.Checked == CswEnumTristate.True )
                {
                    MatchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: ApplyToAllRolesOCP,
                    Value: CswEnumTristate.True.ToString(),
                    SubFieldName: CswNbtFieldTypeRuleLogical.SubFieldName.Checked,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                }
                else
                {
                    MatchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: RoleOCP,
                    Value: Role.RelatedNodeId.PrimaryKey.ToString(),
                    SubFieldName: CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                }
                MatchingPermissionsView.AddViewPropertyAndFilter( parent,
                    MetaDataProp: GroupOCP,
                    Value: PermissionGroup.RelatedNodeId.PrimaryKey.ToString(),
                    SubFieldName: CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                    FilterMode: CswEnumNbtFilterMode.Equals );
                parent.NodeIdsToFilterOut.Add( this.NodeId );

                ICswNbtTree MatchingPermissionsTree = _CswNbtResources.Trees.getTreeFromView( MatchingPermissionsView, false, false, false );
                MatchingPermissionsTree.goToRoot();
                if( MatchingPermissionsTree.getChildNodeCount() > 0 )
                {
                    MatchingPermissionsTree.goToNthChild( 0 );
                    CswPrimaryKey DuplicateNodeId = MatchingPermissionsTree.getNodeIdForCurrentPosition();
                    throw new CswDniException(
                        CswEnumErrorType.Warning,
                        "A Permission with this Role, WorkUnit and " + PermissionGroup.PropName + " already exists",
                        "A node of nodeid " + DuplicateNodeId + " already exists with Role: \"" + Role.CachedNodeName +
                        "\", WorkUnit: \"" + WorkUnit.CachedNodeName + "\", and " + PermissionGroup.PropName + ": \"" + PermissionGroup.CachedNodeName + "\"." );
                }
            }
        }

        public static void createDefaultWildcardPermission( CswNbtResources _CswNbtResources, CswEnumNbtObjectClass PermissionClass, CswPrimaryKey GroupId )
        {
            CswNbtMetaDataObjectClass PermissionOC = _CswNbtResources.MetaData.getObjectClass( PermissionClass );
            CswNbtView PermissionsView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship PermissionVR = PermissionsView.AddViewRelationship( PermissionOC, IncludeDefaultFilters: false );
            CswNbtMetaDataObjectClassProp GroupOCP = PermissionOC.getObjectClassProp( PropertyName.PermissionGroup );
            PermissionsView.AddViewPropertyAndFilter( PermissionVR, GroupOCP, Value: GroupId.ToString(), SubFieldName: CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID );
            ICswNbtTree PermissionTree = _CswNbtResources.Trees.getTreeFromView( PermissionsView, false, false, false );
            if( PermissionTree.getChildNodeCount() == 0 )
            {
                CswNbtMetaDataNodeType PermissionNT = PermissionOC.FirstNodeType;
                if( null != PermissionNT )
                {
                    _CswNbtResources.Nodes.makeNodeFromNodeTypeId( PermissionNT.NodeTypeId, delegate( CswNbtNode NewNode )
                        {
                            CswNbtPropertySetPermission WildCardPermission = NewNode;
                            WildCardPermission.ApplyToAllRoles.Checked = CswEnumTristate.True;
                            WildCardPermission.ApplyToAllWorkUnits.Checked = CswEnumTristate.True;
                            WildCardPermission.PermissionGroup.RelatedNodeId = GroupId;
                            WildCardPermission.View.Checked = CswEnumTristate.True;
                            WildCardPermission.Edit.Checked = CswEnumTristate.True;
                            WildCardPermission.setWildCardValues();
                        } );
                }
            }
        }

        private void _validateWorkUnit()
        {
            if( false == IsTemp && ApplyToAllWorkUnits.Checked == CswEnumTristate.False && null == WorkUnit.RelatedNodeId )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "A Work Unit must be selected if 'Apply To All Work Units' is unchecked.", "User did not supply a valid Work Unit." );
            }
        }

        private void _validateRole()
        {
            if( false == IsTemp && ApplyToAllRoles.Checked == CswEnumTristate.False && null == Role.RelatedNodeId )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "A Role must be selected if 'Apply To All Roles' is unchecked.", "User did not supply a valid Role." );
            }
        }

        #endregion Custom Logic

        #region Property Set specific properties

        /// <summary>
        /// The Group with which all Targets shall have these Permissions applied
        /// </summary>
        public CswNbtNodePropRelationship PermissionGroup { get { return _CswNbtNode.Properties[PropertyName.PermissionGroup]; } }
        public CswNbtNodePropRelationship WorkUnit { get { return _CswNbtNode.Properties[PropertyName.WorkUnit]; } }
        private void OnWorkUnitPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if( null == WorkUnit.RelatedNodeId )
            {
                ApplyToAllWorkUnits.Checked = CswEnumTristate.True;
            }
        }
        public CswNbtNodePropLogical ApplyToAllWorkUnits { get { return _CswNbtNode.Properties[PropertyName.ApplyToAllWorkUnits]; } }
        private void OnApplyToAllWorkUnitsPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if( ApplyToAllWorkUnits.Checked == CswEnumTristate.True )
            {
                WorkUnit.RelatedNodeId = null;
                WorkUnit.RefreshNodeName();
            }
            else
            {
                _validateWorkUnit();
            }
        }
        public CswNbtNodePropRelationship Role { get { return _CswNbtNode.Properties[PropertyName.Role]; } }
        private void OnRolePropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if( null == Role.RelatedNodeId )
            {
                ApplyToAllRoles.Checked = CswEnumTristate.True;
            }
        }
        public CswNbtNodePropLogical ApplyToAllRoles { get { return _CswNbtNode.Properties[PropertyName.ApplyToAllRoles]; } }
        private void OnApplyToAllRolesPropChange( CswNbtNodeProp NodeProp, bool Creating )
        {
            if( ApplyToAllRoles.Checked == CswEnumTristate.True )
            {
                Role.RelatedNodeId = null;
                Role.RefreshNodeName();
            }
            else
            {
                _validateRole();
            }
        }
        public CswNbtNodePropLogical View { get { return _CswNbtNode.Properties[PropertyName.View]; } }
        public CswNbtNodePropLogical Edit { get { return _CswNbtNode.Properties[PropertyName.Edit]; } }

        #endregion Property Set specific properties

    }//CswNbtPropertySetPermission

}//namespace ChemSW.Nbt.ObjClasses