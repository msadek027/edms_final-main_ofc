app.controller('DocumentDistributionSharingCtr', ['$scope', '$http', 'ImageProcessServices', 'environmentServices', '$compile', '$injector', '$interval', function ($scope, $http, ImageProcessServices, environmentServices, $compile, $injector, $interval) {

    'use strict'
    $scope.Zoom_Count = .5;
    applySecurity();
    $scope.PDF_Images = [];
    $scope.last_page = 0;
    $scope.file_list = [];
    $scope.Zoom_Count = .5;
    $scope.Page_Rotation = [];
    $scope.Blank_Page_Count = [];
    $scope.Uploadable_files = [];
    $scope.Total_Page = 0;
    $scope.Url_Ref = null;
    $scope.docPropIdentityGridData2 = [];
    $scope.btnDistribute = false;

    var selectedValues = "";
    $http.get('/DocScanningModule/DocDistribution/GetEmployee')
        .success(function (response) {
            // Assuming the response is an array of objects with 'id' and 'name' properties
            debugger;
            $scope.items = response;       
            $(document).ready(function () {            
                $('#employeeSelect').select2({
                    placeholder: '--Multi Select--', // Set your placeholder text
                });
                $('#employeeSelect').on('change', function () {
                     selectedValues = $(this).val(); // Get an array of selected values
                    console.log('Selected values: ', selectedValues);
                });
            });
        })
        .catch(function (error) {
            console.error('Error loading data:', error);
        });

   
    //$scope.ZoomIn = function () {
     
    //    var doc_view_ref = document.getElementById('Large-pdf-Viewer');
    //    var dz = parseFloat($scope.Zoom_Count) + parseFloat('0.2');
    //    doc_view_ref.style = 'zoom: ' + dz;
    //    $scope.Zoom_Count = dz;
    //};

    //$scope.ZoomOut = function () {
      
    //    var doc_view_ref = document.getElementById('Large-pdf-Viewer');
    //    var dk = parseFloat($scope.Zoom_Count) - parseFloat('0.2');
    //    doc_view_ref.style = 'zoom: ' + dk;
    //    $scope.Zoom_Count = dk;
    //};



    var pdfjsLib = window['pdfjs-dist/build/pdf'];
    pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.6.347/pdf.worker.min.js';
    var pdfDoc = null;
    var scale = 1; //Set Scale for zooming PDF.
    var resolution = 1; //Set Resolution to Adjust PDF clarity.


    $scope.LoadImage = function (tableRow, e) {

        debugger;
        var selectedItem = $scope.GridDisplayCollection.find(function (item) {
            return item === tableRow;
        });
        if (selectedItem) {
            selectedItem.isSelected = true;// !selectedItem.isSelected;
        }

        $scope.loading = true;
        $(".mydiv").show();
        $scope.btnDistribute = true;
        $scope.docPropIdentityGridData2 = tableRow;  

        if ($scope.docPropIdentityModel.DidtributionOf === 'Original') {

           var url = "DocScanningModule/MultiDocScan/GetFilePassWord_r?" + encodeURIComponent("serverIP") + "=" + encodeURIComponent(tableRow.ServerIP)
                + "&" + encodeURIComponent("ftpPort") + "=" + encodeURIComponent(tableRow.ServerPort)
                + "&" + encodeURIComponent("ftpUserName") + "=" + encodeURIComponent(tableRow.FtpUserName)
                + "&" + encodeURIComponent("ftpPassword") + "=" + encodeURIComponent(tableRow.FtpPassword)
                + "&" + encodeURIComponent("serverURL") + "=" + encodeURIComponent(tableRow.FileServerURL)
                + "&" + encodeURIComponent("documentID") + "=" + encodeURIComponent(tableRow.DocumentID)
                + "&" + encodeURIComponent("isObsolete") + "=" + encodeURIComponent(false)
                + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent(false)

            ImageProcessServices.detailView(url).then(function (data) {
                debugger
                var arrayBuffer = data.data;
                var data = { data: arrayBuffer }
                $scope.Url_Ref = data;
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
             //   $scope.LoadPdfFromUrl(data);
                $scope.LargeViewer(data)

                $scope.loading = false;
                $(".mydiv").hide();
            }, function (error) {
                console.log(error.data);
                alert(error.data);

                $scope.loading = false;
                $(".mydiv").hide();


            });

        }
        else if ($scope.docPropIdentityModel.DidtributionOf === 'Version') {
            var DocVersion = tableRow.DocVersionID + "_v_" + tableRow.VersionNo;

            var url = "DocScanningModule/MultiDocScan/GetFilePassWord_r?" + encodeURIComponent("serverIP") + "=" + encodeURIComponent(tableRow.ServerIP)
                + "&" + encodeURIComponent("ftpPort") + "=" + encodeURIComponent(tableRow.ServerPort)
                + "&" + encodeURIComponent("ftpUserName") + "=" + encodeURIComponent(tableRow.FtpUserName)
                + "&" + encodeURIComponent("ftpPassword") + "=" + encodeURIComponent(tableRow.FtpPassword)
                + "&" + encodeURIComponent("serverURL") + "=" + encodeURIComponent(tableRow.FileServerURL)
                + "&" + encodeURIComponent("documentID") + "=" + encodeURIComponent(DocVersion)
                + "&" + encodeURIComponent("isObsolete") + "=" + encodeURIComponent(false)
                + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent(false)

            ImageProcessServices.detailView(url).then(function (data) {
                debugger
                var arrayBuffer = data.data;
                var data = { data: arrayBuffer }
                $scope.Url_Ref = data;
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
               // $scope.LoadPdfFromUrl(data);     
                $scope.LargeViewer(data)

                $scope.loading = false;
                $(".mydiv").hide();
            }, function (error) {
                console.log(error.data);
                alert(error.data);

                $scope.loading = false;
                $(".mydiv").hide();


            });
        }
    };

    $scope.SelectAll = function () {
        debugger;

        if ($scope.selectAllChecked == true) {
            $scope.GridDisplayCollection.forEach(function (item) {
                // Add a new key "newKey" with a default value (you can set it to whatever you need)
                // item['chk' + item.DocumentID] = true;
                item.isSelected = true;
                //  item.isSelected = !item.isSelected;   
            });
            $scope.btnDistribute = true;
        }
        else {
            $scope.GridDisplayCollection.forEach(function (item) {
                // Add a new key "newKey" with a default value (you can set it to whatever you need)
                // item['chk' + item.DocumentID] = true;
                item.isSelected = false;
                //  item.isSelected = !item.isSelected;   
            });
            $scope.btnDistribute = false;
        }
        console.log($scope.GridDisplayCollection)
    };
    $scope.checkboxRowClicked = function (row) {
        // Do something with the clicked row, for example, log the selected state
        console.log(row.isSelected);
        $scope.selectAllChecked = false;

        var selectedItems = $scope.GridDisplayCollection.filter(function (item) {
            return item.isSelected;
        });
        if (selectedItems.length > 0) {
            $scope.btnDistribute = true;
        }
        else {
            $scope.btnDistribute = false;
        }
    };

    $scope.docPropIdentityModel = {
        OwnerLevel: { OwnerLevelID: "", LevelName: "" },
        Owner: { OwnerID: "", OwnerName: "" },
        DocCategory: { DocCategoryID: "", DocCategoryName: "" },
        DocType: { DocTypeID: "", DocTypeName: "" },
        DocProperty: { DocPropertyID: "", DocPropertyName: "" },
        DocMetaID: "",
        DocPropIdentifyID: "",
        MetaValue: "",
        Remarks: "",
        IdentificationAttribute: "",
        AttributeValue: "",
        SearchBy: "",
        DidtributionOf: "",
        DocVersionID: ""
    };

    $scope.pagingInfo = {
        page: 1,
        itemsPerPage: 20,
        sortBy: null,
        reverse: false,
        search: null,
        totalItems: 0
    };

    $scope.selectPage = function () {
        $scope.BindDataToGrid();
    };

    $scope.AttrName = '';
    $scope.AttrValue = '';

    $scope.search = function () {
        $scope.pagingInfo.page = 1;
        $scope.BindDataToGrid();
    };

    //$http.get('/DocScanningModule/OwnerProperIdentity/GetOwnerLevel?_OwnerLevelID=')
    //    .success(function (response) {
    //        $scope.ownerLevels = response.result;
    //        $scope.docPropIdentityModel.OwnerLevel = "";
    //        $scope.docPropIdentityModel.Owner = "";
    //        $scope.docPropIdentityModel.DocCategory = "";
    //        $scope.docPropIdentityModel.DocType = "";
    //        $scope.docPropIdentityModel.DocProperty = "";
    //        $scope.loading = false;
    //    })
    //    .error(function () {
    //        $scope.loading = false;
    //    });

    //$scope.loadOwner = function () {
    //    $scope.docPropIdentityModel.Owner = "";
    //    $scope.docPropIdentityModel.DocCategory = "";
    //    $scope.docPropIdentityModel.DocType = "";
    //    $scope.docPropIdentityModel.DocProperty = "";
    //    $scope.docPropertyForSpecificDocType = "";
    //    $scope.docPropIdentityGridData = [];

    //    $http.post('/DocScanningModule/OwnerProperIdentity/GetOwnerForSpecificOwnerLevel',
    //        { _OwnerLevelID: $scope.docPropIdentityModel.OwnerLevel.OwnerLevelID })
    //        .success(function (response) {
    //            $scope.ownersForSpecificOwnerLevel = response.result;
    //            $scope.loading = false;
    //        }).error(function () {
    //            $scope.loading = false;
    //        });
    //};

    //$scope.loadCategory = function () {
    //    $scope.docPropIdentityModel.DocCategory = "";
    //    $scope.docPropIdentityModel.DocType = "";
    //    $scope.docPropIdentityModel.DocProperty = "";
    //    $scope.docPropertyForSpecificDocType = "";
    //    $scope.docPropIdentityGridData = [];

    //    $http.post('/DocScanningModule/OwnerProperIdentity/GetDocumentCategoriesForSpecificOwner',
    //        { _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID })
    //        .success(function (response) {
    //            $scope.docCategoriesForSpecificOwner = response.result;
    //            $scope.loading = false;
    //        }).error(function () {
    //            $scope.loading = false;
    //        });
    //};

    //$scope.loadType = function () {
    //    $scope.docPropIdentityModel.DocType = "";
    //    $scope.docPropIdentityModel.DocProperty = "";
    //    $scope.docPropertyForSpecificDocType = "";
    //    $scope.docPropIdentityGridData = [];

    //    $http.post('/DocScanningModule/OwnerProperIdentity/GetDocumentTypeForSpecificDocCategory',
    //        {
    //            _DocCategoryID: $scope.docPropIdentityModel.DocCategory.DocCategoryID,
    //            _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID
    //        })
    //        .success(function (response) {
    //            $scope.docTypeForSpecificDocCategory = response.result;
    //            $scope.loading = false;
    //        }).error(function () {
    //            $scope.loading = false;
    //        });
    //}
    $http.get('/DocScanningModule/OwnerProperIdentity/GetOwnerLevel?_OwnerLevelID=')
        .success(function (response) {
            $scope.ownerLevels = response.result;
            //$scope.docPropIdentityModel.OwnerLevel = "";
            $scope.docPropIdentityModel.OwnerLevel = response.result[0];
            $scope.loading = false;
        })
        .error(function () {
            $scope.loading = false;
        });
    $scope.$watch('docPropIdentityModel.OwnerLevel', function (newVal, oldVal) {
        if (newVal) {
            if (newVal != "" && newVal != oldVal) {
                $scope.docPropIdentityModel.Owner = "";
                $scope.docPropIdentityModel.DocCategory = "";
                $scope.docPropIdentityModel.DocType = "";
                $scope.docPropIdentityModel.DocProperty = "";
                $scope.docPropIdentityModel.SearchBy = "";

                $http.post('/DocScanningModule/OwnerProperIdentity/GetOwnerForSpecificOwnerLevel',
                    { _OwnerLevelID: $scope.docPropIdentityModel.OwnerLevel.OwnerLevelID })
                    .success(function (response) {
                        $scope.ownersForSpecificOwnerLevel = response.result;
                        $scope.docPropIdentityModel.Owner = response.result[0];
                        $scope.loading = false;
                    }).error(function () {
                        $scope.loading = false;
                    });
            }
        }
    });

    $scope.$watch('docPropIdentityModel.Owner', function (newVal) {
        if (newVal) {
            $scope.docPropIdentityModel.DocCategory = "";
            $scope.docPropIdentityModel.DocType = "";
            $scope.docPropIdentityModel.DocProperty = "";
            $scope.docPropIdentityModel.SearchBy = "";
            $http.post('/DocScanningModule/OwnerProperIdentity/GetDocumentCategoriesForSpecificOwner',
                { _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID })
                .success(function (response) {
                    $scope.docCategoriesForSpecificOwner = response.result;
                    $scope.loading = false;
                }).error(function () {
                    $scope.loading = false;
                });
        }
    });

    $scope.$watch('docPropIdentityModel.DocCategory', function (newVal) {
        if (newVal) {
            $scope.docPropIdentityModel.DocType = "";
            $scope.docPropIdentityModel.DocProperty = "";
            $scope.docPropIdentityModel.SearchBy = "";
            $http.post('/DocScanningModule/OwnerProperIdentity/GetDocumentTypeForSpecificDocCategory',
                {
                    _DocCategoryID: $scope.docPropIdentityModel.DocCategory.DocCategoryID,
                    _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID
                })
                .success(function (response) {
                    $scope.docTypeForSpecificDocCategory = response.result;
                    $scope.loading = false;
                }).error(function () {
                    $scope.loading = false;
                });
        }
    });
    $scope.$watch('docPropIdentityModel.DocType', function (newVal) {
        if (newVal) {
            $scope.docPropIdentityModel.DocProperty = "";
            $scope.docPropIdentityModel.SearchBy = "";

            $http.post('/DocScanningModule/MultiDocScan/GetDocumentProperty',
                {
                    _DocCategoryID: $scope.docPropIdentityModel.DocCategory.DocCategoryID,
                    _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                    _DocTypeID: $scope.docPropIdentityModel.DocType.DocTypeID
                })
                .success(function (response) {
                    $scope.docPropertyForSpecificDocType = response.result;
                    $scope.loading = false;
                    //$scope.BindDataToGrid();
                }).error(function () {
                    $scope.loading = false;
                });
        }
    });
    $scope.loadPropert = function () {
        $scope.docPropIdentityModel.DocProperty = "";
        $scope.docPropertyForSpecificDocType = "";
        $scope.docPropIdentityGridData = [];

        $http.post('/DocScanningModule/MultiDocScan/GetDocumentProperty',
            {
                _DocCategoryID: $scope.docPropIdentityModel.DocCategory.DocCategoryID,
                _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                _DocTypeID: $scope.docPropIdentityModel.DocType.DocTypeID
            })
            .success(function (response) {
                $scope.docPropertyForSpecificDocType = response.result;
                $scope.loading = false;
                //$scope.BindDataToGrid();
            }).error(function () {
                $scope.loading = false;
            });
    }
    /*
    $scope.loadPropertyIdentify = function () {
        $('#dwtHorizontalThumbnil').empty();
        $scope.BindDataToGrid();
    }

    $scope.BindDataToGrid = function () {
        if ($scope.docPropIdentityModel.DidtributionOf === "Original") {
            $scope.loading = true;
            $http.post('/DocScanningModule/OriginalDocSearching/GetDocumentsBySearchParam',
                {
                    _DocCategoryID: $scope.docPropIdentityModel.DocCategory.DocCategoryID,
                    _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                    _DocTypeID: $scope.docPropIdentityModel.DocType.DocTypeID,
                    _DocPropertyID: $scope.docPropIdentityModel.DocProperty.DocPropertyID,
                    _SearchBy: $scope.docPropIdentityModel.SearchBy,
                    page: $scope.pagingInfo.page,
                    itemsPerPage: $scope.pagingInfo.itemsPerPage,
                    sortBy: $scope.pagingInfo.itemsPerPage,
                    reverse: $scope.pagingInfo.reverse,
                    attribute: $scope.AttrName,
                    search: $scope.AttrName == '' ? $scope.pagingInfo.search : $scope.AttrValue
                })
                .success(function (pageable) {
                    $scope.GridDisplayCollection = pageable.lstDocSearch;
                    $scope.pagingInfo.totalItems = pageable.totalPages;
                    $scope.loading = false;
                }).error(function () {
                    $scope.loading = false;
                });
        } else if ($scope.docPropIdentityModel.DidtributionOf === "Version") {
            $scope.loading = true;
            $http.post('/DocScanningModule/DocDistribution/GetVersionDocBySearchParam',
                {
                    _DocCategoryID: $scope.docPropIdentityModel.DocCategory.DocCategoryID,
                    _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                    _DocTypeID: $scope.docPropIdentityModel.DocType.DocTypeID,
                    _DocPropertyID: $scope.docPropIdentityModel.DocProperty.DocPropertyID,
                    _SearchBy: $scope.docPropIdentityModel.SearchBy,
                    page: $scope.pagingInfo.page,
                    itemsPerPage: $scope.pagingInfo.itemsPerPage,
                    sortBy: $scope.pagingInfo.itemsPerPage,
                    reverse: $scope.pagingInfo.reverse,
                    search: $scope.pagingInfo.search
                })
                .success(function (pageable) {
                    $scope.GridDisplayCollection = pageable.content;
                    $scope.pagingInfo.totalItems = pageable.totalPages;
                    $scope.loading = false;
                }).error(function () {
                    $scope.loading = false;
                });
        }
    };
    */
    $scope.loadPropertyIdentify = function (newVal) {
     //   $('#dwtHorizontalThumbnil').empty();
        $('#dwtLargeViewer').empty();
        $('#dwtVerticalThumbnil').empty();
        $('#dwtLargeViewer').css('min-height', '0');
        $('#dwtLargeViewer').css('max-height', '0');

        $('#dwtVerticalThumbnil').css('min-height', '0');
        $('#dwtVerticalThumbnil').css('max-height', '0');
        $scope.BindDataToGrid();
    };

    $scope.BindDataToGrid = function () {

        $scope.loading = true;
        $(".mydiv").show();
        if ($scope.docPropIdentityModel.DidtributionOf === "Original") {
         
            $http.post('/DocScanningModule/DocDistribution/GetDocPropIdentityForSelectedDocTypes',
                {
                    _DocCategoryID: $scope.docPropIdentityModel.DocCategory.DocCategoryID,
                    _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                    _DocTypeID: $scope.docPropIdentityModel.DocType.DocTypeID,
                    _DocPropertyID: $scope.docPropIdentityModel.DocProperty.DocPropertyID,
                    _SearchBy: $scope.docPropIdentityModel.SearchBy
                })
                .success(function (response) {
                    $scope.docPropIdentityGridData = response;
                    $scope.GridDisplayCollection = response.result;

                    $scope.loading = false;
                    $(".mydiv").hide();
                }).error(function () {

                    $scope.loading = false;
                    $(".mydiv").hide();
                });
        }
        else if ($scope.docPropIdentityModel.DidtributionOf  === "Version") {

            $http.post('/DocScanningModule/VersionDocSearching/GetVersionDocBySearchParam',
                {
                    _DocCategoryID: $scope.docPropIdentityModel.DocCategory.DocCategoryID,
                    _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                    _DocTypeID: $scope.docPropIdentityModel.DocType.DocTypeID,
                    _DocPropertyID: $scope.docPropIdentityModel.DocProperty.DocPropertyID,
                    _SearchBy: $scope.docPropIdentityModel.SearchBy
                })
                .success(function (response) {
                    $scope.docPropIdentityGridData = response.result;
                    $scope.GridDisplayCollection = response.result;
                    console.log($scope.GridDisplayCollection);

                    $scope.loading = false;
                    $(".mydiv").hide();
                }).error(function () {

                    $scope.loading = false;
                    $(".mydiv").hide();
                });
        }
    };

    $scope.toggleSingleEmail = function (row) {   
        debugger;
        $scope.docPropIdentityGridData2 = row;

        var selectedItem = $scope.GridDisplayCollection.find(function (item) {
            return item === row;
        });
        if (selectedItem) {
            selectedItem.isSelected = true;// !selectedItem.isSelected;
        }


     //   $('#addModal').modal('show');

        $scope.ServerIP = row.ServerIP;
        $scope.FileServerURL = row.FileServerURL;
        $scope.FtpUserName = row.FtpUserName;
        $scope.FtpPassword = row.FtpPassword;
        $scope.DocumentID = row.DocumentID;
        var selectedItems = $scope.GridDisplayCollection.filter(function (item) {
            return item.isSelected;
        });
        console.log(selectedItems);
        if (selectedItems.length > 0) {
            $('#addModal').modal('show');
        }
        else {
            toastr.error('please select al least one row');
        }
    };
    $scope.toggleAllEmail = function () {
        debugger;
 

        var selectedItems = $scope.GridDisplayCollection.filter(function (item) {
            return item.isSelected;
        });
        console.log(selectedItems);
        if (selectedItems.length > 0) {
            $('#addModal').modal('show');
        }
        else {
            toastr.error('please select al least one row');
        }
    };
    $scope.SendMail = function () {
        debugger;
        var selectedItems = $scope.GridDisplayCollection.filter(function (item) {
            return item.isSelected;
        });
        $scope.loading = true;
        $(".mydiv").show();
     
        $http.post('/DocScanningModule/DocSharing/SendMailWithDownloadLink',
            {
                ToEmail: $scope.To,
                ccAddress: $scope.CC,
                bccAddress: $scope.BCC,
                Subj: $scope.Subject,
                Message: $scope.Message,
          
                modelDocumentsInfo: $scope.docPropIdentityModel,            
                docMetaValues: selectedItems,// $scope.docPropIdentityGridData2,
                _OwnerLevelID: $scope.docPropIdentityModel.OwnerLevel.OwnerLevelID,
                _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                _DocCategoryID: $scope.docPropIdentityModel.DocCategory.DocCategoryID,
                _DocTypeID: $scope.docPropIdentityModel.DocType.DocTypeID,
                _DocPropertyID: $scope.docPropIdentityModel.DocProperty.DocPropertyID,

            })
            .success(function (response) {
               // $scope.docPropIdentityModel = [];
                $scope.To = "";
                $scope.CC = "";
                $scope.BCC = "";
                $scope.Subject = "";
                $scope.Message = "";

                //console.log(response);
                $('#addModal').modal('hide');

                if (response.Msg == '2') {
                    toastr.success('Mail send successfully.');
                }
                else {
                    toastr.error('Error: Faild to send email !!!');
                }


                $scope.loading = false;
                $(".mydiv").hide();
            }).error(function () {
                $('#addModal').modal('hide');
                toastr.error('Error: Faild to send email !!!');


                $scope.loading = false;
                $(".mydiv").hide();
            });
    };

    $scope.ResetModel = function () {
    };
    $scope.CloseModalView = function (modalWindowId) {
        $scope.loading = false;
        $('#' + modalWindowId).modal('hide');
    };
    $scope.showPDF = function () {

        const blobUrl = URL.createObjectURL($scope.PDF_Images);
        window.open().document.write('<iframe src="' + blobUrl + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');
    }
  
    $scope.Save = function () {   
        debugger;
        $scope.loading = true;
        if ($scope.selectedItem == null || $scope.selectedItem == "--Select--") {
            $scope.loading = false;
            toastr.warning("Please enter the values according to the fields.");
            return;
        }
        selectedValues = JSON.stringify(selectedValues);
        $http.post('/DocScanningModule/DocDistribution/AddDocumentInfoWithUser',
            {
                modelDocumentsInfo: $scope.docPropIdentityModel,
                selectedPropId: $scope.docPropIdentityModel.DocProperty.DocPropertyID,
                docMetaValues: $scope.docPropIdentityGridData2,
                employeeId: selectedValues, // $scope.selectedItem,
                _OwnerLevelID: $scope.docPropIdentityModel.OwnerLevel.OwnerLevelID,
                _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                _DocCategoryID: $scope.docPropIdentityModel.DocCategory.DocCategoryID,           
                _DocTypeID: $scope.docPropIdentityModel.DocType.DocTypeID,
                _DocPropertyID: $scope.docPropIdentityModel.DocProperty.DocPropertyID,
               

            })
            .success(function (response) {
                $scope.loading = false;
                if (response.Message === "No column name match with property name. Please upload valid excel file.") {
                    toastr.error(response.Message);
                    $scope.btnDistribute = true;
                }
                else if (response.Message == null) {
                    $scope.btnDistribute = true;
                    toastr.error("Error Occured.");
                }
                else if (response.Message === "Saved Successfully.") {
                    toastr.success(response.Message);
                    $scope.btnDistribute = false;
                    $scope.docPropIdentityModel = [];
                    $scope.docPropIdentityGridData = [];
                    $scope.GridDisplayCollection = [];

                    $('#dwtHorizontalThumbnil').empty();
                     $('#employeeSelect').val([]).trigger('change');
                    $scope.pagingInfo.page = 1;
                    $scope.pagingInfo.totalItems = 0;
                   
                }
            })
            .error(function () {
                $scope.loading = false;
                toastr.error("Failed to Save Meta Data.");
            });
    };

    $scope.toggleRefreshTable = function (row) {
        location.reload();
    };


    //$scope.LoadPdfFromUrl = function (url) {
    //    $scope.Total_Page = 0;
    //    $scope.last_page = 0;
    //    //Read PDF from URL.
    //    $('#dwtHorizontalThumbnil').empty();
    //    $scope.loading = true;      
    //    debugger
    //    pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {
    //        debugger
    //        pdfDoc = pdfDoc_;
    //        $scope.Total_Page = pdfDoc.numPages;
    //        //Reference the Container DIV.
    //        var pdf_container = document.getElementById("dwtHorizontalThumbnil");
    //        //pdf_container.style.display = "inline-flex";

    //        //Loop and render all pages.
    //        for (var i = 1; i <= pdfDoc.numPages; i++) {

    //            $scope.RenderPage(pdf_container, i, ($scope.last_page + i));
    //            var rotation = {
    //                page_no: $scope.last_page + i,
    //                rotate_degree: 0
    //            }
    //            $scope.Page_Rotation.push(rotation);
    //        }

    //        $scope.last_page = pdfDoc.numPages;
    //        $scope.loading = false;

    //    });
    //};
    //$scope.RenderPage = function (pdf_container, num, serial) {
    //    $scope.loading = true;

    //    pdfDoc.getPage(num).then(function (page) {
    //        //Create Canvas element and append to the Container DIV.
    //        debugger
    //        var div_add = document.createElement('div');
    //        const att = document.createAttribute("class");
    //        att.value = "col-md-4";
    //        div_add.setAttributeNode(att);
    //        div_add.id = "page_" + serial;

    //        pdf_container.appendChild(div_add);

    //        const att_fun_call = document.createAttribute("ng-dblclick");
    //        att_fun_call.value = "View_Full_page(" + serial + ")";
    //        div_add.setAttributeNode(att_fun_call);


    //        var page_input = document.createElement('input');
    //        page_input.id = 'page-' + serial;
    //        page_input.type = 'checkbox';
    //        page_input.value = false;

    //        var canvas = document.createElement('canvas');
    //        canvas.id = 'pdf-' + serial;
    //        canvas.style = 'zoom: .3'

    //        var ctx = canvas.getContext('2d');

    //        var page_label = document.createElement("LABEL");
    //        var t = document.createTextNode("Page - " + serial);
    //        page_label.setAttribute("for", "Page");
    //        page_label.appendChild(t);
    //        div_add.appendChild(page_input);

    //        div_add.appendChild(page_label);

    //        div_add.appendChild(canvas);

    //        //Create and add empty DIV to add SPACE between pages.
    //        var spacer = document.createElement("div");
    //        spacer.style.height = "20px";
    //        pdf_container.appendChild(spacer);

    //        //Set the Canvas dimensions using ViewPort and Scale.
    //        var viewport = page.getViewport({ scale: scale });
    //        canvas.height = resolution * viewport.height;
    //        canvas.width = resolution * viewport.width;
    //        debugger

    //        //Render the PDF page.
    //        var renderContext = {
    //            canvasContext: ctx,
    //            viewport: viewport,
    //            transform: [resolution, 0, 0, resolution, 0, 0]
    //        };

    //        page.render(renderContext);

    //        $compile(angular.element(document.querySelector('#dwtHorizontalThumbnil')))($scope);
    //        $scope.loading = false;

    //    });
    //};
    //$scope.View_Full_page = function (page_num) {
    //    debugger;
    //    $('#viewerModal').modal('show');
    //    $scope.LargeViewer($scope.PDF_Images, page_num);

    //}
    //$scope.LargeViewer = function (url, page_num) {
    //    $('#dwtLargeViewer').empty();

    //    debugger
    //    $scope.Zoom_Count = 1;
    //    $scope.page_num_show_large = page_num;
    //    //url = new Blob([url], 'workerPdf.pdf', { type: 'application/pdf' });

    //    pdfjsLib.getDocument($scope.Url_Ref).promise.then(function (pdfDoc_) {
    //        debugger
    //        pdfDoc = pdfDoc_;

    //        //Reference the Container DIV.
    //        var pdf_container = document.getElementById("dwtLargeViewer");
    //        //pdf_container.style.display = "inline-flex";

    //        //Loop and render all pages.
    //        $scope.Large_Viewer_RenderPage(pdf_container, $scope.page_num_show_large);

    //    });
    //};
    //$scope.Large_Viewer_RenderPage = function (pdf_container, num) {
    //    $scope.loading = true;

    //    pdfDoc.getPage(num).then(function (page) {
    //        //Create Canvas element and append to the Container DIV.
    //        debugger
    //        $('#dwtLargeViewer').empty();
    //        var div_add = document.createElement('div');
    //        const att = document.createAttribute("class");
    //        att.value = "col-md-12";
    //        div_add.setAttributeNode(att);
    //        div_add.style = 'overflow: scroll; max-height: 650px';
    //        pdf_container.appendChild(div_add);


    //        var canvas = document.createElement('canvas');
    //        canvas.id = 'Large-pdf-Viewer';
    //        canvas.style = 'zoom: .5'

    //        var ctx = canvas.getContext('2d');

    //        div_add.appendChild(canvas);


    //        //Create and add empty DIV to add SPACE between pages.
    //        var spacer = document.createElement("div");
    //        spacer.style.height = "20px";
    //        pdf_container.appendChild(spacer);

    //        //Set the Canvas dimensions using ViewPort and Scale.
    //        var viewport = page.getViewport({ scale: scale });
    //        canvas.height = 3 * viewport.height;
    //        canvas.width = 3 * viewport.width;

    //        //Render the PDF page.
    //        var renderContext = {
    //            canvasContext: ctx,
    //            viewport: viewport,
    //            transform: [3, 0, 0, 3, 0, 0]
    //        };

    //        page.render(renderContext);

    //        $scope.loading = false;

    //    });
    //};

   

    //------------Add
    $scope.LargeViewer = function (url) {
        debugger
        $scope.Zoom_Count = 1;
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {
            debugger
            pdfDoc = pdfDoc_;
            var pdf_container = document.getElementById("dwtLargeViewer");
            $scope.page_backup = 1;

            $scope.Large_Viewer_RenderPage(pdf_container, 1);
            var pdf_container_thumb = document.getElementById("dwtVerticalThumbnil");
            $scope.Large_Thumb_Viewer_RenderPage(pdf_container_thumb);
        });
    };



    $scope.Large_Thumb_Viewer_RenderPage = function (pdf_container) {
        $scope.loading = true;
        $(".mydiv").show();
        $('#dwtVerticalThumbnil').empty();
        pdf_container.style = 'min-height: 100px; max-height: 100px; overflow-x: scroll; display: flex;'; // Set display to flex

        for (var i = 1; i <= pdfDoc.numPages; i++) {
            pdfDoc.getPage(i).then(function (page) {
                var div_add = document.createElement('div');
                const att = document.createAttribute("class");
                att.value = "col-md-12";
                div_add.setAttributeNode(att);
                pdf_container.appendChild(div_add);

                var canvas = document.createElement('canvas');
                canvas.id = 'Large-thumb-Viewer_' + i;
                canvas.style = 'zoom: 0.03';
                var ctx = canvas.getContext('2d');

                div_add.appendChild(canvas);

                const att_fun_call = document.createAttribute("ng-click");
                var page_number = page._pageIndex + 1;
                att_fun_call.value = "View_thumb_onChange(" + page_number + ")";
                div_add.setAttributeNode(att_fun_call);

                var spacer = document.createElement("div");
                spacer.style.width = "1px"; // Change height to width
                pdf_container.appendChild(spacer);

                var viewport = page.getViewport({ scale: scale });
                canvas.height = 3 * viewport.height;
                canvas.width = 3 * viewport.width;

                var renderContext = {
                    canvasContext: ctx,
                    viewport: viewport,
                    transform: [3, 0, 0, 3, 0, 0]
                };

                page.render(renderContext);
                $compile(angular.element(document.querySelector('#dwtVerticalThumbnil')))($scope);
                $scope.loading = false;
                $(".mydiv").hide();
            });
        }
        $scope.loading = false;
        $(".mydiv").hide();
    };

    $scope.page_backup = 0;
    $scope.View_thumb_onChange = function (page_num) {
        if ($scope.page_backup != page_num) {
            var pdf_container = document.getElementById("dwtLargeViewer");
            pdf_container.replaceChildren();
            $scope.Large_Viewer_RenderPage(pdf_container, page_num);
            $scope.page_backup = page_num;
        }

    }

    $scope.Large_Viewer_RenderPage = function (pdf_container, page_num) {

        //$scope.loading = true;
        //$(".mydiv").show();
        debugger;
        pdf_container.replaceChildren();

        $('#dwtLargeViewer').empty();
        pdfDoc.getPage(parseInt(page_num)).then(function (page) {
            // Create Canvas element and append it to the Container DIV.
            debugger;
            // Create a parent container div
            var div_add = document.createElement('div');
            div_add.classList.add("col-md-10"); // Use classList for adding classes
            div_add.style.position = 'relative';
            pdf_container.appendChild(div_add);

            pdf_container.style.minHeight = '600px';
            pdf_container.style.maxHeight = '700px';
            pdf_container.style.minWidth = '100%';
            pdf_container.style.maxWidth = '700px';

            pdf_container.style.position = 'relative';
            pdf_container.style.overflow = 'hidden';
            pdf_container.style.paddingLeft = "auto";
            pdf_container.style.margin = "0 auto";

            var mainCanvas = document.createElement('canvas');
            mainCanvas.id = 'Large-pdf-Viewer';
            // mainCanvas.style.border = '1px solid green';
            mainCanvas.style.position = 'absolute';
            div_add.appendChild(mainCanvas);

            var mainCtx = mainCanvas.getContext('2d');

            // Create a second canvas for drawing rectangles
            var rectCanvas = document.createElement('canvas');
            rectCanvas.style.position = 'absolute';
            rectCanvas.style.top = '0';
            rectCanvas.style.left = '10';
            rectCanvas.width = mainCanvas.width;
            rectCanvas.height = mainCanvas.height;
            rectCanvas.style.border = '1px solid white'; //Add New

            div_add.appendChild(rectCanvas);

            var rectCtx = rectCanvas.getContext('2d');
            var scale_factor = 1.0;
            var viewport = page.getViewport({ scale: scale_factor });
            mainCanvas.height = viewport.height;
            mainCanvas.width = viewport.width;
            mainCanvas.style.border = '1px solid white'; //Add New

            //rectCanvas.width = mainCanvas.width;
            //rectCanvas.height = mainCanvas.height;

            //var isDrawing = false;
            //var startX, startY, endX, endY;
            //var rectangles = [];

            //rectCanvas.addEventListener('mousedown', function (e) {
            //    rectangles = [];
            //    isDrawing = true;
            //    startX = e.clientX - rectCanvas.getBoundingClientRect().left;
            //    startY = e.clientY - rectCanvas.getBoundingClientRect().top;
            //});

            //rectCanvas.addEventListener('mousemove', function (e) {
            //    if (!isDrawing) return;
            //    endX = e.clientX - rectCanvas.getBoundingClientRect().left;
            //    endY = e.clientY - rectCanvas.getBoundingClientRect().top;

            //    rectCtx.clearRect(0, 0, rectCanvas.width, rectCanvas.height);

            //    for (var i = 0; i < rectangles.length; i++) {
            //        var rect = rectangles[i];
            //        rectCtx.strokeStyle = 'rgba(255, 0, 0, 0.5)';
            //        rectCtx.lineWidth = 3;
            //        rectCtx.strokeRect(rect.startX, rect.startY, rect.endX - rect.startX, rect.endY - rect.startY);
            //    }

            //    rectCtx.strokeStyle = 'rgba(255, 0, 0, 0.5)';
            //    rectCtx.lineWidth = 3;
            //    rectCtx.strokeRect(startX, startY, endX - startX, endY - startY);
            //});

            //rectCanvas.addEventListener('mouseup', function () {
            //    isDrawing = false;
            //    rectangles.push({ startX: startX, startY: startY, endX: endX, endY: endY });

            //    console.log("Selected area: startX=" + startX + ", startY=" + startY + ", endX=" + endX + ", endY=" + endY);


            //    $scope.startX = startX * scale_factor * 1.035;
            //    $scope.startY = startY * scale_factor * 1.035;
            //    $scope.endX = endX * scale_factor * 1.045; //Width
            //    $scope.endY = endY * scale_factor * 1.040; //Height
            //    $scope.$apply();

            //});

            var mainRenderContext = {
                canvasContext: mainCtx,
                viewport: viewport,
                transform: [1, 0, 0, 1, 0, 0]
            };

            page.render(mainRenderContext);
            $compile(angular.element(document.querySelector('#dwtLargeViewer')))($scope);

            $scope.loading = false;
            $(".mydiv").hide();
        });

    };



//-----------End

}]);

