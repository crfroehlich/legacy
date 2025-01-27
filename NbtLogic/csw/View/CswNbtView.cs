using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt
{
    /// <summary>
    /// Represents an NBT View based on Relationships
    /// </summary>
    [DataContract]
    public class CswNbtView: IEquatable<CswNbtView>
    {
        /// <summary>
        /// CswNbtResources reference
        /// </summary>
        public CswNbtResources _CswNbtResources;

        /// <summary>
        /// Character delimiter used for saving the view as a string
        /// </summary>
        public static char delimiter = '|';

        /// <summary>
        /// Type of View (for this class, it's always RelationshipView)
        /// </summary>
        private static string _ViewType = "RelationshipView";

        /// <summary>
        /// Root View Node
        /// </summary>
        [DataMember]
        public CswNbtViewRoot Root;

        #region DataMembers

        /// <summary>
        /// Name of View
        /// </summary>
        [DataMember]
        public string ViewName
        {
            get { return Root.ViewName; }
            set
            {
                if( null != Root )
                {
                    Root.ViewName = value;
                }
            }
        }

        [DataMember( Name = "SessionViewId" )]
        public string NbtSessionViewId
        {
            get { return SessionViewId.ToString(); }
            set { SessionViewId = new CswNbtSessionDataId( value ); }
        }

        [DataMember( Name = "Visibility" )]
        public string ViewVisibility
        {
            get { return Visibility.ToString(); }
            set { Visibility = (CswEnumNbtViewVisibility) value; }
        }

        /// <summary>
        /// Is Demo View
        /// </summary>
        [DataMember]
        public bool IsDemo
        {
            get { return Root.IsDemo; }
            set
            {
                if( null != Root )
                {
                    Root.IsDemo = value;
                }
            }
        }

        /// <summary>
        /// Is System View
        /// </summary>
        [DataMember]
        public bool IsSystem
        {
            get { return Root.IsSystem; }
            set
            {
                if( null != Root )
                {
                    Root.IsSystem = value;
                }
            }
        }

        /// <summary>
        /// Whether or not Temp Nodes should be included in the view
        /// </summary>
        [DataMember]
        public bool IncludeTempNodes
        {
            get { return Root.IncludeTempNodes; }
            set
            {
                if( null != Root )
                {
                    Root.IncludeTempNodes = value;
                }
            }
        }

        [DataMember( Name = "ViewMode" )]
        public string NbtViewMode
        {
            get { return ViewMode.ToString(); }
            set { ViewMode = (CswEnumNbtViewRenderingMode) value; }
        }

        /// <summary>
        /// Category name (arbitrary string) 
        /// </summary>
        [DataMember]
        public string Category
        {
            get { return Root.Category; }
            set
            {
                if( null != Root )
                {
                    Root.Category = value;
                }
            }
        }

        /// <summary>
        /// Grid Width
        /// </summary>
        [DataMember]
        public Int32 Width
        {
            get { return Root.Width; }
            set
            {
                if( null != Root )
                {
                    Root.Width = value;
                }
            }
        }

        [DataMember( Name = "ViewId" )]
        public string NbtViewId
        {
            get { return ViewId.ToString(); }
            set { ViewId = new CswNbtViewId( value ); }
        }

        [DataMember( Name = "VisibilityUserId" )]
        public string VisibilityUserIdStr
        {
            get
            {
                string ret = "";
                if( null != VisibilityUserId )
                {
                    ret = VisibilityUserId.ToString();
                }
                return ret;
            }
            private set { VisibilityUserId = CswConvert.ToPrimaryKey( value ); }
        }

        [DataMember( Name = "VisibilityRoleId" )]
        public string VisibilityRoleIdStr
        {
            get
            {
                string ret = "";
                if( null != VisibilityRoleId )
                {
                    ret = VisibilityRoleId.ToString();
                }
                return ret;
            }
            private set { VisibilityRoleId = CswConvert.ToPrimaryKey( value ); }
        }

        #endregion DataMembers


        /// <summary>
        /// Icon for View
        /// </summary>
        [DataMember]
        public string IconFileName
        {
            get { return Root.IconFileName; }
            set
            {
                if( null != Root )
                {
                    Root.IconFileName = value;
                }
            }
        }

        /// <summary>
        /// Determines if View should be added to QuickLaunch items
        /// </summary>
        [DataMember]
        public bool IsQuickLaunch
        {
            get
            {
                bool ReturnVal = ( ( ( ViewId != null && ViewId.isSet() ) ||
                                     ( SessionViewId != null && SessionViewId.isSet() ) ) &&
                                   ( Visibility != CswEnumNbtViewVisibility.Property ) &&
                                   ( Visibility != CswEnumNbtViewVisibility.Hidden ) );
                return ReturnVal;
            }
            private set { }
        } // IsQuickLaunch

        /// <summary>
        /// Visibility permission setting
        /// </summary>
        public CswEnumNbtViewVisibility Visibility
        {
            get { return Root.Visibility; }
            set
            {
                if( null != Root )
                {
                    Root.Visibility = value;
                }
            }
        }

        /// <summary>
        /// Visibility permission setting (restrict to user)
        /// </summary>
        public CswPrimaryKey VisibilityRoleId
        {
            get { return Root.VisibilityRoleId; }
            set
            {
                if( null != Root )
                {
                    Root.VisibilityRoleId = value;
                }
            }
        }
        /// <summary>
        /// Visibility permission setting (restrict to role)
        /// </summary>
        public CswPrimaryKey VisibilityUserId
        {
            get { return Root.VisibilityUserId; }
            set
            {
                if( null != Root )
                {
                    Root.VisibilityUserId = value;
                }
            }
        }

        /// <summary>
        /// Group by sibling nodetypes
        /// </summary>
        [DataMember]
        public bool GroupBySiblings
        {
            get { return Root.GroupBySiblings; }
            set
            {
                if( null != Root )
                {
                    Root.GroupBySiblings = value;
                }
            }
        }

        [DataMember]
        public string GridGroupByCol
        {
            get { return Root.GridGroupByCol; }
            set
            {
                if( null != Root )
                {
                    Root.GridGroupByCol = value;
                }
            }
        }

        /// <summary>
        /// Sets the View's Visibility
        /// </summary>
        public void SetVisibility( CswEnumNbtViewVisibility Visibility, CswPrimaryKey RoleId, CswPrimaryKey UserId )
        {
            this.Visibility = Visibility;
            this.VisibilityRoleId = RoleId;
            this.VisibilityUserId = UserId;
        }
        /// <summary>
        /// Rendering Mode
        /// </summary>
        public CswEnumNbtViewRenderingMode ViewMode
        {
            get { return Root.ViewMode; }
            set
            {
                if( null != Root )
                {
                    Root.ViewMode = value;
                }
            }
        }

        /// <summary>
        /// Sets the ViewMode
        /// </summary>
        public void SetViewMode( CswEnumNbtViewRenderingMode value )
        {
            Root.ViewMode = value;
        }

        /// <summary>
        /// Database Primary Key
        /// </summary>
        public CswNbtViewId ViewId
        {
            get
            {
                CswNbtViewId Ret = new CswNbtViewId();
                if( null != Root && null != Root.ViewId )
                {
                    Ret = Root.ViewId;
                }
                return Ret;
            }
            set
            {
                if( null != Root )
                {
                    Root.ViewId = value;
                }
            }
        }

        /// <summary>
        /// True if the View can be used as a search (contains a property filter)
        /// </summary>
        /// <returns></returns>
        public bool IsSearchable()
        {
            bool IsSearchable = false;
            ArrayList PropFilters = Root.GetAllChildrenOfType( CswEnumNbtViewNodeType.CswNbtViewPropertyFilter );
            IsSearchable = ( IsFullyEnabled() && PropFilters.Cast<CswNbtViewPropertyFilter>().Any( Filter => Filter.SubfieldName != CswEnumNbtSubFieldName.NodeID ) );
            return IsSearchable;
        }

        /// <summary>
        /// True if the View is visible by the current user
        /// </summary>
        /// <returns></returns>
        public bool IsVisible()
        {
            return _CswNbtResources.ViewSelect.isVisible( this, _CswNbtResources.CurrentNbtUser, true, false );
        }

        /// <summary>
        /// Constructor - empty view
        /// </summary>
        public CswNbtView( CswNbtResources CswNbtResources )
        {
            _CswNbtResources = CswNbtResources;
            Clear();
        }

        /// <summary>
        /// Dummy Constructor for Wcf
        /// </summary>
        public CswNbtView() { }

        #region Child constructors

        /// <summary>
        /// Creates a new <see cref="CswNbtViewRelationship"/> for this view.
        /// For copying an existing relationship
        /// </summary>
        public CswNbtViewRelationship AddViewRelationship( CswEnumNbtObjectClass NbtObjectClass, bool IncludeDefaultFilters, out CswNbtMetaDataObjectClass ObjectClass )
        {
            if( NbtObjectClass == CswNbtResources.UnknownEnum )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Cannot create an view relationship if the object class is unknown.", "Attempted to call AddViewRelationship with an Unknown Object Class." );
            }
            ObjectClass = _CswNbtResources.MetaData.getObjectClass( NbtObjectClass );
            return AddViewRelationship( ObjectClass, IncludeDefaultFilters );
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewRelationship"/> for this view.
        /// For copying an existing relationship
        /// </summary>
        public CswNbtViewRelationship CopyViewRelationship( CswNbtViewNode ParentViewNode, CswNbtViewRelationship CopyFrom, bool IncludeDefaultFilters )
        {
            CswNbtViewRelationship NewRelationship = new CswNbtViewRelationship( _CswNbtResources, this, CopyFrom, IncludeDefaultFilters );
            if( ParentViewNode != null )
                ParentViewNode.AddChild( NewRelationship );
            return NewRelationship;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewRelationship"/> for this view.
        /// For adding a nodetype to the root level of the view.
        /// </summary>
        public CswNbtViewRelationship AddViewRelationship( CswNbtMetaDataNodeType NodeType, bool IncludeDefaultFilters )
        {
            CswNbtViewRelationship NewRelationship = new CswNbtViewRelationship( _CswNbtResources, this, NodeType, IncludeDefaultFilters );
            if( this.Root != null )
                this.Root.AddChild( NewRelationship );
            return NewRelationship;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewRelationship"/> for this view.
        /// For adding an object class to the root level of the view
        /// </summary>
        public CswNbtViewRelationship AddViewRelationship( CswNbtMetaDataObjectClass ObjectClass, bool IncludeDefaultFilters )
        {
            CswNbtViewRelationship NewRelationship = new CswNbtViewRelationship( _CswNbtResources, this, ObjectClass, IncludeDefaultFilters );
            if( this.Root != null )
                this.Root.AddChild( NewRelationship );
            return NewRelationship;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewRelationship"/> for this view.
        /// For adding a property set to the root level of the view
        /// </summary>
        public CswNbtViewRelationship AddViewRelationship( CswNbtMetaDataPropertySet PropertySet, bool IncludeDefaultFilters )
        {
            CswNbtViewRelationship NewRelationship = new CswNbtViewRelationship( _CswNbtResources, this, PropertySet, IncludeDefaultFilters );
            if( this.Root != null )
                this.Root.AddChild( NewRelationship );
            return NewRelationship;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewRelationship"/> for this view.
        /// For a relationship below the root level, determined by a property
        /// </summary>
        public CswNbtViewRelationship AddViewRelationship( CswNbtViewRelationship ParentViewRelationship, CswEnumNbtViewPropOwnerType OwnerType, ICswNbtMetaDataProp Prop, bool IncludeDefaultFilters )
        {
            CswNbtViewRelationship NewRelationship = new CswNbtViewRelationship( _CswNbtResources, this, OwnerType, Prop, IncludeDefaultFilters );
            if( ParentViewRelationship != null )
                ParentViewRelationship.addChildRelationship( NewRelationship );
            return NewRelationship;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewProperty"/> for this view
        /// </summary>
        public CswNbtViewProperty AddViewProperty( CswNbtViewRelationship ParentViewRelationship, ICswNbtMetaDataProp Prop, Int32 Order = Int32.MinValue )
        {
            CswNbtViewProperty NewProperty = new CswNbtViewProperty( _CswNbtResources, this, Prop );
            if( ParentViewRelationship != null )
                ParentViewRelationship.addProperty( NewProperty );
            if( Int32.MinValue != Order )
            {
                NewProperty.Order = Order;
            }
            return NewProperty;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewProperty"/> for this view by property name
        /// </summary>
        public CswNbtViewProperty AddViewPropertyByName( CswNbtViewRelationship ParentViewRelationship, CswNbtMetaDataObjectClass ObjectClass, string PropertyName )
        {
            CswNbtMetaDataObjectClassProp ObjectClassProp = ObjectClass.getObjectClassProp( PropertyName );
            if( null != ObjectClassProp )
            {
                return AddViewProperty( ParentViewRelationship, ObjectClassProp );
            }

            return ( from NodeType in ObjectClass.getLatestVersionNodeTypes()
                     select NodeType.getNodeTypeProp( PropertyName )
                         into NodeTypeProp
                         where null != NodeTypeProp
                         select AddViewProperty( ParentViewRelationship, NodeTypeProp ) ).FirstOrDefault();
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewProperty"/> for this view for unique FieldTypes (Barcode/Location)
        /// </summary>
        public CswNbtViewProperty AddViewPropertyByFieldType( CswNbtViewRelationship ParentViewRelationship, ICswNbtMetaDataObject MetaDataObject, CswEnumNbtFieldType FieldType )
        {
            if( MetaDataObject.UniqueIdFieldName == CswNbtMetaDataObjectClass.MetaDataUniqueType )
            {
                CswNbtMetaDataObjectClass ObjectClass = (CswNbtMetaDataObjectClass) MetaDataObject;



                foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.getNodeTypes() )
                {
                    AddViewPropertyByFieldType( ParentViewRelationship, NodeType, FieldType );
                }
            }
            else if( MetaDataObject.UniqueIdFieldName == CswNbtMetaDataNodeType.MetaDataUniqueType )
            {
                CswNbtMetaDataNodeType NodeType = (CswNbtMetaDataNodeType) MetaDataObject;
                switch( FieldType )
                {
                    case CswEnumNbtFieldType.Barcode:
                        CswNbtMetaDataNodeTypeProp BarcodeNtp = (CswNbtMetaDataNodeTypeProp) NodeType.getBarcodeProperty();
                        if( null != BarcodeNtp )
                        {
                            return AddViewProperty( ParentViewRelationship, BarcodeNtp );
                        }
                        break;
                    case CswEnumNbtFieldType.Button:
                        foreach( CswNbtMetaDataNodeTypeProp ButtonNtp in NodeType.getButtonProperties() )
                        {
                            AddViewProperty( ParentViewRelationship, ButtonNtp );
                        }
                        break;
                    case CswEnumNbtFieldType.Location:
                        CswNbtMetaDataNodeTypeProp LocationNtp = NodeType.getLocationProperty();
                        if( null != LocationNtp )
                        {
                            return AddViewProperty( ParentViewRelationship, LocationNtp );
                        }
                        break;
                    default:
                        throw new CswDniException( CswEnumErrorType.Error,
                                                  "Cannot add a View Property without a Location or Barcode property.",
                                                  "Attempted to call AddViewPropertyByFieldType() with an unsupported FieldType." );
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewPropertyFilter"/> for this view
        /// </summary>
        public CswNbtViewPropertyFilter AddViewPropertyFilter( CswNbtViewProperty ParentViewProperty,
                                                               CswEnumNbtSubFieldName SubFieldName = null,
                                                               CswEnumNbtFilterMode FilterMode = null,
                                                               string Value = "",
                                                               bool CaseSensitive = false,
                                                               bool ShowAtRuntime = false )
        {
            return AddViewPropertyFilter( ParentViewProperty, CswEnumNbtFilterConjunction.And, CswEnumNbtFilterResultMode.Hide, SubFieldName, FilterMode, Value, CaseSensitive, ShowAtRuntime );
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewPropertyFilter"/> for this view
        /// </summary>
        public CswNbtViewPropertyFilter AddViewPropertyFilter( CswNbtViewProperty ParentViewProperty,
                                                               CswEnumNbtFilterConjunction Conjunction,
                                                               CswEnumNbtSubFieldName SubFieldName = null,
                                                               CswEnumNbtFilterMode FilterMode = null,
                                                               string Value = "",
                                                               bool CaseSensitive = false,
                                                               bool ShowAtRuntime = false )
        {
            return AddViewPropertyFilter( ParentViewProperty, Conjunction, CswEnumNbtFilterResultMode.Hide, SubFieldName, FilterMode, Value, CaseSensitive, ShowAtRuntime );
        }

        public CswNbtViewPropertyFilter AddViewPropertyFilter( CswNbtViewProperty ParentViewProperty,
                                                               CswEnumNbtFilterConjunction Conjunction,
                                                               CswEnumNbtFilterResultMode ResultMode,
                                                               CswEnumNbtSubFieldName SubFieldName = null,
                                                               CswEnumNbtFilterMode FilterMode = null,
                                                               string Value = "",
                                                               bool CaseSensitive = false,
                                                               bool ShowAtRuntime = false )
        {
            SubFieldName = SubFieldName ?? ParentViewProperty.MetaDataProp.getFieldTypeRule().SubFields.Default.Name;
            FilterMode = FilterMode ?? CswEnumNbtFilterMode.Equals;
            CswNbtViewPropertyFilter NewFilter = new CswNbtViewPropertyFilter( _CswNbtResources, this, SubFieldName, FilterMode, Value, ResultMode, Conjunction, CaseSensitive, ShowAtRuntime );
            if( ParentViewProperty != null )
            {
                ParentViewProperty.addFilter( NewFilter );
            }
            return NewFilter;
        }

        /// <summary>
        /// Creates a new <see cref="CswNbtViewPropertyFilter"/> for this view
        /// </summary>
        public CswNbtViewPropertyFilter AddViewPropertyAndFilter( CswNbtViewRelationship ParentViewRelationship,
                                                                  ICswNbtMetaDataProp MetaDataProp,
                                                                  string Value = "",
                                                                  CswEnumNbtSubFieldName SubFieldName = null,
                                                                  bool CaseSensitive = false,
                                                                  CswEnumNbtFilterMode FilterMode = null,
                                                                  bool ShowAtRuntime = false,
                                                                  bool ShowInGrid = true )
        {
            return AddViewPropertyAndFilter( ParentViewRelationship, MetaDataProp, CswEnumNbtFilterConjunction.And, CswEnumNbtFilterResultMode.Hide, Value, SubFieldName, CaseSensitive, FilterMode, ShowAtRuntime, ShowInGrid );
        }
        /// <summary>
        /// Creates a new <see cref="CswNbtViewPropertyFilter"/> for this view
        /// </summary>
        public CswNbtViewPropertyFilter AddViewPropertyAndFilter( CswNbtViewRelationship ParentViewRelationship,
                                                                  ICswNbtMetaDataProp MetaDataProp,
                                                                  CswEnumNbtFilterConjunction Conjunction,
                                                                  string Value = "",
                                                                  CswEnumNbtSubFieldName SubFieldName = null,
                                                                  bool CaseSensitive = false,
                                                                  CswEnumNbtFilterMode FilterMode = null,
                                                                  bool ShowAtRuntime = false,
                                                                  bool ShowInGrid = true )
        {
            return AddViewPropertyAndFilter( ParentViewRelationship, MetaDataProp, Conjunction, CswEnumNbtFilterResultMode.Hide, Value, SubFieldName, CaseSensitive, FilterMode, ShowAtRuntime, ShowInGrid );
        }
        /// <summary>
        /// Creates a new <see cref="CswNbtViewPropertyFilter"/> for this view
        /// </summary>
        public CswNbtViewPropertyFilter AddViewPropertyAndFilter( CswNbtViewRelationship ParentViewRelationship,
                                                                  ICswNbtMetaDataProp MetaDataProp,
                                                                  CswEnumNbtFilterConjunction Conjunction,
                                                                  CswEnumNbtFilterResultMode ResultMode,
                                                                  string Value = "",
                                                                  CswEnumNbtSubFieldName SubFieldName = null,
                                                                  bool CaseSensitive = false,
                                                                  CswEnumNbtFilterMode FilterMode = null,
                                                                  bool ShowAtRuntime = false,
                                                                  bool ShowInGrid = true )
        {
            CswNbtViewPropertyFilter NewFilter = null;
            if( null != ParentViewRelationship && null != MetaDataProp )
            {
                //FilterMode = FilterMode ?? CswEnumNbtFilterMode.Equals;
                //SubFieldName = SubFieldName ?? MetaDataProp.getFieldTypeRule().SubFields.Default.Name;
                CswNbtViewProperty ViewProp = AddViewProperty( ParentViewRelationship, MetaDataProp );
                ViewProp.ShowInGrid = ShowInGrid;
                NewFilter = AddViewPropertyFilter( ViewProp, Conjunction, ResultMode, SubFieldName, FilterMode, Value, CaseSensitive, ShowAtRuntime );
            }
            return NewFilter;
        }

        private void _removeViewPropertyRecursive( ICswNbtMetaDataProp MetaDataProp, IEnumerable<CswNbtViewRelationship> Relationships )
        {
            Collection<CswNbtViewProperty> DoomedProps = new Collection<CswNbtViewProperty>();
            foreach( CswNbtViewRelationship ChildRelationship in Relationships )
            {
                foreach( CswNbtViewProperty Property in ChildRelationship.Properties )
                {
                    if( null != Property.MetaDataProp && Property.MetaDataProp.FirstPropVersionId == MetaDataProp.FirstPropVersionId )
                    {
                        DoomedProps.Add( Property );
                    }
                }
                if( ChildRelationship.ChildRelationships.Count > 0 )
                {
                    _removeViewPropertyRecursive( MetaDataProp, ChildRelationship.ChildRelationships );
                }
                foreach( CswNbtViewProperty DoomedProp in DoomedProps )
                {
                    ChildRelationship.removeProperty( DoomedProp );
                }
            }
        }

        /// <summary>
        /// Removes a <see cref="CswNbtViewProperty"/> by matching MetaDataProp from every relationship of this view.
        /// </summary>
        public void removeViewProperty( ICswNbtMetaDataProp MetaDataProp )
        {
            if( null != MetaDataProp )
            {
                _removeViewPropertyRecursive( MetaDataProp, Root.ChildRelationships );
            }
        }

        #endregion Child constructors

        [DataMember]
        private Int32 _lastUniqueId = 0;
        /// <summary>
        /// This allows child View nodes to fetch an ID which is guaranteed unique within a view
        /// </summary>
        public string GenerateUniqueId()
        {
            _lastUniqueId++;
            return _lastUniqueId.ToString();
        }

        /// <summary>
        /// load view by view name
        /// </summary>
        /// <param name="ViewName">viewname in node_views table</param>
        /// <returns></returns>
        public bool LoadView( string ViewName )
        {
            bool ReturnVal = false;

            CswTableSelect NodeViewsTableSelect = _CswNbtResources.makeCswTableSelect( "loadviewbyviewname", "node_views" );
            DataTable NodeViewsTable = NodeViewsTableSelect.getTable( " where lower(viewname) = '" + ViewName.ToLower() + "'" );

            if( 1 == NodeViewsTable.Rows.Count )
            {
                if( null != NodeViewsTable.Rows[0]["viewxml"] )
                {
                    LoadXml( NodeViewsTable.Rows[0]["viewxml"].ToString() );
                    ReturnVal = true;
                }
            }

            return ( ReturnVal );
        }

        /// <summary>
        /// Load View XML into this View (String)
        /// </summary>
        public bool LoadXml( string ViewString )
        {
            string ViewXmlAsString = ViewString;
            //Prefixing the Type to the ViewXml is not required, for backwards compatibility
            int indexOfXmlString = ViewString.IndexOf( delimiter ) + 1;

            if( indexOfXmlString != 1 ) //string.IndexOf returns 0 if not found
            {
                ViewXmlAsString = ViewString.Substring( indexOfXmlString );
            }

            XmlDocument ViewXmlDoc = new XmlDocument();
            ViewXmlDoc.LoadXml( ViewXmlAsString );
            return _load( ViewXmlDoc );
        }

        /// <summary>
        /// Load View JSON into this View
        /// </summary>
        public bool LoadJson( JObject ViewObj )
        {
            bool Ret = false;
            try
            {
                if( ViewObj.HasValues )
                {
                    Ret = _load( ViewObj );
                }
            }
            catch( Exception ex )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Attempt to restore view failed.", "JObject.Parse() failed on view JSON with " + ex.ToString() );
            }
            return Ret;
        }

        /// <summary>
        /// Load View JSON into this View (String)
        /// </summary>
        public bool LoadJson( string ViewString )
        {
            bool Ret = false;
            JObject ViewJson;
            try
            {
                ViewJson = JObject.Parse( ViewString );
                Ret = LoadJson( ViewJson );
            }
            catch( Exception ex )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Attempt to restore view failed.", "JObject.Parse() failed on view JSON with " + ex.ToString() );
            }
            return Ret;
        }

        /// <summary>
        /// Load View XML into this View (XML)
        /// </summary>
        public bool LoadXml( XmlDocument ViewXmlDoc )
        {
            return _load( ViewXmlDoc );
        }

        private bool _load( XmlDocument ViewXmlDoc )
        {
            XmlNode RootXmlNode = ViewXmlDoc.DocumentElement;

            // This handles the recursive load operation internally
            Root = new CswNbtViewRoot( _CswNbtResources, this, RootXmlNode );

            return true;
        }

        private bool _load( JObject ViewJson )
        {
            // This handles the recursive load operation internally
            Root = new CswNbtViewRoot( _CswNbtResources, this, ViewJson );
            return true;
        }

        public delegate void BeforeEditViewEventHandler( CswNbtView View );
        public delegate void AfterEditViewEventHandler( CswNbtView View );
        public static string BeforeEditViewEventName = "BeforeEditView";
        public static string AfterEditViewEventName = "AfterEditView";

        /// <summary>
        /// Save the view to the database
        /// </summary>
        public void save()
        {
            if( !ViewId.isSet() )
            {
                throw new CswDniException( CswEnumErrorType.Error, "Invalid View: " + ViewName, "You must call saveNew() before calling save() on a new view." );
            }

            CswTableUpdate ViewTableUpdate = _CswNbtResources.makeCswTableUpdate( "CswNbtView_save_update", "node_views" );
            DataTable ViewTable = ViewTableUpdate.getTable( "nodeviewid", ViewId.get(), true );
            if( ViewTable.Rows.Count == 0 )
                throw new CswDniException( CswEnumErrorType.Error, "Invalid View", "No views that match viewid = " + ViewId.ToString() + " were found while attempting to save" );

            // Enforce name-visibility compound unique constraint
            if( ViewName == string.Empty )
                throw new CswDniException( CswEnumErrorType.Warning, "View name cannot be blank", "View name cannot be blank" );

            if( Visibility == CswEnumNbtViewVisibility.Unknown )
                throw new CswDniException( CswEnumErrorType.Error, "View visibility is Unknown", "User attempted to save a view (" + ViewId + ", " + ViewName + ") with visibility == Unknown" );

            if( Visibility == CswEnumNbtViewVisibility.User && VisibilityUserId == null )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "View Visibility User is required", "User attempted to save a view (" + ViewId + ", " + ViewName + ") with visibility == User, but no user selected" );
            }
            if( Visibility == CswEnumNbtViewVisibility.Role && VisibilityRoleId == null )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "View Visibility Role is required", "User attempted to save a view (" + ViewId + ", " + ViewName + ") with visibility == Role, but no role selected" );
            }

            if( !ViewIsUnique( _CswNbtResources, ViewId, ViewName, Visibility, VisibilityUserId, VisibilityRoleId ) )
                throw new CswDniException( CswEnumErrorType.Warning, "View name is already in use", "There is already a view with name: " + ViewName + " and visibility setting: " + Visibility.ToString() );

            // Before Edit View Events
            Collection<object> BeforeEditEvents = _CswNbtResources.CswEventLinker.Trigger( BeforeEditViewEventName );
            foreach( object Handler in BeforeEditEvents )
            {
                if( Handler is BeforeEditViewEventHandler )
                    ( (BeforeEditViewEventHandler) Handler )( this );
            }


            ViewTable.Rows[0]["viewname"] = ViewName;
            ViewTable.Rows[0]["category"] = Category;
            ViewTable.Rows[0]["viewxml"] = this.ToString();
            ViewTable.Rows[0]["groupbysiblings"] = CswConvert.ToDbVal( GroupBySiblings );
            ViewTable.Rows[0]["visibility"] = Visibility.ToString();
            ViewTable.Rows[0]["viewmode"] = ViewMode.ToString();
            ViewTable.Rows[0]["isdemo"] = CswConvert.ToDbVal( IsDemo );
            ViewTable.Rows[0]["issystem"] = CswConvert.ToDbVal( IsSystem );
            if( Visibility == CswEnumNbtViewVisibility.Role )
                ViewTable.Rows[0]["roleid"] = CswConvert.ToDbVal( VisibilityRoleId.PrimaryKey );
            else
                ViewTable.Rows[0]["roleid"] = DBNull.Value;
            if( Visibility == CswEnumNbtViewVisibility.User )
                ViewTable.Rows[0]["userid"] = CswConvert.ToDbVal( VisibilityUserId.PrimaryKey );
            else
                ViewTable.Rows[0]["userid"] = DBNull.Value;
            ViewTableUpdate.update( ViewTable );


            // After Edit View Events
            Collection<object> AfterEditEvents = _CswNbtResources.CswEventLinker.Trigger( AfterEditViewEventName );
            foreach( object Handler in AfterEditEvents )
            {
                if( Handler is AfterEditViewEventHandler )
                    ( (AfterEditViewEventHandler) Handler )( this );
            }
        }

        /// <summary>
        /// Create a new view
        /// </summary>
        /// <param name="ViewName">Name of New View</param>
        /// <param name="Visibility">Visibility Permission setting</param>
        /// <param name="RoleId">Primary key of role restriction (if Visibility is Role-based)</param>
        /// <param name="UserId">Primary key of user restriction (if Visibility is User-based)</param>
        /// <param name="CopyViewId">Primary key of view to copy</param>
        public void saveNew( string ViewName, CswEnumNbtViewVisibility Visibility, CswPrimaryKey RoleId = null, CswPrimaryKey UserId = null, Int32 CopyViewId = Int32.MinValue )
        {
            CswNbtView CopyView = null;
            if( CopyViewId > 0 )
            {
                CswTableSelect ViewTableSelect = _CswNbtResources.makeCswTableSelect( "makenewview_select", "node_views" );
                DataTable CopyViewTable = ViewTableSelect.getTable( "nodeviewid", CopyViewId );
                if( CopyViewTable.Rows.Count > 0 )
                {
                    string CopyViewAsString = CopyViewTable.Rows[0]["viewxml"].ToString();
                    CopyView = new CswNbtView( _CswNbtResources );
                    CopyView.LoadXml( CopyViewAsString );
                }
            }
            saveNew( ViewName, Visibility, RoleId, UserId, CopyView );
        }

        public delegate void BeforeNewViewEventHandler();
        public delegate void AfterNewViewEventHandler( CswNbtView View );
        public static string BeforeNewViewEventName = "BeforeNewView";
        public static string AfterNewViewEventName = "AfterNewView";

        /// <summary>
        /// Create a new view
        /// </summary>
        /// <param name="ViewName">Name of New View</param>
        /// <param name="Visibility">Visibility Permission setting</param>
        /// <param name="RoleId">Primary key of role restriction (if Visibility is Role-based)</param>
        /// <param name="UserId">Primary key of user restriction (if Visibility is User-based)</param>
        /// <param name="CopyView">View to copy</param>
        public void saveNew( string ViewName, CswEnumNbtViewVisibility Visibility, CswPrimaryKey RoleId, CswPrimaryKey UserId, CswNbtView CopyView )
        {
            if( ViewName == string.Empty )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "View name cannot be blank", "CswNbtView.saveNew() called with empty ViewName parameter" );
            }

            // Enforce name-visibility compound unique constraint
            if( !ViewIsUnique( _CswNbtResources, new CswNbtViewId(), ViewName, Visibility, UserId, RoleId ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, "View name is already in use", "There is already a view with conflicting name and visibility settings" );
            }

            // Before New View Events
            Collection<object> BeforeNewEvents = _CswNbtResources.CswEventLinker.Trigger( BeforeNewViewEventName );
            foreach( object Handler in BeforeNewEvents )
            {
                if( Handler is BeforeNewViewEventHandler )
                {
                    ( (BeforeNewViewEventHandler) Handler )();
                }
            }

            // Insert a new view
            CswTableUpdate ViewTableUpdate = _CswNbtResources.makeCswTableUpdate( "ViewTableUpdate", "node_views" );
            DataTable ViewTable = ViewTableUpdate.getEmptyTable();

            CswEnumNbtViewRenderingMode NewViewMode = this.ViewMode;
            string NewViewCategory = Category;
            if( null != CopyView )
            {
                NewViewMode = CopyView.ViewMode;
                NewViewCategory = CopyView.Category;
            }

            DataRow NewRow = ViewTable.NewRow();
            NewRow["viewname"] = ViewName;
            NewRow["groupbysiblings"] = CswConvert.ToDbVal( GroupBySiblings );
            NewRow["visibility"] = Visibility.ToString();
            NewRow["viewmode"] = NewViewMode.ToString();
            NewRow["category"] = NewViewCategory;
            NewRow["userid"] = CswConvert.ToDbVal( Int32.MinValue );
            if( UserId != null )
            {
                NewRow["userid"] = CswConvert.ToDbVal( UserId.PrimaryKey );
            }

            NewRow["roleid"] = CswConvert.ToDbVal( Int32.MinValue );
            if( Visibility == CswEnumNbtViewVisibility.Role && RoleId != null )
            {
                NewRow["roleid"] = CswConvert.ToDbVal( RoleId.PrimaryKey );
            }

            ViewTable.Rows.Add( NewRow );
            ViewTableUpdate.update( ViewTable );

            // Reset this view info to the new one
            Clear();
            if( CopyView != null )
            {
                this.LoadXml( CopyView.ToXml() );
            }
            this.ViewId = new CswNbtViewId( CswConvert.ToInt32( NewRow["nodeviewid"] ) );
            this.ViewName = ViewName;
            this.ViewMode = NewViewMode;
            this.Visibility = Visibility;
            this.VisibilityRoleId = RoleId;
            this.VisibilityUserId = UserId;
            this.Category = NewViewCategory;
            this.GroupBySiblings = GroupBySiblings;

            // The XML includes the viewid and viewname, so it has to be updated before it can be saved
            NewRow["viewxml"] = this.ToString();
            ViewTableUpdate.update( ViewTable );

            _CswNbtResources.ViewSelect.clearCache();  // BZ 9197


            // After New View Events
            Collection<object> AfterNewEvents = _CswNbtResources.CswEventLinker.Trigger( AfterNewViewEventName );
            foreach( object Handler in AfterNewEvents )
            {
                if( Handler is AfterNewViewEventHandler )
                {
                    ( (AfterNewViewEventHandler) Handler )( this );
                }
            }
        }

        /// <summary>
        /// For importing a view from another schema
        /// </summary>
        public void ImportView( string ViewName, string ViewXml, Dictionary<Int32, Int32> NodeTypeMap, Dictionary<Int32, Int32> NodeTypePropMap, Dictionary<string, Int32> NodeMap )
        {
            this.saveNew( ViewName, CswEnumNbtViewVisibility.Unknown, null, null, null );
            CswNbtViewId NewViewId = this.ViewId;
            this.LoadXml( ViewXml );  // this overwrites the viewid
            this.ViewId = NewViewId;  // so set it back

            foreach( CswNbtViewRelationship Relationship in this.Root.GetAllChildrenOfType( CswEnumNbtViewNodeType.CswNbtViewRelationship ) )
            {
                if( Relationship.PropId != Int32.MinValue )
                {
                    if( Relationship.PropType == CswEnumNbtViewPropIdType.NodeTypePropId )
                    {
                        CswNbtMetaDataNodeTypeProp NewProp = _CswNbtResources.MetaData.getNodeTypeProp( NodeTypePropMap[Relationship.PropId] );
                        Relationship.overrideProp( Relationship.PropOwner, NewProp );
                    }
                }
                else
                {
                    if( Relationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                    {
                        CswNbtMetaDataNodeType NewNodeType = _CswNbtResources.MetaData.getNodeType( NodeTypeMap[Relationship.SecondId] );
                        Relationship.overrideSecond( NewNodeType );
                    }
                }
            }
            foreach( CswNbtViewProperty Property in this.Root.GetAllChildrenOfType( CswEnumNbtViewNodeType.CswNbtViewProperty ) )
            {
                if( Property.Type == CswEnumNbtViewPropType.NodeTypePropId )
                {
                    if( NodeTypePropMap.ContainsKey( Property.NodeTypePropId ) )
                        Property.NodeTypePropId = NodeTypePropMap[Property.NodeTypePropId];
                }
            }

            // Map the role and user
            if( this.VisibilityRoleId != null && this.VisibilityRoleId.PrimaryKey != Int32.MinValue )
                this.VisibilityRoleId = new CswPrimaryKey( "nodes", NodeMap[this.VisibilityRoleId.PrimaryKey.ToString().ToLower()] );
            if( this.VisibilityUserId != null && this.VisibilityUserId.PrimaryKey != Int32.MinValue )
                this.VisibilityUserId = new CswPrimaryKey( "nodes", NodeMap[this.VisibilityUserId.PrimaryKey.ToString().ToLower()] );

        } // ImportView()


        public delegate void BeforeDeleteViewEventHandler( CswNbtView View );
        public delegate void AfterDeleteViewEventHandler();
        public static string BeforeDeleteViewEventName = "BeforeDeleteView";
        public static string AfterDeleteViewEventName = "AfterDeleteView";
        /// <summary>
        /// Delete this view from the database
        /// This will reset all users using this view as their default to not having a default view.
        /// </summary>
        public void Delete()
        {
            // Before Delete View Events
            Collection<object> BeforeDeleteEvents = _CswNbtResources.CswEventLinker.Trigger( BeforeDeleteViewEventName );
            foreach( object Handler in BeforeDeleteEvents )
            {
                if( Handler is BeforeDeleteViewEventHandler )
                    ( (BeforeDeleteViewEventHandler) Handler )( this );
            }


            // Reset user information

            CswNbtMetaDataObjectClass User_ObjectClass = _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.UserClass );

            // generate the view
            CswNbtView View = new CswNbtView( _CswNbtResources );
            View.ViewName = "CswNbtView.Delete(" + ViewId + ")";
            CswNbtViewRelationship UserRelationship = View.AddViewRelationship( User_ObjectClass, false );

            // generate the tree
            ICswNbtTree UserTree = _CswNbtResources.Trees.getTreeFromView( _CswNbtResources.CurrentNbtUser, View, true, false, false );

            // reset the default view and quick launch views
            UserTree.goToRoot();
            if( UserTree.getChildNodeCount() > 0 )
            {
                for( Int32 u = 0; u < UserTree.getChildNodeCount(); u++ )
                {
                    UserTree.goToNthChild( u );
                    CswNbtNode UserNode = UserTree.getNodeForCurrentPosition();
                    CswNbtObjClassUser UserNodeAsUser = (CswNbtObjClassUser) UserNode;
                    // case 23924
                    if( UserNodeAsUser.Username != CswNbtObjClassUser.ChemSWAdminUsername )
                    {
                        // Remove this view from the Quick Launch views
                        if( UserNodeAsUser.FavoriteViews != null )
                        {
                            if( UserNodeAsUser.FavoriteViews.ContainsViewId( this.ViewId ) )
                                UserNodeAsUser.FavoriteViews.RemoveViewId( this.ViewId );
                        }
                        UserNode.postChanges( false );
                        UserTree.goToParentNode();
                    }
                }
            }

            // Now remove all foreign keys to this view
            CswTableUpdate NodeTypePropsUpdate = _CswNbtResources.makeCswTableUpdate( "DeleteView_prop_update", "nodetype_props" );
            DataTable NodeTypePropsTable = NodeTypePropsUpdate.getTable( "nodeviewid", ViewId.get() );
            foreach( DataRow NTPRow in NodeTypePropsTable.Rows )
            {
                NTPRow["nodeviewid"] = CswConvert.ToDbVal( Int32.MinValue );
            }
            NodeTypePropsUpdate.update( NodeTypePropsTable );

            _CswNbtResources.ViewSelect.removeSessionView( this );

            // Now delete the view
            CswTableUpdate ViewTableUpdate = _CswNbtResources.makeCswTableUpdate( "Delete_view_nodeview_update", "node_views" );
            DataTable ViewTable = ViewTableUpdate.getTable( "nodeviewid", ViewId.get() );
            if( ViewTable.Rows.Count > 0 )
            {
                ViewTable.Rows[0].Delete();
                ViewTableUpdate.update( ViewTable );
            }

            _CswNbtResources.ViewSelect.clearCache();

            // After Delete View Events
            Collection<object> AfterDeleteEvents = _CswNbtResources.CswEventLinker.Trigger( AfterDeleteViewEventName );
            foreach( object Handler in AfterDeleteEvents )
            {
                if( Handler is AfterDeleteViewEventHandler )
                    ( (AfterDeleteViewEventHandler) Handler )();
            }
        } // Delete()

        public const Int32 ViewNameLength = 200;

        public static bool ViewIsUnique( CswNbtResources CswNbtResources, CswNbtViewId ViewId, string ViewName, CswEnumNbtViewVisibility Visibility, CswPrimaryKey UserId, CswPrimaryKey RoleId )
        {
            // truncate the name
            if( ViewName.Length > ViewNameLength )
                ViewName = ViewName.Substring( 0, ViewNameLength );

            if( Visibility != CswEnumNbtViewVisibility.Hidden &&
                Visibility != CswEnumNbtViewVisibility.Property )
            {
                CswTableSelect CheckViewTableSelect = CswNbtResources.makeCswTableSelect( "ViewIsUnique_select", "node_views" );
                string WhereClause = "where viewname = '" + CswTools.SafeSqlParam( ViewName ) + "'";
                if( ViewId.get() > 0 )
                    WhereClause += " and nodeviewid <> " + ViewId.get().ToString();

                if( Visibility == CswEnumNbtViewVisibility.User )
                {
                    if( null != UserId )
                    {
                        // Must be unique against other private views for this user
                        // Must be unique against all role and global views 
                        WhereClause += " and ((visibility = '" + Visibility.ToString() + "'";
                        WhereClause += "       and userid = " + UserId.PrimaryKey.ToString() + ")";
                        WhereClause += "      or visibility <> '" + Visibility.ToString() + "')";
                    }
                    else
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "User is required", "A specific user must be selected for User-visible views" );
                    }
                }
                else if( Visibility == CswEnumNbtViewVisibility.Role )
                {
                    if( null != RoleId )
                    {
                        // Must be unique against other role views for the same role
                        // Must be unique against all private and global views 
                        WhereClause += " and ((visibility = '" + Visibility.ToString() + "'";
                        WhereClause += "       and roleid = " + RoleId.PrimaryKey.ToString() + ")";
                        WhereClause += "      or visibility <> '" + Visibility.ToString() + "')";
                    }
                    else
                    {
                        throw new CswDniException( CswEnumErrorType.Warning, "Role is required", "A specific role must be selected for Role-visible views" );
                    }
                }
                else if( Visibility == CswEnumNbtViewVisibility.Global )
                {
                    // Must be globally unique 
                }

                // don't include Property or Hidden views for uniqueness
                WhereClause += " and visibility <> '" + CswEnumNbtViewVisibility.Property.ToString() + "'";
                WhereClause += " and visibility <> '" + CswEnumNbtViewVisibility.Hidden.ToString() + "'";
                //DataTable CheckViewTable = CheckViewTableSelect.getTable( WhereClause );
                return ( CheckViewTableSelect.getRecordCount( WhereClause ) == 0 );
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Convert the View XML to a String
        /// </summary>
        public override string ToString()
        {
            XmlDocument Doc = this.ToXml();
            return _ViewType.ToString() + delimiter + Doc.InnerXml;
        }
        /// <summary>
        /// Returns the View XML as XML
        /// </summary>
        public XmlDocument ToXml()
        {
            XmlDocument Doc = new XmlDocument();
            Doc.AppendChild( Root.ToXml( Doc ) );
            return Doc;
        }

        /// <summary>
        /// Returns the View as JSON
        /// </summary>
        /// <returns></returns>
        public JObject ToJson()
        {
            JObject Doc = Root.ToJson();
            return Doc;
        }

        /// <summary>
        /// Blank out the View
        /// </summary>
        public void Clear()
        {
            string OldName = string.Empty;
            CswNbtViewId ViewId = new CswNbtViewId();
            if( Root != null )
            {
                OldName = Root.ViewName;
                ViewId = Root.ViewId;
            }
            Root = new CswNbtViewRoot( _CswNbtResources, this );
            Root.ViewName = OldName;
            Root.ViewId = ViewId;
        }

        public void CopyFromView( CswNbtView ViewToCopy )
        {
            if( null != ViewToCopy && ViewToCopy.Root.ChildRelationships.Count > 0 )
            {
                this.Root.ChildRelationships.Clear();
                foreach( CswNbtViewRelationship Relationship in ViewToCopy.Root.ChildRelationships )
                {
                    this.Root.addChildRelationship( Relationship );
                }
                this.save();
            }
        }

        /// <summary>
        /// Sets the internal CswNbtResources obj is null, sets it to the supplied one
        /// </summary>
        public void SetResources( CswNbtResources NbtResources )
        {
            if( null == _CswNbtResources )
            {
                _CswNbtResources = NbtResources;
            }
        }

        #region Find ViewNode

        public bool IsEmpty()
        {
            return Root.ChildRelationships.Count == 0;
        }

        /// <summary>
        /// Determines if all the object classes and nodetypes are enabled in a view
        /// </summary>
        public bool IsFullyEnabled()
        {
            return IsFullyEnabledRecursive( Root );
        }
        private bool IsFullyEnabledRecursive( CswNbtViewNode ViewNode )
        {
            bool ret = true;
            if( ViewNode is CswNbtViewRoot )
            {
                foreach( CswNbtViewRelationship Child in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
                {
                    ret = ret && IsFullyEnabledRecursive( Child );
                    if( !ret ) break;
                }
            }
            else if( ViewNode is CswNbtViewRelationship )
            {
                // Make sure this nodetype/object class is enabled
                CswNbtViewRelationship ThisRelationship = ViewNode as CswNbtViewRelationship;
                if( ThisRelationship.SecondType == CswEnumNbtViewRelatedIdType.NodeTypeId )
                {
                    CswNbtMetaDataNodeType ThisNodeType = _CswNbtResources.MetaData.getNodeType( ThisRelationship.SecondId );
                    ret = ret && ( ThisNodeType != null );
                }
                else if( ThisRelationship.SecondType == CswEnumNbtViewRelatedIdType.ObjectClassId )
                {
                    CswNbtMetaDataObjectClass ThisObjectClass = _CswNbtResources.MetaData.getObjectClass( ThisRelationship.SecondId );
                    ret = ret && ( ThisObjectClass != null );
                }
                else if( ThisRelationship.SecondType == CswEnumNbtViewRelatedIdType.PropertySetId )
                {
                    IEnumerable<CswNbtMetaDataObjectClass> ObjectClasses = _CswNbtResources.MetaData.getObjectClassesByPropertySetId( ThisRelationship.SecondId );
                    ret = ret && ObjectClasses.Any( ThisObjectClass => ThisObjectClass != null );
                }

                // Recurse
                foreach( CswNbtViewProperty Child in ( (CswNbtViewRelationship) ViewNode ).Properties )
                {
                    ret = ret && IsFullyEnabledRecursive( Child );
                    if( !ret ) break;
                }
                if( ret )
                {
                    foreach( CswNbtViewRelationship Child in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
                    {
                        ret = ret && IsFullyEnabledRecursive( Child );
                        if( !ret ) break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Returns true if the NodeType is present in one of the View Relationships
        /// </summary>
        public bool ContainsNodeType( CswNbtMetaDataNodeType NodeType )
        {
            return ContainsNodeTypeRecursive( Root.ChildRelationships, NodeType );
        }
        private bool ContainsNodeTypeRecursive( Collection<CswNbtViewRelationship> Relationships, CswNbtMetaDataNodeType NodeType )
        {
            bool ReturnVal = false;
            foreach( CswNbtViewRelationship CurrentRelationship in Relationships )
            {
                //if( ( ( NbtViewRelatedIdType.NodeTypeId == CurrentRelationship.FirstType ) &&
                //     ( CurrentRelationship.FirstId == NodeType.FirstVersionNodeTypeId ) ) ||
                //    ( ( NbtViewRelatedIdType.NodeTypeId == CurrentRelationship.SecondType ) &&
                //     ( CurrentRelationship.SecondId == NodeType.FirstVersionNodeTypeId ) ) )
                if( CurrentRelationship.FirstMatches( NodeType, IgnoreVersions : true ) || CurrentRelationship.SecondMatches( NodeType, IgnoreVersions : true ) )
                {
                    ReturnVal = true;
                    break;
                }
                else if( CurrentRelationship.ChildRelationships.Count > 0 )
                {
                    ReturnVal = ContainsNodeTypeRecursive( CurrentRelationship.ChildRelationships, NodeType );
                    if( ReturnVal ) break;
                }
            }
            return ( ReturnVal );
        }

        /// <summary>
        /// Returns true if the NodeType Prop is present in one of the View Properties
        /// </summary>
        public bool ContainsNodeTypeProp( CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            return ContainsNodeTypePropRecursive( Root.ChildRelationships, NodeTypeProp );
        }

        private bool ContainsNodeTypePropRecursive( Collection<CswNbtViewRelationship> Relationships, CswNbtMetaDataNodeTypeProp NodeTypeProp )
        {
            bool ReturnVal = false;
            foreach( CswNbtViewRelationship CurrentRelationship in Relationships )
            {
                foreach( CswNbtViewProperty CurrentProp in CurrentRelationship.Properties )
                {
                    if( CurrentProp.Type == CswEnumNbtViewPropType.NodeTypePropId &&
                        CurrentProp.NodeTypePropId == NodeTypeProp.FirstPropVersionId )
                    {
                        ReturnVal = true;
                        break;
                    }
                }

                if( ReturnVal )
                    break;

                if( CurrentRelationship.ChildRelationships.Count > 0 )
                {
                    ReturnVal = ContainsNodeTypePropRecursive( CurrentRelationship.ChildRelationships, NodeTypeProp );
                }

                if( ReturnVal )
                    break;
            }
            return ( ReturnVal );
        }

        /// <summary>
        /// Returns true if the ViewNode is present in the View
        /// </summary>
        public CswNbtViewNode FindViewNode( CswDelimitedString ViewNodeToFindAsString )
        {
            CswNbtViewNode ViewNodeToFind = CswNbtViewNode.makeViewNode( _CswNbtResources, this, ViewNodeToFindAsString );
            return FindViewNode( ViewNodeToFind );
        }

        /// <summary>
        /// Returns true if the ViewNode is present in the View
        /// </summary>
        public CswNbtViewNode FindViewNode( CswNbtViewNode ViewNodeToFind )
        {
            return FindViewNodeRecursive( Root, ViewNodeToFind );
        }

        private CswNbtViewNode FindViewNodeRecursive( CswNbtViewNode ViewNode, CswNbtViewNode Match )
        {
            CswNbtViewNode ret = null;
            if( ViewNode == Match )
            {
                ret = ViewNode;
            }
            else
            {
                // Recurse
                if( ViewNode is CswNbtViewRoot )
                {
                    foreach( CswNbtViewRelationship Child in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
                    {
                        ret = FindViewNodeRecursive( Child, Match );
                        if( ret != null )
                            break;
                    }
                }
                else if( ViewNode is CswNbtViewRelationship )
                {
                    foreach( CswNbtViewProperty Child in ( (CswNbtViewRelationship) ViewNode ).Properties )
                    {
                        ret = FindViewNodeRecursive( Child, Match );
                        if( ret != null )
                            break;
                    }
                    if( ret == null )
                    {
                        foreach( CswNbtViewRelationship Child in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
                        {
                            ret = FindViewNodeRecursive( Child, Match );
                            if( ret != null )
                                break;
                        }
                    }
                }
                else if( ViewNode is CswNbtViewProperty )
                {
                    foreach( CswNbtViewPropertyFilter Child in ( (CswNbtViewProperty) ViewNode ).Filters )
                    {
                        ret = FindViewNodeRecursive( Child, Match );
                        if( ret != null )
                            break;
                    }
                }
            }
            return ret;
        } // FindViewNodeRecursive

        public CswNbtViewNode FindViewNodeByArbitraryId( string ArbitraryId )
        {
            return FindViewNodeByArbitraryIdRecursive( Root, ArbitraryId );
        }
        private CswNbtViewNode FindViewNodeByArbitraryIdRecursive( CswNbtViewNode ViewNode, string ArbitraryId )
        {
            CswNbtViewNode ret = null;
            if( ViewNode.ArbitraryId == ArbitraryId )
            {
                ret = ViewNode;
            }
            else
            {
                // Recurse
                if( ViewNode is CswNbtViewRoot )
                {
                    foreach( CswNbtViewRelationship Child in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
                    {
                        ret = FindViewNodeByArbitraryIdRecursive( Child, ArbitraryId );
                        if( ret != null )
                            break;
                    }
                }
                else if( ViewNode is CswNbtViewRelationship )
                {
                    foreach( CswNbtViewProperty Child in ( (CswNbtViewRelationship) ViewNode ).Properties )
                    {
                        ret = FindViewNodeByArbitraryIdRecursive( Child, ArbitraryId );
                        if( ret != null )
                            break;
                    }
                    if( ret == null )
                    {
                        foreach( CswNbtViewRelationship Child in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
                        {
                            ret = FindViewNodeByArbitraryIdRecursive( Child, ArbitraryId );
                            if( ret != null )
                                break;
                        }
                    }
                }
                else if( ViewNode is CswNbtViewProperty )
                {
                    foreach( CswNbtViewPropertyFilter Child in ( (CswNbtViewProperty) ViewNode ).Filters )
                    {
                        ret = FindViewNodeByArbitraryIdRecursive( Child, ArbitraryId );
                        if( ret != null )
                            break;
                    }
                }
            }
            return ret;
        } // FindViewNodeByArbitraryIdRecursive

        public CswNbtViewNode FindViewNodeByUniqueId( string UniqueId )
        {
            return FindViewNodeByUniqueIdRecursive( Root, UniqueId );
        }
        private CswNbtViewNode FindViewNodeByUniqueIdRecursive( CswNbtViewNode ViewNode, string UniqueId )
        {
            CswNbtViewNode ret = null;
            if( ViewNode.UniqueId == UniqueId )
            {
                ret = ViewNode;
            }
            else
            {
                // Recurse
                if( ViewNode is CswNbtViewRoot )
                {
                    foreach( CswNbtViewRelationship Child in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
                    {
                        ret = FindViewNodeByUniqueIdRecursive( Child, UniqueId );
                        if( ret != null )
                            break;
                    }
                }
                else if( ViewNode is CswNbtViewRelationship )
                {
                    foreach( CswNbtViewProperty Child in ( (CswNbtViewRelationship) ViewNode ).Properties )
                    {
                        ret = FindViewNodeByUniqueIdRecursive( Child, UniqueId );
                        if( ret != null )
                            break;
                    }
                    if( ret == null )
                    {
                        foreach( CswNbtViewRelationship Child in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
                        {
                            ret = FindViewNodeByUniqueIdRecursive( Child, UniqueId );
                            if( ret != null )
                                break;
                        }
                    }
                }
                else if( ViewNode is CswNbtViewProperty )
                {
                    foreach( CswNbtViewPropertyFilter Child in ( (CswNbtViewProperty) ViewNode ).Filters )
                    {
                        ret = FindViewNodeByUniqueIdRecursive( Child, UniqueId );
                        if( ret != null )
                            break;
                    }
                }
            }
            return ret;
        } // FindViewNodeByUniqueIdRecursive

        /// <summary>
        /// Returns an ordered LinkedList of CswNbtViewProperty
        /// </summary>
        public LinkedList<CswNbtViewProperty> getOrderedViewProps( bool SelectDistinctPropNames )
        {
            var OrderedViewProps = new LinkedList<CswNbtViewProperty>();
            var ViewRelationships = new Collection<CswNbtViewRelationship>();
            ViewRelationships = getAllNbtViewRelationships();
            var ViewProps = new Collection<CswNbtViewProperty>();
            var ViewPropNames = new Collection<string>();

            foreach( CswNbtViewProperty ChildProp in ViewRelationships.Select( Relationship => Relationship.Properties )
                                                                      .SelectMany( ChildProps => ChildProps )
                                                                      .Where( ChildProp => !SelectDistinctPropNames || !ViewPropNames.Contains( ChildProp.Name ) ) )
            {
                ViewProps.Add( ChildProp );
                ViewPropNames.Add( ChildProp.Name );
            }

            // Add View props with defined order to the stack in ascending ViewProp.Order order
            foreach( var ViewProperty in from NbtViewProperty
                                           in ViewProps
                                         where NbtViewProperty.Order > 0
                                         orderby NbtViewProperty.Order descending, NbtViewProperty.Name ascending
                                         select NbtViewProperty )
            {
                OrderedViewProps.AddFirst( ViewProperty );
                ViewProps.Remove( ViewProperty );
            }

            // Add View props with undefined order to the end of the stack in ascending ViewProp.NbtNodeTypeProp.PropName order
            foreach( var ViewProperty in from NbtViewProperty
                                           in ViewProps
                                         orderby NbtViewProperty.Name ascending
                                         select NbtViewProperty )
            {
                OrderedViewProps.AddLast( ViewProperty );
            }
            return OrderedViewProps;
        }

        public Collection<CswNbtViewRelationship> getAllNbtViewRelationships()
        {
            var RelationshipCollection = new Collection<CswNbtViewRelationship>();
            foreach( CswNbtViewRelationship ChildRelationship in Root.ChildRelationships )
            {
                RelationshipCollection.Add( ChildRelationship );
                getAllNbtViewRelationshipsRecursive( ref RelationshipCollection, ChildRelationship );
            }
            return RelationshipCollection;
        }

        private void getAllNbtViewRelationshipsRecursive( ref Collection<CswNbtViewRelationship> RelationshipCollection, CswNbtViewRelationship Relationship )
        {
            foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
            {
                if( ChildRelationship.ChildRelationships.Count > 0 )
                {
                    getAllNbtViewRelationshipsRecursive( ref RelationshipCollection, ChildRelationship );
                }
                RelationshipCollection.Add( ChildRelationship );
            }
        }

        /// <summary>
        /// Returns the MetaDataTypeId of the first View property. Determines the MetaDataType to 'Add' in grids.
        /// </summary>
        public Int32 ViewMetaDataTypeId
        {
            get
            {
                Int32 ReturnVal = Int32.MinValue;
                if( null != findFirstProperty() )
                {
                    if( null != findFirstProperty().NodeTypeProp )
                    {
                        ReturnVal = findFirstProperty().NodeTypeProp.NodeTypeId;
                    }
                    else if( null != findFirstProperty().ObjectClassProp )
                    {
                        ReturnVal = findFirstProperty().ObjectClassProp.ObjectClassPropId;
                    }
                }
                return ReturnVal;
            }
        }

        /// <summary>
        /// Returns the CswNbtViewProperty which corresponds to the property type and primary key provided
        /// </summary>
        public CswNbtViewProperty findPropertyById( CswEnumNbtViewPropType PropType, Int32 PropId )
        {
            CswNbtViewProperty ret = null;
            foreach( CswNbtViewRelationship Child in Root.ChildRelationships )
            {
                ret = findPropertyByIdRecursive( Child, PropType, PropId );
                if( ret != null )
                    break;
            }
            return ret;
        }
        private static CswNbtViewProperty findPropertyByIdRecursive( CswNbtViewRelationship Relationship, CswEnumNbtViewPropType PropType, Int32 PropId )
        {
            CswNbtViewProperty ret = null;
            foreach( CswNbtViewProperty ChildProperty in Relationship.Properties )
            {
                if( ( PropType == CswEnumNbtViewPropType.ObjectClassPropId && ChildProperty.ObjectClassPropId == PropId ) ||
                    ( PropType == CswEnumNbtViewPropType.NodeTypePropId && ChildProperty.NodeTypePropId == PropId ) )
                {
                    ret = ChildProperty;
                    break;
                }
            }
            foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
            {
                if( ret != null )
                    break;
                ret = findPropertyByIdRecursive( ChildRelationship, PropType, PropId );
            }
            return ret;

        }
        /// <summary>
        /// Returns the CswNbtViewProperty which corresponds to the property type and name provided
        /// </summary>
        public CswNbtViewProperty findPropertyByName( string PropName )
        {
            CswNbtViewProperty ret = null;
            foreach( CswNbtViewRelationship Child in Root.ChildRelationships )
            {
                ret = findPropertyByNameRecursive( Child, PropName );
                if( ret != null )
                    break;
            }
            return ret;
        }
        private static CswNbtViewProperty findPropertyByNameRecursive( CswNbtViewRelationship Relationship, string PropName )
        {
            CswNbtViewProperty ret = null;
            foreach( CswNbtViewProperty ChildProperty in Relationship.Properties )
            {
                if( ChildProperty.Name.ToLower() == PropName.ToLower() )
                {
                    ret = ChildProperty;
                    break;
                }
            }
            foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
            {
                if( ret != null )
                    break;
                ret = findPropertyByNameRecursive( ChildRelationship, PropName );
            }
            return ret;

        }

        /// <summary>
        /// Find the first Property in the view tree
        /// </summary>
        public CswNbtViewProperty findFirstProperty()
        {
            return _findFirstPropertyRecursive( Root );
        }

        private static CswNbtViewProperty _findFirstPropertyRecursive( CswNbtViewNode ViewNode )
        {
            CswNbtViewProperty ret = null;
            if( ViewNode is CswNbtViewProperty )
            {
                ret = (CswNbtViewProperty) ViewNode;
            }
            else if( ViewNode is CswNbtViewRoot )
            {
                foreach( CswNbtViewRelationship R in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
                {
                    ret = _findFirstPropertyRecursive( R );
                    if( ret != null ) break;
                }
            }
            else if( ViewNode is CswNbtViewRelationship )
            {
                if( ( (CswNbtViewRelationship) ViewNode ).Properties.Count > 0 )
                    ret = (CswNbtViewProperty) ( (CswNbtViewRelationship) ViewNode ).Properties[0];

                if( ret == null )
                {
                    foreach( CswNbtViewRelationship R in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
                    {
                        ret = _findFirstPropertyRecursive( R );
                        if( ret != null ) break;
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// Find the first Property Filter in the view tree
        /// </summary>
        public CswNbtViewPropertyFilter findFirstPropertyFilter()
        {
            return _findFirstFilterRecursive( Root );
        }

        private static CswNbtViewPropertyFilter _findFirstFilterRecursive( CswNbtViewNode ViewNode )
        {
            CswNbtViewPropertyFilter ret = null;
            if( ViewNode is CswNbtViewPropertyFilter )
            {
                ret = (CswNbtViewPropertyFilter) ViewNode;
            }
            else if( ViewNode is CswNbtViewRoot )
            {
                foreach( CswNbtViewRelationship R in ( (CswNbtViewRoot) ViewNode ).ChildRelationships )
                {
                    ret = _findFirstFilterRecursive( R );
                    if( ret != null ) break;
                }
            }
            else if( ViewNode is CswNbtViewRelationship )
            {
                foreach( CswNbtViewProperty P in ( (CswNbtViewRelationship) ViewNode ).Properties )
                {
                    ret = _findFirstFilterRecursive( P );
                    if( ret != null ) break;
                }
                if( ret == null )
                {
                    foreach( CswNbtViewRelationship R in ( (CswNbtViewRelationship) ViewNode ).ChildRelationships )
                    {
                        ret = _findFirstFilterRecursive( R );
                        if( ret != null ) break;
                    }
                }
            }
            else if( ViewNode is CswNbtViewProperty )
            {
                if( ( (CswNbtViewProperty) ViewNode ).Filters.Count > 0 )
                    ret = (CswNbtViewPropertyFilter) ( (CswNbtViewProperty) ViewNode ).Filters[0];
            }
            return ret;
        }

        #endregion Find ViewNode

        #region Sort Property

        /// <summary>
        /// Returns the property used to sort the View
        /// </summary>
        public CswNbtViewProperty getSortProperty()
        {
            CswNbtViewProperty ret = null;
            foreach( CswNbtViewRelationship Child in Root.ChildRelationships )
            {
                ret = getSortPropertyRecursive( Child );
                if( ret != null )
                    break;
            }
            return ret;
        }
        private CswNbtViewProperty getSortPropertyRecursive( CswNbtViewRelationship ViewNode )
        {
            CswNbtViewProperty ret = null;
            foreach( CswNbtViewProperty Child in ViewNode.Properties )
            {
                if( Child.SortBy )
                {
                    ret = Child;
                    break;
                }
            }
            foreach( CswNbtViewRelationship Child in ViewNode.ChildRelationships )
            {
                if( ret != null )
                    break;
                ret = getSortPropertyRecursive( Child );
            }
            return ret;
        }

        /// <summary>
        /// Sets the Property used to sort the View.  Only one property can have this setting.
        /// </summary>
        public void setSortProperty( CswNbtViewProperty Property, CswEnumNbtViewPropertySortMethod SortMethod, bool ClearExistingSort = true )
        {
            if( ClearExistingSort )
            {
                clearSortProperty();
            }
            Property.SortBy = true;
            Property.SortMethod = SortMethod;
        }
        /// <summary>
        /// Clears all properties of the Sort Property setting
        /// </summary>
        public void clearSortProperty()
        {
            foreach( CswNbtViewRelationship Child in Root.ChildRelationships )
                clearSortPropertyRecursive( Child );
        }
        private void clearSortPropertyRecursive( CswNbtViewRelationship ViewNode )
        {
            foreach( CswNbtViewProperty Child in ViewNode.Properties )
                Child.SortBy = false;

            foreach( CswNbtViewRelationship Child in ViewNode.ChildRelationships )
                clearSortPropertyRecursive( Child );
        }

        #endregion Sort Property

        #region Session Cache functions

        /// <summary>
        /// Save this View to Session's data cache
        /// </summary>
        public void SaveToCache( bool IncludeInQuickLaunch, bool UpdateCache = false, bool KeepInQuickLaunch = false )
        {
            // don't cache twice
            if( SessionViewId == null ||
                false == SessionViewId.isSet() ||
                UpdateCache ||
                IncludeInQuickLaunch )  // case 23999
            {
                bool ForQuickLaunch = ( IncludeInQuickLaunch && IsQuickLaunch );
                _SessionViewId = _CswNbtResources.ViewSelect.saveSessionView( this, ForQuickLaunch, KeepInQuickLaunch, UpdateCache );
            }
        } // SaveToCache()

        public void clearSessionViewId()
        {
            _SessionViewId = null;
        }

        private CswNbtSessionDataId _SessionViewId = null;
        /// <summary>
        /// Key for retrieving the view from the Session's data cache
        /// </summary>
        public CswNbtSessionDataId SessionViewId
        {
            get
            {
                return _SessionViewId ?? new CswNbtSessionDataId();
            }
            set { _SessionViewId = value; }
        }



        #endregion Session Cache functions

        #region IEquatable
        /// <summary>
        /// IEquatable: ==
        /// </summary>
        public static bool operator ==( CswNbtView view1, CswNbtView view2 )
        {
            // If both are null, or both are same instance, return true.
            if( System.Object.ReferenceEquals( view1, view2 ) )
            {
                return true;
            }

            // If one is null, but not both, return false.
            if( ( (object) view1 == null ) || ( (object) view2 == null ) )
            {
                return false;
            }

            // Now we know neither are null.  Compare values.
            if( view1.ToString() == view2.ToString() )
                return true;
            else
                return false;
        }

        /// <summary>
        /// IEquatable: !=
        /// </summary>
        public static bool operator !=( CswNbtView view1, CswNbtView view2 )
        {
            return !( view1 == view2 );
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public override bool Equals( object obj )
        {
            if( !( obj is CswNbtView ) )
                return false;
            return this == (CswNbtView) obj;
        }

        /// <summary>
        /// IEquatable: Equals
        /// </summary>
        public bool Equals( CswNbtView obj )
        {
            return this == (CswNbtView) obj;
        }

        /// <summary>
        /// IEquatable: GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            int hashcode = 0;
            if( this.ViewId.isSet() )
            {
                // Positive hashes are for saved views
                hashcode = this.ViewId.get();
            }
            else
            {
                // Negative hashes are for dynamic views or searches
                hashcode = Math.Abs( this.ToString().GetHashCode() ) * -1;
            }
            return hashcode;
        }
        #endregion IEquatable

        /// <summary>
        /// For displaying property grids outside of a node context, 
        /// </summary>
        public CswNbtView PrepGridView( CswPrimaryKey NodeId )
        {
            CswNbtView RetView = _CswNbtResources.ViewSelect.restoreView( this.ToString() );
            if( null != RetView )
            {
                if( RetView.Visibility == CswEnumNbtViewVisibility.Property )
                {
                    ( RetView.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Clear(); // case 21676. Clear() to avoid cache persistence.
                    ( RetView.Root.ChildRelationships[0] ).NodeIdsToFilterIn.Add( NodeId );
                }

                foreach( CswNbtViewRelationship ChildRelationship in RetView.Root.ChildRelationships )
                {
                    _clearGroupBy( ChildRelationship );
                }
                if( RetView.SessionViewId.isSet() )
                {
                    RetView.SaveToCache( false );
                }
            }
            return RetView;
        }


        private void _clearGroupBy( CswNbtViewRelationship Relationship )
        {
            Relationship.clearGroupBy();
            foreach( CswNbtViewRelationship ChildRelationship in Relationship.ChildRelationships )
            {
                _clearGroupBy( ChildRelationship );
            }
        }

    } // class CswNbtView


} // namespace ChemSW.Nbt



