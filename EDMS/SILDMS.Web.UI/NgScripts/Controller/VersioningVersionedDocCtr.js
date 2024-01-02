app.controller('VersioningVersionedDocCtr', ['$scope', '$http', 'ImageProcessServices', 'environmentServices', '$compile', '$injector', '$interval', function ($scope, $http, ImageProcessServices, environmentServices, $compile, $injector, $interval) {

    'use strict'
    applySecurity();
    $scope.itemsByPage = 10;
    $scope.loading = true;
    $scope.PDF_Images = null;
    var pdfDoc = null;
    var scale = 1; //Set Scale for zooming PDF.
    var resolution = 1; //Set Resolution to Adjust PDF clarity.
    var DWObject;
    var DWObjectLargeViewer;
    var DWObjectQuickViewer;
    var zoomFactor = .5;

    var _left = 0;
    var _right = 0;
    var _top = 0;
    var _bottom = 0;

    $scope.CloseModalView = function (modalWindowId) {
        $scope.loading = false;
        $('#' + modalWindowId).modal('hide');
    };
    $scope.ZoomIn = function () {    
        var doc_view_ref = document.getElementById('Large-pdf-Viewer');
        var dz = parseFloat($scope.Zoom_Count) + parseFloat('0.2');
        doc_view_ref.style = 'zoom: ' + dz;
        $scope.Zoom_Count = dz;
    };

    $scope.ZoomOut = function () {    

        var doc_view_ref = document.getElementById('Large-pdf-Viewer');
        var dk = parseFloat($scope.Zoom_Count) - parseFloat('0.2');

        doc_view_ref.style = 'zoom: ' + dk;
        $scope.Zoom_Count = dk;
    };

    $scope.Crop = function () {
        if ($scope.page_backup > 0) {
            $scope.Crop_Page($scope.PDF_Images, $scope.page_backup, $scope.startX, $scope.startY, $scope.endX, $scope.endY);
        } else {
            toastr.error("Please Select at least one page to crop");
        }
    }
    $scope.AddWatermark = function () {
        if ($scope.page_backup > 0) {
            //var fileURL = URL.createObjectURL(file);
            debugger;
            var fileUpload = $("#watermark").get(0);
            var files = fileUpload.files;
            const file = files[0]; // Assuming you want to work with the first selected file
            const blob = new Blob([file], { type: 'image/png' });
            var height = $("#watermark_height").val();
            var width = $("#watermark_width").val();
            $scope.watermark_add($scope.PDF_Images, blob, height, width, $scope.page_backup);
        } else {
            toastr.error("Please Select at least one page to add watermark");
        }

        $("#AddWatermark").toggleClass("hidden");
    }
    $scope.AddImage = function () {
        if ($scope.page_backup > 0) {
            //var fileURL = URL.createObjectURL(file);
            debugger;
            var fileUpload = $("#image_file").get(0);
            var files = fileUpload.files;
            const file = files[0]; // Assuming you want to work with the first selected file
            const blob = new Blob([file], { type: 'image/png' });
            $scope.add_Image($scope.PDF_Images, blob, $scope.page_backup, $scope.startX, $scope.startY, $scope.endX, $scope.endY);
        } else {
            toastr.error("Please Select at least one page to add watermark");
        }

        $("#AddImage").toggleClass("hidden");
    }
    $scope.Masking = function () {

        if ($scope.page_backup > 0) {
            //var fileURL = URL.createObjectURL(file);
            $scope.mask_Page($scope.PDF_Images, $scope.page_backup, $scope.startX, $scope.startY, $scope.endX, $scope.endY);

        } else {
            toastr.error("Please Select at least one page to mask");
        }
    }
    $scope.HighlightPage = function () {
        if ($scope.page_backup > 0) {
            //var fileURL = URL.createObjectURL(file);
            $scope.highlight_Page($scope.PDF_Images, $scope.page_backup, $scope.startX, $scope.startY, $scope.endX, $scope.endY);

        } else {
            toastr.error("Please Select at least one page to highlight");
        }
    }
    $scope.RotateRight = function () {
        if ($scope.page_backup > 0) {
            $scope.rotate_Page($scope.PDF_Images, $scope.page_backup, 270);
        } else {
            toastr.error("Please Select at least one page to rotate");

        }
    }
    $scope.Ocr = function () {
        if ($scope.page_backup > 0) {
            $scope.ocr_file($scope.PDF_Images, $scope.page_backup);
        } else {
            toastr.error("Please Select at least one page to mask");
        }
    }

    $scope.ShowWatermarkDiv = function () {
        $("#watermark_width").val("500");
        $("#watermark_height").val("500");
        $("#AddWatermark").toggleClass("hidden");
    }
    $scope.ShowAddImageDiv = function () {
        $("#AddImage").toggleClass("hidden");
    }
    $scope.ShowReSizeDiv = function () {

        $("#NewImageWidth").val("1200");
        $("#NewImageHeight").val("1000");

        $("#ReSize").toggleClass("hidden");
    }
    $scope.ShowAddTextDiv = function () {

        $("#TextToAdd").val("");
        $("#ColorToAddR").val("255");
        $("#ColorToAddG").val("255");
        $("#ColorToAddB").val("0");

        $("#AddText").toggleClass("hidden");
    }
    $scope.ShowMoveImageDiv = function () {
        $("#WhichImage").val("1");
        $("#Where").val("2");
        $("#MoveImage").toggleClass("hidden");
    }
    $scope.ReSize = function () {
        debugger;
        if ($scope.page_backup > 0) {
            $scope.resize_Page($scope.PDF_Images, $scope.page_backup, $("#NewImageWidth").val(), $("#NewImageHeight").val());
        } else {
            toastr.error("Please Select at least one page to resize");
        }
        $("#ReSize").toggleClass("hidden");
    }

    $scope.AddText = function () {

        if ($scope.page_backup > 0) {
            $scope.TestBox_Page($scope.PDF_Images, $scope.page_backup, $("#TextToAdd").val(), $scope.endX, $scope.endY, 2, $("#ColorToAddR").val(), $("#ColorToAddG").val(), $("#ColorToAddB").val(), 10);
        } else {
            toastr.error("Please Select at least one page to add text");

        }

        $("#AddText").toggleClass("hidden");
    }

    $scope.RemovePage = function () {

        if ($scope.page_backup > 0) {
            $scope.remove_Page($scope.PDF_Images, $scope.page_backup);
        } else {
            toastr.error("Please Select at least one page to remove page");
        }

    }
    $scope.MoveImage = function () {
        //DWObject.MoveImage(($("#WhichImage").val() - 1), ($("#Where").val() - 1));
        if ($scope.page_backup > 0) {
            $scope.move_Page($scope.PDF_Images, $("#WhichImage").val(), $("#Where").val());
        } else {
            toastr.error("Please Select at least one page to move image");
        }
        $("#MoveImage").toggleClass("hidden");
    };

    var GetDocumentsAttributeList = function () {
        $http.post('/DocScanningModule/VersioningOfOriginalDoc/GetDocPropIdentityForSpecificDocType',
            {
                _DocCategoryID: $scope.docPropIdentityModel.DocCategory.DocCategoryID,
                _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                _DocTypeID: $scope.docPropIdentityModel.DocType.DocTypeID,
                _DocPropertyID: $scope.docPropIdentityModel.DocProperty.DocPropertyID
            })
            .success(function (response) {
                $scope.DocumentsAttributeList = response.result;
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });
    };
    $scope.showPDF = function () {


        const blobUrl = URL.createObjectURL($scope.PDF_Images);
        window.open().document.write('<iframe src="' + blobUrl + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');

    }
    $scope.LoadImage = function (tableRow, e) {

        debugger;
        var docId = tableRow.DocumentID;
        var docVersionId = tableRow.DocVersionID;
        var version = tableRow.VersionNo;
        var DocVersion = tableRow.DocVersionID + "_v_" + tableRow.VersionNo;



        applySecurity();
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
            $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });

            $scope.QuickViewer(data);
            //---------
            $('#QuickViewModal').modal('show');

            $('#serverIp').val(tableRow.ServerIP);
            $('#ServerPort').val(tableRow.ServerPort);
            $('#FtpUserName').val(tableRow.FtpUserName);
            $('#FtpPassword').val(tableRow.FtpPassword);
            $('#FileServerURL').val(tableRow.FileServerURL);
            $('#DocumentID').val(tableRow.DocumentID);
            $('#IsObsolutable').val(tableRow.IsObsolutable);

            //-------------
            $scope.loading = false;

        }, function (error) {
            console.log(error.data);
            alert(error.data);
            $scope.loading = false;


        });
    };

    $scope.ShowDetailView = function (tableRow, e) {
       

        //DWObject.FTPDownload(tableRow.ServerIP, (tableRow.FileServerURL + "//" + tableRow.DocVersionID + "_v_" + tableRow.VersionNo + ".pdf"));
        GetDocumentsAttributeList();
        applySecurity();

        debugger;
        var docId = tableRow.DocumentID;
        var docVersionId = tableRow.DocVersionID;
        var version = tableRow.VersionNo;
        var DocVersion = tableRow.DocVersionID + "_v_" + tableRow.VersionNo;
        var url = "DocScanningModule/MultiDocScan/GetFilePassWord_r?" + encodeURIComponent("serverIP") + "=" + encodeURIComponent(tableRow.ServerIP)
            + "&" + encodeURIComponent("ftpPort") + "=" + encodeURIComponent(tableRow.ServerPort)
            + "&" + encodeURIComponent("ftpUserName") + "=" + encodeURIComponent(tableRow.FtpUserName)
            + "&" + encodeURIComponent("ftpPassword") + "=" + encodeURIComponent(tableRow.FtpPassword)
            + "&" + encodeURIComponent("serverURL") + "=" + encodeURIComponent(tableRow.FileServerURL)
            + "&" + encodeURIComponent("documentID") + "=" + encodeURIComponent(DocVersion)  
            + "&" + encodeURIComponent("isObsolete") + "=" + encodeURIComponent(false)
            + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent(false)
        $("#DocumentID").val(tableRow.DocumentID);
        $("#DocVersionID").val(tableRow.DocVersionID);
        ImageProcessServices.detailView(url).then(function (data) {
            debugger

            var arrayBuffer = data.data;

            var data = { data: arrayBuffer }
            $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });

            $scope.LargeViewer(data);
            $('#DetailViewModal').modal('show');
           

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

    $scope.CloseWindow = function () {
        $('#DetailViewModal').modal('hide');
    }

    $scope.docPropIdentityModel = {
        OwnerLevel: { OwnerLevelID: "", LevelName: "" },
        Owner: { OwnerID: "", OwnerName: "" },
        DocCategory: { DocCategoryID: "", DocCategoryName: "" },
        DocType: { DocTypeID: "", DocTypeName: "" },
        DocProperty: { DocPropertyID: "", DocPropertyName: "" },
        SearchBy: ""
    };

    var DocMetaValues = {
        VersionMetaValue: "",
        DocPropIdentifyID: ""
    };

    var FinalObject = {
        "OwnerLevelID": "", "OwnerID": "", "DocCategoryID": "", "DocumentID": "", "DocVersionID": "",
        "DocTypeID": "", "DocPropertyID": "", "DocMetaValues": []
    };

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

    $scope.SaveImage = function () {
        //$scope.loading = true;

        angular.forEach($scope.DocumentsAttributeList, function (item) {

            DocMetaValues.DocPropIdentifyID = item.DocPropIdentifyID;
            DocMetaValues.VersionMetaValue = item.MetaValue;

            FinalObject.DocMetaValues.push(DocMetaValues);

            DocMetaValues = {
                VersionMetaValue: "",
                DocPropIdentifyID: ""
            };
        });

        FinalObject.OwnerLevelID = $scope.docPropIdentityModel.OwnerLevel.OwnerLevelID;
        FinalObject.OwnerID = $scope.docPropIdentityModel.Owner.OwnerID;
        FinalObject.DocCategoryID = $scope.docPropIdentityModel.DocCategory.DocCategoryID;
        FinalObject.DocTypeID = $scope.docPropIdentityModel.DocType.DocTypeID;
        FinalObject.DocPropertyID = $scope.docPropIdentityModel.DocProperty.DocPropertyID;
        FinalObject.DocumentID = $("#DocumentID").val();
        FinalObject.DocVersionID = $("#DocVersionID").val();
        FinalObject.PageName = "VersionOfVersionDoc";

        $.ajax({
            url: '/DocScanningModule/VersioningOfOriginalDoc/AddVersionDocumentInfo',
            data: JSON.stringify(FinalObject),
            type: 'POST',
            contentType: 'application/json;',
            dataType: 'json',
            //async: false,
            success: function (response) {
                //DWObject.IfShowFileDialog = false;
                $scope.response = response;
                //alert(response.result.ServerIP);
                var strFTPServer = response.result.ServerIP;
                //DWObject.FTPPort = response.result.ServerPort;
                //DWObject.FTPUserName = response.result.FtpUserName;
                //DWObject.FTPPassword = response.result.FtpPassword;
                ImageProcessServices.DocUpload(strFTPServer, response.result.ServerPort, response.result.FtpUserName, response.result.FtpPassword, response.result.FileServerUrl + "//" +
                    response.result.DocVersionID + "_v_" + response.result.VersionNo + ".pdf",
                    response.result.DocVersionID + "_v_" + response.result.VersionNo + ".pdf", $scope.PDF_Images,
                    $scope.getCookie('access')).then(function (data) {
                        debugger

                        if (data.data == "Success") {
                            $('#DetailViewModal').modal('hide');
                            toastr.success("Upload Successful");
                            $scope.BindDataToGrid();
                            $scope.ResetModel();
                            $scope.docPropIdentityGridData = $scope.response.result;
                        }
                        else {
                            $scope.loading = false;
                            $('#DetailViewModal').modal('hide');
                            toastr.success("Upload Failed");
                            // Meta Data Delete Request.

                            $http.post('/DocScanningModule/VersioningOfOriginalDoc/DeleteVersionDocumentInfo',
                                {
                                    _DocumentIDs: response.result.DocVersionID
                                })
                                .success(function () {
                                    $scope.loading = false;
                                    toastr.success("Upload Failed");
                                })
                                .error(function () {
                                    $scope.loading = false;
                                });

                        }
                        $scope.loading = false;

                    }, function (error) {
                        console.log(error.data);
                        alert(error.data);
                        $scope.loading = false;

                    });

            },
            error: function (response) {
                $scope.loading = false;
                toastr.success("Failed to Save Meta Data.");
            }
        });

        FinalObject = {
            "OwnerLevelID": "", "OwnerID": "", "DocCategoryID": "",
            "DocTypeID": "", "DocPropertyID": "", "DocMetaValues": []
        };
    };

    $scope.itemsByPage = 10;
    $scope.loading = true;

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
                $scope.docPropIdentityGridData = [];

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
            $scope.docPropIdentityGridData = [];

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
            $scope.docPropIdentityGridData = [];

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
    });

    $scope.$watch('docPropIdentityModel.DocProperty', function (newVal) {

        if (newVal) {
            $scope.docPropIdentityModel.SearchBy = "";
            $scope.docPropIdentityGridData = [];
        }
    });

    $scope.$watch('docPropIdentityModel.SearchBy', function (newVal) {

        if (newVal) {
            $scope.BindDataToGrid();
        }
    });

    $scope.BindDataToGrid = function () {
        $scope.loading = true;
        $http.post('/DocScanningModule/VersionDocSearching/GetVersionDocBySearchParam',
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
                $scope.GridDisplayCollection = pageable.result;
                $scope.pagingInfo.totalItems = pageable.totalPages;
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });
    };

    $scope.QuickViewer = function (url) {
        debugger
        $scope.Zoom_Count = 1;
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {
            debugger
            pdfDoc = pdfDoc_;

            var pdf_container = document.getElementById("dwtQuickViewer");
            pdf_container.replaceChildren();
            $scope.Quick_Viewer_RenderPage(pdf_container);

        });
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
                canvas.id = 'Large-pdf-Viewer_' + i;
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
                canvas.id = 'Large-thumb-Viewer_' + i;
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

    $scope.page_backup = 0;
    $scope.View_thumb_onChange = function (page_num) {
        if ($scope.page_backup != page_num) {
            var pdf_container = document.getElementById("dwtLargeViewer");
            pdf_container.replaceChildren();
            $scope.Large_Viewer_RenderPage(pdf_container, page_num);
            $scope.page_backup = page_num;
        }

    }
    $scope.startX = 0;
    $scope.startY = 0;
    $scope.endX = 0;
    $scope.endY = 0;
    $scope.Large_Viewer_RenderPage = function (pdf_container, page_num) {

        $scope.loading = true;
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

            pdf_container.style.maxHeight = '500px';
            pdf_container.style.maxWidth = '650px';
            //pdf_container.style.maxHeight = '600px';
            //pdf_container.style.maxWidth = '950px';
            pdf_container.style.position = 'relative';

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
            //  rectCanvas.style.border = '2px solid blue';
            div_add.appendChild(rectCanvas);

            var rectCtx = rectCanvas.getContext('2d');

            var viewport = page.getViewport({ scale: 1 });
            mainCanvas.height = viewport.height;
            mainCanvas.width = viewport.width;

            rectCanvas.width = mainCanvas.width;
            rectCanvas.height = mainCanvas.height;


            var isDrawing = false;
            var startX, startY, endX, endY;
            var rectangles = [];

            rectCanvas.addEventListener('mousedown', function (e) {
                rectangles = [];
                isDrawing = true;
                startX = e.clientX - rectCanvas.getBoundingClientRect().left;
                startY = e.clientY - rectCanvas.getBoundingClientRect().top;
            });

            rectCanvas.addEventListener('mousemove', function (e) {
                if (!isDrawing) return;
                endX = e.clientX - rectCanvas.getBoundingClientRect().left;
                endY = e.clientY - rectCanvas.getBoundingClientRect().top;

                rectCtx.clearRect(0, 0, rectCanvas.width, rectCanvas.height);

                for (var i = 0; i < rectangles.length; i++) {
                    var rect = rectangles[i];
                    rectCtx.strokeStyle = 'rgba(255, 0, 0, 0.5)';
                    rectCtx.lineWidth = 3;
                    rectCtx.strokeRect(rect.startX, rect.startY, rect.endX - rect.startX, rect.endY - rect.startY);
                }

                rectCtx.strokeStyle = 'rgba(255, 0, 0, 0.5)';
                rectCtx.lineWidth = 3;
                rectCtx.strokeRect(startX, startY, endX - startX, endY - startY);
            });

            rectCanvas.addEventListener('mouseup', function () {
                isDrawing = false;
                rectangles.push({ startX: startX, startY: startY, endX: endX, endY: endY });

                console.log("Selected area: startX=" + startX + ", startY=" + startY + ", endX=" + endX + ", endY=" + endY);

                $scope.startX = startX; $scope.startY = startY; $scope.endX = endX; $scope.endY = endY;
                $scope.$apply();

                //var vl = startX + ", " + startY + ", " + endX + ", " + endY;
                //$("#TextId").val(vl);           

            });

            var mainRenderContext = {
                canvasContext: mainCtx,
                viewport: viewport,
                transform: [1, 0, 0, 1, 0, 0]
            };

            page.render(mainRenderContext);
            $compile(angular.element(document.querySelector('#dwtLargeViewer')))($scope);

            $scope.loading = false;
        });
    };
    $scope.Large_Viewer_RenderPage22 = function (pdf_container, page_num) {
        $scope.loading = true;
        debugger
        pdf_container.replaceChildren();

        $('#dwtLargeViewer').empty();
        pdfDoc.getPage(parseInt(page_num)).then(function (page) {
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
            canvas.style = 'zoom: 1';
            const att_fun_call1 = document.createAttribute("ng-mousedown");
            att_fun_call1.value = "handleSelection($event)";
            canvas.setAttributeNode(att_fun_call1);
            const att_fun_call2 = document.createAttribute("ng-mouseup");
            att_fun_call2.value = "handleSelectionEnd($event)";
            canvas.setAttributeNode(att_fun_call2);

            var ctx = canvas.getContext('2d');



            div_add.appendChild(canvas);


            //Create and add empty DIV to add SPACE between pages.
            var spacer = document.createElement("div");
            spacer.style.height = "20px";
            pdf_container.appendChild(spacer);

            ////Set the Canvas dimensions using ViewPort and Scale.
            var viewport = page.getViewport({ scale: 1.5 });
            canvas.height = 1 * viewport.height;
            canvas.width = 1 * viewport.width;
            console.log(canvas.height, canvas.width);
            //Render the PDF page.
            var renderContext = {
                canvasContext: ctx,
                viewport: viewport,
                transform: [1, 0, 0, 1, 0, 0]
            };

            page.render(renderContext);
            $compile(angular.element(document.querySelector('#dwtLargeViewer')))($scope);

            $scope.loading = false;

        });


    };


    $scope.startX = 0;
    $scope.startY = 0;
    $scope.endX = 0;
    $scope.endY = 0;
    $scope.height_mul = 0;
    $scope.width_mul = 0;
    $scope.width = 3000;
    $scope.height = 3000;

    var startX, startY, endX, endY;

    $scope.handleSelection = function (event) {
        console.log(window.screen.height * window.devicePixelRatio);
        console.log(window.screen.width * window.devicePixelRatio);

        var canvasRect = document.getElementById("Large-pdf-Viewer");

        //var canvasRect = event.target.getBoundingClientRect();

        if (window.screen.width > 1200 && window.screen.width < 1400) {
            if (window.devicePixelRatio > .60 && window.devicePixelRatio < .70) {
                startX = event.clientX - canvasRect.offsetLeft - 450;
                startY = event.clientY - canvasRect.offsetTop - 205;
            } else if (window.devicePixelRatio > .70 && window.devicePixelRatio <= .80) {
                startX = event.clientX - canvasRect.offsetLeft - 395;
                startY = event.clientY - canvasRect.offsetTop - 140;
            } else {
                startX = event.clientX - canvasRect.offsetLeft - 340;
                startY = event.clientY - canvasRect.offsetTop - 85;
            }
        } else {
            if (window.devicePixelRatio > .60 && window.devicePixelRatio < .70) {
                startX = event.clientX - canvasRect.offsetLeft - 450;
                startY = event.clientY - canvasRect.offsetTop - 205;
            } else if (window.devicePixelRatio > .70 && window.devicePixelRatio <= .80) {
                startX = event.clientX - canvasRect.offsetLeft - 395;
                startY = event.clientY - canvasRect.offsetTop - 140;
            }
            else {
                startX = event.clientX - canvasRect.offsetLeft - 340;
                startY = event.clientY - canvasRect.offsetTop - 85;
            }
        }


        // Update the initial selection coordinates
        $scope.height_mul = $scope.height / canvasRect.height;
        $scope.width_mul = $scope.width / canvasRect.width;



        if (startX < 0) {
            $scope.startX = 0;
        } else {
            $scope.startX = startX * $scope.width_mul;
        }
        if (startY < 0) {
            $scope.startY = 0;
        } else {
            $scope.startY = startY * $scope.height_mul;
        }


    }
    $scope.handleSelectionMove = function (event) {
        var canvasRect = document.getElementById("Large-pdf-Viewer");
        endX = event.clientX - canvasRect.offsetLeft - 450;
        endY = event.clientY - canvasRect.offsetTop - 205;

        // Update the final selection coordinates
        $scope.height_mul = $scope.height / canvasRect.height;
        $scope.width_mul = $scope.width / canvasRect.width;

        $scope.endX = endX * $scope.width_mul;
        $scope.endY = endY * $scope.height_mul;

        // Do something with the selected dimensions here

        // Resets the selection coordinates for the next selection

    }

    $scope.handleSelectionEnd = function (event) {


        var canvasRect = document.getElementById("Large-pdf-Viewer");

        if (window.screen.width > 1200 && window.screen.width < 1400) {
            if (window.devicePixelRatio > .60 && window.devicePixelRatio < .70) {
                endX = event.clientX - canvasRect.offsetLeft - 450;
                endY = event.clientY - canvasRect.offsetTop - 205;
            } else if (window.devicePixelRatio > .70 && window.devicePixelRatio <= .80) {
                endX = event.clientX - canvasRect.offsetLeft - 395;
                endY = event.clientY - canvasRect.offsetTop - 140;
            }
            else {
                endX = event.clientX - canvasRect.offsetLeft - 340;
                endY = event.clientY - canvasRect.offsetTop - 85;
            }
        }
        else {
            if (window.devicePixelRatio > .60 && window.devicePixelRatio < .70) {
                endX = event.clientX - canvasRect.offsetLeft - 450;
                endY = event.clientY - canvasRect.offsetTop - 205;
            } else if (window.devicePixelRatio > .70 && window.devicePixelRatio <= .80) {
                endX = event.clientX - canvasRect.offsetLeft - 395;
                endY = event.clientY - canvasRect.offsetTop - 140;
            }
            else {
                endX = event.clientX - canvasRect.offsetLeft - 340;
                endY = event.clientY - canvasRect.offsetTop - 85;
            }
        }
        // Update the final selection coordinates
        $scope.height_mul = $scope.height / canvasRect.height;
        $scope.width_mul = $scope.width / canvasRect.width;
        if (endX < 0) {
            $scope.endX = 0;
        } else {
            $scope.endX = endX * $scope.width_mul;
        }
        if (endY < 0) {
            $scope.endY = 0;
        } else {
            $scope.endY = endY * $scope.height_mul;
        }
        // Do something with the selected dimensions here

        // Resets the selection coordinates for the next selection

    }


    $scope.watermark_add = function (file, watermark, watermark_height, watermark_width, page_num) {
        $scope.loading = true;
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');
        if (file != undefined && cookie != "") {
            ImageProcessServices.watermark(file, watermark, watermark_width, watermark_height, cookie).then(function (data) {
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


    $scope.add_Image = function (file, image_file, page_num, startX, startY, EndX, EndY) {
        $scope.loading = true;
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');
        if (file != undefined && cookie != "") {
            ImageProcessServices.addImage(file, image_file, page_num, startX, startY, EndX, EndY, cookie).then(function (data) {
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
    $scope.Crop_Page = function (file, page_num, startX, startY, EndX, EndY) {
        $scope.loading = true;
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');
        var dimension = (startX, startY, EndX, EndY)
        if (file != undefined && page_num > 0 && cookie != "") {
            ImageProcessServices.crop(file, page_num, startX, startY, EndX, EndY, cookie).then(function (data) {
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
    $scope.ocr_file = function (file, page_num) {
        $scope.loading = true;
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');
        debugger;
        if (file != undefined && page_num > 0 && cookie != "") {

            const apiEndpoint = 'http://202.59.140.136:8000/api/ocr/';
            const formData = new FormData();
            formData.append('file', file);

            fetch(apiEndpoint, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${cookie}`, },
                body: formData,
            })
                .then(response => {
                    debugger;
                    if (!response.ok) {
                        throw new Error('Network response was not ok');
                    }
                    return response.blob();
                })
                .then(blob => {

                    $scope.ZoomRef = blob;
                    $scope.PDF_Images = new Blob([blob], { type: 'application/octet-stream' });
                    const blobUrl = URL.createObjectURL(blob);

                    window.open().document.write('<iframe src="' + blobUrl + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');


                    // window.open(blobUrl, '_blank');

                    //const container = document.getElementById('dwtLargeViewer2'); // Replace 'your-div-id' with your div's actual ID
                    //// Create an <iframe> element
                    //const iframe = document.createElement('iframe');
                    //iframe.src = blobUrl;
                    //container.style.display = 'flex';
                    //container.style.alignItems = 'center';
                    //container.style.justifyContent = 'center';
                    //iframe.width = '100%'; // Set the width to fit your container
                    //iframe.height = '100%'; // Set the height to fit your container
                    //// Append the <iframe> to the <div>
                    //container.appendChild(iframe);


                    //const popup = window.open('', '_blank', 'width=1000,height=400');
                    //const iframe = document.createElement('iframe');
                    //iframe.src = blobUrl;
                    //iframe.style.width = '100%';
                    //iframe.style.height = '100%';
                    //iframe.style.border = 'none';

                    //// Append the iframe to the popup window's document body
                    //popup.document.body.appendChild(iframe);


                    // Calculate the center position of the screen
                    //const screenWidth = window.screen.width;
                    //const screenHeight = window.screen.height;
                    //const popupWidth = 600; // Replace with your desired width
                    //const popupHeight = 480; // Replace with your desired height

                    //const left = (screenWidth - popupWidth) / 2;
                    //const top = (screenHeight - popupHeight) / 2;

                    //// Open the popup window in the center
                    //const popup = window.open('', '_blank', `width=${popupWidth},height=${popupHeight},left=${left},top=${top}`);

                    //const iframe = document.createElement('iframe');
                    //iframe.src = blobUrl;
                    //iframe.style.width = '100%';
                    //iframe.style.height = '100%';
                    //iframe.style.border = 'none';

                    //// Append the iframe to the popup window's document body
                    //popup.document.body.appendChild(iframe);


                    //const downloadUrl = URL.createObjectURL(zipBlob);               
                    //const downloadLink = document.createElement('a');
                    //downloadLink.href = downloadUrl;
                    //downloadLink.download = 'ocr.pdf'; // Set the desired file name
                    //downloadLink.click();                
                    //URL.revokeObjectURL(downloadUrl);

                    //var pdfUrl2 = URL.createObjectURL(vvBlob);
                    //var vvBlob = new Blob([blob], { type: 'application/octet-stream' });





                    //    $scope.ZoomRef = zipBlob;
                    //    $scope.PDF_Images = new Blob([zipBlob], { type: 'application/pdf' });
                    //    pdfjsLib.getDocument(zipBlob).promise.then(function (pdfDoc_) {
                    //    debugger
                    //    pdfDoc = pdfDoc_;
                    //    var pdf_container = document.getElementById("dwtLargeViewer");
                    //        $scope.Large_Viewer_RenderPage(pdf_container, $scope._rotate_page_no);

                    //        $scope.loading = false;
                    //});

                });

            //ImageProcessServices.ocr(file, cookie).then(function (data) {
            //    debugger
            //    $scope.ZoomRef = data;           
            //    $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
            //  //  $scope.Temp_PDF_Images = new Blob([data.data], { type: 'application/octet-stream' });
            //    var pdfUrl = URL.createObjectURL($scope.PDF_Images);
            //   // window.open(pdfUrl, '_blank');         
            //    //pdfjsLib.getDocument(data).promise.then(function (pdfDoc_) {
            //    //    debugger
            //    //    pdfDoc = pdfDoc_;
            //    //    var pdf_container = document.getElementById("dwtLargeViewer");
            //    //    $scope.Large_Viewer_RenderPage(pdf_container, $scope._rotate_page_no);
            //    //});
            //    $scope.loading = false;

            //}, function (error) {
            //    console.log(error.data);
            //    alert(error.data);
            //    $scope.loading = false;

            //});


        }



        else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;

            }
        }
    }
    //----------------------------------------------------------
    $scope.mask_Page = function (file, page_num, startX, startY, EndX, EndY) {
        $scope.loading = true;
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');
        var dimension = (startX, startY, EndX, EndY)
        if (file != undefined && page_num > 0 && cookie != "") {
            ImageProcessServices.mask(file, page_num, startX, startY, EndX, EndY, cookie).then(function (data) {
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
    $scope.highlight_Page = function (file, page_num, startX, startY, EndX, EndY) {
        $scope.loading = true;
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');
        var dimension = (startX, startY, EndX, EndY)
        if (file != undefined && page_num > 0 && cookie != "") {
            ImageProcessServices.highlight(file, page_num, startX, startY, EndX, EndY, cookie).then(function (data) {
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
    $scope.add_Image = function (file, image_file, page_num, startX, startY, EndX, EndY) {
        $scope.loading = true;
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');
        var dimension = (startX, startY, EndX, EndY)
        if (file != undefined && page_num > 0 && cookie != "") {
            ImageProcessServices.addImage(file, image_file, page_num, startX, startY, EndX, EndY, cookie).then(function (data) {
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


    $scope.rotate_Page = function (file, page_num, angle) {
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
    $scope.resize_Page = function (file, page_num, height, width) {
        debugger;
        $scope.loading = true;
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');
        if (file != undefined && page_num > 0 && cookie != "") {

            debugger;
            ImageProcessServices.resize(file, page_num, height, width, cookie).then(function (data) {
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


    $scope.TestBox_Page = function (file, page_num, text, endx, endy, font_scale, colorR, colorG, colorB, thickness) {
        $scope.loading = true;
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');
        if (file != undefined && page_num > 0 && cookie != "") {

            ImageProcessServices.TestBox(file, page_num, text, endx, endy, font_scale, colorR, colorG, colorB, thickness, cookie).then(function (data) {
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
    $scope.move_Page = function (file, from_page_num, to_page_num) {
        $scope.loading = true;
        $scope._rotate_page_no = to_page_num;
        var cookie = $scope.getCookie('access');
        if (file != undefined && from_page_num > 0 && cookie != "") {

            ImageProcessServices.movepage(file, from_page_num, to_page_num, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });

                pdfjsLib.getDocument(data).promise.then(function (pdfDoc_) {
                    debugger
                    pdfDoc = pdfDoc_;

                    var pdf_container = document.getElementById("dwtLargeViewer");

                    $scope.Large_Viewer_RenderPage(pdf_container, $scope._rotate_page_no);
                    var pdf_container_thumb = document.getElementById("dwtVerticalThumbnil");
                    $scope.Large_Thumb_Viewer_RenderPage(pdf_container_thumb);
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

    $scope.remove_Page = function (file, page_num) {
        $scope.loading = true;
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');
        if (file != undefined && page_num > 0 && cookie != "") {

            ImageProcessServices.removepage(file, page_num, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });

                pdfjsLib.getDocument(data).promise.then(function (pdfDoc_) {
                    debugger
                    pdfDoc = pdfDoc_;

                    var pdf_container = document.getElementById("dwtLargeViewer");

                    $scope.Large_Viewer_RenderPage(pdf_container, $scope._rotate_page_no);
                    var pdf_container_thumb = document.getElementById("dwtVerticalThumbnil");
                    $scope.Large_Thumb_Viewer_RenderPage(pdf_container_thumb);
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
    };
}]);


