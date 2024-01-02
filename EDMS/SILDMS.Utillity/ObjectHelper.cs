using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using System.Diagnostics.Contracts;

namespace SILDMS.Utillity
{
    public static class ObjectHelper
    {
        public static ExpandoObject ConvertToExpando(object obj)
        {
            //Get Properties Using Reflections
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            PropertyInfo[] properties = obj.GetType().GetProperties(flags);

            //Add Them to a new Expando
            ExpandoObject expando = new ExpandoObject();
            foreach (PropertyInfo property in properties)
            {
                AddProperty(expando, property.Name, property.GetValue(obj));
            }

            return expando;
        }

        public static ExpandoObject AddProperty(ExpandoObject expandoObject, string propertyName, object propertyValue)
        {
            var expandoDictionary = expandoObject as IDictionary<string, object>;

            if (expandoDictionary.ContainsKey(propertyName))
            {
                expandoDictionary[propertyName] = propertyValue;
            }
            else
            {
                expandoDictionary.Add(propertyName, propertyValue);
            }

            return (ExpandoObject)expandoDictionary;
        }

        public static ExpandoObject AddPropertyList(ExpandoObject expandoObject, List<string> propertyNameList, List<string> propertyValueList)
        {
            if (propertyNameList.Count != propertyValueList.Count)
            {
                return expandoObject;
            }
            else
            {
                var expandoDictionary = expandoObject as IDictionary<String, object>;

                for (int i = 0; i < propertyNameList.Count; i++)
                {
                    expandoObject = ObjectHelper.AddProperty(expandoObject, propertyNameList[i], propertyValueList[i]);

                    if (expandoDictionary.ContainsKey(propertyNameList[i]))
                    {
                        expandoDictionary[propertyNameList[i]] = propertyValueList[i];
                    }
                    else
                    {
                        expandoDictionary.Add(propertyNameList[i], propertyValueList[i]);
                    }
                }

                return (ExpandoObject)expandoDictionary;
            }
        }

        public static ExpandoObject RemoveProperty(ExpandoObject expandoObject, string propertyName)
        {
            var expandoDictionary = expandoObject as IDictionary<string, object>;

            if (expandoDictionary.ContainsKey(propertyName))
            {
                expandoDictionary.Remove(propertyName);
            }

            return (ExpandoObject)expandoDictionary;
        }

        public static ExpandoObject RemovePropertyList(ExpandoObject expandoObject, List<string> propertyNameList)
        {
            var expandoDictionary = expandoObject as IDictionary<String, object>;

            for (int i = 0; i < propertyNameList.Count; i++)
            {
                if (expandoDictionary.ContainsKey(propertyNameList[i]))
                {
                    expandoDictionary.Remove(propertyNameList[i]);
                }
            }

            return (ExpandoObject)expandoDictionary;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //public static dynamic ExpandObject<T>(T docType, List<string> targetAttribute, List<string> valueList)
        //{
        //    Type objectType = typeof(T);
        //    PropertyInfo[] objectPropertyList = typeof(T).GetProperties();

        //    dynamic docObject = new ExpandoObject() as IDictionary<string, Object>;
        //    int targetPosition = Array.IndexOf(objectPropertyList, targetAttribute);

        //    if (targetPosition > -1)
        //    {

        //    }

        //    int count = 0;
        //    for (int i = 0; i < objectPropertyList.Length; i++)
        //    {
        //        docObject.Add("NewProp", string.Empty);
        //    }


        //}



    }
}
