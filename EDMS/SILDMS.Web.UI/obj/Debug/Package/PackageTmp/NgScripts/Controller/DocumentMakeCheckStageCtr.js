app.controller('ProcessingStageCtrl', ['$scope', '$http','$sce', 'ImageProcessServices', 'environmentServices', '$compile', '$injector', '$interval', function ($scope, $http, $sce, ImageProcessServices, environmentServices, $compile, $injector, $interval) {

    'use strict'

    $scope.isFirstStage = false;
    $scope.hasChack = true;
    $scope.hasMake = true;
    $scope.isUserMaker = true;
    $scope.isUserChecker = true;
    $scope.OwnerName = "";
    $scope.OwnerID = "";
    $scope.DocCategoryID = "";
    $scope.DocCategoryName = "";
    $scope.DocTypeID = "";
    $scope.DocTypeName = "";
    $scope.OwnerLevelName = "";
    $scope.OwnerLevelID = ""

    var qrStr = window.location.search.substring(1);
    var qrStrVariables = qrStr.split('&');
    var qrStrone = qrStrVariables[0].split('=');
    $scope.stageID = qrStrone[1];




    $scope.PDF_Images = null;
    $scope.file_list = [];
    $scope.Zoom_Count = .5;
    $scope.Page_Rotation = [];
    $scope.Total_Page = 0;

    $http.get('/WorkflowModule/DocMkCkStage/GetStageAndUserPermission?stageMapID=' + $scope.stageID + '')
        .success(function (response) {

            $scope.hasChack = response.obj.HaveCk;
            $scope.hasMake = response.obj.HaveMk;
            $scope.stageName = response.obj.StageName;
            $scope.isFirstStage = response.obj.StageSL > 1 ? false : true;
            $scope.isUserMaker = response.obj.NotifyMk;
            $scope.isUserChecker = response.obj.NotifyCk;
            $scope.OwnerName = response.obj.OwnerName;
            $scope.OwnerID = response.obj.OwnerID;
            $scope.DocCategoryID = response.obj.DocCategoryID;
            $scope.DocCategoryName = response.obj.DocCategoryName;
            $scope.DocTypeID = response.obj.DocTypeID;
            $scope.DocTypeName = response.obj.DocTypeName;
            $scope.OwnerLevelName = response.obj.OwnerLevelName;
            $scope.OwnerLevelID = response.obj.OwnerLevelID;

            $scope.loading = false;
            BindDataToGrid();
            loadPropsDocs();
        })
        .error(function () {
            $scope.loading = false;
        });

  

    $scope.ShowUploadFileDiv = function () { 
        $("#LoadPdf").toggleClass("hidden");
    }
    $scope.LoadPdf = function () {
        $scope.loading = true;
        $(".mydiv").show();

        var fileUpload = $("#file_upload").get(0);
        var files = fileUpload.files;
        const file = files[0]; 
        if (file != null && file != "" && file != "undefined") {
            debugger;
           
            
                var fileUpload = $("#file_upload")[0];
                var files = fileUpload.files;
                if (files.length > 0) {
                    for (var i = 0; i < files.length; i++) {
                        $scope.file_list.push(files[i]);
                        $scope.PDF_TO_Images_AnotherPDF(files[i], 1, 0);
                    }               
                    $scope.ShowUploadImageDivVar = false;

                } else {
                    $scope.loading = false;
                    $(".mydiv").hide();
                    toastr.error('No files selected. Please choose at least one image file to load.');
                }                  

            $("#LoadPdf").toggleClass("hidden");
        }
        else {
            $scope.loading = false;
            $(".mydiv").hide();
            toastr.error("Please the values according to the fields.");
        }


    }

    $scope.PDF_TO_Images_AnotherPDF = function (file, page_num, angle) {
        debugger;
        $scope.loading = true;
        var cookie = $scope.getCookie('access');

        var pdf_container = document.getElementById("dwtHorizontalThumbnil");
        var innerHTMLContent = pdf_container.innerHTML;
        var innerTextContent = pdf_container.innerText;
  
    
        debugger;
        if (file != undefined && page_num > 0 && cookie != "") {
            $scope.pdfToArrayBuffer(file, function (pdfData) {
                debugger;

                // You can use the pdfData ArrayBuffer here
                console.log('PDF data as ArrayBuffer:', pdfData);
                debugger;
                var vBlob = new Blob(pdfData, { type: 'application/pdf' });
                const vvBlob = new Blob([pdfData], { type: 'application/octet-stream' });
                $scope.Temp_PDF_Images = null;
                $scope.Temp_PDF_Images = new Blob([pdfData], { type: 'application/octet-stream' });
                $scope.file_list = [];
                if (innerTextContent !== null && innerTextContent !== "") {
                    // pdf_container exists and is not null
                    $scope.file_list.push(new File([$scope.PDF_Images], 'PDF_0.pdf', { type: 'application/pdf' }));
                    $scope.file_list.push(new File([$scope.Temp_PDF_Images], 'PDF_1.pdf', { type: 'application/pdf' }));
                    $scope.Merge_Page_DirectApi($scope.file_list);

                } else {
                    $scope.file_list.push(new File([$scope.Temp_PDF_Images], 'PDF_1.pdf', { type: 'application/pdf' }));
                    $scope.Merge_Page_DirectApi($scope.file_list);
                }
               
                $scope.ShowUploadImageDivVar = false;
                $scope.loading = false;
                debugger;
                // You can call any function or perform further processing here
                // displayPDF(pdfData);
            });

        } else {
            if (cookie == "") {
                $scope.loading = false;
                $(".mydiv").hide();
                alart("Please log in to generate new access token");
            }
   
        }

    }
    $scope.Merge_Page_DirectApi = function (files) {
        $scope.loading = true;
        $(".mydiv").show();
        debugger
        var cookie = $scope.getCookie('access');

        if (files != null && files.length > 0 && cookie != "") {
            const apiEndpoint = 'http://202.59.140.136:8000/api/merge/';
            var formData = new FormData();
            for (var i = 0; i < files.length; i++) {
                formData.append('files', files[i]);
            }
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
                        $scope.LoadPdfFromUrl_Generic(pdfData);
                        debugger;

                        $scope.loading = false;
                        $(".mydiv").hide();
                    });


                })
                .catch(error => {
                    $scope.loading = false;
                    $(".mydiv").hide();
                    console.error('Error:', error);
                });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $(".mydiv").hide();

            }
        }

    }
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
            $(".mydiv").hide();

        });

    };
    $scope.RenderPage_Generic = function (pdf_container, num, serial) {
        $scope.loading = true;

        pdfDoc.getPage(num).then(function (page) {
            //Create Canvas element and append to the Container DIV.
            debugger;
            $scope.loading = false;
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
            $(".mydiv").hide();

        });
     
    };

    $scope.RemoveImage_New = function () {
        $scope.loading = true;
        $(".mydiv").show();
        debugger;
        var page_no = 0;
        var file_count = $scope.file_list.length;
        var align_count = 0;

        var arr = []
        for (var i = 1; i <= $scope.last_page; i++) {
            var x = document.getElementById("page-" + i);
            if (x != null && x.checked) {
                page_no = i;
                arr.push(i)
            }
        }
        const arrayString = '[' + JSON.stringify(arr).slice(1, -1) + ']';
        const arrayString2 = JSON.stringify(arr).slice(1, -1);
        debugger;
        if (page_no > 0) {
            //  $scope.Remove_Page($scope.PDF_Images, arrayString);
            $scope.Remove_Page_DirectApi($scope.PDF_Images, arrayString);
            //$scope.Remove_Page_MyApi($scope.PDF_Images, arrayString2);

        } else {
            toastr.error("Please Select at least one page to remove");
            $scope.loading = false;
            $(".mydiv").hide();

        }

      
    }
    $scope.Remove_Page_DirectApi = function (file, page_num) {
        $scope.loading = true;
        $(".mydiv").show();
        var cookie = $scope.getCookie('access');
        var token = $scope.getCookie('access');
        if (file != undefined && page_num.length > 0 && cookie != "") {
            const apiEndpoint = 'http://202.59.140.136:8000/api/removepage/';
            const formData = new FormData();
            formData.append('file', file);
            formData.append('page_num', page_num);
            fetch(apiEndpoint, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}` },
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
                        $scope.LoadPdfFromUrl_Generic(pdfData);
                        debugger;
                        $scope.loading = false;
                        $(".mydiv").hide();
                 
                    });


                })
                .catch(error => {
                    $scope.loading = false;
                    // Handle any errors that occurred during the fetch
                    console.error('Error:', error);
                });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
            }
        }
    }

    $scope.RotateImage = function () {
        $scope.loading = true;
        $(".mydiv").show();
        debugger;
        var page_no = 0;
        var file_count = $scope.file_list.length;
        var align_count = 0;
        for (var i = 1; i <= $scope.last_page; i++) {
            var x = document.getElementById("page-" + i);
            if (x != null && x.checked) {
                page_no = i;
                break;
                //var f = $scope.Page_Rotation.findIndex(x => x.page_no == i);
                //if (f > -1) {
                //    page_no = i;
                //    $scope.Page_Rotation[f].rotate_degree = $scope.Page_Rotation[f].rotate_degree + 90;
                //    align_count = $scope.Page_Rotation[f].rotate_degree;
                //    break;
                //}
            }
        }
        if (page_no > 0) {      
            $scope.Rotate_Page_DirectApi($scope.PDF_Images, page_no, 270);

        } else {
            toastr.error("Please Select at least one page to rotate");
            $scope.loading = false;
            $(".mydiv").hide();

        }
      
    }


    $scope.Rotate_Page_DirectApi = function (file, page_num, angle) {
        $scope.loading = true;
        $(".mydiv").show();
        var cookie = $scope.getCookie('access');
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
                        $scope.LoadPdfFromUrl_Generic(pdfData);
                        debugger;
                        $scope.loading = false;
                        $(".mydiv").hide();
                    });


                })
                .catch(error => {
                    $scope.loading = false;
                    // Handle any errors that occurred during the fetch
                    console.error('Error:', error);
                });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;

            }
        }
    }
    $scope.ShowMoveImageDiv = function () {
        $("#WhichImage").val("1");
        $("#Where").val("2");
        $("#MoveImage").toggleClass("hidden");
    }
    $scope.MoveImage = function () {
        $scope.loading = true;
        $(".mydiv").show();
        $scope.ShowUploadImageDivVar = false;

        var cookie = $scope.getCookie('access');    
        var which = $("#WhichImage").val();
        var where = $("#Where").val();
        var file = $scope.PDF_Images;

        if (file != undefined && which > 0 && where > 0 && cookie != "") {       
            ImageProcessServices.movepage(file, which, where, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $scope.LoadPdfFromUrl_Generic(data);
                $scope.loading = false;
                $(".mydiv").hide();

            }, function (error) {
                console.log(error.data);
           
                $scope.loading = false;
                $(".mydiv").hide();

            });

        } else {
            toastr.error("Please the values according to the fields.");
            $scope.loading = false;
            $(".mydiv").hide();
        }
        $("#MoveImage").toggleClass("hidden");
    };


 
    $scope.move_Page = function (file, from_page_num, to_page_num) {
        $scope.loading = true;
        $(".mydiv").show();
        var cookie = $scope.getCookie('access');

        if (file != undefined && from_page_num > 0 && to_page_num > 0 && cookie != "") {
            ImageProcessServices.movepage(file, from_page_num, to_page_num, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;

                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $scope.LoadPdfFromUrl_Generic(data);
                $scope.loading = false;
                $(".mydiv").hide();

            }, function (error) {
                console.log(error.data);
                alert(error.data);
                $scope.loading = false;
                $(".mydiv").hide();

            });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $(".mydiv").hide();

            }
        }
    }

    $scope.docGenerationModelCollection = {
        docsForSpecificDocType: [],
        propsForSpecificDocType: []
    };

    $scope.docGenerationVM = {};

    var loadPropsDocs = function () {
        $scope.docGenerationModelCollection.docsForSpecificDocType = [];
        $scope.docGenerationModelCollection.propsForSpecificDocType = [];
        $scope.docGenerationModelCollection.listPropsForSpecificDocType = [];

        $http.post('/WorkflowModule/DocMkCkStage/GetDocumentProperty',
            {
                _DocCategoryID: $scope.DocCategoryID,
                _OwnerID: $scope.OwnerID,
                _DocTypeID: $scope.DocTypeID,
                _StageMapID: $scope.stageID
            })
            .success(function (response) {

                $scope.docGenerationModelCollection.docsForSpecificDocType = response.obj.Documents;
                $scope.docGenerationModelCollection.propsForSpecificDocType = response.obj.TypeProperties;
                $scope.docGenerationModelCollection.listPropsForSpecificDocType = response.obj.ListTypeProperties;

                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });
    };

    $scope.docGenerationVM.SelectAll = function () {
        angular.forEach($scope.docGenerationModelCollection.docsForSpecificDocType, function (item) {
            if (item.StageMapID == $scope.stageID && item.DocClassification != 'Required') {
                item.IsSelected = true;
            }
        });
    };

    $scope.docGenerationVM.UnSelectAll = function () {
        angular.forEach($scope.docGenerationModelCollection.docsForSpecificDocType, function (item) {
            if (item.StageMapID == $scope.stageID && item.DocClassification != 'Required') {
                item.IsSelected = false;
            }
        });
    };

    $scope.SaveImage = function () {

        $('#ConfirmSave').modal('hide');

        $scope.loading = true;
        $(".mydiv").show();
        var selectedPropID = new Array();
        var DocIDsCounter = 0;     
    
            angular.forEach($scope.docGenerationModelCollection.docsForSpecificDocType, function (item) {
                if (item.IsSelected) {
                    selectedPropID.push(item.DocPropertyID);
                };
            });
        
    



        var dataToSend = {
            '_modelDocumentsInfo': { OwnerLevelID: $scope.OwnerLevelID, OwnerID: $scope.OwnerID, DocCategoryID: $scope.DocCategoryID, DocTypeID: $scope.DocTypeID },
            '_SelectedPropID': selectedPropID.join(),
            '_docMetaValues': $scope.docGenerationModelCollection.propsForSpecificDocType,
            '_listProperty': $scope.docGenerationModelCollection.listPropsForSpecificDocType,

            '_extentions': "",
            '_otherUpload': $scope.ShowUploadImageDivVar
        };
        // console.log(dataToSend);
        $http.post('/WorkflowModule/DocMkCkStage/AddDocumentInfo', JSON.stringify(dataToSend))
            .success(function (response) {
                debugger;

                var uploadCount = 1;
                var docCounts =1;
                console.log(response);
                
                if (response.DistinctID.length > 0) {
                    if (docCounts > 0 && docCounts == response.DistinctID.length) {

                        $scope.response = response;
                       // var vBlob = new Blob(pdfData, { type: 'application/pdf' });
                        $scope.PDF1 = $scope.PDF_Images;
                  
                        var vfilePath = response.DistinctID[0].FileServerUrl;
                        var vfileName = response.DistinctID[0].DocumentID;
                 

                        ImageProcessServices.DocUpload($scope.response.result[0].ServerIP, $scope.response.result[0].ServerPort, $scope.response.result[0].FtpUserName, $scope.response.result[0].FtpPassword, vfilePath + "//" +
                            vfileName + ".pdf",
                            vfileName + ".pdf", $scope.PDF1,
                            $scope.getCookie('access')).then(function (data) {
                                debugger;
                                if (data.data == "Success") {
                                    toastr.success("Upload Successful");
                                    $("#dwtHorizontalThumbnil").empty();                                 

                                    $scope.loading = false;
                                    $(".mydiv").hide();
                                } else {
                                    $http.post('/WorkflowModule/DocMkCkStage/DeleteDocumentInfo',
                                        {
                                            _DocumentIDs: response.result[0].ObjectID
                                        })
                                        .success(function () {
                                            toastr.success("Upload Failed");

                                            $scope.loading = false;
                                            $(".mydiv").hide();
                                        })
                                        .error(function () {
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



                        if (uploadCount == response.DistinctID.length) {
                            loadPropsDocs();

                            $scope.loading = false;
                            $(".mydiv").hide();
                            toastr.success("Upload Successful");
                        } else {
                            $http.post('/WorkflowModule/DocMkCkStage/DeleteDocumentInfo',
                                {
                                    _DocumentIDs: response.result[0].ObjectID
                                })
                                .success(function (response) {

                                    $scope.loading = false;
                                    $(".mydiv").hide();
                                    toastr.success("File Upload Failed to FTP");
                                })
                                .error(function () {

                                    $scope.loading = false;
                                    $(".mydiv").hide();
                                });
                        }
                    }
                    else {
                        $http.post('/WorkflowModule/DocMkCkStage/DeleteDocumentInfo',
                            {
                                _DocumentIDs: response.result[0].ObjectID
                            })
                            .success(function (response) {

                                $scope.loading = false;
                                $(".mydiv").hide();
                                toastr.success("Scanned documents count is different than your selection");
                            })
                            .error(function () {

                                $scope.loading = false;
                                $(".mydiv").hide();
                            });
                    }
                }
                else {
                    toastr.success(response.Message);

                    $scope.loading = false;
                    $(".mydiv").hide();
                }
            })
            .error(function () {

                $scope.loading = false;
                $(".mydiv").hide();
                toastr.success("Failed to Save Meta Data.");
            });
    };

    $scope.docGenerationVM.ShowConfirmModal = function () {
        $('#ConfirmSave').modal('show');
    };
    

    //#region Document_MakerChecker
    $scope.docMakeCheckModelCollection = {
        updatePropertyCollection: [],
        updateDocumentCollection: [],
        newDocumentCollection: [],
        allDocumentCollection: [],
        listPropertyCollectionCk: [],
        listPropertyCollectionMk: [],
        ParentStages: [],
        objectID: "",
        listPropertyHead: {},
        listPropertyBody: {}
    };

    $scope.pagingInfo = {
        page: 1,
        itemsPerPage: 5,
        sortBy: null,
        reverse: false,
        search: null,
        totalItems: 0
    };

    $scope.search = function (keyEvent) {
        if (keyEvent.which === 13) {
            $scope.pagingInfo.page = 1;
            BindDataToGrid();
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
        BindDataToGrid();
    };

    $scope.selectPage = function () {
        BindDataToGrid();
    };

    $scope.docMakeCheckVM = {};

    $scope.docMakeCheckVM.ShowMakeModal = function (ObjectID) {
        $scope.docMakeCheckModelCollection.objectID = ObjectID;
        loadMakeModal(ObjectID);
        $('#docMakerModal').modal('show');
    };

    var loadMakeModal = function (ObjectID) {
        debugger;
        $scope.loading = true;
        $http.get('/WorkflowModule/DocMkCkStage/GetDocumentPropertyValuesMake?_ObjectID=' + ObjectID + '&_StageMapID=' + $scope.stageID)
            .success(function (response) {
                debugger;
                $scope.docMakeCheckModelCollection.updatePropertyCollection = response.updatePropertyCollection;
                $scope.docMakeCheckModelCollection.updateDocumentCollection = response.updateDocumentCollection;
                $scope.docMakeCheckModelCollection.ParentStages = response.ParentStages;
                $scope.docMakeCheckModelCollection.newDocumentCollection = response.newDocumentCollection;
                $scope.docMakeCheckModelCollection.IsBacked = response.IsBacked;
                $scope.docMakeCheckModelCollection.BackReason = response.BackReason;
                $scope.docMakeCheckModelCollection.listPropertyCollectionMk = [];
             
                angular.forEach(response.listPropHtml, function (item) {
                    if (item) {
                        $scope.docMakeCheckModelCollection.listPropertyCollectionMk.push($sce.trustAsHtml(item));
                    }
                });

                $scope.loading = false;
            })
            .error(function () {

                $scope.loading = false;
                $(".mydiv").hide();
            });
    };

    $scope.docMakeCheckVM.ShowCheckModal = function (ObjectID) {
        //console.log(ObjectID);
        $scope.docMakeCheckModelCollection.objectID = ObjectID;
        loadCheckModal(ObjectID);
        $('#docCheckerModal').modal('show');
    };

    var loadCheckModal = function (ObjectID) {
        $scope.loading = true;
        $http.get('/WorkflowModule/DocMkCkStage/GetDocumentPropertyValuesCheck?_ObjectID=' + ObjectID + '&_StageMapID=' + $scope.stageID)
            .success(function (response) {
                $scope.docMakeCheckModelCollection.updatePropertyCollection = response.updatePropertyCollection;
                $scope.docMakeCheckModelCollection.allDocumentCollection = response.allDocumentCollection;
                $scope.docMakeCheckModelCollection.ParentStages = response.ParentStages;

                $scope.docMakeCheckModelCollection.IsBacked = response.IsBacked;
                $scope.docMakeCheckModelCollection.BackReason = response.BackReason;

                $scope.docMakeCheckModelCollection.listPropertyCollectionCk = [];
              
                angular.forEach(response.listPropHtml, function (item) {
                    if (item) {
                        $scope.docMakeCheckModelCollection.listPropertyCollectionCk.push($sce.trustAsHtml(item));
                    }
                });

                $scope.loading = false;
            })
            .error(function () {
                $scope.loading = false;
            });
    };

    $scope.docMakeCheckVM.LoadListProperty = function (tableRefID, index) {
        $http.get('/WorkflowModule/DocMkCkStage/GetDocumentListPropertyValuesCheck?_ObjectID=' + $scope.docMakeCheckModelCollection.objectID + '&&_TableRefID=' + tableRefID + '')
            .success(function (response) {
                $scope.docMakeCheckModelCollection.listPropertyCollectionCk[index].tableData = $sce.trustAsHtml(response);
                $scope.loading = false;
            })
            .error(function () {
                $scope.loading = false;
            });
    };

   

    $scope.LargeViewOfCheck = function (item) {
        $scope.loading = true;
        $(".mydiv").show();
        if (item.DocumentID.length > 0) {     
            debugger;
            UpdatedItem = item;
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
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $('#dwtLVCheck').empty();
                $scope.LoadPdfFromUrl_ModalView(data,"dwtLVCheck");
                $('#LargeViewerModal_Check').modal('show');
                // $("#DocumentID").val(tableRow.DocumentID); 

                $scope.loading = false;
                $(".mydiv").hide();

            }, function (error) {
                console.log(error.data);
            

                $scope.loading = false;
                $(".mydiv").hide();
            });
        }
        else {
            toastr.error("Document Hasn't Been Uploaded");
            $scope.loading = false;
            $(".mydiv").hide();
        }
    }



   

    //#region Initialize data
    var BindDataToGrid = function () {
        $scope.loading = true;
        $scope.tableData = '';

        $http.post('/WorkflowModule/DocMkCkStage/GetMkCkDocuments',
            {
                _DocCategoryID: $scope.DocCategoryID,
                _OwnerID: $scope.OwnerID,
                _DocTypeID: $scope.DocTypeID,
                _StageMapID: $scope.stageID,
                isUserMaker: $scope.isUserMaker,
                isUserChecker: $scope.isUserChecker,
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
                $(".mydiv").hide();
            }).error(function () {
                $scope.loading = false;
                $(".mydiv").hide();
            });
    };
    //#endregion


    //#region ActionMethods

    var UpdatedItem = {};

    $scope.docMakeCheckVM.GetImage = function (item) {
        $scope.loading = true;
        $(".mydiv").show();
        UpdatedItem = item;
        var url = "DocScanningModule/MultiDocScan/GetFilePassWord_r?" + encodeURIComponent("serverIP") + "=" + encodeURIComponent(item.ServerIP)
            + "&" + encodeURIComponent("ftpPort") + "=" + encodeURIComponent(item.ServerPort)
            + "&" + encodeURIComponent("ftpUserName") + "=" + encodeURIComponent(item.FtpUserName)
            + "&" + encodeURIComponent("ftpPassword") + "=" + encodeURIComponent(item.FtpPassword)
            + "&" + encodeURIComponent("serverURL") + "=" + encodeURIComponent(item.FileServerURL)
            + "&" + encodeURIComponent("documentID") + "=" + encodeURIComponent(item.DocumentID)
            + "&" + encodeURIComponent("isObsolete") + "=" + encodeURIComponent(false)
            + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent(false)

        ImageProcessServices.detailView(url).then(function (data) {
            debugger
            var arrayBuffer = data.data;
            var data = { data: arrayBuffer }
            $scope.Url_Ref = data;
            $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
            $('#dwtHTMakeUpdate').empty();
            $scope.LoadPdfFromUrl_ModalView(data, "dwtHTMakeUpdate"); 
            $('#docUpdateModal').modal('show');
            $scope.loading = false;
            $(".mydiv").hide();
        }, function (error) {
            console.log(error.data);
            alert(error.data);
            $scope.loading = false;
            $(".mydiv").hide();
        });       
      
    };
    $scope.LargeViewOfMakeCheck = function (item) {
        $scope.loading = true;
        $(".mydiv").show();
        if (item.DocumentID.length > 0) {
            debugger;
            UpdatedItem = item;
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
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $('#dwtLargeViewer').empty();
                $scope.LoadPdfFromUrl_ModalView(data, "dwtLargeViewer");
                $('#viewerModal').modal('show');

                $scope.loading = false;
                $(".mydiv").hide();

            }, function (error) {
                console.log(error.data);
            

                $scope.loading = false;
                $(".mydiv").hide();
            });
        }
        else {
            toastr.error("Document Hasn't Been Uploaded");

            $scope.loading = false;
            $(".mydiv").hide();
        }
    }
  
    
    $scope.docMakeCheckVM.UpdateDocument = function () {
      
        debugger;
        $scope.loading = true;
        $(".mydiv").show();
        if (UpdatedItem.DocumentID) {         
            var SelectedObj = [];
          
            var obj = { "StageMapID": UpdatedItem.StageMapID, "StageID": $scope.stageID }
            SelectedObj.push(obj);
         

            $http.post('/WorkflowModule/DocMkCkStage/RevertFromMakeUpdate',
                {
                    objectID: $scope.docMakeCheckModelCollection.objectID,
                    stages: SelectedObj,
                    revertReason: $scope.docMakeCheckModelCollection.BackReason
                })
                .success(function (response) {

                    $scope.PDF1 = $scope.PDF_Images;
                    var vfilePath = UpdatedItem.FileServerUrl;
                    var vfileName = UpdatedItem.DocumentID;


                    ImageProcessServices.DocUpload(UpdatedItem.ServerIP, UpdatedItem.ServerPort, UpdatedItem.FtpUserName, UpdatedItem.FtpPassword, vfilePath + "//" +
                        vfileName + ".pdf",
                        vfileName + ".pdf", $scope.PDF1,
                        $scope.getCookie('access')).then(function (data) {
                            debugger;
                            if (data.data == "Success") {
                                toastr.success("Upload Successful");
                                $("#dwtHTMakeUpdate").empty();
                                $('#docUpdateModal').modal('hide');
                                $('#docMakerModal').modal('hide');
                                BindDataToGrid();

                                $scope.loading = false;
                                $(".mydiv").hide();
                            
                            } else {
                                $http.post('/WorkflowModule/DocMkCkStage/DeleteDocumentInfo',
                                    {
                                        _DocumentIDs: response.result[0].ObjectID
                                    })
                                    .success(function () {
                                        toastr.success("Upload Failed");

                                        $scope.loading = false;
                                        $(".mydiv").hide();
                                    })
                                    .error(function () {
                                        toastr.success("Upload Failed");

                                        $scope.loading = false;
                                        $(".mydiv").hide();
                                    });

                            }
                        

                        }, function (error) {
                            console.log(error.data);

                            $scope.loading = false;
                            $(".mydiv").hide();
                        });
                })

        }
        else {
            toastr.error('No Document Selected');

            $scope.loading = false;
            $(".mydiv").hide();
        }
    };

    $scope.docMakeCheckVM.CreateDocument = function () {

        debugger;
        $scope.loading = true;
        $(".mydiv").show();
        var selectedPropIDMk = new Array();
        var DocIDsCounterMk = 0;

        angular.forEach($scope.docMakeCheckModelCollection.newDocumentCollection, function (item) {
            if (item.IsSelected) {
                selectedPropIDMk.push(item.DocPropertyID);
            };
        });

        $http.post('/WorkflowModule/DocMkCkStage/UpdateDocumentInfo',
            {
                objectID: $scope.docMakeCheckModelCollection.objectID,
                docs: selectedPropIDMk.join(),
                props: $scope.docMakeCheckModelCollection.updatePropertyCollection,
            })
            .success(function (response) {

                $scope.loading = false;
                $(".mydiv").hide();

                var uploadCount = 1;
                var docCounts =1;
                console.log(response);

                if (selectedPropIDMk.length == 0 && selectedPropIDMk.length == response.result.length) {
                    $http.post('/WorkflowModule/DocMkCkStage/SetMakeDone',
                        {
                            objectID: $scope.docMakeCheckModelCollection.objectID,
                            stageMapID: $scope.stageID
                        })
                        .success(function (response) {
                            BindDataToGrid();
                            $('#docMakerModal').modal('hide');
                            if (response.Code == "0") {
                                toastr.error(response.Message);

                            }
                            else {
                                toastr.success(response.Message);
                            }

                        }).error(function () {
                        });

                    return;
                }            

                if (docCounts > 0 && docCounts == response.result.length) {

                    $scope.response = response;
                    console.log(response);
                    debugger;
                    // var vBlob = new Blob(pdfData, { type: 'application/pdf' });
                    $scope.PDF1 = $scope.PDF_Images;                

                   var vfilePath = response.result[DocIDsCounterMk].FileServerURL;
                   var vfileName = response.result[DocIDsCounterMk].DocumentID;


                    ImageProcessServices.DocUpload($scope.response.result[0].ServerIP, $scope.response.result[0].ServerPort, $scope.response.result[0].FtpUserName, $scope.response.result[0].FtpPassword, vfilePath + "//" +
                        vfileName + ".pdf",
                        vfileName + ".pdf", $scope.PDF1,$scope.getCookie('access') ).then(function (data) {
                            debugger;
                            if (data.data == "Success") {
                                toastr.success("Upload Successful");
                                $("#dwtHTMakeCreate").empty();

                                $scope.loading = false;
                                $(".mydiv").hide();

                            } else {
                                $http.post('/WorkflowModule/DocMkCkStage/DeleteDocumentInfo',
                                    {
                                        _DocumentIDs: response.result[0].ObjectID
                                    })
                                    .success(function () {
                                        toastr.success("Upload Failed");

                                        $scope.loading = false;
                                        $(".mydiv").hide();
                                    })
                                    .error(function () {
                                        toastr.success("Upload Failed");

                                        $scope.loading = false;
                                        $(".mydiv").hide();
                                    });

                            }
                            $scope.loading = false;

                        }, function (error) {
                            console.log(error.data);

                            $scope.loading = false;
                            $(".mydiv").hide();
                        });


                    if (uploadCount == response.result.length) {
                        $scope.loading = false;
                        $http.post('/WorkflowModule/DocMkCkStage/SetMakeDone',
                            {
                                objectID: $scope.docMakeCheckModelCollection.objectID,
                                stageMapID: $scope.stageID
                            })
                            .success(function (response) {
                                BindDataToGrid();
                                $('#docMakerModal').modal('hide');
                                toastr.success("Document Updated Successfully");

                                $scope.loading = false;
                                $(".mydiv").hide();
                            }).error(function () {

                                $scope.loading = false;
                                $(".mydiv").hide();
                            });
                    }
                    else {
                        $scope.loading = false;
                        $http.post('/WorkflowModule/DocMkCkStage/DeleteDocumentInfo',
                            {
                                objectID: '',
                                documentIDs: response.distinctIDs.join(),
                                action: 'doc'
                            })
                            .success(function (response) {

                                $scope.loading = false;
                                $(".mydiv").hide();
                                toastr.success("File Upload Failed to FTP");
                            })
                            .error(function () {

                                $scope.loading = false;
                                $(".mydiv").hide();
                            });
                    }
                }
                else {
                    $http.post('/WorkflowModule/DocMkCkStage/DeleteDocumentInfo',
                        {
                            objectID: '',
                            documentIDs: response.distinctIDs.join(),
                            action: 'doc'
                        })
                        .success(function (response) {

                            $scope.loading = false;
                            $(".mydiv").hide();
                            toastr.success("Scanned documents count is different than your selection");
                        })
                        .error(function () {

                            $scope.loading = false;
                            $(".mydiv").hide();
                        });
                }
            })
            .error(function () {

                $scope.loading = false;
                $(".mydiv").hide();
                toastr.success("Failed to Save Meta Data.");
            });
    };

    $scope.docMakeCheckVM.DoneChecking = function () {

        debugger;
        $scope.loading = true;
        $(".mydiv").show();
        var selectedPropIDMk = new Array();
        var DocIDsCounterMk = 0;

        angular.forEach($scope.docMakeCheckModelCollection.newDocumentCollection, function (item) {
            if (item.IsSelected) {
                selectedPropIDMk.push(item.DocPropertyID);
            };
        });

        $http.post('/WorkflowModule/DocMkCkStage/UpdateDocumentInfo',
            {
                objectID: $scope.docMakeCheckModelCollection.objectID,
                docs: selectedPropIDMk.join(),
                props: $scope.docMakeCheckModelCollection.updatePropertyCollection,
            })
            .success(function (response) {
              
                $http.post('/WorkflowModule/DocMkCkStage/SetCheckDone',
                    {
                        objectID: $scope.docMakeCheckModelCollection.objectID,
                        stageMapID: $scope.stageID
                    })
                    .success(function (response) {
                        if (response.Code == '1') {
                            BindDataToGrid();
                            $('#docCheckerModal').modal('hide');
                            toastr.success(response.Message);
                        }
                        else {
                            toastr.error(response.Message);
                        }
                    }).error(function () {
                        toastr.error("Document Check Failed");
                        $scope.loading = false;
                    });

            })
            .error(function () {
                $scope.loading = false;
            });
    };

    $scope.docMakeCheckVM.RevertFromMake = function (_revertFromCheck) {
        debugger;
       var  SelectedObj = [];
        angular.forEach($scope.docMakeCheckModelCollection.ParentStages, function (item) {
            if (item.IsChecked) {
                SelectedObj.push(item);
            };
        });

        if (SelectedObj.length < 1) {
            toastr.error('Please! Select a stage to revert');
            return;
        }
        $http.post('/WorkflowModule/DocMkCkStage/RevertFromMake',
            {
                objectID: $scope.docMakeCheckModelCollection.objectID,
                stages: SelectedObj,
                revertReason: _revertFromCheck
            })
            .success(function (response) {
                if (response.Code == '1') {
                    BindDataToGrid();
                    $('#docMakerModal').modal('hide');
                    toastr.success(response.Message);
                }
                else {
                    toastr.success(response.Message);
                }
            }).error(function () {
                toastr.error("Failed to revert document");
                $scope.loading = false;
            });
    };

    $scope.docMakeCheckVM.RevertFromCheck = function (_revertFromCheck) {

      var  SelectedObj = [];
        angular.forEach($scope.docMakeCheckModelCollection.ParentStages, function (item) {
            if (item.IsChecked) {
                SelectedObj.push(item);
            };
        });

        if (SelectedObj.length < 1) {
            toastr.error('Please! Select a stage to revert');
            return;
        }
        $http.post('/WorkflowModule/DocMkCkStage/RevertFromCheck',
            {
                objectID: $scope.docMakeCheckModelCollection.objectID,
                stages: SelectedObj,
                revertReason: _revertFromCheck
            })
            .success(function (response) {
                if (response.Code == '1') {
                    BindDataToGrid();
                    $('#docCheckerModal').modal('hide');
                    toastr.success(response.Message);
                }
                else {
                    toastr.success(response.Message);
                }

            }).error(function () {
                toastr.error("Failed to revert document");
                $scope.loading = false;
            });
    };

    $scope.docMakeCheckVM.DeleteListItem = function (tableRefID, id) {
        if (tableRefID == '') {
            toastr.error("Item cant be deleted");
            return;
        }
        if (id == '') {
            toastr.error("Item cant be deleted");
            return;
        }

        $http.post('/WorkflowModule/DocMkCkStage/DeleteListItem',
            {
                tableRefID: tableRefID,
                id: id
            })
            .success(function (response) {
                if (response.Code == '1') {
                    toastr.success(response.Message);
                    loadMakeModal($scope.docMakeCheckModelCollection.objectID);
                }
                else {
                    toastr.success(response.Message);
                }
            }).error(function () {
                toastr.error("Failed to revert document");
                $scope.loading = false;
            });
    }

    $scope.ListItems = [];

    $scope.docMakeCheckVM.toggleAddNew = function (tableRefID) {
        $('#addNewListItemModal').modal('show');

        $http.get('/WorkflowModule/DocMkCkStage/ToggleAddNewListItem?tableRefID=' + tableRefID + '')
            .success(function (response) {
                $scope.ListItems = response;
                $scope.loading = false;
            })
            .error(function () {
                $scope.loading = false;
            });
    };

    $scope.docMakeCheckVM.AddSingleListItem = function () {
        $http.post('/WorkflowModule/DocMkCkStage/AddSingleListItem',
            {
                listItemColumn: $scope.ListItems.ColumnList,
                tableRefID: $scope.ListItems.TableRefID,
                objectID: $scope.docMakeCheckModelCollection.objectID
            })
            .success(function (response) {
                if (response.Code == '1') {
                    toastr.success(response.Message);
                    loadMakeModal($scope.docMakeCheckModelCollection.objectID);
                    $('#addNewListItemModal').modal('hide');
                }
                else {
                    toastr.success(response.Message);
                }
            }).error(function () {
                toastr.error("Failed to revert document");
                $scope.loading = false;
            });
    }

    $scope.toggleRefreshTable = function (row) {
        location.reload();
    };

    $scope.getMasterDg = function (pindex, index, master) {
        $http.get('/WorkflowModule/DocMkCkStage/GetMasterDataBySearch', {
            params: {
                searchKey: 'm',
                masterID: master
            }
        }).then(function (response) {
            $scope.docGenerationModelCollection.listPropsForSpecificDocType[pindex].ColumnList[index].List = response.data;
        });
    };

    $scope.getMasterMk = function (index, master) {
        $http.get('/WorkflowModule/DocMkCkStage/GetMasterDataBySearch', {
            params: {
                searchKey: 'm',
                masterID: master
            }
        }).then(function (response) {
            $scope.ListItems.ColumnList[index].List = response.data;
        });
    };
 
 














    $scope.CloseModalView = function (modalWindowId) {
        $scope.loading = false;
      
        $(".mydiv").hide();

        $('#' + modalWindowId).modal('hide');
    };



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



    //------MakerModal-------------------------------------

    $scope.ShowUploadFileDiv_MakerModal = function () {
        $("#LoadPdfMakerModal").toggleClass("hidden");
    }
    $scope.LoadPdfMakerModal = function () {
        $scope.loading = true;
        $(".mydiv").show();

        var fileUpload = $("#file_uploadMake").get(0);
        var files = fileUpload.files;
        const file = files[0];
        if (file != null && file != "" && file != "undefined") {
            debugger;

    
            var fileUpload = $("#file_uploadMake")[0];
            var files = fileUpload.files;
            if (files.length > 0) {
                for (var i = 0; i < files.length; i++) {
                    $scope.file_list.push(files[i]);
                    $scope.PDF_TO_Images_AnotherPDF_MakerModal(files[i], 1, 0);
                }
                $scope.ShowUploadImageDivVar = false;

            } else {
                $scope.loading = false;
                $(".mydiv").hide();
                toastr.error('No files selected. Please choose at least one image file to load.');
            }

            $("#LoadPdfMakerModal").toggleClass("hidden");
        }
        else {
            toastr.error("Please the values according to the fields.");
            $scope.loading = false;
            $(".mydiv").hide();
        }

    }

    $scope.PDF_TO_Images_AnotherPDF_MakerModal = function (file, page_num, angle) {
        debugger;
        $scope.loading = true;
        $(".mydiv").show();
        var cookie = $scope.getCookie('access');

        var pdf_container = document.getElementById("dwtHTMakeCreate");
        var innerHTMLContent = pdf_container.innerHTML;
        var innerTextContent = pdf_container.innerText;


        debugger;
        if (file != undefined && page_num > 0 && cookie != "") {
            $scope.pdfToArrayBuffer(file, function (pdfData) {
                debugger;

                // You can use the pdfData ArrayBuffer here
                console.log('PDF data as ArrayBuffer:', pdfData);
                debugger;
                var vBlob = new Blob(pdfData, { type: 'application/pdf' });
                const vvBlob = new Blob([pdfData], { type: 'application/octet-stream' });
                $scope.Temp_PDF_Images = null;
                $scope.Temp_PDF_Images = new Blob([pdfData], { type: 'application/octet-stream' });
                $scope.file_list = [];
                if (innerTextContent !== null && innerTextContent !== "") {
                    // pdf_container exists and is not null
                    $scope.file_list.push(new File([$scope.PDF_Images], 'PDF_0.pdf', { type: 'application/pdf' }));
                    $scope.file_list.push(new File([$scope.Temp_PDF_Images], 'PDF_1.pdf', { type: 'application/pdf' }));
                    $scope.Merge_Page_DirectApi_MakerModal($scope.file_list);

                    $scope.loading = false;
                    $(".mydiv").hide();

                } else {
                    $scope.file_list.push(new File([$scope.Temp_PDF_Images], 'PDF_1.pdf', { type: 'application/pdf' }));
                    $scope.Merge_Page_DirectApi_MakerModal($scope.file_list);

                    $scope.loading = false;
                    $(".mydiv").hide();
                }

                $scope.ShowUploadImageDivVar = false;

            
                debugger;
                // You can call any function or perform further processing here
                // displayPDF(pdfData);
            });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
            }

            $scope.loading = false;
            $(".mydiv").hide();
        }

    }
    $scope.Merge_Page_DirectApi_MakerModal = function (files) {
        $scope.loading = true;
        $(".mydiv").show();

        debugger
        var cookie = $scope.getCookie('access');

        if (files != null && files.length > 0 && cookie != "") {
            const apiEndpoint = 'http://202.59.140.136:8000/api/merge/';
            var formData = new FormData();
            for (var i = 0; i < files.length; i++) {
                formData.append('files', files[i]);
            }
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
                        $scope.LoadPdfFromUrl_Generic_MakerModal(pdfData);
                        debugger;

                        $scope.loading = false;
                        $(".mydiv").hide();
                        // You can call any function or perform further processing here
                        // displayPDF(pdfData);
                    });


                })
                .catch(error => {

                    $scope.loading = false;
                    $(".mydiv").hide();
                    // Handle any errors that occurred during the fetch
                    console.error('Error:', error);
                });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");

                $scope.loading = false;
                $(".mydiv").hide();

            }
        }

    }
    $scope.LoadPdfFromUrl_Generic_MakerModal = function (url) {
        $scope.loading = true;
        $(".mydiv").show();

        //Read PDF from URL.
        debugger
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {

            debugger
            pdfDoc = pdfDoc_;
            $scope.last_page = 0;
            //Reference the Container DIV.
            var pdf_container = document.getElementById("dwtHTMakeCreate");
            //pdf_container.style.display = "inline-flex";
            pdf_container.replaceChildren();


            //Loop and render all pages.
            for (var i = 1; i <= pdfDoc.numPages; i++) {
                $scope.RenderPage_Generic_MakerModal(pdf_container, i, ($scope.last_page + i));
            }

            $scope.last_page = pdfDoc.numPages;
            $scope.loading = false;
            $(".mydiv").hide();

        });

    };
    $scope.RenderPage_Generic_MakerModal = function (pdf_container, num, serial) {
        $scope.loading = true;
        $(".mydiv").show();

        pdfDoc.getPage(num).then(function (page) {
            //Create Canvas element and append to the Container DIV.
            debugger;
            $scope.loading = false;
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
            $(".mydiv").hide();

        });
 
    };

    $scope.RemoveImage_New_MakerModal = function () {
        $scope.loading = true;
        $(".mydiv").show();
        debugger;
        var page_no = 0;
        var file_count = $scope.file_list.length;
        var align_count = 0;

        var arr = []
        for (var i = 1; i <= $scope.last_page; i++) {
            var x = document.getElementById("page-" + i);
            if (x != null && x.checked) {
                page_no = i;
                arr.push(i)
            }
        }
        const arrayString = '[' + JSON.stringify(arr).slice(1, -1) + ']';
        const arrayString2 = JSON.stringify(arr).slice(1, -1);
        debugger;
        if (page_no > 0) {
            //  $scope.Remove_Page($scope.PDF_Images, arrayString);
            $scope.Remove_Page_DirectApi_MakerModal($scope.PDF_Images, arrayString);
            //$scope.Remove_Page_MyApi($scope.PDF_Images, arrayString2);

        } else {
            toastr.error("Please Select at least one page to remove");
            $scope.loading = false;
            $(".mydiv").hide();

        }

        
    }
    $scope.Remove_Page_DirectApi_MakerModal = function (file, page_num) {
        $scope.loading = true;
        $(".mydiv").show();
        var cookie = $scope.getCookie('access');
        var token = $scope.getCookie('access');
        if (file != undefined && page_num.length > 0 && cookie != "") {
            const apiEndpoint = 'http://202.59.140.136:8000/api/removepage/';
            const formData = new FormData();
            formData.append('file', file);
            formData.append('page_num', page_num);
            fetch(apiEndpoint, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}` },
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
                        $scope.LoadPdfFromUrl_Generic_MakerModal(pdfData);
                        debugger;

                        $scope.loading = false;
                        $(".mydiv").hide();

                    });


                })
                .catch(error => {

                    $scope.loading = false;
                    $(".mydiv").hide();
                    // Handle any errors that occurred during the fetch
                    console.error('Error:', error);
                });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");

                $scope.loading = false;
                $(".mydiv").hide();
            }
        }
    }

    $scope.RotateImage_MakerModal = function () {
        $scope.loading = true;
        $(".mydiv").show();
        debugger;
        var page_no = 0;
        var file_count = $scope.file_list.length;
        var align_count = 0;
        for (var i = 1; i <= $scope.last_page; i++) {
            var x = document.getElementById("page-" + i);
            if (x != null && x.checked) {
                page_no = i;
                break;
                //var f = $scope.Page_Rotation.findIndex(x => x.page_no == i);
                //if (f > -1) {
                //    page_no = i;
                //    $scope.Page_Rotation[f].rotate_degree = $scope.Page_Rotation[f].rotate_degree + 90;
                //    align_count = $scope.Page_Rotation[f].rotate_degree;
                //    break;
                //}
            }
        }
        if (page_no > 0) {
            $scope.Rotate_Page_DirectApi_MakerModal($scope.PDF_Images, page_no, 270);

        } else {
            toastr.error("Please Select at least one page to rotate");

            $scope.loading = false;
            $(".mydiv").hide();

        }
      
    }


    $scope.Rotate_Page_DirectApi_MakerModal = function (file, page_num, angle) {
        $scope.loading = true;
        $(".mydiv").show();
        var cookie = $scope.getCookie('access');
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
                        $scope.LoadPdfFromUrl_Generic_MakerModal(pdfData);
                        debugger;


                        $scope.loading = false;
                        $(".mydiv").hide();
                        // You can call any function or perform further processing here
                        // displayPDF(pdfData);
                    });


                })
                .catch(error => {


                    $scope.loading = false;
                    $(".mydiv").hide();
                    // Handle any errors that occurred during the fetch
                    console.error('Error:', error);
                });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");


                $scope.loading = false;
                $(".mydiv").hide();

            }
        }
    }
    $scope.ShowMoveImageDiv_MakerModal = function () {
        $("#WhichImage").val("1");
        $("#Where").val("2");
        $("#MoveImageMakerModal").toggleClass("hidden");
    }

    $scope.MoveImageMakerModal = function () {
        $scope.loading = true;
        $(".mydiv").show();
        $scope.ShowUploadImageDivVar = false;

        var cookie = $scope.getCookie('access');
        var which = $("#WhichImage").val();
        var where = $("#Where").val();
        var file = $scope.PDF_Images;

        if (file != undefined && which > 0 && where > 0 && cookie != "") {
            ImageProcessServices.movepage(file, which, where, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $scope.LoadPdfFromUrl_Generic_MakerModal(data);
                $scope.loading = false;
                $(".mydiv").hide();

            }, function (error) {
                console.log(error.data);
                $scope.loading = false;
                $(".mydiv").hide();

            });

        } else {
            toastr.error("Please the values according to the fields.");
            $scope.loading = false;
            $(".mydiv").hide();
        }
        $("#MoveImageMakerModal").toggleClass("hidden");
    };



 
    //----End Make Modal----
    //----Generic Method----
    $scope.LoadPdfFromUrl_ModalView = function (url,dwtDivName) {
        $scope.Total_Page = 0;
        $scope.last_page = 0;
        //Read PDF from URL.
        $scope.loading = true;
        $(".mydiv").show();
        debugger
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {
            debugger
            pdfDoc = pdfDoc_;
            $scope.Total_Page = pdfDoc.numPages;
            //Reference the Container DIV.
            var  pdf_container = null;
            pdf_container = document.getElementById(dwtDivName);
            //pdf_container.style.display = "inline-flex";

            //Loop and render all pages.
            for (var i = 1; i <= pdfDoc.numPages; i++) {

                $scope.RenderPage(pdf_container, i, ($scope.last_page + i), dwtDivName);
                var rotation = {
                    page_no: $scope.last_page + i,
                    rotate_degree: 0
                }
                $scope.Page_Rotation.push(rotation);
            }

            $scope.last_page = pdfDoc.numPages;
            $scope.loading = false;
            $(".mydiv").hide();

        });
    };
    $scope.RenderPage = function (pdf_container, num, serial, dwtDivName) {
        $scope.loading = true;
        $(".mydiv").show();


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

            $compile(angular.element(document.querySelector('#' + dwtDivName)))($scope);
            $scope.loading = false;
            $(".mydiv").hide();

        });
    };
    //-------------------------------------------------
    //-----Update Modal

    $scope.ShowUploadFileDiv_UpdateModal = function () {
        $("#LoadPdfUpdateModal").toggleClass("hidden");
    }
    $scope.LoadPdfUpdateModal = function () {
        $scope.loading = true;
        $(".mydiv").show();

        debugger;
        var fileUpload = $('#file_uploadUpdate')[0]; // Get the file input element
        var files = fileUpload.files; // Get the selected files

        const file = files[0];
        if (file != null && file != "" && file != "undefined") {
            debugger;

         
            var fileUpload = $("#file_uploadUpdate")[0];
            var files = fileUpload.files;
            if (files.length > 0) {
                for (var i = 0; i < files.length; i++) {
                    $scope.file_list.push(files[i]);
                    $scope.PDF_TO_Images_AnotherPDF_UpdateModal(files[i], 1, 0);
                }
                $scope.ShowUploadImageDivVar = false;

            } else {
                $scope.loading = false;
                $(".mydiv").hide();
                toastr.error('No files selected. Please choose at least one image file to load.');
            }

            $("#LoadPdfUpdateModal").toggleClass("hidden");
        }
        else {
            toastr.error("Please the values according to the fields.");
            $scope.loading = false;
            $(".mydiv").hide();
        }

       
    }

    $scope.PDF_TO_Images_AnotherPDF_UpdateModal = function (file, page_num, angle) {
        debugger;
        $scope.loading = true;
        $(".mydiv").show();
        var cookie = $scope.getCookie('access');

        var pdf_container = document.getElementById("dwtHTMakeUpdate");
        var innerHTMLContent = pdf_container.innerHTML;
        var innerTextContent = pdf_container.innerText;


        debugger;
        if (file != undefined && page_num > 0 && cookie != "") {
            $scope.pdfToArrayBuffer(file, function (pdfData) {
                debugger;

                // You can use the pdfData ArrayBuffer here
                console.log('PDF data as ArrayBuffer:', pdfData);
                debugger;
                var vBlob = new Blob(pdfData, { type: 'application/pdf' });
                const vvBlob = new Blob([pdfData], { type: 'application/octet-stream' });
                $scope.Temp_PDF_Images = null;
                $scope.Temp_PDF_Images = new Blob([pdfData], { type: 'application/octet-stream' });
                $scope.file_list = [];
                if (innerTextContent !== null && innerTextContent !== "") {
                    // pdf_container exists and is not null
                    $scope.file_list.push(new File([$scope.PDF_Images], 'PDF_0.pdf', { type: 'application/pdf' }));
                    $scope.file_list.push(new File([$scope.Temp_PDF_Images], 'PDF_1.pdf', { type: 'application/pdf' }));
                    $scope.Merge_Page_DirectApi_UpdateModal($scope.file_list);
                    $scope.loading = false;
                    $(".mydiv").hide();

                } else {
                    $scope.file_list.push(new File([$scope.Temp_PDF_Images], 'PDF_1.pdf', { type: 'application/pdf' }));
                    $scope.Merge_Page_DirectApi_UpdateModal($scope.file_list);
                    $scope.loading = false;
                    $(".mydiv").hide();
                }

                $scope.ShowUploadImageDivVar = false;
           
                debugger;
                // You can call any function or perform further processing here
                // displayPDF(pdfData);
            });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
            }
            $scope.loading = false;
        }

    }
    $scope.Merge_Page_DirectApi_UpdateModal = function (files) {
        $scope.loading = true;
        $(".mydiv").show();
        debugger
        var cookie = $scope.getCookie('access');

        if (files != null && files.length > 0 && cookie != "") {
            const apiEndpoint = 'http://202.59.140.136:8000/api/merge/';
            var formData = new FormData();
            for (var i = 0; i < files.length; i++) {
                formData.append('files', files[i]);
            }
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
                        $scope.LoadPdfFromUrl_Generic_UpdateModal(pdfData);
                        debugger;
                        $scope.loading = false;
                        $(".mydiv").hide();
                        // You can call any function or perform further processing here
                        // displayPDF(pdfData);
                    });


                })
                .catch(error => {
                    $scope.loading = false;
                    $(".mydiv").hide();
                    console.error('Error:', error);
                });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $(".mydiv").hide();

            }
        }

    }
    $scope.LoadPdfFromUrl_Generic_UpdateModal = function (url) {
        $scope.loading = true;
        $(".mydiv").show();
        //Read PDF from URL.
        debugger
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {

            debugger
            pdfDoc = pdfDoc_;
            $scope.last_page = 0;
            //Reference the Container DIV.
            var pdf_container = document.getElementById("dwtHTMakeUpdate");
            //pdf_container.style.display = "inline-flex";
            pdf_container.replaceChildren();


            //Loop and render all pages.
            for (var i = 1; i <= pdfDoc.numPages; i++) {
                $scope.RenderPage_Generic_UpdateModal(pdf_container, i, ($scope.last_page + i));
            }

            $scope.last_page = pdfDoc.numPages;
            $scope.loading = false;
            $(".mydiv").hide();

        });

    };
    $scope.RenderPage_Generic_UpdateModal = function (pdf_container, num, serial) {
        $scope.loading = true;
        $(".mydiv").show();
        pdfDoc.getPage(num).then(function (page) {
            //Create Canvas element and append to the Container DIV.
            debugger;
            $scope.loading = false;
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

            $compile(angular.element(document.querySelector('#dwtHTMakeUpdate')))($scope);


            $scope.loading = false;
            $(".mydiv").hide();


        });
     
    };

    $scope.RemoveImage_New_UpdateModal = function () {
        $scope.loading = true;
        $(".mydiv").show();

        debugger;
        var page_no = 0;
        var file_count = $scope.file_list.length;
        var align_count = 0;

        var arr = []
        for (var i = 1; i <= $scope.last_page; i++) {
            var x = document.getElementById("page-" + i);
            if (x != null && x.checked) {
                page_no = i;
                arr.push(i)
            }
        }
        const arrayString = '[' + JSON.stringify(arr).slice(1, -1) + ']';
        const arrayString2 = JSON.stringify(arr).slice(1, -1);
        debugger;
        if (page_no > 0) {
            //  $scope.Remove_Page($scope.PDF_Images, arrayString);
            $scope.Remove_Page_DirectApi_UpdateModal($scope.PDF_Images, arrayString);
            //$scope.Remove_Page_MyApi($scope.PDF_Images, arrayString2);

        } else {
            toastr.error("Please Select at least one page to remove");
            $scope.loading = false;
            $(".mydiv").hide();

        }

       
    }
    $scope.Remove_Page_DirectApi_UpdateModal = function (file, page_num) {
        $scope.loading = true;
        $(".mydiv").show();

        var cookie = $scope.getCookie('access');
        var token = $scope.getCookie('access');
        if (file != undefined && page_num.length > 0 && cookie != "") {
            const apiEndpoint = 'http://202.59.140.136:8000/api/removepage/';
            const formData = new FormData();
            formData.append('file', file);
            formData.append('page_num', page_num);
            fetch(apiEndpoint, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}` },
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
                        $scope.LoadPdfFromUrl_Generic_UpdateModal(pdfData);
                        debugger;
                        $scope.loading = false;
                        $(".mydiv").hide();

                    });


                })
                .catch(error => {
                    $scope.loading = false;
                    $(".mydiv").hide();
                });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
                $(".mydiv").hide();
            }
        }
    }

    $scope.RotateImage_UpdateModal = function () {
        $scope.loading = true;
        $(".mydiv").show();
        debugger;
        var page_no = 0;
        var file_count = $scope.file_list.length;
        var align_count = 0;
        for (var i = 1; i <= $scope.last_page; i++) {
            var x = document.getElementById("page-" + i);
            if (x != null && x.checked) {
                page_no = i;
                break;
                //var f = $scope.Page_Rotation.findIndex(x => x.page_no == i);
                //if (f > -1) {
                //    page_no = i;
                //    $scope.Page_Rotation[f].rotate_degree = $scope.Page_Rotation[f].rotate_degree + 90;
                //    align_count = $scope.Page_Rotation[f].rotate_degree;
                //    break;
                //}
            }
        }
        if (page_no > 0) {
            $scope.Rotate_Page_DirectApi_UpdateModal($scope.PDF_Images, page_no, 270);

        } else {
            toastr.error("Please Select at least one page to rotate");
            $scope.loading = false;
            $(".mydiv").hide();

        }
       
    }


    $scope.Rotate_Page_DirectApi_UpdateModal = function (file, page_num, angle) {
        $scope.loading = true;
        $(".mydiv").show();
        var cookie = $scope.getCookie('access');
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
                        $scope.LoadPdfFromUrl_Generic_UpdateModal(pdfData);
                        debugger;

                        $scope.loading = false;
                        $(".mydiv").hide();
                        // You can call any function or perform further processing here
                        // displayPDF(pdfData);
                    });


                })
                .catch(error => {

                    $scope.loading = false;
                    $(".mydiv").hide();
                    // Handle any errors that occurred during the fetch
                    console.error('Error:', error);
                });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");

                $scope.loading = false;
                $(".mydiv").hide();

            }
        }
    }
    $scope.ShowMoveImageDiv_UpdateModal = function () {
        $("#WhichImage").val("1");
        $("#Where").val("2");
        $("#MoveImageUpdateModal").toggleClass("hidden");
    }

    $scope.MoveImageUpdateModal = function () {
        $scope.loading = true;
        $(".mydiv").show();
        $scope.ShowUploadImageDivVar = false;

        var cookie = $scope.getCookie('access');
        var which = $("#WhichImage").val();
        var where = $("#Where").val();
        var file = $scope.PDF_Images;

        if (file != undefined && which > 0 && where > 0 && cookie != "") {
            ImageProcessServices.movepage(file, which, where, cookie).then(function (data) {
                debugger
                $scope.ZoomRef = data;
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $scope.LoadPdfFromUrl_Generic_UpdateModal(data);

                $scope.loading = false;
                $(".mydiv").hide();

            }, function (error) {
                console.log(error.data);

                $scope.loading = false;
                $(".mydiv").hide();

            });

        } else {
            toastr.error("Please the values according to the fields.");

            $scope.loading = false;
            $(".mydiv").hide();
        }
        $("#MoveImageUpdateModal").toggleClass("hidden");
    };







    //********Start Generate Document******************

    //************* Scanning Interfacing: Doc from Sanner*********************

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

            processScannedImage(scannedImage, "dwtHorizontalThumbnil");
        }
    }

    /** Images scanned so far. */
    var imagesScanned = [];

    /** Processes a ScannedImage */
    function processScannedImage(scannedImage, targetElementId) {
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
        $scope.convertImageToPDF(bstr_, 'example.pdf', u8arr_, targetElementId);
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


    $scope.src = '';
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

        $scope.convertImageToPDF(bstr_, 'example.pdf', u8arr_, targetElementId);
        // Optional: Create a download link for the Blob file

    }
    $scope.convertImageToPDF = async function (imageData, pdfPath, u8arr_, targetElementId) {
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

        $scope.PDF_TO_Images_ScanningPDF(pdfBlob, 1, 0, targetElementId);

     
    }
    $scope.PDF_TO_Images_ScanningPDF = function (file, page_num, angle, targetElementId) {
        debugger;
        $scope.loading = true;
        var cookie = $scope.getCookie('access');

        //var pdf_container = document.getElementById("dwtHorizontalThumbnil");
        var pdf_container = document.getElementById(targetElementId);
        var innerHTMLContent = pdf_container.innerHTML;
        var innerTextContent = pdf_container.innerText;


        debugger;
        if (file != undefined && page_num > 0 && cookie != "") {
            $scope.pdfToArrayBuffer(file, function (pdfData) {
                debugger;

                // You can use the pdfData ArrayBuffer here
                console.log('PDF data as ArrayBuffer:', pdfData);
                debugger;
                var vBlob = new Blob(pdfData, { type: 'application/pdf' });
                const vvBlob = new Blob([pdfData], { type: 'application/octet-stream' });
                $scope.Temp_PDF_Images = null;
                $scope.Temp_PDF_Images = new Blob([pdfData], { type: 'application/octet-stream' });
                $scope.file_list = [];
                if (innerTextContent !== null && innerTextContent !== "") {
                    // pdf_container exists and is not null
                    $scope.file_list.push(new File([$scope.PDF_Images], 'PDF_0.pdf', { type: 'application/pdf' }));
                    $scope.file_list.push(new File([$scope.Temp_PDF_Images], 'PDF_1.pdf', { type: 'application/pdf' }));
                    $scope.Merge_Page_DirectApiScanningPDF($scope.file_list, targetElementId);

                } else {
                    $scope.file_list.push(new File([$scope.Temp_PDF_Images], 'PDF_1.pdf', { type: 'application/pdf' }));
                    $scope.Merge_Page_DirectApiScanningPDF($scope.file_list, targetElementId);
                }

                $scope.ShowUploadImageDivVar = false;
                $scope.loading = false;
                debugger;
                // You can call any function or perform further processing here
                // displayPDF(pdfData);
            });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
            }
            $scope.loading = false;
        }

    }
    $scope.Merge_Page_DirectApiScanningPDF = function (files, targetElementId) {
        $scope.loading = true;
        debugger
        var cookie = $scope.getCookie('access');

        if (files != null && files.length > 0 && cookie != "") {
            const apiEndpoint = 'http://202.59.140.136:8000/api/merge/';
            var formData = new FormData();
            for (var i = 0; i < files.length; i++) {
                formData.append('files', files[i]);
            }
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
                        $scope.LoadPdfFromUrl_GenericScanningPDF(pdfData, targetElementId);
                        debugger;

                        // You can call any function or perform further processing here
                        // displayPDF(pdfData);
                    });


                })
                .catch(error => {
                    // Handle any errors that occurred during the fetch
                    console.error('Error:', error);
                });

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;

            }
        }

    }
    $scope.LoadPdfFromUrl_GenericScanningPDF = function (url, targetElementId) {
        $scope.loading = true;

        //Read PDF from URL.
        debugger
        pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {

            debugger
            pdfDoc = pdfDoc_;
            $scope.last_page = 0;
            //Reference the Container DIV.
           // var pdf_container = document.getElementById("dwtHorizontalThumbnil");
            var pdf_container = document.getElementById(targetElementId);
            //pdf_container.style.display = "inline-flex";
            pdf_container.replaceChildren();


            //Loop and render all pages.
            for (var i = 1; i <= pdfDoc.numPages; i++) {
                $scope.RenderPage_GenericScanningPDF(pdf_container, i, ($scope.last_page + i), targetElementId);
            }

            $scope.last_page = pdfDoc.numPages;
            $scope.loading = false;

        });

    };
    $scope.RenderPage_GenericScanningPDF = function (pdf_container, num, serial, targetElementId) {
        $scope.loading = true;

        pdfDoc.getPage(num).then(function (page) {
            //Create Canvas element and append to the Container DIV.
            debugger;
            $scope.loading = false;
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
        $scope.loading = false;
    };
//*****End of Scanning Interface***************************************************************
    //-------Scanning of Maker Modal

    $scope.scanToJpg_MakerModal = function () {
        scanner.scan(displayImagesOnPage_MakerModal,
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
    function displayImagesOnPage_MakerModal(successful, mesg, response) {
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

            processScannedImage(scannedImage, "dwtHTMakeCreate");
        }
    }
    //-------Checker
    $scope.scanToJpg_UpdateModal = function () {
        scanner.scan(displayImagesOnPage_UpdateModal,
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
    function displayImagesOnPage_UpdateModal(successful, mesg, response) {
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

            processScannedImage(scannedImage, "dwtHTMakeUpdate");
        }
    }
}]);

