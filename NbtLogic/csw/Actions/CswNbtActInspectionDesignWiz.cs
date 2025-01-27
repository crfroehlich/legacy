using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using ChemSW.Audit;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.PropTypes;
using ChemSW.Nbt.Security;
using Newtonsoft.Json.Linq;


namespace ChemSW.Nbt.Actions
{
    public class CswNbtActInspectionDesignWiz
    {
        #region ctor
        CswNbtResources _CswNbtResources = null;
        private readonly ICswNbtUser _CurrentUser;
        private readonly TextInfo _TextInfo;
        private bool _IsSchemaUpdater = false;
        private CswEnumNbtViewVisibility _newViewVis;
        private Int32 _VisId = Int32.MinValue;
        private bool _targetAlreadyExists;

        public CswNbtActInspectionDesignWiz( CswNbtResources CswNbtResources, CswEnumNbtViewVisibility newViewVis, ICswNbtUser newViewUser, bool isSchemaUpdater )
        {
            _CswNbtResources = CswNbtResources;
            _IsSchemaUpdater = isSchemaUpdater;
            _newViewVis = newViewVis;
            _CurrentUser = newViewUser;

            if( CswEnumNbtViewVisibility.User == _newViewVis && null != _CurrentUser ) _VisId = _CurrentUser.UserId.PrimaryKey;
            if( CswEnumNbtViewVisibility.Role == _newViewVis && null != _CurrentUser ) _VisId = _CurrentUser.RoleId.PrimaryKey;

            if( false == _IsSchemaUpdater && _CswNbtResources.CurrentNbtUser.Rolename != CswNbtObjClassRole.ChemSWAdminRoleName )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Only the ChemSW Admin role can access the Inspection Design wizard.", "Attempted to access the Inspection Design wizard with role of " + _CswNbtResources.CurrentNbtUser.Rolename );
            }

            CultureInfo Culture = Thread.CurrentThread.CurrentCulture;
            _TextInfo = Culture.TextInfo;
        }//ctor
        #endregion ctor

        #region Private

        private const string _SectionName = "section";
        private const string _QuestionName = "question";
        private const string _AllowedAnswersName = "allowed_answers";
        private const string _CompliantAnswersName = "compliant_answers";
        private const string _PreferredAnswer = "preferred_answer";
        private const string _HelpTextName = "help_text";

        private readonly CswCommaDelimitedString _ColumnNames = new CswCommaDelimitedString
                                                           {
                                                               _SectionName,
                                                               _QuestionName,
                                                               _AllowedAnswersName,
                                                               _CompliantAnswersName,
                                                               _PreferredAnswer,
                                                               _HelpTextName
                                                           };

        private const string _DefaultSectionName = "Questions";
        private const string _DefaultAllowedAnswers = "Yes,No,N/A";
        private const string _DefaultCompliantAnswers = "Yes";

        private CswCommaDelimitedString _ProposedNodeTypeNames = new CswCommaDelimitedString();
        private Int32 _DesignNtId = 0;
        private Int32 _TargetNtId = 0;
        private Int32 _GroupNtId = 0;

        #region MetaData

        /// <summary>
        /// Verify that NodeTypeName is Unique in Database and in Session
        /// </summary>
        private void _checkUniqueNodeType( string NodeTypeName )
        {
            string NameToTest = _standardizeName( NodeTypeName );

            if( null != _CswNbtResources.MetaData.getNodeType( NameToTest ) )
            {
                if( _ProposedNodeTypeNames.Contains( NameToTest ) )
                {
                    throw new CswDniException( CswEnumErrorType.Warning, "The provided name is not unique.", "A proposed NodeType with the name " + NameToTest + " already exists in ProposedNodeTypeNames." );
                }
            }
        }

        private string _buildString( string Padding, string StringToAdd )
        {
            string Ret = "";
            if( false == string.IsNullOrEmpty( Padding ) )
            {
                Ret += Padding;
            }
            if( false == string.IsNullOrEmpty( StringToAdd ) )
            {
                Ret += StringToAdd;
            }
            return Ret;
        }

        private string _guaranteeCategoryName( string Category, CswNbtMetaDataNodeType InspectionTargetNt, CswNbtMetaDataNodeType InspectionDesignNt, string InspectionTargetName )
        {
            string CategoryName = Category;
            if( string.IsNullOrEmpty( CategoryName ) )
            {
                if( null != InspectionDesignNt )
                {
                    CategoryName = InspectionDesignNt.Category;
                }

                if( null != InspectionTargetNt )
                {
                    CategoryName += _buildString( ": ", InspectionTargetNt.Category );
                }
                else
                {
                    CategoryName += _buildString( ": ", InspectionTargetName );
                }

            }
            CategoryName = _standardizeName( CategoryName );
            return CategoryName;
        }

