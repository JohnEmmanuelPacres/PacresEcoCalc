using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacres_EcoCalc
{
    public enum EnergyType
    {
        NaturalGas,
        FuelOil,
        Propane,
        Electricity
    }
    public enum UnitType
    {
        Bill,
        CubicFeet,
        Therm,
        Gallons,
        kWh
    }
    abstract class HomeEnergyCalculation
    {
        abstract public double CalculateHE(EnergyType energyType, UnitType unitType, double Input);
    }
    class HomeEnergy : HomeEnergyCalculation
    {
        private double input;
        private double NGpricePerCubicFt = 10.68, NGemf = 119.577540412585, NGtherm = 11.68890913124;
        private double pricePerkwh = 13.52 /*2024 Veco*/, PHElectricity_emf = 1.37, pricePerGallonPropane = 2.47, pricePerGallonFuelOil = 4.02;
        private double Pemf = 12.4260044803915, FOemf = 22.6131018642659, monthsInYear = 12.00;

        public double Input
        {
            get { return this.input; }
            set { this.input = value; }
        }
        public HomeEnergy()
        {
            this.Input = 0;
        }
        public HomeEnergy(EnergyType energyType, UnitType unitType, double Input)
        {
            this.Input = Input;
        }
        public override double CalculateHE(EnergyType energyType, UnitType unitType, double Input)
        {
            return energyType switch
            {
                EnergyType.NaturalGas => CalculateNaturalGasEmission(unitType, input),
                EnergyType.FuelOil => CalculateFuelOilEmission(unitType, input),
                EnergyType.Propane => CalculatePropaneEmission(unitType, input),
                EnergyType.Electricity => CalculateElectricityEmission(unitType, input),
                _ => throw new ArgumentException("Invalid energy or unit type.")
            };
        }

        private double CalculateNaturalGasEmission(UnitType unitType, double value)
        {
            return unitType switch
            {
                UnitType.Bill => ((input * 0.017) / NGpricePerCubicFt) * NGemf * monthsInYear,
                UnitType.CubicFeet => input * NGemf * monthsInYear,
                UnitType.Therm => input * NGtherm * monthsInYear,
                _ => throw new ArgumentException("Invalid unit for natural gas.")
            };
        }

        private double CalculateFuelOilEmission(UnitType unitType, double value)
        {
            return unitType switch
            {
                UnitType.Bill => ((input * 0.017) / pricePerGallonFuelOil) * FOemf * monthsInYear,
                UnitType.Gallons => input * FOemf * monthsInYear,
                _ => throw new ArgumentException("Invalid unit for fuel oil.")
            };
        }

        private double CalculatePropaneEmission(UnitType unitType, double Input)
        {
            return unitType switch
            {
                UnitType.Bill => ((input * 0.017) / pricePerGallonPropane) * Pemf * monthsInYear,
                UnitType.Gallons => input * Pemf * monthsInYear,
                _ => throw new ArgumentException("Invalid unit for propane.")
            };
        }

        private double CalculateElectricityEmission(UnitType unitType, double Input)
        {
            return unitType switch
            {
                UnitType.Bill => ((input * 0.017) / pricePerkwh) * PHElectricity_emf * monthsInYear,
                UnitType.kWh => input * PHElectricity_emf * monthsInYear,
                _ => throw new ArgumentException("Invalid unit for electricity.")
            };
        }
    }
}