using ChemSW.Exceptions;
using ChemSW.Nbt.Actions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassEnterprisePart : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string GCAS = "GCAS";
            public const string Request = "Request";
            public const string Description = "Description";
            public const string Version = "Version";
            public const string EPNoLookup = "EPNo Lookup";
        }

        public CswNbtObjClassEnterprisePart( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.EnterprisePartClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassEnterprisePart
        /// </summary>
        public static implicit operator CswNbtObjClassEnterprisePart( CswNbtNode Node )
        {
            CswNbtObjClassEnterprisePart ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.EnterprisePartClass ) )
            {
                ret = (CswNbtObjClassEnterprisePart) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override bool onButtonClick( NbtButtonData ButtonData )
        {
            if( null != ButtonData.NodeTypeProp )
            {
                string OCPPropName = ButtonData.NodeTypeProp.getObjectClassPropName();
                switch( OCPPropName )
                {
                    case PropertyName.Request:
                        if( _CswNbtResources.Permit.can( CswEnumNbtActionName.Submit_Request ) )
                        {
                            ButtonData.Action = CswEnumNbtButtonAction.request;
                            CswNbtActRequesting RequestAct = new CswNbtActRequesting( _CswNbtResources );
                            CswNbtObjClassRequestItem RequestItem = RequestAct.makeEnterprisePartRequestItem( this, ButtonData );
                            ButtonData.Data["titleText"] = "Add to Cart: " + RequestItem.Type.Value;
                            ButtonData.Data["requestItemProps"] = RequestAct.getRequestItemAddProps( RequestItem.Node );
                            ButtonData.Data["requestItemNodeTypeId"] = RequestItem.NodeTypeId;
                        }
                        else
                        {
                            throw new CswDniException( CswEnumErrorType.Warning, "You do not have permission to the Submit Request action.", "You do not have permission to the Submit Request action." );
                        }
                        break;
                }
            }
            return true;
        }
        #endregion

        #region Object class specific properties

        public CswNbtNodePropText GCAS { get { return _CswNbtNode.Properties[PropertyName.GCAS]; } }
        public CswNbtNodePropButton Request { get { return _CswNbtNode.Properties[PropertyName.Request]; } }
        public CswNbtNodePropButton Description { get { return _CswNbtNode.Properties[PropertyName.Description]; } }
        public CswNbtNodePropButton Version { get { return _CswNbtNode.Properties[PropertyName.Version]; } }
        public CswNbtNodePropButton EPNoLookup { get { return _CswNbtNode.Properties[PropertyName.EPNoLookup]; } }

        #endregion

    }//CswNbtObjClassEnterprisePart

}//namespace ChemSW.Nbt.ObjClasses
