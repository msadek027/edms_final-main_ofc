app.controller('PrintWithBatchNoCtr', ['$scope', '$http', 'ImageProcessServices', 'environmentServices', '$compile', '$injector', '$interval', function ($scope, $http, ImageProcessServices, environmentServices, $compile, $injector, $interval) {

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
        var zoomFactor = .5;

        var _left = 0;
        var _right = 0;
        var _top = 0;
        var _bottom = 0;

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

            DWObjectLargeViewer.RegisterEvent('OnImageAreaSelected', function (sImageIndex, left, top, right, bottom) {
                _left = left;
                _right = right;
                _top = top;
                _bottom = bottom;

            });

        };

        $scope.ResetImageViewrs = function () {
            DWObjectQuickViewer.RemoveAllImages();
            DWObject.RemoveAllImages();
            DWObjectLargeViewer.RemoveAllImages();

            DWObjectQuickViewer2.RemoveAllImages();
        }

        $scope.ZoomIn = function () {
            DWObjectLargeViewer.Zoom = zoomFactor * 1.2;
            zoomFactor = zoomFactor * 1.2;
        };

        $scope.ZoomOut = function () {
            DWObjectLargeViewer.Zoom = zoomFactor / 1.2;
            zoomFactor = zoomFactor / 1.2;
        };

        $scope.RotateRight = function () {
            DWObjectLargeViewer.RotateRight(DWObjectLargeViewer.GetSelectedImageIndex(0));
        }

        $scope.LoadImageNew = function () {
            DWObject.IfShowFileDialog = true;
            DWObject.LoadImageEx("", EnumDWT_ImageType.IT_ALL);
        };

        $scope.LoadImage = function (tableRow, e) {
            //DWObject.Addon.PDF.Download("../Resources/addon/Pdf.zip",  // specify the url of the add-on resource
            //    function () { console.log('Successfully Downloaded PDF add-on'); }, // successful callback
            //    function (errorCode, errorString) { alert(errorString); }  // failure callback and show error message
            //);

            //DWObject.Addon.PDF.SetResolution(200);
            //DWObject.Addon.PDF.SetConvertMode(EnumDWT_ConverMode.CM_RENDERALL);

            //DWObject.IfShowFileDialog = true;

            //DWObject.RemoveAllImages();
            //DWObjectQuickViewer.RemoveAllImages();

            debugger;
            // For Remarks
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
            //

            applySecurity();

            DWObjectQuickViewer.Addon.PDF.Download("../Resources/addon/Pdf.zip",   // specify the url of the add-on resource
                function () { console.log('Successfully Downloaded PDF add-on'); }, // successful callback
                function (errorCode, errorString) { alert(errorString); }  // failure callback and show error message
            );
            DWObjectQuickViewer.IfShowFileDialog = false;
            DWObjectQuickViewer.RemoveAllImages();
            DWObjectQuickViewer.Addon.PDF.SetResolution(200);
            DWObjectQuickViewer.Addon.PDF.SetConvertMode(EnumDWT_ConverMode.CM_RENDERALL);

            DWObjectQuickViewer.HTTPDownloadEx(location.host, "DocScanningModule/MultiDocScan/GetFilePassWord_r?" + encodeURIComponent("serverIP") + "=" + encodeURIComponent(tableRow.ServerIP)
                + "&" + encodeURIComponent("ftpPort") + "=" + encodeURIComponent(tableRow.ServerPort)
                + "&" + encodeURIComponent("ftpUserName") + "=" + encodeURIComponent(tableRow.FtpUserName)
                + "&" + encodeURIComponent("ftpPassword") + "=" + encodeURIComponent(tableRow.FtpPassword)
                + "&" + encodeURIComponent("serverURL") + "=" + encodeURIComponent(tableRow.FileServerURL)
                + "&" + encodeURIComponent("documentID") + "=" + encodeURIComponent(tableRow.DocumentID)
                + "&" + encodeURIComponent("isObsolete") + "=" + encodeURIComponent(tableRow.IsObsolutable)
                + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent(tableRow.IsSecured), 4, function () {
                    $('#QuickViewModal').modal('show');
                    $('#serverIp').val(tableRow.ServerIP);
                    $('#ServerPort').val(tableRow.ServerPort);
                    $('#FtpUserName').val(tableRow.FtpUserName);
                    $('#FtpPassword').val(tableRow.FtpPassword);
                    $('#FileServerURL').val(tableRow.FileServerURL);
                    $('#DocumentID').val(tableRow.DocumentID);
                    $('#IsObsolutable').val(tableRow.IsObsolutable);
                }, function () {
                    toastr.error('File Not Found');
                });
        };

        $scope.ShowAddTextDiv = function () {
            $("#AddText").toggleClass("hidden");
        }

        $scope.AddText = function () {

            var from = $("#PgFrom").val() - 1;
            var to = $("#PgTo").val() - 1;

            for (var i = 0; i <= DWObject.HowManyImagesInBuffer - 1; i++) {
                if (i >= from && i <= to) {
                    var s = i;
                    DWObject.CurrentImageIndexInBuffer = i;
                    DWObject.CreateTextFont(50, 30, 0, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0, "Arial");
                    DWObject.AddText(i, _left, _top, $("#TextToAdd").val(), 0x0000ff, 0xffffff, 0.5, 0.5);
                }
            }
            $("#AddText").toggleClass("hidden");
            //$scope.DoneEditing();
        }

        $scope.DoneEditing = function () {
            var selectedIndex = DWObject.CurrentImageIndexInBuffer;

            DWObject.CopyToClipboard(0);

            DWObject.LoadDibFromClipboard();

            setTimeout(function () {
                DWObject.SwitchImage(selectedIndex, (DWObject.HowManyImagesInBuffer - 1));
                DWObject.RemoveImage((DWObject.HowManyImagesInBuffer) - 1);
            }, 500);

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
                + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent(true), 4, function () {
                    //$('#QuickViewModal2').modal('show');
                    $scope.Print2();
                }, function () {
                    toastr.error('File Not Found');
                });


        };

        $scope.Print2 = function () {
            //DWObjectQuickViewer2.Print();
            DWObject.Print();
        }

        $scope.DetailPrint = function () {
            $scope.Print();
        };

        $scope.Obj = {
            OwnerLevelID: "",
            OwnerID: "",
            DocCategoryID: "",
            DocTypeID: "",
            DocPropertyID: "",
            DocumentID: ""
        };

        $scope.ShowDetailView = function (tableRow, e) {
            DWObject.Addon.PDF.Download("../Resources/addon/Pdf.zip",  // specify the url of the add-on resource
                function () { console.log('Successfully Downloaded PDF add-on'); }, // successful callback
                function (errorCode, errorString) { alert(errorString); }  // failure callback and show error message
            );

            DWObject.Addon.PDF.SetResolution(200);
            DWObject.Addon.PDF.SetConvertMode(EnumDWT_ConverMode.CM_RENDERALL);

            DWObject.IfShowFileDialog = true;
            DWObject.RemoveAllImages();
            DWObjectLargeViewer.RemoveAllImages();


            $scope.Obj.OwnerLevelID = tableRow.OwnerLevelID;
            $scope.Obj.OwnerID = tableRow.OwnerID;
            $scope.Obj.DocCategoryID = tableRow.DocCategoryID;
            $scope.Obj.DocTypeID = tableRow.DocTypeID;
            $scope.Obj.DocPropertyID = tableRow.DocPropertyID;
            $scope.Obj.DocumentID = tableRow.DocumentID;


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



            DWObject.HTTPDownloadEx(location.host, "DocScanningModule/MultiDocScan/GetFilePassWord_r?" + encodeURIComponent("serverIP") + "=" + encodeURIComponent(tableRow.ServerIP)
                + "&" + encodeURIComponent("ftpPort") + "=" + encodeURIComponent(tableRow.ServerPort)
                + "&" + encodeURIComponent("ftpUserName") + "=" + encodeURIComponent(tableRow.FtpUserName)
                + "&" + encodeURIComponent("ftpPassword") + "=" + encodeURIComponent(tableRow.FtpPassword)
                + "&" + encodeURIComponent("serverURL") + "=" + encodeURIComponent(tableRow.FileServerURL)
                + "&" + encodeURIComponent("documentID") + "=" + encodeURIComponent(tableRow.DocumentID)
                + "&" + encodeURIComponent("isObsolete") + "=" + encodeURIComponent(tableRow.IsObsolutable)
                + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent(tableRow.IsSecured), 4, function () {
                    $('#DetailViewModal').modal('show');
                    $('#serverIp').val(tableRow.ServerIP);
                    $('#ServerPort').val(tableRow.ServerPort);
                    $('#FtpUserName').val(tableRow.FtpUserName);
                    $('#FtpPassword').val(tableRow.FtpPassword);
                    $('#FileServerURL').val(tableRow.FileServerURL);
                    $('#DocumentID').val(tableRow.DocumentID);
                    $('#IsObsolutable').val(tableRow.IsObsolutable);
                }, function () {
                    toastr.error('File Not Found');
                });
        };


        $scope.updateLargeViewer = function () {
            DWObject.CopyToClipboard(DWObject.CurrentImageIndexInBuffer);
            DWObjectLargeViewer.LoadDibFromClipboard();
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
                $scope.ResetImageViewrs();
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
                $scope.ResetImageViewrs();

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
                $scope.ResetImageViewrs();
            }
        });

        $scope.$watch('docPropIdentityModel.SearchBy', function (newVal, oldVal) {
            if (newVal) {
                if (newVal != oldVal) {
                    $scope.BindDataToGrid();
                    $scope.ResetImageViewrs();
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


        var DocMetaValues = {
            MetaValue: "",
            DocMetaID: ""
        };

        var FinalObject = { "DocMetaValues": [] };

        // For Meta Value Edit
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

        //For Document Edit
        $scope.SaveImage = function () {


            $scope.loading = true;




            $http.post('/DocScanningModule/DocModification/UpdateDocumentInfo',
                {
                    _modelDocumentsInfo: $scope.Obj
                })
                .success(function (response) {
                    DWObject.IfShowFileDialog = false;

                    var strFTPServer = response.result[0].ServerIP;
                    DWObject.FTPPort = response.result[0].ServerPort;
                    DWObject.FTPUserName = response.result[0].FtpUserName;
                    DWObject.FTPPassword = response.result[0].FtpPassword;

                    var isSave = DWObject.FTPUploadAllAsPDF(strFTPServer,
                        response.result[0].FileServerUrl + "//" +
                        response.result[0].DocumentID + ".pdf");

                    if (isSave) {
                        $scope.loading = false;
                        toastr.success("Upload Successful");
                        $scope.BindDataToGrid();
                        $scope.ResetImageViewrs();

                        if (response.result[0].FileServerUrl === $scope.OriginalFileServerURL) {

                        } else {
                            $http.post('/DocScanningModule/DocModification/DeleteFtpDocuments',
                                {
                                    serverIP: response.result[0].ServerIP,
                                    uri: $scope.OriginalDocURL,
                                    userName: response.result[0].FtpUserName,
                                    password: response.result[0].FtpPassword
                                })
                                .success(function (response) {
                                    console.log(response);
                                })
                                .error(function (response) {
                                    console.log(response);
                                });
                        }

                    } else {
                        //Take Document Info back to previous stage!

                    }

                })
                .error(function () {
                    $scope.loading = false;
                    toastr.success("Failed to Update Meta Data.");
                });
        };

}]);

