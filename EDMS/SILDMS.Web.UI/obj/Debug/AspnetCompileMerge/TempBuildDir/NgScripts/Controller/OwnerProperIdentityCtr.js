app.controller('OwnerProperIdentityCtr', ['$scope', '$http', 'ImageProcessServices', 'environmentServices', '$compile', '$injector', '$interval', function ($scope, $http, ImageProcessServices, environmentServices, $compile, $injector, $interval) {

    'use strict'
    $scope.Zoom_Count = .5;
    //$scope.PDF_Images;
    applySecurity();
    // applySecurity();
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
    //Dynamsoft.WebTwainEnv.RegisterEvent('OnWebTwainReady', Dynamsoft_OnReady);

    var DWObjectLargeViewer;
    var dwtHorizontalThumbnil;
    var zoomFactor = .5;

    function Dynamsoft_OnReady() {
        dwtHorizontalThumbnil = Dynamsoft.WebTwainEnv.GetWebTwain('dwtHorizontalThumbnil');
        DWObjectLargeViewer = Dynamsoft.WebTwainEnv.GetWebTwain('dwtLargeViewer');

        dwtHorizontalThumbnil.SetViewMode(4, -1);

        DWObjectLargeViewer.SetViewMode(-1, -1);
        DWObjectLargeViewer.MaxImagesInBuffer = 1;

        dwtHorizontalThumbnil.SetViewMode(4, -1);
        dwtHorizontalThumbnil.FitWindowType = 0;
        dwtHorizontalThumbnil.SelectionImageBorderColor = 0x691254;
        dwtHorizontalThumbnil.ShowPageNumber = true;
        dwtHorizontalThumbnil.IfAppendImage = true;

        dwtHorizontalThumbnil.RegisterEvent('OnMouseDoubleClick', $scope.updateLargeViewer);
        dwtHorizontalThumbnil.RegisterEvent('OnImageAreaSelected', function (sImageIndex, left, top, right, bottom) {
            dwtHorizontalThumbnil.Erase(sImageIndex, left, top, right, bottom);
        });

        if (dwtHorizontalThumbnil) {
            var count = DWObject.SourceCount;

            if (count == 0 && Dynamsoft.Lib.env.bMac) {
                dwtHorizontalThumbnil.CloseSourceManager();
                dwtHorizontalThumbnil.ImageCaptureDriverType = 0;
                dwtHorizontalThumbnil.OpenSourceManager();
                count = DWObject.SourceCount;
            }

            for (var i = 0; i < count; i++) {
                document.getElementById("source").options.add(new Option(dwtHorizontalThumbnil.GetSourceNameItems(i), i)); // Get Data Source names from Data Source Manager and put them in a drop-down box
            }
        }

        dwtHorizontalThumbnil.Addon.PDF.Download("../Resources/addon/Pdf.zip",  // specify the url of the add-on resource
            function () { console.log('Successfully Downloaded PDF add-on'); }, // successful callback
            function (errorCode, errorString) { alert(errorString); }  // failure callback and show error message
        );
    };

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


    $scope.LoadImagePDF = function () {
        dwtHorizontalThumbnil.Addon.PDF.Download("../Resources/addon/Pdf.zip",  // specify the url of the add-on resource
            function () { console.log('Successfully Downloaded PDF add-on'); }, // successful callback
            function (errorCode, errorString) { alert(errorString); }  // failure callback and show error message
        );

        dwtHorizontalThumbnil.Addon.PDF.SetResolution(200);
        dwtHorizontalThumbnil.Addon.PDF.SetConvertMode(EnumDWT_ConverMode.CM_RENDERALL);

        dwtHorizontalThumbnil.IfShowFileDialog = true;

        dwtHorizontalThumbnil.RemoveAllImages();
        dwtHorizontalThumbnil.LoadImageEx("", EnumDWT_ImageType.IT_PDF, function () { }, function () { });

        $scope.ShowUploadImageDivVar = false;
    };

    var pdfjsLib = window['pdfjs-dist/build/pdf'];
    pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.6.347/pdf.worker.min.js';
    var pdfDoc = null;
    var scale = 1; //Set Scale for zooming PDF.
    var resolution = 1; //Set Resolution to Adjust PDF clarity.


    $scope.LoadImage = function (tableRow, e) {
        $scope.btnDistribute = true;
        $scope.docPropIdentityGridData2 = tableRow;
        //dwtHorizontalThumbnil.IfShowFileDialog = false;
        //dwtHorizontalThumbnil.RemoveAllImages();

        //dwtHorizontalThumbnil.FTPPort = tableRow.ServerPort;
        //dwtHorizontalThumbnil.FTPUserName = tableRow.FtpUserName;
        //dwtHorizontalThumbnil.FTPPassword = tableRow.FtpPassword;



        if ($scope.docPropIdentityModel.DidtributionOf === 'Original') {

            //dwtHorizontalThumbnil.FTPDownload(tableRow.ServerIP, (tableRow.FileServerURL + "//" + tableRow.DocumentID + ".pdf"));
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

                $scope.LoadPdfFromUrl(data);


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

        }
        else if ($scope.docPropIdentityModel.DidtributionOf === 'Version') {
            //dwtHorizontalThumbnil.FTPDownload(tableRow.ServerIP, (tableRow.FileServerURL + "//" + tableRow.DocVersionID + "_v_" + tableRow.VersionNo + ".pdf"));

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

                $scope.LoadPdfFromUrl(data);


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
        }
    };

    $scope.updateLargeViewer = function () {
        dwtHorizontalThumbnil.CopyToClipboard(dwtHorizontalThumbnil.CurrentImageIndexInBuffer);
        DWObjectLargeViewer.LoadDibFromClipboard();
        $('#viewerModal').modal('show');
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

    $http.get('/DocScanningModule/OwnerProperIdentity/GetOwnerLevel?_OwnerLevelID=')
        .success(function (response) {
            $scope.ownerLevels = response.result;
            $scope.docPropIdentityModel.OwnerLevel = "";
            $scope.docPropIdentityModel.Owner = "";
            $scope.docPropIdentityModel.DocCategory = "";
            $scope.docPropIdentityModel.DocType = "";
            $scope.docPropIdentityModel.DocProperty = "";
            $scope.loading = false;
        })
        .error(function () {
            $scope.loading = false;
        });

    $scope.loadOwner = function () {
        $scope.docPropIdentityModel.Owner = "";
        $scope.docPropIdentityModel.DocCategory = "";
        $scope.docPropIdentityModel.DocType = "";
        $scope.docPropIdentityModel.DocProperty = "";
        $scope.docPropertyForSpecificDocType = "";
        $scope.docPropIdentityGridData = [];

        $http.post('/DocScanningModule/OwnerProperIdentity/GetOwnerForSpecificOwnerLevel',
            { _OwnerLevelID: $scope.docPropIdentityModel.OwnerLevel.OwnerLevelID })
            .success(function (response) {
                $scope.ownersForSpecificOwnerLevel = response.result;
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });
    };

    $scope.loadCategory = function () {
        $scope.docPropIdentityModel.DocCategory = "";
        $scope.docPropIdentityModel.DocType = "";
        $scope.docPropIdentityModel.DocProperty = "";
        $scope.docPropertyForSpecificDocType = "";
        $scope.docPropIdentityGridData = [];

        $http.post('/DocScanningModule/OwnerProperIdentity/GetDocumentCategoriesForSpecificOwner',
            { _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID })
            .success(function (response) {
                $scope.docCategoriesForSpecificOwner = response.result;
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });
    };

    $scope.loadType = function () {
        $scope.docPropIdentityModel.DocType = "";
        $scope.docPropIdentityModel.DocProperty = "";
        $scope.docPropertyForSpecificDocType = "";
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

    $scope.loadPropertyIdentify = function () {
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

    $scope.ResetModel = function () {
    };

    $scope.Save = function () {
        if ($("#fileName").text() === "Drop Files Here") {
            toastr.error("Please upload an excel file.");
        }
        $scope.loading = true;

        $http.post('/DocScanningModule/DocDistribution/AddDocumentInfo',
            {
                modelDocumentsInfo: $scope.docPropIdentityModel,
                selectedPropId: $scope.docPropIdentityModel.DocProperty.DocPropertyID,
                docMetaValues: $scope.docPropIdentityGridData2
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
                    $scope.btnDistribute = false;
                    $scope.docPropIdentityModel = [];
                    $scope.docPropIdentityGridData = [];
                    $scope.GridDisplayCollection = [];
                    UploadFile.ResetFileSelect();
                    dwtHorizontalThumbnil.RemoveAllImages();
                    $scope.pagingInfo.page = 1;
                    $scope.pagingInfo.totalItems = 0;
                    toastr.success(response.Message);
                }
            })
            .error(function () {
                $scope.loading = false;
                toastr.error("Failed to Save Meta Data.");
            });
    };

    $scope.toggleRefreshTable = function (row) {
        $scope.loading = false;
    };


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

        pdfjsLib.getDocument($scope.Url_Ref).promise.then(function (pdfDoc_) {
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

}]);

