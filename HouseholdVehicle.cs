using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;

namespace Pacres_EcoCalc
{
    public enum EmissionPeriod
    {
        Weekly,
        Monthly,
        Yearly
    }
    abstract class HouseholdVehicleCalculation
    {
        abstract public double CalculateHV(double Miles, EmissionPeriod period);
    }

    class HouseholdVehicle : HouseholdVehicleCalculation
    {
        private double miles;
        private double GHGratio = 1.0136847440446, emissionFactor = 19.4, 
            fuelEfficiency = 25.4; //2023 update
        public double Miles
        {
            get { return this.miles; }
            set { this.miles = value; }
        }

        public HouseholdVehicle()
        {
            this.Miles = 0;
        }

        public HouseholdVehicle(double Miles)
        {
            this.Miles = Miles;
        }

        public override double CalculateHV(double miles, EmissionPeriod period)
        {
            double emissions;
            if (period == EmissionPeriod.Weekly)
            {
                double weeksInYear = 52;
                emissions = ((miles * weeksInYear) / fuelEfficiency) * emissionFactor * GHGratio;
            }
            else // Yearly
            {
                emissions = (miles / fuelEfficiency) * emissionFactor * GHGratio;
            }
            return emissions;
        }
    }
}