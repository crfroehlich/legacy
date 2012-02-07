﻿using ChemSW.Core;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Schema
{
    /// <summary>
    /// Updates the schema to version 01L-21
    /// </summary>
    public class CswUpdateSchemaTo01L21 : CswUpdateSchemaTo
    {
        public override CswSchemaVersion SchemaVersion { get { return new CswSchemaVersion( 1, 'L', 21 ); } }
        public override string Description { get { return "Update to schema version " + SchemaVersion.ToString(); } }

        public override void update()
        {

            #region Case 24943


            CswNbtMetaDataObjectClass GeneratorOc = _CswNbtSchemaModTrnsctn.MetaData.getObjectClass( CswNbtMetaDataObjectClass.NbtObjectClass.GeneratorClass );
            CswNbtMetaDataObjectClassProp OwnerOcp = GeneratorOc.getObjectClassProp( CswNbtObjClassGenerator.OwnerPropertyName );

            foreach( CswNbtNode GeneratorNode in GeneratorOc.getNodes( true, false ) )
            {
                CswNbtObjClassGenerator NodeAsGenerator = CswNbtNodeCaster.AsGenerator( GeneratorNode );
                if( null == NodeAsGenerator.Owner ||
                    null == NodeAsGenerator.Owner.RelatedNodeId ||
                    null == _CswNbtSchemaModTrnsctn.Nodes.GetNode( NodeAsGenerator.Owner.RelatedNodeId ) )
                {
                    NodeAsGenerator.Enabled.Checked = Tristate.False;
                    GeneratorNode.postChanges( true );
                }
            }

            _CswNbtSchemaModTrnsctn.MetaData.UpdateObjectClassProp( OwnerOcp, CswNbtMetaDataObjectClassProp.ObjectClassPropAttributes.isrequired, true );

            #endregion Case 24943


        }//Update()

    }//class CswUpdateSchemaTo01L21

}//namespace ChemSW.Nbt.Schema


