using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassGenerator : CswNbtObjClass, ICswNbtPropertySetScheduler
    {
        public const string InspectionGeneratorNodeTypeName = "Inspection Schedule";

        public sealed class PropertyName
        {
            public static string DueDateInterval = "Due Date Interval";
            public static string RunTime = "Run Time";
            public static string FinalDueDate = "Final Due Date";
            public static string NextDueDate = "Next Due Date";
            public static string WarningDays = "Warning Days";
            //public static string GraceDaysPropertyName { get { return "Grace Days"; } }
            public static string Enabled = "Enabled";
            public static string RunStatus = "Run Status";
            public static string TargetType = "Target Type";
            public static string Owner = "Owner";
            public static string Description = "Description";
            public static string Summary = "Summary";
            public static string ParentType = "Parent Type";
            public static string ParentView = "Parent View";
            public static string RunNow = "Run Now";
        }



        //ICswNbtPropertySetScheduler
        public string SchedulerFinalDueDatePropertyName { get { return PropertyName.FinalDueDate; } }
        public string SchedulerNextDueDatePropertyName { get { return PropertyName.NextDueDate; } }
        public string SchedulerRunStatusPropertyName { get { return PropertyName.RunStatus; } }
        public string SchedulerWarningDaysPropertyName { get { return PropertyName.WarningDays; } }
        public string SchedulerDueDateIntervalPropertyName { get { return PropertyName.DueDateInterval; } }
        public string SchedulerRunTimePropertyName { get { return PropertyName.RunTime; } }
        public string SchedulerRunNowPropertyName { get { return PropertyName.RunNow; } }

        private CswNbtObjClassDefault _CswNbtObjClassDefault = null;
        private CswNbtPropertySetSchedulerImpl _CswNbtPropertySetSchedulerImpl;

        public CswNbtObjClassGenerator( CswNbtResources CswNbtResources, CswNbtNode Node )
            : base( CswNbtResources, Node )
        {
            _CswNbtObjClassDefault = new CswNbtObjClassDefault( _CswNbtResources, Node );
            _CswNbtPropertySetSchedulerImpl = new CswNbtPropertySetSchedulerImpl( _CswNbtResources, this, Node );
        }//ctor()

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassGenerator
        /// </summary>
        public static implicit operator CswNbtObjClassGenerator( CswNbtNode Node )
        {
            CswNbtObjClassGenerator ret = null;
            if( null != Node && _Validate( Node, CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass ) )
            {
                ret = (CswNbtObjClassGenerator) Node.ObjClass;
            }
            return ret;
        }

        private void _setDefaultValues()
        {
            if( TargetType.Empty )
            {
                Enabled.Checked = Tristate.False;
            }
        }

        #region Inherited Events

        public override void beforeWriteNode( bool IsCopy, bool OverrideUniqueValidation )
        {
            _CswNbtObjClassDefault.beforeWriteNode( IsCopy, OverrideUniqueValidation );
            _setDefaultValues();

            //Case 24572
            bool DeleteFutureNodes = ( TargetType.WasModified || ParentType.WasModified );
            _CswNbtPropertySetSchedulerImpl.updateNextDueDate( DeleteFutureNodes );


            _trySetNodeTypeSelectDefaultValues();

            // BZ 7845
            if( TargetType.Empty )
            {
                Enabled.Checked = Tristate.False;
            }
        } //beforeWriteNode()

        public override void afterWriteNode()
        {
            _CswNbtPropertySetSchedulerImpl.setLastFutureDate();
            _CswNbtObjClassDefault.afterWriteNode();
        }//afterWriteNode()

        private void _trySetNodeTypeSelectDefaultValues()
        {
            bool RequiresParentView = Owner.RelatedNodeId != null &&
                          ( Node.getNodeType().getFirstVersionNodeType().NodeTypeName == InspectionGeneratorNodeTypeName ||
                            ( ParentView.ViewId != null &&
                              ParentView.ViewId.isSet() ) );

            if( RequiresParentView )
            {
                CswNbtNode OwnerNode = _CswNbtResources.Nodes.GetNode( Owner.RelatedNodeId );
                Collection<CswNbtMetaDataNodeType> MatchingInspectionTargetNts = new Collection<CswNbtMetaDataNodeType>();

                bool SetDefaultParentType = ( ( false == ParentType.WasModified ||
                                                ParentType.SelectedNodeTypeIds.Count == 0 ) &&
                                                null != OwnerNode &&
                                                OwnerNode.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetGroupClass &&
                                                ParentType.SelectMode != PropertySelectMode.Blank );
                if( SetDefaultParentType )
                {
                    CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass );
                    foreach( CswNbtMetaDataNodeType InspectionTargetNt in InspectionTargetOc.getNodeTypes() )
                    {
                        if( InspectionTargetNt.IsLatestVersion() )
                        {
                            CswNbtMetaDataNodeTypeProp TargetGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.InspectionTargetGroup );
                            if( TargetGroupNtp.IsFK &&
                                NbtViewRelatedIdType.NodeTypeId.ToString() == TargetGroupNtp.FKType &&
                                Int32.MinValue != TargetGroupNtp.FKValue )
                            {
                                CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.getNodeType( TargetGroupNtp.FKValue ).getNodeTypeLatestVersion();
                                if( null != InspectionTargetGroupNt &&
                                    false == MatchingInspectionTargetNts.Contains( InspectionTargetNt ) &&
                                    InspectionTargetGroupNt == OwnerNode.getNodeTypeLatestVersion() )
                                {
                                    MatchingInspectionTargetNts.Add( InspectionTargetNt );
                                    ParentType.SelectedNodeTypeIds.Add( InspectionTargetNt.NodeTypeId.ToString(), false, true );
                                    if( ParentType.SelectMode == PropertySelectMode.Single )
                                    {
                                        break;
                                    }
                                }
                            } // is valid FK
                        } // if( InspectionTargetNt.IsLatestVersion )
                    } // foreach( CswNbtMetaDataNodeType InspectionTargetNt in InspectionTargetOc.NodeTypes )
                } // if( SetDefaultTargetType )

                bool SetDefaultTargetType = ( ( false == TargetType.WasModified ||
                                            TargetType.SelectedNodeTypeIds.Count == 0 ) &&
                                          TargetType.SelectMode != PropertySelectMode.Blank &&
                                          ( MatchingInspectionTargetNts.Count > 0 ||
                                            TargetType.SelectedNodeTypeIds.Count > 0 ) );
                if( SetDefaultTargetType )
                {
                    if( MatchingInspectionTargetNts.Count == 0 )
                    {
                        foreach( Int32 InspectionTargetNodeTypeId in TargetType.SelectedNodeTypeIds.ToIntCollection() )
                        {
                            CswNbtMetaDataNodeType InspectionTargetNt = _CswNbtResources.MetaData.getNodeType( InspectionTargetNodeTypeId );
                            if( null != InspectionTargetNt )
                            {
                                CswNbtMetaDataNodeType LatestInspectionTargetNt = InspectionTargetNt.getNodeTypeLatestVersion();
                                if( LatestInspectionTargetNt.getObjectClass().ObjectClass == CswNbtMetaDataObjectClass.NbtObjectClass.InspectionTargetClass &&
                                    false == MatchingInspectionTargetNts.Contains( LatestInspectionTargetNt ) )
                                {
                                    MatchingInspectionTargetNts.Add( LatestInspectionTargetNt );
                                }
                            }
                        }
                    }
                    if( MatchingInspectionTargetNts.Count > 0 )
                    {
                        CswNbtMetaDataObjectClass InspectionDesignOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.InspectionDesignClass );
                        foreach( CswNbtMetaDataNodeType InspectionDesignNt in InspectionDesignOc.getNodeTypes() )
                        {
                            if( InspectionDesignNt.IsLatestVersion() )
                            {
                                CswNbtMetaDataNodeTypeProp DesignTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target );
                                foreach( CswNbtMetaDataNodeType MatchingInspectionTargetNt in MatchingInspectionTargetNts )
                                {
                                    if( DesignTargetNtp.IsFK &&
                                        NbtViewRelatedIdType.NodeTypeId.ToString() == DesignTargetNtp.FKType &&
                                        Int32.MinValue != DesignTargetNtp.FKValue )
                                    {
                                        if( MatchingInspectionTargetNt.NodeTypeId == DesignTargetNtp.FKValue )
                                        {
                                            TargetType.SelectedNodeTypeIds.Add( InspectionDesignNt.NodeTypeId.ToString(), false, true );
                                            if( TargetType.SelectMode == PropertySelectMode.Single )
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void _deleteFutureNodes()
        {
            // BZ 6754 - Delete all future nodes
            CswNbtMetaDataObjectClass GeneratorObjectClass = ObjectClass;

            if( TargetType.SelectedNodeTypeIds.Count == 1 )
            {
                Int32 SelectedTargetNtId = CswConvert.ToInt32( TargetType.SelectedNodeTypeIds[0] );
                if( Int32.MinValue != SelectedTargetNtId )
                {
                    CswNbtMetaDataNodeType TargetNodeType = _CswNbtResources.MetaData.getNodeType( SelectedTargetNtId );
                    if( null != TargetNodeType )
                    {
                        CswNbtMetaDataObjectClass TargetObjectClass = TargetNodeType.getObjectClass();

                        CswNbtObjClass TargetObjClass = CswNbtObjClassFactory.makeObjClass( _CswNbtResources, TargetObjectClass );
                        if( !( TargetObjClass is ICswNbtPropertySetGeneratorTarget ) )
                        {
                            throw new CswDniException( "CswNbtObjClassGenerator.beforeDeleteNode() got an invalid object class: " + TargetObjectClass.ObjectClass.ToString() );
                        }
                        ICswNbtPropertySetGeneratorTarget GeneratorTarget = (ICswNbtPropertySetGeneratorTarget) TargetObjClass;

                        CswNbtMetaDataNodeTypeProp GeneratorProp = TargetNodeType.getNodeTypePropByObjectClassProp( GeneratorTarget.GeneratorTargetGeneratorPropertyName );
                        CswNbtMetaDataNodeTypeProp IsFutureProp = TargetNodeType.getNodeTypePropByObjectClassProp( GeneratorTarget.GeneratorTargetIsFuturePropertyName );

                        CswNbtView View = new CswNbtView( _CswNbtResources );
                        View.ViewName = "CswNbtObjClassSchedule.beforeDeleteNode()";
                        CswNbtViewRelationship GeneratorRelationship = View.AddViewRelationship( GeneratorObjectClass, false );
                        GeneratorRelationship.NodeIdsToFilterIn.Add( _CswNbtNode.NodeId );
                        CswNbtViewRelationship TargetRelationship = View.AddViewRelationship( GeneratorRelationship, NbtViewPropOwnerType.Second, GeneratorProp, false );
                        CswNbtViewProperty IsFutureProperty = View.AddViewProperty( TargetRelationship, IsFutureProp );
                        View.AddViewPropertyFilter( IsFutureProperty, CswNbtSubField.SubFieldName.Checked, CswNbtPropFilterSql.PropertyFilterMode.Equals, "True", false );

                        ICswNbtTree TargetTree = _CswNbtResources.Trees.getTreeFromView( View, true, true, false, false );

                        TargetTree.goToRoot();
                        if( TargetTree.getChildNodeCount() > 0 ) // should always be the case
                        {
                            TargetTree.goToNthChild( 0 );
                            if( TargetTree.getChildNodeCount() > 0 ) // might not always be the case
                            {
                                for( int i = 0; i < TargetTree.getChildNodeCount(); i += 1 )
                                {
                                    TargetTree.goToNthChild( i );

                                    CswNbtNode TargetNode = TargetTree.getNodeForCurrentPosition();
                                    TargetNode.delete();

                                    TargetTree.goToParentNode();
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void beforeDeleteNode( bool DeleteAllRequiredRelatedNodes = false )
        {
            _deleteFutureNodes();
            _CswNbtObjClassDefault.beforeDeleteNode( DeleteAllRequiredRelatedNodes );
        } //beforeDeleteNode()

        public override void afterDeleteNode()
        {
            _CswNbtObjClassDefault.afterDeleteNode();
        }//afterDeleteNode()        

        public override void afterPopulateProps()
        {
            DueDateInterval.SetOnPropChange( OnDueDateIntervalChange );
            _CswNbtObjClassDefault.afterPopulateProps();
        }//afterPopulateProps()

        public override void addDefaultViewFilters( CswNbtViewRelationship ParentRelationship )
        {
            _CswNbtObjClassDefault.addDefaultViewFilters( ParentRelationship );
        }

        public override bool onButtonClick( NbtButtonData ButtonData )
        {




            CswNbtMetaDataObjectClassProp OCP = ButtonData.NodeTypeProp.getObjectClassProp();
            if( null != ButtonData.NodeTypeProp && null != OCP )
            {
                if( PropertyName.RunNow == OCP.PropName )
                {
                    NextDueDate.DateTimeValue = DateTime.Now;
                    //case 25702 - empty comment?
                    //RunStatus.StaticText = string.Empty;
                    //RunStatus_new.a
                    Node.postChanges( false );
                    ButtonData.Action = NbtButtonAction.refresh;
                }
            }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropDateTime FinalDueDate { get { return ( _CswNbtNode.Properties[PropertyName.FinalDueDate] ); } }
        public CswNbtNodePropDateTime NextDueDate { get { return ( _CswNbtNode.Properties[PropertyName.NextDueDate] ); } }

        /// <summary>
        /// Node type of target, where target is the node generated by schedule
        /// </summary>
        public CswNbtNodePropNodeTypeSelect TargetType { get { return ( _CswNbtNode.Properties[PropertyName.TargetType] ); } }

        public CswNbtNodePropMemo Description { get { return ( _CswNbtNode.Properties[PropertyName.Description] ); } }

        /// <summary>
        /// In IMCS, owner == Equipment or Assembly node, in FE owner == Location Group node
        /// </summary>
        public CswNbtNodePropRelationship Owner { get { return ( _CswNbtNode.Properties[PropertyName.Owner] ); } }
        public CswNbtNodePropComments RunStatus { get { return ( _CswNbtNode.Properties[PropertyName.RunStatus] ); } }

        /// <summary>
        /// Days before due date to anticipate upcoming event
        /// </summary>
        public CswNbtNodePropNumber WarningDays { get { return ( _CswNbtNode.Properties[PropertyName.WarningDays] ); } }
        public CswNbtNodePropText Summary { get { return ( _CswNbtNode.Properties[PropertyName.Summary] ); } }
        public CswNbtNodePropTimeInterval DueDateInterval { get { return ( _CswNbtNode.Properties[PropertyName.DueDateInterval] ); } }
        public void OnDueDateIntervalChange( CswNbtNodeProp Prop )
        {
            if( DueDateInterval.RateInterval.RateType == CswRateInterval.RateIntervalType.Hourly )
            {
                RunTime.setHidden( value: true, SaveToDb: true );
            }
            else
            {
                RunTime.setHidden( value: false, SaveToDb: true );
            }
        } // OnDueDateIntervalChange
        public CswNbtNodePropDateTime RunTime { get { return ( _CswNbtNode.Properties[PropertyName.RunTime] ); } }
        public CswNbtNodePropLogical Enabled { get { return ( _CswNbtNode.Properties[PropertyName.Enabled] ); } }

        /// <summary>
        /// Node type of parent. In FE parent is node type of Fire Extinguisher or Inspection Target. In IMCS, parent type is not used.
        /// </summary>
        public CswNbtNodePropNodeTypeSelect ParentType { get { return ( _CswNbtNode.Properties[PropertyName.ParentType] ); } }

        /// <summary>
        /// View from owner to parent. In FE this is Location Group > Location > Inspection Target > Inspection. Parent view not utilized elsewhere, yet.
        /// </summary>
        public CswNbtNodePropViewReference ParentView { get { return ( _CswNbtNode.Properties[PropertyName.ParentView] ); } }

        /// <summary>
        /// Run Now button clears the Last Run Date thereby forcing scheduler to process the Generator node on its next iteration 
        /// </summary>
        public CswNbtNodePropButton RunNow { get { return ( _CswNbtNode.Properties[PropertyName.RunNow] ); } }

        #endregion

    }//CswNbtObjClassGenerator

}//namespace ChemSW.Nbt.ObjClasses
