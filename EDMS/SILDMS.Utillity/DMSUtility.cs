using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SILDMS.Utillity
{
    public static class DMSUtility
    {
        public static DateTime FormatDate(string strDate)
        {
            var newDate = new DateTime();
            if (string.IsNullOrEmpty(strDate)) return newDate;
            var strDatepart = strDate.Substring(0, 10);
            if (strDatepart.Contains('/'))
            {
                var str = strDatepart.Split('/');
                newDate = new DateTime(Convert.ToInt32(str[2]), Convert.ToInt32(str[1]),
                    Convert.ToInt32(str[0]));
            }
            else if (strDatepart.Contains('-'))
            {
                var str = strDatepart.Split('-');
                newDate = new DateTime(Convert.ToInt32(str[2]), Convert.ToInt32(str[1]),
                    Convert.ToInt32(str[0]));
            }
            else
            {
                var str = Regex.Split(strDate, @"\s+");
                newDate = new DateTime(Convert.ToInt32(str[2]), ReturnMonthCode(str[1]),
                   Convert.ToInt32(str[0]));

            }
            return newDate;
        }

        public static int ReturnMonthCode(string name)
        {
            switch (name)
            {
                case "Jan":
                    return 1;

                case "Feb":
                    return 2;

                case "Mar":
                    return 3;

                case "Apr":
                    return 4;

                case "May":
                    return 5;

                case "Jun":
                    return 6;

                case "Jul":
                    return 7;

                case "Aug":
                    return 8;

                case "Sep":
                    return 9;

                case "Oct":
                    return 10;

                case "Nov":
                    return 11;

                case "Dec":
                    return 12;

                default:
                    return 0;

            }
        }

        public static string IdentifyPropertySeparator(string metaString,int propertyCount) {

            List<string> collection = metaString.Split(',').Select(sValue => sValue.Trim()).ToList();

            var chunkCount = collection.Count() / propertyCount;

            var chunks = new List<List<string>>();
            var count = 0;
            var temp = new List<string>();

            foreach (var element in collection)
            {
                if (count++ == chunkCount)
                {
                    chunks.Add(temp);
                    temp = new List<string>();
                    count = 1;
                }
                temp.Add(element);
            }
            chunks.Add(temp);

            string output = "";

            for (int j = 0; j < chunkCount; j++)
            {
                for (int i = 0; i < chunks.Count; i++)
                {
                    output = output + chunks[i][j];
                    if (i < chunks.Count - 1)
                    {
                        output = output + ", ";
                    }
                }
                output = output +"; ";
            }
            return output;
        }
      



    }
}
