using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.DataAccessInterface.MasterManager
{
    public interface IMasterManagerDataService
    {
        List<MasterData> GetMasterDataByTable(int id, string userID);
        void AddItemToMaster(string action, MasterData obj, int masterId, string userID, out string res_code, out string res_message);
        List<MasterTableTracker> GetMasterForTableType();
    }
}
