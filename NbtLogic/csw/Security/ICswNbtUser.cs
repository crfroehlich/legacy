using System;
using System.Collections.ObjectModel;
using ChemSW.Core;
using ChemSW.Nbt.ObjClasses;
using ChemSW.Security;

namespace ChemSW.Nbt.Security
{
    public interface ICswNbtUser : ICswUser
    {
        //void postChanges( bool ForceUpdate ); //bz# 5446
        //CswNbtNodePropText UsernameProperty { get; }
        //CswNbtNodePropPassword PasswordProperty { get; }
        //CswNbtObjClassRole RoleNode { get; }
        //CswNbtObjClassUser UserNode { get; }
        new bool IsAdministrator();
        //CswNbtNodePropText FirstNameProperty { get; }
        //CswNbtNodePropText LastNameProperty { get; }
        string Email { get; }
        //CswNbtNodePropText EmailProperty { get; }
        Int32 PageSize { get; }
        //CswNbtNodePropLocation DefaultLocationProperty { get; }
        //CswNbtNodePropRelationship WorkUnitProperty { get; }
        CswPrimaryKey DefaultLocationId { get; }
        CswPrimaryKey DefaultPrinterId { get; }
        CswPrimaryKey DefaultBalanceId { get; }
        CswPrimaryKey WorkUnitId { get; }
        CswPrimaryKey JurisdictionId { get; }
        Int32 UserNodeTypeId { get; }
        Int32 UserObjectClassId { get; }
        Int32 RoleNodeTypeId { get; }
        Int32 RoleObjectClassId { get; }
        Int32 PasswordPropertyId { get; }
        new bool PasswordIsExpired { get; }
        string Language { get; }
        CswNbtPropertySetPermission getPermissionForGroup( CswPrimaryKey PermissionGroupId );
        Collection<CswPrimaryKey> getUserPermissions();

    }//ICswNbtUser
}//namespace ChemSW.Nbt

