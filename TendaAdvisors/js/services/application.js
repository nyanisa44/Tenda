
(function () {
    var service;

    tendaApp.factory('applicationService', ['$http', '$q', function ($http, $q) {
        function formTransform(obj) {
            //Seems to be null a lot
            if (obj === undefined) return;
            var str = [];
            for (var p in obj) str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
            return str.join("&");
        }

        if (!service) {
            service = {
                GetApplications: function GetApplications() {
                    return $q(function (resolve, reject) {
                        $http({
                            method: 'GET',
                            url: 'api/Applications/',
                            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                            transformRequest: formTransform
                        }).then(function success(response) {

                            if (!response.data) {
                                reject("No applications were returned. Please try again later");
                                return;
                            }

                            resolve(response.data);
                        }, function failure(response) {
                            if (response.data) {
                                if (response.data.error === "invalid_client") {
                                    reject("Invalid credentials were supplied");
                                    return;
                                }
                            }
                            reject("There was an unknown error during the get application process. Please try again later");
                        });
                    });
                },

                GetCompleteApplications: function GetCompleteApplications() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Applications/complete/').then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                GetAllApplicationsIds: function GetAllApplicationsIds() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Applications/AllApplications/').then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                GetCompleteApplicationsForAdvisor: function GetCompleteApplicationsForAdvisor(AdviId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Applications/completeAdvisor/'+AdviId).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                GetApplicationsbyClientId: function GetApplicationsbyClientId(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Applications/clientId/'+id).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                GetPendingApplications: function GetPendingApplications() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Applications/pending/').then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                GetApplicationAdvisorHistory: function GetApplicationAdvisorHistory(applicationId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Application/GetApplicationHistory/'+applicationId).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                getApplicationAdvisorEditHistory: function getApplicationAdvisorEditHistory(applicationId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Application/GetApplicationAdvisorEditHistory/' + applicationId).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                postApplicationAdvisorEditHistory: function PostApplicationAvisorEditHistory(applicationId) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Application/PostApplicationAvisorEditHistory/' + applicationId).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                GetPendingApplicationsForAdvisor: function GetPendingApplicationsForAdvisor(AdviId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Applications/PendingAdvisor/' + AdviId).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                GetNewApplications: function GetNewApplications() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Applications/new').then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                GetNewApplicationsForAdvisor: function GetNewApplicationsForAdvisor(AdviId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Applications/NewAdvisor/' + AdviId).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                GetBasicSupplier: function GetBasicSupplier(name) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Supplier/Basic/' + name).then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },

                GetAllProducts: function GetAllProducts()
                {
                    return $q(function (resolve, reject) {
                        $http.get('api/Product/all/').then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },

                GetBasicProduct: function GetBasicProduct(name,id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Product/Basic/' + name + '/' + id).then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },

                GetApplication: function GetApplication(id) {
                    return $q(function (resolve, reject) {
                        $http({
                            method: 'GET',
                            url: 'api/applications/' + id,
                            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                            transformRequest: formTransform
                        }).then(function success(response) {

                            if (!response.data) {
                                reject("No applications were returned. Please try again later");
                                return;
                            }

                            resolve(response.data);
                        }, function failure(response) {
                            if (response.data) {
                                if (response.data.error === "invalid_client") {
                                    reject("Invalid credentials were supplied");
                                    return;
                                }
                            }
                            reject("There was an unknown error during the get application process. Please try again later");
                        });
                    });
                },

                //Adds a new application into the DB:
                PostApplication: function PostApplication(application) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Applications/', application).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                PutchangeIsExpired: function PutchangeIsExpired(appDocId, status) {
                    return $q(function (resolve, reject) {
                        $http.put('api/ApplicationDocuments/' + appDocId + '/' + status).then(
                            function (a) {
                                resolve(a);
                            }, function (a) {
                                reject(a);
                            });
                    });
                },
                
                 PutApplicationStatus: function PutApplicationStatus(applicationId, applicationStatus) {
                    return $q(function (resolve, reject) {
                        $http.put('api/Application/PutApplicationStatus/' + applicationId + '/' + applicationStatus).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                 },
            
                 GetApplicationDocuments: function GetApplicationDocuments() {
                        return $q(function (resolve, reject) {
                            $http.get('api/ApplicationDocuments').then(function (a) {
                                resolve(a);
                            }, function (a) {
                                reject(a);
                            });
                        });
                 },

                 GetApplicationDocumentsByApplicationId: function GetApplicationDocumentsByApplicationId(applicationId) {
                     return $q(function (resolve, reject) {
                        $http.get('api/Applications/ApplicationDocumentsByApplicationId/' + applicationId + '/').then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                     });
                 },

                 DeleteApplication: function DeleteApplication(id) {
                     return $q(function (resolve, reject) {
                         $http.delete('api/Application/' + id).then(function (dl) {
                             resolve(dl);
                         }, function (dl) {
                             reject(dl);
                         });
                     });
                 },

                GetApplicationStatuses: function GetApplicationStatuses() {
                    return $q(function (resolve, reject) {
                        $http.get('api/ApplicationStatus').then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                GetApplicationTypes: function GetApplicationTypes() {
                    return $q(function (resolve, reject) {
                        $http.get('api/ApplicationTypes').then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                //id is an application id, and application is an application:
                PutApplication: function PutApplication(id, application) {
                    return $q(function (resolve, reject) {
                        $http.put('api/Applications/putApplication/' + id +'/', application).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
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

                GetSuppliersByApplication: function GetSuppliersByApplication(applicationId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Suppliers/' + applicationId + '/Application/').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                //Parameter is the supplier's id:
                GetSupplierProducts: function GetSupplierProducts(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Suppliers/' + id + '/Products').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                //Parameter is the ApplicationId:
                GetQuerysByApplication: function GetQuerysByApplicatison(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Queries/Application/' + id).then(function (qs) {
                            resolve(qs);
                        }, function (qs) {
                            reject(qs);
                        });
                    });
                },

                GetQueryType: function GetQueryType() {
                    return $q(function (resolve, reject) {
                        $http.get('api/QueryTypes/').then(function (qt) {
                            resolve(qt);
                        }, function (qt) {
                            reject(qt);
                        });
                    });
                },

                ///////////////////////////////////////////////////////////////////////////////////////////
                //Queries Post
                PostQuery: function PostQuery(query) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Queries',query).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },
                ///////////////////////////////////////////////////////////////////////////////////////////

                //Delete Query
                    DeleteQuery: function DeleteQuery(id) {
                        return $q(function (resolve, reject) {
                            $http.delete('api/Queries/' + id).then(function (dl) {
                                resolve(dl);
                            }, function (dl) {
                                reject(dl);
                            });
                        });
                    },

                ///////////////////////////////////////////////////////////////////////////////////////////////
                //Receives an application Id and an application then PUTs the application, then the products get saved:
                PutApplicationProducts: function PutApplicationProducts(id, application) {
                    return $q(function (resolve, reject) {
                        $http.put('api/Applications/' + id + '/Products', application).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                // Asserts whether or not the today's date falls within the two dates.
                AssertIfExpired: function AssertIfExpired(startDate, endDate) {
                    var today = new Date();
                    if (startDate <= today && today <= endDate) {
                        return false;
                    }
                    if (today <= startDate && today <= endDate) {
                        return false;
                    }
                    return true;
                },
                
                // Asserts whether or not the today's date falls before the given date.
                AssertIfExpiredForSingleDay: function AssertIfExpiredForSingleDay(endDate) {
                    var today = new Date();
                    if (today > endDate) {
                        return false;
                    }
                    return true;
                },

                //Parameter is the ApplicationId:
                GetApplicationDocumentFile: function GetApplicationDocumentFile(id) {
                    return $q(function (resolve, reject) {
                        $http({
                            url: 'api/ApplicationDocuments/' + id + "/File",
                            method: 'GET',
                            headers: {
                                'Content-type': 'application/download'
                            },
                            responseType: 'arraybuffer'
                        }).success(function (data, status, headers, config) {
                            var file = new Blob([data], {
                                type: 'application/download'
                            });
                            var fileURL = URL.createObjectURL(file);
                            var a = document.createElement('a');
                            a.href = fileURL;
                            a.target = '_blank';
                            a.download = headers('content-disposition');
                            a.download = ((/filename[^;=\n]*=((['\"]).*?\2|[^;\n]*)/i).exec(a.download)[1]).replace(/["]/g, '');
                            document.body.appendChild(a);
                            a.click();
                        }).error(function (data, status, headers, config) {
                            console.log("We haz a prob: " + data);
                        });
                    });
                },
            
                //Use the application ID  and the product ID to upload a json document:
                PostApplicationDocument: function PostApplicationDocument(applicationId,productId, documentId, document, submitted) {
                    return $q(function (resolve, reject) {
                        var formData = new FormData();
                        formData.append('file', document.file);
                        $http.post('api/ApplicationDocuments/' + applicationId + '/ProductId/' + productId + '/DocumentTypeId/' + documentId + '/' + submitted, document).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                //Use the applicationId  and applicationDocument to update an existing document and link it to applicationId:
                PutApplicationDocument: function PutApplicationDocument(applicationId, document) {
                    return $q(function (resolve, reject) {
                        console.log(applicationId, document);
                        if (!document.application) {
                            document.application = { "Id": applicationId };
                        }
                        $http.put('api/applicationDocuments/' + applicationId, document).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetUsersBySAId: function GetUsersBySAId(saId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Contacts/SAId/'+saId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetDashboard: function GetDashboard() {
                    return $q(function (resolve, reject) {
                        $http({
                            method: 'GET',
                            url: 'api/Dashboard'
                        }).then(function success(response) {
                            if (!response.data) {
                                reject("No applications were returned. Please try again later");
                                return;
                            }

                            resolve(response.data);
                        }, function failure(response) {
                            if (response.data) {
                                if (response.data.error === "invalid_client") {
                                    reject("Invalid credentials were supplied");
                                    return;
                                }
                            }
                            reject("There was an unknown error during the get application process. Please try again later");
                        });
                    });
                },

                deleteApplicationDocuments: function deleteApplicationDocuments(id, AppId, DocType) {
                    return $q(function (resolve, reject) {
                        $http.delete('api/ApplicationDocuments/DeleteDupAppocument/' + id + '/' + AppId + '/' + DocType).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                checkClientDuplicateSupplier: function CheckClientDuplicateSupplier(clientId, supplierId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Application/CheckClientDuplicateSupplier/' + clientId + '/' + supplierId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                }
            };
        }

        return service;
    }]);

    tendaApp.config(['$httpProvider', function ($httpProvider) {
        $httpProvider.interceptors.push(['$q', '$injector', function ($q, $injector) {
            return {
                'request': function (config) {
                    var applicationService = $injector.get('applicationService');
                    return config;
                },
                'responseError': function (rejection) {
                    return $q.reject(rejection);
                }
            };
        }]);
    }]);
})();