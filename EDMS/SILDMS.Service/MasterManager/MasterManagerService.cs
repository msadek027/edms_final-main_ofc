using SILDMS.DataAccessInterface.MasterManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SILDMS.Model.DocScanningModule;
using SILDMS.Model.WorkflowModule;

namespace SILDMS.Service.MasterManager
{
    public class MasterManagerService : IMasterManagerService
    {
        private IMasterManagerDataService masterManagerDataService;
        public MasterManagerService(IMasterManagerDataService _masterManagerDataService)
        {
            this.masterManagerDataService = _masterManagerDataService;
        }

        public void AddItemToMaster(string action,MasterData obj, int masterId, string userID, out string res_code, out string res_message)
        {
            masterManagerDataService.AddItemToMaster(action,obj, masterId, userID,out res_code,out res_message);
        }

        public void GetMasterDataByTable(int id, string userID, out List<MasterData> objs)
        {
            objs = masterManagerDataService.GetMasterDataByTable(id,userID);
        }

        public void GetMasterForTableType(out List<MasterTableTracker> prm)
        {
            prm = masterManagerDataService.GetMasterForTableType();
        }
    }
}
