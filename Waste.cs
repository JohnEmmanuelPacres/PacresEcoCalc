using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacres_EcoCalc
{
    abstract class WasteCalculation
    {
        abstract public double WasteEmission(int HouseMembers, string metalRecycle, 
            string plasticRecycle, string glassRecycle, string newspaperRecycle, 
            string magazineRecycle);
    }

    class Waste : WasteCalculation
    {
        private int members;
        private double averageWasteEmission = 691.5, metalRecycling = 89.38,
            plasticRecycling = 35.56, glassRecycling = 25.39, newspaperRecycling = 113.14,
            magazineRecycling = 27.46;
        public int HouseMembers
        {
            get { return this.members; }
            set { this.members = value; }
        }

        public Waste()
        {
            this.HouseMembers = 0;
        }

        public Waste(int HouseMembers)
        {
            this.HouseMembers = HouseMembers;
        }

        public override double WasteEmission(int HouseMembers, string metalRecycle,
            string plasticRecycle, string glassRecycle, string newspaperRecycle,
            string magazineRecycle)
        {
            double emission = HouseMembers * averageWasteEmission;
            if (metalRecycle == "Yes")
            {
                emission -= metalRecycling * HouseMembers;
            }
            if (plasticRecycle == "Yes")
            {
                emission -= plasticRecycling * HouseMembers;
            }
            if (glassRecycle == "Yes")
            {
                emission -= glassRecycling * HouseMembers;
            }
            if(newspaperRecycle == "Yes")
            {
                emission -= newspaperRecycling * HouseMembers;
            }
            if(magazineRecycle == "Yes")
            {
                emission -= magazineRecycling * HouseMembers;
            }
            return emission;
        }
    }
}