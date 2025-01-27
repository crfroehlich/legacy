using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.ServiceDrivers;
using ChemSW.Nbt.UnitsOfMeasure;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassInventoryLevel : CswNbtObjClass
    {
        /// <summary>
        /// Property Nammes
        /// </summary>
        public new sealed class PropertyName : CswNbtObjClass.PropertyName
        {
            public const string Material = "Material";
            public const string Type = "Type";
            public const string Level = "Level";
            public const string Subscribe = "Subscribe";
            public const string Location = "Location";
            public const string LastNotified = "Last Notified";
            public const string Status = "Status";
            public const string CurrentQuantity = "Current Quantity";
            public const string CurrentQuantityLog = "Current Quantity Log";
        }

        /// <summary>
        /// Possible statuses
        /// </summary>
        public sealed class Statuses
        {
            public const string Above = "Above Inventory Level";
            public const string Below = "Below Inventory Level";
            public const string Ok = "Ok";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Above, Below, Ok };
        }

        /// <summary>
        /// Possible Types
        /// </summary>
        public sealed class Types
        {
            public const string Minimum = "Minimum";
            public const string Maximum = "Maximum";
            public static readonly CswCommaDelimitedString Options = new CswCommaDelimitedString { Minimum, Maximum };
        }
        private CswNbtSdInventoryLevelMgr _LevelMgr = null;

        public static implicit operator CswNbtObjClassInventoryLevel( CswNbtNode Node )
        {
            CswNbtObjClassInventoryLevel ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.InventoryLevelClass ) )
            {
                ret = (CswNbtObjClassInventoryLevel) Node.ObjClass;
            }
            return ret;
        }

        public CswNbtObjClassInventoryLevel copyNode()
        {
            CswNbtObjClassInventoryLevel RetCopy = _CswNbtResources.Nodes.makeNodeFromNodeTypeId( NodeTypeId, delegate( CswNbtNode CopyNode )
                {
                    CopyNode.copyPropertyValues( Node );
                    //RetCopy.postChanges( true );
                } );
            return RetCopy;
        }

        public CswNbtObjClassInventoryLevel( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _LevelMgr = new CswNbtSdInventoryLevelMgr( _CswNbtResources );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InventoryLevelClass ); }
        }

        #region Inherited Events

        protected override void afterPopulateProps()
        {
            Level.SetOnPropChange( OnLevelPropChange );
            Material.SetOnPropChange( OnMaterialPropChange );
            CurrentQuantity.SetOnPropChange( OnCurrrentQuantityPropChange );
            Location.SetOnPropChange( OnLocationPropChange );
        }//afterPopulateProps()

        #endregion

        #region Object class specific properties

        public CswNbtNodePropList Type { get { return _CswNbtNode.Properties[PropertyName.Type]; } }
        public CswNbtNodePropQuantity Level { get { return _CswNbtNode.Properties[PropertyName.Level]; } }
        private void OnLevelPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( Level.UnitId != CurrentQuantity.UnitId )
            {
                CurrentQuantity.UnitId = Level.UnitId;
                CurrentQuantity.Quantity = _LevelMgr.getCurrentInventoryLevel( this );
                CurrentQuantityLog.AddComment( "Set initial Inventory Level Quantity: " + CurrentQuantity.Gestalt );
            }
        }
        public CswNbtNodePropRelationship Material { get { return _CswNbtNode.Properties[PropertyName.Material]; } }
        private void OnMaterialPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( CswTools.IsPrimaryKey( Material.RelatedNodeId ) )
            {
                CswNbtNode MaterialNode = _CswNbtResources.Nodes[Material.RelatedNodeId];
                if( null != MaterialNode )
                {
                    CswNbtUnitViewBuilder Vb = new CswNbtUnitViewBuilder( _CswNbtResources );
                    Vb.setQuantityUnitOfMeasureView( MaterialNode, CurrentQuantity );
                    Vb.setQuantityUnitOfMeasureView( MaterialNode, Level );
                }
            }
        }

        public CswNbtNodePropLocation Location { get { return _CswNbtNode.Properties[PropertyName.Location]; } }
        private void OnLocationPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            CurrentQuantity.Quantity = _LevelMgr.getCurrentInventoryLevel( this );
        }

        public CswNbtNodePropDateTime LastNotified { get { return _CswNbtNode.Properties[PropertyName.LastNotified]; } }
        public CswNbtNodePropUserSelect Subscribe { get { return _CswNbtNode.Properties[PropertyName.Subscribe]; } }
        public CswNbtNodePropList Status { get { return _CswNbtNode.Properties[PropertyName.Status]; } }
        public CswNbtNodePropQuantity CurrentQuantity { get { return _CswNbtNode.Properties[PropertyName.CurrentQuantity]; } }
        private void OnCurrrentQuantityPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( _LevelMgr.isLevelPastThreshhold( this ) )
            {
                if( CurrentQuantity.Quantity > Level.Quantity )
                {
                    Status.Value = Statuses.Above;
                }
                else
                {
                    Status.Value = Statuses.Below;
                }
            }
            else
            {
                Status.Value = Statuses.Ok;
            }
            if( _LevelMgr.doSendEmail( this ) )
            {
                LastNotified.DateTimeValue = _LevelMgr.sendPastThreshholdEmail( this );
            }
        }

        public CswNbtNodePropComments CurrentQuantityLog { get { return _CswNbtNode.Properties[PropertyName.CurrentQuantityLog]; } }

        #endregion

    }//CswNbtObjClassInventoryLevel

}//namespace ChemSW.Nbt.ObjClasses