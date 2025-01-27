﻿using System;
using ChemSW.Core;
using ChemSW.Exceptions;
using ChemSW.Nbt.MetaData;
using ChemSW.Nbt.ObjClasses;

namespace ChemSW.Nbt.Conversion
{
    public class CswNbtUnitConversion
    {
        #region Properties and ctor

        private CswNbtResources _CswNbtResources;
        private CswEnumNbtUnitTypes _OldUnitType = CswEnumNbtUnitTypes.Unknown;
        private CswEnumNbtUnitTypes _NewUnitType = CswEnumNbtUnitTypes.Unknown;
        private Double _OldConversionFactor = Double.NaN;
        private Double _NewConversionFactor = Double.NaN;
        private Double _MaterialSpecificGravity = Double.NaN;

        public CswNbtUnitConversion()
        { }

        public CswNbtUnitConversion( CswNbtResources _CswNbtResourcesIn, CswPrimaryKey OldUnitNodeId, CswPrimaryKey NewUnitNodeId, CswPrimaryKey MaterialNodeId = null )
        {
            _CswNbtResources = _CswNbtResourcesIn;

            if( OldUnitNodeId != null && NewUnitNodeId != null )
            {
                CswNbtObjClassUnitOfMeasure OldUnitNode = _CswNbtResources.Nodes.GetNode( OldUnitNodeId );
                CswNbtObjClassUnitOfMeasure NewUnitNode = _CswNbtResources.Nodes.GetNode( NewUnitNodeId );
                setOldUnitProps( OldUnitNode );
                setNewUnitProps( NewUnitNode );
                if( MaterialNodeId != null )
                {
                    CswNbtNode MaterialNode = _CswNbtResources.Nodes.GetNode( MaterialNodeId );
                    if( MaterialNode.ObjClass.ObjectClass.ObjectClass == CswEnumNbtObjectClass.ChemicalClass )
                    {
                        setMaterialProps( MaterialNode );
                    }
                }
            }
        }

        #endregion

        #region Setters

        public void setOldUnitProps( CswNbtObjClassUnitOfMeasure OldUnitNode )
        {
            if( OldUnitNode != null )
            {
                _OldConversionFactor = OldUnitNode.ConversionFactor.RealValue;
                _OldUnitType = (CswEnumNbtUnitTypes) OldUnitNode.UnitType.Value;
            }
        }

        public void setNewUnitProps( CswNbtObjClassUnitOfMeasure NewUnitNode )
        {
            if( NewUnitNode != null )
            {
                _NewConversionFactor = NewUnitNode.ConversionFactor.RealValue;
                _NewUnitType = (CswEnumNbtUnitTypes) NewUnitNode.UnitType.Value;
            }
        }

        public void setMaterialProps( CswNbtObjClassChemical MaterialNode )
        {
            if( MaterialNode != null )
            {
                _MaterialSpecificGravity = MaterialNode.SpecificGravity.Value;
            }
        }

        #endregion

        #region Unit Conversion Functions

        /// <summary>
        /// Takes a numeric value and interconverts it between different Unit Types using the class-set UnitOfMeasure and Material prop values.
        /// If unit conversion cannot be applied, an error is thrown.
        /// </summary>
        public Double convertUnit( Double ValueToConvert )
        {
            Double ConvertedValue = ValueToConvert;
            if( false == CswTools.IsDouble( ConvertedValue ) )
            {
                ConvertedValue = 0.0;
            }
            else if( CswTools.IsDouble( _OldConversionFactor ) && CswTools.IsDouble( _NewConversionFactor ) )
            {
                CswEnumNbtUnitTypeRelationship UnitRelationship = _getUnitTypeRelationship( _OldUnitType, _NewUnitType );
                if( UnitRelationship == CswEnumNbtUnitTypeRelationship.Same )
                {
                    ConvertedValue = _applyUnitConversion( ValueToConvert, _OldConversionFactor, _NewConversionFactor );
                }
                else if( UnitRelationship != CswEnumNbtUnitTypeRelationship.NotSupported )
                {
                    if( CswTools.IsDouble( _MaterialSpecificGravity ) )
                    {
                        //UnitType-specific logic (Operator logic defined in W1005)
                        if( UnitRelationship == CswEnumNbtUnitTypeRelationship.WeightToVolume )
                        {
                            ConvertedValue = _applyUnitConversion( ValueToConvert, _OldConversionFactor, _NewConversionFactor, 1.0 / _MaterialSpecificGravity );
                        }
                        else if( UnitRelationship == CswEnumNbtUnitTypeRelationship.VolumeToWeight )
                        {
                            ConvertedValue = _applyUnitConversion( ValueToConvert, _OldConversionFactor, _NewConversionFactor, _MaterialSpecificGravity );
                        }
                    }
                    else
                    {
                        _CswNbtResources.logMessage( "Conversion failed: The Container Material's specific gravity is undefined." );
                    }
                }
                else
                {
                    _CswNbtResources.logMessage( "Conversion failed: Unable to apply unit conversion between the selected unit types." );
                }
            }
            else
            {
                _CswNbtResources.logMessage( "Conversion failed: Unable to determine appropriate conversion factors." );
            }
            return ConvertedValue;
        }

