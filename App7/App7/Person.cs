using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace App7
{
    public class Per
    {
        [PrimaryKey, AutoIncrement]
        public int MeterNo { get; set; }
        public double PresentReading { get; set; }
        public double PreviousReading { get; set; }
        public double ConsumptionReading { get; set; }
        public double ElectricityCharge { get; set; }
        public double DemandCharge { get; set; }
        public double ServiceCharge { get; set; }
        public string TypeOfRegistration { get; set; }
        public double PrincipalAmount { get; set; }
        public double AmountPayable { get; set; }

        public double Vat { get; set; }
    }

}
    