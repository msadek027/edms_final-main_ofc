app.controller('DocumentDodificationCtr', ['$scope', '$http', 'ImageProcessServices', 'environmentServices', '$compile', '$injector', '$interval', function ($scope, $http, ImageProcessServices, environmentServices, $compile, $injector, $interval) {

    'use strict'

    applySecurity();
    $scope.PDF_Images = null;
    $scope.last_page = 0;
    $scope.file_list = [];
    $scope.Zoom_Count = .5;
    $scope.Page_Rotation = [];
    $scope.Blank_Page_Count = [];
    $scope.Uploadable_files = [];
    $scope.Total_Page = 0;
    $scope.response = [];
    $scope.blankpages_ = [];
    $scope.totalPages_ = 0;
    $scope.PDF1 = null;
    $scope.PDF1_remove_list = [];
    $scope.PDF1_remove_list_length = 0;
    $scope.PDF1_Next_Val = 0;
    $scope.PDF2_Next_Val = 0;

    $scope.PDF2 = null;
    $scope.PDF2_remove_list = [];
    $scope.PDF2_remove_list_length = 0;


    $scope.IsLoadImageClicked = false;
    var DWObject;
    var DWObjectLargeViewer;
    var zoomFactor = .5;

    function Dynamsoft_OnReady() {
        DWObject = Dynamsoft.WebTwainEnv.GetWebTwain('dwtHorizontalThumbnil');
        DWObjectLargeViewer = Dynamsoft.WebTwainEnv.GetWebTwain('dwtLargeViewer');

        DWObjectLargeViewer.SetViewMode(-1, -1);
        DWObjectLargeViewer.MaxImagesInBuffer = 1;

        DWObject.SetViewMode(4, -1);
        DWObject.PDFCompressionType = EnumDWT_PDFCompressionType.PDF_JPEG;
        DWObject.JPEGQuality = 20;
        DWObject.FitWindowType = 0;
        DWObject.SelectionImageBorderColor = 0x691254;
        DWObject.ShowPageNumber = true;
        DWObject.IfAppendImage = true;

        DWObject.RegisterEvent('OnMouseDoubleClick', $scope.updateLargeViewer);
        DWObject.RegisterEvent('OnImageAreaSelected', function (sImageIndex, left, top, right, bottom) { DWObject.Erase(sImageIndex, left, top, right, bottom); });

        if (DWObject) {
            var count = DWObject.SourceCount;

            if (count == 0 && Dynamsoft.Lib.env.bMac) {
                DWObject.CloseSourceManager();
                DWObject.ImageCaptureDriverType = 0;
                DWObject.OpenSourceManager();
                count = DWObject.SourceCount;
            }

            for (var i = 0; i < count; i++) {
                document.getElementById("source").options.add(new Option(DWObject.GetSourceNameItems(i), i)); // Get Data Source names from Data Source Manager and put them in a drop-down box
            }
        }
        DWObject.Addon.PDF.Download("../Resources/addon/Pdf.zip",  // specify the url of the add-on resource
            function () { console.log('Successfully Downloaded PDF add-on'); }, // successful callback
            function (errorCode, errorString) { alert(errorString); }  // failure callback and show error message
        );
    };

    $scope.AcquireImage = function () {
        //if (DWObject) {
        //    DWObject.IfAutomaticDeskew = true;
        //    DWObject.IfAutoDiscardBlankpages = false;
        //    DWObject.AcquireImage();
        //}

        if (DWObject) {

            DWObject.IfAutomaticDeskew = true;
            DWObject.IfAutoDiscardBlankpages = false;
            $scope.ShowUploadImageDivVar = false;
            var OnAcquireImageSuccess, OnAcquireImageFailure;

            OnAcquireImageSuccess = OnAcquireImageFailure = function () {
                DWObject.CloseSource();
            };

            DWObject.SelectSourceByIndex(document.getElementById("source").selectedIndex); //Use method SelectSourceByIndex to avoid the 'Select Source' dialog
            DWObject.OpenSource();
            DWObject.IfDisableSourceAfterAcquire = true;	// Scanner source will be disabled/closed automatically after the scan.
            DWObject.AcquireImage(OnAcquireImageSuccess, OnAcquireImageFailure);
        }
    };

    $scope.docPropIdentityModel = {
        DocumentID: "",
        OwnerLevel: { OwnerLevelID: "", LevelName: "" },
        Owner: { OwnerID: "", OwnerName: "" },
        DocCategory: { DocCategoryID: "", DocCategoryName: "" },
        DocType: { DocTypeID: "", DocTypeName: "" },
        DocProperty: { DocPropertyID: "", DocPropertyName: "" },
        SearchBy: 1,
        SearchFor: "",
        Status: "",
        DocCat: '',
        //DocCat: [],
        DocType: '',
        DocMetaValues: []
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

    $scope.ShowConfirmModal = function () {

        $('#ConfirmSave').modal('show');
    };

    $scope.ResetImageViewrs = function () {
        //DWObject.RemoveAllImages();
        //DWObjectLargeViewer.RemoveAllImages();
        var pdf_container = document.getElementById("dwtHorizontalThumbnil");
        pdf_container.replaceChildren();
    }
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
    //$scope.ZoomIn = function () {
    //    DWObjectLargeViewer.Zoom = zoomFactor * 1.2;
    //    zoomFactor = zoomFactor * 1.2;
    //};

    //$scope.ZoomOut = function () {
    //    DWObjectLargeViewer.Zoom = zoomFactor / 1.2;
    //    zoomFactor = zoomFactor / 1.2;
    //};

    $scope.ShowAttributes = 0;

    $scope.Obj = {
        OwnerLevelID: "",
        OwnerID: "",
        DocPropertyID: "",
        DocTypeID: "",
        DocCategoryID: "",
        DocumentID: ""
    };

    $scope.LoadImage = function () {
        DWObject.Addon.PDF.Download("../Resources/addon/Pdf.zip",  // specify the url of the add-on resource
            function () { console.log('Successfully Downloaded PDF add-on'); }, // successful callback
            function (errorCode, errorString) { alert(errorString); }  // failure callback and show error message
        );

        DWObject.Addon.PDF.SetResolution(200);
        DWObject.Addon.PDF.SetConvertMode(EnumDWT_ConverMode.CM_RENDERALL);

        DWObject.IfShowFileDialog = true;

        //DWObject.RemoveAllImages();
        DWObject.LoadImageEx("", EnumDWT_ImageType.IT_ALL);
    };

    $scope.RemoveAllSelectedImages = function () {
        DWObject.RemoveAllSelectedImages();
    }

    $scope.RotateRight = function () {
        DWObject.RotateRight(DWObject.GetSelectedImageIndex(0));
    }

    $scope.ShowMoveImageDiv = function () {
        $("#MoveImage").toggleClass("hidden");
        $scope.ShowUploadImageDivVar = false;
    }
    $scope.MoveImage = function () {
        DWObject.MoveImage(($("#WhichImage").val() - 1), ($("#Where").val() - 1));
        $("#MoveImage").toggleClass("hidden");
    };

    $scope.updateLargeViewer = function () {
        DWObject.CopyToClipboard(DWObject.CurrentImageIndexInBuffer);
        DWObjectLargeViewer.LoadDibFromClipboard();
        $('#viewerModal').modal('show');
    };

    $scope.loading = true;
    $scope.OwnerDisable = true;
    $scope.CatDisable = true;
    $scope.DocTypeDisable = true;
    $scope.DocumentDisable = true;

    $http.get('/DocScanningModule/OwnerProperIdentity/GetOwnerLevel?_OwnerLevelID=')
        .success(function (response) {
            $scope.ownerLevels = response.result;
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
                $scope.OwnerDisable = false;

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
            $scope.CatDisable = false;

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
            $scope.CatDisable = false;

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
            $scope.DocumentDisable = false;

            $http.post('/DocScanningModule/MultiDocScan/GetDocumentProperty',
                {
                    _DocCategoryID: $scope.docPropIdentityModel.DocCat,
                    _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                    _DocTypeID: $scope.docPropIdentityModel.DocType
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

    $scope.$watch('docPropIdentityModel.DocProperty', function (newVal, oldVal) {
        if (newVal) {
            $scope.BindDataToGrid();
            $scope.ResetImageViewrs();
        }
    });

    $scope.BindDataToGrid = function () {
        $scope.loading = true;
        $http.post('/DocScanningModule/OriginalDocSearching/GetDocumentsBySearchParamForVersion',
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
                $scope.GridDisplayCollection = pageable.lstDocSearch;
                $scope.pagingInfo.totalItems = pageable.totalPages;
                mailNotify = pageable.mail;
                if (mailNotify) {
                    $.ajax({
                        url: '/DocScanningModule/OriginalDocSearching/GetMailNotifyAndExpDate',
                        data: { DocumentId: pageable.lstDocSearch[0].DocumentID },
                        type: 'POST',


                        //async: false,
                        success: function (response) {
                            $scope.loading = false;

                            $("#notifyDate").val(ToJavaScriptDate(response.NotifyDate));
                            $("#expDate").val(ToJavaScriptDate(response.ExpDate));
                        },
                        error: function (response) {
                            $scope.loading = false;
                            toastr.error(response.Message);
                        }
                    });
                    $('#MailNotificationDate').show();

                }
                else {
                    $('#MailNotificationDate').hide();
                }
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });
    };
    function ToJavaScriptDate(value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        return dt.getFullYear() + "-" + (dt.getMonth() + 1) + "-" + dt.getDate();
    }
    $scope.SelectImage = function (tableRow, e) {
        $scope.Obj.OwnerLevelID = tableRow.OwnerLevelID;
        $scope.Obj.OwnerID = tableRow.OwnerID;
        $scope.docPropIdentityModel.DocCat = tableRow.DocCategoryID;

        $scope.docPropIdentityModel.DocType = tableRow.DocTypeID;

        $scope.Obj.DocPropertyID = tableRow.DocPropertyID;
        $scope.Obj.DocTypeID = tableRow.DocTypeID;
        $scope.Obj.DocCategoryID = tableRow.DocCategoryID;
        $scope.Obj.DocumentID = tableRow.DocumentID;
        $scope.OriginalDocURL = (tableRow.FileServerURL + "//" + tableRow.DocumentID + ".pdf");

        $scope.OriginalFileServerURL = tableRow.FileServerURL;
        var pdf_container = document.getElementById("dwtHorizontalThumbnil");
        pdf_container.replaceChildren();
        //DWObject.IfShowFileDialog = false;
        //DWObject.RemoveAllImages();
        //DWObject.FTPPort = tableRow.ServerPort;
        //DWObject.FTPUserName = tableRow.FtpUserName;
        //DWObject.FTPPassword = tableRow.FtpPassword;
        //DWObject.FTPDownload(tableRow.ServerIP, (tableRow.FileServerURL + "//" + tableRow.DocumentID + ".pdf"));
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
            $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
            $scope.ZoomRef = data;
            $scope.LoadPdfFromUrl(data);
            $scope.showLoader = false;
            $scope.loading = false;
            //$scope.QuickViewer(data);


        }, function (error) {
            console.log(error.data);
            alert(error.data);
            $scope.loading = false;


        });
        $scope.ShowAttributes = 1;
    };

    var DocMetaValues = {
        MetaValue: "",
        DocPropIdentifyID: ""
    };

    $scope.toggleEdit = function (tableRow) {
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

        $('#addModal').modal('show');
    };

    var DocMetaValues = {
        MetaValue: "",
        DocMetaID: "",
        DocumentID: "",
        Remarks: "",//To Pass notifyDate
        VersionMetaValue: ""//To Pass ExpDate
    };

    var FinalObject = { "DocMetaValues": [] };

    $scope.SaveMeta = function () {
        $scope.loading = true;


        angular.forEach($scope.DocumentsAttributeList, function (item) {

            DocMetaValues.DocMetaID = item.DocMetaID;
            DocMetaValues.MetaValue = item.MetaValue;
            DocMetaValues.DocumentID = $scope.DocumentsAttributeList[0].DocID;
            DocMetaValues.Remarks = $("#notifyDate").val(),//To Pass notifyDate
                DocMetaValues.VersionMetaValue = $("#expDate").val()//To Pass ExpDate
            FinalObject.DocMetaValues.push(DocMetaValues);

            DocMetaValues = {
                MetaValue: "",
                DocMetaID: "",
                DocumentID: "",
                Remarks: "",//To Pass notifyDate
                VersionMetaValue: ""//To Pass ExpDate
            };
        });

        $.ajax({
            url: '/DocScanningModule/OriginalDocSearching/UpdateDocMetaInfoWithMailNotifyDate',
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

    $scope.SaveImage = function () {

        $('#ConfirmSave').modal('hide');
        $scope.loading = true;

        if ($scope.ShowAttributes = 1) {

            $scope.Obj.DocCategoryID = $scope.Obj.DocCategoryID;
            $scope.Obj.DocTypeID = $scope.Obj.DocTypeID;
        }
        else {

            $scope.Obj.DocCategoryID = $scope.docPropIdentityModel.DocCat;
            $scope.Obj.DocTypeID = $scope.docPropIdentityModel.DocType;
        }

        $http.post('/DocScanningModule/DocModification/UpdateDocumentInfo',
            {
                _modelDocumentsInfo: $scope.Obj
            })
            .success(function (response) {
                /*DWObject.IfShowFileDialog = false;*/
                $scope.response = response;
                var strFTPServer = response.result.ServerIP;
                //DWObject.FTPPort = response.result[0].ServerPort;
                //DWObject.FTPUserName = response.result[0].FtpUserName;
                //DWObject.FTPPassword = response.result[0].FtpPassword;

                //var isSave = DWObject.FTPUploadAllAsPDF(strFTPServer,
                //    response.result[0].FileServerUrl + "//" +
                //    response.result[0].DocumentID + ".pdf");

                //var isSave = DWObject.FTPUploadAsMultiPagePDF(strFTPServer,
                //    response.result[0].FileServerUrl + "//" +
                //    response.result[0].DocumentID + ".pdf");
                ImageProcessServices.DocUpload($scope.response.result[0].ServerIP, $scope.response.result[0].ServerPort, $scope.response.result[0].FtpUserName, $scope.response.result[0].FtpPassword, $scope.response.result[0].FileServerUrl + "//" +
                    $scope.response.result[0].DocumentID + ".pdf",
                    $scope.response.result[0].DocumentID + ".pdf", $scope.PDF_Images,
                    $scope.getCookie('access')).then(function (data) {
                        debugger

                        if (data.data == "Success") {
                            toastr.success("Upload Successful");
                            //DocIDsCounter++;
                            //uploadCount++;
                            $scope.BindDataToGrid();
                            $scope.ResetImageViewrs();
                            debugger
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
                            $http.post('/DocScanningModule/VersioningOfOriginalDoc/DeleteVersionDocumentInfo',
                                {
                                    _DocumentIDs: response.DistinctID[DocIDsCounter].DocumentID
                                })
                                .success(function () {
                                    //$scope.loading = false;
                                    $scope.loading = false;
                                    toastr.success("Upload Successful");


                                  
                                })
                                .error(function () {
                                    //$scope.loading = false;
                                    toastr.success("Upload Failed");
                                });

                        }
                        $scope.loading = false;

                    }, function (error) {
                        console.log(error.data);
                        alert(error.data);
                        $scope.loading = false;

                    });
                
            })
            .error(function () {
                $scope.loading = false;
                toastr.success("Failed to Update Meta Data.");
            });
    };


    //------------------

    var pdfjsLib = window['pdfjs-dist/build/pdf'];
    pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.6.347/pdf.worker.min.js';
    var pdfDoc = null;
    var scale = 1; //Set Scale for zooming PDF.
    var resolution = 1; //Set Resolution to Adjust PDF clarity.


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

    $scope.LoadImage_New = function () {
        if (!$scope.IsLoadImageClicked) {
            //$('#file_upload').click();
            var fileUpload = $("#file_upload")[0];
            for (var i = 0; i < fileUpload.files.length; i++) {
                $scope.file_list.push(fileUpload.files[i]);

                $scope.PDF_TO_Images(fileUpload.files[i], 1, 0);
                $scope.IsLoadImageClicked = true;
            }
            $scope.ShowUploadImageDivVar = false;
        } else {
            toastr.error('Already loaded a PDF. Please click on "Load Another PDF" to load rest of the files');
        }

    };
    $scope.PDF_TO_Images = function (file, page_num, angle) {
        var cookie = $scope.getCookie('access');
        $scope.loading = true;

        if (file != undefined && page_num > 0 && cookie != "") {
            ImageProcessServices.rotate(file, page_num, angle, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $scope.LoadPdfFromUrl(data);
                $scope.showLoader = false;
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
    $scope.LoadPdfFromUrl = function (url) {
        //Read PDF from URL.
        $scope.loading = true;

        debugger
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {
            debugger
            pdfDoc = pdfDoc_;
            $scope.Total_Page = pdfDoc.numPages;
            //Reference the Container DIV.
            var pdf_container = document.getElementById("dwtHorizontalThumbnil");
            //pdf_container.style.display = "inline-flex";

            //Loop and render all pages.
            for (var i = 1; i <= pdfDoc.numPages; i++) {

                $scope.RenderPage(pdf_container, i, ($scope.last_page + i));
                var rotation = {
                    page_no: $scope.last_page + i,
                    rotate_degree: 0
                }
                $scope.Page_Rotation.push(rotation);
            }

            $scope.last_page = pdfDoc.numPages;
            $scope.loading = false;

        });
    };
    $scope.RenderPage = function (pdf_container, num, serial) {
        $scope.loading = true;

        pdfDoc.getPage(num).then(function (page) {
            //Create Canvas element and append to the Container DIV.
            debugger
            var div_add = document.createElement('div');
            const att = document.createAttribute("class");
            att.value = "col-md-4";
            div_add.setAttributeNode(att);
            div_add.id = "page_" + serial;

            pdf_container.appendChild(div_add);

            const att_fun_call = document.createAttribute("ng-dblclick");
            att_fun_call.value = "View_Full_page(" + serial + ")";
            div_add.setAttributeNode(att_fun_call);


            var page_input = document.createElement('input');
            page_input.id = 'page-' + serial;
            page_input.type = 'checkbox';
            page_input.value = false;

            var canvas = document.createElement('canvas');
            canvas.id = 'pdf-' + serial;
            canvas.style = 'zoom: .3'

            var ctx = canvas.getContext('2d');

            var page_label = document.createElement("LABEL");
            var t = document.createTextNode("Page - " + serial);
            page_label.setAttribute("for", "Page");
            page_label.appendChild(t);
            div_add.appendChild(page_input);

            div_add.appendChild(page_label);

            div_add.appendChild(canvas);

            //Create and add empty DIV to add SPACE between pages.
            var spacer = document.createElement("div");
            spacer.style.height = "20px";
            pdf_container.appendChild(spacer);

            //Set the Canvas dimensions using ViewPort and Scale.
            var viewport = page.getViewport({ scale: scale });
            canvas.height = resolution * viewport.height;
            canvas.width = resolution * viewport.width;
            debugger

            //Render the PDF page.
            var renderContext = {
                canvasContext: ctx,
                viewport: viewport,
                transform: [resolution, 0, 0, resolution, 0, 0]
            };

            page.render(renderContext);

            $compile(angular.element(document.querySelector('#dwtHorizontalThumbnil')))($scope);
            $scope.loading = false;

        });
    };

    $scope.LoadAnotherImage_New = function () {
        if ($scope.IsLoadImageClicked) {
            //$('#file_upload').click();
            var fileUpload = $("#file_upload")[0];
            var files = fileUpload.files[0];
            for (var i = 0; i < fileUpload.files.length; i++) {
                $scope.file_list.push(fileUpload.files[i]);

                $scope.PDF_TO_Images_AnotherPDF(fileUpload.files[i], 1, 0);
                $scope.IsLoadImageClicked = true;
            }
            $scope.ShowUploadImageDivVar = false;
        } else {
            toastr.error('To load a PDF, Please click on "Load PDF"');
        }

    };
    $scope.PDF_TO_Images_AnotherPDF = function (file, page_num, angle) {
        var cookie = $scope.getCookie('access');
        $scope.loading = true;

        if (file != undefined && page_num > 0 && cookie != "") {
            ImageProcessServices.rotate(file, page_num, angle, cookie).then(function (data) {
                debugger
                //$('#file_upload').click();
                var fileUpload = $("#file_upload")[0];
                $scope.file_list = [];
                $scope.file_list.push(new File([$scope.PDF_Images], 'PDF_0.pdf', { type: 'application/pdf' }));
                $scope.file_list.push(new File([data.data], 'PDF_1.pdf', { type: 'application/pdf' }));
                $scope.Merge_Page($scope.file_list);

                $scope.IsLoadImageClicked = true;

                $scope.ShowUploadImageDivVar = false;


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

    $scope.View_Full_page = function (page_num) {
        debugger

        $('#viewerModal').modal('show');

        $scope.LargeViewer($scope.PDF_Images, page_num);

    }
    $scope.LargeViewer = function (url, page_num) {

        //Read PDF from URL.
        debugger
        $scope.Zoom_Count = 1;
        $scope.page_num_show_large = page_num;
        //url = new Blob([url], 'workerPdf.pdf', { type: 'application/pdf' });
        pdfjsLib.getDocument($scope.ZoomRef).promise.then(function (pdfDoc_) {
            debugger
            pdfDoc = pdfDoc_;

            //Reference the Container DIV.
            var pdf_container = document.getElementById("dwtLargeViewer");
            //pdf_container.style.display = "inline-flex";

            //Loop and render all pages.
            $scope.Large_Viewer_RenderPage(pdf_container, $scope.page_num_show_large);

        });
    };
    $scope.Large_Viewer_RenderPage = function (pdf_container, num) {
        $scope.loading = true;

        pdfDoc.getPage(num).then(function (page) {
            //Create Canvas element and append to the Container DIV.
            debugger
            $('#dwtLargeViewer').empty();
            var div_add = document.createElement('div');
            const att = document.createAttribute("class");
            att.value = "col-md-12";
            div_add.setAttributeNode(att);
            div_add.style = 'overflow: scroll; max-height: 650px';
            pdf_container.appendChild(div_add);


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

            $scope.loading = false;

        });
    };

    $scope.RotateImage = function () {
        $scope.loading = true;

        var page_no = 0;
        var file_count = $scope.file_list.length;
        var align_count = 0;


        for (var i = 1; i <= $scope.last_page; i++) {
            var x = document.getElementById("page-" + i);
            if (x != null && x.checked) {
                var f = $scope.Page_Rotation.findIndex(x => x.page_no == i);
                if (f > -1) {
                    page_no = i;
                    $scope.Page_Rotation[f].rotate_degree = $scope.Page_Rotation[f].rotate_degree + 90;
                    align_count = $scope.Page_Rotation[f].rotate_degree;
                    break;

                }

            }
        }
        if (page_no > 0) {
            $scope.Rotate_Page($scope.PDF_Images, page_no, align_count);
            $scope.loading = false;


        } else {
            toastr.error("Please Select at least one page to rotate");
            $scope.loading = false;

        }



    }
    $scope.Rotate_Page = function (file, page_num, angle) {
        $scope.loading = true;

        var cookie = $scope.getCookie('access');

        if (file != undefined && page_num > 0 && cookie != "") {
            ImageProcessServices.rotate(file, page_num, angle, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;

                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $scope.LoadPdfFromUrl_Generic(data);
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


    $scope.RemoveImage_New = function () {
        $scope.loading = true;

        var page_no = 0;
        var file_count = $scope.file_list.length;
        var align_count = 0;


        for (var i = 1; i <= $scope.last_page; i++) {
            var x = document.getElementById("page-" + i);
            if (x != null && x.checked) {
                page_no = i;
                break;

            }


        }
        if (page_no > 0) {
            $scope.Remove_Page($scope.PDF_Images, page_no);
            $scope.loading = false;


        } else {
            toastr.error("Please Select at least one page to remove");
            $scope.loading = false;

        }



    }
    $scope.Remove_Page = function (file, page_num) {
        $scope.loading = true;

        var cookie = $scope.getCookie('access');

        if (file != undefined && page_num > 0 && cookie != "") {
            ImageProcessServices.removepage(file, page_num, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;

                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $scope.LoadPdfFromUrl_Generic(data);
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
    $scope.Remove_Page_Recursive = function (file, page_num) {
        $scope.loading = true;

        var cookie = $scope.getCookie('access');

        if (file != undefined && page_num > 0 && cookie != "") {
            ImageProcessServices.removepage(file, page_num, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;

                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $scope.LoadPdfFromUrl_Generic(data);
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


    $scope.BlankImage_New = function () {
        $scope.loading = true;

        var page_no = 0;


        for (var i = 1; i <= $scope.last_page; i++) {
            var x = document.getElementById("page-" + i);
            if (x != null && x.checked) {
                page_no = i;
                break;

            }


        }
        if (page_no > 0) {
            $scope.Blank_Page($scope.PDF_Images, page_no);
            $scope.loading = false;
        } else {
            toastr.error("Please Select at least one page to remove");
            $scope.loading = false;
        }

    }
    $scope.Blank_Page = function (file, page_num) {
        $scope.loading = true;

        var cookie = $scope.getCookie('access');

        if (file != undefined && page_num > 0 && cookie != "") {
            $scope.Blank_Page_Count.push(page_num + 1);
            ImageProcessServices.blankpage(file, page_num, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;
                $scope.Total_Page = $scope.Total_Page + 1;
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $scope.LoadPdfFromUrl_Generic(data);
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



    $scope.MoveImage_New = function () {
        $scope.loading = true;
        $scope.ShowUploadImageDivVar = false;
        var from_page_num = $("#WhichImage").val();
        var to_page_num = $("#Where").val();
        $scope.Move_Page($scope.PDF_Images, from_page_num, to_page_num);
        $("#MoveImage").toggleClass("hidden");
        $scope.loading = false;
    }
    $scope.Move_Page = function (file, from_page_num, to_page_num) {
        $scope.loading = true;

        var cookie = $scope.getCookie('access');

        if (file != undefined && from_page_num > 0 && to_page_num > 0 && cookie != "") {
            ImageProcessServices.movepage(file, from_page_num, to_page_num, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;

                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $scope.LoadPdfFromUrl_Generic(data);
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


    $scope.MergeImage_New = function () {
        if ($scope.IsLoadImageClicked) {
            //$('#file_upload').click();
            var fileUpload = $("#file_upload")[0];
            $scope.file_list = [];
            $scope.file_list.push(new File([$scope.PDF_Images], 'PDF_0' + '.pdf', { type: 'application/pdf' }));
            for (var i = 0; i < fileUpload.files.length; i++) {
                var file_ = new File([fileUpload.files[i]], 'PDF_' + (i + 1) + '.pdf', { type: 'application/pdf' });

                $scope.file_list.push(file_);

                $scope.Merge_Page($scope.file_list);

                $scope.IsLoadImageClicked = true;
            }
            $scope.ShowUploadImageDivVar = false;
        } else {
            toastr.error('To load a PDF, Please click on "Load PDF"');
        }

    }
    $scope.Merge_Page = function (files) {
        $scope.loading = true;
        debugger
        var cookie = $scope.getCookie('access');

        if (files != null && files.length > 0 && cookie != "") {
            ImageProcessServices.FileMerge(files, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;

                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                var data1 = new Blob([data.data], { type: 'application/pdf' });
                var fileURL = URL.createObjectURL(data1);

                //var a = document.createElement('a');
                //a.href = fileURL;
                //a.download = 'example.pdf';

                //document.body.appendChild(a);
                //a.click();
                //document.body.removeChild(a);
                $scope.LoadPdfFromUrl_Generic_AnotherPdf(data);
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
    $scope.LoadPdfFromUrl_Generic_AnotherPdf = function (url) {
        $scope.loading = true;

        //Read PDF from URL.
        debugger
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {
            debugger
            pdfDoc = pdfDoc_;
            $scope.Total_Page = $scope.Total_Page + pdfDoc.numPages;
            $scope.last_page = 0;
            //Reference the Container DIV.
            var pdf_container = document.getElementById("dwtHorizontalThumbnil");
            //pdf_container.style.display = "inline-flex";
            pdf_container.replaceChildren();


            //Loop and render all pages.
            for (var i = 1; i <= pdfDoc.numPages; i++) {
                $scope.RenderPage_Generic(pdf_container, i, ($scope.last_page + i));
            }

            $scope.last_page = pdfDoc.numPages;
            $scope.loading = false;

        });

    };

    $scope.LoadPdfFromUrl_Generic = function (url) {
        $scope.loading = true;

        //Read PDF from URL.
        debugger
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {
            debugger
            pdfDoc = pdfDoc_;
            $scope.last_page = 0;
            //Reference the Container DIV.
            var pdf_container = document.getElementById("dwtHorizontalThumbnil");
            //pdf_container.style.display = "inline-flex";
            pdf_container.replaceChildren();


            //Loop and render all pages.
            for (var i = 1; i <= pdfDoc.numPages; i++) {
                $scope.RenderPage_Generic(pdf_container, i, ($scope.last_page + i));
            }

            $scope.last_page = pdfDoc.numPages;
            $scope.loading = false;

        });

    };
    $scope.RenderPage_Generic = function (pdf_container, num, serial) {
        $scope.loading = true;

        pdfDoc.getPage(num).then(function (page) {
            //Create Canvas element and append to the Container DIV.
            debugger
            var div_add = document.createElement('div');
            const att = document.createAttribute("class");
            att.value = "col-md-4";
            div_add.setAttributeNode(att);
            div_add.id = "page_" + serial;

            pdf_container.appendChild(div_add);

            const att_fun_call = document.createAttribute("ng-dblclick");
            att_fun_call.value = "View_Full_page(" + serial + ")";
            div_add.setAttributeNode(att_fun_call);


            var page_input = document.createElement('input');
            page_input.id = 'page-' + serial;
            page_input.type = 'checkbox';
            page_input.value = false;

            var canvas = document.createElement('canvas');
            canvas.id = 'pdf-' + serial;
            canvas.style = 'zoom: .3'

            var ctx = canvas.getContext('2d');

            var page_label = document.createElement("LABEL");
            var t = document.createTextNode("Page - " + serial);
            page_label.setAttribute("for", "Page");
            page_label.appendChild(t);
            div_add.appendChild(page_input);

            div_add.appendChild(page_label);

            div_add.appendChild(canvas);

            //Create and add empty DIV to add SPACE between pages.
            var spacer = document.createElement("div");
            spacer.style.height = "20px";
            pdf_container.appendChild(spacer);

            //Set the Canvas dimensions using ViewPort and Scale.
            var viewport = page.getViewport({ scale: scale });
            canvas.height = resolution * viewport.height;
            canvas.width = resolution * viewport.width;
            debugger

            //Render the PDF page.
            var renderContext = {
                canvasContext: ctx,
                viewport: viewport,
                transform: [resolution, 0, 0, resolution, 0, 0]
            };

            page.render(renderContext);

            $compile(angular.element(document.querySelector('#dwtHorizontalThumbnil')))($scope);


            $scope.loading = false;

        });
    };


    $scope.selectFile = function (event) {
        var file = event.target.files[0];

        // Check if the file is an image or a PDF
        if (file.type === 'image/jpeg' || file.type === 'image/png' || file.type === 'application/pdf') {
            // Process the file further
            // You can send the file to a server for scanning or use JavaScript APIs to process it locally
        } else {
            // Display an error message if the selected file is not supported
            console.error('Unsupported file type');
        }
    };
    var uploadCount = 0;

    $scope.generatePDFfrompdf = function (data, url, blankpages, response) {
        debugger
        $scope.response = response;
        $scope.blankpages_ = blankpages;
        $scope.totalPages_ = 0;
        $scope.PDF1 = data;
        $scope.PDF1_remove_list = [];
        $scope.PDF1_remove_list_length = 0;

        $scope.PDF2 = data;
        $scope.PDF2_remove_list = [];
        $scope.PDF2_remove_list_length = 0;

        pdfjsLib.getDocument(url).promise.then(function (pdf) {
            $scope.totalPages = pdf.numPages;
            for (var i = 0; i < $scope.blankpages_.length; i++) {
                for (var j = 1; j <= $scope.totalPages; j++) {
                    var cx = $scope.PDF1_remove_list.findIndex(x => x == j);
                    var cy = $scope.PDF2_remove_list.findIndex(x => x == j);

                    if (cx < 0 && j >= $scope.blankpages_[i]) {
                        $scope.PDF1_remove_list.push(j);
                    } else if (i > 0 && cy < 0 && (j <= $scope.blankpages_[i - 1] || j == $scope.blankpages_[i])) {
                        $scope.PDF2_remove_list.push(j);
                    }
                }
            }
            $scope.PDF1_remove_list_length = $scope.PDF1_remove_list.length;
            $scope.PDF2_remove_list_length = $scope.PDF2_remove_list.length;

            $scope.PDF1_remove_list = $scope.PDF1_remove_list.sort(function (a, b) {
                return b - a;
            });
            $scope.PDF2_remove_list = $scope.PDF2_remove_list.sort(function (a, b) {
                return b - a;
            });
            $scope.PDF1_Next_Val = $scope.PDF1_remove_list[0];
            $scope.PDF2_Next_Val = $scope.PDF2_remove_list[0];
            $scope.PDF1_Revursive();
            $scope.PDF2_Recursive();

            //for (var i = 0; i < $scope.PDF1_remove_list.length; i++) {

            //    ImageProcessServices.removepage($scope.PDF1, $scope.PDF1_remove_list[i], $scope.getCookie('access')).then(function (data) {
            //        debugger
            //        $scope.ZoomRef = data;
            //        $scope.PDF1_remove_list_length = $scope.PDF1_remove_list_length - 1;
            //        $scope.PDF1 = new Blob([data.data], { type: 'application/pdf' });
            //        if ($scope.PDF1_remove_list_length == 0) {

            //            ImageProcessServices.DocUpload($scope.response.result[0].ServerIP, $scope.response.result[0].ServerPort, $scope.response.result[0].FtpUserName, $scope.response.result[0].FtpPassword, $scope.response.DistinctID[0].FileServerUrl + "//" +
            //                $scope.response.DistinctID[0].DocumentID + ".pdf",
            //                $scope.response.DistinctID[0].DocumentID + ".pdf", $scope.PDF1,
            //                $scope.getCookie('access')).then(function (data) {
            //                    debugger

            //                    if (data.data == "Success") {
            //                        toastr.success("Upload Successful");
            //                        //DocIDsCounter++;
            //                        //uploadCount++;
            //                    } else {
            //                        $http.post('/DocScanningModule/VersioningOfOriginalDoc/DeleteVersionDocumentInfo',
            //                            {
            //                                _DocumentIDs: response.DistinctID[DocIDsCounter].DocumentID
            //                            })
            //                            .success(function () {
            //                                //$scope.loading = false;
            //                                toastr.success("Upload Failed");
            //                            })
            //                            .error(function () {
            //                                //$scope.loading = false;
            //                                toastr.success("Upload Failed");
            //                            });

            //                    }
            //                    $scope.loading = false;

            //                }, function (error) {
            //                    console.log(error.data);
            //                    alert(error.data);
            //                    $scope.loading = false;

            //                });
            //        }
            //        else { }
            //        $scope.loading = false;

            //    }, function (error) {
            //        console.log(error.data);
            //        alert(error.data);
            //        $scope.loading = false;

            //    });
            //}
            //for (var i = 0; i < $scope.PDF2_remove_list.length; i++) {

            //    ImageProcessServices.removepage($scope.PDF2, $scope.PDF2_remove_list[i], $scope.getCookie('access')).then(function (data) {
            //        debugger
            //        $scope.ZoomRef = data;
            //        $scope.PDF2_remove_list_length = $scope.PDF2_remove_list_length - 1;
            //        $scope.PDF2 = new Blob([data.data], { type: 'application/pdf' });
            //        if ($scope.PDF2_remove_list_length == 0) {

            //            ImageProcessServices.DocUpload($scope.response.result[0].ServerIP, $scope.response.result[1].ServerPort, $scope.response.result[1].FtpUserName, $scope.response.result[1].FtpPassword, $scope.response.DistinctID[1].FileServerUrl + "//" +
            //                $scope.response.DistinctID[1].DocumentID + ".pdf",
            //                $scope.response.DistinctID[1].DocumentID + ".pdf", $scope.PDF2,
            //                $scope.getCookie('access')).then(function (data) {
            //                    debugger

            //                    if (data.data == "Success") {
            //                        toastr.success("Upload Successful");
            //                        //DocIDsCounter++;
            //                        //uploadCount++;
            //                    } else {
            //                        $http.post('/DocScanningModule/VersioningOfOriginalDoc/DeleteVersionDocumentInfo',
            //                            {
            //                                _DocumentIDs: response.DistinctID[DocIDsCounter].DocumentID
            //                            })
            //                            .success(function () {
            //                                //$scope.loading = false;
            //                                toastr.success("Upload Failed");
            //                            })
            //                            .error(function () {
            //                                //$scope.loading = false;
            //                                toastr.success("Upload Failed");
            //                            });

            //                    }
            //                    $scope.loading = false;

            //                }, function (error) {
            //                    console.log(error.data);
            //                    alert(error.data);
            //                    $scope.loading = false;

            //                });
            //        }
            //        $scope.loading = false;

            //    }, function (error) {
            //        console.log(error.data);
            //        alert(error.data);
            //        $scope.loading = false;

            //    });
            //}


        });

        $scope.showLoader = false;
        $scope.loading = false;

        if (uploadCount == response.DistinctID.length) {
            $scope.docPropIdentityGridData = response.result;
            $scope.loading = false;
            toastr.success("Upload Successful");
            $scope.BindDataToGrid();
        }
        //else {
        //    deleteFailedDocuments(response.DistinctID)
        //}
    };

    $scope.PDF1_Revursive = function () {
        ImageProcessServices.removepage($scope.PDF1, $scope.PDF1_Next_Val, $scope.getCookie('access')).then(function (data) {
            debugger
            $scope.ZoomRef = data;
            $scope.PDF1_remove_list_length = $scope.PDF1_remove_list_length - 1;
            //$scope.PDF1_remove_list = $scope.PDF1_remove_list.shift();
            var d = $scope.PDF1_remove_list.length - $scope.PDF1_remove_list_length;
            $scope.PDF1_Next_Val = $scope.PDF1_remove_list[d];
            $scope.PDF1 = new Blob([data.data], { type: 'application/pdf' });
            if ($scope.PDF1_remove_list_length == 0) {

                ImageProcessServices.DocUpload($scope.response.result[0].ServerIP, $scope.response.result[0].ServerPort, $scope.response.result[0].FtpUserName, $scope.response.result[0].FtpPassword, $scope.response.DistinctID[0].FileServerUrl + "//" +
                    $scope.response.DistinctID[0].DocumentID + ".pdf",
                    $scope.response.DistinctID[0].DocumentID + ".pdf", $scope.PDF1,
                    $scope.getCookie('access')).then(function (data) {
                        debugger

                        if (data.data == "Success") {
                            toastr.success("Upload Successful");
                            //DocIDsCounter++;
                            //uploadCount++;
                        } else {
                            $http.post('/DocScanningModule/VersioningOfOriginalDoc/DeleteVersionDocumentInfo',
                                {
                                    _DocumentIDs: response.DistinctID[DocIDsCounter].DocumentID
                                })
                                .success(function () {
                                    //$scope.loading = false;
                                    toastr.success("Upload Failed");
                                })
                                .error(function () {
                                    //$scope.loading = false;
                                    toastr.success("Upload Failed");
                                });

                        }
                        $scope.loading = false;

                    }, function (error) {
                        console.log(error.data);
                        alert(error.data);
                        $scope.loading = false;

                    });
            }
            else {
                $scope.PDF1_Revursive();
            }
            $scope.loading = false;

        }, function (error) {
            console.log(error.data);
            alert(error.data);
            $scope.loading = false;

        });
    }
    $scope.PDF2_Recursive = function () {
        ImageProcessServices.removepage($scope.PDF2, $scope.PDF2_Next_Val, $scope.getCookie('access')).then(function (data) {
            debugger
            $scope.ZoomRef = data;
            $scope.PDF2_remove_list_length = $scope.PDF2_remove_list_length - 1;
            //$scope.PDF2_remove_list = $scope.PDF2_remove_list.shift();
            var c = $scope.PDF2_remove_list.length - $scope.PDF2_remove_list_length;
            $scope.PDF2_Next_Val = $scope.PDF2_remove_list[c];
            $scope.PDF2 = new Blob([data.data], { type: 'application/pdf' });
            if ($scope.PDF2_remove_list_length == 0) {

                ImageProcessServices.DocUpload($scope.response.result[0].ServerIP, $scope.response.result[1].ServerPort, $scope.response.result[1].FtpUserName, $scope.response.result[1].FtpPassword, $scope.response.DistinctID[1].FileServerUrl + "//" +
                    $scope.response.DistinctID[1].DocumentID + ".pdf",
                    $scope.response.DistinctID[1].DocumentID + ".pdf", $scope.PDF2,
                    $scope.getCookie('access')).then(function (data) {
                        debugger

                        if (data.data == "Success") {
                            toastr.success("Upload Successful");
                            //DocIDsCounter++;
                            //uploadCount++;
                        } else {
                            $http.post('/DocScanningModule/VersioningOfOriginalDoc/DeleteVersionDocumentInfo',
                                {
                                    _DocumentIDs: response.DistinctID[DocIDsCounter].DocumentID
                                })
                                .success(function () {
                                    //$scope.loading = false;
                                    toastr.success("Upload Failed");
                                })
                                .error(function () {
                                    //$scope.loading = false;
                                    toastr.success("Upload Failed");
                                });

                        }
                        $scope.loading = false;

                    }, function (error) {
                        console.log(error.data);
                        alert(error.data);
                        $scope.loading = false;

                    });
            }
            else {
                $scope.PDF2_Recursive();

            }
            $scope.loading = false;

        }, function (error) {
            console.log(error.data);
            alert(error.data);
            $scope.loading = false;

        });
    }
    $scope.createEmptyPDF = function () {
        return new Promise((resolve, reject) => {
            const canvas = document.createElement('canvas');
            canvas.width = 595; // Setting the canvas width and height to match the PDF dimensions
            canvas.height = 842;

            const context = canvas.getContext('2d');

            pdfjsLib.getDocument({}).promise.then((pdf) => {
                const page = pdf.getPage(1); // Get the first page of the PDF document
                const viewport = page.getViewport({ scale: 1 });
                canvas.height = viewport.height;

                const renderContext = {
                    canvasContext: context,
                    viewport: viewport
                };

                page.render(renderContext).promise.then(() => {
                    const dataURI = canvas.toDataURL('application/pdf');
                    resolve(dataURI);
                }).catch((error) => {
                    reject(error);
                });
            }).catch((error) => {
                reject(error);
            });
        });
    }
    $scope.scanToJpg = function () {
        scanner.scan(displayImagesOnPage,
            {
                "output_settings": [
                    {
                        "type": "return-base64",
                        "format": "jpg"
                    }
                ]
            }
        );
    }

    /** Processes the scan result */
    function displayImagesOnPage(successful, mesg, response) {
        if (!successful) { // On error
            console.error('Failed: ' + mesg);
            return;
        }

        if (successful && mesg != null && mesg.toLowerCase().indexOf('user cancel') >= 0) { // User cancelled.
            console.info('User cancelled');
            return;
        }

        var scannedImages = scanner.getScannedImages(response, true, false); // returns an array of ScannedImage
        for (var i = 0; (scannedImages instanceof Array) && i < scannedImages.length; i++) {
            debugger
            var scannedImage = scannedImages[i];

            processScannedImage(scannedImage);
        }
    }

    /** Images scanned so far. */
    var imagesScanned = [];

    /** Processes a ScannedImage */
    function processScannedImage(scannedImage) {
        imagesScanned.push(scannedImage);
        var elementImg = scanner.createDomElementFromModel({
            'name': 'img',
            'attributes': {
                'class': 'scanned',
                'src': scannedImage.src
            }
        });
        debugger
        var sst = scannedImage.src.split(','),
            mime_ = sst[0].match(/:(.*?);/)[1],
            bstr_ = atob(sst[sst.length - 1]),
            n_ = bstr_.length,
            u8arr_ = new Uint8Array(n_);
        while (n_--) {
            u8arr_[n_] = bstr_.charCodeAt(n_);
        }

        $scope.convertImageToPDF(bstr_, 'example.pdf', u8arr_);


    }

    //< !--Previous lines are same as demo - 01, below is new addition to demo - 02 -- >

    /** Upload scanned images by submitting the form */
    function submitFormWithScannedImages() {
        if (scanner.submitFormWithImages('form1', imagesScanned, function (xhr) {
            if (xhr.readyState == 4) { // 4: request finished and response is ready
                document.getElementById('server_response').innerHTML = "<h2>Response from the server: </h2>" + xhr.responseText;
                document.getElementById('images').innerHTML = ''; // clear images
                imagesScanned = [];
            }
        })) {
            document.getElementById('server_response').innerHTML = "Submitting, please stand by ...";
        } else {
            document.getElementById('server_response').innerHTML = "Form submission cancelled. Please scan first.";
        }
    }


    $scope.dataURLtoFile = function (dataurl, filename) {
        var arr = dataurl.split(','),
            mime = arr[0].match(/:(.*?);/)[1],
            bstr = atob(arr[arr.length - 1]),
            n = bstr.length,
            u8arr = new Uint8Array(n);
        while (n--) {
            u8arr[n] = bstr.charCodeAt(n);
        }
        return new File([u8arr], filename, { type: mime });
    }

    //Usage example:
    $scope.scancopy = function () {
        debugger
        //var blob = $scope.dataURLtoFile($scope.src, 'hello');
        //var downloadLink = document.createElement('a');
        //downloadLink.href = URL.createObjectURL(blob);
        //downloadLink.download = 'file.jpg';
        //downloadLink.click();
        //document.getElementById('images').appendChild(elementImg);
        var sst = $scope.src.split(','),
            mime_ = sst[0].match(/:(.*?);/)[1],
            bstr_ = atob(sst[sst.length - 1]),
            n_ = bstr_.length,
            u8arr_ = new Uint8Array(n_);
        while (n_--) {
            u8arr_[n_] = bstr_.charCodeAt(n_);
        }

        $scope.convertImageToPDF(bstr_, 'example.pdf', u8arr_);
        // Optional: Create a download link for the Blob file

    }
    $scope.convertImageToPDF = async function (imageData, pdfPath, u8arr_) {
        const pdfDoc = await PDFLib.PDFDocument.create();
        const imageBytes = u8arr_;


        const image = await pdfDoc.embedJpg(imageBytes);
        const page = pdfDoc.addPage([image.width, image.height]);
        page.drawImage(image, {
            x: 0,
            y: 0,
            width: image.width,
            height: image.height,
        });


        const pdfBytes = await pdfDoc.save();
        const pdfBlob = new Blob([pdfBytes], { type: 'application/pdf' });
        //const pdfURL = URL.createObjectURL(pdfBlob);
        //const downloadLink = document.createElement('a');
        //downloadLink.href = pdfURL;
        //downloadLink.download = pdfPath;
        //downloadLink.click();
        if ($scope.PDF_Images == null) {
            $scope.PDF_TO_Images(pdfBlob, 1, 0);

        } else {
            $scope.PDF_TO_Images_AnotherPDF(pdfBlob, 1, 0);

        }
    }
}]);
