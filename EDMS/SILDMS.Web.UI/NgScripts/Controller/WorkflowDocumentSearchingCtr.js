app.controller('workflowDocSearchingCtrl', ['$scope', '$http','$sce', 'ImageProcessServices', 'environmentServices', '$compile', '$injector', '$interval', function ($scope, $http, $sce, ImageProcessServices, environmentServices, $compile, $injector, $interval) {

    'use strict'
    $scope.PDF_Images = null;
    $scope.file_list = [];
    $scope.Zoom_Count = .5;
    $scope.Page_Rotation = [];
    $scope.Total_Page = 0;

    //applySecurity();
    $scope.docPropIdentityModel = {
        OwnerLevel: { OwnerLevelID: "", LevelName: "" },
        Owner: { OwnerID: "", OwnerName: "" },
        DocCategory: { DocCategoryID: "", DocCategoryName: "" },
        DocType: { DocTypeID: "", DocTypeName: "" },
        DocProperty: { DocPropertyID: "", DocPropertyName: "" },
        SearchBy: 1,
        SearchFor: "",
        Status: ""
    };

    $scope.pagingInfo = {
        page: 1,
        itemsPerPage: 20,
        sortBy: null,
        reverse: false,
        search: null,
        totalItems: 0
    };


    $scope.search = function (keyEvent) {
        if (keyEvent.which === 13) {
            $scope.pagingInfo.page = 1;
            $scope.BindDataToGrid();
        }
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

    var preIndex = -1;
    $scope.QuickView = function (item, index) {
        debugger;
        $("#dwtQuickViewer").appendTo("#q_view_loader_" + index + "");
        var targetElementId = "q_view_loader_" + index;  

        var url = "DocScanningModule/MultiDocScan/GetFilePassWord_r?" + encodeURIComponent("serverIP") + "=" + encodeURIComponent(item.ServerIP)
            + "&" + encodeURIComponent("ftpPort") + "=" + encodeURIComponent(item.ServerPort)
            + "&" + encodeURIComponent("ftpUserName") + "=" + encodeURIComponent(item.FtpUserName)
            + "&" + encodeURIComponent("ftpPassword") + "=" + encodeURIComponent(item.FtpPassword)
            + "&" + encodeURIComponent("serverURL") + "=" + encodeURIComponent(item.FileServerURL)
            + "&" + encodeURIComponent("documentID") + "=" + encodeURIComponent(item.DocumentID)
            + "&" + encodeURIComponent("isObsolete") + "=" + encodeURIComponent(false)
            + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent(false)

        ImageProcessServices.detailView(url).then(function (data) {
            debugger;
            var arrayBuffer = data.data;
            var data = { data: arrayBuffer }
            $scope.ZoomRef = data;
            $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
            $('#' + targetElementId).empty();
            $scope.LoadPdfFromUrl_ModalView(data, targetElementId);
 
            $scope.loading = false;

        }, function (error) {
            console.log(error.data);
            alert(error.data);
            $scope.loading = false;
        });



        //if (preIndex != -1) {
        //    $("#showqview_" + preIndex + "").collapse("hide");
        //}
        //preIndex = index;
    };

    ////$("#DetailViewModal").on("hidden.bs.modal", function () {
    ////    $("#dwtQuickViewer").appendTo("#q-view-body");
    ////});

    $scope.Print = function () {
        const blobUrl = URL.createObjectURL($scope.PDF_Images);
        window.open().document.write('<iframe src="' + blobUrl + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');
    };
    $scope.loading = true;
    $http.get('/DocScanningModule/OwnerProperIdentity/GetOwnerLevel?_OwnerLevelID=')
        .success(function (response) {
            $scope.ownerLevels = response.result;
            $scope.docPropIdentityModel.OwnerLevel = "";
            $scope.loading = false;
        })
        .error(function () {
            $scope.loading = false;
        });


    $scope.$watch('docPropIdentityModel.OwnerLevel', function (newVal) {
        if (newVal) {
            $scope.docPropIdentityModel.Owner = "";
            $scope.docPropIdentityModel.DocCategory = "";
            $scope.docPropIdentityModel.DocType = "";
            $scope.docPropIdentityModel.DocProperty = "";
            $http.post('/DocScanningModule/OwnerProperIdentity/GetOwnerForSpecificOwnerLevel',
                { _OwnerLevelID: $scope.docPropIdentityModel.OwnerLevel.OwnerLevelID })
                .success(function (response) {
                    $scope.ownersForSpecificOwnerLevel = response.result;
                    $scope.loading = false;
                }).error(function () {
                    $scope.loading = false;
                });
        }
    });

    $scope.$watch('docPropIdentityModel.Owner', function (newVal) {
        if (newVal) {
            $scope.docPropIdentityModel.DocCategory = "";
            $scope.docPropIdentityModel.DocType = "";
            $scope.docPropIdentityModel.DocProperty = "";
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
            $scope.BindDataToGrid();
        }
    });


    $scope.BindDataToGrid = function () {
        $scope.loading = true;
        $scope.tableData = '';

        $http.post('/WorkflowModule/WorkflowDocSearching/GetDocumentsBySearchParam',
            {
                _DocCategoryID: $scope.docPropIdentityModel.DocCategory.DocCategoryID,
                _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                _DocTypeID: $scope.docPropIdentityModel.DocType.DocTypeID,
                page: $scope.pagingInfo.page,
                itemsPerPage: $scope.pagingInfo.itemsPerPage,
                sortBy: '[ObjectID]',
                reverse: $scope.pagingInfo.reverse,
                search: $scope.pagingInfo.search
            })
            .success(function (pageable) {
                $scope.tableData = $sce.trustAsHtml(pageable.html);
                $scope.pagingInfo.totalItems = pageable.totalPages;
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });
    };

    $scope.Documents = [];

    $scope.viewDetail = function (id) {
        $http.get('/WorkflowModule/WorkflowDocSearching/GetDocumentForSpecificObject?_ObjectID=' + id)
            .success(function (response) {
                $('#DetailViewModal').modal('show');
                $scope.Documents = response;
                $scope.loading = false;
            })
            .error(function () {
                $scope.loading = false;
            });
    };

    $scope.toggleRefreshTable = function (row) {
        location.reload();
    };
    //---------------------------

    var pdfjsLib = window['pdfjs-dist/build/pdf'];
    pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.6.347/pdf.worker.min.js';
    var pdfDoc = null;
    var scale = 1; //Set Scale for zooming PDF.
    var resolution = 1; //Set Resolution to Adjust PDF clarity.


    $scope.LoadPdfFromUrl_ModalView = function (url, targetElementId) {
        $scope.Total_Page = 0;
        $scope.last_page = 0;
        //Read PDF from URL.
        $scope.loading = true;
        debugger
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {
            debugger
            pdfDoc = pdfDoc_;
            $scope.Total_Page = pdfDoc.numPages;
            //Reference the Container DIV.
            var pdf_container = null;
            pdf_container = document.getElementById(targetElementId);
            //pdf_container.style.display = "inline-flex";

            //Loop and render all pages.
            for (var i = 1; i <= pdfDoc.numPages; i++) {

                $scope.RenderPage(pdf_container, i, ($scope.last_page + i), targetElementId);
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
    $scope.RenderPage = function (pdf_container, num, serial, targetElementId) {
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

            $compile(angular.element(document.querySelector('#' + targetElementId)))($scope);
            $scope.loading = false;

        });
    };

    $scope.View_Full_page = function (page_num) {
        debugger;
        $('#viewerModal').modal('show');
        $scope.LargeViewer($scope.PDF_Images, page_num);

    }
    $scope.LargeViewer = function (url, page_num) {

        //Read PDF from URL.
        debugger
        $scope.Zoom_Count = 1;
        $scope.page_num_show_large = page_num;   
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
    $scope.CloseModalView = function (modalWindowId) {
        $scope.loading = false;
        $('#' + modalWindowId).modal('hide');
    };
}]);

