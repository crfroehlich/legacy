using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassReportGroup : CswNbtObjClass, ICswNbtPermissionGroup
    {
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            /// <summary>
            /// The name of the Report Group
            /// </summary>
            public const string Name = "Name";
            /// <summary>
            /// Description
            /// </summary>
            public const string Description = "Description";
            /// <summary>
            /// A grid of Reports that are assigned to this Group
            /// </summary>
            public const string Reports = "Reports";
            /// <summary>
            /// A grid of Permissions that are assigned to this Group
            /// </summary>
            public const string Permissions = "Permissions";
        }

        public CswEnumNbtObjectClass PermissionClass { get { return CswEnumNbtObjectClass.ReportGroupPermissionClass; } }
        public CswEnumNbtObjectClass TargetClass { get { return CswEnumNbtObjectClass.ReportClass; } }

        #region Base

        public CswNbtObjClassReportGroup( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.ReportGroupClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassReportGroup
        /// </summary>
        public static implicit operator CswNbtObjClassReportGroup( CswNbtNode Node )
        {
            CswNbtObjClassReportGroup ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.ReportGroupClass ) )
            {
                ret = (CswNbtObjClassReportGroup) Node.ObjClass;
            }
            return ret;
        }

        #endregion Base

        #region Inherited Events

        protected override void afterPromoteNodeLogic()
        {
            CswNbtPropertySetPermission.createDefaultWildcardPermission( _CswNbtResources, PermissionClass, NodeId );
        }
       
        #endregion Inherited Events

        #region Object class specific properties

        public CswNbtNodePropText Name { get { return _CswNbtNode.Properties[PropertyName.Name]; } }
        public CswNbtNodePropMemo Description { get { return _CswNbtNode.Properties[PropertyName.Description]; } }
        public CswNbtNodePropGrid Targets { get { return _CswNbtNode.Properties[PropertyName.Reports]; } }
        public CswNbtNodePropGrid Permissions { get { return _CswNbtNode.Properties[PropertyName.Permissions]; } }

        #endregion Object class specific properties

    }//CswNbtObjClassReportGroup

}//namespace ChemSW.Nbt.ObjClasses
