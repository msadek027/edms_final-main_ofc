using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Model.CBPSModule
{
    public class DonutChartFor
    {
        public string daysForReceive { get; set; }
        public string daysForparking { get; set; }
        public string daysForPosting { get; set; }
        public string daysForClearing { get; set; }
        public string daysForCheckProcess { get; set; }
        public string daysForDisbursment { get; set; }

        public string ReceiveToNow { get; set; }
        public string ParkingToNow { get; set; }
        public string PostingToNow { get; set; }
        public string ClearToNow { get; set; }
        public string PrintRecToNow { get; set; }
        public string DisbursmentRecToNow { get; set; }

        public string ReceiveDT { get; set; }
        public string ParkInitiateDT { get; set; }
        public string ParkDoneDT { get; set; }
        public string PostInitiateDT { get; set; }
        public string PostDoneDT { get; set; }
        public string ClearInitiateDT { get; set; }
        public string ClearDoneDT { get; set; }
        public string CheckPrintDT { get; set; }
        public string AuditInitDT { get; set; }
        public string AuditDoneDT { get; set; }
        public string SignInitDT { get; set; }
        public string SignDoneDT { get; set; }
        public string ApproveInitDT { get; set; }
        public string ApproveDoneDT { get; set; }
        public string DisburseInitDT { get; set; }
        public string DisburseDoneDT { get; set; }

        public string CurrentStage { get; set; }

        public string TotDays { get; set; }

        //.........for bar chart........

        public string value { get; set; }
        public string label { get; set; }
        public string color { get; set; }
        public string highlight { get; set; }

    }
}