        private void _validateNodeType( CswNbtMetaDataNodeType NodeType, CswEnumNbtObjectClass ObjectClass )
        {
            if( null == NodeType )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "The expected object was not defined", "NodeType for ObjectClass " + ObjectClass + " was null." );
            }
            if( ObjectClass != NodeType.getObjectClass().ObjectClass )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot use a " + NodeType.NodeTypeName + " as an " + ObjectClass, "Attempted to use a NodeType of an unexpected ObjectClass" );
            }
        }

        private void _setNodeTypePermissions( CswNbtMetaDataNodeType NodeType )
        {
            _CswNbtResources.Permit.set( CswEnumNbtNodeTypePermission.Create, NodeType, _CurrentUser, true );
            _CswNbtResources.Permit.set( CswEnumNbtNodeTypePermission.Edit, NodeType, _CurrentUser, true );
            _CswNbtResources.Permit.set( CswEnumNbtNodeTypePermission.Delete, NodeType, _CurrentUser, true );
            _CswNbtResources.Permit.set( CswEnumNbtNodeTypePermission.View, NodeType, _CurrentUser, true );
        }

        /// <summary>
        /// Standardize the NodeType Name, check for uniqueness, and add to cached list of new, unique nodetypenames
        /// </summary>
        private string _validateNodeTypeName( object Name, Int32 AllowedLength = Int32.MinValue )
        {
            string RetString = _standardizeName( Name, AllowedLength );
            _checkUniqueNodeType( RetString );
            _ProposedNodeTypeNames.Add( RetString );
            return RetString;
        }

        /// <summary>
        /// Convert the name into Title Case, trim spaces and optionally truncate the name to a specified length
        /// </summary>
        private string _standardizeName( object Name, Int32 AllowedLength = Int32.MinValue )
        {
            string RetString = CswConvert.ToString( Name ).Trim();
            if( 0 < AllowedLength &&
                    AllowedLength < RetString.Length )
            {
                RetString = RetString.Substring( 0, ( AllowedLength - 1 ) );
            }
            return RetString;
        }

        private Dictionary<string, CswNbtMetaDataNodeTypeTab> _getTabsForInspection( JArray Grid, CswNbtMetaDataNodeType NodeType )
        {
            Int32 TabCount = 0;
            Dictionary<string, CswNbtMetaDataNodeTypeTab> RetDict = new Dictionary<string, CswNbtMetaDataNodeTypeTab>();
            for( Int32 Index = 0; Index < Grid.Count; Index += 1 )
            {
                if( Grid[Index].Type == JTokenType.Object )
                {
                    JObject ThisRow = (JObject) Grid[Index];
                    string TabName = _standardizeName( ThisRow[_SectionName] );
                    if( string.IsNullOrEmpty( TabName ) )
                    {
                        TabName = _DefaultSectionName;
                    }
                    if( false == RetDict.ContainsKey( TabName ) )
                    {
                        CswNbtMetaDataNodeTypeTab ThisTab = NodeType.getNodeTypeTab( TabName );
                        if( null == ThisTab )
                        {
                            TabCount += 1;
                            ThisTab = _CswNbtResources.MetaData.makeNewTab( NodeType, TabName, TabCount );
                        }
                        RetDict.Add( TabName, ThisTab );
                    }
                }
            }
            CswNbtMetaDataNodeTypeTab ActionTab = NodeType.getNodeTypeTab( "Action" );
            if( null != ActionTab )
            {
                TabCount += 1;
                //ActionTab.TabOrder = TabCount;
                ActionTab.DesignNode.Order.Value = TabCount;
                ActionTab.DesignNode.postChanges( false );
            }
            CswNbtMetaDataNodeTypeTab DetailsTab = NodeType.getNodeTypeTab( "Details" );
            if( null != DetailsTab )
            {
                //DetailsTab.TabOrder = 0;
                DetailsTab.DesignNode.Order.Value = 0;
                DetailsTab.DesignNode.postChanges( false );
            }
            CswNbtMetaDataNodeTypeTab PictureTab = NodeType.getNodeTypeTab( "Pictures" );
            if( null == PictureTab )
            {
                PictureTab = _CswNbtResources.MetaData.makeNewTab( NodeType, "Pictures" );
                PictureTab.DesignNode.Order.Value = TabCount;
                PictureTab.DesignNode.postChanges( false );
            }
            CswNbtMetaDataNodeTypeProp picturesNTP = NodeType.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Pictures );
            if( null != picturesNTP )
            {
                //picturesNTP.MaxValue = 10;
                picturesNTP.DesignNode.AttributeProperty[CswNbtFieldTypeRuleImage.AttributeName.MaximumValue].AsNumber.Value = 10;
                picturesNTP.DesignNode.postChanges( false );
                _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Edit, NodeType.NodeTypeId, picturesNTP, true, PictureTab.TabId );
            }
            return RetDict;
        }

        private Int32 _createInspectionProps( JArray Grid, CswNbtMetaDataNodeType InspectionDesignNt,
            Dictionary<string, CswNbtMetaDataNodeTypeTab> Tabs, CswCommaDelimitedString GridRowsSkipped )
        {
            Int32 RetCount = 0;
            for( Int32 Index = 0; Index < Grid.Count; Index += 1 )
            {
                if( Grid[Index].Type == JTokenType.Object )
                {
                    JObject ThisRow = (JObject) Grid[Index];
                    string TabName = _standardizeName( ThisRow[_SectionName] );
                    if( string.IsNullOrEmpty( TabName ) )
                    {
                        TabName = _DefaultSectionName;
                    }
                    string Question = CswConvert.ToString( ThisRow[_QuestionName] );
                    string AllowedAnswers = CswConvert.ToString( ThisRow[_AllowedAnswersName] );
                    string CompliantAnswers = CswConvert.ToString( ThisRow[_CompliantAnswersName] );
                    string PreferredAnswer = CswConvert.ToString( ThisRow[_PreferredAnswer] );
                    string HelpText = CswConvert.ToString( ThisRow[_HelpTextName] );

                    if( false == string.IsNullOrEmpty( Question ) )
                    {
                        CswNbtMetaDataNodeTypeTab ThisTab;
                        Tabs.TryGetValue( TabName, out ThisTab );
                        Int32 ThisTabId;
                        if( null != ThisTab )
                        {
                            ThisTabId = ThisTab.TabId;
                        }
                        else
                        {
                            ThisTabId = Tabs[_DefaultSectionName].TabId;
                        }

                        CswNbtMetaDataNodeTypeProp ThisQuestion = InspectionDesignNt.getNodeTypeProp( Question.ToLower() );
                        if( null == ThisQuestion )
                        {
                            ThisQuestion = _CswNbtResources.MetaData.makeNewProp(
                                new CswNbtWcfMetaDataModel.NodeTypeProp( InspectionDesignNt, _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Question ), Question )
                                {
                                    TabId = ThisTabId
                                } );

                            if( null == ThisQuestion )
                            {
                                GridRowsSkipped.Add( Index.ToString() );
                            }
                            else
                            {
                                _validateAnswers( ref CompliantAnswers, ref AllowedAnswers, ref PreferredAnswer );
                                if( false == string.IsNullOrEmpty( HelpText ) )
                                {
                                    //ThisQuestion.HelpText = HelpText;
                                    ThisQuestion.DesignNode.HelpText.Text = HelpText;
                                }
                                //ThisQuestion.ValueOptions = CompliantAnswers;
                                //ThisQuestion.ListOptions = AllowedAnswers;
                                //ThisQuestion.Extended = PreferredAnswer;

                                CswCommaDelimitedString CompliantAnswersCDS = new CswCommaDelimitedString();
                                CompliantAnswersCDS.FromString( CompliantAnswers );

                                ThisQuestion.DesignNode.AttributeProperty[CswNbtFieldTypeRuleQuestion.AttributeName.CompliantAnswers].AsMultiList.Value = CompliantAnswersCDS;
                                ThisQuestion.DesignNode.AttributeProperty[CswNbtFieldTypeRuleQuestion.AttributeName.PossibleAnswers].AsText.Text = AllowedAnswers;
                                ThisQuestion.DesignNode.AttributeProperty[CswNbtFieldTypeRuleQuestion.AttributeName.PreferredAnswer].AsList.Value = PreferredAnswer;
                                ThisQuestion.DesignNode.postOnlyChanges( false );

                                ThisQuestion.removeFromLayout( CswEnumNbtLayoutType.Add );
                                RetCount += 1;
                            }
                        }
                        else
                        {
                            GridRowsSkipped.Add( Index.ToString() );
                        }
                    }
                    else
                    {
                        GridRowsSkipped.Add( Index.ToString() );
                    }
                }
            }
            InspectionDesignNt.DesignNode.RecalculateQuestionNumbers();
            return RetCount;
        }

        private void _confirmInspectionDesignTarget( CswNbtMetaDataNodeType InspectionDesignNt, string InspectionTargetName, ref string Category, out CswNbtMetaDataNodeType InspectionTargetNt, out CswNbtMetaDataNodeType InspectionTargetGroupNt )
        {
            if( string.IsNullOrEmpty( InspectionTargetName ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot generate an Inspection Design without a Target name.", "InspectionTargetName was null or empty." );
            }
            InspectionTargetNt = _CswNbtResources.MetaData.getNodeType( InspectionTargetName );
            Category = _guaranteeCategoryName( Category, InspectionTargetNt, InspectionDesignNt, InspectionTargetName );

            if( null == InspectionTargetNt )
            {
                //This is a New Target
                _createNewInspectionTargetAndGroup( InspectionTargetName, Category, InspectionDesignNt, out InspectionTargetNt, out InspectionTargetGroupNt );
            }
            else
            {
                InspectionTargetGroupNt = _CswNbtResources.MetaData.getNodeType( InspectionTargetName + " Group" );
                _targetAlreadyExists = true;
                _updateInspectionsGridView( InspectionDesignNt, InspectionTargetNt );
            }
            _validateNodeType( InspectionTargetNt, CswEnumNbtObjectClass.InspectionTargetClass );
        }

        private void _createNewInspectionTargetAndGroup( string InspectionTargetName, string Category, CswNbtMetaDataNodeType InspectionDesignNt,
                                                         out CswNbtMetaDataNodeType RetInspectionTargetNt, out CswNbtMetaDataNodeType InspectionTargetGroupNt )
        {
            RetInspectionTargetNt = null;
            if( string.IsNullOrEmpty( InspectionTargetName ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot create Inspection Target without a name.", "InspectionTargetName was null or empty." );
            }
            _validateNodeType( InspectionDesignNt, CswEnumNbtObjectClass.InspectionDesignClass );

            CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
            _validateNodeType( GeneratorNt, CswEnumNbtObjectClass.GeneratorClass );
            _setNodeTypePermissions( GeneratorNt );

            //if we're here, we're validated
            CswNbtMetaDataObjectClass InspectionTargetOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionTargetClass );
            CswNbtMetaDataObjectClass InspectionTargetGroupOc = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionTargetGroupClass );
            //CswNbtMetaDataObjectClass InspectionRouteOc = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClassName.NbtObjectClass.InspectionRouteClass );

            //This will validate names and throw if not unique.
            //Case 24408: In Db, NodeTypeName == varchar(50)
            InspectionTargetName = _validateNodeTypeName( InspectionTargetName, 44 );
            //Create the new NodeTypes
            RetInspectionTargetNt = _CswNbtResources.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( InspectionTargetOc )
                {
                    NodeTypeName = InspectionTargetName,
                    Category = Category
                } );
            _setNodeTypePermissions( RetInspectionTargetNt );

            string InspectionGroupName = _validateNodeTypeName( InspectionTargetName + " Group" );
            InspectionTargetGroupNt = _CswNbtResources.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( InspectionTargetGroupOc )
                {
                    NodeTypeName = InspectionGroupName,
                    Category = Category
                } );
            _GroupNtId = InspectionTargetGroupNt.FirstVersionNodeTypeId;

            _setNodeTypePermissions( InspectionTargetGroupNt );

            #region Set new InspectionTarget Props and Tabs

            {
                //Inspection Target has Inspection Target Group Relationship
                CswNbtMetaDataNodeTypeProp ItInspectionGroupNtp = RetInspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.InspectionTargetGroup );
                //ItInspectionGroupNtp.SetFK( CswEnumNbtViewRelatedIdType.NodeTypeId.ToString(), InspectionTargetGroupNt.NodeTypeId );
                //ItInspectionGroupNtp.PropName = InspectionGroupName;
                CswNbtNodePropMetaDataList TargetProp = ItInspectionGroupNtp.DesignNode.AttributeProperty[CswNbtFieldTypeRuleRelationship.AttributeName.Target].AsMetaDataList;
                TargetProp.clearCachedOptions();
                TargetProp.setValue( CswEnumNbtViewRelatedIdType.NodeTypeId, InspectionTargetGroupNt.NodeTypeId );
                ItInspectionGroupNtp.DesignNode.PropName.Text = InspectionGroupName;
                ItInspectionGroupNtp.DesignNode.postChanges( false );

                //NodeTypeName Template
                CswNbtMetaDataNodeTypeProp ItDescriptionNtp = RetInspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Description );
                //RetInspectionTargetNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( RetInspectionTargetNt.getBarcodeProperty().PropName ) + " " + CswNbtMetaData.MakeTemplateEntry( ItDescriptionNtp.PropName ) );
                RetInspectionTargetNt.DesignNode.NameTemplateText.Text = CswNbtMetaData.MakeTemplateEntry( RetInspectionTargetNt.getBarcodeProperty().PropName ) + " " + CswNbtMetaData.MakeTemplateEntry( ItDescriptionNtp.PropName );
                RetInspectionTargetNt.DesignNode.postChanges( false );
                ItDescriptionNtp.updateLayout( CswEnumNbtLayoutType.Add, ItInspectionGroupNtp, true );

                CswNbtMetaDataNodeTypeProp ItBarcodeNtp = RetInspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Barcode );
                //ItBarcodeNtp.ReadOnly = true; /* Case 25044 */
                ItBarcodeNtp.DesignNode.ReadOnly.Checked = CswEnumTristate.True;
                CswNbtObjClassDesignSequence SequenceNode = CswNbtObjClassDesignSequence.getSequence( _CswNbtResources, "Inspection Barcode" );
                if( null != SequenceNode )
                {
                    ItBarcodeNtp.DesignNode.AttributeProperty[CswNbtFieldTypeRuleBarCode.AttributeName.Sequence].AsRelationship.RelatedNodeId = SequenceNode.NodeId;
                }
                ItBarcodeNtp.updateLayout( CswEnumNbtLayoutType.Add, ItDescriptionNtp, true );
                ItBarcodeNtp.DesignNode.postChanges( false );
            }

            //Inspection Target has a tab to host a grid view of Inspections
            {
                CswNbtMetaDataNodeTypeTab ItInspectionsTab = _CswNbtResources.MetaData.makeNewTab( RetInspectionTargetNt, "Inspections", 2 );

                CswNbtMetaDataNodeTypeProp ItInspectionsNtp = _CswNbtResources.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( RetInspectionTargetNt, _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Grid ), "Inspections" )
                    {
                        TabId = ItInspectionsTab.TabId
                    } );
                CswNbtView ItInspectionsGridView = _createInspectionsGridView( InspectionDesignNt, RetInspectionTargetNt );
                //ItInspectionsNtp.ViewId = ItInspectionsGridView.ViewId;
                ItInspectionsNtp.DesignNode.AttributeProperty[CswNbtFieldTypeRuleGrid.AttributeName.View].AsViewReference.ViewId = ItInspectionsGridView.ViewId;
                ItInspectionsNtp.DesignNode.postChanges( false );
                ItInspectionsNtp.removeFromLayout( CswEnumNbtLayoutType.Add );
            }

            #endregion Set new InspectionTarget Props and Tabs

            #region Set InspectionTargetGroup Props and Tabs

            //NodeTypeName Template
            {
                CswNbtMetaDataNodeTypeProp ItgNameNtp = InspectionTargetGroupNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTargetGroup.PropertyName.Name );

                //InspectionTargetGroupNt.setNameTemplateText( CswNbtMetaData.MakeTemplateEntry( ItgNameNtp.PropName ) );
                InspectionTargetGroupNt.DesignNode.NameTemplateText.Text = CswNbtMetaData.MakeTemplateEntry( ItgNameNtp.PropName );
                InspectionTargetGroupNt.DesignNode.postChanges( false );

                //Description is useful.
                _CswNbtResources.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( InspectionTargetGroupNt, _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Text ), "Description" )
                    {
                        TabId = InspectionTargetGroupNt.getFirstNodeTypeTab().TabId
                    } );

                //Inspection Target Group has a tab to host a grid view of Inspection Targets
                CswNbtMetaDataNodeTypeTab ItgLocationsTab = _CswNbtResources.MetaData.makeNewTab( InspectionTargetGroupNt, InspectionTargetName + " Locations", 3 );
                CswNbtMetaDataNodeTypeProp ItgLocationsNtp = _CswNbtResources.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( InspectionTargetGroupNt, _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Grid ), InspectionTargetName + " Locations" )
                    {
                        TabId = ItgLocationsTab.TabId
                    } );
                CswNbtView ItgInspectionPointsGridView = _createAllInspectionPointsGridView( InspectionTargetGroupNt, RetInspectionTargetNt, string.Empty, CswEnumNbtViewRenderingMode.Grid, InspectionTargetName + " Grid Prop View" );
                //ItgLocationsNtp.ViewId = ItgInspectionPointsGridView.ViewId;
                ItgLocationsNtp.DesignNode.AttributeProperty[CswNbtFieldTypeRuleGrid.AttributeName.View].AsViewReference.ViewId = ItgInspectionPointsGridView.ViewId;
                ItgLocationsNtp.DesignNode.postChanges( false );
                ItgLocationsNtp.removeFromLayout( CswEnumNbtLayoutType.Add );
            }
            {
                CswNbtMetaDataNodeTypeTab ItgSchedulesTab = _CswNbtResources.MetaData.makeNewTab( InspectionTargetGroupNt, InspectionTargetName + " Schedules", 2 );
                CswNbtMetaDataNodeTypeProp ItgSchedulesNtp = _CswNbtResources.MetaData.makeNewProp( new CswNbtWcfMetaDataModel.NodeTypeProp( InspectionTargetGroupNt, _CswNbtResources.MetaData.getFieldType( CswEnumNbtFieldType.Grid ), InspectionTargetName + " Schedules" )
                    {
                        TabId = ItgSchedulesTab.TabId
                    } );

                CswNbtView ItgSchedulesView = new CswNbtView( _CswNbtResources );
                ItgSchedulesView.saveNew( InspectionTargetName + " Schedules", CswEnumNbtViewVisibility.Property );
                ItgSchedulesView.NbtViewMode = CswEnumNbtViewRenderingMode.Grid.ToString();

                CswNbtViewRelationship Rel = ItgSchedulesView.AddViewRelationship( InspectionTargetGroupNt, IncludeDefaultFilters : true );
                CswNbtViewRelationship SchedRel = ItgSchedulesView.AddViewRelationship( Rel, CswEnumNbtViewPropOwnerType.Second, GeneratorNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner ), IncludeDefaultFilters : true );
                ItgSchedulesView.AddViewProperty( SchedRel, GeneratorNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.Description ) );
                ItgSchedulesView.AddViewProperty( SchedRel, GeneratorNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.NextDueDate ) );
                ItgSchedulesView.AddViewProperty( SchedRel, GeneratorNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.RunStatus ) );
                ItgSchedulesView.AddViewProperty( SchedRel, GeneratorNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.RunTime ) );

                ItgSchedulesView.save();
                ItgSchedulesNtp.DesignNode.AttributeProperty[CswEnumNbtPropertyAttributeName.View].AsViewReference.ViewId = ItgSchedulesView.ViewId;
                ItgSchedulesNtp.DesignNode.postChanges( false );
                ItgSchedulesNtp.removeFromLayout( CswEnumNbtLayoutType.Add );
            }

            #endregion Set InspectionTargetGroup Props and Tabs

            _setAuditing( RetInspectionTargetNt );
            _setAuditing( InspectionTargetGroupNt );
        }

        // case 30874 - set auditing enabled by default
        private void _setAuditing( CswNbtMetaDataNodeType NodeType )
        {
            //NodeType.AuditLevel = CswEnumAuditLevel.PlainAudit;
            NodeType.DesignNode.AuditLevel.Value = CswEnumAuditLevel.PlainAudit.ToString();
            NodeType.DesignNode.postOnlyChanges( false );
            foreach( CswNbtMetaDataNodeTypeProp Prop in NodeType.getNodeTypeProps() )
            {
                //Prop.AuditLevel = CswEnumAuditLevel.PlainAudit;
                Prop.DesignNode.AttributeProperty[CswEnumNbtPropertyAttributeName.AuditLevel].AsList.Value = CswEnumAuditLevel.PlainAudit.ToString();
                Prop.DesignNode.postOnlyChanges( false );
            }
        }

        private void _setInspectionDesignTabsAndProps( CswNbtMetaDataNodeType InspectionDesignNt, CswNbtMetaDataNodeType InspectionTargetNt )
        {
            _validateNodeType( InspectionDesignNt, CswEnumNbtObjectClass.InspectionDesignClass );

            _DesignNtId = InspectionDesignNt.FirstVersionNodeTypeId;

            //CswNbtMetaDataNodeTypeProp IdNameNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.NamePropertyName );
            CswNbtMetaDataNodeTypeProp IdNameNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Name );
            //IdNameNtp.updateLayout( CswEnumNbtLayoutType.Add );
            _CswNbtResources.MetaData.NodeTypeLayout.updatePropLayout( CswEnumNbtLayoutType.Add, InspectionDesignNt.NodeTypeId, IdNameNtp, true, Int32.MinValue, Int32.MinValue, Int32.MinValue );
            //NodeTypeName Template
            if( string.IsNullOrEmpty( InspectionDesignNt.NameTemplateValue ) )
            {
                InspectionDesignNt.DesignNode.NameTemplateText.Text = CswNbtMetaData.MakeTemplateEntry( IdNameNtp.PropName );
                InspectionDesignNt.DesignNode.postChanges( false );
            }

            //Inspection Design Target is Inspection Target NT
            CswNbtMetaDataNodeTypeProp IdTargetNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target );
            IdTargetNtp.updateLayout( CswEnumNbtLayoutType.Add, true );
            IdTargetNtp.DesignNode.Required.Checked = CswEnumTristate.True;
            //IdTargetNtp.SetFKDeprecated( CswEnumNbtViewRelatedIdType.NodeTypeId.ToString(), InspectionTargetNt.NodeTypeId );
            CswNbtNodePropMetaDataList TargetTargetProp = IdTargetNtp.DesignNode.AttributeProperty[CswNbtFieldTypeRuleRelationship.AttributeName.Target].AsMetaDataList;
            TargetTargetProp.clearCachedOptions();
            TargetTargetProp.setValue( CswEnumNbtViewRelatedIdType.NodeTypeId, InspectionTargetNt.NodeTypeId );

            CswNbtMetaDataNodeTypeProp ITargetLocationNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Location );
            CswNbtMetaDataNodeTypeProp IDesignLocationNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Location );
            //IDesignLocationNtp.SetFKDeprecated( CswEnumNbtViewPropIdType.NodeTypePropId.ToString(), IdTargetNtp.PropId, CswEnumNbtViewPropIdType.NodeTypePropId.ToString(), ITargetLocationNtp.PropId );
            IDesignLocationNtp.DesignNode.AttributeProperty[CswNbtFieldTypeRulePropertyReference.AttributeName.FKType].AsText.Text = CswEnumNbtViewPropIdType.NodeTypePropId.ToString();
            IDesignLocationNtp.DesignNode.AttributeProperty[CswNbtFieldTypeRulePropertyReference.AttributeName.Relationship].AsList.Value = IdTargetNtp.PropId.ToString();
            IDesignLocationNtp.DesignNode.AttributeProperty[CswNbtFieldTypeRulePropertyReference.AttributeName.RelatedPropType].AsText.Text = CswEnumNbtViewPropIdType.NodeTypePropId.ToString();
            IDesignLocationNtp.DesignNode.AttributeProperty[CswNbtFieldTypeRulePropertyReference.AttributeName.RelatedProperty].AsList.Value = ITargetLocationNtp.PropId.ToString();

            //Inspection Design Generator is SI Inspection Schedule
            CswNbtMetaDataNodeType GeneratorNt = _CswNbtResources.MetaData.getNodeType( CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName );
            _validateInspectionScheduleNt( GeneratorNt );

            CswNbtMetaDataNodeTypeProp IdGeneratorNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Generator );
            if( IdGeneratorNtp.FKType != CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() &&
                IdGeneratorNtp.FKValue != GeneratorNt.NodeTypeId )
            {
                //IdGeneratorNtp.SetFKDeprecated( CswEnumNbtViewRelatedIdType.NodeTypeId.ToString(), GeneratorNt.NodeTypeId );
                CswNbtNodePropMetaDataList GeneratorTargetProp = IdGeneratorNtp.DesignNode.AttributeProperty[CswNbtFieldTypeRuleRelationship.AttributeName.Target].AsMetaDataList;
                GeneratorTargetProp.clearCachedOptions();
                GeneratorTargetProp.setValue( CswEnumNbtViewRelatedIdType.NodeTypeId, GeneratorNt.NodeTypeId );

                IdGeneratorNtp.DesignNode.PropName.Text = CswNbtObjClassGenerator.InspectionGeneratorNodeTypeName;
            }

            CswNbtMetaDataNodeTypeProp IdDueDateNtp = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.DueDate );
            IdDueDateNtp.DesignNode.Required.Checked = CswEnumTristate.True;
            IdDueDateNtp.updateLayout( CswEnumNbtLayoutType.Add, true );
        }

        private void _validateInspectionScheduleNt( CswNbtMetaDataNodeType InspectionScheduleNt )
        {
            _validateNodeType( InspectionScheduleNt, CswEnumNbtObjectClass.GeneratorClass );
            CswNbtMetaDataNodeTypeProp OwnerNtp = InspectionScheduleNt.getNodeTypePropByObjectClassProp( CswNbtObjClassGenerator.PropertyName.Owner );
            CswNbtMetaDataObjectClass GroupOC = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionTargetGroupClass );

            if( OwnerNtp.FKType != CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() || OwnerNtp.FKValue != GroupOC.ObjectClassId )
            {
                //OwnerNtp.SetFKDeprecated( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), GroupOC.ObjectClassId );
                // twice to set the view
                //OwnerNtp.SetFKDeprecated( CswEnumNbtViewRelatedIdType.ObjectClassId.ToString(), GroupOC.ObjectClassId );
                CswNbtNodePropMetaDataList GeneratorTargetProp = OwnerNtp.DesignNode.AttributeProperty[CswNbtFieldTypeRuleRelationship.AttributeName.Target].AsMetaDataList;
                GeneratorTargetProp.clearCachedOptions();
                GeneratorTargetProp.setValue( CswEnumNbtViewRelatedIdType.ObjectClassId, GroupOC.ObjectClassId );
            }

        }

        #endregion MetaData

        #region Views

        private JObject _createInspectionDesignViews( string Category, CswNbtMetaDataNodeType InspectionDesignNt, CswNbtMetaDataNodeType InspectionTargetNt, CswNbtMetaDataNodeType InspectionTargetGroupNt )
        {
            JObject RetObj = new JObject();

            //Inspection Target Group Assignment view
            CswNbtView InspectionTargetGroupAssignmentView = _createInspectionGroupAssignmentView( Category, InspectionTargetNt, InspectionDesignNt, InspectionTargetGroupNt );
            RetObj["viewid"] = InspectionTargetGroupAssignmentView.ViewId.ToString();
            return RetObj;
        }

        private CswNbtView _createInspectionGroupAssignmentView( string Category, CswNbtMetaDataNodeType InspectionTargetNt, CswNbtMetaDataNodeType InspectionDesignNt, CswNbtMetaDataNodeType InspectionTargetGroupNt )
        {
            _validateNodeType( InspectionTargetNt, CswEnumNbtObjectClass.InspectionTargetClass );
            CswNbtMetaDataNodeTypeProp ItTargetGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.InspectionTargetGroup );
            //CswNbtMetaDataNodeType InspectionTargetGroupNt = _CswNbtResources.MetaData.getNodeType( ItTargetGroupNtp.FKValue );
            _validateNodeType( InspectionTargetGroupNt, CswEnumNbtObjectClass.InspectionTargetGroupClass );

            CswNbtView RetView = null;
            string GroupAssignmentViewName = "Groups: " + InspectionTargetNt.NodeTypeName;

            foreach( CswNbtView SchedulingView in _CswNbtResources.ViewSelect.restoreViews( GroupAssignmentViewName, _newViewVis, _VisId ) )
            {
                RetView = SchedulingView;
                break;
            }
            if( _targetAlreadyExists )
            {
                foreach( CswNbtView SchedulingView in _CswNbtResources.ViewSelect.restoreViews( InspectionTargetNt.NodeTypeName, true ) )
                {
                    if( SchedulingView.ViewName.Contains( "Groups" ) && _isVisibleToCurrentUser( SchedulingView ) )
                    {
                        RetView = SchedulingView;
                        break;
                    }
                }
                if( null != RetView )
                {
                    CswNbtView View = RetView;
                    RetView.Root.eachRelationship( Relationship =>
                  {
                      if( Relationship.SecondMetaDataDefinitionObject().UniqueId == InspectionTargetNt.NodeTypeId )
                      {
                          View.AddViewRelationship( Relationship, CswEnumNbtViewPropOwnerType.Second, InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target ), false );
                      }

                  }, null );
                }
            }

            if( null == RetView )
            {
                try
                {
                    RetView = new CswNbtView( _CswNbtResources );
                    if( CswEnumNbtViewVisibility.Global == _newViewVis )
                    {
                        RetView.saveNew( GroupAssignmentViewName, _newViewVis, null, null, null );
                    }
                    else
                    {
                        RetView.saveNew( GroupAssignmentViewName, _newViewVis, _CurrentUser.RoleId, _CurrentUser.UserId, null );
                    }
                    RetView.ViewMode = CswEnumNbtViewRenderingMode.Tree;
                    RetView.Category = Category;

                    /* View:
                     *   [Group]
                     *       [Target]
                     *          [Inspectin Design] 
                    */
                    CswNbtViewRelationship IpGroupRelationship = RetView.AddViewRelationship( InspectionTargetGroupNt, false );
                    CswNbtViewRelationship IpTargetRelationship = RetView.AddViewRelationship( IpGroupRelationship, CswEnumNbtViewPropOwnerType.Second, ItTargetGroupNtp, false );
                    RetView.AddViewRelationship( IpTargetRelationship, CswEnumNbtViewPropOwnerType.Second, InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target ), false );
                }
                catch( Exception ex )
                {
                    throw new CswDniException( CswEnumErrorType.Error, "Failed to create view: " + GroupAssignmentViewName, "View creation failed: " + ex.StackTrace, ex );
                }
            }
            RetView.save();
            RetView.SaveToCache( true );
            return RetView;
        }

        private bool _isVisibleToCurrentUser( CswNbtView View )
        {
            return View.Visibility == CswEnumNbtViewVisibility.Global ||
                     ( View.Visibility == _newViewVis && ( CswTools.IsPrimaryKey( View.VisibilityRoleId ) && View.VisibilityRoleId.PrimaryKey == _VisId ||
                                                           CswTools.IsPrimaryKey( View.VisibilityUserId ) && View.VisibilityUserId.PrimaryKey == _VisId ) );
        }

        private CswNbtView _createInspectionsGridView( CswNbtMetaDataNodeType InspectionDesignNt, CswNbtMetaDataNodeType RetInspectionTargetNt )
        {
            String GridViewName = RetInspectionTargetNt.NodeTypeName + " Inspections Grid Prop View";
            CswNbtView RetView = new CswNbtView( _CswNbtResources );
            try
            {
                RetView.saveNew( GridViewName, CswEnumNbtViewVisibility.Property, null, null, null );
                RetView.ViewMode = CswEnumNbtViewRenderingMode.Grid;
                CswNbtViewRelationship TargetVr = RetView.AddViewRelationship( RetInspectionTargetNt, true );
                CswNbtViewRelationship InspectionVr = RetView.AddViewRelationship( TargetVr, CswEnumNbtViewPropOwnerType.Second, InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target ), true );
                CswNbtViewProperty DueDateVp = RetView.AddViewProperty( InspectionVr, InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.DueDate ) );
                CswNbtViewProperty StatusVp = RetView.AddViewProperty( InspectionVr, InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Status ) );
                RetView.save();
            }
            catch( Exception ex )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Failed to create view: " + GridViewName, "View creation failed", ex );
            }
            return RetView;
        }

        private void _updateInspectionsGridView( CswNbtMetaDataNodeType InspectionDesignNt, CswNbtMetaDataNodeType RetInspectionTargetNt )
        {
            String GridViewName = RetInspectionTargetNt.NodeTypeName + " Inspections Grid Prop View";
            foreach( CswNbtView View in _CswNbtResources.ViewSelect.restoreViews( GridViewName ) )
            {
                CswNbtViewRelationship TargetVr = View.Root.ChildRelationships[0];
                if( null != TargetVr )
                {
                    CswNbtMetaDataNodeTypeProp DesignTargetNTP = InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Target );
                    bool AlreadyExists = TargetVr.ChildRelationships.Any( DesignNTRel => DesignNTRel.PropId == DesignTargetNTP.PropId );
                    if( false == AlreadyExists )
                    {
                        CswNbtViewRelationship InspectionVr = View.AddViewRelationship( TargetVr, CswEnumNbtViewPropOwnerType.Second, DesignTargetNTP, true );
                        CswNbtViewProperty DueDateVp = View.AddViewProperty( InspectionVr, InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.DueDate ) );
                        CswNbtViewProperty StatusVp = View.AddViewProperty( InspectionVr, InspectionDesignNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionDesign.PropertyName.Status ) );
                        View.save();
                        View.SaveToCache( false, true );
                    }
                }
            }
        }

        private CswNbtView _createAllInspectionPointsGridView( CswNbtMetaDataNodeType InspectionGroupNt, CswNbtMetaDataNodeType InspectionTargetNt, string Category, CswEnumNbtViewRenderingMode ViewMode,
             string AllInspectionPointsViewName )
        {
            _validateNodeType( InspectionTargetNt, CswEnumNbtObjectClass.InspectionTargetClass );
            CswNbtView RetView = new CswNbtView( _CswNbtResources );

            try
            {
                RetView.saveNew( AllInspectionPointsViewName, CswEnumNbtViewVisibility.Property, null, null, null );
                RetView.Category = Category;
                RetView.ViewMode = ViewMode;

                CswNbtViewRelationship InspectionGroupVr = RetView.AddViewRelationship( InspectionGroupNt, true );

                CswNbtMetaDataNodeTypeProp InspectionGroupNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.InspectionTargetGroup );
                CswNbtViewRelationship InspectionTargetVr = RetView.AddViewRelationship( InspectionGroupVr, CswEnumNbtViewPropOwnerType.Second, InspectionGroupNtp, true );

                CswNbtMetaDataNodeTypeProp BarcodeNtp = (CswNbtMetaDataNodeTypeProp) InspectionTargetNt.getBarcodeProperty();
                RetView.AddViewProperty( InspectionTargetVr, BarcodeNtp ).Order = 0;

                CswNbtMetaDataNodeTypeProp DescriptionNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Description );
                RetView.AddViewProperty( InspectionTargetVr, DescriptionNtp ).Order = 1;

                CswNbtMetaDataNodeTypeProp LocationNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Location );
                RetView.AddViewProperty( InspectionTargetVr, LocationNtp ).Order = 2;

                //CswNbtMetaDataNodeTypeProp DateNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.LastInspectionDatePropertyName );
                //RetView.AddViewProperty( InspectionTargetVr, DateNtp ).Order = 3;

                CswNbtMetaDataNodeTypeProp StatusNtp = InspectionTargetNt.getNodeTypePropByObjectClassProp( CswNbtObjClassInspectionTarget.PropertyName.Status );
                RetView.AddViewProperty( InspectionTargetVr, StatusNtp ).Order = 4;

                RetView.save();
            }
            catch( Exception ex )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Failed to create view: " + AllInspectionPointsViewName, "View creation failed", ex );
            }
            return RetView;
        }


        #endregion Views

        /// <summary>
        /// Ensure that Allowed Answers contains all Compliant Answers and that both collections contain only unique answers.
        /// </summary>
        private void _validateAnswers( ref string CompliantAnswersString, ref string AllowedAnswersString, ref string PreferredAnswerString )
        {
            string RetCompliantAnswersString = _DefaultCompliantAnswers;
            string RetAllowedAnswersString = _DefaultAllowedAnswers;

            CswCommaDelimitedString AllowedAnswers = new CswCommaDelimitedString();
            AllowedAnswers.FromString( AllowedAnswersString );

            CswCommaDelimitedString CompliantAnswers = new CswCommaDelimitedString();
            CompliantAnswers.FromString( CompliantAnswersString );

            if( false == CompliantAnswers.Contains( PreferredAnswerString, CaseSensitive : false ) )
            {
                PreferredAnswerString = "";
            }

            if( AllowedAnswers.Count > 0 ||
                    CompliantAnswers.Count > 0 )
            {
                Dictionary<string, string> UniqueCompliantAnswers = new Dictionary<string, string>();
                //Get the unique answers from each collection
                foreach( string CompliantAnswer in CompliantAnswers )
                {
                    string ThisAnswer = CompliantAnswer.ToLower().Trim();
                    if( false == string.IsNullOrEmpty( ThisAnswer ) &&
                            false == UniqueCompliantAnswers.ContainsKey( ThisAnswer ) )
                    {
                        UniqueCompliantAnswers.Add( ThisAnswer, CompliantAnswer );
                    }
                }
                Dictionary<string, string> UniqueAllowedAnswers = new Dictionary<string, string>();
                foreach( string AllowedAnswer in AllowedAnswers )
                {
                    string ThisAnswer = AllowedAnswer.ToLower().Trim();
                    if( false == string.IsNullOrEmpty( ThisAnswer ) &&
                            false == UniqueAllowedAnswers.ContainsKey( ThisAnswer ) )
                    {
                        UniqueAllowedAnswers.Add( ThisAnswer, AllowedAnswer );
                    }
                }

                //Allowed answers must contain all compliant answers
                CswCommaDelimitedString RetCompliantAnswers = new CswCommaDelimitedString();
                foreach( KeyValuePair<string, string> UniqueCompliantAnswer in UniqueCompliantAnswers )
                {
                    RetCompliantAnswers.Add( UniqueCompliantAnswer.Value );
                    if( false == UniqueAllowedAnswers.ContainsKey( UniqueCompliantAnswer.Key ) )
                    {
                        UniqueAllowedAnswers.Add( UniqueCompliantAnswer.Key, UniqueCompliantAnswer.Value );
                    }
                }

                //Get unique allowed answers
                CswCommaDelimitedString RetAllowedAnswers = new CswCommaDelimitedString();
                foreach( KeyValuePair<string, string> UniqueAllowedAnswer in UniqueAllowedAnswers )
                {
                    RetAllowedAnswers.Add( UniqueAllowedAnswer.Value );
                }

                if( CompliantAnswers.Count > 0 )
                {
                    RetCompliantAnswersString = RetCompliantAnswers.ToString();
                }
                else //We need at least one compliant answer. If none are provided, then all allowed answers are compliant.
                {
                    RetCompliantAnswersString = RetAllowedAnswers.ToString();
                }

                RetAllowedAnswersString = RetAllowedAnswers.ToString();
            }
            CompliantAnswersString = RetCompliantAnswersString;
            AllowedAnswersString = RetAllowedAnswersString;
        }

        #endregion Private

        #region public

        public Int32 DesignNtId { get { return ( _DesignNtId ); } }
        public Int32 TargetNtId { get { return ( _TargetNtId ); } }
        public Int32 GroupNtId { get { return ( _GroupNtId ); } }

        private CswCommaDelimitedString _UniqueQuestions = new CswCommaDelimitedString();

        public DataTable prepareDataTable( DataTable UploadDataTable )
        {
            DataTable RetDataTable = new DataTable();
            try
            {
                //Normalize the incoming column names
                foreach( DataColumn Column in UploadDataTable.Columns )
                {
                    Column.ColumnName = Column.ColumnName.ToUpper().Replace( " ", "_" );
                }

                //Prep the outgoing column names
                foreach( string ColumnName in _ColumnNames )
                {
                    RetDataTable.Columns.Add( ColumnName );
                }
                RetDataTable.Columns.Add( "RowNumber" );

                Int32 RowNumber = 0;
                foreach( DataRow Row in UploadDataTable.Rows )
                {
                    string Question = _standardizeName( Row[_QuestionName] );
                    if( false == _UniqueQuestions.Contains( Question, CaseSensitive : false ) )
                    {
                        _UniqueQuestions.Add( Question );
                        if( false == string.IsNullOrEmpty( Question ) )
                        {
                            DataRow NewRow = RetDataTable.NewRow();
                            NewRow[_QuestionName] = Question;

                            string AllowedAnswers = CswConvert.ToString( Row[_AllowedAnswersName] );
                            string ComplaintAnswers = CswConvert.ToString( Row[_CompliantAnswersName] );
                            string PreferredAnswer = CswConvert.ToString( Row[_PreferredAnswer] );
                            _validateAnswers( ref ComplaintAnswers, ref AllowedAnswers, ref PreferredAnswer );

                            NewRow[_AllowedAnswersName] = AllowedAnswers;
                            NewRow[_CompliantAnswersName] = ComplaintAnswers;
                            NewRow[_PreferredAnswer] = PreferredAnswer;
                            NewRow[_HelpTextName] = CswConvert.ToString( Row[_HelpTextName] );

                            string SectionName = _standardizeName( Row[_SectionName] );
                            if( string.Empty == SectionName )
                            {
                                SectionName = _DefaultSectionName;
                            }
                            NewRow[_SectionName] = SectionName;
                            NewRow["RowNumber"] = RowNumber;

                            RetDataTable.Rows.Add( NewRow );
                            RowNumber += 1;
                        }
                    }
                }
            }
            catch( Exception Exception )
            {
                _CswNbtResources.CswLogger.reportError( Exception );
            }

            return ( RetDataTable );
        }

        public JObject copyInspectionDesign( string InspectionDesignName, string InspectionTargetName, string Category )
        {
            JObject RetObj = new JObject();
            CswNbtMetaDataNodeType InspectionDesignNt = _CswNbtResources.MetaData.getNodeType( InspectionDesignName );
            if( null != InspectionDesignNt )
            {
                string CopyInspectionNameOrig = CswTools.makeUniqueCopyName( InspectionDesignName, MaxLength : 50 );
                string CopyInspectionNameFinal = CopyInspectionNameOrig;
                Int32 Iterator = 0;
                while( null != _CswNbtResources.MetaData.getNodeType( CopyInspectionNameFinal ) )
                {
                    Iterator += 1;
                    CopyInspectionNameFinal = CopyInspectionNameOrig + " " + Iterator;
                }
                //CswNbtMetaDataNodeType CopiedInspectionDesignNt = _CswNbtResources.MetaData.CopyNodeType( InspectionDesignNt, CopyInspectionNameFinal );
                CswNbtObjClassDesignNodeType CopiedNode = InspectionDesignNt.DesignNode.CopyNode();
                CswNbtMetaDataNodeType CopiedInspectionDesignNt = CopiedNode.RelationalNodeType;
                CswNbtMetaDataNodeType InspectionTargetGroupNt;
                CswNbtMetaDataNodeType InspectionTargetNt;
                _confirmInspectionDesignTarget( CopiedInspectionDesignNt, InspectionTargetName, ref Category, out InspectionTargetNt, out InspectionTargetGroupNt );
                _setInspectionDesignTabsAndProps( CopiedInspectionDesignNt, InspectionTargetNt );
                _TargetNtId = InspectionTargetNt.FirstVersionNodeTypeId;

                RetObj = _createInspectionDesignViews( Category, CopiedInspectionDesignNt, InspectionTargetNt, InspectionTargetGroupNt );
            }
            return RetObj;
        }

        public JObject createInspectionDesignTabsAndProps( string GridArrayString, string InspectionDesignName, string InspectionTargetName, string Category )
        {
            JArray GridArray = JArray.Parse( GridArrayString );
            return ( _createInspectionDesignTabsAndProps( GridArray, InspectionDesignName, InspectionTargetName, Category ) );
        }

        private JObject _createInspectionDesignTabsAndProps( JArray GridArray, string InspectionDesignName, string InspectionTargetName, string Category )
        {
            CswCommaDelimitedString GridRowsSkipped = new CswCommaDelimitedString();

            InspectionDesignName = _validateNodeTypeName( InspectionDesignName );

            if( null == GridArray || GridArray.Count == 0 )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "Cannot create Inspection Design " + InspectionDesignName + ", because the import contained no questions.", "GridArray was null or empty." );
            }

            Int32 TotalRows = GridArray.Count;

            CswNbtMetaDataNodeType InspectionDesignNt = _CswNbtResources.MetaData.makeNewNodeType( new CswNbtWcfMetaDataModel.NodeType( _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.InspectionDesignClass ) )
                {
                    NodeTypeName = InspectionDesignName,
                    Category = string.Empty
                } );
            _setNodeTypePermissions( InspectionDesignNt );

            //Get distinct tabs
            Dictionary<string, CswNbtMetaDataNodeTypeTab> Tabs = _getTabsForInspection( GridArray, InspectionDesignNt );

            //Create the props
            Int32 PropsWithoutError = _createInspectionProps( GridArray, InspectionDesignNt, Tabs, GridRowsSkipped );

            //Build the MetaData
            CswNbtMetaDataNodeType InspectionTargetNt;
            CswNbtMetaDataNodeType InspectionTargetGroupNt;
            _confirmInspectionDesignTarget( InspectionDesignNt, InspectionTargetName, ref Category, out InspectionTargetNt, out InspectionTargetGroupNt );

            _setInspectionDesignTabsAndProps( InspectionDesignNt, InspectionTargetNt );
            _TargetNtId = InspectionTargetNt.FirstVersionNodeTypeId;

            //The Category name is now set
            InspectionDesignNt.DesignNode.Category.Text = Category;
            InspectionDesignNt.DesignNode.postChanges( false );

            //Get the views
            JObject RetObj = _createInspectionDesignViews( Category, InspectionDesignNt, InspectionTargetNt, InspectionTargetGroupNt );

            //More return data
            RetObj["totalrows"] = TotalRows.ToString();
            RetObj["rownumbersskipped"] = new JArray( GridRowsSkipped.ToString() );
            RetObj["countsucceeded"] = PropsWithoutError.ToString();

            _setAuditing( InspectionDesignNt );

            return ( RetObj );
        }

        #endregion public

    }//class CswNbtActInspectionDesignWiz

}//namespace ChemSW.Actions
