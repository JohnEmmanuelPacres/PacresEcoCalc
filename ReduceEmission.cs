using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Pacres_EcoCalc
{
    abstract class ReduceEmission
    {
        public virtual double RE(double newTotalEmission, double totalEmission)
        {
            double percentage = (newTotalEmission / totalEmission) * 100;
            return percentage;
        }

    }
    class ReduceEmissionVehicle : ReduceEmission
    {
        private double miles;
        private double GHGratio = 1.0136847440446, emissionFactor = 19.4,
            fuelEfficiency = 25.4, vehicleEfficiency = 0.04; //2023 update
        public double Miles
        {
            get { return this.miles; }
            set { this.miles = value; }
        }
        public ReduceEmissionVehicle()
        {
            this.miles = 0;
        }
        public ReduceEmissionVehicle(double miles)
        {
            this.miles = miles;
        }
        public double RE(double Miles)
        {
            double weeksInYear = 52;
            double newEmission = ((Miles * weeksInYear) / fuelEfficiency) * emissionFactor * GHGratio;
            return newEmission;
        }
        public double REdeduct(double Miles)
        {
            double weeksInYear = 52;
            double newEmission = ((Miles * weeksInYear) / fuelEfficiency) * emissionFactor * GHGratio * vehicleEfficiency;
            return newEmission;
        }
        public double REpercent(double newTotalEmission, double totalEmission)
        {
            double percentage = (newTotalEmission / totalEmission) * 100;
            return percentage;
        }
    }

    class ReduceEmissionHousehold : ReduceEmission
    {
        private double appliances;
        private double heatingPercentNG = 0.627198849944028, heatingPercentE = 0.0929607996815207,
                       heatingPercentFO = 0.87202416049608, heatingPercentP = 0.697129102095252, thermostatSaving = 0.03,
                       fridgeReplacementKwhSaving = 322, lampKwHSaving = 33, NG_heatSource = 728, FO_heatSource = 1056,
                       KwhPerLaundry = 0.96, computerSaving = 107.1, thermostatCoolingSaving = 0.06,
                       ACpercent = 0.144768107287701, eFactor = 1.259639, dryerEnergy = 769;
        private double windowReplacementSaving = 25210000, BTUperKwh = 3412, EF_NG = 119.577540412585, BTUper1000cfNG = 1023000,
                       EF_FO = 22.6131018642659, BTUperGalFO = 138691.086802163, EF_P = 12.4260044803915, 
                       BTUperGalP = 91335.9395011829;

        public double Appliances
        {
            get { return this.appliances; }
            set { this.appliances = value; }
        }
        public ReduceEmissionHousehold()
        {
            this.Appliances = 0;
        }
        public ReduceEmissionHousehold(double appliances, EnergyType energy)
        {
            this.Appliances = appliances;
        }
        public ReduceEmissionHousehold(double appliances)
        {
            this.Appliances = appliances;
        }
        public double REheater(double previousEnergy, double tempThermo, EnergyType energy)
        {
            double emission = 0;
            if (energy == EnergyType.NaturalGas)
            {
                emission = previousEnergy * heatingPercentNG * thermostatSaving * ((tempThermo * 9 / 5) + 32);
            }
            else if (energy == EnergyType.Electricity)
            {
                emission = previousEnergy * heatingPercentE * thermostatSaving * ((tempThermo * 9 / 5) + 32);
            }
            else if (energy == EnergyType.FuelOil)
            {
                emission = previousEnergy * heatingPercentFO * thermostatSaving * ((tempThermo * 9 / 5) + 32);
            }
            else
            {
                emission = previousEnergy * heatingPercentP * thermostatSaving * ((tempThermo * 9 / 5) + 32);
            }
            return emission;
        }
        public double REairCon(double previousEnergy, double airCon)
        {
            double emission = previousEnergy * ACpercent * thermostatCoolingSaving * ((airCon * 9 / 5) + 32);
            return emission;
        }
        public double REComputer(double computer)
        {
            double emission = 134.9073369;
            return emission;
        }
        public double RELaundry(double laundry)
        {
            double emission = KwhPerLaundry * eFactor * 52 * laundry;
            return emission;
        }
        public double REDryer(double percentageDry)
        {
            double emission = (dryerEnergy * (percentageDry*0.01)) * eFactor;
            return emission;
        }
        public double REGreenEnergy(double greenEnergy, double electricity)
        {
            double emission = (greenEnergy * 0.01) * electricity;
            return emission;
        }
        public double REBulb(int bulbs)
        {
            double emission = bulbs * lampKwHSaving * eFactor;
            return emission;
        }
        public double REFridge(double refrigerator)
        {
            double emission = refrigerator * fridgeReplacementKwhSaving * eFactor;
            return emission;
        }
        public double REFurnace(double furnace, EnergyType energy)
        {
            double emission = 0;
            if (energy == EnergyType.NaturalGas)
            {
                emission = furnace * NG_heatSource;
                return emission;
            }
            else if (energy == EnergyType.FuelOil)
            {
                emission = furnace * FO_heatSource;
                return emission;
            }
            return emission;
        }
        public double REWindows(double windows, EnergyType energy)
        {
            double emission = 0;
            if (energy == EnergyType.NaturalGas)
            {
                emission = EF_NG * (windowReplacementSaving / BTUper1000cfNG);
            }
            else if (energy == EnergyType.Electricity)
            {
                emission = eFactor * (windowReplacementSaving / BTUperKwh);
            }
            else if (energy == EnergyType.FuelOil)
            {
                emission = EF_FO * (windowReplacementSaving / BTUperGalFO);
            }
            else
            {
                emission = EF_P * (windowReplacementSaving / BTUperGalP);
            }
            return emission;
        }
        public double RE(double appliances, double emissionHeater, double emissionAircon, double emissionComputer,
            double emissionLaundry, double emissionDryer, double emissionGE, double emissionBulb,
            double emissionFridge, double emissionFurnace, double emissionWindows)
        {
            double emission = appliances * (emissionHeater + emissionAircon + emissionComputer + emissionLaundry +
                emissionDryer + emissionGE + emissionBulb + emissionFridge + emissionFurnace + emissionWindows);
            return emission;
        }
        public double REpercent(double newTotalEmission, double totalEmission)
        {
            double percentage = (newTotalEmission / totalEmission) * 100;
            return percentage;
        }
    }

    class ReduceEmissionWaste : ReduceEmission
    {
        private int houseMember;
        private double averageWasteEmission = 691.5, metalRecycling = 89.38,
            plasticRecycling = 35.56, glassRecycling = 25.39, newspaperRecycling = 113.14,
            magazineRecycling = 27.46;
        public int HouseMember
        {
            get { return this.houseMember; }
            set { this.houseMember = value; }
        }
        public ReduceEmissionWaste()
        {
            this.HouseMember = 0;
        }
        public ReduceEmissionWaste(int HouseMember)
        {
            this.HouseMember = HouseMember;
        }
        public double RE(int HouseMembers, string metalRecycle,
            string plasticRecycle, string glassRecycle, string newspaperRecycle,
            string magazineRecycle)
        {
            double metal = 0, plastic = 0, glass = 0, newspaper = 0, magazine = 0;
            if (metalRecycle == "No")
            {
                metal = metalRecycling * HouseMembers;
            }
            if (plasticRecycle == "No")
            {
                plastic = plasticRecycling * HouseMembers;
            }
            if (glassRecycle == "No")
            {
                 glass = glassRecycling * HouseMembers;
            }
            if (newspaperRecycle == "No")
            {
                newspaper = newspaperRecycling * HouseMembers;
            }
            if (magazineRecycle == "No")
            {
                magazine = magazineRecycling * HouseMembers;
            }
            double emission = metal + plastic + glass + newspaper + magazine;
            return emission;
        }
        public double REpercent(double newTotalEmission, double totalEmission)
        {
            double percentage = (newTotalEmission / totalEmission) * 100;
            return percentage;
        }
    }
}