﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Nbt.MetaData;
using Newtonsoft.Json.Linq;

namespace ChemSW.Nbt.WebServices
{
	public class CswNbtWebServiceTree
	{
		private readonly CswNbtResources _CswNbtResources;
		
		public CswNbtWebServiceTree( CswNbtResources CswNbtResources )
		{
			_CswNbtResources = CswNbtResources;
		}

        public XElement getTree( CswNbtView View, string IDPrefix, string ViewMode )
        {
            var ReturnNode = new XElement( "root" );
            string EmptyOrInvalid = "Not a Tree or List view.";
            NbtViewRenderingMode RenderingMode;
            NbtViewRenderingMode.TryParse( ViewMode, true, out RenderingMode );

            if( RenderingMode == NbtViewRenderingMode.Tree || RenderingMode == NbtViewRenderingMode.List )
            {
                ICswNbtTree Tree = _CswNbtResources.Trees.getTreeFromView( View, true, false, false, false );
                if( Tree.getChildNodeCount() > 0 )
                {

                    var RootNode = new XElement( "root" );
                    if( RenderingMode == NbtViewRenderingMode.Tree )
                    {
                        var RootItemNode = new XElement( "item",
                                                new XAttribute( "id", IDPrefix + "root" ),
                                                new XAttribute( "rel", "root" ),
                                                new XElement( "content",
                                                            new XElement( "name", View.ViewName ) ) );
                        RootNode.Add( RootItemNode );
                        _runTreeNodesRecursive( Tree, IDPrefix, RootItemNode );
                    }
                    else if( RenderingMode == NbtViewRenderingMode.List )
                    {
                        _runTreeNodesRecursive( Tree, IDPrefix, RootNode );
                    }
                    ReturnNode = new XElement( "result",
                                    new XElement( "tree", RootNode ),
                                    new XElement( "types", getTypes( View ).ToString() ) );
                }
                else
                {
                    EmptyOrInvalid = "No Results";
                }
            } // if( Tree.getChildNodeCount() > 0 )
            else
            {
                EmptyOrInvalid = "Not a Tree or List view.";
            }
            if( !string.IsNullOrEmpty( EmptyOrInvalid ) )
            {
                ReturnNode.Add( new XElement( "item",
                                    new XAttribute( "id", "-1" ),
                                    new XAttribute( "rel", "root" ),
                                        new XElement( "content",
                                            new XElement( "name", EmptyOrInvalid ))) );
            }
            return ReturnNode;
		} // getTree()

		public JObject getTypes( CswNbtView View )
		{
			var TypesJson = new JObject( 
				                new JProperty( "root",
			                        new JObject(
			                            new JProperty( "icon",
			                                new JObject(
			                                    new JProperty( "image", "Images/view/viewtree.gif" )
			                                )
			                            )
			                        )
								)
							) {new JProperty( "default", "" )};

			var NodeTypes = new Dictionary<Int32, string>();
			ArrayList Relationships = View.Root.GetAllChildrenOfType( NbtViewNodeType.CswNbtViewRelationship );
			foreach( CswNbtViewRelationship Rel in Relationships )
			{
				if( Rel.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
				{
					CswNbtMetaDataNodeType NodeType = _CswNbtResources.MetaData.getNodeType( Rel.SecondId );
					if( !NodeTypes.ContainsKey(NodeType.FirstVersionNodeTypeId) )
					{
						NodeTypes.Add( NodeType.FirstVersionNodeTypeId, NodeType.IconFileName );
					}
				} // if( Rel.SecondType == CswNbtViewRelationship.RelatedIdType.NodeTypeId )
				else
				{
					CswNbtMetaDataObjectClass ObjectClass = _CswNbtResources.MetaData.getObjectClass( Rel.SecondId );
					foreach( CswNbtMetaDataNodeType NodeType in ObjectClass.NodeTypes )
					{
						if( !NodeTypes.ContainsKey( NodeType.FirstVersionNodeTypeId ) )
						{
							NodeTypes.Add( NodeType.FirstVersionNodeTypeId, NodeType.IconFileName );
						}
					}
				} // else
			} // foreach( CswNbtViewRelationship Rel in Relationships )

			foreach( var JProperty in NodeTypes
								.Select( NodeType => new JProperty( "nt_" + NodeType.Key, 
														new JObject( new JProperty( "icon", 
															new JObject( new JProperty( "image", "Images/icons/" + NodeType.Value ) ) ) ) ) )
								.Where( NodeTypeJProp => null == TypesJson.Property( NodeTypeJProp.Name ) ) )
			{
				TypesJson.Add( JProperty );
			}
			return TypesJson;
		} // getTypes()

		/// <summary>
		/// Recursively iterate the tree and add child nodes according to parent hierarchy
		/// </summary>
		private void _runTreeNodesRecursive( ICswNbtTree Tree, string IDPrefix, XElement GrandParentNode )
		{
			for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
			{
				Tree.goToNthChild( c );

				CswNbtNode ThisNode = Tree.getNodeForCurrentPosition();
				CswNbtNodeKey ThisNodeKey = Tree.getNodeKeyForCurrentPosition();

			    string ThisNodeKeyString = wsTools.ToSafeJavaScriptParam( ThisNodeKey.ToJavaScriptParam()) ;
				string ThisNodeName = Tree.getNodeNameForCurrentPosition();
				string ThisNodeId = IDPrefix + ThisNode.NodeId.ToString();
				string ThisNodeRel = "nt_" + ThisNode.NodeType.FirstVersionNodeTypeId;

				var ParentNode = ( new XElement( "item",
										new XAttribute( "id", ThisNodeId ),
										new XAttribute( "rel", ThisNodeRel ),
                                        new XAttribute( "cswnbtnodekey", ThisNodeKeyString ), 
											new XElement( "content" ,
												new XElement( "name" , ThisNodeName )
												)
											)
									);
				if( Tree.getChildNodeCount() > 0 )
				{
					// XElement ChildNode = _runTreeNodesRecursive()
					_runTreeNodesRecursive( Tree, IDPrefix, ParentNode );
				}
				
				GrandParentNode.Add( ParentNode );
				Tree.goToParentNode();
			} // for( Int32 c = 0; c < Tree.getChildNodeCount(); c++ )
			
		} // _runTreeNodesRecursive()

	} // class CswNbtWebServiceTree

} // namespace ChemSW.Nbt.WebServices
