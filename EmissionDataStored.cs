using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacres_EcoCalc
{
    class EmissionDataStored
    {
        private string _dataName;
        private DateTime _date;
        private double _totalEmission;
        private double _reducedAnnualEmission;
        private double _newAnnualEmission;
        private double _householdMembers;
        private double _averagePerMemberCurrent;
        private double _averagePerMemberReduced;
        //hidden
        private double _totalVehicle;
        private double _totalHE;
        private double _totalWaste;
        private double _reducedTotalVehicle;
        private double _reducedTotalHE;
        private double _reducedTotalWaste;

        public string DataName 
        { 
            get => _dataName;
            set => _dataName = value; 
        }
        public DateTime Date 
        { 
            get => _date;
            set => _date = value; 
        }
        public double TotalEmission 
        { 
            get => _totalEmission;
            set => _totalEmission = value;
        }
        public double ReducedAnnualEmission 
        { 
            get => _reducedAnnualEmission;
            set => _reducedAnnualEmission = value;
        }
        public double NewAnnualEmission 
        { 
            get => _newAnnualEmission;
            set => _newAnnualEmission = value;
        }
        public double HouseholdMembers 
        { 
            get => _householdMembers;
            set => _householdMembers = value;
        }
        public double AveragePerMemberCurrent 
        { 
            get => _averagePerMemberCurrent;
            set => _averagePerMemberCurrent = value;
        }
        public double AveragePerMemberReduced 
        { 
            get => _averagePerMemberReduced;
            set => _averagePerMemberReduced = value;
        }
        public double totalVehicle
        {
            get => _totalVehicle;
            set => _totalVehicle = value;
        }
        public double totalHE
        {
            get => _totalHE;
            set => _totalHE = value;
        }
        public double totalWaste
        {
            get => _totalWaste;
            set => _totalWaste = value;
        }
        public double reducedTotalVehicle
        {
            get => _reducedTotalVehicle; 
            set => _reducedTotalVehicle = value;
        }
        public double reducedTotalHE
        {
            get => _reducedTotalHE;
            set => _reducedTotalHE = value;
        }
        public double reducedTotalWaste
        { 
            get => _reducedTotalWaste; 
            set => _reducedTotalWaste = value;
        }
        public EmissionDataStored(
            string dataName, 
            DateTime date, 
            double totalEmission,
            double reducedAnnualEmission,
            double newAnnualEmission,
            double householdMembers,
            double averagePerMemberCurrent, 
            double averagePerMemberReduced,
            double totalVehicle,
            double totalHE,
            double totalWaste,
            double reducedTotalVehicle,
            double reducedTotalHE,
            double reducedTotalWaste)
        {
            DataName = dataName;
            Date = date;
            TotalEmission = totalEmission;
            ReducedAnnualEmission = reducedAnnualEmission;
            NewAnnualEmission = newAnnualEmission;
            HouseholdMembers = householdMembers;
            AveragePerMemberCurrent = averagePerMemberCurrent;
            AveragePerMemberReduced = averagePerMemberReduced;
            this.totalVehicle = totalVehicle;
            this.totalHE = totalHE;
            this.totalWaste = totalWaste;
            this.reducedTotalVehicle = reducedTotalVehicle;
            this.reducedTotalHE = reducedTotalHE;
            this.reducedTotalWaste = reducedTotalWaste;
        }
    }
}