using System;
using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassProblem : CswNbtObjClass
    {
        public static string PartsPropertyName { get { return "Parts"; } }
        public static string OwnerPropertyName { get { return "Owner"; } }
        public static string DateOpenedPropertyName { get { return "Date Opened"; } }
        public static string DateClosedPropertyName { get { return "Date Closed"; } }
        public static string ClosedPropertyName { get { return "Closed"; } }
        public static string ReportedByPropertyName { get { return "Reported By"; } }
        public static string FailurePropertyName { get { return "Failure"; } }

        public static string PartsXValueName { get { return "Service"; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;

        public CswNbtObjClassProblem( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.ProblemClass ); }
        }

        #region Inherited Events
        public override void beforeCreateNode( bool OverrideUniqueValidation )
        {
            // BZ 10051 - Set the Date Opened to today
            if( DateOpened.DateTimeValue == DateTime.MinValue )
            {
                DateOpened.DateTimeValue = DateTime.Now;
            }
            if( ReportedBy.RelatedNodeId == null )
            {
                ReportedBy.RelatedNodeId = _CswNbtResources.CurrentNbtUser.UserId;
                ReportedBy.CachedNodeName = _CswNbtResources.CurrentNbtUser.Username;
            }
            _checkClosed();

            _CswNbtObjClassDefault.beforeCreateNode( OverrideUniqueValidation );
        } // beforeCreateNode()

        public override void afterCreateNode()
        {
            _CswNbtObjClassDefault.afterCreateNode();
        } // afterCreateNode()

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _checkClosed();

            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
        }//beforeWriteNode()

        private void _checkClosed()
        {
            // BZ 10051 - If we're closing the Problem, set the Date Closed to today
            if( Closed.Checked == Tristate.True && DateClosed.DateTimeValue == DateTime.MinValue )
                DateClosed.DateTimeValue = DateTime.Today;

            // case 25838 - don't clear existing values
            //// BZ 10051 - If we're reopening the Problem, clear the Date Closed
            //if( Closed.Checked == Tristate.False && DateClosed.DateTimeValue != DateTime.MinValue )
            //    DateClosed.DateTimeValue = DateTime.MinValue;
        }

        public override void afterWriteNode()
        {
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        public override void beforeDeleteNode()
        {
            _CswNbtObjClassDefault.beforeDeleteNode();

        }//beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            if( Owner.RelatedNodeId != null )
            {
                CswNbtNode EquipmentOrAssemblyNode = _CswNbtResources.Nodes[Owner.RelatedNodeId];
                if( EquipmentOrAssemblyNode != null )
                {
                    if( EquipmentOrAssemblyNode.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass )
                    {
                        CswCommaDelimitedString NewYValues = new CswCommaDelimitedString();
                        CswNbtObjClassEquipment EquipmentNodeAsEquipment = CswNbtNodeCaster.AsEquipment( EquipmentOrAssemblyNode );
                        //CswNbtObjClassEquipment Equipment = CswNbtObjClassFactory.Make( CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentClass ) as CswNbtObjClassEquipment;
                        foreach( string YValue in EquipmentNodeAsEquipment.Parts.YValues )
                        {
                            if( EquipmentNodeAsEquipment.Parts.CheckValue( CswNbtObjClassEquipment.PartsXValueName, YValue ) )
                                NewYValues.Add( YValue );
                        }
                        this.Parts.YValues = NewYValues;
                    }
                    else if( EquipmentOrAssemblyNode.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.EquipmentAssemblyClass )
                    {
                        CswCommaDelimitedString NewYValues = new CswCommaDelimitedString();
                        CswNbtObjClassEquipmentAssembly AssemblyNodeAsAssembly = CswNbtNodeCaster.AsEquipmentAssembly( EquipmentOrAssemblyNode );
                        foreach( string YValue in AssemblyNodeAsAssembly.AssemblyParts.YValues )
                        {
                            if( AssemblyNodeAsAssembly.AssemblyParts.CheckValue( CswNbtObjClassEquipmentAssembly.PartsXValueName, YValue ) )
                                NewYValues.Add( YValue );
                        }
                        this.Parts.YValues = NewYValues;
                    }
                } // if( EquipmentOrAssemblyNode != null )
            } // if( Owner.RelatedNodeId != null )

            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( CswNbtMetaDataNodeTypeProp NodeTypeProp, out NbtButtonAction ButtonAction, out string ActionData, out string Message )
        {
            Message = string.Empty;
            ActionData = string.Empty;
            ButtonAction = NbtButtonAction.Unknown;
            if( null != NodeTypeProp ) { /*Do Something*/ }
            return true;
        }
        #endregion

        #region Object class specific properties



        //public CswNbtNodePropRelationship Equipment
        //{
        //    get
        //    {
        //        return ( _CswNbtNode.Properties[EquipmentPropertyName].AsRelationship );
        //    }
        //}
        public CswNbtNodePropRelationship Owner
        {
            get
            {
                return ( _CswNbtNode.Properties[OwnerPropertyName].AsRelationship );
            }
        }

        public CswNbtNodePropRelationship ReportedBy
        {
            get
            {
                return ( _CswNbtNode.Properties[ReportedByPropertyName].AsRelationship );
            }
        }
        public CswNbtNodePropLogicalSet Parts
        {
            get
            {
                return ( _CswNbtNode.Properties[PartsPropertyName].AsLogicalSet );
            }
        }
        public CswNbtNodePropLogical Closed
        {
            get
            {
                return ( _CswNbtNode.Properties[ClosedPropertyName].AsLogical );
            }
        }

        public CswNbtNodePropDateTime DateOpened
        {
            get
            {
                return ( _CswNbtNode.Properties[DateOpenedPropertyName].AsDateTime );
            }
        }
        public CswNbtNodePropDateTime DateClosed
        {
            get
            {
                return ( _CswNbtNode.Properties[DateClosedPropertyName].AsDateTime );
            }
        }
        public CswNbtNodePropLogical Failure
        {
            get
            {
                return ( _CswNbtNode.Properties[FailurePropertyName].AsLogical );
            }
        }

        #endregion

    }//CswNbtObjClassProblem

}//namespace ChemSW.Nbt.ObjClasses
