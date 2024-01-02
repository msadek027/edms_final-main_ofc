app.service("ImageProcessServices", function ($http) {

    this.RegisterUser = function (username,email,password) {
        return $http.post(this.BaseRoute + '/user/register/', {
            "username": username,
            "email": email,
            "password": password
        });
    }
    this.LoginUser = function (username, password) {
        return $http.post(this.BaseRoute + '/user/login/', {
            "username": username,
            "password": password
        });
    }
    this.FileMerge = function (files,token) {
        var formData = new FormData();
        for (var i = 0; i < files.length; i++) {
            formData.append('files', files[i]);
        }
        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'content-type': undefined

            },
            transformRequest: angular.identity
            
        };
        return $http.post('http://202.59.140.136:8000' + '/api/merge/', formData, config);
    }
    //New Api
    this.imageToPdf = function (file, token) {

        var formData = new FormData();
        formData.append('images', file);

        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*',
                'content-type': undefined
            },

            // responseType: "arrayBuffer"
            transformRequest: angular.identity
        };

        return $http.post('http://202.59.140.136:8000' + '/api/images_to_pdf/', formData, config);
    }
    this.pdfSplit = function (file, token) {
        var formData = new FormData();
        formData.append('file', file);

        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,            
            },
           
        };

        return $http.post('http://202.59.140.136:8000' + '/api/split_pdf/', formData, config);
    }
    this.TestBox = function (file,page_num, text, endx,endy, font_scale, colorR,colorG,colorB, thickness, token) {

        var formData = new FormData();
        formData.append('file', file);
        formData.append('page_num', page_num);
        formData.append('text', text);
        formData.append('origin', '[' + parseInt(endx) + ',' + parseInt(endy) + ']');
        formData.append('font_scale', font_scale);
        formData.append('color', '[' + parseInt(colorR) + ',' + parseInt(colorG) + ',' + parseInt(colorB) + ']');
        formData.append('thickness', thickness);

        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*',
                'content-type': undefined

            },
            responseType: "arrayBuffer"
        };

        return $http.post('http://202.59.140.136:8000' + '/api/text/', formData, config);
    }

    this.crop = function (file, page_num, startx, starty, endx, endy, token) {
        //var Vendx = endx * 4;
        //var Vendy = endy * 3;
        var Vendx = endx ;
        var Vendy = endy ;

        var formData = new FormData();
        formData.append('file', file);
        formData.append('page_num', page_num);
        formData.append('crop_dimensions', '[' + parseInt(startx) + ',' + parseInt(starty) + ',' + parseInt(Vendx) + ',' + parseInt(Vendy) + ']');

        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*',
                'content-type': undefined

            },
            responseType: "arrayBuffer"
        };

        return $http.post('http://202.59.140.136:8000' + '/api/crop/', formData, config);

       
    }
    this.mask = function (file, page_num, startx, starty, endx, endy, token) {
        debugger;
        var Vendx = endx;
        var Vendy = endy;
        //var Vendx = endx * 3.5;
        //var Vendy = endy * 4;

        var formData = new FormData();
        formData.append('file', file);
        formData.append('page_num', page_num);
        formData.append('mask_dimensions', '[' + parseInt(startx) + ',' + parseInt(starty) + ',' + parseInt(Vendx) + ',' + parseInt(Vendy) + ']');

        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*',
                'content-type': undefined

            },
            responseType: "arrayBuffer"
        };

        return $http.post('http://202.59.140.136:8000' + '/api/mask/', formData, config);

    }
    this.highlight = function (file, page_num, startx, starty, endx, endy, token) {
        debugger;

        //var Vendx = endx*4.8;
        //var Vendy = endy * 4;

        var Vendx = endx;
        var Vendy = endy;
        var formData = new FormData();
        formData.append('file', file);
        formData.append('page_num', page_num);
        formData.append('highlight_dimensions', '[' + parseInt(startx) + ',' + parseInt(starty) + ',' + parseInt(Vendx) + ',' + parseInt(Vendy) + ']');

        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*',
                'content-type': undefined

            },
            responseType: "arrayBuffer"
        };

        return $http.post('http://202.59.140.136:8000' + '/api/highlight/', formData, config);

    }
    this.addImage = function (file, image_file, page_num, startx, starty, endx, endy, token) {
        debugger;

        //var Vendx = endx * 4;
        //var Vendy = endy * 3;
        var Vendx = endx ;
        var Vendy = endy ;

        var formData = new FormData();
        formData.append('file', file);
        formData.append('image_file', image_file);
        formData.append('page_num', page_num);
        formData.append('image_dimensions', '[' + parseInt(startx) + ',' + parseInt(starty) + ',' + parseInt(Vendx) + ',' + parseInt(Vendy) + ']'); // '[0,0,500,500]');

        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*',
                'content-type': undefined

            },
            responseType: "arrayBuffer"
        };

        return $http.post('http://202.59.140.136:8000' + '/api/addimage/', formData, config);

    }


    this.blankpage = function (file, page_num, token) {
        var formData = new FormData();
        formData.append('file', file);
        formData.append('page_num', page_num);
        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*',
                'content-type': undefined

            },
            responseType: 'arraybuffer'
        };
        return $http.post('http://202.59.140.136:8000' + '/api/blankpage/', formData, config);
    }
    this.removepage = function (file, page_num,token) {
        var formData = new FormData();
        formData.append('file', file);
        formData.append('page_num', page_num);
        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*',
                'content-type': undefined

            },
            responseType: "arrayBuffer"
        };
        return $http.post('http://202.59.140.136:8000' + '/api/removepage/', formData, config);
       
    }
    this.resize = function (file, page_num, height, width,token) {
        var formData = new FormData();
        formData.append('file', file);
        formData.append('height', height);
        formData.append('width', width);
        formData.append('page_num', page_num);
        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*',
                'content-type': undefined

            },
            responseType: "arrayBuffer"
        };

        return $http.post('http://202.59.140.136:8000' + '/api/resize/', formData, config);

      
    }
    this.watermark = function (file, watermark, watermark_width, watermark_height, token) {

        debugger;
        var formData = new FormData();
        formData.append('file', file);
        formData.append('watermark', watermark);
        formData.append('watermark_width', watermark_width);
        formData.append('watermark_height', watermark_height);

        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*',
                'content-type': undefined

            },
            responseType: "arrayBuffer"
        };

        return $http.post('http://202.59.140.136:8000' + '/api/watermark/', formData, config);
       
    }
    this.movepage = function (file, from_page_num, to_page_num,token) {
        
        var formData = new FormData();
        formData.append('file', file);
        formData.append('from_page_num', from_page_num);
        formData.append('to_page_num', to_page_num);
        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*',
                'content-type': undefined

            },
            responseType: "arrayBuffer"
        };

        return $http.post('http://202.59.140.136:8000' + '/api/movepage/', formData, config);
    }
 
    this.ocr = function (file, token) {
   
        var formData = new FormData();
        formData.append('file', file);   

        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*',
                'content-type': undefined

            },
            responseType: "arrayBuffer"
        };

        return $http.post('http://202.59.140.136:8000' + '/api/ocr/', formData, config);



        //return $http.post(this.BaseRoute + '/api/ocr/', {
        //    "file": file
        //});
    }
    this.refresh = function (refresh) {
        return $http.post(this.BaseRoute + '/user/refresh/', {
            "refresh": refresh
        });
    }
    this.rotate = function (file, page_num, angle, token) {
        var formData = new FormData();
        formData.append('file', file);
        formData.append('page_num', page_num);
        formData.append('angle', angle);
        var config = {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Accept': '*/*',
                'content-type': undefined

            },
            responseType: "arrayBuffer"
        };

        return $http.post('http://202.59.140.136:8000' + '/api/rotate/', formData, config);
    }

    this.detailView = function (url) {      
        var config = {
            headers: {
                'content-type': undefined
            },
            responseType: 'arraybuffer'
        };
        return $http.get('/' + url, config);
    }

    this.DocUpload = function (server, port, name, password, path, file_name, file) {
        var formData = new FormData();
        formData.append('file', file);
        formData.append('server', server);
        formData.append('port', port);
        formData.append('name', name);
        formData.append('password', password);
        formData.append('path', path);
        formData.append('file_name', file_name);
        var config = {
            headers: {
                'content-type': undefined
            }
        };

        return $http.post('/Home/DocUpload/', formData, config);
    }

//Write by me
    // new Pdf Upload and render 
   this.pdfToArrayBuffer = function (file, callback) {
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

    this.Remove_Page_From_Pdf = async function (file, page_num, cookie) {
        const formData = new FormData();
        formData.append('file', file);
        formData.append('page_num', page_num);

        try {
            const response = await fetch('http://202.59.140.136:8000/api/removepage/', {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${cookie}` },
                body: formData,
            });

            if (response.ok) {
                const blob = await response.blob();
                const vBlob = new Blob([blob], { type: 'application/pdf' });
                const vvBlob = new Blob([blob], { type: 'application/octet-stream' });
                const pdfData = await new Promise((resolve, reject) => {
                    this.pdfToArrayBuffer(vvBlob, function (pdfData) {
                        resolve(pdfData);
                    });
                });
                console.log('PDF data as ArrayBuffer:', pdfData);
                return pdfData; // Return the ArrayBuffer
            } else {
                // Handle errors here
                throw new Error('Request failed');
            }
        } catch (error) {
            $scope.loading = false;
            // Handle any errors that occurred during the fetch
            console.error('Error:', error);
            return null; // Handle error case by returning null or some other value
        }
    }

    this.Rotate_Page_From_Pdf = async function (file, page_num, angle, cookie) {
        const formData = new FormData();
        formData.append('file', file);
        formData.append('page_num', page_num);
        formData.append('angle', angle);

        try {
            const response = await fetch('http://202.59.140.136:8000/api/rotate/', {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${cookie}` },
                body: formData,
            });

            if (response.ok) {
                const blob = await response.blob();
                const vBlob = new Blob([blob], { type: 'application/pdf' });
                const vvBlob = new Blob([blob], { type: 'application/octet-stream' });
                const pdfData = await new Promise((resolve, reject) => {
                    this.pdfToArrayBuffer(vvBlob, function (pdfData) {
                        resolve(pdfData);
                    });
                });
                console.log('PDF data as ArrayBuffer:', pdfData);
                return pdfData; // Return the ArrayBuffer
            } else {
                // Handle errors here
                throw new Error('Request failed');
            }
        } catch (error) {
            $scope.loading = false;
            // Handle any errors that occurred during the fetch
            console.error('Error:', error);
            return null; // Handle error case by returning null or some other value
        }
    }
    this.Add_Blank_Page_To_Pdf = async function ( file, page_num, cookie) {
        const formData = new FormData();
        formData.append('file', file);
        formData.append('page_num', page_num);


        try {
            const response = await fetch(webUrl, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${cookie}` },
                body: formData,
            });

            if (response.ok) {
                const blob = await response.blob();
                const vBlob = new Blob([blob], { type: 'application/pdf' });
                const vvBlob = new Blob([blob], { type: 'application/octet-stream' });
                const pdfData = await new Promise((resolve, reject) => {
                    this.pdfToArrayBuffer(vvBlob, function (pdfData) {
                        resolve(pdfData);
                    });
                });
                console.log('PDF data as ArrayBuffer:', pdfData);
                return pdfData; // Return the ArrayBuffer
            } else {
                // Handle errors here
                throw new Error('Request failed');
            }
        } catch (error) {
            $scope.loading = false;
            // Handle any errors that occurred during the fetch
            console.error('Error:', error);
            return null; // Handle error case by returning null or some other value
        }
    }

    this.RotatePage_RemovePage_AddBlankPage_Pdf = async function (webUrl, formData, cookie) {
        //const formData = new FormData();
        //formData.append('file', file);
        //formData.append('page_num', page_num);


        try {
            const response = await fetch(webUrl, {
                method: 'POST',
                headers: { 'Authorization': `Bearer ${cookie}` },
                body: formData,
            });

            if (response.ok) {
                const blob = await response.blob();
                const vBlob = new Blob([blob], { type: 'application/pdf' });
                const vvBlob = new Blob([blob], { type: 'application/octet-stream' });
                const pdfData = await new Promise((resolve, reject) => {
                    this.pdfToArrayBuffer(vvBlob, function (pdfData) {
                        resolve(pdfData);
                    });
                });
                console.log('PDF data as ArrayBuffer:', pdfData);
                return pdfData; // Return the ArrayBuffer
            } else {
                // Handle errors here
                throw new Error('Request failed');
            }
        } catch (error) {
            $scope.loading = false;
            // Handle any errors that occurred during the fetch
            console.error('Error:', error);
            return null; // Handle error case by returning null or some other value
        }
    }
});