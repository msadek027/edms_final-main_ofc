using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using SILDMS.DataAccessInterface.OriginalDocSearching;
using SILDMS.Model.DocScanningModule;
using SILDMS.Utillity;
using SILDMS.Utillity.Localization;
using SILDMS.Web.UI.Models;
using System.Reflection;
using System.Text.RegularExpressions;
using System;
using System.Runtime.Remoting;

namespace SILDMS.Service.OriginalDocSearching
{
    public class OriginalDocSearchingService: IOriginalDocSearchingService
    {
        private readonly IOriginalDocSearchingDataService _originalDocSearchingDataService;
        private readonly ILocalizationService _localizationService;
        private string errorNumber = "";

        public OriginalDocSearchingService(IOriginalDocSearchingDataService originalDocSearchingDataService, ILocalizationService localizationService)
        {
            _originalDocSearchingDataService = originalDocSearchingDataService;
            _localizationService = localizationService;
        }

        public ValidationResult UpdateDocMetaInfo(DocumentsInfo _modelDocumentsInfo, string _UserID, out string outStatus)
        {
            _originalDocSearchingDataService.UpdateDocMetaInfo(_modelDocumentsInfo, _UserID, out outStatus);

            if (outStatus.Length > 0)
            {
                return new ValidationResult(outStatus, _localizationService.GetResource(outStatus));
            }

            return ValidationResult.Success;
        }
        public ValidationResult UpdateDocMailNotifyAndExpDate(string UserID,string DocID, string NotifyDate,  string ExpDate, out string outStatus)
        {
            _originalDocSearchingDataService.UpdateDocMailNotifyAndExpDate(UserID,DocID,  NotifyDate,  ExpDate,out outStatus);

            if (outStatus.Length > 0)
            {
                return new ValidationResult(outStatus, _localizationService.GetResource(outStatus));
            }


            return ValidationResult.Success;
        }

        public ValidationResult DeleteDocument(string _DocumentID, string _DocDistributionID, string _DocumentType, string _UserID, out string outStatus)
        {
            _originalDocSearchingDataService.DeleteDocument(_DocumentID, _DocDistributionID, _DocumentType, _UserID, out outStatus);
            return ValidationResult.Success;
        }

        public ValidationResult GetOriginalDocBySearchParam(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string _SearchBy, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out List<DocSearch> docList)
        {
            docList = _originalDocSearchingDataService.GetOriginalDocBySearchParam(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, _UserID, page, itemsPerPage, sortBy, reverse, attribute, search, out errorNumber);

            if (errorNumber.Length > 0)
            {
                return new ValidationResult(errorNumber, _localizationService.GetResource(errorNumber));
            }

            return ValidationResult.Success;
        }
       
