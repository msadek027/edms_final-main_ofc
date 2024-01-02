

app.controller('DocumentDistributionSearchCtr', ['$scope', '$http', 'ImageProcessServices', 'environmentServices', '$compile', '$injector', '$interval', function ($scope, $http, ImageProcessServices, environmentServices, $compile, $injector, $interval) {

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


    $scope.DistributionShare = 'Distribution';

/*    var dwtHorizontalThumbnil;*/
 /*   var zoomFactor = .5;*/

    var date = new Date(), y = date.getFullYear(), m = date.getMonth();
    var firstDay = new Date(y, m, 1);
    var dateInFirst = firstDay.getFullYear() + '-' + (firstDay.getMonth() + 1) + '-' + '0' + (firstDay.getDate());

    var currentDate = new Date();
    var dateInmmddyyyy = currentDate.getFullYear() + '-' + (currentDate.getMonth() + 1) + '-' + (currentDate.getDate() + 1);


    $scope.FromDate = dateInFirst;
    $scope.ToDate = dateInmmddyyyy;

    $scope.validateDates = function () {

    };
    $scope.toggleRefreshTable = function (row) {
        location.reload();
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




    var pdfjsLib = window['pdfjs-dist/build/pdf'];
    pdfjsLib.GlobalWorkerOptions.workerSrc = 'https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.6.347/pdf.worker.min.js';
    var pdfDoc = null;
    var scale = 1; //Set Scale for zooming PDF.
    var resolution = 1; //Set Resolution to Adjust PDF clarity.


    $scope.LoadImage = function (tableRow, e) {
        debugger;
        $scope.btnDistribute = true;

        $scope.loading = true;
        $(".mydiv").show();
        if ($scope.DistributionOf === 'Original') {

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
        else if ($scope.DistributionOf === 'Version') {
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
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $scope.Url_Ref = data;
              //  $scope.LoadPdfFromUrl(data);
                $scope.LargeViewer(data)

                $scope.loading = false;
                $(".mydiv").hide();
            }, function (error) {
                console.log(error.data);

                $scope.loading = false;
                $(".mydiv").hide();

            });
        }
    };

    
 

   

   
    $scope.View = function () {

        debugger;
        //$('#dwtHorizontalThumbnil').empty();
        $scope.loading = true;
        $(".mydiv").show();

        $('#dwtLargeViewer').empty();
        $('#dwtVerticalThumbnil').empty();
        $('#dwtLargeViewer').css('min-height', '0');
        $('#dwtLargeViewer').css('max-height', '0');

        $('#dwtVerticalThumbnil').css('min-height', '0');
        $('#dwtVerticalThumbnil').css('max-height', '0');

        $scope.BindDataToGrid();
        $scope.loading = false;
        $(".mydiv").hide();
    }

    $scope.loadDataMode = function () {
        $scope.loading = true;
        $(".mydiv").show();
        // $('#dwtHorizontalThumbnil').empty();
        $('#dwtLargeViewer').empty();
        $('#dwtVerticalThumbnil').empty();
  
        $scope.BindDataToGrid();
        $scope.loading = false;
        $(".mydiv").hide();
    }
    $scope.loadPropertyIdentify = function () {
        $scope.loading = true;
        $(".mydiv").show();
       // $('#dwtHorizontalThumbnil').empty();
        $('#dwtLargeViewer').empty();
        $('#dwtVerticalThumbnil').empty();
        $('#dwtLargeViewer').css('min-height', '0');
        $('#dwtLargeViewer').css('max-height', '0');

        $('#dwtVerticalThumbnil').css('min-height', '0');
        $('#dwtVerticalThumbnil').css('max-height', '0');
        $scope.BindDataToGrid();
        $scope.loading = false;
        $(".mydiv").hide();
    }

    $scope.CloseModalView = function (modalWindowId) {
        $scope.loading = false;
        $('#' + modalWindowId).modal('hide');
    };
    $scope.showPDF = function () {

        const blobUrl = URL.createObjectURL($scope.PDF_Images);
        window.open().document.write('<iframe src="' + blobUrl + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');
    }

   
    

    $scope.BindDataToGrid = function () {
        debugger;

  

        if ($scope.FromDate != null && $scope.FromDate != "" && $scope.FromDate != 'undefined' && $scope.ToDate != null && $scope.ToDate != "" && $scope.ToDate != 'undefined' && $scope.DistributionOf != "--Select--" && $scope.DistributionOf != "" && $scope.DistributionOf != "undefined" ) {

            if ($scope.DistributionOf === "Original" && $scope.DistributionShare == 'Distribution') {
                $scope.loading = true;
                $http.post('/DocScanningModule/DocDistribution/GetDistributionOriginalDocuments',
                    {
                        fromDate: $scope.FromDate, toDate: $scope.ToDate
                    })
                    .success(function (pageable) {
                        $scope.GridDisplayCollection = pageable.lstDocSearch;
                        $scope.loading = false;
                    }).error(function () {
                        $scope.loading = false;
                    });
            }
            if ($scope.DistributionOf === "Version" && $scope.DistributionShare == 'Distribution') {
                $scope.loading = true;
                $http.post('/DocScanningModule/DocDistribution/GetDistributionVersionDocuments',
                    {
                        fromDate: $scope.FromDate, toDate: $scope.ToDate
                    })
                    .success(function (pageable) {
                        $scope.GridDisplayCollection = pageable.lstDocSearch;
                        $scope.loading = false;
                    }).error(function () {
                        $scope.loading = false;
                    });
            }
            if ($scope.DistributionOf === "Original" && $scope.DistributionShare == 'Share') {
                $scope.loading = true;
                $http.post('/DocScanningModule/DocDistribution/GetShareOriginalDocuments',
                    {
                        fromDate: $scope.FromDate, toDate: $scope.ToDate
                    })
                    .success(function (pageable) {
                        $scope.GridDisplayCollection = pageable.lstDocSearch;
                        $scope.loading = false;
                    }).error(function () {
                        $scope.loading = false;
                    });
            }
            if ($scope.DistributionOf === "Version" && $scope.DistributionShare == 'Share') {
                $scope.loading = true;
                $http.post('/DocScanningModule/DocDistribution/GetShareVersionDocuments',
                    {
                        fromDate: $scope.FromDate, toDate: $scope.ToDate
                    })
                    .success(function (pageable) {
                        $scope.GridDisplayCollection = pageable.lstDocSearch;
                        $scope.loading = false;
                    }).error(function () {
                        $scope.loading = false;
                    });
            }
            else {
                $scope.loading = false;
                $(".mydiv").hide();
              
            }
        }
        else {
            $scope.loading = false;
            $(".mydiv").hide();
        }
     
    };
  


    //$scope.LoadPdfFromUrl = function (url) {
    //    $scope.Total_Page = 0;
    //    $scope.last_page = 0;
    //    $scope.loading = true;
    //    $('#dwtHorizontalThumbnil').empty();
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

