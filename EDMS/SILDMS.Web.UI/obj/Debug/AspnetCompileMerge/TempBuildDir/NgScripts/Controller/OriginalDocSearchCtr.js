app.controller('OriginalDocSearchCtr', ['$scope', '$http', 'ImageProcessServices', 'environmentServices', '$compile', '$injector', '$interval', function ($scope, $http, ImageProcessServices, environmentServices, $compile, $injector, $interval) {

    'use strict'
    $scope.Zoom_Count = .5;
    $scope.PDF_Images;
    applySecurity();

    $scope.docPropIdentityModel = {
        OwnerLevel: { OwnerLevelID: "", LevelName: "" },
        Owner: { OwnerID: "", OwnerName: "" },
        DocCategory: { DocCategoryID: "", DocCategoryName: "" },
        DocType: { DocTypeID: "", DocTypeName: "" },
        DocProperty: { DocPropertyID: "", DocPropertyName: "" },
        SearchBy: '1',
        SearchFor: "",
        DocCat: '',
        DocType: '',
        Status: ""
    };
    $scope.DWReadOnly = true;
    $scope.pagingInfo = {
        page: 1,
        itemsPerPage: 20,
        sortBy: null,
        reverse: false,
        search: null,
        totalItems: 0
    };


    $scope.search = function () {
        $scope.pagingInfo.page = 1;
        $scope.BindDataToGrid();
    };

    $scope.AttrName = '';
    $scope.AttrValue = '';


    $scope.sort = function (sortBy) {
        if (sortBy === $scope.pagingInfo.sortBy) {
            $scope.pagingInfo.reverse = !$scope.pagingInfo.reverse;
        } else {
            $scope.pagingInfo.sortBy = sortBy;
            $scope.pagingInfo.reverse = false;
        }
        $scope.pagingInfo.page = 1;
        $scope.BindDataToGrid();
    };

    $scope.selectPage = function () {
        $scope.BindDataToGrid();
    };


    $scope.ShowDeleteConfirmModal = function (row) {
        $scope.DocumentIDForDelete = row.DocumentID;
        $scope.DocDistributionIDForDelete = row.DocDistributionID;
        $('#ConfirmDelete').modal('show');
    };

    $scope.DeleteDocument = function () {
        $scope.loading = true;
        $http.post('/DocScanningModule/OriginalDocSearching/DeleteDocument',
            {
                _DocumentID: $scope.DocumentIDForDelete,
                _DocDistributionID: $scope.DocDistributionIDForDelete,
                _DocumentType: "Original"
            })
            .success(function (response) {
                $('#ConfirmDelete').modal('hide');
                $scope.BindDataToGrid();
                $scope.loading = false;
                toastr.success("Delete Successful");
            }).error(function (error) {
                $scope.loading = false;
                toastr.error("Delete Failed");
            });
    };


    //Dynamsoft.WebTwainEnv.RegisterEvent('OnWebTwainReady', Dynamsoft_OnReady);

    var DWObject;
    var DWObjectLargeViewer;
    var DWObjectQuickViewer;

    var DWObjectQuickViewer2;
    var zoomFactor = 1;



    function Dynamsoft_OnReady() {
        DWObject = Dynamsoft.WebTwainEnv.GetWebTwain('dwtVerticalThumbnil');
        DWObjectQuickViewer = Dynamsoft.WebTwainEnv.GetWebTwain('dwtQuickViewer');

        DWObjectQuickViewer2 = Dynamsoft.WebTwainEnv.GetWebTwain('dwtQuickViewer2');
        DWObjectLargeViewer = Dynamsoft.WebTwainEnv.GetWebTwain('dwtLargeViewer');

        DWObjectQuickViewer.SetViewMode(1, 1);
        DWObjectQuickViewer2.SetViewMode(1, 1);

        DWObjectQuickViewer.Width = 9.69 * 100;
        DWObjectQuickViewer.Height = 10.69 * 70;

        DWObjectQuickViewer2.Width = 9.69 * 100;
        DWObjectQuickViewer2.Height = 10.69 * 70;

        DWObjectLargeViewer.SetViewMode(-1, -1);
        DWObjectLargeViewer.MaxImagesInBuffer = 1;
        DWObjectLargeViewer.Width = 9.69 * 90;

        DWObject.SetViewMode(-1, 4);
        DWObject.FitWindowType = 0;
        DWObject.SelectionImageBorderColor = 0x691254;
        DWObject.ShowPageNumber = true;
        DWObject.IfAppendImage = true;

        DWObject.RegisterEvent('OnMouseClick', $scope.updateLargeViewer);


        //DWObject.Addon.PDF.Download("../Resources/addon/Pdf.zip",  // specify the url of the add-on resource
        //    function () { console.log('Successfully Downloaded PDF add-on'); }, // successful callback
        //    function (errorCode, errorString) { alert(errorString); }  // failure callback and show error message
        //);

        //DWObject.Addon.PDF.SetResolution(200);
        //DWObject.Addon.PDF.SetConvertMode(EnumDWT_ConverMode.CM_RENDERALL);

        //DWObject.IfShowFileDialog = true;

        //DWObject.RemoveAllImages();
        //DWObjectQuickViewer.RemoveAllImages();
        //DWObjectLargeViewer.RemoveAllImages();
    };

    //$scope.ResetImageViewrs = function () {
    //    DWObjectQuickViewer.RemoveAllImages();
    //    DWObject.RemoveAllImages();
    //    DWObjectLargeViewer.RemoveAllImages();

    //    DWObjectQuickViewer2.RemoveAllImages();
    //}

    $scope.ZoomIn = function () {
        //DWObjectLargeViewer.Zoom = zoomFactor * 1.2;
        //zoomFactor = zoomFactor * 1.2;
        var doc_view_ref = document.getElementById('Large-pdf-Viewer');
        var dz = parseFloat($scope.Zoom_Count) + parseFloat('0.2');
        doc_view_ref.style = 'zoom: ' + dz;
        $scope.Zoom_Count = dz;
    };

    $scope.ZoomOut = function () {
        //DWObjectLargeViewer.Zoom = zoomFactor / 1.2;
        //zoomFactor = zoomFactor / 1.2;
        var doc_view_ref = document.getElementById('Large-pdf-Viewer');
        var dk = parseFloat($scope.Zoom_Count) - parseFloat('0.2');

        doc_view_ref.style = 'zoom: ' + dk;
        $scope.Zoom_Count = dk;
    };

    $scope.RotateRight = function () {
        DWObjectLargeViewer.RotateRight(DWObjectLargeViewer.GetSelectedImageIndex(0));
    }

    $scope.LoadImage = function (tableRow, e) {
        
        $http.post('/DocScanningModule/OriginalDocSearching/GetDocPropIdentityForSpecificDocType',
            {
                _DocumentID: tableRow.DocumentID,
                _DocDistributionID: tableRow.DocDistributionID
            })
            .success(function (response) {
                $scope.DocumentsAttributeList = response;
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });

        applySecurity();

        var url = "DocScanningModule/MultiDocScan/GetFilePassWord_r?" + encodeURIComponent("serverIP") + "=" + encodeURIComponent(tableRow.ServerIP)
            + "&" + encodeURIComponent("ftpPort") + "=" + encodeURIComponent(tableRow.ServerPort)
            + "&" + encodeURIComponent("ftpUserName") + "=" + encodeURIComponent(tableRow.FtpUserName)
            + "&" + encodeURIComponent("ftpPassword") + "=" + encodeURIComponent(tableRow.FtpPassword)
            + "&" + encodeURIComponent("serverURL") + "=" + encodeURIComponent(tableRow.FileServerURL)
            + "&" + encodeURIComponent("documentID") + "=" + encodeURIComponent(tableRow.DocumentID)
            + "&" + encodeURIComponent("isObsolete") + "=" + encodeURIComponent(tableRow.IsObsolutable)
            + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent(tableRow.IsSecured)

        ImageProcessServices.detailView(url).then(function (data) {
            debugger
            var arrayBuffer = data.data;
           
            var data = { data: arrayBuffer }

            $scope.QuickViewer(data);
            $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });

            
            $('#QuickViewModal').modal('show');

            $('#serverIp').val(tableRow.ServerIP);
            $('#ServerPort').val(tableRow.ServerPort);
            $('#FtpUserName').val(tableRow.FtpUserName);
            $('#FtpPassword').val(tableRow.FtpPassword);
            $('#FileServerURL').val(tableRow.FileServerURL);
            $('#DocumentID').val(tableRow.DocumentID);
            $('#IsObsolutable').val(tableRow.IsObsolutable);

            $scope.loading = false;

        }, function (error) {
            console.log(error.data);
            alert(error.data);
            $scope.loading = false;


        });
      
       
    };


    var MetaValues = {
        MetaValue: ""
    };

    var FinalObj = { "MetaValues": [] };

    $scope.LoadMergeImage = function (GridDisplayCollection, e) {

        angular.forEach($scope.GridDisplayCollection, function (item) {

            MetaValue = item.DocumentID;

            FinalObj.MetaValues.push(MetaValue);

        });

        var Docs = FinalObj.MetaValues.join();

        $http.post('/DocScanningModule/MossDocumentPrint/Marge',
            {
                _DocCategoryID: $scope.docPropIdentityModel.DocCat,
                _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                _DocTypeID: $scope.docPropIdentityModel.DocType,
                _DocPropertyID: $scope.docPropIdentityModel.DocProperty.DocPropertyID,
                _SearchBy: $scope.docPropIdentityModel.SearchBy,
                page: $scope.pagingInfo.page,
                itemsPerPage: $scope.pagingInfo.itemsPerPage,
                sortBy: $scope.pagingInfo.itemsPerPage,
                reverse: $scope.pagingInfo.reverse,
                attribute: $scope.AttrName,
                Docs: Docs,
                search: $scope.AttrName == '' ? $scope.pagingInfo.search : $scope.AttrValue
            })
            .success(function (response) {

                $scope.showPrint = false;
                $scope.loading = false;
                if (response.Message == "Document Merge Successfully") {

                    DWObjectQuickViewer.Addon.PDF.Download("../Resources/addon/Pdf.zip",   // specify the url of the add-on resource
                        function () { console.log('Successfully Downloaded PDF add-on'); }, // successful callback
                        function (errorCode, errorString) { alert(errorString); }  // failure callback and show error message
                    );
                    DWObjectQuickViewer.IfShowFileDialog = false;
                    DWObjectQuickViewer.RemoveAllImages();
                    DWObjectQuickViewer.Addon.PDF.SetResolution(200);
                    DWObjectQuickViewer.Addon.PDF.SetConvertMode(EnumDWT_ConverMode.CM_RENDERALL);

                    DWObjectQuickViewer.HTTPDownloadEx(location.host, "DocScanningModule/OriginalDocSearching/GetPdf1?" + encodeURIComponent("serverIP") + "=" + encodeURIComponent($scope.GridDisplayCollection[0].ServerIP)
                        + "&" + encodeURIComponent("ftpPort") + "=" + encodeURIComponent($scope.GridDisplayCollection[0].ServerPort)
                        + "&" + encodeURIComponent("ftpUserName") + "=" + encodeURIComponent($scope.GridDisplayCollection[0].FtpUserName)
                        + "&" + encodeURIComponent("ftpPassword") + "=" + encodeURIComponent($scope.GridDisplayCollection[0].FtpPassword)
                        + "&" + encodeURIComponent("serverURL") + "=" + encodeURIComponent($scope.GridDisplayCollection[0].FileServerURL)
                        + "&" + encodeURIComponent("documentID") + "=" + encodeURIComponent($scope.GridDisplayCollection[0].DocumentID)
                        + "&" + encodeURIComponent("isObsolete") + "=" + encodeURIComponent($scope.GridDisplayCollection[0].IsObsolutable)
                        + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent($scope.GridDisplayCollection[0].IsSecured), 4, function () {
                            $('#QuickViewModal').modal('show');
                            //$('#serverIp').val(tableRow.ServerIP);
                            //$('#ServerPort').val(tableRow.ServerPort);
                            //$('#FtpUserName').val(tableRow.FtpUserName);
                            //$('#FtpPassword').val(tableRow.FtpPassword);
                            //$('#FileServerURL').val(tableRow.FileServerURL);
                            //$('#DocumentID').val(tableRow.DocumentID);
                            //$('#IsObsolutable').val(tableRow.IsObsolutable);
                        }, function () {
                            toastr.error('File Not Found');
                        });





                    $scope.DWReadOnly = false;
                    toastr.success(response.Message);
                }
                else if (response.Message == "Document Not Found") {
                    $scope.DWReadOnly = true;
                    toastr.error(response.Message);
                }
                else {
                    $scope.DWReadOnly = true;
                    toastr.error(response.Message);
                }
                //toggleRefreshTable();
            })
            .error(function (response) {
                $scope.loading = false;
                $scope.DWReadOnly = true;
                e.PreverntDefault();
                toastr.error("Document Merge Failed.");
            });
    };


    $scope.Print = function () {
        applySecurity();
        DWObjectQuickViewer2.Addon.PDF.Download(
            "../Resources/addon/Pdf.zip",  // specify the url of the add-on resource
            function () { console.log('Successfully Downloaded PDF add-on'); }, // successful callback
            function (errorCode, errorString) { alert(errorString); }  // failure callback and show error message
        );
        var ServerIP = $('#serverIp').val();
        var ServerPort = $('#ServerPort').val();
        var FtpUserName = $('#FtpUserName').val();
        var FtpPassword = $('#FtpPassword').val();
        var FileServerURL = $('#FileServerURL').val();
        var DocumentID = $('#DocumentID').val();
        var IsObsolutable = $('#IsObsolutable').val();
        var IsSecured = "";

        DWObjectQuickViewer2.IfShowFileDialog = false;
        DWObjectQuickViewer2.RemoveAllImages();
        DWObjectQuickViewer2.Addon.PDF.SetResolution(200);
        DWObjectQuickViewer2.Addon.PDF.SetConvertMode(EnumDWT_ConverMode.CM_RENDERALL);

        DWObjectQuickViewer2.HTTPDownloadEx(location.host, "DocScanningModule/MultiDocScan/GetInformationCopy?" + encodeURIComponent("serverIP") + "=" + encodeURIComponent(ServerIP)
            + "&" + encodeURIComponent("ftpPort") + "=" + encodeURIComponent(ServerPort)
            + "&" + encodeURIComponent("ftpUserName") + "=" + encodeURIComponent(FtpUserName)
            + "&" + encodeURIComponent("ftpPassword") + "=" + encodeURIComponent(FtpPassword)
            + "&" + encodeURIComponent("serverURL") + "=" + encodeURIComponent(FileServerURL)
            + "&" + encodeURIComponent("documentID") + "=" + encodeURIComponent(DocumentID)
            + "&" + encodeURIComponent("isObsolete") + "=" + encodeURIComponent(IsObsolutable)
            + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent(true), 3, function () {
                //$('#QuickViewModal2').modal('show');
                $scope.Print2();
            }, function () {
                toastr.error('File Not Found');
            });
        //DWObjectQuickViewer2.HTTPDownloadEx(location.host, "DocScanningModule/MultiDocScan/GetInformationCopy?serverIP=173.16.191.141&ftpPort=21&ftpUserName=administrator&ftpPassword=sysadmiN&serverURL=DUDepartment/DUQASPL/ECP/FU1/QC&documentID=19031900005&Ext=pdf", 4, function () {
        //       //$('#QuickViewModal2').modal('show');
        //       $scope.Print2();
        //   }, function () {
        //       toastr.error('File Not Found');
        //   });

    };

    $scope.Print2 = function () {
        //DWObjectQuickViewer2.Print();

    }


    $scope.DetailPrint = function () {
        var ServerIP = $('#serverIp').val();
        var ServerPort = $('#ServerPort').val();
        var FtpUserName = $('#FtpUserName').val();
        var FtpPassword = $('#FtpPassword').val();
        var FileServerURL = $('#FileServerURL').val();
        var DocumentID = $('#DocumentID').val();
        var IsObsolutable = $('#IsObsolutable').val();
        var IsSecured = 0;
        var InformationCopy = 0;
        var addTextAction = 0;
        $http.post('/DocScanningModule/MultiDocScan/GetInformationCopyToPrint',
            {
                serverIP: ServerIP,
                ftpPort: ServerPort,
                ftpUserName: FtpUserName,
                ftpPassword: FtpPassword,
                serverURL: FileServerURL,
                documentID: DocumentID,
                isObsolete: IsObsolutable,
                isSecured: IsSecured,
                InformationCopy: InformationCopy,
                Action: 1,// 1 Stands for Printing
                addTextAction: addTextAction,
                TextAddPropertyCollection: TextAddPropertyCollection


            }).success(function (response) {
                //var pdfAsDataUri = "data:application/pdf;base64," +response;
                //$scope.content = $sce.trustAsResourceUrl(pdfAsDataUri);
                //var win = window.open();
                //win.document.write('<iframe src="' + pdfAsDataUri + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');
                var arrBuffer = base64ToArrayBuffer(response);

                var file = new Blob([arrBuffer], { type: 'application/pdf' });
                var fileURL = URL.createObjectURL(file);
                //window.open(fileURL, "EPrescription");
                window.open().document.write('<iframe src="' + fileURL + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');
            });
    };
    $scope.ShowDetailView_new = function (tableRow, e) {
       
        $http.post('/DocScanningModule/OriginalDocSearching/GetDocPropIdentityForSpecificDocType',
            {
                _DocumentID: tableRow.DocumentID,
                _DocDistributionID: tableRow.DocDistributionID
            })
            .success(function (response) {
                debugger
                $scope.DocumentsAttributeList = response;
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });
        applySecurity();
        var url = "DocScanningModule/MultiDocScan/GetFilePassWord_r?" + encodeURIComponent("serverIP") + "=" + encodeURIComponent(tableRow.ServerIP)
            + "&" + encodeURIComponent("ftpPort") + "=" + encodeURIComponent(tableRow.ServerPort)
            + "&" + encodeURIComponent("ftpUserName") + "=" + encodeURIComponent(tableRow.FtpUserName)
            + "&" + encodeURIComponent("ftpPassword") + "=" + encodeURIComponent(tableRow.FtpPassword)
            + "&" + encodeURIComponent("serverURL") + "=" + encodeURIComponent(tableRow.FileServerURL)
            + "&" + encodeURIComponent("documentID") + "=" + encodeURIComponent(tableRow.DocumentID)
            + "&" + encodeURIComponent("isObsolete") + "=" + encodeURIComponent(tableRow.IsObsolutable)
            + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent(tableRow.IsSecured)

        ImageProcessServices.detailView(url).then(function (data) {
            debugger
            var arrayBuffer = data.data;
            //data.data = $scope.arrayBufferToFile(arrayBuffer, 'example.pdf');
            ////var downloadLink = document.createElement('a');
            ////downloadLink.href = URL.createObjectURL(data.data);
            ////downloadLink.download = data.data.name;
            ////downloadLink.click();
            //var data = new Uint8Array(fs.readFileSync(data.data));

            //// Case 2: If the PDF data is already a buffer from a source such as an HTTP response
            //// In this case, you don't passing the PDF data stream directly into getDocument.
            // You pass it as an object property.
            var data = { data: arrayBuffer }
            $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });

            $scope.LargeViewer(data);


            $('#DetailViewModal').modal('show');
            $('#serverIp').val(tableRow.ServerIP);
            $('#ServerPort').val(tableRow.ServerPort);
            $('#FtpUserName').val(tableRow.FtpUserName);
            $('#FtpPassword').val(tableRow.FtpPassword);
            $('#FileServerURL').val(tableRow.FileServerURL);
            $('#DocumentID').val(tableRow.DocumentID);
            $('#IsObsolutable').val(tableRow.IsObsolutable);

            $scope.loading = false;

        }, function (error) {
            console.log(error.data);
            alert(error.data);
            $scope.loading = false;


        });

        

    };


    $scope.updateLargeViewer = function () {
        DWObject.CopyToClipboard(DWObject.CurrentImageIndexInBuffer);
        DWObjectLargeViewer.LoadDibFromClipboard();
    };

    $scope.urlParameter = function (tableRow) {
        var serverIP = tableRow.ServerIP;
        var ftpPort = tableRow.ServerPort;
        var ftpUserName = tableRow.FtpUserName;
        var ftpPassword = tableRow.FtpPassword;
        var serverURL = tableRow.FileServerURL;
        var documentID = tableRow.DocumentID;

        var token = "" + serverIP + "..|.." + ftpPort + "..|.." + ftpUserName + "..|.." + ftpPassword + "..|.." + serverURL + "..|.." + documentID;
        return btoa(token);
    };
    $scope.DocumentIDForDelete = "";
    $scope.loading = true;


    $http.get('/DocScanningModule/OwnerProperIdentity/GetOwnerLevel?_OwnerLevelID=')
        .success(function (response) {
            $scope.ownerLevels = response.result;
            $scope.docPropIdentityModel.OwnerLevel = response.result[0];
            //$scope.docPropIdentityModel.OwnerLevel = "";
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
                //$scope.ResetImageViewrs();
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




    $scope.DocCatForOwner = [];
    $scope.$watch('docPropIdentityModel.Owner', function (newVal) {
        if (newVal) {
            $scope.docPropIdentityModel.DocCategory = "";
            $scope.docPropIdentityModel.DocType = "";
            $scope.docPropIdentityModel.DocProperty = "";
            //$scope.ResetImageViewrs();
            $http.post('/DocScanningModule/OwnerProperIdentity/GetDocumentCategoriesForSpecificOwner',
                { _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID })
                .success(function (response) {
                    $scope.docCategoriesForSpecificOwner = response.result;
                    $scope.DocCatForOwner = response.result;
                    $scope.loading = false;
                }).error(function () {
                    $scope.loading = false;
                });
        }
    });

    $scope.DocTypeForOwner = [];
    $scope.$watch('docPropIdentityModel.DocCat', function (newVal) {
        if (newVal) {
            $scope.docPropIdentityModel.DocType = "";
            $scope.docPropIdentityModel.DocProperty = "";
            //$scope.ResetImageViewrs();
            $http.post('/DocScanningModule/OwnerProperIdentity/GetDocumentTypeForSpecificDocCategory',
                {
                    _DocCategoryID: $scope.docPropIdentityModel.DocCat,
                    _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID
                })
                .success(function (response) {
                    $scope.docTypeForSpecificDocCategory = response.result;
                    $scope.DocTypeForOwner = response.result;
                    $scope.loading = false;
                }).error(function () {
                    $scope.loading = false;
                });
        }
    });

    $scope.$watch('docPropIdentityModel.DocType', function (newVal) {
        if (newVal) {
            $scope.docPropIdentityModel.DocProperty = "";
            //$scope.ResetImageViewrs();

            $http.post('/DocScanningModule/MultiDocScan/GetDocumentProperty',
                {
                    _DocCategoryID: $scope.docPropIdentityModel.DocCat,
                    _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                    _DocTypeID: $scope.docPropIdentityModel.DocType
                })
                .success(function (response) {
                    $scope.docPropertyForSpecificDocType = response.result;
                    $scope.loading = false;
                }).error(function () {
                    $scope.loading = false;
                });
        }
    });

    $scope.$watch('docPropIdentityModel.DocProperty', function (newVal, oldVal) {
        if (newVal) {
            $scope.BindDataToGrid();
        //    $scope.ResetImageViewrs();
        }
    });

    $scope.$watch('docPropIdentityModel.SearchBy', function (newVal, oldVal) {
        if (newVal) {
            if (newVal != oldVal) {
                $scope.BindDataToGrid();
            //    $scope.ResetImageViewrs();
            }

        }
    });

    $scope.BindDataToGrid = function () {
        $scope.loading = true;
        $http.post('/DocScanningModule/OriginalDocSearching/GetDocumentsBySearchParam',
            {
                _DocCategoryID: $scope.docPropIdentityModel.DocCat,
                _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                _DocTypeID: $scope.docPropIdentityModel.DocType,
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
                if (pageable.respStatus == null) {

                    $scope.GridDisplayCollection = pageable.lstDocSearch;
                    $scope.pagingInfo.totalItems = pageable.totalPages;
                    $scope.loading = false;
                }
                else {
                    $scope.GridDisplayCollection = pageable.lstDocSearch;
                    $scope.pagingInfo.totalItems = pageable.totalPages;
                    toastr.error(pageable.respStatus.Message);
                    $scope.loading = false;
                }

                applySecurity();
            }).error(function () {
                $scope.loading = false;
            });

    };

    $scope.toggleEdit = function (tableRow) {
        $http.post('/DocScanningModule/OriginalDocSearching/GetDocPropIdentityForSpecificDocType',
            {
                _DocumentID: tableRow.DocumentID,
                _DocDistributionID: tableRow.DocDistributionID
            })
            .success(function (response) {
                $scope.DocumentsAttributeList = response;
                $scope.documentID = tableRow.DocumentID;
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });

        $('#addModal').modal('show');
    };
    var TextAddPropertyCollection = [];
    $scope.showPDF = function () {

        var ServerIP = $('#serverIp').val();
        var ServerPort = $('#ServerPort').val();
        var FtpUserName = $('#FtpUserName').val();
        var FtpPassword = $('#FtpPassword').val();
        var FileServerURL = $('#FileServerURL').val();
        var DocumentID = $('#DocumentID').val();
        var IsObsolutable = $('#IsObsolutable').val();
        var IsSecured = 0;
        var InformationCopy = 0;
        var addTextAction = 0;
        $http.post('/DocScanningModule/MultiDocScan/GetInformationCopyToPrint',
            {
                serverIP: ServerIP,
                ftpPort: ServerPort,
                ftpUserName: FtpUserName,
                ftpPassword: FtpPassword,
                serverURL: FileServerURL,
                documentID: DocumentID,
                isObsolete: IsObsolutable,
                isSecured: IsSecured,
                InformationCopy: InformationCopy,
                Action: 1,// 1 Stands for Printing
                addTextAction: addTextAction,
                TextAddPropertyCollection: TextAddPropertyCollection


            }).success(function (response) {
                //var pdfAsDataUri = "data:application/pdf;base64," +response;
                //$scope.content = $sce.trustAsResourceUrl(pdfAsDataUri);
                //var win = window.open();
                //win.document.write('<iframe src="' + pdfAsDataUri + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');
                var arrBuffer = base64ToArrayBuffer(response);

                var file = new Blob([arrBuffer], { type: 'application/pdf' });
                var fileURL = URL.createObjectURL(file);
                //window.open(fileURL, "EPrescription");
                window.open().document.write('<iframe src="' + fileURL + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');
            });
        addTextAction = 0;
        TextAddPropertyCollection = [];

    };

    //Download Method for the documnet
    $scope.Download = function (tableRow, e) {
        applySecurity();
        DWObjectQuickViewer2.Addon.PDF.Download(
            "../Resources/addon/Pdf.zip",  // specify the url of the add-on resource
            function () { console.log('Successfully Downloaded PDF add-on'); }, // successful callback
            function (errorCode, errorString) { alert(errorString); }  // failure callback and show error message
        );
        debugger;
        var ServerIP = tableRow.ServerIP;
        var ServerPort = tableRow.ServerPort;
        var FtpUserName = tableRow.FtpUserName;
        var FtpPassword = tableRow.FtpPassword;
        var FileServerURL = tableRow.FileServerURL;
        var DocumentID = tableRow.DocumentID;
        var IsObsolutable = tableRow.IsObsolutable;
        var IsSecured = tableRow.IsSecured;
        var InformationCopy = 0;


        DWObjectQuickViewer2.IfShowFileDialog = false;
        DWObjectQuickViewer2.RemoveAllImages();
        DWObjectQuickViewer2.Addon.PDF.SetResolution(200);
        DWObjectQuickViewer2.Addon.PDF.SetConvertMode(EnumDWT_ConverMode.CM_RENDERALL);

        //if (addTextAction) {

        //    PageFrom = from;
        //    PageTo = to;
        //    TextToAdd = textThatWillBeAdded;

        //}
        $http.post('/DocScanningModule/MultiDocScan/GetInformationCopyToPrint',
            {
                serverIP: ServerIP,
                ftpPort: ServerPort,
                ftpUserName: FtpUserName,
                ftpPassword: FtpPassword,
                serverURL: FileServerURL,
                documentID: DocumentID,
                isObsolete: IsObsolutable,
                isSecured: IsSecured,
                InformationCopy: InformationCopy,
                Action: 0, // 0 Stands for downloading
                addTextAction: 0,
                TextAddPropertyCollection: TextAddPropertyCollection


            }).success(function (response) {

                //var element = document.createElement('a');
                //var pdfAsDataUri = "data:application/pdf;base64," + response;
                //element.setAttribute('href', pdfAsDataUri);
                //element.setAttribute('download', DocumentID+ ".pdf");

                //element.style.display = 'none';
                //document.body.appendChild(element);

                //element.click();

                //document.body.removeChild(element);
                $scope.downloadPDFWithArrayBuffer(response, DocumentID);

            });



    }
    $scope.downloadPDFWithArrayBuffer = function (base64Data, DocumentID) {

        var arrBuffer = base64ToArrayBuffer(base64Data);

        // It is necessary to create a new blob object with mime-type explicitly set
        // otherwise only Chrome works like it should
        var newBlob = new Blob([arrBuffer], { type: "application/pdf" });

        // IE doesn't allow using a blob object directly as link href
        // instead it is necessary to use msSaveOrOpenBlob
        if (window.navigator && window.navigator.msSaveOrOpenBlob) {
            window.navigator.msSaveOrOpenBlob(newBlob);
            return;
        }

        // For other browsers:
        // Create a link pointing to the ObjectURL containing the blob.
        var data = window.URL.createObjectURL(newBlob);

        var link = document.createElement('a');
        document.body.appendChild(link); //required in FF, optional for Chrome
        link.href = data;
        link.download = DocumentID + ".pdf";
        link.click();
        window.URL.revokeObjectURL(data);
        link.remove();
    }

    function base64ToArrayBuffer(data) {
        var binaryString = window.atob(data);
        var binaryLen = binaryString.length;
        var bytes = new Uint8Array(binaryLen);
        for (var i = 0; i < binaryLen; i++) {
            var ascii = binaryString.charCodeAt(i);
            bytes[i] = ascii;
        }
        return bytes;
    };
    var DocMetaValues = {
        MetaValue: "",
        DocMetaID: ""
    };

    var FinalObject = { "DocMetaValues": [] };


    $scope.Save = function () {
        $scope.loading = true;

        angular.forEach($scope.DocumentsAttributeList, function (item) {

            DocMetaValues.DocMetaID = item.DocMetaID;
            DocMetaValues.MetaValue = item.MetaValue;

            FinalObject.DocMetaValues.push(DocMetaValues);
            FinalObject.DocumentID = $scope.documentID;
            DocMetaValues = {
                MetaValue: "",
                DocMetaID: ""
            };

        });

        $.ajax({
            url: '/DocScanningModule/OriginalDocSearching/UpdateDocMetaInfo',
            data: JSON.stringify(FinalObject),
            type: 'POST',
            contentType: 'application/json;',
            dataType: 'json',
            //async: false,
            success: function (response) {
                $scope.loading = false;
                $('#addModal').modal('hide');
                $scope.BindDataToGrid();
                toastr.success(response.Message);
            },
            error: function (response) {
                $scope.loading = false;
                toastr.error(response.Message);
            }
        });

        FinalObject = { "DocMetaValues": [] };
    }

    $scope.toggleRefreshTable = function (row) {
        location.reload();
    };

    $scope.Marge = function () {
        $scope.DWReadOnly = true;
        $scope.loading = true;
        $http.post('/DocScanningModule/OriginalDocSearching/Marge',
            {
                _DocCategoryID: $scope.docPropIdentityModel.DocCat,
                _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                _DocTypeID: $scope.docPropIdentityModel.DocType,
                _DocPropertyID: $scope.docPropIdentityModel.DocProperty.DocPropertyID,
                _SearchBy: $scope.docPropIdentityModel.SearchBy,
                page: $scope.pagingInfo.page,
                itemsPerPage: $scope.pagingInfo.itemsPerPage,
                sortBy: $scope.pagingInfo.itemsPerPage,
                reverse: $scope.pagingInfo.reverse,
                attribute: $scope.AttrName,
                search: $scope.AttrName == '' ? $scope.pagingInfo.search : $scope.AttrValue
            })
            .success(function (response) {

                $scope.showPrint = false;
                $scope.loading = false;
                if (response.Message == "Document Merge Successfully") {
                    $scope.DWReadOnly = false;
                    toastr.success(response.Message);
                }
                else if (response.Message == "Document Not Found") {
                    $scope.DWReadOnly = true;
                    toastr.error(response.Message);
                }
                else {
                    $scope.DWReadOnly = true;
                    toastr.error(response.Message);
                }
                //toggleRefreshTable();
            })
            .error(function (response) {
                $scope.loading = false;
                $scope.DWReadOnly = true;
                e.PreverntDefault();
                toastr.error("Document Merge Failed.");
            });
    }



    $scope.arrayBufferToFile = function (arrayBuffer, fileName) {
        var blob = new Blob([arrayBuffer], { type: 'application/pdf' });
        return new File([blob], fileName, { type: 'application/pdf' });
    }

    var pdfDoc = null;
    var scale = 1; //Set Scale for zooming PDF.
    var resolution = 1; //Set Resolution to Adjust PDF clarity.

    $scope.QuickViewer = function (url) {
        debugger
        $scope.Zoom_Count = 1;
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {
            debugger
            pdfDoc = pdfDoc_;

            var pdf_container = document.getElementById("dwtQuickViewer");

            $scope.Quick_Viewer_RenderPage(pdf_container);
       
        });
    };
    $scope.LargeViewer = function (url) {
        debugger
        $scope.Zoom_Count = 1;
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {
            debugger
            pdfDoc = pdfDoc_;
            var pdf_container = document.getElementById("dwtLargeViewer");
            $scope.page_backup = 1;

            $scope.Large_Viewer_RenderPage(pdf_container,1);
            var pdf_container_thumb = document.getElementById("dwtVerticalThumbnil");
            $scope.Large_Thumb_Viewer_RenderPage(pdf_container_thumb);

            

        });
    };
    $scope.Large_Thumb_Viewer_RenderPage = function (pdf_container) {
        $scope.loading = true;
        debugger
        $('#dwtVerticalThumbnil').empty();
        for (var i = 1; i <= pdfDoc.numPages; i++) {
            pdfDoc.getPage(i).then(function (page) {
                //Create Canvas element and append to the Container DIV.
                debugger

                var div_add = document.createElement('div');
                const att = document.createAttribute("class");
                att.value = "col-md-12";
                div_add.setAttributeNode(att);
                pdf_container.appendChild(div_add);
                pdf_container.style = 'max-height: 650px;overflow:scroll';

                var canvas = document.createElement('canvas');
                canvas.id = 'Large-thumb-Viewer_'+i;
                canvas.style = 'zoom: .1'

                var ctx = canvas.getContext('2d');

                div_add.appendChild(canvas);
                const att_fun_call = document.createAttribute("ng-click");

                var page_number = page._pageIndex + 1;
                att_fun_call.value = "View_thumb_onChange(" + page_number + ")";
                div_add.setAttributeNode(att_fun_call);

                //Create and add empty DIV to add SPACE between pages.
                var spacer = document.createElement("div");
                spacer.style.height = "20px";
                pdf_container.appendChild(spacer);

                //Set the Canvas dimensions using ViewPort and Scale.
                var viewport = page.getViewport({ scale: scale });
                canvas.height = 3 * viewport.height;
                canvas.width = 3 * viewport.width;

                //Render the PDF page.
                var renderContext = {
                    canvasContext: ctx,
                    viewport: viewport,
                    transform: [3, 0, 0, 3, 0, 0]
                };

                page.render(renderContext);
                $compile(angular.element(document.querySelector('#dwtVerticalThumbnil')))($scope);

                $scope.loading = false;

            });

        }
    };


    $scope.Quick_Viewer_RenderPage = function (pdf_container) {
        $scope.loading = true;
        debugger
        $('#dwtLargeViewer').empty();
        for (var i = 1; i <= pdfDoc.numPages; i++) {
            pdfDoc.getPage(i).then(function (page) {
                //Create Canvas element and append to the Container DIV.
                debugger

                var div_add = document.createElement('div');
                const att = document.createAttribute("class");
                att.value = "col-md-12";
                div_add.setAttributeNode(att);
                pdf_container.appendChild(div_add);
                pdf_container.style = 'max-height: 650px;overflow:scroll';


                var canvas = document.createElement('canvas');
                canvas.id = 'Large-pdf-Viewer_'+i;
                canvas.style = 'zoom: .5'

                var ctx = canvas.getContext('2d');

                div_add.appendChild(canvas);


                //Create and add empty DIV to add SPACE between pages.
                var spacer = document.createElement("div");
                spacer.style.height = "20px";
                pdf_container.appendChild(spacer);

                //Set the Canvas dimensions using ViewPort and Scale.
                var viewport = page.getViewport({ scale: 1 });
                canvas.height = 4 * viewport.height;
                canvas.width = 4 * viewport.width;

                //Render the PDF page.
                var renderContext = {
                    canvasContext: ctx,
                    viewport: viewport,
                    transform: [4, 0, 0, 4, 0, 0]
                };

                page.render(renderContext);

                $scope.loading = false;

            });

        }
    };

    $scope.page_backup = 0;
    $scope.View_thumb_onChange = function (page_num) {
        if ($scope.page_backup != page_num) {
            var pdf_container = document.getElementById("dwtLargeViewer");
            $scope.Large_Viewer_RenderPage(pdf_container, page_num);
            $scope.page_backup = page_num;
        }
       
    }

    $scope.Large_Viewer_RenderPage = function (pdf_container,page_num) {
        $scope.loading = true;
        debugger
        pdf_container.replaceChildren();

        $('#dwtLargeViewer').empty();
        pdfDoc.getPage(page_num).then(function (page) {
                //Create Canvas element and append to the Container DIV.
                debugger

                var div_add = document.createElement('div');
                const att = document.createAttribute("class");
                att.value = "col-md-12";
                div_add.setAttributeNode(att);
                pdf_container.appendChild(div_add);
                pdf_container.style = 'max-height: 650px;overflow:scroll';


                var canvas = document.createElement('canvas');
                canvas.id = 'Large-pdf-Viewer';
                canvas.style = 'zoom: .5'

                var ctx = canvas.getContext('2d');

                div_add.appendChild(canvas);


                //Create and add empty DIV to add SPACE between pages.
                var spacer = document.createElement("div");
                spacer.style.height = "20px";
                pdf_container.appendChild(spacer);

                //Set the Canvas dimensions using ViewPort and Scale.
                var viewport = page.getViewport({ scale: 1 });
                canvas.height = 3 * viewport.height;
                canvas.width = 3 * viewport.width;

                //Render the PDF page.
                var renderContext = {
                    canvasContext: ctx,
                    viewport: viewport,
                    transform: [3, 0, 0, 3, 0, 0]
                };

                page.render(renderContext);

                $scope.loading = false;

            });

        
    };
    $scope.RotateImage = function () {
        $scope.loading = true;

        
        if ($scope.page_backup > 0) {
            //var fileURL = URL.createObjectURL(file);

            $scope.Rotate_Page($scope.PDF_Images, $scope.page_backup, 90);
            $scope.loading = false;


        } else {
            toastr.error("Please Select at least one page to rotate");
            $scope.loading = false;

        }



    }
    $scope.Rotate_Page = function (file, page_num, angle) {
        $scope.loading = true;
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');

        if (file != undefined && page_num > 0 && cookie != "") {
            ImageProcessServices.rotate(file, page_num, angle, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });

                pdfjsLib.getDocument(data).promise.then(function (pdfDoc_) {
                    debugger
                    pdfDoc = pdfDoc_;

                    var pdf_container = document.getElementById("dwtLargeViewer");

                    $scope.Large_Viewer_RenderPage(pdf_container, $scope._rotate_page_no);

                });
                $scope.loading = false;

            }, function (error) {
                console.log(error.data);
                alert(error.data);
                $scope.loading = false;

            });


        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;

            }
        }
    }
    $scope.getCookie = function (cname) {
        let name = cname + "=";
        let ca = document.cookie.split(';');
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1);
            }
            if (c.indexOf(name) == 0) {
                return c.substring(name.length, c.length);
            }

        }


        return "";
    }
}]);