        public ValidationResult GetOriginalDocBySearchParamV2(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string _SearchBy, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, string searchType, out List<dynamic> finalDocList, out List<string> finalAttributeList)
        {
            finalDocList = new List<dynamic>();
            finalAttributeList = new List<string>();

            //var attributeCount = DataHelper.DataCount(DataHelper.GetData(DataHelper.GenerateQueryForDocumentPropertyAttributeData(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID)));
            //itemsPerPage = attributeCount > 1 ? (attributeCount * itemsPerPage) : itemsPerPage;

            List<Model.DocScanningModule.DocSearch> originalDocList = _originalDocSearchingDataService.GetOriginalDocBySearchParamV2(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, _UserID, page, itemsPerPage, sortBy, reverse, attribute, search, searchType, out errorNumber);

            if (errorNumber.Length > 0)
            {
                return new ValidationResult(errorNumber, _localizationService.GetResource(errorNumber));
            }

            #region Previous
            //List<string> docPropertyAttributeNameList = originalDocList.SelectMany(s => s.DocPropIdentifyName.Replace(" ", string.Empty).Split(',')).Distinct().ToList();
            //finalAttributeList = docPropertyAttributeNameList;
            ////List<string> docPropertyAttributeValueList = originalDocList.SelectMany(s => s.MetaValue.Split(',')).Distinct().ToList();

            //for (int i = 0; i < originalDocList.Count; i++)
            //{
            //    dynamic expandoObject = ObjectHelper.ConvertToExpando(originalDocList[i]);

            //    List<string> selectedDocPropertyAttributeNameList = originalDocList[i].DocPropIdentifyName.Replace(" ", string.Empty).Split(',').ToList();  //Remove space from DocPropIdentifyName
            //    List<string> selectedDocPropertyAttributeValueList = originalDocList[i].MetaValue.Split(',').ToList();                                      //Get Array from MetaValue separated by Comma(,)

            //    expandoObject = ObjectHelper.AddPropertyList(expandoObject, selectedDocPropertyAttributeNameList, selectedDocPropertyAttributeValueList);
            //    finalDocList.Add(expandoObject);
            //}
            #endregion

            #region New
            //List<string> docPropertyAttributeNameList = originalDocList.Select(s => Regex.Replace(s.DocPropIdentifyName, @"\s+", "")).Distinct().ToList();
            //finalAttributeList = docPropertyAttributeNameList;

            //for (int i = 0; i < originalDocList.Count; i++)
            //{
            //    dynamic finalDoc = ObjectHelper.ConvertToExpando(originalDocList[i]);

            //    if (!finalDocList.Any(a => a.DocumentID == originalDocList[i].DocumentID))                                                                                          //If Object does not exist in Final List
            //    {
            //        finalDoc = ObjectHelper.AddPropertyList(finalDoc, docPropertyAttributeNameList, Enumerable.Repeat(string.Empty, docPropertyAttributeNameList.Count).ToList());  //Add Attribute as Property
            //        finalDoc = ObjectHelper.AddProperty(finalDoc, Regex.Replace(originalDocList[i].DocPropIdentifyName, @"\s+", ""), originalDocList[i].MetaValue);                 //Update Attribute Value

            //        finalDocList.Add(finalDoc);                                                                                                                                     //Insert Object in Final List
            //    }
            //    else                                                                                                                                                                //If Object exists in Final List
            //    {
            //        int index = finalDocList.FindIndex(a => a.DocumentID == originalDocList[i].DocumentID);                                                                         //Get Object Index Value from Final List

            //        if (docPropertyAttributeNameList.Any(a => a == Regex.Replace(originalDocList[i].DocPropIdentifyName, @"\s+", "")))                                              //If Value needs to be updated in Object in Final List
            //        {
            //            finalDoc = finalDocList.ElementAt(index);                                                                                                                   //Grab Object from Final List
            //            finalDocList.RemoveAt(index);                                                                                                                               //Remove Object from Final List

            //            finalDoc = ObjectHelper.AddProperty(finalDoc, Regex.Replace(originalDocList[i].DocPropIdentifyName, @"\s+", ""), originalDocList[i].MetaValue);             //Update Attribute Value
            //            finalDocList.Insert(index, finalDoc);                                                                                                                       //Insert Object in same position as Previous in Final List
            //        }
            //    }
            //}
            #endregion
            
            if (originalDocList.Count > 0)
            {
                List<string> docPropertyAttributeNameList = originalDocList.Select(s => Regex.Replace(s.DocPropIdentifyName, @"\s+", "")).Distinct().ToList();
                finalAttributeList = docPropertyAttributeNameList;

                List<string> docIdList = originalDocList.Select(s => s.DocumentID).Distinct().ToList();

                foreach (var docId in docIdList)
                {
                    var matchedDocList = originalDocList.Where(w => w.DocumentID == docId).ToList();

                    #region NotWorkingFrontend
                    //ExpandoObject finalDoc = new ExpandoObject();
                    //finalDoc = ObjectHelper.AddProperty(finalDoc, "DocumentID", matchedDocList.FirstOrDefault().DocumentID);
                    //finalDoc = ObjectHelper.AddProperty(finalDoc, "DocDistributionID", matchedDocList.FirstOrDefault().DocDistributionID);
                    //finalDoc = ObjectHelper.AddPropertyList(finalDoc, docPropertyAttributeNameList, Enumerable.Repeat(string.Empty, docPropertyAttributeNameList.Count).ToList());
                    //finalDoc = ObjectHelper.AddProperty(finalDoc, "FileExtenstion", matchedDocList.FirstOrDefault().FileExtenstion);
                    //finalDoc = ObjectHelper.AddProperty(finalDoc, "ServerIP", matchedDocList.FirstOrDefault().ServerIP);
                    //finalDoc = ObjectHelper.AddProperty(finalDoc, "ServerPort", matchedDocList.FirstOrDefault().ServerPort);
                    //finalDoc = ObjectHelper.AddProperty(finalDoc, "FtpUserName", matchedDocList.FirstOrDefault().FtpUserName);
                    //finalDoc = ObjectHelper.AddProperty(finalDoc, "FtpPassword", matchedDocList.FirstOrDefault().FtpPassword);
                    //finalDoc = ObjectHelper.AddProperty(finalDoc, "FileServerURL", matchedDocList.FirstOrDefault().FileServerURL);
                    #endregion

                    if (matchedDocList.Count > 0)
                    {
                        dynamic finalDocStructure = ObjectHelper.ConvertToExpando(matchedDocList.FirstOrDefault());
                        finalDocStructure = ObjectHelper.AddPropertyList(finalDocStructure, docPropertyAttributeNameList, Enumerable.Repeat(string.Empty, docPropertyAttributeNameList.Count).ToList());

                        foreach (var docPropertyAttributeName in docPropertyAttributeNameList)
                        {
                            var item = matchedDocList.Where(w => Regex.Replace(w.DocPropIdentifyName, @"\s+", "") == docPropertyAttributeName).FirstOrDefault();
                            //finalDocStructure = ObjectHelper.AddProperty(finalDocStructure, docPropertyAttributeName, item.MetaValue != null ? item.MetaValue : "");

                            if (item != null)
                            {
                                finalDocStructure = ObjectHelper.AddProperty(finalDocStructure, docPropertyAttributeName, item.MetaValue);
                            }
                            else
                            {
                                finalDocStructure = ObjectHelper.AddProperty(finalDocStructure, docPropertyAttributeName, "");
                            }
                        }

                        finalDocList.Add(finalDocStructure);
                    }
                }
            }

            #region hahaha
            //var recordList = new List<dynamic>();
            //foreach (var finalDoc in finalDocList)
            //{
            //    dynamic expando = new ExpandoObject();
            //    var tempObject = expando as IDictionary<String, object>;

            //    foreach (var item in finalDoc)
            //    {
            //        var propertyName = item.Key;
            //        var propertyValue = item.Value;

            //        tempObject["" + propertyName + ""] = propertyValue;
            //    }

            //    recordList.Add(expando);
            //}

            //finalDocList = recordList;
            #endregion

            return ValidationResult.Success;
        }

