using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pacres_EcoCalc
{
    internal interface FileHandler
    {
        public void UpdateData(string dataName, EmissionDataStored newData);
        public void SaveData(EmissionDataStored data);
        public List<EmissionDataStored> ReadAllData();
        public void DeleteData(string dataName);
    }
}