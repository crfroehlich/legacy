using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.Actions
{
    public class CswNbtActUpdatePropertyValue
    {
        private CswNbtResources _CswNbtResources;

        public CswNbtActUpdatePropertyValue( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public void UpdateNode( CswNbtNode Node, bool ForceUpdate )
        {
            // BZ 10240
            if( ( Node.PendingUpdate || ForceUpdate ) && Node.getObjectClass().ObjectClass == CswEnumNbtObjectClass.EquipmentClass )
            {
                ( (CswNbtObjClassEquipment) Node ).SyncEquipmentToAssembly();
            }
            // Case 30126
            if( null != Node.getObjectClass().getPropertySet() )
            {
                if( ( Node.PendingUpdate || ForceUpdate ) && Node.getObjectClass().getPropertySet().Name == CswEnumNbtPropertySetName.MaterialSet )
                {
                    ( (CswNbtPropertySetMaterial) Node ).onUpdatePropertyValue();
                }
            }

            // Update all out of date values for a given node
            foreach( CswNbtNodePropWrapper PropWrapper in Node.Properties )
            {
                if( PropWrapper.PendingUpdate || Node.PendingUpdate || ForceUpdate )
                {
                    switch( PropWrapper.getFieldTypeValue() )
                    {
                        case CswEnumNbtFieldType.Composite:
                            CswNbtNodePropComposite CompositeProp = PropWrapper.AsComposite;
                            CompositeProp.RecalculateCompositeValue();
                            break;
                        case CswEnumNbtFieldType.Location:
                            CswNbtNodePropLocation LocationProp = PropWrapper.AsLocation;
                            LocationProp.RefreshNodeName();
                            break;
                        case CswEnumNbtFieldType.LogicalSet:
                            CswNbtNodePropLogicalSet LogicalSetProp = PropWrapper.AsLogicalSet;
                            LogicalSetProp.RefreshStringValue( SetPendingUpdate: false );
                            break;
                        case CswEnumNbtFieldType.NodeTypeSelect:
                            CswNbtNodePropNodeTypeSelect NodeTypeSelectProp = PropWrapper.AsNodeTypeSelect;
                            NodeTypeSelectProp.RefreshSelectedNodeTypeNames();
                            break;
                        case CswEnumNbtFieldType.PropertyReference:
                            CswNbtNodePropPropertyReference PropertyReferenceProp = PropWrapper.AsPropertyReference;
                            PropertyReferenceProp.RecalculateReferenceValue();
                            break;
                        case CswEnumNbtFieldType.Quantity:
                            CswNbtNodePropQuantity QuantityProp = PropWrapper.AsQuantity;
                            QuantityProp.RefreshNodeName();
                            break;
                        case CswEnumNbtFieldType.Relationship:
                            CswNbtNodePropRelationship RelationshipProp = PropWrapper.AsRelationship;
                            RelationshipProp.RefreshNodeName();
                            break;
                        case CswEnumNbtFieldType.ViewPickList:
                            CswNbtNodePropViewPickList ViewPickListProp = PropWrapper.AsViewPickList;
                            ViewPickListProp.RefreshViewName();
                            break;
                        case CswEnumNbtFieldType.ViewReference:
                            CswNbtNodePropViewReference ViewReferenceProp = PropWrapper.AsViewReference;
                            ViewReferenceProp.RefreshViewName();
                            break;
                        case CswEnumNbtFieldType.MTBF:
                            CswNbtNodePropMTBF MTBFProp = PropWrapper.AsMTBF;
                            MTBFProp.RefreshCachedValue();
                            break;
                        default:
                            if( PropWrapper.PendingUpdate )
                            {
                                PropWrapper.setPendingUpdate( false );
                            }
                            break;
                    } // switch (PropWrapper.FieldType.FieldType)
                } // if(PropWrapper.PendingUpdate)
            } // foreach (CswNbtNodePropWrapper PropWrapper in Node.Properties)

            if( Node.PendingUpdate )
            {
                Node.PendingUpdate = false;
            }
        }// UpdateNode()
    } // public class CswNbtActUpdatePropertyValue
} // namespace ChemSW.Nbt.Actions
