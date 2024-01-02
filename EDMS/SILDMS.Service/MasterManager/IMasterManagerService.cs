using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.Service.MasterManager
{
    public interface IMasterManagerService
    {
        void GetMasterDataByTable(int id, string userID, out List<MasterData> objs);
        void AddItemToMaster(string action,MasterData obj, int masterId, string userID, out string res_code, out string res_message);
        void GetMasterForTableType(out List<MasterTableTracker> prm);
    }
}
