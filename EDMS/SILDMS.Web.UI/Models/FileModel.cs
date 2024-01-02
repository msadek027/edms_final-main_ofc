using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SILDMS.Web.UI.Models
{
    public class FileModel
    {
        //[Required(ErrorMessage = "Please select file.")]
        //[Display(Name = "Browse File")]
        public HttpPostedFileBase[] files { get; set; }
    }
}