        public ValidationResult GetDocumentsByWildSearch(string _SearchFor, string _UserID, out List<Model.DocScanningModule.DocSearch> docList)
        {
            docList = _originalDocSearchingDataService.GetOriginalDocumentsByWildSearch(_SearchFor, _UserID, out errorNumber);

            if (errorNumber.Length > 0)
            {
                return new ValidationResult(errorNumber, _localizationService.GetResource(errorNumber));
            }

            return ValidationResult.Success;
        }

        public ValidationResult GetOriginalDocMeta(string _DocumentId, string _DocDistributionID, out List<OriginalDocMeta> metaList)
        {
            metaList = _originalDocSearchingDataService.GetOriginalDocMeta(_DocumentId, _DocDistributionID, out errorNumber);

            if (errorNumber.Length > 0)
            {
                return new ValidationResult(errorNumber, _localizationService.GetResource(errorNumber));
            }

            return ValidationResult.Success;
        }

        public ValidationResult GetDocumentsBySearchParamForVersion(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string _SearchBy, string _UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out List<DocSearch> docList)
        {
            docList = _originalDocSearchingDataService.GetDocumentsBySearchParamForVersion(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, _UserID, page, itemsPerPage, sortBy, reverse, attribute, search, out errorNumber);

            if (errorNumber.Length > 0)
            {
                return new ValidationResult(errorNumber, _localizationService.GetResource(errorNumber));
            }

            return ValidationResult.Success;
        }

        public ValidationResult GetOriginalDocBySearchFromList(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string _SearchBy, string value, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out List<DocSearch> docList)
        {
            docList = _originalDocSearchingDataService.GetOriginalDocBySearchFromList(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy,value, UserID, page, itemsPerPage, sortBy, reverse, attribute, search, out errorNumber);

            if (errorNumber.Length > 0)
            {
                return new ValidationResult(errorNumber, _localizationService.GetResource(errorNumber));
            }

            return ValidationResult.Success;
        }

        public ValidationResult GetOriginalDocBySearchForPrint(string _OwnerID, string _DocCategoryID, string _DocTypeID, string _DocPropertyID, string _SearchBy, string Docs, string UserID, int page, int itemsPerPage, string sortBy, bool reverse, string attribute, string search, out List<DocSearch> docList)
        {
            docList = _originalDocSearchingDataService.GetOriginalDocBySearchForPrint(_OwnerID, _DocCategoryID, _DocTypeID, _DocPropertyID, _SearchBy, Docs, UserID, page, itemsPerPage, sortBy, reverse, attribute, search, out errorNumber);

            if (errorNumber.Length > 0)
            {
                return new ValidationResult(errorNumber, _localizationService.GetResource(errorNumber));
            }

            return ValidationResult.Success;
        }
        public ValidationResult GetMailNotifyAndExpDate(string DocumentId, out DSM_VM_Property NotifyAndExpDate)
        {
            
            NotifyAndExpDate=_originalDocSearchingDataService.GetMailNotifyAndExpDate( DocumentId);

            if (NotifyAndExpDate==null)
            {
                return new ValidationResult(errorNumber, _localizationService.GetResource(errorNumber));
            }

            return ValidationResult.Success;

        }

    }
}
