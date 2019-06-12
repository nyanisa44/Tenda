
(function () {
    var service;

    tendaApp.factory('adminService', ['$http', '$q', function ($http, $q) {
        function formTransform(obj) {
            var str = [];
            for (var p in obj) str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
            return str.join("&");
        }

        if (!service) {
            service = {

                GetImportTypes: function GetImportTypes() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Admin/ImportTypes/').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetImportCompanies: function GetImportCompanies() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Companies/').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetImportFileListByImportTypeId: function GetImportFileListByImportTypeId(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Admin/Process/' + id, id).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                MemberListExceptionFile: function MemberListExceptionFile() {
                    return $q(function (resolve, reject) {
                        var req = {
                            method: 'POST',
                            url: 'api/Admin/MemberListExceptionFile/',
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded'
                            },
                            responseType: 'arraybuffer'
                        };
                         $http(req).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                MemberListExceptionTableTruncation: function MemberListExceptionTableTruncation() {
                    return $q(function (resolve, reject) {
                        var req = {
                            method: 'POST',
                            url: 'api/Admin/MemberListExceptionTableTruncation/',
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded'
                            },
                            responseType: 'arraybuffer'
                        };
                        $http(req).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                CommissionExceptionTableTruncation: function CommissionExceptionTableTruncation() {
                    return $q(function (resolve, reject) {
                        var req = {
                            method: 'POST',
                            url: 'api/Admin/CommissionExceptionTableTruncation/',
                            headers: {
                                'Content-Type': 'application/x-www-form-urlencoded'
                            },
                            responseType: 'arraybuffer'
                        };
                        $http(req).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                ApproveImport: function ApproveImport(id, SupplierId, FromDate, ToDate) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Admin/approveImport/' + id + '/' + SupplierId+ '/'+ FromDate + '/' + ToDate).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                refreshUnmatched: function refreshUnmatched() {
                    return $q(function (resolve, reject) {
                        $http.post('api/Admin/refreshUnmatched/').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                PostFileImport: function PostFileImport(data) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Admin/ImportFile', data).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },
                // TODO: Here is where will call routing of admin pages 

                UpdateFileOwner: function UpdateFileOwner(importFileId, advisorId) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Admin/UpdateFileOwner/'+ importFileId + '/' + advisorId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                //Action is specified by url passed to method
                PutProcessAction: function PutProcessAction(importFileId, selectedProductID) {
                    return $q(function (resolve, reject) {
                        $http.put('api/Admin/Process/Commission/' + importFileId + '/' + selectedProductID)
                            .then(function (d) {
                                resolve(d);
                            }, function (d) {
                                reject(d);
                            });
                    });
                },

                //Update fieldMap on an importfile.
                PutFieldMap: function PutFieldMap(importFileId, fieldMap) {
                    console.log(fieldMap);
                    return $q(function (resolve, reject) {
                        $http.put('api/Admin/Save/FieldMap/' + importFileId, fieldMap).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetSuppliers: function GetSuppliers() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Suppliers/').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetLicenseCategories: function GetLicenseCategories() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Licenses/LicenseCategories').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetLicenseTypes: function GetLicenseTypes(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Licenses/LicenseTypes/'+id).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                CheckComissionRun: function CheckComissionRun(SupplierId,datefrom , dateto) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Admin/CheckComissionRun/' + SupplierId+'/'+datefrom+'/'+dateto).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorLicenseTypes: function GetAdvisorLicenseTypes(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisor/AdvisorLicenseTypes/' + id).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorLicensedProducts: function GetAdvisorLicenseProducts(id, licId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisor/AdvisorLicensedProducts/' + id + '/' + licId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                AddProductToSupplier: function AddProductToSupplier(product, supplierId) {
                    product.supplier = { "Id": supplierId };
                    return $q(function (resolve, reject) {
                        $http.post('api/Products/', product).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                SaveSupplier: function SaveSupplier(supplier) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Suppliers/', supplier).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                getUsers: function GetUsers() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Admin/GetUsers').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                UpdateUser: function UpdateUser(user) {
                return $q(function (resolve, reject) {
                    $http.post('api/Admin/AddOrUpdateUser', user).then(function (d) {
                        resolve(d);
                    }, function (d) {
                        reject(d);
                    });
                });
                },
               
                GetSystemSettings: function GetSystemSettings() {
                    return $q(function (resolve, reject) {
                        $http.get('api/SystemSettings').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                PutSystemSettings: function PutSystemSettings(ID,SystemSetting) {
                    return $q(function (resolve, reject) {
                        $http.put('api/SystemSettings/' + ID , SystemSetting).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                }
            }; //end service{}
        }

        return service;
    }]);


    tendaApp.config(['$httpProvider', function ($httpProvider) {

        $httpProvider.interceptors.push(['$q', '$injector', function ($q, $injector) {
            return {
                'request': function (config) {

                    var adminService = $injector.get('adminService');
                    return config;
                },

                'responseError': function (rejection) {
                    return $q.reject(rejection);
                }
            };
        }]); //end httpProvider.interceptors.
    }]);// end .config
})();
