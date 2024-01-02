app.controller('VersionSearchCtr', ['$scope', '$http', 'ImageProcessServices', 'environmentServices', '$compile', '$injector', '$interval', function ($scope, $http, ImageProcessServices, environmentServices, $compile, $injector, $interval) {

    'use strict'
    $scope.Zoom_Count = .5;
    $scope.PDF_Images;
    applySecurity();
    $scope.ShowDeleteConfirmModal = function (row) {
        $scope.DocumentIDForDelete = row.DocVersionID;
        $('#ConfirmDelete').modal('show');
    };

    $scope.DeleteDocument = function () {
        $scope.loading = true;
        $http.post('/DocScanningModule/OriginalDocSearching/DeleteDocument',
            {
                _DocumentID: $scope.DocumentIDForDelete,
                _DocumentType: "Version"
            })
            .success(function (response) {
                $('#ConfirmDelete').modal('hide');
                $scope.WildSearch();
                $scope.loading = false;
                toastr.success("Delete Successful");
            }).error(function () {
                $scope.loading = false;
                toastr.error("Delete Failed");
            });
    };






    //Dynamsoft.WebTwainEnv.RegisterEvent('OnWebTwainReady', Dynamsoft_OnReady);

    var DWObject;
    var DWObjectLargeViewer;
    var DWObjectQuickViewer;
    var zoomFactor = .5;

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
        console.log(tableRow);
        //DWObjectQuickViewer.IfShowFileDialog = false;
        //DWObjectQuickViewer.RemoveAllImages();

        //DWObjectQuickViewer.FTPPort = tableRow.ServerPort;
        //DWObjectQuickViewer.FTPUserName = tableRow.FtpUserName;
        //DWObjectQuickViewer.FTPPassword = tableRow.FtpPassword;

        //alert(tableRow.ServerPort + "," + tableRow.ServerIP);
        //alert(tableRow.FileServerURL + "//" + tableRow.DocVersionID + "_v_" + tableRow.VersionNo + ".pdf");

        //DWObjectQuickViewer.FTPDownload(tableRow.ServerIP, (tableRow.FileServerURL + "//" + tableRow.DocVersionID + "_v_" + tableRow.VersionNo + ".pdf"));
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
    $scope.DetailPrint = function () {
        var fileURL = URL.createObjectURL($scope.PDF_Images);
        window.open(fileURL, '_blank');
    };

    $scope.ShowDetailView = function (tableRow, e) {
        //DWObject.IfShowFileDialog = false;
        
        //DWObject.FTPDownload(tableRow.ServerIP, (tableRow.FileServerURL + "//" + tableRow.DocVersionID + "_v_" + tableRow.VersionNo + ".pdf"));
        $('#DetailViewModal').modal('show');

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
            + "&" + encodeURIComponent("isObsolete") + "=" + encodeURIComponent(false)
            + "&" + encodeURIComponent("isSecured") + "=" + encodeURIComponent(false)

        ImageProcessServices.detailView(url).then(function (data) {
            debugger
            var arrayBuffer = data.data;
          
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

    $scope.Print = function () {
        DWObjectQuickViewer.Print();
    };

    $scope.docPropIdentityModel = {
        OwnerLevel: { OwnerLevelID: "", LevelName: "" },
        Owner: { OwnerID: "", OwnerName: "" },
        DocCategory: { DocCategoryID: "", DocCategoryName: "" },
        DocType: { DocTypeID: "", DocTypeName: "" },
        DocProperty: { DocPropertyID: "", DocPropertyName: "" },
        SearchBy: ""
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
                        $scope.ddOwner();
                        $scope.loading = false;
                    }).error(function () {
                        $scope.loading = false;
                    });
            }
        }
    });

    $scope.ddOwnerLevel = function () {
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
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });
    };

    $scope.ddOwner = function () {
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
    };

    $scope.ddDocCategory = function () {
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

    $scope.ddDocType = function () {
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

            }).error(function () {
                $scope.loading = false;
            });
    };

    $scope.ddDocProperty = function () {
        $scope.docPropIdentityModel.SearchBy = "";
        $scope.docPropIdentityGridData = [];
    };

    $scope.GrdSearchBy = function () {
        $scope.BindDataToGrid();
    }

    $scope.BindDataToGrid = function () {
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
                search: $scope.pagingInfo.search,
            })
            .success(function (pageable) {
                $scope.GridDisplayCollection = pageable.result;
                $scope.pagingInfo.totalItems = pageable.totalPages;
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });

    };

    $scope.WildSearch = function () {
        $http.post('/DocScanningModule/VersionDocSearching/GetDocumentsByWildSearch',
            {
                _SearchFor: $scope.docPropIdentityModel.SearchFor,
                page: $scope.pagingInfo.page,
                itemsPerPage: $scope.pagingInfo.itemsPerPage,
                sortBy: $scope.pagingInfo.itemsPerPage,
                reverse: $scope.pagingInfo.reverse,
                search: $scope.pagingInfo.search,
            })
            .success(function (pageable) {
                $scope.GridDisplayCollection = pageable.content;
                $scope.pagingInfo.totalItems = pageable.totalPages;
                $scope.loading = false;
            }).error(function () {
                $scope.loading = false;
            });

    };

    $scope.ResetModel = function () {

    };

    $scope.toggleEdit = function (tableRow) {

        $http.post('/DocScanningModule/VersionDocSearching/GetDocPropIdentityForSpecificDocType',
            {
                _DocCategoryID: tableRow.DocCategoryID,
                _OwnerID: tableRow.OwnerID,
                _DocTypeID: tableRow.DocTypeID,
                _DocPropertyID: tableRow.DocPropertyID,
                _DocVersionID: tableRow.DocVersionID,
                _DocDistributionID: tableRow.DocDistributionID,
                _SearchBy: "1"
            })
            .success(function (response) {
                $scope.DocumentsAttributeList = response;

                var number = response.length;
                setTimeout(function () {
                    for (var i = 0; i < number; i++) {
                        $("#" + response[i].DocMetaID).val(response[i].MetaValue);
                    };
                }, 0);


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

    $scope.Save = function () {
        $scope.loading = true;

        angular.forEach($scope.DocumentsAttributeList, function (item) {

            DocMetaValues.DocMetaID = item.DocMetaID;
            DocMetaValues.MetaValue = $("#" + item.DocMetaID).val();

            FinalObject.DocMetaValues.push(DocMetaValues);

            DocMetaValues = {
                MetaValue: "",
                DocMetaID: ""
            };

        });

        $.ajax({
            url: '/DocScanningModule/VersionDocSearching/UpdateDocMetaInfo',
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
        $scope.loading = true;
        //$scope.BindDataToGrid();
        $scope.loading = false;
    };


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
            pdf_container.replaceChildren();
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

    $scope.page_backup = 0;
    $scope.View_thumb_onChange = function (page_num) {
        if ($scope.page_backup != page_num) {
            var pdf_container = document.getElementById("dwtLargeViewer");
            $scope.Large_Viewer_RenderPage(pdf_container, page_num);
            $scope.page_backup = page_num;
        }

    }

    $scope.Large_Viewer_RenderPage = function (pdf_container, page_num) {
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

}]);

