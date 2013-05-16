using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Grid.ExtJs;
using ChemSW.Nbt.Grid;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.Security;
using NbtWebApp;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
    public class CswNbtWebServiceView
    {

        public static void HandleStep( ICswResources CswResources, CswNbtViewEditorResponse Return, CswNbtViewEditorData Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;

            if( 2 == Request.StepNo )
            {
                Return.Data.Step2 = new CswNbtViewEditorStep2();
                CswNbtViewId selectedViewId = new CswNbtViewId( Request.ViewId );
                if( null != Request.CurrentView )
                {
                    Request.CurrentView.Root.SetViewRootView( Request.CurrentView );
                    Request.CurrentView.SetResources( NbtResources );
                    _addViewNodeViews( Request.CurrentView );
                    Return.Data.CurrentView = Request.CurrentView;
                }
                else
                {
                    Return.Data.CurrentView = NbtResources.ViewSelect.restoreView( selectedViewId );
                }

                CswNbtView TempView = new CswNbtView( NbtResources );
                foreach( CswNbtMetaDataNodeType NodeType in NbtResources.MetaData.getNodeTypes() )
                {
                    CswNbtViewRelationship Relationship = TempView.AddViewRelationship( NodeType, true );
                    CswNbtViewNode foundNode = Return.Data.CurrentView.FindViewNodeByArbitraryId( Relationship.ArbitraryId );
                    Return.Data.Step2.Relationships.Add( new CswNbtViewEditorRelationship()
                        {
                            Checked = null != foundNode,
                            Relationship = Relationship
                        } );
                    _addNameTemplateProps( TempView, Relationship, NodeType );
                }

                foreach( CswNbtMetaDataObjectClass ObjClass in NbtResources.MetaData.getObjectClasses() )
                {
                    CswNbtViewRelationship Relationship = TempView.AddViewRelationship( ObjClass, true );
                    CswNbtViewNode foundNode = Return.Data.CurrentView.FindViewNodeByArbitraryId( Relationship.ArbitraryId );
                    Return.Data.Step2.Relationships.Add( new CswNbtViewEditorRelationship()
                        {
                            Checked = null != foundNode,
                            Relationship = Relationship
                        } );
                    foreach( CswNbtMetaDataNodeType NodeType in ObjClass.getNodeTypes() )
                    {
                        _addNameTemplateProps( TempView, Relationship, NodeType );
                    }
                }

                foreach( CswNbtMetaDataPropertySet PropSet in NbtResources.MetaData.getPropertySets() )
                {
                    CswNbtViewRelationship Relationship = TempView.AddViewRelationship( PropSet, true );
                    CswNbtViewNode foundNode = Return.Data.CurrentView.FindViewNodeByArbitraryId( Relationship.ArbitraryId );
                    Return.Data.Step2.Relationships.Add( new CswNbtViewEditorRelationship()
                        {
                            Checked = null != foundNode,
                            Relationship = Relationship
                        } );
                    foreach( CswNbtMetaDataObjectClass ObjClass in PropSet.getObjectClasses() )
                    {
                        foreach( CswNbtMetaDataNodeType NodeType in ObjClass.getNodeTypes() )
                        {
                            _addNameTemplateProps( TempView, Relationship, NodeType );
                        }
                    }
                }
            }
            else if( 3 == Request.StepNo )
            {
                Request.CurrentView.Root.SetViewRootView( Request.CurrentView );
                Request.CurrentView.SetResources( NbtResources );
                _addViewNodeViews( Request.CurrentView );

                Return.Data.Step3 = new CswNbtViewEditorStep3();
                Return.Data.CurrentView = Request.CurrentView;

                string ViewStr = Return.Data.CurrentView.ToString();
                CswNbtView TempView = NbtResources.ViewSelect.restoreView( ViewStr );

                HashSet<string> seenProps = new HashSet<string>();

                CswNbtViewRoot.forEachRelationship forEachRelationship = relationship =>
                    {
                        foreach( CswNbtViewProperty ExistingViewProp in relationship.Properties )
                        {
                            seenProps.Add( ExistingViewProp.TextLabel );
                            Return.Data.Step3.Properties.Add( new CswNbtViewEditorProperty()
                                {
                                    Checked = true,
                                    Property = ExistingViewProp
                                } );
                        }

                        _populatePropsCollection( NbtResources, relationship, Return, TempView, seenProps );

                        //Get all props related to this relationship
                        Return.Data.Step3.SecondRelationships = getViewChildRelationshipOptions( NbtResources, TempView, relationship.ArbitraryId );
                        foreach( CswNbtViewRelationship relatedRelationship in Return.Data.Step3.SecondRelationships )
                        {
                            _populatePropsCollection( NbtResources, relatedRelationship, Return, TempView, seenProps, true );
                            relatedRelationship.Properties = new Collection<CswNbtViewProperty>(); //otherwise this has every prop
                        }

                    };
                TempView.Root.eachRelationship( forEachRelationship, null );
            }
            else if( 4 == Request.StepNo )
            {
                Request.CurrentView.Root.SetViewRootView( Request.CurrentView );
                Request.CurrentView.SetResources( NbtResources );
                _addViewNodeViews( Request.CurrentView );

                Return.Data.CurrentView = Request.CurrentView;
                _getFilters( Return, Return.Data.CurrentView );

                CswNbtViewRoot.forEachRelationship eachRelationship = relationship =>
                {
                    Return.Data.Step4.Relationships.Add( relationship );
                };
                Return.Data.CurrentView.Root.eachRelationship( eachRelationship, null );

                Return.Data.Step4.ViewJson = CswConvert.ToString( Return.Data.CurrentView.ToJson() );
            }

        }

        private static void _populatePropsCollection( CswNbtResources NbtResources, CswNbtViewRelationship relationship, CswNbtViewEditorResponse Return, CswNbtView TempView, HashSet<string> seenProps, bool UseMetaName = false )
        {
            if( relationship.SecondType.Equals( CswEnumNbtViewRelatedIdType.NodeTypeId ) )
            {
                CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( relationship.SecondId );
                if( null != NodeType )
                {
                    foreach( CswNbtMetaDataNodeTypeProp ntp in NodeType.getNodeTypeProps() )
                    {
                        CswNbtViewProperty viewProp = TempView.AddViewProperty( relationship, ntp );
                        if( UseMetaName )
                        {
                            viewProp.TextLabel = NodeType.NodeTypeName + "'s " + viewProp.TextLabel;
                        }
                        if( false == seenProps.Contains( viewProp.TextLabel ) )
                        {
                            seenProps.Add( viewProp.TextLabel );
                            Return.Data.Step3.Properties.Add( new CswNbtViewEditorProperty()
                            {
                                Property = viewProp
                            } );
                        }
                    }
                }
            }
            else if( relationship.SecondType.Equals( CswEnumNbtViewRelatedIdType.ObjectClassId ) )
            {
                CswNbtMetaDataObjectClass ObjClass = NbtResources.MetaData.getObjectClass( relationship.SecondId );
                if( null != ObjClass )
                {
                    foreach( CswNbtMetaDataObjectClassProp ocp in ObjClass.getObjectClassProps() )
                    {
                        CswNbtViewProperty viewProp = TempView.AddViewProperty( relationship, ocp );
                        if( UseMetaName )
                        {
                            viewProp.TextLabel = ObjClass.ObjectClass.Value + "'s " + viewProp.TextLabel;
                        }
                        if( false == seenProps.Contains( viewProp.TextLabel ) )
                        {
                            seenProps.Add( viewProp.TextLabel );
                            Return.Data.Step3.Properties.Add( new CswNbtViewEditorProperty()
                            {
                                Property = viewProp
                            } );
                        }
                    }
                }
            }
            else if( relationship.SecondType.Equals( CswEnumNbtViewRelatedIdType.PropertySetId ) )
            {
                CswNbtMetaDataPropertySet PropSet = NbtResources.MetaData.getPropertySet( relationship.SecondId );
                if( null != PropSet )
                {
                    foreach( CswNbtMetaDataObjectClassProp ocp in PropSet.getPropertySetProps() )
                    {
                        CswNbtViewProperty viewProp = TempView.AddViewProperty( relationship, ocp );
                        if( UseMetaName )
                        {
                            viewProp.TextLabel = PropSet.Name.Value + "'s " + viewProp.TextLabel;
                        }
                        if( false == seenProps.Contains( viewProp.TextLabel ) )
                        {
                            seenProps.Add( viewProp.TextLabel );
                            Return.Data.Step3.Properties.Add( new CswNbtViewEditorProperty()
                            {
                                Property = viewProp
                            } );
                        }
                    }
                }
            }
        }

        #region Helpers
        private static Collection<CswNbtViewRelationship> getViewChildRelationshipOptions( CswNbtResources NbtResources, CswNbtView View, string ArbitraryId )
        {
            Collection<CswNbtViewRelationship> ret = new Collection<CswNbtViewRelationship>();

            if( View.ViewId != null )
            {
                CswNbtViewNode SelectedViewNode = View.FindViewNodeByArbitraryId( ArbitraryId );
                if( View.ViewMode != CswEnumNbtViewRenderingMode.Unknown || View.Root.ChildRelationships.Count == 0 )
                {
                    if( SelectedViewNode is CswNbtViewRelationship )
                    {
                        CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) SelectedViewNode;
                        Int32 CurrentLevel = 0;
                        CswNbtViewNode Parent = CurrentRelationship;
                        while( !( Parent is CswNbtViewRoot ) )
                        {
                            CurrentLevel++;
                            Parent = Parent.Parent;
                        }

                        // Child options are all relations to this nodetype
                        Int32 CurrentId = CurrentRelationship.SecondId;

                        Collection<CswNbtViewRelationship> Relationships = null;
                        if( CurrentRelationship.SecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
                        {
                            Relationships = getPropertySetRelated( NbtResources, CurrentId, View, CurrentLevel );
                        }
                        else if( CurrentRelationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                        {
                            Relationships = getObjectClassRelated( NbtResources, CurrentId, View, CurrentLevel );
                        }
                        else if( CurrentRelationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                        {
                            Relationships = getNodeTypeRelated( NbtResources, CurrentId, View, CurrentLevel );
                        }

                        foreach( CswNbtViewRelationship R in from CswNbtViewRelationship _R in Relationships orderby _R.SecondName select _R )
                        {
                            if( !CurrentRelationship.ChildRelationships.Contains( R ) )
                            {
                                R.Parent = CurrentRelationship;
                                ret.Add( R );
                            } //  if( !CurrentRelationship.ChildRelationships.Contains( R ) )
                        } // foreach( CswNbtViewRelationship R in Relationships )

                    }
                }
            }
            return ret;
        } // getViewChildOptions()

        private static Collection<CswNbtViewRelationship> getNodeTypeRelated( CswNbtResources NbtResources, Int32 FirstVersionId, CswNbtView View, Int32 Level )
        {
            Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == CswEnumNbtViewRenderingMode.Grid || View.ViewMode == CswEnumNbtViewRenderingMode.Table ) &&
                            ( View.Visibility != CswEnumNbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataNodeType FirstVersionNodeType = NbtResources.MetaData.getNodeType( FirstVersionId );
            CswNbtMetaDataObjectClass ObjectClass = FirstVersionNodeType.getObjectClass();
            CswNbtMetaDataPropertySet PropertySet = ObjectClass.getPropertySet();

            CswStaticSelect RelationshipPropsSelect = NbtResources.makeCswStaticSelect( "getRelationsForNodeTypeId_select", "getRelationsForNodeTypeId" );
            RelationshipPropsSelect.S4Parameters.Add( "getnodetypeid", new CswStaticParam( "getnodetypeid", FirstVersionNodeType.NodeTypeId ) );
            //RelationshipPropsQueryCaddy.S4Parameters.Add("getroleid", _CswNbtResources.CurrentUser.RoleId);
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                    PropRow["fkvalue"].ToString() != String.Empty )
                {
                    CswNbtMetaDataNodeTypeProp ThisProp = NbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );

                    if( ( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() &&
                          PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
                        ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() &&
                          PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) )
                    {
                        if( NbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, FirstVersionNodeType ) )
                        {
                            // Special case -- relationship to my own type
                            // We need to create two relationships from this

                            CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                            _InsertRelationshipStatic( Relationships, R1 );

                            if( !Restrict )
                            {
                                CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                                _InsertRelationshipStatic( Relationships, R2 );
                            }
                        }
                    }
                    else if( ( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() &&
                               PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
                             ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() &&
                               PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) )
                    {
                        // Special case -- relationship to my own class
                        // We need to create two relationships from this

                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                        R1.overrideFirst( FirstVersionNodeType );
                        R1.overrideSecond( ObjectClass );
                        _InsertRelationshipStatic( Relationships, R1 );

                        if( !Restrict )
                        {
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( FirstVersionNodeType );
                            R2.overrideSecond( ObjectClass );
                            _InsertRelationshipStatic( Relationships, R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() &&
                            PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() )
                        {
                            // my relation to something else
                            R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                            //if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() )
                            //    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            //else
                            //    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            R.overrideSecond( PropRow["fktype"].ToString(), CswConvert.ToInt32( PropRow["fkvalue"] ) );
                            if( R.SecondType != CswEnumNbtViewRelatedIdType.NodeTypeId ||
                                NbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, NbtResources.MetaData.getNodeType( R.SecondId ) ) )
                            {
                                _InsertRelationshipStatic( Relationships, R );
                            }
                        }
                        else if( ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() &&
                                   PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) ||
                                 ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() &&
                                   PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) ||
                                 ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() && PropertySet != null &&
                                   PropRow["fkvalue"].ToString() == PropertySet.PropertySetId.ToString() ) )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me or my object class
                                R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                                if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    R.overrideSecond( NbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                else
                                {
                                    R.overrideSecond( NbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }

                                if( R.SecondType != CswEnumNbtViewRelatedIdType.NodeTypeId ||
                                    NbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, NbtResources.MetaData.getNodeType( R.SecondId ) ) )
                                {
                                    _InsertRelationshipStatic( Relationships, R );
                                }
                            }
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "An unexpected data condition has occurred", "getNodeTypeRelated() found a relationship which did not match the original nodetypeid" );
                        }
                        if( R != null )
                            R.overrideFirst( FirstVersionNodeType );

                    }
                }
            }

            return Relationships;
        }

        private static Collection<CswNbtViewRelationship> getObjectClassRelated( CswNbtResources NbtResources, Int32 ObjectClassId, CswNbtView View, Int32 Level )
        {
            Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == CswEnumNbtViewRenderingMode.Grid || View.ViewMode == CswEnumNbtViewRenderingMode.Table ) &&
                            ( View.Visibility != CswEnumNbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataObjectClass ObjectClass = NbtResources.MetaData.getObjectClass( ObjectClassId );
            CswNbtMetaDataPropertySet PropertySet = ObjectClass.getPropertySet();

            CswStaticSelect RelationshipPropsSelect = NbtResources.makeCswStaticSelect( "getRelationsForObjectClassId_select", "getRelationsForObjectClassId" );
            RelationshipPropsSelect.S4Parameters.Add( "getobjectclassid", new CswStaticParam( "getobjectclassid", ObjectClassId ) );
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                     PropRow["fkvalue"].ToString() != String.Empty )
                {
                    if( ( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() &&
                          PropRow["typeid"].ToString() == ObjectClassId.ToString() ) &&
                        ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() &&
                          PropRow["fkvalue"].ToString() == ObjectClassId.ToString() ) )
                    {
                        CswNbtMetaDataObjectClassProp ThisProp = NbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );

                        // Special case -- relationship to my own class
                        // We need to create two relationships from this
                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                        R1.overrideFirst( ObjectClass );
                        R1.overrideSecond( ObjectClass );
                        _InsertRelationshipStatic( Relationships, R1 );

                        if( !Restrict )
                        {
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( ObjectClass );
                            R2.overrideSecond( ObjectClass );
                            _InsertRelationshipStatic( Relationships, R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() &&
                            PropRow["typeid"].ToString() == ObjectClassId.ToString() )
                        {
                            // my relation to something else
                            CswNbtMetaDataObjectClassProp ThisProp = NbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                            R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                            //if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.PropertySetId.ToString() )
                            //    R.overrideSecond( _CswNbtResources.MetaData.getPropertySet( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            //else if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() )
                            //    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            //else if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.NodeTypeId.ToString() )
                            //    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            R.overrideSecond( PropRow["fktype"].ToString(), CswConvert.ToInt32( PropRow["fkvalue"] ) );
                            R.overrideFirst( ObjectClass );
                            _InsertRelationshipStatic( Relationships, R );
                        }
                        else if( ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() &&
                                   PropRow["fkvalue"].ToString() == ObjectClassId.ToString() ) ||
                                 ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() && PropertySet != null &&
                                   PropRow["fkvalue"].ToString() == PropertySet.PropertySetId.ToString() ) )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me
                                ICswNbtMetaDataProp ThisProp = null;
                                if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    ThisProp = NbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                                }
                                else
                                {
                                    ThisProp = NbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );
                                }
                                R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                                if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    R.overrideSecond( NbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                else
                                {
                                    R.overrideSecond( NbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                R.overrideFirst( ObjectClass );
                                _InsertRelationshipStatic( Relationships, R );
                            }
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "An unexpected data condition has occurred", "getObjectClassRelated() found a relationship which did not match the original objectclassid" );
                        }
                    }
                }
            }

            return Relationships;
        }

        private static Collection<CswNbtViewRelationship> getPropertySetRelated( CswNbtResources NbtResources, Int32 PropertySetId, CswNbtView View, Int32 Level )
        {
            Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == CswEnumNbtViewRenderingMode.Grid || View.ViewMode == CswEnumNbtViewRenderingMode.Table ) &&
                            ( View.Visibility != CswEnumNbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataPropertySet PropertySet = NbtResources.MetaData.getPropertySet( PropertySetId );

            CswStaticSelect RelationshipPropsSelect = NbtResources.makeCswStaticSelect( "getRelationsForPropertySetId_select", "getRelationsForPropertySetId" );
            RelationshipPropsSelect.S4Parameters.Add( "getpropertysetid", new CswStaticParam( "getpropertysetid", PropertySetId ) );
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                     PropRow["fkvalue"].ToString() != String.Empty )
                {
                    ICswNbtMetaDataProp ThisProp = null;
                    if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                    {
                        ThisProp = NbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                    }
                    else if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() )
                    {
                        ThisProp = NbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );
                    }

                    if( PropRow["propertysetid"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() &&
                        PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() &&
                        PropRow["fkvalue"].ToString() == PropertySetId.ToString() )
                    {
                        // Special case -- relationship to my own set
                        // We need to create two relationships from this
                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                        R1.overrideFirst( PropertySet );
                        R1.overrideSecond( PropertySet );
                        _InsertRelationshipStatic( Relationships, R1 );

                        if( !Restrict )
                        {
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( PropertySet );
                            R2.overrideSecond( PropertySet );
                            _InsertRelationshipStatic( Relationships, R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["propertysetid"].ToString() == PropertySetId.ToString() )
                        {
                            // my relation to something else
                            R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                            R.overrideSecond( PropRow["fktype"].ToString(), CswConvert.ToInt32( PropRow["fkvalue"] ) );
                            R.overrideFirst( PropertySet );
                            _InsertRelationshipStatic( Relationships, R );
                        }
                        else if( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() &&
                                 PropRow["fkvalue"].ToString() == PropertySetId.ToString() )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me
                                R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                                if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    R.overrideSecond( NbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                else
                                {
                                    R.overrideSecond( NbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                R.overrideFirst( PropertySet );
                                _InsertRelationshipStatic( Relationships, R );
                            }
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "An unexpected data condition has occurred", "getPropertySetRelated() found a relationship which did not match the original propertysetid" );
                        }
                    }
                }
            }

            return Relationships;
        }

        private static void _InsertRelationshipStatic( Collection<CswNbtViewRelationship> Relationships, CswNbtViewRelationship AddMe )
        {
            Int32 InsertAt = Relationships.Count;
            for( Int32 i = 0; i < Relationships.Count; i++ )
            {
                if( Relationships[i].SecondName.CompareTo( AddMe.SecondName ) > 0 ||
                    ( Relationships[i].SecondName.CompareTo( AddMe.SecondName ) == 0 &&
                      Relationships[i].PropName.CompareTo( AddMe.PropName ) >= 0 ) )
                {
                    InsertAt = i;
                    break;
                }
            }
            Relationships.Insert( InsertAt, AddMe );
        } // _InsertRelationship

        private static ICollection _getObjectClassPropsCollection( CswNbtResources NbtResources, Int32 ObjectClassId )
        {
            // Need to generate all properties on all nodetypes of this object class
            SortedList AllProps = new SortedList();
            CswNbtMetaDataObjectClass ObjectClass = NbtResources.MetaData.getObjectClass( ObjectClassId );
            foreach( CswNbtMetaDataNodeType NodeType in from CswNbtMetaDataNodeType _NodeType in ObjectClass.getNodeTypes() orderby _NodeType.NodeTypeName select _NodeType )
            {
                ICollection NodeTypeProps = _getNodeTypePropsCollection( NbtResources, NodeType.NodeTypeId );
                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in from CswNbtMetaDataNodeTypeProp _NodeTypeProp in NodeTypeProps orderby _NodeTypeProp.PropName select _NodeTypeProp )
                {
                    string ThisKey = NodeTypeProp.PropName.ToLower();
                    if( !AllProps.ContainsKey( ThisKey ) )
                        AllProps.Add( ThisKey, NodeTypeProp );
                }
            }
            return AllProps.Values;
        }

        private static ICollection _getNodeTypePropsCollection( CswNbtResources NbtResources, Int32 NodeTypeId )
        {
            // Need to generate a set of all Props, including latest version props and
            // all historical ones from previous versions that are no longer included in the latest.
            SortedList PropsByName = new SortedList();
            SortedList PropsById = new SortedList();

            CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( NodeTypeId );
            CswNbtMetaDataNodeType ThisVersionNodeType = NbtResources.MetaData.getNodeTypeLatestVersion( NodeType );
            while( ThisVersionNodeType != null )
            {
                foreach( CswNbtMetaDataNodeTypeProp ThisProp in from CswNbtMetaDataNodeTypeProp _ThisProp in ThisVersionNodeType.getNodeTypeProps() orderby _ThisProp.PropName select _ThisProp )
                {
                    if( !PropsByName.ContainsKey( ThisProp.PropNameWithQuestionNo.ToLower() ) &&
                        !PropsById.ContainsKey( ThisProp.FirstPropVersionId ) )
                    {
                        PropsByName.Add( ThisProp.PropNameWithQuestionNo.ToLower(), ThisProp );
                        PropsById.Add( ThisProp.FirstPropVersionId, ThisProp );
                    }
                }
                ThisVersionNodeType = ThisVersionNodeType.getPriorVersionNodeType();
            }
            return PropsByName.Values;
        }

        private static ICollection _getPropertySetPropsCollection( CswNbtResources NbtResources, Int32 PropertySetId )
        {
            // Need to generate all properties on all nodetypes of all object classes of this propertyset
            SortedList AllProps = new SortedList();
            foreach( CswNbtMetaDataObjectClass ObjectClass in NbtResources.MetaData.getObjectClassesByPropertySetId( PropertySetId ) )
            {
                foreach( CswNbtMetaDataNodeType NodeType in from CswNbtMetaDataNodeType _NodeType in ObjectClass.getNodeTypes() orderby _NodeType.NodeTypeName select _NodeType )
                {
                    ICollection NodeTypeProps = _getNodeTypePropsCollection( NbtResources, NodeType.NodeTypeId );
                    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in from CswNbtMetaDataNodeTypeProp _NodeTypeProp in NodeTypeProps orderby _NodeTypeProp.PropName select _NodeTypeProp )
                    {
                        string ThisKey = NodeTypeProp.PropName.ToLower();
                        if( !AllProps.ContainsKey( ThisKey ) )
                        {
                            AllProps.Add( ThisKey, NodeTypeProp );
                        }
                    }
                }
            }
            return AllProps.Values;
        }

        #endregion

        public static void GetFilterProps( ICswResources CswResources, CswNbtViewEditorResponse Return, CswNbtViewEditorFilterData Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Request.CurrentView.Root.SetViewRootView( Request.CurrentView );
            Request.CurrentView.SetResources( NbtResources );
            _addViewNodeViews( Request.CurrentView );

            string viewStr = Request.CurrentView.ToString();
            CswNbtView TempView = new CswNbtView( NbtResources );
            TempView.LoadXml( viewStr );
            HashSet<string> seenProps = new HashSet<string>();

            CswNbtViewRoot.forEachRelationship eachRelationship = Relationship =>
                {
                    if( Relationship.ArbitraryId == ( Request.Relationship.ParentArbitraryId + '_' + Request.Relationship.ArbitraryId ) )
                    {
                        foreach( CswNbtViewProperty viewProp in Relationship.Properties )
                        {
                            seenProps.Add( viewProp.TextLabel );
                            Return.Data.Step4.Properties.Add( viewProp );
                        }

                        if( Relationship.SecondType.Equals( CswEnumNbtViewRelatedIdType.PropertySetId ) )
                        {
                            CswNbtMetaDataPropertySet PropSet = NbtResources.MetaData.getPropertySet( Relationship.SecondId );
                            if( null != PropSet )
                            {
                                foreach( CswNbtMetaDataObjectClass ObjClass in PropSet.getObjectClasses() )
                                {
                                    _getProps( Return, ObjClass, TempView, seenProps, Relationship );
                                }
                            }
                        }
                        else if( Relationship.SecondType.Equals( CswEnumNbtViewRelatedIdType.ObjectClassId ) )
                        {
                            CswNbtMetaDataObjectClass ObjClass = NbtResources.MetaData.getObjectClass( Relationship.SecondId );
                            if( null != ObjClass )
                            {
                                _getProps( Return, ObjClass, TempView, seenProps, Relationship );
                            }
                        }
                        else if( Relationship.SecondType.Equals( CswEnumNbtViewRelatedIdType.NodeTypeId ) )
                        {
                            CswNbtMetaDataNodeType NodeType = NbtResources.MetaData.getNodeType( Relationship.SecondId );
                            if( null != NodeType )
                            {
                                _getProps( Return, NodeType, TempView, seenProps, Relationship );
                            }
                        }
                    }
                };
            TempView.Root.eachRelationship( eachRelationship, null );

            Return.Data.Step4.ViewJson = TempView.ToJson().ToString();
        }

        public static void RemoveFilter( ICswResources CswResources, CswNbtViewEditorResponse Return, CswNbtViewEditorFilterData Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Request.CurrentView.Root.SetViewRootView( Request.CurrentView );
            Request.CurrentView.SetResources( NbtResources );
            _addViewNodeViews( Request.CurrentView );

            Return.Data.CurrentView = Request.CurrentView;
            CswNbtViewProperty prop = (CswNbtViewProperty) Return.Data.CurrentView.FindViewNodeByArbitraryId( Request.FilterToRemove.ParentArbitraryId );
            if( null != prop )
            {
                if( prop.ShowInGrid ) //if ShowInGrid == true, just remove the filter
                {
                    Request.FilterToRemove.Parent = prop; //We don't have a parent when the filter comes from the client, set it here so it can be removed
                    prop.removeFilter( Request.FilterToRemove );
                }
                else //otherwise, remove the property as well
                {
                    ICswNbtMetaDataProp propToRemove;
                    if( prop.Type.Equals( CswEnumNbtViewPropType.ObjectClassPropId ) )
                    {
                        propToRemove = NbtResources.MetaData.getObjectClassProp( prop.ObjectClassPropId );
                    }
                    else
                    {
                        propToRemove = NbtResources.MetaData.getNodeTypeProp( prop.NodeTypePropId );
                    }

                    if( null != propToRemove )
                    {
                        Return.Data.CurrentView.removeViewProperty( propToRemove );
                    }
                }
            }

            _getFilters( Return, Return.Data.CurrentView );

            CswNbtViewRoot.forEachRelationship eachRelationship = relationship =>
            {
                Return.Data.Step4.Relationships.Add( relationship );
            };
            Return.Data.CurrentView.Root.eachRelationship( eachRelationship, null );

            Return.Data.Step4.ViewJson = CswConvert.ToString( Return.Data.CurrentView.ToJson() );
        }

        public static void AddFilter( ICswResources CswResources, CswNbtViewEditorResponse Return, CswNbtViewEditorFilterData Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Request.CurrentView.Root.SetViewRootView( Request.CurrentView );
            Request.CurrentView.SetResources( NbtResources );
            _addViewNodeViews( Request.CurrentView );

            Return.Data.CurrentView = Request.CurrentView;

            CswNbtViewProperty ViewProp = (CswNbtViewProperty) Return.Data.CurrentView.FindViewNodeByArbitraryId( Request.PropArbId );
            if( null != ViewProp )
            {
                Return.Data.CurrentView.AddViewPropertyFilter( ViewProp,
                                                               Conjunction : (CswEnumNbtFilterConjunction) Request.FilterConjunction,
                                                               SubFieldName : (CswEnumNbtSubFieldName) Request.FilterSubfield,
                                                               FilterMode : (CswEnumNbtFilterMode) Request.FilterMode,
                                                               Value : Request.FilterValue );
            }
            else
            {
                ICswNbtMetaDataProp Prop = null;
                if( Request.Property.Type.Equals( CswEnumNbtViewPropType.NodeTypePropId ) )
                {
                    Prop = NbtResources.MetaData.getNodeTypeProp( Request.Property.NodeTypePropId );
                }
                else if( Request.Property.Type.Equals( CswEnumNbtViewPropType.ObjectClassPropId ) )
                {
                    Prop = NbtResources.MetaData.getObjectClassProp( Request.Property.ObjectClassPropId );
                }

                CswNbtViewRelationship parent = (CswNbtViewRelationship) Return.Data.CurrentView.FindViewNodeByArbitraryId( Request.Property.ParentArbitraryId );
                if( null != parent )
                {
                    if( null != Prop )
                    {
                        Return.Data.CurrentView.AddViewPropertyAndFilter( parent, Prop,
                                                                          Value : Request.FilterValue,
                                                                          Conjunction : Request.FilterConjunction,
                                                                          SubFieldName : (CswEnumNbtSubFieldName) Request.FilterSubfield,
                                                                          FilterMode : (CswEnumNbtFilterMode) Request.FilterMode,
                                                                          ShowInGrid : false // the user is filtering on a prop not in the grid, don't show it in the grid
                            );
                    }
                }
            }

            _getFilters( Return, Return.Data.CurrentView );
            CswNbtViewRoot.forEachRelationship eachRelationship = relationship =>
            {
                Return.Data.Step4.Relationships.Add( relationship );
            };
            Return.Data.CurrentView.Root.eachRelationship( eachRelationship, null );

            Return.Data.Step4.ViewJson = CswConvert.ToString( Return.Data.CurrentView.ToJson() );
        }

        public static void AddProp( ICswResources CswResources, CswNbtViewEditorResponse Return, CswNbtViewEditorPropertyData Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Request.CurrentView.Root.SetViewRootView( Request.CurrentView );
            Request.CurrentView.SetResources( NbtResources );
            _addViewNodeViews( Request.CurrentView );

            ICswNbtMetaDataProp prop = null;
            if( Request.Property.Type.Equals( CswEnumNbtViewPropType.NodeTypePropId ) )
            {
                prop = NbtResources.MetaData.getNodeTypeProp( Request.Property.NodeTypePropId );
            }
            else if( Request.Property.Type.Equals( CswEnumNbtViewPropType.ObjectClassPropId ) )
            {
                prop = NbtResources.MetaData.getObjectClassProp( Request.Property.ObjectClassPropId );
            }

            CswNbtViewRelationship parent = (CswNbtViewRelationship) Request.CurrentView.FindViewNodeByArbitraryId( Request.Property.ParentArbitraryId );
            if( null != parent )
            {

                if( null != prop )
                {
                    Request.CurrentView.AddViewProperty( parent, prop );
                }
            }
            else
            {
                parent = (CswNbtViewRelationship) Request.CurrentView.FindViewNodeByArbitraryId( Request.Relationship.ParentArbitraryId );
                if( null != parent )
                {
                    ICswNbtMetaDataProp relProp = null;
                    if( Request.Relationship.PropType.Equals( CswEnumNbtViewPropIdType.NodeTypePropId ) )
                    {
                        relProp = NbtResources.MetaData.getNodeTypeProp( Request.Relationship.PropId );
                    }
                    else if( Request.Relationship.PropType.Equals( CswEnumNbtViewPropIdType.ObjectClassPropId ) )
                    {
                        relProp = NbtResources.MetaData.getObjectClassProp( Request.Relationship.PropId );
                    }
                    CswNbtViewRelationship newParent = Request.CurrentView.AddViewRelationship( parent, Request.Relationship.PropOwner, relProp, true );
                    Request.CurrentView.AddViewProperty( newParent, prop );
                }
            }

            Return.Data.CurrentView = Request.CurrentView;
        }

        public static void RemoveProp( ICswResources CswResources, CswNbtViewEditorResponse Return, CswNbtViewEditorPropertyData Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Request.CurrentView.Root.SetViewRootView( Request.CurrentView );
            Request.CurrentView.SetResources( NbtResources );
            _addViewNodeViews( Request.CurrentView );

            ICswNbtMetaDataProp prop = null;
            if( Request.Property.Type.Equals( CswEnumNbtViewPropType.NodeTypePropId ) )
            {
                prop = NbtResources.MetaData.getNodeTypeProp( Request.Property.NodeTypePropId );
            }
            else if( Request.Property.Type.Equals( CswEnumNbtViewPropType.ObjectClassPropId ) )
            {
                prop = NbtResources.MetaData.getObjectClassProp( Request.Property.ObjectClassPropId );
            }

            if( null != prop )
            {
                Request.CurrentView.removeViewProperty( prop );
                CswNbtViewRelationship parentRel = (CswNbtViewRelationship) Request.CurrentView.FindViewNodeByArbitraryId( Request.Property.ParentArbitraryId );
                if( null != parentRel )
                {
                    if( parentRel.Properties.Count == 0 )
                    {
                        CswNbtViewRelationship parentsParent = (CswNbtViewRelationship) Request.CurrentView.FindViewNodeByArbitraryId( parentRel.ParentArbitraryId );
                        if( null != parentsParent )
                        {
                            parentRel.Parent = parentsParent; //otherwise parentRel.Parent is null
                            parentsParent.removeChildRelationship( parentRel );
                        }
                    }
                }
            }

            Return.Data.CurrentView = Request.CurrentView;
        }

        public static void UpdateViewAttributes( ICswResources CswResources, CswNbtViewEditorResponse Return, CswNbtViewEditorAttributeData Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Request.CurrentView.SetResources( NbtResources );
            Request.CurrentView.Root.SetViewRootView( Request.CurrentView );
            _addViewNodeViews( Request.CurrentView );

            Request.CurrentView.ViewName = Request.NewViewName;
            Request.CurrentView.Visibility = (CswEnumNbtViewVisibility) Request.NewViewVisibility;
            Request.CurrentView.VisibilityRoleId = CswConvert.ToPrimaryKey( Request.NewVisibilityRoleId );
            Request.CurrentView.VisibilityUserId = CswConvert.ToPrimaryKey( Request.NewVisbilityUserId );
            Request.CurrentView.Category = Request.NewViewCategory;
            if( Int32.MinValue != Request.NewViewWidth )
            {
                Request.CurrentView.Width = Request.NewViewWidth;
            }

            Return.Data.CurrentView = Request.CurrentView;
        }

        public static void Finalize( ICswResources CswResources, CswNbtViewEditorResponse Return, CswNbtViewEditorData Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Request.CurrentView.SetResources( NbtResources );
            Request.CurrentView.Root.SetViewRootView( Request.CurrentView );
            _addViewNodeViews( Request.CurrentView );

            Request.CurrentView.save();

            Return.Data.CurrentView = Request.CurrentView;
        }

        private static void _getProps( CswNbtViewEditorResponse Return, CswNbtMetaDataObjectClass ObjClass, CswNbtView TempView, HashSet<string> seenProps, CswNbtViewRelationship Relationship )
        {
            if( null != ObjClass )
            {
                foreach( CswNbtMetaDataNodeType NodeType in ObjClass.getNodeTypes() )
                {
                    _getProps( Return, NodeType, TempView, seenProps, Relationship );
                }
            }
        }

        private static void _getProps( CswNbtViewEditorResponse Return, CswNbtMetaDataNodeType NodeType, CswNbtView TempView, HashSet<string> seenProps, CswNbtViewRelationship Relationship )
        {
            foreach( CswNbtMetaDataNodeTypeProp ntp in NodeType.getNodeTypeProps() )
            {
                CswNbtViewProperty viewProp = TempView.AddViewProperty( Relationship, ntp );
                if( false == seenProps.Contains( viewProp.TextLabel ) )
                {
                    seenProps.Add( viewProp.TextLabel );
                    Return.Data.Step4.Properties.Add( viewProp );
                }
            }
        }

        private static void _getFilters( CswNbtViewEditorResponse Return, CswNbtView View )
        {
            Return.Data.Step4 = new CswNbtViewEditorStep4();

            CswNbtViewRoot.forEachProperty eachProp = property =>
            {
                foreach( CswNbtViewPropertyFilter filter in property.Filters )
                {
                    Return.Data.Step4.Filters.Add( filter );
                }
            };
            View.Root.eachRelationship( null, eachProp );
        }

        private static void _addNameTemplateProps( CswNbtView View, CswNbtViewRelationship Relationship, CswNbtMetaDataNodeType NodeType )
        {
            foreach( string TemplateId in NodeType.NameTemplatePropIds )
            {
                CswNbtMetaDataNodeTypeProp ntp = NodeType.getNodeTypeProp( CswConvert.ToInt32( TemplateId ) );
                if( null != ntp )
                {
                    View.AddViewProperty( Relationship, ntp );
                }
            }
        }

        public static void GetPreview( ICswResources CswResources, CswNbtViewEditorResponse Return, CswNbtViewEditorData Request )
        {
            CswNbtResources NbtResources = (CswNbtResources) CswResources;
            Request.CurrentView.Root.SetViewRootView( Request.CurrentView );
            Request.CurrentView.SetResources( NbtResources );
            _addViewNodeViews( Request.CurrentView );

            CswNbtWebServiceGrid wsGrid = new CswNbtWebServiceGrid( NbtResources, Request.CurrentView, false );
            Return.Data.Preview = wsGrid.runGrid( "Preview", false ).ToString();
        }

        private static void _addViewNodeViews( CswNbtView View )
        {
            CswNbtViewRoot.forEachProperty eachProperty = property =>
                {
                    property.SetViewRootView( View );
                    foreach( CswNbtViewPropertyFilter filter in property.Filters )
                    {
                        filter.Parent = property;
                        filter.SetViewRootView( View );
                    }
                };
            CswNbtViewRoot.forEachRelationship eachRelationship = relationship =>
            {
                if( null == relationship.Parent )
                {
                    relationship.Parent = View.Root;
                }
                relationship.SetViewRootView( View );
                foreach( CswNbtViewRelationship childRel in relationship.ChildRelationships )
                {
                    if( null == childRel.Parent )
                    {
                        childRel.Parent = relationship;
                    }
                }
                foreach( CswNbtViewProperty viewProp in relationship.Properties )
                {
                    if( null == viewProp.Parent )
                    {
                        viewProp.Parent = relationship;
                    }
                }
            };
            View.Root.eachRelationship( eachRelationship, eachProperty );
        }



        private CswNbtResources _CswNbtResources;

        public CswNbtWebServiceView( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
        }

        public JArray getAllViewNames()
        {
            JArray Ret = new JArray();

            bool IsAdmin = _CswNbtResources.CurrentNbtUser.IsAdministrator();
            Dictionary<CswNbtViewId, CswNbtView> AllViews = null;

            if( IsAdmin )
            {
                _CswNbtResources.ViewSelect.getAllEnabledViews( out AllViews );
            }
            else
            {
                _CswNbtResources.ViewSelect.getUserViews( out AllViews );
            }
            if( null != AllViews )
            {
                foreach( KeyValuePair<CswNbtViewId, CswNbtView> ViewPair in AllViews )
                {
                    if( null != ViewPair.Key && ViewPair.Key.isSet() && null != ViewPair.Value )
                    {
                        Ret.Add( new JObject
                            {
                                new JProperty("id", ViewPair.Key.ToString()), 
                                new JProperty("name", ViewPair.Value.ViewName)
                            } );
                    }
                }
            }

            return Ret;
        }

        public JObject getViewGrid( bool All )
        {
            JObject ReturnVal = new JObject();
            CswNbtGrid gd = new CswNbtGrid( _CswNbtResources );
            bool IsAdmin = _CswNbtResources.CurrentNbtUser.IsAdministrator();

            DataTable ViewsTable = null;
            if( IsAdmin )
            {
                if( All )
                {
                    ViewsTable = _CswNbtResources.ViewSelect.getAllEnabledViews();
                }
                else
                {
                    //ViewsTable = (via out)
                    _CswNbtResources.ViewSelect.getVisibleViews( string.Empty, _CswNbtResources.CurrentNbtUser,
                                                                         true, false, false, CswEnumNbtViewRenderingMode.Any,
                                                                         out ViewsTable, ForEdit : true );
                }
            }
            else
            {
                ViewsTable = _CswNbtResources.ViewSelect.getUserViews();
            }

            if( ViewsTable != null )
            {
                ViewsTable.Columns.Add( "viewid" );      // string CswNbtViewId
                foreach( DataRow Row in ViewsTable.Rows )
                {
                    Row["viewid"] = new CswNbtViewId( CswConvert.ToInt32( Row["nodeviewid"] ) ).ToString();
                }

                if( ViewsTable.Columns.Contains( "viewxml" ) )
                    ViewsTable.Columns.Remove( "viewxml" );
                if( ViewsTable.Columns.Contains( "roleid" ) )
                    ViewsTable.Columns.Remove( "roleid" );
                if( ViewsTable.Columns.Contains( "userid" ) )
                    ViewsTable.Columns.Remove( "userid" );
                if( ViewsTable.Columns.Contains( "mssqlorder" ) )
                    ViewsTable.Columns.Remove( "mssqlorder" );

                if( !IsAdmin )
                {
                    if( ViewsTable.Columns.Contains( "visibility" ) )
                        ViewsTable.Columns.Remove( "visibility" );
                    if( ViewsTable.Columns.Contains( "username" ) )
                        ViewsTable.Columns.Remove( "username" );
                    if( ViewsTable.Columns.Contains( "rolename" ) )
                        ViewsTable.Columns.Remove( "rolename" );
                }

                CswExtJsGrid grid = gd.DataTableToGrid( ViewsTable );
                grid.getColumn( "nodeviewid" ).hidden = true;
                grid.getColumn( "viewid" ).hidden = true;
                grid.getColumn( "issystem" ).hidden = true;

                ReturnVal = grid.ToJson();
            } // if(ViewsTable != null)

            return ReturnVal;
        } // getViewGrid()

        public JObject getViewChildOptions( string ViewJson, string ArbitraryId, Int32 StepNo )
        {
            JObject ret = new JObject();

            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.LoadJson( ViewJson );

            if( View.ViewId != null )
            {
                CswNbtViewNode SelectedViewNode = View.FindViewNodeByArbitraryId( ArbitraryId );
                if( View.ViewMode != CswEnumNbtViewRenderingMode.Unknown || View.Root.ChildRelationships.Count == 0 )
                {
                    if( SelectedViewNode is CswNbtViewRelationship )
                    {
                        if( StepNo == 3 && View.ViewMode != CswEnumNbtViewRenderingMode.List )
                        {
                            // Potential child relationships

                            CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) SelectedViewNode;
                            Int32 CurrentLevel = 0;
                            CswNbtViewNode Parent = CurrentRelationship;
                            while( !( Parent is CswNbtViewRoot ) )
                            {
                                CurrentLevel++;
                                Parent = Parent.Parent;
                            }

                            // Child options are all relations to this nodetype
                            Int32 CurrentId = CurrentRelationship.SecondId;

                            Collection<CswNbtViewRelationship> Relationships = null;
                            if( CurrentRelationship.SecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
                            {
                                Relationships = getPropertySetRelated( CurrentId, View, CurrentLevel );
                            }
                            else if( CurrentRelationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                            {
                                Relationships = getObjectClassRelated( CurrentId, View, CurrentLevel );
                            }
                            else if( CurrentRelationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                            {
                                Relationships = getNodeTypeRelated( CurrentId, View, CurrentLevel );
                            }

                            foreach( CswNbtViewRelationship R in from CswNbtViewRelationship _R in Relationships orderby _R.SecondName select _R )
                            {
                                if( !CurrentRelationship.ChildRelationships.Contains( R ) )
                                {
                                    R.Parent = CurrentRelationship;
                                    string Label = String.Empty;

                                    if( R.PropOwner == CswEnumNbtViewPropOwnerType.First )
                                    {
                                        Label = R.SecondName + " (by " + R.PropName + ")";
                                    }
                                    else if( R.PropOwner == CswEnumNbtViewPropOwnerType.Second )
                                    {
                                        Label = R.SecondName + " (by " + R.SecondName + "'s " + R.PropName + ")";
                                    }

                                    JProperty RProp = R.ToJson( Label, true );
                                    if( null == ret[RProp.Name] ) // no dupes
                                    {
                                        ret.Add( RProp );
                                    }

                                } //  if( !CurrentRelationship.ChildRelationships.Contains( R ) )
                            } // foreach( CswNbtViewRelationship R in Relationships )
                        } // if( StepNo == 3)
                        else if( StepNo == 4 )
                        {
                            // Potential child properties

                            CswNbtViewRelationship CurrentRelationship = (CswNbtViewRelationship) SelectedViewNode;

                            ICollection PropsCollection = null;
                            if( CurrentRelationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                            {
                                PropsCollection = _getObjectClassPropsCollection( CurrentRelationship.SecondId );
                            }
                            else if( CurrentRelationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                            {
                                PropsCollection = _getNodeTypePropsCollection( CurrentRelationship.SecondId );
                            }
                            else if( CurrentRelationship.SecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
                            {
                                PropsCollection = _getPropertySetPropsCollection( CurrentRelationship.SecondId );
                            }
                            else
                            {
                                throw new CswDniException( CswEnumErrorType.Error, "A Data Misconfiguration has occurred", "getViewChildOptions() has a relationship type which is not recognized: " + CurrentRelationship.SecondType );
                            }

                            foreach( CswNbtMetaDataNodeTypeProp ThisProp in from CswNbtMetaDataNodeTypeProp _ThisProp in PropsCollection orderby _ThisProp.PropNameWithQuestionNo select _ThisProp )
                            {
                                // BZs 7085, 6651, 6644, 7092
                                if( ThisProp.getFieldTypeRule().SearchAllowed ||
                                    ThisProp.getFieldTypeValue() == CswEnumNbtFieldType.Button )
                                {
                                    CswNbtViewProperty ViewProp = View.AddViewProperty( null, (CswNbtMetaDataNodeTypeProp) ThisProp );
                                    if( !CurrentRelationship.Properties.Contains( ViewProp ) )
                                    {
                                        ViewProp.Parent = CurrentRelationship;

                                        string PropName = ViewProp.MetaDataProp.PropNameWithQuestionNo;
                                        if( false == ThisProp.getNodeType().IsLatestVersion() )
                                            PropName += "&nbsp;(v" + ThisProp.getNodeType().VersionNo + ")";

                                        JProperty PropJProp = ViewProp.ToJson( PropName, true );
                                        ret.Add( PropJProp );

                                    } // if( !CurrentRelationship.Properties.Contains( ViewProp ) )
                                } // if( ThisProp.FieldTypeRule.SearchAllowed )
                            } // foreach (DataRow Row in Props.Rows)
                        } // if-else(StepNo == 4)
                    } // if( SelectedViewNode is CswNbtViewRelationship )
                    else if( SelectedViewNode is CswNbtViewRoot )
                    {
                        // Set NextOptions to be all viewable nodetypes, objectclasses, property sets
                        foreach( CswNbtMetaDataNodeType LatestNodeType in
                                 from CswNbtMetaDataNodeType _LatestNodeType
                                   in _CswNbtResources.MetaData.getNodeTypesLatestVersion()
                                 orderby _LatestNodeType.NodeTypeName
                                 select _LatestNodeType )
                        {
                            if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, LatestNodeType ) )
                            {
                                // This is purposefully not the typical way of creating CswNbtViewRelationships.
                                CswNbtViewRelationship R = new CswNbtViewRelationship( _CswNbtResources, View, LatestNodeType.getFirstVersionNodeType(), false );
                                R.Parent = SelectedViewNode;

                                bool IsChildAlready = false;
                                foreach( CswNbtViewRelationship ChildRel in from CswNbtViewRelationship _ChildRel in ( (CswNbtViewRoot) SelectedViewNode ).ChildRelationships orderby _ChildRel.SecondName select _ChildRel )
                                {
                                    if( ChildRel.SecondType == R.SecondType && ChildRel.SecondId == R.SecondId )
                                    {
                                        IsChildAlready = true;
                                    }
                                }

                                if( !IsChildAlready )
                                {
                                    JProperty RProp = R.ToJson( LatestNodeType.NodeTypeName, true );
                                    ret.Add( RProp );
                                }
                            }
                        }

                        foreach( CswNbtMetaDataObjectClass ObjectClass in
                            from CswNbtMetaDataObjectClass _ObjectClass
                                in _CswNbtResources.MetaData.getObjectClasses()
                            orderby _ObjectClass.ObjectClass
                            where _ObjectClass.ObjectClass != CswNbtResources.UnknownEnum
                            select _ObjectClass )
                        {
                            // This is purposefully not the typical way of creating CswNbtViewRelationships.

                            CswNbtViewRelationship R = new CswNbtViewRelationship( _CswNbtResources, View, ObjectClass, false );
                            R.Parent = SelectedViewNode;

                            if( !( (CswNbtViewRoot) SelectedViewNode ).ChildRelationships.Contains( R ) )
                            {
                                JProperty RProp = R.ToJson( "Any " + ObjectClass.ObjectClass, true );
                                ret.Add( RProp );
                            }
                        }

                        foreach( CswNbtMetaDataPropertySet PropertySet in
                                from CswNbtMetaDataPropertySet _PropertySet
                                    in _CswNbtResources.MetaData.getPropertySets()
                                orderby _PropertySet.Name
                                where _PropertySet.Name != CswNbtResources.UnknownEnum
                                select _PropertySet )
                        {
                            // This is purposefully not the typical way of creating CswNbtViewRelationships.

                            CswNbtViewRelationship R = new CswNbtViewRelationship( _CswNbtResources, View, PropertySet, false );
                            R.Parent = SelectedViewNode;

                            if( !( (CswNbtViewRoot) SelectedViewNode ).ChildRelationships.Contains( R ) )
                            {
                                JProperty RProp = R.ToJson( "Any " + PropertySet.Name, true );
                                ret.Add( RProp );
                            }
                        }

                    } // else if( SelectedViewNode is CswNbtViewRoot )
                    else if( SelectedViewNode is CswNbtViewProperty )
                    {
                        ret.Add( new JProperty( "filters", "" ) );
                    }
                    else if( SelectedViewNode is CswNbtViewPropertyFilter )
                    {
                    }

                } // if( _View.ViewMode != NbtViewRenderingMode.List || _View.Root.ChildRelationships.Count == 0 )
            } // if( _View != null )

            return ret;
        } // getViewChildOptions()

        public JObject getRuntimeViewFilters( CswNbtView View )
        {
            JObject ret = new JObject();
            if( View != null )
            {
                // We need the property arbitrary id, so we're doing this by property, not by filter.  
                // However, we're filtering to only those properties that have filters that have ShowAtRuntime == true
                foreach( CswNbtViewProperty Property in from CswNbtViewProperty _Property
                                                          in View.Root.GetAllChildrenOfType( CswEnumNbtViewNodeType.CswNbtViewProperty )
                                                        where null != _Property.MetaDataProp
                                                        orderby _Property.MetaDataProp.PropNameWithQuestionNo
                                                        select _Property )
                {
                    JProperty PropertyJson = Property.ToJson( ShowAtRuntimeOnly : true );
                    if( ( (JObject) PropertyJson.Value["filters"] ).Count > 0 )
                    {
                        // case 26166 - collapse redundant filters
                        bool foundMatch = false;
                        foreach( JProperty OtherPropertyJson in ret.Properties() )
                        {
                            if( PropertyJson.Value["name"].ToString() == OtherPropertyJson.Value["name"].ToString() &&
                                PropertyJson.Value["fieldtype"].ToString() == OtherPropertyJson.Value["fieldtype"].ToString() )
                            {
                                foundMatch = true;
                            }
                        }
                        if( false == foundMatch )
                        {
                            ret.Add( PropertyJson );
                        }
                    }
                }
            }
            return ret;
        } // getRuntimeViewFilters()

        public JObject updateRuntimeViewFilters( CswNbtView View, JObject NewFiltersJson )
        {
            foreach( JProperty NewFilterProp in NewFiltersJson.Properties() )
            {
                string FilterArbitraryId = NewFilterProp.Name;
                JObject NewFilter = (JObject) NewFilterProp.Value;
                if( NewFilter.Children().Count() > 0 )
                {
                    // case 26166 - apply to all matching properties
                    CswNbtViewPropertyFilter ViewPropFilter = (CswNbtViewPropertyFilter) View.FindViewNodeByArbitraryId( FilterArbitraryId );
                    string OrigValue = ViewPropFilter.Value;

                    CswNbtViewProperty ViewParentProp = (CswNbtViewProperty) ViewPropFilter.Parent;
                    foreach( CswNbtViewPropertyFilter OtherPropFilter in View.Root.GetAllChildrenOfType( CswEnumNbtViewNodeType.CswNbtViewPropertyFilter ) )
                    {
                        CswNbtViewProperty OtherParentProp = ( (CswNbtViewProperty) OtherPropFilter.Parent );
                        if( OtherParentProp.Name == ViewParentProp.Name &&
                            OtherParentProp.FieldType == ViewParentProp.FieldType &&
                            OtherPropFilter.Value == OrigValue )
                        {
                            OtherPropFilter.Conjunction = (CswEnumNbtFilterConjunction) NewFilter["conjunction"].ToString();
                            OtherPropFilter.FilterMode = (CswEnumNbtFilterMode) NewFilter["filter"].ToString();
                            OtherPropFilter.SubfieldName = (CswEnumNbtSubFieldName) NewFilter["subfieldname"].ToString();
                            OtherPropFilter.Value = NewFilter["filtervalue"].ToString();
                        }
                    }
                }
            }

            View.SaveToCache( true, true );

            JObject ret = new JObject();
            ret["newviewid"] = View.SessionViewId.ToString();
            return ret;
        }

        #region Helper Functions

        private Collection<CswNbtViewRelationship> getNodeTypeRelated( Int32 FirstVersionId, CswNbtView View, Int32 Level )
        {
            Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == CswEnumNbtViewRenderingMode.Grid || View.ViewMode == CswEnumNbtViewRenderingMode.Table ) &&
                            ( View.Visibility != CswEnumNbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataNodeType FirstVersionNodeType = _CswNbtResources.MetaData.getNodeType( FirstVersionId );
            CswNbtMetaDataObjectClass ObjectClass = FirstVersionNodeType.getObjectClass();
            CswNbtMetaDataPropertySet PropertySet = ObjectClass.getPropertySet();

            CswStaticSelect RelationshipPropsSelect = _CswNbtResources.makeCswStaticSelect( "getRelationsForNodeTypeId_select", "getRelationsForNodeTypeId" );
            RelationshipPropsSelect.S4Parameters.Add( "getnodetypeid", new CswStaticParam( "getnodetypeid", FirstVersionNodeType.NodeTypeId ) );
            //RelationshipPropsQueryCaddy.S4Parameters.Add("getroleid", _CswNbtResources.CurrentUser.RoleId);
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                    PropRow["fkvalue"].ToString() != String.Empty )
                {
                    CswNbtMetaDataNodeTypeProp ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );

                    if( ( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() &&
                          PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
                        ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() &&
                          PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) )
                    {
                        if( _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, FirstVersionNodeType ) )
                        {
                            // Special case -- relationship to my own type
                            // We need to create two relationships from this

                            CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                            _InsertRelationship( Relationships, R1 );

                            if( !Restrict )
                            {
                                CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                                _InsertRelationship( Relationships, R2 );
                            }
                        }
                    }
                    else if( ( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() &&
                               PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) &&
                             ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() &&
                               PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) )
                    {
                        // Special case -- relationship to my own class
                        // We need to create two relationships from this

                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                        R1.overrideFirst( FirstVersionNodeType );
                        R1.overrideSecond( ObjectClass );
                        _InsertRelationship( Relationships, R1 );

                        if( !Restrict )
                        {
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( FirstVersionNodeType );
                            R2.overrideSecond( ObjectClass );
                            _InsertRelationship( Relationships, R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() &&
                            PropRow["typeid"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() )
                        {
                            // my relation to something else
                            R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                            //if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() )
                            //    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            //else
                            //    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            R.overrideSecond( PropRow["fktype"].ToString(), CswConvert.ToInt32( PropRow["fkvalue"] ) );
                            if( R.SecondType != CswEnumNbtViewRelatedIdType.NodeTypeId ||
                                _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, _CswNbtResources.MetaData.getNodeType( R.SecondId ) ) )
                            {
                                _InsertRelationship( Relationships, R );
                            }
                        }
                        else if( ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.NodeTypeId.ToString() &&
                                   PropRow["fkvalue"].ToString() == FirstVersionNodeType.NodeTypeId.ToString() ) ||
                                 ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() &&
                                   PropRow["fkvalue"].ToString() == ObjectClass.ObjectClassId.ToString() ) ||
                                 ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() && PropertySet != null &&
                                   PropRow["fkvalue"].ToString() == PropertySet.PropertySetId.ToString() ) )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me or my object class
                                R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                                if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                else
                                {
                                    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }

                                if( R.SecondType != CswEnumNbtViewRelatedIdType.NodeTypeId ||
                                    _CswNbtResources.Permit.canNodeType( CswEnumNbtNodeTypePermission.View, _CswNbtResources.MetaData.getNodeType( R.SecondId ) ) )
                                {
                                    _InsertRelationship( Relationships, R );
                                }
                            }
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "An unexpected data condition has occurred", "getNodeTypeRelated() found a relationship which did not match the original nodetypeid" );
                        }
                        if( R != null )
                            R.overrideFirst( FirstVersionNodeType );

                    }
                }
            }

            return Relationships;
        }

        private Collection<CswNbtViewRelationship> getObjectClassRelated( Int32 ObjectClassId, CswNbtView View, Int32 Level )
        {
            Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == CswEnumNbtViewRenderingMode.Grid || View.ViewMode == CswEnumNbtViewRenderingMode.Table ) &&
                            ( View.Visibility != CswEnumNbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
            CswNbtMetaDataPropertySet PropertySet = ObjectClass.getPropertySet();

            CswStaticSelect RelationshipPropsSelect = _CswNbtResources.makeCswStaticSelect( "getRelationsForObjectClassId_select", "getRelationsForObjectClassId" );
            RelationshipPropsSelect.S4Parameters.Add( "getobjectclassid", new CswStaticParam( "getobjectclassid", ObjectClassId ) );
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                     PropRow["fkvalue"].ToString() != String.Empty )
                {
                    if( ( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() &&
                          PropRow["typeid"].ToString() == ObjectClassId.ToString() ) &&
                        ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() &&
                          PropRow["fkvalue"].ToString() == ObjectClassId.ToString() ) )
                    {
                        CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );

                        // Special case -- relationship to my own class
                        // We need to create two relationships from this
                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                        R1.overrideFirst( ObjectClass );
                        R1.overrideSecond( ObjectClass );
                        _InsertRelationship( Relationships, R1 );

                        if( !Restrict )
                        {
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( ObjectClass );
                            R2.overrideSecond( ObjectClass );
                            _InsertRelationship( Relationships, R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() &&
                            PropRow["typeid"].ToString() == ObjectClassId.ToString() )
                        {
                            // my relation to something else
                            CswNbtMetaDataObjectClassProp ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                            R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                            //if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.PropertySetId.ToString() )
                            //    R.overrideSecond( _CswNbtResources.MetaData.getPropertySet( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            //else if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.ObjectClassId.ToString() )
                            //    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            //else if( PropRow["fktype"].ToString() == NbtViewRelatedIdType.NodeTypeId.ToString() )
                            //    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["fkvalue"] ) ) );
                            R.overrideSecond( PropRow["fktype"].ToString(), CswConvert.ToInt32( PropRow["fkvalue"] ) );
                            R.overrideFirst( ObjectClass );
                            _InsertRelationship( Relationships, R );
                        }
                        else if( ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.ObjectClassId.ToString() &&
                                   PropRow["fkvalue"].ToString() == ObjectClassId.ToString() ) ||
                                 ( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() && PropertySet != null &&
                                   PropRow["fkvalue"].ToString() == PropertySet.PropertySetId.ToString() ) )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me
                                ICswNbtMetaDataProp ThisProp = null;
                                if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                                }
                                else
                                {
                                    ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );
                                }
                                R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                                if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                else
                                {
                                    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                R.overrideFirst( ObjectClass );
                                _InsertRelationship( Relationships, R );
                            }
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "An unexpected data condition has occurred", "getObjectClassRelated() found a relationship which did not match the original objectclassid" );
                        }
                    }
                }
            }

            return Relationships;
        }

        private Collection<CswNbtViewRelationship> getPropertySetRelated( Int32 PropertySetId, CswNbtView View, Int32 Level )
        {
            Collection<CswNbtViewRelationship> Relationships = new Collection<CswNbtViewRelationship>();

            // If we're doing a grid, we can only pick things in which the provided nodetype has a relationship to, 
            // rather than things that are related to the provided nodetype.
            // If this is a property grid, then the above rule does not apply to the first level.
            bool Restrict = ( View.ViewMode == CswEnumNbtViewRenderingMode.Grid || View.ViewMode == CswEnumNbtViewRenderingMode.Table ) &&
                            ( View.Visibility != CswEnumNbtViewVisibility.Property || Level >= 2 );

            CswNbtMetaDataPropertySet PropertySet = _CswNbtResources.MetaData.getPropertySet( PropertySetId );

            CswStaticSelect RelationshipPropsSelect = _CswNbtResources.makeCswStaticSelect( "getRelationsForPropertySetId_select", "getRelationsForPropertySetId" );
            RelationshipPropsSelect.S4Parameters.Add( "getpropertysetid", new CswStaticParam( "getpropertysetid", PropertySetId ) );
            DataTable RelationshipPropsTable = RelationshipPropsSelect.getTable();

            foreach( DataRow PropRow in RelationshipPropsTable.Rows )
            {
                // Ignore relationships that don't have a target
                if( PropRow["fktype"].ToString() != String.Empty &&
                     PropRow["fkvalue"].ToString() != String.Empty )
                {
                    ICswNbtMetaDataProp ThisProp = null;
                    if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                    {
                        ThisProp = _CswNbtResources.MetaData.getObjectClassProp( CswConvert.ToInt32( PropRow["propid"] ) );
                    }
                    else if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.NodeTypePropId.ToString() )
                    {
                        ThisProp = _CswNbtResources.MetaData.getNodeTypeProp( CswConvert.ToInt32( PropRow["propid"] ) );
                    }

                    if( PropRow["propertysetid"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() &&
                        PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() &&
                        PropRow["fkvalue"].ToString() == PropertySetId.ToString() )
                    {
                        // Special case -- relationship to my own set
                        // We need to create two relationships from this
                        CswNbtViewRelationship R1 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                        R1.overrideFirst( PropertySet );
                        R1.overrideSecond( PropertySet );
                        _InsertRelationship( Relationships, R1 );

                        if( !Restrict )
                        {
                            CswNbtViewRelationship R2 = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                            R2.overrideFirst( PropertySet );
                            R2.overrideSecond( PropertySet );
                            _InsertRelationship( Relationships, R2 );
                        }
                    }
                    else
                    {
                        CswNbtViewRelationship R = null;
                        if( PropRow["propertysetid"].ToString() == PropertySetId.ToString() )
                        {
                            // my relation to something else
                            R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.First, ThisProp, false );
                            R.overrideSecond( PropRow["fktype"].ToString(), CswConvert.ToInt32( PropRow["fkvalue"] ) );
                            R.overrideFirst( PropertySet );
                            _InsertRelationship( Relationships, R );
                        }
                        else if( PropRow["fktype"].ToString() == CswEnumNbtViewRelatedIdType.PropertySetId.ToString() &&
                                 PropRow["fkvalue"].ToString() == PropertySetId.ToString() )
                        {
                            if( !Restrict )
                            {
                                // something else's relation to me
                                R = View.AddViewRelationship( null, CswEnumNbtViewPropOwnerType.Second, ThisProp, false );
                                if( PropRow["proptype"].ToString() == CswEnumNbtViewPropIdType.ObjectClassPropId.ToString() )
                                {
                                    R.overrideSecond( _CswNbtResources.MetaData.getObjectClass( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                else
                                {
                                    R.overrideSecond( _CswNbtResources.MetaData.getNodeType( CswConvert.ToInt32( PropRow["typeid"] ) ) );
                                }
                                R.overrideFirst( PropertySet );
                                _InsertRelationship( Relationships, R );
                            }
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Error, "An unexpected data condition has occurred", "getPropertySetRelated() found a relationship which did not match the original propertysetid" );
                        }
                    }
                }
            }

            return Relationships;
        }

        private void _InsertRelationship( Collection<CswNbtViewRelationship> Relationships, CswNbtViewRelationship AddMe )
        {
            Int32 InsertAt = Relationships.Count;
            for( Int32 i = 0; i < Relationships.Count; i++ )
            {
                if( Relationships[i].SecondName.CompareTo( AddMe.SecondName ) > 0 ||
                    ( Relationships[i].SecondName.CompareTo( AddMe.SecondName ) == 0 &&
                      Relationships[i].PropName.CompareTo( AddMe.PropName ) >= 0 ) )
                {
                    InsertAt = i;
                    break;
                }
            }
            Relationships.Insert( InsertAt, AddMe );
        } // _InsertRelationship


        private ICollection _getNodeTypePropsCollection( Int32 NodeTypeId )
        {
            // Need to generate a set of all Props, including latest version props and
            // all historical ones from previous versions that are no longer included in the latest.
            SortedList PropsByName = new SortedList();
            SortedList PropsById = new SortedList();

            CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeId );
            CswNbtMetaDataNodeType ThisVersionNodeType = _CswNbtResources.MetaData.getNodeTypeLatestVersion( NodeType );
            while( ThisVersionNodeType != null )
            {
                foreach( CswNbtMetaDataNodeTypeProp ThisProp in from CswNbtMetaDataNodeTypeProp _ThisProp in ThisVersionNodeType.getNodeTypeProps() orderby _ThisProp.PropName select _ThisProp )
                {
                    if( !PropsByName.ContainsKey( ThisProp.PropNameWithQuestionNo.ToLower() ) &&
                        !PropsById.ContainsKey( ThisProp.FirstPropVersionId ) )
                    {
                        PropsByName.Add( ThisProp.PropNameWithQuestionNo.ToLower(), ThisProp );
                        PropsById.Add( ThisProp.FirstPropVersionId, ThisProp );
                    }
                }
                ThisVersionNodeType = ThisVersionNodeType.getPriorVersionNodeType();
            }
            return PropsByName.Values;
        }

        private ICollection _getObjectClassPropsCollection( Int32 ObjectClassId )
        {
            // Need to generate all properties on all nodetypes of this object class
            SortedList AllProps = new SortedList();
            CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( ObjectClassId );
            foreach( CswNbtMetaDataNodeType NodeType in from CswNbtMetaDataNodeType _NodeType in ObjectClass.getNodeTypes() orderby _NodeType.NodeTypeName select _NodeType )
            {
                ICollection NodeTypeProps = _getNodeTypePropsCollection( NodeType.NodeTypeId );
                foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in from CswNbtMetaDataNodeTypeProp _NodeTypeProp in NodeTypeProps orderby _NodeTypeProp.PropName select _NodeTypeProp )
                {
                    string ThisKey = NodeTypeProp.PropName.ToLower();
                    if( !AllProps.ContainsKey( ThisKey ) )
                        AllProps.Add( ThisKey, NodeTypeProp );
                }
            }
            return AllProps.Values;
        }
        private ICollection _getPropertySetPropsCollection( Int32 PropertySetId )
        {
            // Need to generate all properties on all nodetypes of all object classes of this propertyset
            SortedList AllProps = new SortedList();
            foreach( CswNbtMetaDataObjectClass ObjectClass in _CswNbtResources.MetaData.getObjectClassesByPropertySetId( PropertySetId ) )
            {
                foreach( CswNbtMetaDataNodeType NodeType in from CswNbtMetaDataNodeType _NodeType in ObjectClass.getNodeTypes() orderby _NodeType.NodeTypeName select _NodeType )
                {
                    ICollection NodeTypeProps = _getNodeTypePropsCollection( NodeType.NodeTypeId );
                    foreach( CswNbtMetaDataNodeTypeProp NodeTypeProp in from CswNbtMetaDataNodeTypeProp _NodeTypeProp in NodeTypeProps orderby _NodeTypeProp.PropName select _NodeTypeProp )
                    {
                        string ThisKey = NodeTypeProp.PropName.ToLower();
                        if( !AllProps.ContainsKey( ThisKey ) )
                        {
                            AllProps.Add( ThisKey, NodeTypeProp );
                        }
                    }
                }
            }
            return AllProps.Values;
        }

        #endregion Helper Functions

    }

    // class CswNbtWebServiceView

} // namespace ChemSW.Nbt.WebServices
