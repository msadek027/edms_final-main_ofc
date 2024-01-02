﻿app.controller('DocumentScanCtr', ['$scope', '$http', 'ImageProcessServices', 'environmentServices', '$compile', '$injector', '$interval', function ($scope, $http, ImageProcessServices, environmentServices, $compile, $injector, $interval) {

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

    $scope.AcquireImage = function () {

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

    $scope.docPropIdentityModel = {
        OwnerLevel: { OwnerLevelID: "", LevelName: "" },
        Owner: { OwnerID: "", OwnerName: "" },
        DocCategory: { DocCategoryID: "", DocCategoryName: "" },
        DocType: { DocTypeID: "", DocTypeName: "" },
        DocProperty: { DocPropertyID: "", DocPropertyName: "" },
        DocMetaID: {},
        DocPropIdentifyID: {},
        MetaValue: {},
        DocCat: '',
        DocTyp: '',
        Remarks: {}
    };

    $scope.revesionNo = function (row, e) {
        angular.forEach($scope.GridDisplayCollection, function (item) {
            if (item.DocPropertyID == row.DocPropertyID) {
                if (item.IsAuto && row.IsObsoleteIdentity) {
                    var str = row.MetaValue;
                    var lastSlash = str.lastIndexOf("/");
                    var revesionNo = str.substring(lastSlash + 1);

                    $http.post('/DocScanningModule/MultiDocScan/IsRevesionNoValid',
                        {
                            _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                            _DocCatID: $scope.docPropIdentityModel.DocCat,
                            _DocTypID: $scope.docPropIdentityModel.DocTyp,
                            _DocPropertyID: row.DocPropertyID,
                            _DocPropIdentifyID: row.DocPropIdentifyID,
                            _modelDocumentsInfo: $scope.docPropIdentityModel,
                            _MetaValue: row.MetaValue,
                            _revesionNo: revesionNo
                        })
                        .success(function (response) {
                            var rvsNo = parseInt(response.Message) + 1;
                            item.MetaValue = rvsNo;

                            if (rvsNo !== parseInt(revesionNo)) {
                                row.MetaValue = '';
                                item.MetaValue = '';
                                toastr.error("Wrong Revision Number");
                            }
                        }).error(function () {
                            //alert('not ok');
                        });
                }
                else {
                    return;
                }
            }
        });
    };


    $scope.generatePDF_pdfSplit_ZipExtract_FileList = function (data, response, vfilePath, vfileName) {
        debugger
        $scope.response = response;
        $scope.PDF1 = new Blob([data], { type: 'application/pdf' });//pdf      


        ImageProcessServices.DocUpload($scope.response.result[0].ServerIP, $scope.response.result[0].ServerPort, $scope.response.result[0].FtpUserName, $scope.response.result[0].FtpPassword, vfilePath + "//" +
            vfileName + ".pdf",
            vfileName + ".pdf", $scope.PDF1,
            $scope.getCookie('access')).then(function (data) {
                debugger;
                if (data.data == "Success") {
                    toastr.success("Upload Successful");

                } else {
                    $http.post('/DocScanningModule/VersioningOfOriginalDoc/DeleteVersionDocumentInfo',
                        {
                            _DocumentIDs: response.DistinctID[DocIDsCounter].DocumentID
                        })
                        .success(function () {
                            toastr.success("Upload Failed");
                        })
                        .error(function () {
                            toastr.success("Upload Failed");
                        });

                }
                $scope.loading = false;

            }, function (error) {
                console.log(error.data);
                alert(error.data);
                $scope.loading = false;
            });
        $scope.showLoader = false;
        $scope.loading = false;

        if (uploadCount == response.DistinctID.length) {
            $scope.docPropIdentityGridData = response.result;
            $scope.loading = false;
            toastr.success("Upload Successful");
            $scope.BindDataToGrid();
        }

    };



    function pdfSplit_ZipExtract_FileList_Detail(file, token, response, vFilePath1, vFileName1, vFilePath2, vFileName2) {


        const today = new Date();
        let yyyy = today.getFullYear();
        let mm = today.getMonth() + 1; // Months start at 0!
        let dd = today.getDate();
        let vhh = today.getHours();
        let vmm = today.getMinutes();
        let vss = today.getSeconds();

        var vfSuffix = yyyy + mm + dd + "_" + vhh + vmm + vss;
        var fileName = 'splited_' + vfSuffix + '.zip'
        const apiEndpoint = 'http://202.59.140.136:8000/api/split_pdf/';
        const formData = new FormData();
        formData.append('file', file);

        fetch(apiEndpoint, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${token}`, },
            body: formData,
        })
            .then(response => {
                // ...rest of the code remains the same
                debugger;
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
               
                return response.blob(); // Assuming the API response is a blob (binary data).
            })
            .then(zipBlob => {
                // Now you have the ZIP file as a Blob. You can do various things with it.

                // debugger; 
                //// For example, you can create a URL for downloading it:
                //const downloadUrl = URL.createObjectURL(zipBlob);               
                //// Create a link and trigger the download:
                //const downloadLink = document.createElement('a');
                //downloadLink.href = downloadUrl;
                ////downloadLink.download = 'splited.zip'; // Set the desired file name
                //downloadLink.download = fileName; // Set the desired file name
                //downloadLink.click();                
                //URL.revokeObjectURL(downloadUrl);

                debugger;
                const zip = new JSZip();
                JSZip.loadAsync(zipBlob).then(function (zip) {
                    for (let [filename, file] of Object.entries(zip.files)) {
                        // TODO Your code goes here
                        debugger;
                        console.log(filename);
                        var vblob = new Blob();
                        var vvblob = new Blob([vblob], { type: 'application/pdf' });

                        const blob = file.async('blob'); // Read the file as a Blob

                        blob.then((blobData) => {
                            debugger;
                            const a = document.createElement('a');
                            document.body.appendChild(a);
                            a.style.display = 'none';
                            const url = window.URL.createObjectURL(blobData);
                            a.href = url;
                            a.download = filename;
                            a.click();
                            window.URL.revokeObjectURL(url);
                            // Clean up the DOM element
                            document.body.removeChild(a);

                        });
                    }


                }).catch(function (err) {
                    console.error("Failed to open", filename, " as ZIP file:", err);


                })


            })
            .catch(error => {
                console.error('There was a problem with the fetch operation:', error);
            }); 


    }
   




    //-----------------------------


    $scope.fileListF = [];
    function pdfSplit_ZipExtract_FileList(file, token) {
        return new Promise(function (resolve, reject) {
            const today = new Date();
            let yyyy = today.getFullYear();
            let mm = today.getMonth() + 1; // Months start at 0!
            let dd = today.getDate();
            let vhh = today.getHours();
            let vmm = today.getMinutes();
            let vss = today.getSeconds();

            var vfSuffix = yyyy + mm + dd + "_" + vhh + vmm + vss;
            var fileName = 'splited_' + vfSuffix + '.zip';
            const apiEndpoint = 'http://202.59.140.136:8000/api/split_pdf/';
            const formData = new FormData();
            formData.append('file', file);

            fetch(apiEndpoint, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}` },
                body: formData,
            })
                .then(function (response) {
                    debugger;
                    // Check if the response status is OK (e.g., 200)
                    if (!response.ok) { // Changed from response.status !== 200
                        throw new Error('Network response was not ok'); // Use throw to trigger the catch block                        
                    }
                    //console.log(response.body.json());
                    return response.blob(); // Assuming the API response is a blob (binary data).
                })
                .then(function (zipBlob) {
                    return JSZip.loadAsync(zipBlob); // Load the ZIP asynchronously
                })
                .then(function (loadedZip) {
                    const fileList = [];
                    debugger;
                    Promise.all(
                        Object.entries(loadedZip.files).map(function ([filename, file]) {
                            // Read each file as a Blob and store it in fileList
                            return file.async('blob').then(function (blobData) {
                                fileList.push(blobData);
                            });
                        })
                    )
                        .then(function () {
                            resolve(fileList); // Resolve the promise with the file list
                        })
                        .catch(function (err) {
                            console.error("Failed to read files from ZIP:", err);
                            reject(err);
                        });
                })
                .catch(function (error) {
                    console.error('There was a problem:', error);
                    reject(error);
                });
        });
    }


    function pdfSplit_ZipExtract_FileList_Arman(file, token) {
        return new Promise(function (resolve, reject) {
            const today = new Date();
            let yyyy = today.getFullYear();
            let mm = today.getMonth() + 1; // Months start at 0!
            let dd = today.getDate();
            let vhh = today.getHours();
            let vmm = today.getMinutes();
            let vss = today.getSeconds();

            var vfSuffix = yyyy + mm + dd + "_" + vhh + vmm + vss;
            var fileName = 'splited_' + vfSuffix + '.zip';
            const apiEndpoint = 'http://202.59.140.136:8000/api/split_pdf/';
            const formData = new FormData();
            formData.append('file', file);

            fetch(apiEndpoint, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}` },
                body: formData,
            })             
                .then(function (response) {
                    debugger;
                    // Check if the response status is OK (e.g., 200)
                    if (!response.ok) { // Changed from response.status !== 200
                        throw new Error('Network response was not ok'); // Use throw to trigger the catch block                        
                    }
                    //console.log(response.body.json());
                    return response.blob(); // Assuming the API response is a blob (binary data).
                })               
                .then(function (zipBlob) {
                    return JSZip.loadAsync(zipBlob); // Load the ZIP asynchronously
                })
                .then(function (loadedZip) {
                    const fileList = [];
                    debugger;
                    Promise.all(
                        Object.entries(loadedZip.files).map(function ([filename, file]) {
                            // Read each file as a Blob and store it in fileList
                            return file.async('blob').then(function (blobData) {
                                fileList.push(blobData);
                            });
                        })
                    )
                        .then(function () {
                            resolve(fileList); // Resolve the promise with the file list
                        })
                        .catch(function (err) {
                            console.error("Failed to read files from ZIP:", err);
                            reject(err);
                        });
                })
                .catch(function (error) {
                    console.error('There was a problem:', error);
                    //reject(error);
                    resolve(file);
                });
        });
    }

    function pdfSplit_ZipExtract_FileList_Final(file, token) {
        return new Promise(function (resolve, reject) {
            const today = new Date();
            let yyyy = today.getFullYear();
            let mm = today.getMonth() + 1; // Months start at 0!
            let dd = today.getDate();
            let vhh = today.getHours();
            let vmm = today.getMinutes();
            let vss = today.getSeconds();

            var vfSuffix = yyyy + mm + dd + "_" + vhh + vmm + vss;
            var fileName = 'splited_' + vfSuffix + '.zip';
            const apiEndpoint = 'http://202.59.140.136:8000/api/split_pdf/';
            const formData = new FormData();
            formData.append('file', file);

            fetch(apiEndpoint, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${token}` },
                body: formData,
            })
                .then(function (response) {
                    debugger;
                    // Check if the response status is OK (e.g., 200)
                    if (!response.ok) { // Changed from response.status !== 200
                        throw new Error('Network response was not ok'); // Use throw to trigger the catch block                        
                    }
                    //console.log(response.body.json());
                    const contentType = response.headers.get('content-type');

                    if (contentType && contentType.includes('application/zip')) {
                        debugger;
                        return response.blob();
                    } else {
                        debugger;
                        return response.text;
                    }
                })
                .then(data => {
                    if (data instanceof Blob) {
                        //const downloadUrl = URL.createObjectURL(data);
                        //const downloadLink = document.createElement('a');
                        //downloadLink.href = downloadUrl;
                        //downloadLink.download = 'splited.zip';
                        //downloadLink.click();
                        //URL.revokeObjectURL(downloadUrl);

                        debugger;
                        return JSZip.loadAsync(data); // Load the ZIP asynchronously

                    } else {
                        console.log('API response is not a ZIP file. Message:', data);
                        //alert(data);
                        const fileList = [];
                        debugger;
                    
                        fileList.push(file);
                        debugger;
                        //return fileList;
                       
                        resolve(fileList);

                    }
                })
                //.then(function (zipBlob) {
                //    return JSZip.loadAsync(zipBlob); // Load the ZIP asynchronously
                //})
                .then(function (loadedZip) {
                    const fileList = [];
                    debugger;
                    Promise.all(
                        Object.entries(loadedZip.files).map(function ([filename, file]) {
                            // Read each file as a Blob and store it in fileList
                            return file.async('blob').then(function (blobData) {
                                fileList.push(blobData);
                            });
                        })
                    )
                        .then(function () {
                            resolve(fileList); // Resolve the promise with the file list
                        })
                        .catch(function (err) {
                            console.error("Failed to read files from ZIP:", err);
                            reject(err);
                        });
                })
                .catch(function (error) {
                    console.error('There was a problem:', error);
                    debugger;
                    reject(error);
                  //  resolve(file);
                });
        });
    }

    // Call the function to start processing
    function blobUrlToBase64(blobUrl) {
        return new Promise((resolve, reject) => {
            // Fetch the Blob data
            fetch(blobUrl)
                .then(response => response.blob())
                .then(blob => {
                    // Read the Blob as Base64
                    const reader = new FileReader();
                    reader.onload = function () {
                        const base64Data = reader.result.split(',')[1]; // Extract the Base64 data
                        resolve(base64Data);
                    };
                    reader.readAsDataURL(blob);
                })
                .catch(error => {
                    console.error('Error fetching or converting Blob to Base64:', error);
                    reject(error);
                });
        });
    }
    $scope.ImgListFrmBlobUrl = function (url) {
        // Create a promise to handle the asynchronous operation
        return new Promise(function (resolve, reject) {
            // Read PDF from URL.
            pdfjsLib.getDocument(url).promise.then(function (pdfDoc_) {
                pdfDoc = pdfDoc_;
                $scope.Total_Page = pdfDoc.numPages;
                $scope.last_page = 0;

                var ArrImg = [];

                // Loop and render all pages.
                for (var i = 1; i <= pdfDoc.numPages; i++) {
                    pdfDoc.getPage(i).then(function (page) {
                        // Render each page to a canvas
                        var canvas = document.createElement('canvas');
                        var context = canvas.getContext('2d');
                        var viewport = page.getViewport({ scale: 1.5 }); // Adjust scale as needed
                        canvas.width = viewport.width;
                        canvas.height = viewport.height;

                        var renderContext = {
                            canvasContext: context,
                            viewport: viewport,
                        };

                        page.render(renderContext).promise.then(function () {
                            // Convert the canvas content to a Base64 image
                            var imageDataUrl = canvas.toDataURL('image/jpeg'); // Change format as needed
                            ArrImg.push(imageDataUrl);

                            // Check if all pages have been processed
                            if (ArrImg.length === pdfDoc.numPages) {
                                $scope.last_page = pdfDoc.numPages;
                                resolve(ArrImg); // Resolve the promise with the image list
                            }
                        });
                    });
                }
            });
        });
    };


    // Function to convert a Blob (PDF) URL to an array of image Base64 strings
    async function blobPdfUrlToImageBase64Array(pdfUrl) {
   
        const pdf = await pdfjsLib.getDocument(pdfUrl).promise;
        const numPages = pdf.numPages;
        const imageBase64Array = [];
  
        for (let pageNumber = 1; pageNumber <= numPages; pageNumber++) {
            const page = await pdf.getPage(pageNumber);
            const viewport = page.getViewport({ scale: 1.0 }); // You can adjust the scale as needed
         
            const canvas = document.createElement('canvas');
            const context = canvas.getContext('2d');
            canvas.width = viewport.width;
            canvas.height = viewport.height;

            const renderContext = {
                canvasContext: context,
                viewport: viewport,
            };

            await page.render(renderContext).promise;
        
            const imageBase64 = canvas.toDataURL('image/jpeg'); // Change the format as needed
            imageBase64Array.push(imageBase64);

           
        }

        return imageBase64Array;
    }

  
    //-----------------------------
    $scope.SaveImageDemo = function () {

        $('#ConfirmSave').modal('hide');
        var cookie = $scope.getCookie('access');
        debugger;

        var pdfUrl = URL.createObjectURL($scope.PDF_Images); 


        debugger;

        //$scope.ImgListFrmBlobUrl(pdfUrl)
        //    .then(function (listImage) {
        //        // You can now use the 'listImage' array containing Base64 images
        //        console.log('List of Images:', listImage);
        //        debugger;
        //    })
        //    .catch(function (error) {
        //        console.error('Error:', error);
        //    });


        //blobPdfUrlToImageBase64Array(pdfUrl)
        //    .then(function (imageBase64Array) {
        //        debugger;
        //        if (imageBase64Array.length > 0) {
        //            console.log('Base64 Images:', imageBase64Array);

        //            debugger;
        //        } else {
        //            console.log('No images found in the PDF.');
        //        }
        //    })
        //    .catch(function (error) {
        //        console.error('Error converting Blob (PDF) URL to image Base64:', error);
        //    });

        //blobUrlToBase64(pdfUrl)
        //    .then(base64Data => {
        //        if (base64Data) {
        //            // Use the Base64 data as needed
        //            console.log('Base64 Data:', base64Data);

        //            debugger;
        //        } else {
        //            console.error('Base64 data is null.');
        //        }
        //    })
        //    .catch(error => {
        //        console.error('An error occurred:', error);
        //    });
    
    

 

      

        pdfSplit_ZipExtract_FileListNishad($scope.PDF_Images, cookie)
            .then(function (fileList) {
                // Store the fileList in $scope.fileListF
                $scope.fileListF = fileList;
                console.log('File list:', fileList);
                debugger;
                var ln = $scope.fileListF.length;
                debugger;
               
            })
            .catch(function (error) {
                // Handle errors here
                debugger;
                console.error('Error:', error);
            });

    };
    $scope.SaveImage= function () {
        $('#ConfirmSave').modal('hide');
        debugger
        var cookie = $scope.getCookie('access');
        $scope.loading = true;
        $scope.docPropIdentityModel.DocCategory.DocCategoryID = $scope.docPropIdentityModel.DocCat;
        $scope.docPropIdentityModel.DocType.DocTypeID = $scope.docPropIdentityModel.DocTyp;

        var selectedProperties = [];

        if ($scope.docPropertyForSpecificDocType.length < 1) {
            toastr.error("No Document was Selected");
            $scope.loading = false;
            return;
        }

        angular.forEach($scope.docPropertyForSpecificDocType, function (item) {
            if (item.IsSelected) {
                selectedProperties.push({ DocPropertyID: item.DocPropertyID, HasSMS: item.sms, HasEmail: item.email, IsObsoletable: item.obsulate, NotifyDate: item.NotifyDate, ExpDate: item.ExpDate });
            };
        });

        var DocIDsCounter = 0;
        var fileUpload = $("#FileUpload1").get(0);
        var files = fileUpload.files;
        var extentionsArr = [];

        if ($scope.uploadType == '2') {
            angular.forEach(files, function (item) {
                extentionsArr.push(item.name.split('.').pop());
            });
        }

        $http.post('/DocScanningModule/MultiDocScan/AddDocumentInfo',
            {
                _modelDocumentsInfo: $scope.docPropIdentityModel,
                _SelectedPropID: selectedProperties,
                _docMetaValues: $scope.docPropIdentityGridData,
                _otherUpload: $scope.uploadType == '2' ? true : false,
                _extentions: extentionsArr.join(),
                IsSecured: $scope.uploadType == '3' ? true : false
            })
            .success(function (response) {//Response get all of ftp server Info after matadata saved
                $scope.loading = false;
                var uploadCount = 0;
                var docCounts = 0;

                if ($scope.uploadType == '2') {

                    if (files.length > 0 && files.length == response.DistinctID.length) {
                        var breakFlag = false;

                        for (var i = 0; i < files.length || breakFlag; i++) {
                            if (window.FormData !== undefined) {
                                var fileData = new FormData();

                                fileData.append('serverIP', response.result[0].ServerIP);
                                fileData.append('ftpPort', response.result[0].ServerPort);
                                fileData.append('ftpUserName', response.result[0].FtpUserName);
                                fileData.append('ftpPassword', response.result[0].FtpPassword);
                                fileData.append('serverURL', response.DistinctID[i].FileServerUrl);
                                fileData.append('documentID', response.DistinctID[i].DocumentID);
                                fileData.append('Ext', files[i].name.split('.').pop());

                                fileData.append(files[i].name, files[i]);

                                $.ajax({
                                    url: '/DocScanningModule/MultiDocScan/UploadOtherFiles',
                                    type: "POST",
                                    contentType: false,
                                    processData: false,
                                    data: fileData,
                                    success: function (result) {
                                        uploadCount++;
                                        if (uploadCount == files.length) {
                                            $scope.docPropIdentityGridData = response.result;
                                            $scope.BindDataToGrid();
                                            toastr.success("Document Uploaded Succesfully");
                                            $scope.loading = false;

                                            var $el = $('#FileUpload1');
                                            $el.wrap('<form>').closest('form').get(0).reset();
                                            $el.unwrap();
                                        }
                                    },
                                    error: function (err) {
                                        toastr.error("Document Upload Falied");
                                        breakFlag = true;
                                        $scope.loading = false;
                                    }
                                });
                            } else {
                                toastr.error("FormData is not supported.");
                                $scope.loading = false;
                                break;
                            }
                        }
                    }
                    else {
                        deleteFailedDocuments(response.DistinctID)
                    }
                }// $scope.uploadType == '2'
                else if ($scope.uploadType == '3') {

                    debugger;
                    $scope.loading = false;

                    var imagedata = [];

                    for (var i = 0; i <= DWObject.HowManyImagesInBuffer - 1; i++) {
                        if (DWObject.IsBlankImageExpress(i)) {
                            DWObject.IfShowFileDialog = false;
                            DWObject.GetSelectedImagesSize(EnumDWT_ImageType.IT_PDF);
                            imagedata.push(DWObject.SaveSelectedImagesToBase64Binary());
                            DWObject.RemoveAllSelectedImages();
                            DWObject.RemoveImage(0);
                            i = -1;
                        } else {
                            DWObject.SelectedImagesCount = i + 1;
                            DWObject.SetSelectedImageIndex(i, i);
                        }
                    }

                  




                    debugger;
                    if (imagedata.length > 0 && imagedata.length == response.DistinctID.length) {
                        var breakFlag = false;
                        var updateCount = 0;
                        for (var j = 0; j < imagedata.length && !breakFlag; j++) {
                            var param = encodeURIComponent("serverIP") + "=" + encodeURIComponent(response.result[0].ServerIP)
                                + "&" + encodeURIComponent("ftpPort") + "=" + encodeURIComponent(response.result[0].ServerPort)
                                + "&" + encodeURIComponent("ftpUserName") + "=" + encodeURIComponent(response.result[0].FtpUserName)
                                + "&" + encodeURIComponent("ftpPassword") + "=" + encodeURIComponent(response.result[0].FtpPassword)
                                + "&" + encodeURIComponent("serverURL") + "=" + encodeURIComponent(response.DistinctID[j].FileServerUrl)
                                + "&" + encodeURIComponent("documentID") + "=" + encodeURIComponent(response.DistinctID[j].DocumentID)
                                + "&" + encodeURIComponent("RemoteFile") + "=" + encodeURIComponent(imagedata[j]);

                            var ajaxRequest = new XMLHttpRequest();
                            debugger;
                            try {
                                ajaxRequest.open('POST', '/DocScanningModule/MultiDocScan/FilePassWord_r', true);
                                ajaxRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
                                ajaxRequest.onreadystatechange = function () {
                                    if (ajaxRequest.readyState == 4 && ajaxRequest.status == 200) {
                                        updateCount++;
                                        if (updateCount == response.DistinctID.length) {
                                            $scope.docPropIdentityGridData = response.result;
                                            toastr.success("Upload Successful");
                                            $scope.BindDataToGrid();
                                        }
                                    }
                                    else if (ajaxRequest.readyState == 4 && ajaxRequest.status != 200) {
                                        breakFlag = true;
                                        deleteFailedDocuments(response.DistinctID)
                                    }
                                }

                                ajaxRequest.send(param);
                            }
                            catch (e) {
                                break;
                            }
                        }
                    }
                    else {
                        deleteFailedDocuments(response.DistinctID)
                    }
                }// End of  $scope.uploadType == '3'
                else {   //When not equal to: $scope.uploadType == '3' And $scope.uploadType == '2'
           
                    debugger;
                    pdfSplit_ZipExtract_FileList_Final($scope.PDF_Images, cookie)
                        .then(function (fileList) {
                            // Store the fileList in $scope.fileListF
                            $scope.fileListF = fileList;
                            console.log('File list:', fileList);
                            debugger;
                            var ln = $scope.fileListF.length;
                            debugger;
                            if ($scope.fileListF.length == response.DistinctID.length) {
                                for (var i = 0; i < $scope.fileListF.length; i++) {
                                    debugger;
                                    $scope.generatePDF_pdfSplit_ZipExtract_FileList($scope.fileListF[i], response, response.DistinctID[i].FileServerUrl, response.DistinctID[i].DocumentID)
                                }
                            }
                        })
                        .catch(function (error) {
                            // Handle errors here
                            console.error('Error:', error);
                        });               

                 
                    debugger;

                    //var cookie = $scope.getCookie('access');
                    //ImageProcessServices.rotate($scope.PDF_Images, 1, 0, cookie).then(function (data) {
                    //    debugger;
                    //    if ($scope.Blank_Page_Count.length > 0 && $scope.Blank_Page_Count.length == response.DistinctID.length) {
                    //        $scope.generatePDFfrompdf($scope.PDF_Images, data, $scope.Blank_Page_Count, response);
                    //    }                    
                    //}, function (error) {
                    //    console.log(error.data);
                    //    alert(error.data);
                    //    $scope.loading = false;
                    //});
                }
            })
            .error(function () {
                $scope.loading = false;
                toastr.error("Failed to Save Meta Data.");
            });
    };

    var deleteFailedDocuments = function (arrayDistinctID) {
        var savedDocID = new Array();
        angular.forEach(arrayDistinctID, function (item) {
            savedDocID.push(item.DocumentID);
        });

        $scope.loading = true;

        $http.post('/DocScanningModule/MultiDocScan/DeleteDocumentInfo',
            {
                _DocumentIDs: savedDocID.join()
            })
            .success(function (response) {
                $scope.loading = false;
                toastr.error("Scanned documents count is different than your selection");
            })
            .error(function () {
                $scope.loading = false;
            });
    }

    $scope.LoadImage = function () {
        DWObject.Addon.PDF.Download("../Resources/addon/Pdf.zip",  // specify the url of the add-on resource
            function () { console.log('Successfully Downloaded PDF add-on'); }, // successful callback
            function (errorCode, errorString) { alert(errorString); }  // failure callback and show error message
        );

        DWObject.Addon.PDF.SetResolution(200);
        DWObject.Addon.PDF.SetConvertMode(EnumDWT_ConverMode.CM_RENDERALL);

        DWObject.IfShowFileDialog = true;

        DWObject.RemoveAllImages();
        DWObject.LoadImageEx("", EnumDWT_ImageType.IT_PDF, function () { }, function () { });

        $scope.ShowUploadImageDivVar = false;
    };

    $scope.LoadImageWithoutBlank = function () {
        DWObject.IfShowFileDialog = true;
        DWObject.HTTPDownloadEx('', '/Buffer/BlankPDF/blank.pdf', 3);
        $scope.ShowUploadImageDivVar = false;
    };

    $scope.Load = function () {
        DWObject.IfShowFileDialog = true;
        DWObject.LoadImageEx("", EnumDWT_ImageType.IT_PDF);
        $scope.ShowUploadImageDivVar = false;
    };

    $scope.RemoveAllSelectedImages = function () {
        DWObject.RemoveAllSelectedImages();
    };

    $scope.RotateRight = function () {
        DWObject.RotateRight(DWObject.GetSelectedImageIndex(0));
    };

    $scope.ShowMoveImageDiv = function () {
        $("#MoveImage").toggleClass("hidden");
        $scope.ShowUploadImageDivVar = false;
    };

    $scope.MoveImage = function () {
        //DWObject.MoveImage(($("#WhichImage").val() - 1), ($("#Where").val() - 1));
        $("#MoveImage").toggleClass("hidden");
        $scope.ShowUploadImageDivVar = false;
    };

    $scope.updateLargeViewer = function () {
        DWObject.CopyToClipboard(DWObject.CurrentImageIndexInBuffer);
        DWObjectLargeViewer.LoadDibFromClipboard();
        $('#viewerModal').modal('show');
    };

    $scope.SelectAll = function () {
        $scope.btnShow = true;
        angular.forEach($scope.docPropertyForSpecificDocType, function (item) {
            item.IsSelected = true;
        });

        $scope.BindDataToGrid();
    };

    $scope.UnSelectAll = function () {
        $scope.btnShow = false;
        angular.forEach($scope.docPropertyForSpecificDocType, function (item) {
            item.IsSelected = false;
        });

        $scope.BindDataToGrid();
    };

    $scope.itemsByPage = 10;
    $scope.loading = true;

    $http.get('/DocScanningModule/OwnerProperIdentity/GetOwnerLevel?_OwnerLevelID=')
        .success(function (response) {
            $scope.ownerLevels = response.result;
            //$scope.docPropIdentityModel.OwnerLevel ="";
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
                $scope.docPropIdentityModel.DocCategory.DocCategoryID = "";
                $scope.docPropIdentityModel.DocType.DocTypeID = "";
                $scope.docPropIdentityModel.DocProperty = "";
                $scope.docPropertyForSpecificDocType = "";
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

    $scope.DocCatForOwner = [];
    $scope.$watch('docPropIdentityModel.Owner', function (newVal) {
        if (newVal) {
            $scope.docPropIdentityModel.DocCategory.DocCategoryID = "";
            $scope.docPropIdentityModel.DocType.DocTypeID = "";
            $scope.docPropIdentityModel.DocProperty = "";
            $scope.docPropertyForSpecificDocType = "";
            $scope.docPropIdentityGridData = [];

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
            $scope.docPropIdentityModel.DocType.DocTypeID = "";
            $scope.docPropIdentityModel.DocProperty = "";
            $scope.docPropertyForSpecificDocType = "";
            $scope.docPropIdentityGridData = [];

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

    $scope.$watch('docPropIdentityModel.DocTyp', function (newVal) {
        if (newVal) {
            $scope.docPropIdentityModel.DocProperty = "";
            $scope.docPropertyForSpecificDocType = "";
            $scope.docPropIdentityGridData = [];
            //$scope.docPropIdentityGridData = [];

            $http.post('/DocScanningModule/MultiDocScan/GetDocumentProperty',
                {
                    _DocCategoryID: $scope.docPropIdentityModel.DocCat,
                    _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                    _DocTypeID: $scope.docPropIdentityModel.DocTyp
                })
                .success(function (response) {
                    $scope.docPropertyForSpecificDocType = response.result;
                    $scope.loading = false;
                    $scope.BindDataToGrid();
                }).error(function () {
                    $scope.loading = false;
                });
        }
    });

    $scope.BindDataToGrid = function () {
        var selectedPropID = new Array();
        angular.forEach($scope.docPropertyForSpecificDocType, function (item) {
            if (item.IsSelected) {
                selectedPropID.push(item.DocPropertyID);
            };
        });

        if (selectedPropID.length < 1) {
            $scope.docPropIdentityGridData = [];
            $scope.GridDisplayCollection = [];
            return;
        }

        $http.post('/DocScanningModule/MultiDocScan/GetDocPropIdentityForSelectedDocTypes',
            {
                _DocCategoryID: $scope.docPropIdentityModel.DocCat,
                _OwnerID: $scope.docPropIdentityModel.Owner.OwnerID,
                _DocTypeID: $scope.docPropIdentityModel.DocTyp,
                _SelectedPropID: selectedPropID.join()
            })
            .success(function (response) {
                $scope.docPropIdentityGridData = response.objDocPropIdentifies;
                $scope.GridDisplayCollection = response.objDocPropIdentifies;
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });
    };

    $scope.ResetModel = function () {
        $scope.docPropIdentityModel.OwnerLevel = "";
        $scope.docPropIdentityModel.Owner = "";
        //$scope.docPropIdentityModel.DocCategory = "";
        $scope.docPropIdentityModel.DocType = "";
        $scope.docPropIdentityModel.DocProperty = "";
        $scope.docPropertyForSpecificDocType = "";
        $scope.docPropIdentityGridData = [];
    };

    $scope.toggleRefreshTable = function (row) {
        location.reload();
    };

    $scope.ShowConfirmModal = function () {
        $('#ConfirmSave').modal('show');
    };

    $scope.dateOptionsNotify = {
        minDate: new Date()
    };

    $scope.selectDate = function (NotDate, index, obj) {
        $scope.docPropertyForSpecificDocType[index].ExpDate = null;
        obj.minDate = NotDate;
    };

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
        $('#viewerInitialModal').modal('hide');

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
        var pdfUrl = URL.createObjectURL(file);
        if (file != undefined && page_num > 0 && cookie != "") {
            debugger;

          $scope.pdfToArrayBuffer(file, function (pdfData) {
                // You can use the pdfData ArrayBuffer here
              console.log('PDF data as ArrayBuffer:', pdfData);
              debugger;
              $scope.ZoomRef = pdfData;
              const blob = new Blob([pdfData], { type: 'application/octet-stream' });
              //$scope.PDF_Images = new Blob(pdfData, { type: 'application/pdf' });
              $scope.PDF_Images = new Blob([pdfData], { type: 'application/octet-stream' });
                $scope.LoadPdfFromUrl(pdfData);
               $scope.showLoader = false;
               $scope.loading = false;
                debugger;
                // You can call any function or perform further processing here
                // displayPDF(pdfData);
            });
            debugger;
            $scope.loading = false;
   


         





            //ImageProcessServices.rotate(file, page_num, angle, cookie).then(function (data) {
            //    debugger
            //    $scope.ZoomRef = data;              
            //    $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
            //    $scope.LoadPdfFromUrl(data);
            //    $scope.showLoader = false;
            //    $scope.loading = false;

            //}, function (error) {
            //    console.log(error.data);
            //    alert(error.data);
            //    $scope.loading = false;
            //});

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
        debugger;
        if (file != undefined && page_num > 0 && cookie != "") {
            $scope.pdfToArrayBuffer(file, function (pdfData) {
                // You can use the pdfData ArrayBuffer here
                console.log('PDF data as ArrayBuffer:', pdfData);
                // $scope.ZoomRef = pdfData;
                // $scope.PDF_Images = new Blob(pdfData, { type: 'application/pdf' });
                //$scope.LoadPdfFromUrl(pdfData);
                //$scope.showLoader = false;
                //$scope.loading = false;
                debugger;
                var vBlob = new Blob(pdfData, { type: 'application/pdf' });
                const vvBlob = new Blob([pdfData], { type: 'application/octet-stream' });
                $scope.Temp_PDF_Images = null;
                $scope.Temp_PDF_Images = new Blob([pdfData], { type: 'application/octet-stream' });
                $scope.file_list = [];
                $scope.file_list.push(new File([$scope.PDF_Images], 'PDF_0.pdf', { type: 'application/pdf' }));
                $scope.file_list.push(new File([$scope.Temp_PDF_Images], 'PDF_1.pdf', { type: 'application/pdf' }));

                $scope.Merge_Page($scope.file_list);
                $scope.IsLoadImageClicked = true;
                $scope.ShowUploadImageDivVar = false;
                $scope.loading = false;

                debugger;
                // You can call any function or perform further processing here
                // displayPDF(pdfData);
            });
            $scope.loading = false;

            //ImageProcessServices.rotate(file, page_num, angle, cookie).then(function (data) {
            //    debugger;
            //    var fileUpload = $("#file_upload")[0];
            //    $scope.file_list = [];
            //    $scope.file_list.push(new File([$scope.PDF_Images], 'PDF_0.pdf', { type: 'application/pdf' }));
            //    $scope.file_list.push(new File([data.data], 'PDF_1.pdf', { type: 'application/pdf' }));
            //    $scope.Merge_Page($scope.file_list);

            //    $scope.IsLoadImageClicked = true;
            //    $scope.ShowUploadImageDivVar = false;
            //    $scope.loading = false;

            //}, function (error) {
            //    console.log(error.data);
            //    alert(error.data);
            //    $scope.loading = false;
            //});

        } else {
            if (cookie == "") {
                alart("Please log in to generate new access token");
                $scope.loading = false;
            }
        }
    }


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
    function generatePDF(images) {
        // Create a new PDF document
        const pdfDoc = new pdfjsLib.Document();

        // Loop through each image and add it as a page to the PDF
        for (let i = 0; i < images.length; i++) {
            const imageBase64 = images[i];
            const page = pdfDoc.addPage([imageWidth, imageHeight]); // Set the page size as needed
            const image = page.image(imageBase64, 0, 0, { width: imageWidth, height: imageHeight });
        }

        // Generate the PDF file
        const pdfBytes = pdfDoc.save();

        // Create a Blob from the PDF data
        const pdfBlob = new Blob([pdfBytes], { type: 'application/pdf' });

        // Create a download link for the PDF and trigger the download
        const pdfUrl = URL.createObjectURL(pdfBlob);
        const downloadLink = document.createElement('a');
        downloadLink.href = pdfUrl;
        downloadLink.download = 'output.pdf'; // Set the desired filename
        downloadLink.click();

        // Clean up the URL object
        URL.revokeObjectURL(pdfUrl);
    }

    $scope.RemoveImage_New = function () {
        $scope.loading = true;
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
        debugger;
        if (page_no > 0) {
            $scope.Remove_Page($scope.PDF_Images, arrayString);

        } else {
            toastr.error("Please Select at least one page to remove");
            $scope.loading = false;

        }



    }
    $scope.Remove_Page = function (file, page_num) {
        $scope.loading = true;
        var cookie = $scope.getCookie('access');
        if (file != undefined && page_num.length > 0 && cookie != "") {
            ImageProcessServices.removepage(file, page_num, cookie).then(function (data) {
                debugger;
                $scope.ZoomRef = data;
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $scope.LoadPdfFromUrl_Generic(data);
                $scope.loading = false;
            }, function (error) {
                debugger;
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

    //$scope.Remove_Page_Recursive = function (file, page_num) {
    //    $scope.loading = true;
    //    var cookie = $scope.getCookie('access');
    //    if (file != undefined && page_num > 0 && cookie != "") {
    //        ImageProcessServices.removepage(file, page_num, cookie).then(function (data) {
    //            debugger
    //            $scope.ZoomRef = data;
    //            $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
    //            $scope.LoadPdfFromUrl_Generic(data);
    //            $scope.loading = false;

    //        }, function (error) {
    //            console.log(error.data);
    //            alert(error.data);
    //            $scope.loading = false;
    //        });
    //    } else {
    //        if (cookie == "") {
    //            alart("Please log in to generate new access token");
    //            $scope.loading = false;
    //        }
    //    }
    //}


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
                debugger;
                $scope.ZoomRef = data;
                $scope.Total_Page = $scope.Total_Page + 1;
                $scope.PDF_Images = new Blob([data.data], { type: 'application/pdf' });
                $scope.LoadPdfFromUrl_Generic(data);
                $scope.loading = false;

            }, function (error) {
                debugger;
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


    //$scope.MergeImage_New = function () {
    //    if ($scope.IsLoadImageClicked) {
    //        //$('#file_upload').click();
    //        var fileUpload = $("#file_upload")[0];
    //        $scope.file_list = [];
    //        $scope.file_list.push(new File([$scope.PDF_Images], 'PDF_0' + '.pdf', { type: 'application/pdf' }));
    //        for (var i = 0; i < fileUpload.files.length; i++) {
    //            var file_ = new File([fileUpload.files[i]], 'PDF_' + (i + 1) + '.pdf', { type: 'application/pdf' });

    //            $scope.file_list.push(file_);

    //            $scope.Merge_Page($scope.file_list);

    //            $scope.IsLoadImageClicked = true;
    //        }
    //        $scope.ShowUploadImageDivVar = false;
    //    } else {
    //        toastr.error('To load a PDF, Please click on "Load PDF"');
    //    }
    //}


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



    //-----------------------------------------
    $scope.loadPDF_ = function () {
        var fileInput = document.getElementById('file_upload');
        var file = fileInput.files[0];

        if (file) {
            var reader = new FileReader();
            reader.onload = function (e) {
                var pdfData = new Uint8Array(e.target.result);
                displayPDF(pdfData);
            }
            reader.readAsArrayBuffer(file);
        }
    }

    function displayPDF(pdfData) {
        var container = document.getElementById('dwtInitialViewer');
        container.replaceChildren();

        pdfjsLib.getDocument({ data: pdfData }).promise.then(function (pdfDoc) {
            for (var pageNumber = 1; pageNumber <= pdfDoc.numPages; pageNumber++) {
                pdfDoc.getPage(pageNumber).then(function (page) {
                    var canvas = document.createElement('canvas');
                    container.appendChild(canvas);
                    var viewport = page.getViewport({ scale: 1 });
                    canvas.height = viewport.height;
                    canvas.width = viewport.width;

                    page.render({
                        canvasContext: canvas.getContext('2d'),
                        viewport: viewport
                    });
                });
            }
            $('#viewerInitialModal').modal('show');

        });
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

