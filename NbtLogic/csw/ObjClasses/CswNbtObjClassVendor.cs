using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.PropTypes;

namespace ChemSW.Nbt.ObjClasses
{
    public class CswNbtObjClassVendor : CswNbtObjClass
    {
        public new sealed class PropertyName: CswNbtObjClass.PropertyName
        {
            public const string VendorName = "Vendor Name";
            public const string AccountNo = "Account No";
            public const string DeptBillCode = "Dept Bill Code";
            public const string ContactName = "Contact Name";
            public const string Street1 = "Street1";
            public const string Street2 = "Street2";
            public const string City = "City";
            public const string State = "State";
            public const string Zip = "Zip";
            public const string Phone = "Phone";
            public const string Fax = "Fax";
            public const string CorporateEntityName = "Corporate Entity";
            public const string VendorTypeName = "Vendor Type";
            public const string Country = "Country";
            public const string Internal = "Internal";
        }

        public sealed class VendorTypes
        {
            public const string Corporate = "Corporate";
            public const string Sales = "Sales";
            public const string Technical = "Technical";
            public const string Manufacturing = "Manufacturing";

            public CswCommaDelimitedString Options = new CswCommaDelimitedString
                {
                    Corporate,
                    Sales,
                    Technical,
                    Manufacturing
                };
        }

        public CswNbtObjClassVendor( CswNbtResources CswNbtResources, CswNbtNode Node ) : base( CswNbtResources, Node ) {}

        public override CswNbtMetaDataObjectClass ObjectClass
        {
            get { return _CswNbtResources.MetaData.getObjectClass( CswEnumNbtObjectClass.VendorClass ); }
        }

        /// <summary>
        /// Convert a CswNbtNode to a CswNbtObjClassVendor
        /// </summary>
        public static implicit operator CswNbtObjClassVendor( CswNbtNode Node )
        {
            CswNbtObjClassVendor ret = null;
            if( null != Node && _Validate( Node, CswEnumNbtObjectClass.VendorClass ) )
            {
                ret = (CswNbtObjClassVendor) Node.ObjClass;
            }
            return ret;
        }

        #region Inherited Events

        protected override void beforeWriteNodeLogic( bool Creating, bool OverrideUniqueValidation )
        {
            if( VendorType.wasAnySubFieldModified() || CorporateIdentity.wasAnySubFieldModified() )
            {
                //For each Corporate Entity, there can only be one vendortype of Corporate aka Highlander attribute
                if( false == VendorType.Empty && false == CorporateIdentity.Empty )
                {
                    foreach( CswNbtObjClassVendor vendorNode in this.NodeType.getNodes( false, false ) )
                    {
                        if( vendorNode.NodeId != this.NodeId &&
                            vendorNode.CorporateIdentity.Text.Equals( CorporateIdentity.Text ) &&
                            vendorNode.VendorType.Value.Equals( VendorTypes.Corporate ) &&
                            this.VendorType.Value.Equals( VendorTypes.Corporate ) )
                        {
                            throw new CswDniException( CswEnumErrorType.Warning,
                                    "Multiple Corporate Entities with a Vendor Type of " + VendorTypes.Corporate + " are not allowed",
                                    "A Vendor with a Corporate Entity of " + vendorNode.CorporateIdentity.Text + " already exists with a Vendor Type of " + vendorNode.VendorType.Value );
                        }
                    }
                }
            }
        }//beforeWriteNode()

        #endregion

        #region Object class specific properties

        public CswNbtNodePropText VendorName { get { return ( _CswNbtNode.Properties[PropertyName.VendorName] ); } }
        public CswNbtNodePropText AccountNo { get { return ( _CswNbtNode.Properties[PropertyName.AccountNo] ); } }
        public CswNbtNodePropText DeptBillCode { get { return ( _CswNbtNode.Properties[PropertyName.DeptBillCode] ); } }
        public CswNbtNodePropText ContactName { get { return ( _CswNbtNode.Properties[PropertyName.ContactName] ); } }
        public CswNbtNodePropText Street1 { get { return ( _CswNbtNode.Properties[PropertyName.Street1] ); } }
        public CswNbtNodePropText Street2 { get { return ( _CswNbtNode.Properties[PropertyName.Street2] ); } }
        public CswNbtNodePropText City { get { return ( _CswNbtNode.Properties[PropertyName.City] ); } }
        public CswNbtNodePropText State { get { return ( _CswNbtNode.Properties[PropertyName.State] ); } }
        public CswNbtNodePropText Zip { get { return ( _CswNbtNode.Properties[PropertyName.Zip] ); } }
        public CswNbtNodePropText Phone { get { return ( _CswNbtNode.Properties[PropertyName.Phone] ); } }
        public CswNbtNodePropText Fax { get { return ( _CswNbtNode.Properties[PropertyName.Fax] ); } }
        public CswNbtNodePropText CorporateIdentity { get { return ( _CswNbtNode.Properties[PropertyName.CorporateEntityName] ); } }
        public CswNbtNodePropList VendorType { get { return ( _CswNbtNode.Properties[PropertyName.VendorTypeName] ); } }
        public CswNbtNodePropText Country { get { return ( _CswNbtNode.Properties[PropertyName.Country] ); } }
        public CswNbtNodePropLogical Internal { get { return ( _CswNbtNode.Properties[PropertyName.Internal] ); } }

        #endregion

    }//CswNbtObjClassVendor

}//namespace ChemSW.Nbt.ObjClasses