using System;
using System.Data;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.Actions.KioskModeRules.OperationClasses;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassEquipmentAssembly: CswNbtObjClass, ICswNbtKioskModeMoveable, ICswNbtKioskModeOwnerable, ICswNbtKioskModeTransferable, ICswNbtKioskModeStatusable
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string Type = "Assembly Type";
            public const string AssemblyParts = "Assembly Parts";
            public const string Barcode = "Barcode";
            public const string Location = "Location";
            public const string Status = "Status";
            public const string AssemblyCondition = "Assembly Condition";
            public const string AssemblyContractNo = "Assembly Contract No";
            public const string AssemblyDescription = "Assembly Description";
            public const string AssemblyHasServiceContract = "Assembly Has Service Contract";
            public const string AssemblyID = "Assembly ID";
            public const string AssemblyMTBF = "Assembly MTBF";
            public const string AssemblyManualStoredAt = "Assembly Manual Stored At";
            public const string AssemblyManufacturer = "Assembly Manufacturer";
            public const string AssemblyModel = "Assembly Model";
            public const string AssemblyNotes = "Assembly Notes";
            public const string AssemblyOutOn = "Assembly Out On";
            public const string AssemblyPropertyNo = "Assembly Property No";
            public const string AssemblyPurchased = "Assembly Purchased";
            public const string AssemblyReceived = "Assembly Received";
            public const string AssemblySerialNo = "Assembly Serial No";
            public const string AssemblyServiceCost = "Assembly Service Cost";
            public const string AssemblyServiceEndsOn = "Assembly Service Ends On";
            public const string AssemblyServicePhone = "Assembly Service Phone";
            public const string AssemblyServiceVendor = "Assembly Service Vendor";
            public const string AssemblyStartingCost = "Assembly Starting Cost";
            public const string AssemblyVendor = "Assembly Vendor";
            public const string Department = "Department";
            public const string Documents = "Documents";
            public const string Problem = "Problem";
            public const string Responsible = "Responsible";
            public const string Scheduling = "Scheduling";
            public const string Tasks = "Tasks";
            public const string User = "User";
            public const string UserPhone = "User Phone";
        }

        public static string PartsXValueName = "Uses";

        private readonly CswNbtKioskModeMoveableImpl _Mover;
        private readonly CswNbtKioskModeOwnerableImpl _Ownerer;
        private readonly CswNbtKioskModeTransferableImpl _Transferer;
        private readonly CswNbtKioskModeStatusableImpl _Statuser;

        public CswNbtObjClassEquipmentAssembly( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _Mover = new CswNbtKioskModeMoveableImpl( CswNbtResources, this );
            _Ownerer = new CswNbtKioskModeOwnerableImpl( CswNbtResources, this );
            _Transferer = new CswNbtKioskModeTransferableImpl( CswNbtResources, this );
            _Statuser = new CswNbtKioskModeStatusableImpl( CswNbtResources, this );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentAssemblyClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassEquipmentAssembly
        /// </summary>
        public static implicit operator CswNbtObjClassEquipmentAssembly( CswNbtNode Node )
        {
            CswNbtObjClassEquipmentAssembly ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.EquipmentAssemblyClass ) )
            {
                ret = (CswNbtObjClassEquipmentAssembly) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void afterWriteNodeLogic()
        {
            _updateEquipment();
        }//afterWriteNode()

        private void _updateEquipment()
        {
            // For each equipment related to this assembly, mark matching properties as pending update
            if( CswEnumNbtNodeModificationState.Modified == _CswNbtNode.ModificationState )
            {
                CswStaticSelect PropRefsSelect = _CswNbtResources.makeCswStaticSelect( "afterWriteNode_select", "getMatchingEquipPropsForAssembly" );
                CswStaticParam StaticParam = new CswStaticParam( "getassemblynodeid", _CswNbtNode.NodeId.PrimaryKey );
                PropRefsSelect.S4Parameters.Add( "getassemblynodeid", StaticParam );
                DataTable PropRefsTable = PropRefsSelect.getTable();

                // Update the nodes.pendingupdate directly, to avoid having to fetch all the node info for every related node 
                string PkString = String.Empty;
                foreach( DataRow PropRefsRow in PropRefsTable.Rows )
                {
                    if( PkString != String.Empty ) PkString += ",";
                    PkString += PropRefsRow["nodeid"].ToString();
                }
                if( PkString != String.Empty )
                {
                    CswTableUpdate NodesUpdate = _CswNbtResources.makeCswTableUpdate( "afterWriteNode_update", "nodes" );
                    DataTable NodesTable = NodesUpdate.getTable( "where nodeid in (" + PkString + ")" );
                    foreach( DataRow NodesRow in NodesTable.Rows )
                    {
                        NodesRow["pendingupdate"] = "1";
                    }
                    NodesUpdate.update( NodesTable );
                }
            }
        }

        protected override void afterPopulateProps()
        {
            if( Type.RelatedNodeId != null )
            {
                CswNbtNode TypeNode = _CswNbtResources.Nodes[Type.RelatedNodeId];
                if( TypeNode != null )
                {
                    CswNbtObjClassEquipmentType TypeNodeAsType = (CswNbtObjClassEquipmentType) TypeNode;
                    CswDelimitedString PartsString = new CswDelimitedString( '\n' );
                    PartsString.FromString( TypeNodeAsType.Parts.Text.Replace( "\r", "" ) );
                    AssemblyParts.YValues = PartsString;
                }
            }
        }//afterPopulateProps()

        public override CswNbtNode CopyNode( bool IsNodeTemp = false, Action<CswNbtNode> OnCopy = null )
        {
            // Copy this Assembly
            CswNbtNode CopiedAssemblyNode = base.CopyNodeImpl( IsNodeTemp, OnCopy );

            // Copy all Equipment
            CswNbtMetaDataObjectClass EquipmentObjectClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.EquipmentClass );
            CswNbtView EquipmentView = new CswNbtView( _CswNbtResources );
            CswNbtViewRelationship EquipmentRelationship = EquipmentView.AddViewRelationship( EquipmentObjectClass, false );
            CswNbtViewProperty AssemblyProperty = EquipmentView.AddViewProperty( EquipmentRelationship, EquipmentObjectClass.getObjectClassProp( CswNbtObjClassEquipment.PropertyName.Assembly ) );
            CswNbtViewPropertyFilter AssemblyIsOriginalFilter = EquipmentView.AddViewPropertyFilter(
                AssemblyProperty,
                CswNbtFieldTypeRuleRelationship.SubFieldName.NodeID,
                CswEnumNbtFilterMode.Equals,
                NodeId.PrimaryKey.ToString() );

            ICswNbtTree EquipmentTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, EquipmentView, true, false, false );
            EquipmentTree.goToRoot();
            Int32 c = 0;
            while( c < EquipmentTree.getChildNodeCount() )
            {
                EquipmentTree.goToNthChild( c );
                CswNbtObjClassEquipment OriginalEquipmentNode = EquipmentTree.getNodeForCurrentPosition();
                OriginalEquipmentNode.CopyNode( IsNodeTemp, delegate( CswNbtNode CopiedEquipmentNode )
                    {
                        ( (CswNbtObjClassEquipment) CopiedEquipmentNode ).Assembly.RelatedNodeId = CopiedAssemblyNode.NodeId;
                    } );
                EquipmentTree.goToParentNode();
                c++;
            }

            return CopiedAssemblyNode;
        }

        #endregion

        #region Object class specific properties

        public CswNbtNodePropRelationship Type { get { return ( _CswNbtNode.Properties[PropertyName.Type] ); } }
        public CswNbtNodePropLogicalSet AssemblyParts { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyParts] ); } }
        public CswNbtNodePropBarcode Barcode { get { return ( _CswNbtNode.Properties[PropertyName.Barcode] ); } }
        public CswNbtNodePropLocation Location { get { return ( _CswNbtNode.Properties[PropertyName.Location] ); } }
        public CswNbtNodePropList Status { get { return ( _CswNbtNode.Properties[PropertyName.Status] ); } }
        public CswNbtNodePropList AssemblyCondition { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyCondition] ); } }
        public CswNbtNodePropText AssemblyContractNo { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyContractNo] ); } }
        public CswNbtNodePropMemo AssemblyDescription { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyDescription] ); } }
        public CswNbtNodePropLogical AssemblyHasServiceContract { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyHasServiceContract] ); } }
        public CswNbtNodePropText AssemblyID { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyID] ); } }
        public CswNbtNodePropMTBF AssemblyMTBF { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyMTBF] ); } }
        public CswNbtNodePropText AssemblyManualStoredAt { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyManualStoredAt] ); } }
        public CswNbtNodePropText AssemblyManufacturer { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyManufacturer] ); } }
        public CswNbtNodePropText AssemblyModel { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyModel] ); } }
        public CswNbtNodePropMemo AssemblyNotes { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyNotes] ); } }
        public CswNbtNodePropDateTime AssemblyOutOn { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyOutOn] ); } }
        public CswNbtNodePropText AssemblyPropertyNo { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyPropertyNo] ); } }
        public CswNbtNodePropDateTime AssemblyPurchased { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyPurchased] ); } }
        public CswNbtNodePropDateTime AssemblyReceived { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyReceived] ); } }
        public CswNbtNodePropText AssemblySerialNo { get { return ( _CswNbtNode.Properties[PropertyName.AssemblySerialNo] ); } }
        public CswNbtNodePropText AssemblyServiceCost { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyServiceCost] ); } }
        public CswNbtNodePropDateTime AssemblyServiceEndsOn { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyServiceEndsOn] ); } }
        public CswNbtNodePropText AssemblyServicePhone { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyServicePhone] ); } }
        public CswNbtNodePropRelationship AssemblyServiceVendor { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyServiceVendor] ); } }
        public CswNbtNodePropText AssemblyStartingCost { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyStartingCost] ); } }
        public CswNbtNodePropRelationship AssemblyVendor { get { return ( _CswNbtNode.Properties[PropertyName.AssemblyVendor] ); } }
        public CswNbtNodePropRelationship Department { get { return ( _CswNbtNode.Properties[PropertyName.Department] ); } }
        public CswNbtNodePropGrid Documents { get { return ( _CswNbtNode.Properties[PropertyName.Documents] ); } }
        public CswNbtNodePropGrid Problem { get { return ( _CswNbtNode.Properties[PropertyName.Problem] ); } }
        public CswNbtNodePropText Responsible { get { return ( _CswNbtNode.Properties[PropertyName.Responsible] ); } }
        public CswNbtNodePropGrid Scheduling { get { return ( _CswNbtNode.Properties[PropertyName.Scheduling] ); } }
        public CswNbtNodePropGrid Tasks { get { return ( _CswNbtNode.Properties[PropertyName.Tasks] ); } }
        public CswNbtNodePropRelationship User { get { return ( _CswNbtNode.Properties[PropertyName.User] ); } }
        public CswNbtNodePropPropertyReference UserPhone { get { return ( _CswNbtNode.Properties[PropertyName.UserPhone] ); } }

        #endregion

        #region ICswNbtKioskModeMovable

        public bool CanMove( out string Error )
        {
            return _Mover.CanMove( out Error );
        }

        public void Move( CswNbtObjClassLocation LocationToMoveTo )
        {
            _Mover.Move( LocationToMoveTo );
        }

        #endregion

        #region ICswNbtKioskModeOwnerable

        public bool CanUpdateOwner( out string Error )
        {
            return _Ownerer.CanMove( out Error );
        }

        public void UpdateOwner( CswNbtObjClassUser NewOwner )
        {
            _Ownerer.UpdateOwner( NewOwner );
        }

        #endregion

        #region ICswNbtKioskModeTransferable

        public bool CanTransfer( out string Error )
        {
            return _Transferer.CanTransfer( out Error );
        }

        public void Transfer( CswNbtObjClassUser NewUser )
        {
            _Transferer.Transfer( NewUser );
        }

        #endregion

        #region ICswNbtKioskModeStatusable

        public bool CanChangeStatus( out string Error )
        {
            return _Statuser.CanChangeStatus( out Error );
        }

        public void ChangeStatus( string newStatus )
        {
            _Statuser.ChangeStatus( newStatus );
        }

        #endregion

    }//CswNbtObjClassEquipmentAssembly

}//namespace ChemSW.Nbt.ObjClasses
