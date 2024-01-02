using SILDMS.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SILDMS.Web.UI.Controllers
{
    public class CommonController : Controller
    {
        private readonly string _userId;
        public async Task<dynamic> SearchMasterData(string masterDataTye)
        {
            using (var context = new CBPSEntities())
            {
                var TypeID = (from c in context.Sys_MasterDataType
                              where c.TypeName == masterDataTye
                              select c.MasterDataTypeID).FirstOrDefault();


                var list = (from c in context.Sys_MasterData
                            where c.MasterDataTypeID == TypeID & c.Status == 1
                            select new
                            {
                                MasterDataID = c.UDCode,
                                c.MasterDataValue
                            }).ToList();

                var defaultValue = (from c in context.Sys_MasterData
                    where c.MasterDataTypeID == TypeID &
                          c.Status == 1 & c.IsDefault == 1
                    select new
                    {
                        MasterDataID = c.UDCode,
                        c.MasterDataValue
                    }).FirstOrDefault();

                return Json(new { list, defaultValue }, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<dynamic> GetVendorCodedNameList()
        {
            using (var context = new CBPSEntities())
            {
                var list = (from c in context.CMS_Vendor.AsEnumerable()
                            
                            select new
                            {
                                c.VendorID,
                                c.VendorCode,
                                VendorName = c.VendorCode.TrimStart('0') + "- " + c.VendorName
                            }).ToList();

                
                return Json(list , JsonRequestBehavior.AllowGet);
            }
        }



    }
}