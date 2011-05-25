using System;
using System.Collections.Generic;
using System.Data;
using System.Xml;
using System.Xml.Linq;
using ChemSW.Core;
using ChemSW.DB;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.MetaData.FieldTypeRules;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.PropTypes
{
    /// <summary>
    /// Prop Class for UserSelect Properties
    /// </summary>
    public class CswNbtNodePropUserSelect : CswNbtNodeProp
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CswNbtNodePropUserSelect( CswNbtResources CswNbtResources, CswNbtNodePropData CswNbtNodePropData, CswNbtMetaDataNodeTypeProp CswNbtMetaDataNodeTypeProp )
            : base( CswNbtResources, CswNbtNodePropData, CswNbtMetaDataNodeTypeProp )
        {
            _SelectedUserIdsSubField = ( (CswNbtFieldTypeRuleUserSelect) CswNbtMetaDataNodeTypeProp.FieldTypeRule ).SelectedUserIdsSubField;
        }//ctor

        private CswNbtSubField _SelectedUserIdsSubField;

        /// <summary>
        /// Returns whether the property value is empty
        /// </summary>
        override public bool Empty
        {
            get
            {
                return ( 0 == SelectedUserIds.Count );
            }
        }

        /// <summary>
        /// Text value of property
        /// </summary>
        override public string Gestalt
        {
            get
            {
                return _CswNbtNodePropData.Gestalt;
            }

        }//Gestalt

        public static char delimiter = ',';

        /// <summary>
        /// Comma-separated list of Selected UserIds
        /// </summary>
        private CswCommaDelimitedString _SelectedUserIds = null;
        public CswCommaDelimitedString SelectedUserIds
        {
            get
            {
                if( _SelectedUserIds == null )
                {
                    _SelectedUserIds = new CswCommaDelimitedString();
                    _SelectedUserIds.OnChange += _SelectedUserIds_OnChange;
                    _SelectedUserIds.FromString( _CswNbtNodePropData.GetPropRowValue( _SelectedUserIdsSubField.Column ) );
                }
                return _SelectedUserIds;
            }
            set
            {
                _SelectedUserIds = value;
				_SelectedUserIds.OnChange += _SelectedUserIds_OnChange;
				_SelectedUserIds_OnChange();
            }
        }

        // This event handler allows us to save changes made directly to _SelectedNodeTypeIds (like .Add() )
        private void _SelectedUserIds_OnChange()
        {
            if( _CswNbtNodePropData.SetPropRowValue( _SelectedUserIdsSubField.Column, _SelectedUserIds.ToString() ) )
            {
                PendingUpdate = true;
            }
        }

        /// <summary>
        /// True if user is subscribed
        /// </summary>
        public bool IsSubscribed( CswPrimaryKey UserId )
        {
            bool ret = SelectedUserIds.Contains( UserId.PrimaryKey.ToString() );
            return ret;
        }

        /// <summary>
        /// Subscribes a user by adding the userid to the SelectedUserIds list
        /// </summary>
        public void AddUser( CswPrimaryKey UserId )
        {
            if( !IsSubscribed( UserId ) )
            {
				SelectedUserIds.Add( UserId.PrimaryKey.ToString() );
            }
        }

        /// <summary>
        /// Unsubscribes a user by removing the userid from the SelectedUserIds list
        /// </summary>
        public void RemoveUser( CswPrimaryKey UserId )
        {
            if( IsSubscribed( UserId ) )
            {
				SelectedUserIds.Remove( UserId.PrimaryKey.ToString() );
            }
        }

        /// <summary>
        /// Refresh the names of the selected users
        /// </summary>
        public void RefreshSelectedUserNames()
        {
            _CswNbtNodePropData.Gestalt = SelectedUserNames().ToString();
            PendingUpdate = false;
        }

        public DataTable getUserOptions()
        {
            DataTable Data = new CswDataTable( "Userselectdatatable", "" );
            Data.Columns.Add( NameColumn, typeof( string ) );
            Data.Columns.Add( KeyColumn, typeof( int ) );
            Data.Columns.Add( ValueColumn, typeof( bool ) );

            bool first = true;
			//ICswNbtTree UsersTree = _CswNbtResources.Trees.getTreeFromObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
			//for( int c = 0; c < UsersTree.getChildNodeCount(); c++ )
			//{
			//    UsersTree.goToNthChild( c );
			CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
			foreach(CswNbtNode UserNode in UserOC.getNodes(false, false))
			{
                DataRow NTRow = Data.NewRow();
				NTRow[NameColumn] = UserNode.NodeName; // UsersTree.getNodeNameForCurrentPosition();
				NTRow[KeyColumn] = UserNode.NodeId; //  UsersTree.getNodeIdForCurrentPosition().PrimaryKey;
                NTRow[ValueColumn] = ( SelectedUserIds.Contains( UserNode.NodeId.PrimaryKey.ToString() ) ||  //UsersTree.getNodeIdForCurrentPosition().PrimaryKey.ToString() ) ) ||
                                       ( first && Required && SelectedUserIds.Count == 0 ) );
                Data.Rows.Add( NTRow );
                first = false;

                //UsersTree.goToParentNode();
            }
            return Data;
        } // UserOptions()

        public const string NameColumn = "User Name";
        public const string KeyColumn = "UserId";
        public const string ValueColumn = "Include";

        public override void ToXml( XmlNode ParentNode )
        {
            CswXmlDocument.AppendXmlNode( ParentNode, _SelectedUserIdsSubField.ToXmlNodeName(), SelectedUserIds.ToString() );
            XmlNode OptionsNode = CswXmlDocument.AppendXmlNode( ParentNode, "options" );
            DataTable UsersTable = getUserOptions();
            foreach( DataRow UserRow in UsersTable.Rows )
            {
                XmlNode UserNode = CswXmlDocument.AppendXmlNode( OptionsNode, "user" );

                XmlNode UserNameNode = CswXmlDocument.AppendXmlNode( UserNode, "column" );
                CswXmlDocument.AppendXmlAttribute( UserNameNode, "field", NameColumn );
                CswXmlDocument.AppendXmlAttribute( UserNameNode, "value", UserRow[NameColumn].ToString() );

                XmlNode UserIdNode = CswXmlDocument.AppendXmlNode( UserNode, "column" );
                CswXmlDocument.AppendXmlAttribute( UserIdNode, "field", KeyColumn );
                CswXmlDocument.AppendXmlAttribute( UserIdNode, "value", UserRow[KeyColumn].ToString() );

                XmlNode IncludeNode = CswXmlDocument.AppendXmlNode( UserNode, "column" );
                CswXmlDocument.AppendXmlAttribute( IncludeNode, "field", ValueColumn );
                CswXmlDocument.AppendXmlAttribute( IncludeNode, "value", UserRow[ValueColumn].ToString() );
            }
        } // ToXml()

        public override void ReadXml( XmlNode XmlNode, Dictionary<Int32, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            //string UserIds = CswXmlDocument.ChildXmlNodeValueAsString( XmlNode, _SelectedUserIdsSubField.ToXmlNodeName() );
            //SelectedUserIds.FromString( UserIds );

            //foreach( string UserIdString in SelectedUserIds )
            //{
            //    if( CswTools.IsInteger( UserIdString ) )
            //    {
            //        Int32 UserId = CswConvert.ToInt32( UserIdString );
            //        if( NodeMap != null && NodeMap.ContainsKey( UserId ) )
            //            SelectedUserIds.Replace( UserIdString, NodeMap[UserId].ToString() );
            //    }
            //}
            //PendingUpdate = true;


            CswCommaDelimitedString NewSelectedUserIds = new CswCommaDelimitedString();

            foreach( XmlNode ItemNode in CswXmlDocument.ChildXmlNode( XmlNode, "options" ).ChildNodes )
            {
                string key = string.Empty;
                string name = string.Empty;
                bool value = false;
                foreach( XmlNode ColumnNode in ItemNode.ChildNodes )
                {
                    if( KeyColumn == ColumnNode.Attributes["field"].Value )
                        key = ColumnNode.Attributes["value"].Value;
                    if( NameColumn == ColumnNode.Attributes["field"].Value )
                        name = ColumnNode.Attributes["value"].Value;
                    if( ValueColumn == ColumnNode.Attributes["field"].Value )
                        value = CswConvert.ToBoolean( ColumnNode.Attributes["value"].Value );
                }
                if( value )
                {
                    NewSelectedUserIds.Add( key );
                }
            } // foreach( XmlNode ItemNode in CswXmlDocument.ChildXmlNode( XmlNode, "Options" ).ChildNodes )

            SelectedUserIds = NewSelectedUserIds;
        }

        public override void ToXElement( XElement ParentNode )
        {
            throw new NotImplementedException();
        }

        public override void ReadXElement( XElement XmlNode, Dictionary<int, int> NodeMap, Dictionary<int, int> NodeTypeMap )
        {
            throw new NotImplementedException();
        }

        public override void ReadDataRow( DataRow PropRow, Dictionary<string, Int32> NodeMap, Dictionary<Int32, Int32> NodeTypeMap )
        {
            string UserIds = CswTools.XmlRealAttributeName( PropRow[_SelectedUserIdsSubField.ToXmlNodeName()].ToString() );
            SelectedUserIds.FromString( UserIds );

            foreach( string UserIdString in SelectedUserIds )
            {
                Int32 UserId = Int32.MinValue;
                if( NodeMap != null && NodeMap.ContainsKey( UserIdString.ToLower() ) )
                    UserId = NodeMap[UserIdString.ToLower()];
                else if( CswTools.IsInteger( UserIdString ) )
                    UserId = CswConvert.ToInt32( UserIdString );
                if( UserId != Int32.MinValue )
                {
                    SelectedUserIds.Replace( UserIdString, UserId.ToString() );
                }
            }
            PendingUpdate = true;
        }

        public CswCommaDelimitedString SelectedUserNames()
        {
            CswCommaDelimitedString SelectedUserNames = new CswCommaDelimitedString();
			//ICswNbtTree UsersTree = _CswNbtResources.Trees.getTreeFromObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
			//for( int c = 0; c < UsersTree.getChildNodeCount(); c++ )
			//{
			//    UsersTree.goToNthChild( c );
			CswNbtMetaDataObjectClass UserOC = _CswNbtResources.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.UserClass );
			foreach(CswNbtNode UserNode in UserOC.getNodes(false, false))
            {
				CswPrimaryKey ThisUserId = UserNode.NodeId;  //UsersTree.getNodeIdForCurrentPosition();
				string ThisUserName = UserNode.NodeName; // UsersTree.getNodeNameForCurrentPosition();

                foreach( Int32 UserId in SelectedUserIds.ToIntCollection() )
                {
                    if( UserId != Int32.MinValue )
                    {
                        if( ThisUserId.PrimaryKey == UserId )
                        {
                            SelectedUserNames.Add( ThisUserName );
                        }
                    }
                } // foreach( Int32 UserId in SelectedUserIds.ToIntCollection() )

                //UsersTree.goToParentNode();
            } // for( int c = 0; c < UsersTree.getChildNodeCount(); c++ )

            // Sort alphabetically
            SelectedUserNames.Sort();

            return SelectedUserNames;
        } // SelectedUserNames()

    }//CswNbtNodePropUserSelect
}//namespace ChemSW.Nbt.PropTypes
