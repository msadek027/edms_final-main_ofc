using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SILDMS.Utillity
{
    public class ValidationResult
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        public ValidationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="errorCode">Error code.</param>
        /// <param name="message">The message.</param>
        public ValidationResult(string errorCode, string message)
        {
            this.ErrorCode = errorCode;
            this.Message = message;

            this.Status = errorCode[1].ToString() == "2" ? "1" : "0";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public ValidationResult(string message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        /// <value>
        /// Error code.
        /// </value>
        public string ErrorCode { get; set; }
        public string Status { get; set; }
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public static ValidationResult Success
        {
            get
            {
                return null;
            }
        }

        public DataTable ToDataTable<T>(IList<T> data)
        {
            var props = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            for (var i = 0; i < props.Count; i++)
            {
                var prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            var values = new object[props.Count];
            foreach (var item in data)
            {
                for (var i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
        
        public string LoggID { get; set; }
        public string LoggAction { get; set; }
        public string LoggResult { get; set; }
    }
}
