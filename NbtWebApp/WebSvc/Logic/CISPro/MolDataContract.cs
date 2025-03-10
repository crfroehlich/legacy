﻿using System;
using System.Runtime.Serialization;
using ChemSW.Nbt;

namespace NbtWebApp.WebSvc.Logic.CISPro
{
    [DataContract]
    public class MolData
    {
        [DataMember]
        public String molString = String.Empty;
        [DataMember]
        public string nodeId = String.Empty;
        [DataMember]
        public string molImgAsBase64String = String.Empty;
        [DataMember]
        public string propId = String.Empty;
        [DataMember]
        public string href = String.Empty;
        [DataMember]
        public string errorMsg = String.Empty;
    }

    [DataContract]
    public class StructureSearchViewData
    {
        [DataMember]
        public String viewId = String.Empty;
        [DataMember]
        public String viewMode = CswEnumNbtViewRenderingMode.Table.ToString();
        [DataMember]
        public String molString = String.Empty;
        [DataMember]
        public bool exact = false;
    }
}