using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropertySets;
using ChemSW.Nbt.PropTypes;
using ChemSW.WebSvc;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassMailReport: CswNbtObjClass, ICswNbtPropertySetScheduler, ICswNbtPermissionTarget
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string DueDateInterval = "Due Date Interval";
            public const string Enabled = "Enabled";
            public const string Event = "Event";
            public const string FinalDueDate = "Final Due Date";
            public const string LastProcessed = "Last Processed";
            public const string Message = "Message";
            public const string Name = "Name";
            public const string NextDueDate = "Next Due Date";
            public const string NodesToReport = "Nodes To Report";
            public const string OutputFormat = "Output Format";
            public const string Report = "Report";
            public const string ReportView = "Report View";
            public const string Recipients = "Recipients";
            public const string RunNow = "Run Now";
            public const string RunStatus = "Run Status";
            public const string RunTime = "Run Time";
            public const string TargetType = "Target Type";
            public const string Type = "Type";
            public const string WarningDays = "Warning Days";
            public const string MailReportGroup = "Mail Report Group";
        }

        public const string TypeOptionReport = "Report";
        public const string TypeOptionView = "View";

        //ICswNbtPropertySetScheduler
        public string SchedulerFinalDueDatePropertyName { get { return PropertyName.FinalDueDate; } }
        public string SchedulerNextDueDatePropertyName { get { return PropertyName.NextDueDate; } }
        public string SchedulerRunStatusPropertyName { get { return PropertyName.RunStatus; } }
        public string SchedulerWarningDaysPropertyName { get { return PropertyName.WarningDays; } }
        public string SchedulerDueDateIntervalPropertyName { get { return PropertyName.DueDateInterval; } }
        public string SchedulerRunTimePropertyName { get { return PropertyName.RunTime; } }

        private CswNbtPropertySetSchedulerImpl _CswNbtPropertySetSchedulerImpl;
        
        public CswNbtObjClassMailReport( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node )
        {
            _CswNbtPropertySetSchedulerImpl = new CswNbtPropertySetSchedulerImpl( _CswNbtResources, this, Node );
        }

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.MailReportClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassMailReport
        /// </summary>
        public static implicit operator CswNbtObjClassMailReport( CswNbtNode Node )
        {
            CswNbtObjClassMailReport ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.MailReportClass ) )
            {
                ret = (CswNbtObjClassMailReport) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void beforeWriteNodeLogic( bool Creating, bool OverrideUniqueValidation )
        {
            updateNextDueDate( ForceUpdate : false, DeleteFutureNodes : false );

            _assertMailReportIsValid();

            // Set default value for target type to root of view
            if( TargetType.Empty && Type.Value == TypeOptionView )
            {
                CswNbtView View = _CswNbtResources.ViewSelect.restoreView( ReportView.ViewId );
                if( null != View &&
                    View.Root.ChildRelationships.Count > 0 &&
                    View.Root.ChildRelationships[0].SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                {
                    TargetType.SelectedNodeTypeIds.Add( View.Root.ChildRelationships[0].SecondId.ToString() );
                }
            }
        }

        private void _assertMailReportIsValid()
        {
            string mailReportError = _getMailReportError();
            if( Enabled.Checked == CswEnumTristate.True && false == String.IsNullOrEmpty( mailReportError ) )
            {
                Enabled.Checked = CswEnumTristate.False;
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot Enable Mail Report: No " + mailReportError + " Selected.", "No " + mailReportError + " Selected." );
            }
        }

        private string _getMailReportError()
        {
            string mailReportError = String.Empty;
            if( Type.Empty )
            {
                mailReportError = "Type";
            }
            else
            {
                if( Type.Value == TypeOptionReport && Report.Empty )
                {
                    mailReportError = "Report";
                }
                else if( Type.Value == TypeOptionView && ReportView.Empty )
                {
                    mailReportError = "View";
                }
            }
            return mailReportError;
        }     

        protected override void afterPopulateProps()
        {
            Type.SetOnPropChange( OnTypePropChange );
            DueDateInterval.SetOnPropChange( OnDueDateIntervalChange );
            ReportView.SetOnPropChange( OnReportViewChange );
            // Case 29369: Event is a conditional property and therefore only conditionally required. 
            // Setting TemporarilyRequired should be good enough to meet the need.
            Event.TemporarilyRequired = true;
            Event.SetOnPropChange( onEventPropChange );
        }//afterPopulateProps()

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp )
            {
                //Remember: Save is an OCP too
                if( PropertyName.RunNow == ButtonData.NodeTypeProp.getObjectClassPropName() )
                {
                    if( DueDateInterval.getStartDate() < DateTime.Now ) //if the button was clicked after the first due date
                    {
                        NextDueDate.DateTimeValue = DateTime.Now;
                        Node.postChanges( false );
                        ButtonData.Action = CswEnumNbtButtonAction.refresh;
                    }
                    else
                    {
                        _CswNbtResources.Messages.Add( new CswWebSvcReturnBase.ErrorMessage()
                            {
                                ShowError = true,
                                Type = CswEnumErrorType.Warning,
                                Message = "A mail report cannot be run before its first scheduled due date.",
                                Detail = "Run now is invalid before initial due date."
                            } );
                    }
                }
            }
            return true;
        }

        public override CswNbtNode CopyNode( bool IsNodeTemp = false, Action<CswNbtNode> OnCopy = null )
        {
            CswNbtObjClassMailReport CopiedMailReport = base.CopyNodeImpl( IsNodeTemp, OnCopy );
            if( ReportView.ViewId.isSet() )
            {
                CswNbtView OriginalView = _CswNbtResources.ViewSelect.restoreView( ReportView.ViewId );
                CswNbtView CopiedView = _CswNbtResources.ViewSelect.restoreView( CopiedMailReport.ReportView.ViewId );
                CopiedView.CopyFromView( OriginalView );
                CopiedView.save();
            }
            return CopiedMailReport.Node;
        } // CopyNode()

        #endregion

        #region Object class specific properties

        public CswNbtNodePropTimeInterval DueDateInterval { get { return ( _CswNbtNode.Properties[PropertyName.DueDateInterval] ); } }
        public void OnDueDateIntervalChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( DueDateInterval.RateInterval.RateType == CswEnumRateIntervalType.Hourly )
            {
                RunTime.setHidden( value : true, SaveToDb : true );
            }
            else
            {
                RunTime.setHidden( value : false, SaveToDb : true );
            }
        } // OnDueDateIntervalChange
        public CswNbtNodePropLogical Enabled { get { return ( _CswNbtNode.Properties[PropertyName.Enabled] ); } }
        public CswNbtNodePropList Event { get { return ( _CswNbtNode.Properties[PropertyName.Event] ); } }
        private void onEventPropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( string.IsNullOrEmpty( Event.Value ) && Type.Value == TypeOptionView )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "View based Mail Reports must have a value for Event.", "Attempted to set a null or empty value to Event for a Report type of 'View'." );
            }
        }
        public CswNbtNodePropDateTime FinalDueDate { get { return ( _CswNbtNode.Properties[PropertyName.FinalDueDate] ); } }
        public CswNbtNodePropDateTime LastProcessed { get { return ( _CswNbtNode.Properties[PropertyName.LastProcessed] ); } }
        public CswNbtNodePropMemo Message { get { return ( _CswNbtNode.Properties[PropertyName.Message] ); } }
        public CswNbtNodePropText Name { get { return ( _CswNbtNode.Properties[PropertyName.Name] ); } }
        public CswNbtNodePropDateTime NextDueDate { get { return ( _CswNbtNode.Properties[PropertyName.NextDueDate] ); } }
        public CswNbtNodePropMemo NodesToReport { get { return ( _CswNbtNode.Properties[PropertyName.NodesToReport] ); } }
        public CswNbtNodePropList OutputFormat { get { return ( _CswNbtNode.Properties[PropertyName.OutputFormat] ); } }
        public CswNbtNodePropUserSelect Recipients { get { return ( _CswNbtNode.Properties[PropertyName.Recipients] ); } }
        public CswNbtNodePropRelationship Report { get { return ( _CswNbtNode.Properties[PropertyName.Report] ); } }
        public CswNbtNodePropViewReference ReportView { get { return ( _CswNbtNode.Properties[PropertyName.ReportView] ); } }
        public void OnReportViewChange( CswNbtNodeProp Prop, bool Creating )
        {
            // case 28844 - Change view visibility from 'Property' to 'Hidden', so grids will run correctly
            // This will only trigger when the viewid is first set.
            CswNbtView View = _CswNbtResources.ViewSelect.restoreView( ReportView.ViewId );
            View.SetVisibility( CswEnumNbtViewVisibility.Hidden, null, null );
            View.ViewMode = CswEnumNbtViewRenderingMode.Grid;
            View.save();
        } // OnReportViewChange()
        public CswNbtNodePropButton RunNow { get { return ( _CswNbtNode.Properties[PropertyName.RunNow] ); } }
        public CswNbtNodePropComments RunStatus { get { return ( _CswNbtNode.Properties[PropertyName.RunStatus] ); } }
        public CswNbtNodePropDateTime RunTime { get { return ( _CswNbtNode.Properties[PropertyName.RunTime] ); } }
        public CswNbtNodePropNodeTypeSelect TargetType { get { return ( _CswNbtNode.Properties[PropertyName.TargetType] ); } }
        public CswNbtNodePropList Type { get { return ( _CswNbtNode.Properties[PropertyName.Type] ); } }
        public void OnTypePropChange( CswNbtNodeProp Prop, bool Creating )
        {
            if( Type.Value == TypeOptionView )
            {
                OutputFormat.Value = CswEnumNbtMailReportFormatOptions.Link.ToString();

            }
        } // OnTypePropChange()
        public CswNbtNodePropNumber WarningDays { get { return ( _CswNbtNode.Properties[PropertyName.WarningDays] ); } }
        public CswNbtNodePropRelationship MailReportGroup { get { return ( _CswNbtNode.Properties[PropertyName.MailReportGroup] ); } }

        #endregion

        public void updateNextDueDate( bool ForceUpdate, bool DeleteFutureNodes )
        {
            _CswNbtPropertySetSchedulerImpl.updateNextDueDate( ForceUpdate, DeleteFutureNodes );
        }

        public CswCommaDelimitedString GetNodesToReport()
        {
            CswCommaDelimitedString NodesStr = new CswCommaDelimitedString();
            NodesStr.FromString( NodesToReport.Text );
            return NodesStr;
        } // GetNodesToReport()

        public void AddNodeToReport( CswNbtNode Node )
        {
            CswCommaDelimitedString NodesStr = new CswCommaDelimitedString();
            NodesStr.FromString( NodesToReport.Text );

            NodesStr.Add( Node.NodeId.PrimaryKey.ToString(), AllowNullOrEmpty : false, IsUnique : true );

            NodesToReport.Text = NodesStr.ToString();
        } // AddNodeToReport()

        public void ClearNodesToReport()
        {
            NodesToReport.Text = string.Empty;
        } // ClearNodesToReport()

        public CswPrimaryKey getPermissionGroupId()
        {
            return MailReportGroup.RelatedNodeId;
        }

    }//CswNbtObjClassMailReport

}//namespace ChemSW.Nbt.ObjClasses