        /// <summary>
        /// Takes a numeric value and converts it from one Unit of Measurement into another using the given Conversion Factor values
        /// If unit conversion cannot be applied, an error is thrown.
        /// We're assuming the specific gravity is inverted when converting from Weight to Volume.
        /// </summary>
        private double _applyUnitConversion( Double ValueToConvert, Double OldConversionFactor, Double NewConversionFactor, Double SpecificGravity = 1 )
        {
            _validateValuesForConversion( ValueToConvert, OldConversionFactor, NewConversionFactor, SpecificGravity );
            Double ConvertedValue = ValueToConvert * OldConversionFactor * SpecificGravity / NewConversionFactor; //See W1005 for more details
            return ConvertedValue;
        }

        #endregion

        #region Validation

        /// <summary>
        /// Determines if a set of values are valid for unit conversion.
        /// </summary>
        private void _validateValuesForConversion( Double ValueToConvert, Double OldConversionFactor, Double NewConversionFactor, Double SpecificGravity )
        {
            string ErrorMessage = String.Empty;
            if( false == CswTools.IsDouble( ValueToConvert ) )
            {
                ErrorMessage = "Value to convert is undefined: " + ValueToConvert;
            }
            else if( false == CswTools.IsDouble( OldConversionFactor ) || OldConversionFactor <= 0 )
            {
                ErrorMessage = "Current unit's conversion factor is invalid: " + OldConversionFactor;
            }
            else if( false == CswTools.IsDouble( NewConversionFactor ) || NewConversionFactor <= 0 )
            {
                ErrorMessage = "New unit's conversion factor is invalid: " + NewConversionFactor;
            }
            else if( false == CswTools.IsDouble( SpecificGravity ) || SpecificGravity <= 0 )
            {
                ErrorMessage = "Material's specific gravity is invalid: " + SpecificGravity;
            }
            if( false == String.IsNullOrEmpty( ErrorMessage ) )
            {
                throw new CswDniException( CswEnumErrorType.Warning, ErrorMessage, "Unit conversion failed." );
            }
        }

        #endregion

        #region UnitType Relationship

        /// <summary>
        /// Identifies the UnitType relationship between two UnitTypes.
        /// </summary>
        private CswEnumNbtUnitTypeRelationship _getUnitTypeRelationship( CswEnumNbtUnitTypes OldUnitType, CswEnumNbtUnitTypes NewUnitType )
        {
            CswEnumNbtUnitTypeRelationship UnitRelationship;

            if( OldUnitType.ToString() == NewUnitType.ToString() )
            {
                UnitRelationship = CswEnumNbtUnitTypeRelationship.Same;
            }
            else if( OldUnitType == CswEnumNbtUnitTypes.Weight && NewUnitType == CswEnumNbtUnitTypes.Volume )
            {
                UnitRelationship = CswEnumNbtUnitTypeRelationship.WeightToVolume;
            }
            else if( OldUnitType == CswEnumNbtUnitTypes.Volume && NewUnitType == CswEnumNbtUnitTypes.Weight )
            {
                UnitRelationship = CswEnumNbtUnitTypeRelationship.VolumeToWeight;
            }
            else
            {
                UnitRelationship = CswEnumNbtUnitTypeRelationship.NotSupported;
            }

            return UnitRelationship;
        }
        
        #endregion
    }
}
