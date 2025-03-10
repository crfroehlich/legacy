﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ChemSW.Nbt.MetaData;

namespace ChemSW.Nbt.ImportExport
{
    public class CswNbtImportDefBindingCollection : Collection<CswNbtImportDefBinding>
    {
        public IEnumerable<CswNbtImportDefBinding> byProp( Int32 Instance, CswNbtMetaDataNodeTypeProp Prop, CswNbtSubField Subfield = null )
        {
            return this.Where( b => b.DestProperty == Prop &&
                                    ( Subfield == null || b.DestSubfield == Subfield ) &&
                                    ( b.Instance == Instance ) );
        }
    }
}
