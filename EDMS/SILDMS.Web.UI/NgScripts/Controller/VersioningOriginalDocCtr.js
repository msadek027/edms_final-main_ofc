

app.controller('VersioningOriginalDocCtr', ['$scope', '$http', 'ImageProcessServices', 'environmentServices', '$compile', '$injector', '$interval', function ($scope, $http, ImageProcessServices, environmentServices, $compile, $injector, $interval) {

    'use strict'
    applySecurity();
    $scope.itemsByPage = 10;
    $scope.loading = true;
    $scope.PDF_Images = null;
    var pdfDoc = null;
    var scale = 1; //Set Scale for zooming PDF.
    var resolution = 1; //Set Resolution to Adjust PDF clarity.
  

    //var DWObject;
    //var DWObjectLargeViewer;
    //var DWObjectQuickViewer;
    //var zoomFactor = .5;

    var _left = 0;
    var _right = 0;
    var _top = 0;
    var _bottom = 0;



    $scope.isDisabledCrop = false;
    $scope.isDisabledMasking = false;
    $scope.isDisabledHighlight = false;
    $scope.isDisabledRotate = false;
    $scope.isDisabledRemove = false;
    $scope.isDisabledReSize = false;
    $scope.isDisabledAddText = false;
    $scope.isDisabledMoveImage = false;
    $scope.isDisabledWatermark = false;
    $scope.isDisabledAddImage = false;
    $scope.isDisabledOcr = false;


    $('[data-toggle="tooltip"]').tooltip(); 
    $('[data-toggle="tooltip"]').on('mouseleave', function () {
        $(this).tooltip('hide');
    });
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
        $scope.loading = true;
        $(".mydiv").show();
        if ($scope.page_backup > 0) {
            if ($scope.startX > 0 && $scope.startY > 0 && $scope.endX > 0 && $scope.endY > 0) {
                $scope.Crop_Page($scope.PDF_Images, $scope.page_backup, $scope.startX, $scope.startY, $scope.endX, $scope.endY);
            }
            else {
                $scope.loading = false;
                $(".mydiv").hide();
                toastr.error("Please select a rectangular plot on the page!");
            }
        } else {
            $scope.loading = false;
            $(".mydiv").hide();
            toastr.error("Please Select at least one page to crop!");
        }
    }
    $scope.Masking = function () {
        $scope.loading = true;
        $(".mydiv").show();

        if ($scope.page_backup > 0) {
            if ($scope.startX > 0 && $scope.startY > 0 && $scope.endX > 0 && $scope.endY > 0) {
                $scope.mask_Page($scope.PDF_Images, $scope.page_backup, $scope.startX, $scope.startY, $scope.endX, $scope.endY);
            }
            else {
                $scope.loading = false;
                $(".mydiv").hide();
                toastr.error("Please select a rectangular plot on the page!");
            }

        } else {
            $scope.loading = false;
            $(".mydiv").hide();
            toastr.error("Please Select at least one page to mask");
        }
    }
    $scope.HighlightPage = function () {
        $scope.loading = true;
        $(".mydiv").show();
        if ($scope.page_backup > 0) {
            if ($scope.startX > 0 && $scope.startY > 0 && $scope.endX > 0 && $scope.endY > 0) {
            $scope.highlight_Page($scope.PDF_Images, $scope.page_backup, $scope.startX, $scope.startY, $scope.endX, $scope.endY);
        }
            else {
                $scope.loading = false;
                $(".mydiv").hide();
            toastr.error("Please select a rectangular plot on the page!");
        }

        } else {
            $scope.loading = false;
            $(".mydiv").hide();
            toastr.error("Please Select at least one page to highlight");
        }
    }
    $scope.AddWatermark = function () {
        debugger;
        $scope.loading = true;
        $(".mydiv").show();
        var fileUpload = $("#watermark").get(0);
        var files = fileUpload.files;
        const file = files[0]; // Assuming you want to work with the first selected file
        if (file != null && file != "" && file != "undefined" && $("#watermark_height").val() > 0 && $("#watermark_width").val()>0) {
            const blob = new Blob([file], { type: 'image/png' });
            var height = $("#watermark_height").val();
            var width = $("#watermark_width").val();
            $scope.watermark_add($scope.PDF_Images, blob, height, width, $scope.page_backup);
            $("#AddWatermark").toggleClass("hidden");

        }
        else {
            toastr.warning("Please choose a image to add watermark");
            $scope.loading = false;
            $(".mydiv").hide();
        }
          
        
          
         
    }
    $scope.AddImage = function () {
        $scope.loading = true;
        $(".mydiv").show();
        var fileUpload = $("#image_file").get(0);
        var files = fileUpload.files;
        const file = files[0]; // Assuming you want to work with the first selected file

        if ($scope.page_backup > 0 && file != null && file != "" && file != "undefined") {
            debugger;
            if ($scope.startX > 0 && $scope.startY > 0 && $scope.endX > 0 && $scope.endY > 0) {
                const blob = new Blob([file], { type: 'image/png' });
                $scope.add_Image($scope.PDF_Images, blob, $scope.page_backup, $scope.startX, $scope.startY, $scope.endX, $scope.endY);
                $("#AddImage").toggleClass("hidden");
            }
            else {
                toastr.error("Please select a rectangular plot on the page!");
                $(".mydiv").hide();
            }
        }
        else {
            toastr.warning("Please choose a image to add image");
            $scope.loading = false;
            $(".mydiv").hide();
        }

      
    }
   
  
    $scope.RotateRight = function () {
        $scope.loading = true;
        $(".mydiv").show();
        if ($scope.page_backup > 0) {          
            $scope.Rotate_Page_DirectApi($scope.PDF_Images, $scope.page_backup, 270);
        } else {
            $scope.loading = false;
            $(".mydiv").hide();
            toastr.error("Please Select at least one page to rotate");

        }
    }
    $scope.RotateLeft = function () {
        $scope.loading = true;
        $(".mydiv").show();
        if ($scope.page_backup > 0) {
            $scope.Rotate_Page_DirectApi($scope.PDF_Images, $scope.page_backup,-270);
        } else {
            $scope.loading = false;
            $(".mydiv").hide();
            toastr.error("Please Select at least one page to rotate");

        }
    }
    $scope.Ocr = function () {
        $scope.loading = true;
        $(".mydiv").show();
        var cookie = $scope.getCookie('access');
        if ($scope.PDF_Images != undefined  && cookie != "") {      
       
                const apiEndpoint = 'http://202.59.140.136:8000/api/ocr/';
                const formData = new FormData();
                formData.append('file', $scope.PDF_Images);

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
                        $scope.loading = false;
                        $(".mydiv").hide();
                        $scope.startX = 0;
                        $scope.startY = 0;
                        $scope.endX = 0;
                        $scope.endY = 0;
                        $scope.$apply();
                        //window.open().document.write('<iframe src="' + blobUrl + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');


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

                        const container = document.getElementById('dwtOCRViewer'); // Replace 'your-div-id' with your div's actual ID
                        container.style.height = '500px';
                        container.style.width = '900px';
                        // Create an <iframe> element
                        const iframe = document.createElement('iframe');
                        iframe.src = blobUrl;
                        container.style.display = 'flex';
                        container.style.alignItems = 'center';
                        container.style.justifyContent = 'center';
                        iframe.width = '100%'; // Set the width to fit your container
                        iframe.height = '100%'; // Set the height to fit your container
                        // Append the <iframe> to the <div>
                        container.appendChild(iframe);

                        $('#ocrModal').modal('show');
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
              
            //$scope.isDisabledCrop = true;
            //$scope.isDisabledMasking = true;
            //$scope.isDisabledHighlight = true;
            //$scope.isDisabledRotate = true;
            //$scope.isDisabledRemove = true;
            //$scope.isDisabledReSize = true;
            //$scope.isDisabledAddText = true;
            //$scope.isDisabledMoveImage = true;
            //$scope.isDisabledWatermark = true;
            //$scope.isDisabledAddImage = true;

            $scope.isDisabledOcr = false;

        }
        else {
        if (cookie == "") {
            alart("Please log in to generate new access token");
            $scope.loading = false;
            $(".mydiv").hide();

        }
        else {
            toastr.error("Please Select at least one page to mask");
            $(".mydiv").hide();
        }          
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
        var w = $("#NewImageWidth").val();
        var h = $("#NewImageHeight").val();
        if ($scope.page_backup > 0 && h > 0 && w>0) {
            $scope.resize_Page($scope.PDF_Images, $scope.page_backup, $("#NewImageWidth").val(), $("#NewImageHeight").val());
            $("#ReSize").toggleClass("hidden");
        }
        else {
            toastr.error("Please the values according to the fields.");
        }
   }
    function hexToRgb(hex) {
        // Remove the hash (#) character if it's there
        hex = hex.replace(/^#/, '');

        // Parse the hex values into their respective RGB components
        var bigint = parseInt(hex, 16);
        var r = (bigint >> 16) & 255;
        var g = (bigint >> 8) & 255;
        var b = bigint & 255;

        // Return the RGB values as an object
        return { r: r, g: g, b: b };
    }
    $scope.AddText = function () {

        debugger;
        var colorInput = $("#favcolor").val();
      
        // Get the RGB values
        var rgb = hexToRgb(colorInput);

        //var r = $("#ColorToAddR").val();
        //var g = $("#ColorToAddG").val();
        //var b = $("#ColorToAddB").val();
        var r = rgb.r;
        var g = rgb.g;
        var b = rgb.b;
       
        $scope.loading = true;
        $(".mydiv").show();
        if ($scope.page_backup > 0 && $("#TextToAdd").val() != null && $("#TextToAdd").val() != "") {

            //$scope.startX = 0;
            //$scope.startY = 0;
            //$scope.endX = 0;
            //$scope.endY = 0;
            $scope.centerX = ($scope.startX + $scope.endX) / 2;
            $scope.centerY = ($scope.startY + $scope.endY) / 2;


            $scope.TestBox_Page($scope.PDF_Images, $scope.page_backup, $("#TextToAdd").val(), $scope.endX, $scope.endY, 2, r, g, b, 5);

            //  $scope.TestBox_Page($scope.PDF_Images, $scope.page_backup, $("#TextToAdd").val(), $scope.startX, $scope.startY, 2, r,g, b, 10);

           // $scope.TestBox_Page($scope.PDF_Images, $scope.page_backup, $("#TextToAdd").val(), $scope.endX, $scope.endY, 2, r,g, b, 10);


            $("#AddText").toggleClass("hidden");
        } else {
            $scope.loading = false;
            $(".mydiv").hide();
            toastr.error("Please Select at least one page to add text");

        }

       
    }

    $scope.RemovePage= function () {

        if ($scope.page_backup > 0) {
         //   $scope.remove_Page($scope.PDF_Images, $scope.page_backup);
            $scope.remove_Page_DirectApi($scope.PDF_Images, $scope.page_backup);
            
        } else {
            toastr.error("Please Select at least one page to remove page");
        }

    }

 
    $scope.MoveImage = function () {

        $scope.loading = true;
        $(".mydiv").show();
        var w = $("#WhichImage").val();
        var h = $("#Where").val();

        if ($scope.page_backup > 0 && w > 0 && h > 0) {
            $scope.move_Page($scope.PDF_Images, $("#WhichImage").val(), $("#Where").val());
        } else {
            $scope.loading = false;
            $(".mydiv").hide();
            toastr.error("Please the values according to the fields.");
        }
        $("#MoveImage").toggleClass("hidden");
    };



    $scope.LoadImage = function (tableRow, e) {
        debugger;
        $scope.loading = true;
        $(".mydiv").show();
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
            debugger;
            var arrayBuffer = data.data;
            var data = { data: arrayBuffer }
            $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
            $scope.QuickViewer(data);
 
            $('#QuickViewModal').modal('show');

            $scope.loading = false;
            $(".mydiv").hide();

        }, function (error) {
            console.log(error.data);
            alert(error.data);
            $scope.loading = false;
            $(".mydiv").hide();


        });
      
    };
    $scope.showPDF = function () {

        const blobUrl = URL.createObjectURL($scope.PDF_Images);
        window.open().document.write('<iframe src="' + blobUrl + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');


        //const popup = window.open('', '_blank', 'width=1000,height=400');
        //const iframe = document.createElement('iframe');
        //iframe.src = blobUrl;
        //iframe.style.width = '100%';
        //iframe.style.height = '100%';
        //iframe.style.border = 'none';
        //// Append the iframe to the popup window's document body
        //popup.document.body.appendChild(iframe);

        /*
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
                Action: 1,
                addTextAction: addTextAction,
                TextAddPropertyCollection: TextAddPropertyCollection


            }).success(function (response) {
               var arrBuffer = base64ToArrayBuffer(response);

                var file = new Blob([arrBuffer], { type: 'application/pdf' });
                var fileURL = URL.createObjectURL(file);
                window.open().document.write('<iframe src="' + fileURL + '" frameborder="0" style="border:0; top:0px; left:0px; bottom:0px; right:0px; width:100%; height:100%;" allowfullscreen></iframe>');
            });
        addTextAction = 0;
        TextAddPropertyCollection = [];
        */
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

    $scope.ShowDetailView = function (tableRow, e) {
        $scope.loading = true;
        $(".mydiv").show();

     
        GetDocumentsAttributeList();
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
    
          
            var arrayBuffer = data.data;
            var data = { data: arrayBuffer }
            $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
            $scope.LargeViewer(data);            
            $('#DetailViewModal').modal('show');      
            $("#DocumentID").val(tableRow.DocumentID);
            $("#ServerIP").val(tableRow.ServerIP);
            $("#ServerPort").val(tableRow.ServerPort);
            $("#FtpUserName").val(tableRow.FtpUserName);
            $("#FtpPassword").val(tableRow.FtpPassword);
            $("#FileServerURL").val(tableRow.FileServerURL);
            $("#IsObsolutable").val(tableRow.IsObsolutable);
            $("#IsSecured").val(tableRow.IsSecured);
            $scope.loading = false;
            $(".mydiv").hide();     

        }, function (error) {
            console.log(error.data);
            alert(error.data);
            $scope.loading = false;
            $(".mydiv").hide();
        });
   
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
        DocMetaID: {},
        DocPropIdentifyID: {},
        MetaValue: {},
        Remarks: {},
        IdentificationAttribute: {},
        AttributeValue: {},
        UserLevel: {},
        SearchBy: "",
        DocMetaValues: []

    };

    var DocMetaValues = {
        VersionMetaValue: "",
        DocPropIdentifyID: ""
    };

    var FinalObject = {
        "OwnerLevelID": "", "OwnerID": "", "DocCategoryID": "", "DocumentID": "", "PageName": "",
        "DocTypeID": "", "DocPropertyID": "", "DocMetaValues": []
    };
    $scope.ResetModel = function () {
        $('#DetailViewModal').modal('hide');
    }
    $scope.SaveImage = function () {

        $scope.loading = true;
        $(".mydiv").show();
        debugger;
        angular.forEach($scope.DocumentsAttributeList, function (item) {
            DocMetaValues.DocPropIdentifyID = item.DocPropIdentifyID;
            DocMetaValues.VersionMetaValue = item.MetaValue;
            FinalObject.DocMetaValues.push(DocMetaValues);
            DocMetaValues = { VersionMetaValue: "", DocPropIdentifyID: "" };
        });

        FinalObject.OwnerLevelID = $scope.docPropIdentityModel.OwnerLevel.OwnerLevelID;
        FinalObject.OwnerID = $scope.docPropIdentityModel.Owner.OwnerID;
        FinalObject.DocCategoryID = $scope.docPropIdentityModel.DocCategory.DocCategoryID;
        FinalObject.DocTypeID = $scope.docPropIdentityModel.DocType.DocTypeID;
        FinalObject.DocPropertyID = $scope.docPropIdentityModel.DocProperty.DocPropertyID;
        FinalObject.DocumentID = $("#DocumentID").val();
        FinalObject.PageName = "VersionOfOriginalDoc";

        if (FinalObject.DocMetaValues.length > 0) {
            if (FinalObject.DocMetaValues.length == 1) {
                if (FinalObject.DocMetaValues[0].VersionMetaValue == null) {
                    $scope.loading = false;
                    toastr.warning("Please enter the values according to the fields.");
                    return;
                }
            }
            if (FinalObject.DocMetaValues.length == 2) {
                if (FinalObject.DocMetaValues[0].VersionMetaValue == null || FinalObject.DocMetaValues[1].VersionMetaValue == null) {
                    $scope.loading = false;
                    toastr.warning("Please enter the values according to the fields.");
                    return;
                }
            }
       }
        $.ajax({
            url: '/DocScanningModule/VersioningOfOriginalDoc/AddVersionDocumentInfo',
            data: JSON.stringify(FinalObject),
            type: 'POST',
            contentType: 'application/json;',
            dataType: 'json',
            //async: false,
            success: function (response) {
             
                debugger;
                var strFTPServer = response.result.ServerIP;             
                ImageProcessServices.DocUpload(strFTPServer, response.result.ServerPort, response.result.FtpUserName, response.result.FtpPassword, response.result.FileServerUrl + "//" +
                    response.result.DocVersionID + "_v_" + response.result.VersionNo + ".pdf",
                    response.result.DocVersionID + "_v_" + response.result.VersionNo + ".pdf", $scope.PDF_Images,
                    $scope.getCookie('access')).then(function (data) {
                        debugger;

                        if (data.data == "Success") {
                            $('#DetailViewModal').modal('hide');
                            toastr.success("Upload Successful");
                            $scope.BindDataToGrid();
                            $scope.ResetModel();
                            $scope.loading = false;
                            $(".mydiv").hide();
                        } else {
                            $http.post('/DocScanningModule/VersioningOfOriginalDoc/DeleteVersionDocumentInfo',
                                {
                                    _DocumentIDs: response.result.DocVersionID
                                })
                                .success(function () {
                                    //$scope.loading = false;
                                    $('#DetailViewModal').modal('hide');
                                    toastr.success("Upload Failed");
                                    $scope.loading = false;
                                    $(".mydiv").hide();
                                })
                                .error(function () {
                                    //$scope.loading = false;
                                    $('#DetailViewModal').modal('hide');
                                    toastr.success("Upload Failed");
                                    $scope.loading = false;
                                    $(".mydiv").hide();
                                });

                        }
                       
                    }, function (error) {
                        console.log(error.data);
                        alert(error.data);
                        $scope.loading = false;                 
                        $(".mydiv").hide();
                    });

            },
            error: function (response) {
                //$scope.loading = false;
                $('#DetailViewModal').modal('hide');
                toastr.success("Failed to Save Meta Data.");
                $scope.loading = false;
                $(".mydiv").hide();
            }           

        });

        FinalObject = {
            "OwnerLevelID": "",
            "OwnerID": "",
            "DocCategoryID": "",
            "DocTypeID": "",
            "DocPropertyID": "",
            "DocMetaValues": []
        };



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

    $scope.$watch('docPropIdentityModel.DocProperty', function (newVal) {

        if (newVal) {
            $scope.docPropIdentityModel.SearchBy = "";
        }
    });

    $scope.$watch('docPropIdentityModel.SearchBy', function (newVal) {
        if (newVal) {
            $scope.BindDataToGrid();
        }
    });

    $scope.BindDataToGrid = function () {
        $scope.loading = true;
        $http.post('/DocScanningModule/OriginalDocSearching/GetDocumentsBySearchParamForVersion',
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
                search: $scope.pagingInfo.search,
            })
            .success(function (pageable) {
                $scope.GridDisplayCollection = pageable.lstDocSearch;
                $scope.pagingInfo.totalItems = pageable.totalPages;
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });

    };
    $scope.CloseModalView = function (modalWindowId) {
        debugger;
        $scope.loading = false;
        $('#' + modalWindowId).modal('hide');
    };
  
   

    $scope.toggleEdit = function (tableRow) {

        $scope.docPropIdentityModel.DocMetaID = tableRow.DocMetaID;
        $scope.docPropIdentityModel.IdentificationAttribute = tableRow.DocPropIdentifyName;
        $scope.docPropIdentityModel.AttributeValue = (tableRow.MetaValue).toString();
        //$scope.docPropIdentityModel.UserLevel = (tableRow.UserLevel).toString();

        $('#addModal').modal('show');
    };
    $scope.toggleRefreshTable = function (row) {
        location.reload();
    };

    $scope.PopupRefresh = function () {
        $scope.loading = true;
        $(".mydiv").show();
        debugger;
        $scope.startX = 0;
        $scope.startY = 0;
        $scope.endX = 0;
        $scope.endY = 0;

        $scope.isDisabledCrop = false;
        $scope.isDisabledMasking = false;
        $scope.isDisabledHighlight = false;
        $scope.isDisabledRotate = false;
        $scope.isDisabledRemove = false;
        $scope.isDisabledReSize = false;
        $scope.isDisabledAddText = false;
        $scope.isDisabledMoveImage = false;
        $scope.isDisabledWatermark = false;
        $scope.isDisabledAddImage = false;
        $scope.isDisabledOcr = false;

        var DocumentID = $("#DocumentID").val();
        var ServerIP = $("#ServerIP").val();
        var ServerPort = $("#ServerPort").val();
        var FtpUserName = $("#FtpUserName").val();
        var FtpPassword = $("#FtpPassword").val();
        var FileServerURL = $("#FileServerURL").val();

        var IsObsolutable = $("#IsObsolutable").val();
        var IsSecured = $("#IsSecured").val();

        $('#DetailViewModal').modal('hide');

        GetDocumentsAttributeList();
        applySecurity();
        var url = "DocScanningModule/MultiDocScan/GetFilePassWord_r?" + encodeURIComponent("serverIP") + "=" + encodeURIComponent(ServerIP)
            + "&" + encodeURIComponent("ftpPort") + "=" + encodeURIComponent(ServerPort)
            + "&" + encodeURIComponent("ftpUserName") + "=" + encodeURIComponent(FtpUserName)
            + "&" + encodeURIComponent("ftpPassword") + "=" + encodeURIComponent(FtpPassword)
            + "&" + encodeURIComponent("serverURL") + "=" + encodeURIComponent(FileServerURL)
            + "&" + encodeURIComponent("documentID") + "=" + encodeURIComponent(DocumentID)
            + "&" + encodeURIComponent("isObsolete") + "=" + encodeURIComponent(IsObsolutable)
            + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent(IsSecured)

        ImageProcessServices.detailView(url).then(function (data) {
            debugger;
            var arrayBuffer = data.data;
            var data = { data: arrayBuffer }
            $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
            $scope.LargeViewer(data);
            $('#DetailViewModal').modal('show');
           // $("#DocumentID").val(tableRow.DocumentID);
            $scope.loading = false;
            $(".mydiv").hide();

        }, function (error) {
            console.log(error.data);
            alert(error.data);
            $scope.loading = false;
            $(".mydiv").hide();
        });
    };
    $scope.QuickViewer = function (url) {
        debugger;
        $scope.Zoom_Count = 1;
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {
            debugger;
            pdfDoc = pdfDoc_;

            var pdf_container = document.getElementById("dwtQuickViewer");
            pdf_container.replaceChildren();
            $scope.Quick_Viewer_RenderPage(pdf_container);

        });
    };

    $scope.Quick_Viewer_RenderPage = function (pdf_container) {
        $scope.loading = true;
        debugger;
        $('#dwtLargeViewer').empty();
        for (var i = 1; i <= pdfDoc.numPages; i++) {
            pdfDoc.getPage(i).then(function (page) {
                //Create Canvas element and append to the Container DIV.
                debugger

                var div_add = document.createElement('div');
                const att = document.createAttribute("class");
                att.value = "col-md-10";
                div_add.setAttributeNode(att);
    
                pdf_container.appendChild(div_add);
                pdf_container.style = 'min-height: 700px;max-height: 700px;overflow:scroll;';
       

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
        $scope.loading = false;
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
    //$scope.Large_Thumb_Viewer_RenderPage = function (pdf_container) {
    //    $scope.loading = true;
    //    debugger
    //    $('#dwtVerticalThumbnil').empty();
    //    for (var i = 1; i <= pdfDoc.numPages; i++) {
    //        pdfDoc.getPage(i).then(function (page) {
    //            //Create Canvas element and append to the Container DIV.
    //            debugger

    //            var div_add = document.createElement('div');
    //            const att = document.createAttribute("class");
    //            att.value = "col-md-10";
    //            div_add.setAttributeNode(att);
    //            pdf_container.appendChild(div_add);
    //            pdf_container.style = 'min-height: 700px;max-height: 700px;overflow:scroll';

    //            var canvas = document.createElement('canvas');
    //            canvas.id = 'Large-thumb-Viewer_' + i;
    //            canvas.style = 'zoom: .1'
    //           // canvas.style = 'zoom: .5'
    //            var ctx = canvas.getContext('2d');

    //            div_add.appendChild(canvas);
    //            const att_fun_call = document.createAttribute("ng-click");

    //            var page_number = page._pageIndex + 1;
    //            att_fun_call.value = "View_thumb_onChange(" + page_number + ")";
    //            div_add.setAttributeNode(att_fun_call);

    //            //Create and add empty DIV to add SPACE between pages.
    //            var spacer = document.createElement("div");
    //            spacer.style.height = "20px";
    //            pdf_container.appendChild(spacer);

    //            //Set the Canvas dimensions using ViewPort and Scale.
    //            var viewport = page.getViewport({ scale: scale });
    //           // var viewport = page.getViewport({ scale: 1 });
    //            canvas.height = 3 * viewport.height;
    //            canvas.width = 3 * viewport.width;

    //            //Render the PDF page.
    //            var renderContext = {
    //                canvasContext: ctx,
    //                viewport: viewport,
    //                transform: [3, 0, 0, 3, 0, 0]
    //            };
 
         
    //            page.render(renderContext);
    //            $compile(angular.element(document.querySelector('#dwtVerticalThumbnil')))($scope);

    //            $scope.loading = false;

    //        });

    //    }
    //    $scope.loading = false;
    //};
    $scope.Large_Thumb_Viewer_RenderPage = function (pdf_container) {
        $scope.loading = true;
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
            });
        }
        $scope.loading = false;
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
        $(".mydiv").show();
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

            pdf_container.style.minHeight = '700px';
            pdf_container.style.maxHeight = '700px';
            pdf_container.style.minWidth = '100%';
            pdf_container.style.maxWidth = '700px';     

            pdf_container.style.position = 'relative';
            pdf_container.style.overflow = 'auto';
            pdf_container.style.paddingLeft = "0";

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
            rectCanvas.style.border = '1px solid green'; //Add New

            div_add.appendChild(rectCanvas);

            var rectCtx = rectCanvas.getContext('2d');
            var scale_factor = 2.0;
            var viewport = page.getViewport({ scale: scale_factor });
            mainCanvas.height = viewport.height;
            mainCanvas.width = viewport.width;
            mainCanvas.style.border = '1px solid blue'; //Add New          

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


                $scope.startX = startX * scale_factor * 1.035;
                $scope.startY = startY * scale_factor * 1.035;
                $scope.endX = endX * scale_factor * 1.045; //Width
                $scope.endY = endY * scale_factor * 1.040; //Height
                $scope.$apply();               

            });

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

  


  

    
    $scope.watermark_add = function (file, watermark, watermark_height, watermark_width, page_num) {
        $scope.loading = true;
        $(".mydiv").show();
       $scope._rotate_page_no =  page_num;
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

            }, function (error) {
                console.log(error.data);
                alert(error.data);
                $scope.loading = false;
                $(".mydiv").hide();

            });

     

            //$scope.isDisabledCrop = true;
            //$scope.isDisabledMasking = true;
            //$scope.isDisabledHighlight = true;
            //$scope.isDisabledRotate = true;
            //$scope.isDisabledRemove = true;
            //$scope.isDisabledReSize = true;
            //$scope.isDisabledAddText = true;
            //$scope.isDisabledMoveImage = true;
            //$scope.isDisabledWatermark = false;
            //$scope.isDisabledAddImage = true;

            $scope.isDisabledOcr = true;

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $(".mydiv").hide();

            }
        }

    }


    $scope.add_Image = function (file, image_file, page_num, startX, startY, EndX, EndY) {
        $scope.loading = true;
        $(".mydiv").show();
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');
        if (file != undefined && cookie != "") {
            ImageProcessServices.addImage(file, image_file, page_num,startX, startY, EndX, EndY, cookie).then(function (data) {
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
                $("#mydiv").hide();

            });
           // $scope.isDisabled = true;

            //$scope.isDisabledCrop = true;
            //$scope.isDisabledMasking = true;
            //$scope.isDisabledHighlight = true;
            //$scope.isDisabledRotate = true;
            //$scope.isDisabledRemove = true;
            //$scope.isDisabledReSize = true;
            //$scope.isDisabledAddText = true;
            //$scope.isDisabledMoveImage = true;
            //$scope.isDisabledWatermark = true;
            //$scope.isDisabledAddImage = false;

            $scope.isDisabledOcr = true;

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $("#mydiv").hide();

            }
        }
    }
    $scope.Crop_Page = function (file, page_num, startX, startY, EndX, EndY) {
        $scope.loading = true;
        $(".mydiv").show();
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');
        var dimension = (startX, startY, EndX, EndY)
        if (file != undefined && page_num > 0 && cookie != "" && startX > 0 && startY>0 && EndX > 0 && EndY>0) {
            ImageProcessServices.crop(file, page_num, startX, startY, EndX, EndY, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });

                pdfjsLib.getDocument(data).promise.then(function (pdfDoc_) {
                    debugger
                    pdfDoc = pdfDoc_;

                    var pdf_container = document.getElementById("dwtLargeViewer");

                    $scope.Large_Viewer_RenderPage(pdf_container, $scope._rotate_page_no);
                    $scope.loading = false;
                    $(".mydiv").hide();

                    $scope.startX = 0;
                    $scope.startY = 0;
                    $scope.endX = 0;
                    $scope.endY = 0;
                    $scope.$apply();

                });
             

            }, function (error) {
                console.log(error.data);
                alert(error.data);
                $scope.loading = false;
                $(".mydiv").hide();

            });

  

            //$scope.isDisabledCrop = false;
            //$scope.isDisabledMasking = true;
            //$scope.isDisabledHighlight = true;
            //$scope.isDisabledRotate = true;
            //$scope.isDisabledRemove = true;
            //$scope.isDisabledReSize = true;
            //$scope.isDisabledAddText = true;
            //$scope.isDisabledMoveImage = true;
            //$scope.isDisabledWatermark = true;
            //$scope.isDisabledAddImage = true;

            $scope.isDisabledOcr = true;

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $(".mydiv").hide();

            }
            else {
                $(".mydiv").hide();
                $scope.loading = false;
                toastr.error("Please select a rectangular plot on the page!");
            }
        }
    }
 
    //----------------------------------------------------------
    $scope.mask_Page = function (file, page_num, startX, startY, EndX, EndY) {
        $scope.loading = true;
        $(".mydiv").show();
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
                    $scope.loading = false;
                    $(".mydiv").hide();

                    $scope.startX = 0;
                    $scope.startY = 0;
                    $scope.endX = 0;
                    $scope.endY = 0;
                    $scope.$apply();
                });
           

            }, function (error) {
                console.log(error.data);
                alert(error.data);
                $scope.loading = false;
                $(".mydiv").hide();

            });

     

            //$scope.isDisabledCrop = true;
            //$scope.isDisabledMasking = false;
            //$scope.isDisabledHighlight = true;
            //$scope.isDisabledRotate = true;
            //$scope.isDisabledRemove = true;
            //$scope.isDisabledReSize = true;
            //$scope.isDisabledAddText = true;
            //$scope.isDisabledMoveImage = true;
            //$scope.isDisabledWatermark = true;
            //$scope.isDisabledAddImage = true;

            $scope.isDisabledOcr = true;
        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $(".mydiv").hide();

            }
            else {
                $(".mydiv").hide();
                $scope.loading = false;
                toastr.error("Please select a rectangular plot on the page!");
            }
        }
    }
    $scope.highlight_Page = function (file, page_num, startX, startY, EndX, EndY) {
        $scope.loading = true;     
        $(".mydiv").show();
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
                    $scope.loading = false;
                    $(".mydiv").hide();
                    $scope.startX = 0;
                    $scope.startY = 0;
                    $scope.endX = 0;
                    $scope.endY = 0;
                    $scope.$apply();
                });
         

            }, function (error) {
                console.log(error.data);
                alert(error.data);
                $scope.loading = false;
                $(".mydiv").hide();
            });


            //$scope.isDisabledCrop = true;
            //$scope.isDisabledMasking = true;
            //$scope.isDisabledHighlight = false;
            //$scope.isDisabledRotate = true;
            //$scope.isDisabledRemove = true;
            //$scope.isDisabledReSize = true;
            //$scope.isDisabledAddText = true;
            //$scope.isDisabledMoveImage = true;
            //$scope.isDisabledWatermark = true; 
            //$scope.isDisabledAddImage = true;

            $scope.isDisabledOcr = true;

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $(".mydiv").hide();

            }
        }
    }
    $scope.add_Image = function (file, image_file,page_num, startX, startY, EndX, EndY) {
        $scope.loading = true;
        $(".mydiv").show();
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
                    $scope.loading = false;
                    $(".mydiv").hide();
                    $scope.startX = 0;
                    $scope.startY = 0;
                    $scope.endX = 0;
                    $scope.endY = 0;
                    $scope.$apply();

                });
                

            }, function (error) {
                console.log(error.data);
                alert(error.data);
                $scope.loading = false;
                $(".mydiv").hide();

            });


            //$scope.isDisabledCrop = true;
            //$scope.isDisabledMasking = true;
            //$scope.isDisabledHighlight = true;
            //$scope.isDisabledRotate = true;
            //$scope.isDisabledRemove = true;
            //$scope.isDisabledReSize = true;
            //$scope.isDisabledAddText = true;
            //$scope.isDisabledMoveImage = true;
            //$scope.isDisabledWatermark = true;
            //$scope.isDisabledAddImage = false;

           $scope.isDisabledOcr = true;

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $(".mydiv").hide();

            }
        }
    }

    $scope.Rotate_Page_DirectApi = function (file, page_num, angle) {
        $scope.loading = true;   
        $(".mydiv").show();

        var cookie = $scope.getCookie('access');
        $scope._rotate_page_no = page_num;
        if (file != undefined && page_num > 0 && cookie != "") {
            const apiEndpoint = 'http://202.59.140.136:8000/api/rotate/';
            const formData = new FormData();
            formData.append('file', file);
            formData.append('page_num', page_num);
            formData.append('angle', angle);
            fetch(apiEndpoint, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${cookie}` },
                body: formData,
            })
                .then(response => {
                    // Check if the response status code is in the 200 range (successful)
                    if (response.ok) {

                        // If you expect a file download in response, you can handle it like this
                        return response.blob(); // This returns a Blob representing the response body
                    } else {
                        // Handle errors here
                        throw new Error('Request failed');
                    }
                })
                .then(blob => {
                    debugger;
                    // Read the selected a file
                    var vBlob = new Blob([blob], { type: 'application/pdf' });
                    var vvBlob = new Blob([blob], { type: 'application/octet-stream' });
                    var pdfUrl = URL.createObjectURL(vBlob);
                    var pdfUrl2 = URL.createObjectURL(vvBlob);
                    $scope.pdfToArrayBuffer(vvBlob, function (pdfData) {
                        // You can use the pdfData ArrayBuffer here
                        console.log('PDF data as ArrayBuffer:', pdfData);
                        debugger;
                        $scope.ZoomRef = pdfData;
                        const blob = new Blob([pdfData], { type: 'application/octet-stream' });
                        //$scope.PDF_Images = new Blob(pdfData, { type: 'application/pdf' });
                        $scope.PDF_Images = new Blob([pdfData], { type: 'application/octet-stream' });
                        //  $scope.LoadPdfFromUrl(pdfData);
                        pdfjsLib.getDocument(pdfData).promise.then(function (pdfDoc_) {
                            debugger
                            pdfDoc = pdfDoc_;
                            var pdf_container = document.getElementById("dwtLargeViewer");
                            $scope.Large_Viewer_RenderPage(pdf_container, $scope._rotate_page_no);
                        });
                        $scope.loading = false;
                        $(".mydiv").hide();
                        $scope.startX = 0;
                        $scope.startY = 0;
                        $scope.endX = 0;
                        $scope.endY = 0;
                        $scope.$apply();
                    });


                })
                .catch(error => {
                    $scope.loading = false;
                    $(".mydiv").hide();
                    // Handle any errors that occurred during the fetch
                    console.error('Error:', error);
                });
           // $scope.isDisabled = true;

            //$scope.isDisabledCrop = true;
            //$scope.isDisabledMasking = true;
            //$scope.isDisabledHighlight = true;
            //$scope.isDisabledRotate = false;
            //$scope.isDisabledRemove = true;
            //$scope.isDisabledReSize = true;
            //$scope.isDisabledAddText = true;
            //$scope.isDisabledMoveImage = true;
            //$scope.isDisabledWatermark = true;
            //$scope.isDisabledAddImage = true;

            $scope.isDisabledOcr = true;
        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $(".mydiv").hide();
            }
        }

      
    }
 
    $scope.resize_Page = function (file, page_num, height, width) {
        $scope.loading = true;
        $(".mydiv").show();
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
            

            }, function (error) {
                console.log(error.data);
                alert(error.data);
                $scope.loading = false;
                $(".mydiv").hide();

            });



            //$scope.isDisabledCrop = true;
            //$scope.isDisabledMasking = true;
            //$scope.isDisabledHighlight = true;
            //$scope.isDisabledRotate = true;
            //$scope.isDisabledRemove = true;
            //$scope.isDisabledReSize = false;
            //$scope.isDisabledAddText = true;
            //$scope.isDisabledMoveImage = true;
            //$scope.isDisabledWatermark = true;
            //$scope.isDisabledAddImage = true;

            $scope.isDisabledOcr = true;
        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $(".mydiv").hide();

            }
        }
    }


    $scope.TestBox_Page = function (file, page_num, text, endx, endy, font_scale, colorR, colorG, colorB, thickness) {
        $scope.loading = true;   
        $(".mydiv").show();
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
                    $scope.loading = false;
                    $(".mydiv").hide();
                    $scope.startX = 0;
                    $scope.startY = 0;
                    $scope.endX = 0;
                    $scope.endY = 0;
                    $scope.$apply();
                });
            

            }, function (error) {
                console.log(error.data);
                alert(error.data);
                $scope.loading = false;
                $(".mydiv").hide();

            });
           // $scope.isDisabled = true;

            //$scope.isDisabledCrop = true;
            //$scope.isDisabledMasking = true;
            //$scope.isDisabledHighlight = true;
            //$scope.isDisabledRotate = true;
            //$scope.isDisabledRemove = true;
            //$scope.isDisabledReSize = true;
            //$scope.isDisabledAddText = false;
            //$scope.isDisabledMoveImage = true;
            //$scope.isDisabledWatermark = true;
            //$scope.isDisabledAddImage = true;

            $scope.isDisabledOcr = true;

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $(".mydiv").hide();

            }
        }
    }
    $scope.move_Page = function (file, from_page_num, to_page_num) {
        $scope.loading = true;      
        $(".mydiv").show();
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
                 
      
                    $scope.loading = false;
                    $(".mydiv").hide();
                    $scope.startX = 0;
                    $scope.startY = 0;
                    $scope.endX = 0;
                    $scope.endY = 0;
                    $scope.$apply();
                });
             

            }, function (error) {
                console.log(error.data);
                $scope.loading = false;
                $(".mydiv").hide();

            });


            //$scope.isDisabledCrop = true;
            //$scope.isDisabledMasking = true;
            //$scope.isDisabledHighlight = true;
            //$scope.isDisabledRotate = true;
            //$scope.isDisabledRemove = true;
            //$scope.isDisabledReSize = true;
            //$scope.isDisabledAddText = true;
            //$scope.isDisabledMoveImage = false;
            //$scope.isDisabledWatermark = true;
            //$scope.isDisabledAddImage = true;

           $scope.isDisabledOcr = true;
        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $(".mydiv").hide();
            }
        }
    }
 
    $scope.remove_Page_DirectApi= function (file, page_num) {
        $scope.loading = true;
        $(".mydiv").show();
        $scope._rotate_page_no = page_num;
        var cookie = $scope.getCookie('access');
        debugger;
        const arrayString = '[' + page_num + ']';
        if (file != undefined && page_num > 0 && cookie != "") {

            const apiEndpoint = 'http://202.59.140.136:8000/api/removepage/';
            const formData = new FormData();
            formData.append('file', file);
            formData.append('page_num', arrayString);
            fetch(apiEndpoint, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${cookie}` },
                body: formData,
            })
                .then(response => {
                    // Check if the response status code is in the 200 range (successful)
                    if (response.ok) {
                        // If you expect a file download in response, you can handle it like this
                        return response.blob(); // This returns a Blob representing the response body
                    } else {
                        // Handle errors here
                        throw new Error('Request failed');
                    }
                })
                .then(blob => {
                    debugger;
                    // Read the selected a file
                    var vBlob = new Blob([blob], { type: 'application/pdf' });
                    var vvBlob = new Blob([blob], { type: 'application/octet-stream' });
                    var pdfUrl = URL.createObjectURL(vBlob);
                    var pdfUrl2 = URL.createObjectURL(vvBlob);
                    $scope.pdfToArrayBuffer(vvBlob, function (pdfData) {

                        // You can use the pdfData ArrayBuffer here
                        console.log('PDF data as ArrayBuffer:', pdfData);
                        debugger;
                        $scope.ZoomRef = pdfData;
                        const blob = new Blob([pdfData], { type: 'application/octet-stream' });
                        //$scope.PDF_Images = new Blob(pdfData, { type: 'application/pdf' });
                        $scope.PDF_Images = new Blob([pdfData], { type: 'application/octet-stream' });
                        pdfjsLib.getDocument(pdfData).promise.then(function (pdfDoc_) {
                            debugger;

                            pdfDoc = pdfDoc_;
                            var pdf_container = document.getElementById("dwtLargeViewer");
                            $scope.Large_Viewer_RenderPage(pdf_container, $scope._rotate_page_no);
                            var pdf_container_thumb = document.getElementById("dwtVerticalThumbnil");
                            $scope.Large_Thumb_Viewer_RenderPage(pdf_container_thumb);
                            $scope.loading = false;
                            $(".mydiv").hide();
                            $scope.startX = 0;
                            $scope.startY = 0;
                            $scope.endX = 0;
                            $scope.endY = 0;
                            $scope.$apply();
                        });
                      
                    });
                  
                })
                .catch(error => {
                    $scope.loading = false;
                    $(".mydiv").hide();
                    // Handle any errors that occurred during the fetch
                    console.error('Error:', error);
                });


            //$scope.isDisabledCrop = true;
            //$scope.isDisabledMasking = true;
            //$scope.isDisabledHighlight = true;
            //$scope.isDisabledRotate = true;
            //$scope.isDisabledRemove = false;
            //$scope.isDisabledReSize = true;
            //$scope.isDisabledAddText = true;
            //$scope.isDisabledMoveImage = true;
            //$scope.isDisabledWatermark = true;
            //$scope.isDisabledAddImage = true;

           $scope.isDisabledOcr = true;

        } else {
            if (cookie == "") {
                $scope.loading = false;
                $(".mydiv").hide();
                alart("Please log in to generate new access token");
            }
        }

       
    }
    //$scope.remove_Page = function (file, page_num) {
    //    $scope.loading = true;
    //    $scope._rotate_page_no = page_num;
    //    var cookie = $scope.getCookie('access');
    //    debugger;
    //    const arrayString = '[' + page_num+ ']';
    //    if (file != undefined && page_num > 0 && cookie != "") {
    //        ImageProcessServices.removepage(file, arrayString, cookie).then(function (data) {
    //            debugger
    //            $scope.ZoomRef = data;
    //            $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
    //            pdfjsLib.getDocument(data).promise.then(function (pdfDoc_) {
    //                debugger
    //                pdfDoc = pdfDoc_;
    //                var pdf_container = document.getElementById("dwtLargeViewer");
    //                $scope.Large_Viewer_RenderPage(pdf_container, $scope._rotate_page_no);
    //                var pdf_container_thumb = document.getElementById("dwtVerticalThumbnil");
    //                $scope.Large_Thumb_Viewer_RenderPage(pdf_container_thumb);
    //            });
    //            $scope.loading = false;
    //        }, function (error) {
    //            console.log(error.data);
    //            alert(error.data);
    //            $scope.loading = false;
    //        });        
    //    } else {
    //        if (cookie == "") {              
    //            $scope.loading = false;
    //            alart("Please log in to generate new access token");

    //        }
    //    }
    //}

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


    // new Pdf Upload and render 
    $scope.pdfToArrayBuffer = function (file, callback) {
        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                var pdfData = new Uint8Array(e.target.result);
                if (typeof callback === 'function') {
                    callback(pdfData);
                }
            }
            reader.readAsArrayBuffer(file);
        }
    }


   

}]);

