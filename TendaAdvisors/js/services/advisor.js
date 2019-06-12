
(function () {
    var service;

    tendaApp.factory('advisorService', ['$http', '$q', '$localStorage', function ($http, $q, $localStorage) {
        function formTransform(obj) {
            var str = [];
            for (var p in obj) str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
            return str.join("&");
        }

        if (!service) {
            service = {
                
                FetchProductsByLicenses: function FetchProductsByLicenses(licenses,id) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Products/LicenseTypes/' + id, licenses).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                FetchProductsByLicenseId: function FetchProductsByLicenseId(licenseId, id) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Products/LicenseTypesId/' + id + '/licenseId/'+ licenseId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                PostFetchSuppliersByLicenses: function PostFetchSuppliersByLicenses(licenses) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Suppliers/LicenseTypes/', licenses).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorCompanyId: function GetAdvisorCompanyId(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/AdviserType/Id/' +id).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAllApplications: function GetAllApplications() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Applications/AllApplications/').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorOfAdvisorTypeAdvisor: function GetAdvisorOfAdvisorTypeAdvisor(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/AdvisorType/Advisor').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorsNotAdvisorTypeComapanyDisplayIdNumber: function GetAdvisorsNotAdvisorTypeComapanyDisplayIdNumber() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/NotAdvisorTypeComapany/idNumber').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                //Advisor company
                GetAdvisorsCompany: function GetAdvisorsCompany() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/Company').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorIdOfAdvisorType: function GetAdvisorIdOfAdvisorType(contactId, advisorTypeId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/Id/'+contactId+'/AdvisorTypeID/'+advisorTypeId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                //Not Company
                GetAdvisorsNotCompany: function GetAdvisorsNotCompany() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/NotCompany').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorProfile: function GetAdvisorProfile(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/AdvisorProfile/'+ id).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },
                //Advisor  Admin
                GetAdvisorsAdmin: function GetAdvisorsAdmin() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/Admin').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                //Advisor Advisor
                GetAdvisorsAdvisor: function GetAdvisorsAdvisor() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/Advisor').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorByApplicationId: function GetAdvisorByApplicationId(id){
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisor/AdvisorByApplicationId/' + id).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                //Advisor Advisor
                GetSimilarAdvisor: function GetSimilarAdvisor(nameSearch) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/similar/' + nameSearch).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                //Advisor Advisor
                GetAdvisorsAdvisorByString: function GetAdvisorsAdvisorByString(nameSearch) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/advisor/string/' + nameSearch).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                //Advisor Id
                GetAdvisorsAdvisorID: function GetAdvisorsAdvisorID() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/AdvisorId').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                //Get all advisors of not company
                GetAdvisorsNotAdvisorTypeComapany: function GetAdvisorsNotAdvisorTypeComapany() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/NotAdvisorTypeComapany').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                //Key Id
                GetAdvisorsKeyId: function GetAdvisorsKeyId() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/KeyId').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },
                

                GetAdvisorsDirector: function GetAdvisorsDirector() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/Director').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorsKey: function GetAdvisorsKey() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/Key').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorId: function GetAdvisorId(contactId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/Id/' + contactId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorIdTypeAdvisor: function GetAdvisorIdTypeAdvisor(contactId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/Id/AdisorTypeAdvisor/' + contactId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },
                    
                GetAdvisorLicenses: function GetAdvisorLicenses(advisorId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/Licenses/' + advisorId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorsAdvisorId: function GetAdvisorsAdvisorId(advisorId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/Advisor/' + advisorId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },
                
                PutAdvisorStatus: function PutAdvisorStatus(advisorId, advisorStatus) {
                        return $q(function (resolve, reject) {
                            $http.put('api/Advisor/PutAdvisorStatus/' + advisorId + '/' + advisorStatus).then(function (d) {
                                resolve(d);
                            }, function (d) {
                                reject(d);
                            });
                        });
                    },
            
                PutAdvisorProfile: function PutAdvisorProfile(advisorId, advisor) {
                    return $q(function (resolve, reject) {
                        $http.put('api/AdvisorProfile/' + advisorId ,advisor).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetSuppliers: function GetSuppliers() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Suppliers').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetSuppliersById: function GetSuppliersById(Id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Suppliers/'+Id).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetLicences: function GetLicences() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Licenses/Licences').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                FetchSuppliersByLicenses: function FetchSuppliersByLicenses(licenses) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Suppliers/LicenseTypes/', licenses).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetLicenseTypes: function GetLicenseTypes(advisorTypeId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisor/LicenseTypes/' + advisorTypeId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetLicenseCategories: function GetLicenseCategories(advisorTypeId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisor/LicenseCategories/' + advisorTypeId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetDocumentTypes: function GetDocumentTypes(advisorTypeId) {

                    return $q(function (resolve, reject) {
                        $http.get('api/Advisor/DocumentTypes/' + advisorTypeId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorsAdvisorTypeAdvisor: function GetAdvisorsAdvisorTypeAdvisor() {

                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/Advisor/advisorType/Advisor').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },


                GetAdvisorLicenseTypes: function GetAdvisorLicenseTypes(advisorTypeId) {

                    return $q(function (resolve, reject) {
                        $http.get('api/AdvisorShareUnderSupervisions/AdvisorLicenseTypes/' + advisorTypeId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                //underShare
                GetAdvisorShareUnderSupervision: function GetAdvisorShareUnderSupervision(advisorTypeId) {
                    
                    return $q(function (resolve, reject) {
                        $http.get('api/AdvisorShareUnderSupervisions/AdvisorLicenseTypes/' + advisorTypeId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                DeleteAdvisorDocument: function DeleteAdvisorDocument(advisorId) {
                    return $q(function (resolve, reject) {
                        $http.delete('api/AdvisorDocument/' + advisorId).then(function (dl) {
                            resolve(dl);
                        }, function (dl) {
                            reject(dl);
                        });
                    });
                },

                

                DeleteAdvisor: function DeleteAdvisor(advisorId) {
                        return $q(function (resolve, reject) {
                            $http.delete('api/Advisor/' + advisorId).then(function (dl) {
                                resolve(dl);
                            }, function (dl) {
                                reject(dl);
                            });
                        });
                },

                GetContact: function GetContact(name) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisor/' + name).then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },

                PostApplicationAdvisorHisotry: function PostApplicationAdvisorHisotry(applicationId, NewAdvisor, OldAdvisor, effectiveStartDate, effectiveEndDate) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Advisor/PostApplicationAdvisorHisotry/' + applicationId + '/' + NewAdvisor + '/' + OldAdvisor + '/' + effectiveStartDate + '/' + effectiveEndDate).then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },

                PutApplicationAdvisorHistory: function PutApplicationAdvisorHistory(applicationId, effectiveStartDate, effectiveEndDate, advisorId) {
                    return $q(function (resolve, reject) {
                        $http.put('api/Advisor/PutApplicationAdvisorHistory/' + applicationId + '/' + effectiveStartDate + '/' + effectiveEndDate + '/' + advisorId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAllAdvisors: function GetAllAdvisors() {
                    return $q(function (resolve, reject) {
                        $http.get('Advisors/All').then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },

                GetAdvisors: function GetAdvisors(config) {

                    if (config) {
                        config.UrlParam = [];
                        config.UrlParam.push(config.pageIndex);
                        config.UrlParam.push(config.pageSize);
                    }

                    return $q(function (resolve, reject) {

                        $http({
                            method: 'GET',
                            url: 'api/Advisors/' + (config ? config.UrlParam.join('/') : ''),
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

        //////////////////////////////////////////////////////////////////////////////////////////////////////



               /* GetAdvisors: function GetAdvisors() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors').then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },*/
       /////////////////////////////////////////////////////////////////////////////////////
                GetCompanies: function GetCompanies(nameSearch) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Companies/' + nameSearch).then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },

                GetAdvisorDocumentFile: function GetAdvisorDocumentFile(id) {
                    return $q(function (resolve, reject) {
                        $http({
                            url: 'api/AdvisorDocuments/' + id + "/File",
                            method: 'GET',
                            headers: {
                                'Content-type': 'application/download',
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
                            a.download = ((/filename[^;=\n]*=((['\"]).*?\2|[^;\n]*)/i).exec(a.download)[1]).replace(/["]/g, '');;
                            document.body.appendChild(a);
                            a.click();
                        }).error(function (data, status, headers, config) {
                            console.log(data);
                        });
                    });
                },

                GetContactTypes: function GetContactTypes() {
                    return $q(function (resolve, reject) {
                        $http.get('api/ContactTypes').then(function (ct) {
                            resolve(ct);
                        }, function (ct) {
                            reject(ct);
                        });
                    });
                },
                

               GetAdvisorTypes: function GetAdvisorTypes() {
                    return $q(function (resolve, reject) {
                        $http.get('api/AdvisorTypes').then(function (at) {
                            resolve(at);
                        }, function (at) {
                            reject(at);
                        });
                    });
               },

               GetAdvisorStatuses:function GetAdvisorStatuses() {
                   return $q(function (resolve, reject) {
                       $http.get('api/AdvisorStatus').then(function (at) {
                           resolve(at);
                       }, function (at) {
                           reject(at);
                       });
                   });
               },


               GetAdvisorDetails: function GetAdvisorDetails(id) {
                   return $q(function (resolve, reject) {
                       $http.get('api/Advisor/Details/' + id).then(function (d) {
                           resolve(d);
                       }, function (d) {
                           reject(d);
                       });
                   });
                },

                GetAdvisor: function GetAdvisor(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisor/' + id).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetCurrentAdvisorDocuments: function GetCurrentAdvisorDocuments(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisor/Documents/' + id).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },


                GetAdvisorDocuments: function GetAdvisorDocuments() {
                    return $q(function (resolve, reject) {
                        $http.get('api/AdvisorDocuments/').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },
                GetAdvisorDocumentsForAdvisor: function GetAdvisorDocumentsForAdvisor(AdviId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/AdvisorDocumentsForAdvisor/' + AdviId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetUser: function GetUser(id){
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisor/User/' + id).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },
                ///////////////////////////////////////////////////////////////////////////////////////////
                //id is an profile id, and ????
                PutAdvisor: function PutAdvisor(id, advisor) {
                    return $q(function (resolve, reject) {
                        $http.put('api/Advisor/' + id, advisor).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                PostAdvisorDetails: function PostAdvisorDetails(advisor) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Advisor/Details/', advisor).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },


                PutAdvisorShare: function PutAdvisorShare(advisorId, licenseTypeId, supplierName, productName, share, validCommissionFromDate, validCommissionToDate, underSupervision, advisorSupervision, validSupervisionFromDate, validSupervisionToDate) {
                    return $q(function (resolve, reject) {
                        $http.put('api/Advisor/AdvisorLicenseTypes/Put/' + advisorId + '/' + licenseTypeId + '/' + supplierName + '/' + productName + '/' + share + '/' + validCommissionFromDate + '/' + validCommissionToDate + '/' + underSupervision + '/' + advisorSupervision + '/' + validSupervisionFromDate + '/' + validSupervisionToDate).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a); 
                        });
                    });
                },
              

                PostAdvisorShare: function PostAdvisorShare(comSplit) {
                    return $q(function (resolve, reject) {
                        $http.post('api/AdvisorShareUnderSupervisions/AdvisorLicenseTypes/Post/', comSplit).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },
              

                PostAdvisorShareObject: function PostAdvisorShareObject(comSplit) {
                    return $q(function (resolve, reject) {
                        $http.post('api/AdvisorShareUnderSupervisions/AdvisorLicenseTypes/', comSplit).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                putAdvisorAllowance: function putAdvisorAllowance(advisorId, allowance) {
                    return $q(function (resolve, reject) {
                        $http.put('api/Advisor/AdvisorAllowance/Put/' + advisorId + '/' + allowance).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                },

                //Use the advisorId  and the documentTypeId to upload a json base64 file:
                PostAdvisorDocument: function PostAdvisorDocument(advisorId,documentId, document,submitted) {
                    return $q(function (resolve, reject) {
                        var formData = new FormData();
                        formData.append('file', document.file);
                        $http.post('api/AdvisorDocuments/' + advisorId + '/DocumentTypeId/' + documentId + '/' + submitted, document).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });

                    });
                },
                ///////////////////////////////////////////////////////////////////////////////////////////
           
                PostSendAdviserMemberCode: function PostSendAdviserMemberCode(advisorSupplierCode) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Advisor/AdvisorSupplierCode/', advisorSupplierCode).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });

                    });

                },

                PutAdvisorSupplierCode: function PutAdvisorSupplierCode(adviosorId,supplierId, advisorSupplierCode) {
                    return $q(function (resolve, reject) {
                        $http.put('api/Advisor/AdvisorSupplierCode/Put/' + adviosorId+ '/' + supplierId, advisorSupplierCode).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });

                    });

                },

                CreateAdvisor2: function CreateAdvisor2(advisor) {
                    console.log("Registering Advisor: ", advisor);
                    $.ajax({
                        url: 'api/Advisor',
                        type: "POST",
                        data: JSON.stringify(advisor),
                        contentType: "application/json"
                    });
                },

                CreateAdvisor: function CreateAdvisor(advisor) {
                    console.log("Registering Advisor: ", advisor);
                    return $q(function (resolve, reject) {
                        $http.post('api/Advisor/', advisor).then(
                            function (a) {
                                resolve(a);
                            }, function (a) {
                                reject(a);
                            });
                    });
                },

                PutchangeIsExpired: function PutchangeIsExpired(advisorDocId, status) {
                    return $q(function (resolve, reject) {
                        $http.put('api/AdvisorDocuments/' + advisorDocId +'/' + status).then(
                            function (a) {
                                resolve(a);
                            }, function (a) {
                                reject(a);
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

                deleteAdviDoc: function deleteAdviDoc(id, AdviId, DocType) {
                    return $q(function (resolve, reject) {
                        $http.delete('api/Advisor/DeleteDupAdviDocs/' + id + '/' + AdviId + '/' + DocType).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },
        
            };

            Object.defineProperty(service, "currentuser", {
                get: function () {
                    var u = $localStorage.currentuser; return u && u.username;
                },
                enumerable: true,
                configurable: true
            })
        }

        return service;
    }]);


    tendaApp.config(['$httpProvider', function ($httpProvider) {

        $httpProvider.interceptors.push(['$q', '$injector', function ($q, $injector) {
            return {
                'request': function (config) {

                    var advisorService = $injector.get('advisorService');
                    return config;
                },

                'responseError': function (rejection) {
                    return $q.reject(rejection);
                }
            };
        }]);
    }]);

})();