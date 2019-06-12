
(function () {
    var service;

    tendaApp.factory('commissionService', ['$http', '$q', function ($http, $q) {
        function formTransform(obj) {
            var str = [];
            for (var p in obj) str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
            return str.join("&");
        }

        if (!service) {
            service = {
                GetCommissionList: function GetCommissionList(advisorId, config) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisor/' + advisorId + '/Commission/' + config.pageIndex + '/' + config.pageSize + '/' + config.dateFrom+ '/' + config.dateTo).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetCommissionReport: function GetCommissionReport(advisorId, config) {
                    return $q(function (resolve, reject) {
                        $http({
                            url: 'api/Advisor/' + advisorId + '/CommissionReport/' + config.dateFrom + '/' + config.dateTo,
                            method: 'POST',
                            params: {},
                            headers: {
                                'Content-type': 'text/csv',
                                'Content-Disposition': 'attachment;filename=commissionReport.csv',
                            },
                            responseType: 'arraybuffer'
                        }).success(function (data, status, headers, config) {
                            // TODO when WS success
                            var file = new Blob([data], {
                                type: 'application/csv'
                            });
                            //trick to download store a file having its URL
                            var fileURL = URL.createObjectURL(file);
                            var a = document.createElement('a');
                            a.href = fileURL;
                            a.target = '_blank';
                            a.download = 'commissionReport.csv';
                            document.body.appendChild(a);
                            a.click();
                        }).error(function (data, status, headers, config) {
                            //TODO when WS error
                        });
                    });
                },

                exportCommisionForAdvisor: function exportCommisionForAdvisor(advisorCommissionId, dateFromCommissionForAdvisor, dateToCommissionForAdvisor) {
                    return $q(function (resolve, reject) {
                        $http({
                            url: 'api/Advisor/' + advisorCommissionId + '/CommissionTransactionRep/' + dateFromCommissionForAdvisor + '/' + dateToCommissionForAdvisor,
                            method: 'POST',
                            params: {},
                            headers: {
                                'Content-type': 'text/csv',
                                'Content-Disposition': 'attachment;filename=CommissionTransactionRep.csv',
                            },
                            responseType: 'arraybuffer'
                        }).success(function (data, status, headers, config) {
                            // TODO when WS success
                            var file = new Blob([data], {
                                type: 'application/csv'
                            });
                            //trick to download store a file having its URL
                            var fileURL = URL.createObjectURL(file);
                            var a = document.createElement('a');
                            a.href = fileURL;
                            a.target = '_blank';
                            a.download = 'CommissionTransactionRep.csv';
                            document.body.appendChild(a);
                            a.click();
                        }).error(function (data, status, headers, config) {
                            //TODO when WS error
                        });
                    });
                },

                EmailCommissions: function EmailCommissions(advisorId, dateFrom, dateTo) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Advisor/MailAdvisorComission/' + advisorId + '/' + dateFrom + '/' + dateTo).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAllMembers: function GetAllMembers() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Clients/All').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetExceptionReport: function GetExceptionReport() {
                    return $q(function (resolve, reject) {
                        $http({
                            url: 'api/Advisor/ExceptionList/',
                            method: 'POST',
                            params: {},
                            headers: {
                                'Content-type': 'text/csv',
                                'Content-Disposition': 'attachment;filename=exceptionListReport.csv',
                            },
                            responseType: 'arraybuffer'
                        }).success(function (data, status, headers) {
                            // TODO when WS success
                            var file = new Blob([data], {
                                type: 'application/csv'
                            });
                            //trick to download store a file having its URL
                            var fileURL = URL.createObjectURL(file);
                            var a = document.createElement('a');
                            a.href = fileURL;
                            a.target = '_blank';
                            a.download = 'exceptionListReport.csv';
                            document.body.appendChild(a);
                            a.click();
                        }).error(function (data, status, headers) {
                            //TODO when WS error
                        });
                    });
                },

                commissionSummaryListVIP: function commissionSummaryListVIP(dateFromVip, dateToVip) {
                    return $q(function (resolve, reject) {
                        $http({
                            url: 'api/Advisor/CommissionSummary/' + dateFromVip + '/' + dateToVip,
                            method: 'POST',
                            params: {},
                            headers: {
                                'Content-type': 'text/csv',
                                'Content-Disposition': 'attachment;filename=commissionSummaryListVIP.csv',
                            },
                            responseType: 'arraybuffer'
                        }).success(function (data, status, headers) {
                            // TODO when WS success
                            var file = new Blob([data], {
                                type: 'application/csv'
                            });
                            //trick to download store a file having its URL
                            var fileURL = URL.createObjectURL(file);
                            var a = document.createElement('a');
                            a.href = fileURL;
                            a.target = '_blank';
                            a.download = 'commissionSummaryListVIP.csv';
                            document.body.appendChild(a);
                            a.click();
                        }).error(function (data, status, headers) {
                            //TODO when WS error
                        });
                    });
                },

                applicationsPerRepresentativeList: function applicationsPerRepresentativeList(advisorSelectId, advisorStatusId, advisorSupplierId, dateFromSelect, dateToSelect) {
                        return $q(function (resolve, reject) {
                            $http({
                                url: 'api/Advisor/' + advisorSelectId + '/TotalBusiness/' + dateFromSelect + '/' + dateToSelect+ '/' + advisorSupplierId+ '/' + advisorStatusId,
                                method: 'POST',
                                params: {},
                                headers: {
                                    'Content-type': 'text/csv',
                                    'Content-Disposition': 'attachment;filename=applicationsPerRepresentativeList.csv'
                                },
                                responseType: 'arraybuffer'
                            }).success(function (data, status, headers) {
                                // TODO when WS success
                                var file = new Blob([data], {
                                    type: 'application/csv'
                                });
                                //trick to download store a file having its URL
                                var fileURL = URL.createObjectURL(file);
                                var a = document.createElement('a');
                                a.href = fileURL;
                                a.target = '_blank';
                                a.download = 'applicationsPerRepresentativeList.csv';
                                document.body.appendChild(a);
                                a.click();
                            }).error(function (data, status, headers) {
                                //TODO when WS error
                            });
                        });
                },

                GetMemberCommission: function GetMemberCommission(memberId, dateFromMem, dateToMem) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Clients/CommissionPayed/' + memberId + '/' + dateFromMem + '/' + dateToMem).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },
                
                applicationsExport: function applicationsExport(dateFromTotalBusinessApps, dateToTotalBusinessApps) {
                        return $q(function (resolve, reject) {
                            $http({
                                url: 'api/Advisor/TotalBusiness/' + dateFromTotalBusinessApps + '/' + dateToTotalBusinessApps,
                                method: 'POST',
                                params: {},
                                headers: {
                                    'Content-type': 'text/csv',
                                    'Content-Disposition': 'attachment;filename=applicationsExport.csv',
                                },
                                responseType: 'arraybuffer'
                            }).success(function (data, status, headers) {
                                // TODO when WS success
                                var file = new Blob([data], {
                                    type: 'application/csv'
                                });
                                //trick to download store a file having its URL
                                var fileURL = URL.createObjectURL(file);
                                var a = document.createElement('a');
                                a.href = fileURL;
                                a.target = '_blank';
                                a.download = 'applicationsExport.csv';
                                document.body.appendChild(a);
                                a.click();
                            }).error(function (data, status, headers) {
                                //TODO when WS error
                            });
                        });
                    },
                
                representativeList: function representativeList() {
                        return $q(function (resolve, reject) {
                            $http({
                                url: 'api/Advisor/RepList/',
                                method: 'POST',
                                params: {},
                                headers: {
                                    'Content-type': 'text/csv',
                                    'Content-Disposition': 'attachment;filename=representativeList.csv',
                                },
                                responseType: 'arraybuffer'
                            }).success(function (data, status, headers) {
                                // TODO when WS success
                                var file = new Blob([data], {
                                    type: 'application/csv'
                                });
                                //trick to download store a file having its URL
                                var fileURL = URL.createObjectURL(file);
                                var a = document.createElement('a');
                                a.href = fileURL;
                                a.target = '_blank';
                                a.download = 'representativeList.csv';
                                document.body.appendChild(a);
                                a.click();
                            }).error(function (data, status, headers) {
                                //TODO when WS error
                            });
                        });
                },

                TotalBusiness: function TotalBusiness(advisorTotalBusinessSupplierId, dateFromTotalBusinessSelect, dateToTotalBusinessSelect) {
                        return $q(function (resolve, reject) {
                            $http({
                                url: 'api/Advisor/TotalBusiness/' +  dateFromTotalBusinessSelect + '/' + dateToTotalBusinessSelect+ '/' +advisorTotalBusinessSupplierId ,
                                method: 'POST',
                                params: {},
                                headers: {
                                    'Content-type': 'text/csv',
                                    'Content-Disposition': 'attachment;filename=TotalBusiness.csv',
                                },
                                responseType: 'arraybuffer'
                            }).success(function (data, status, headers) {
                                // TODO when WS success
                                var file = new Blob([data], {
                                    type: 'application/csv'
                                });
                                //trick to download store a file having its URL
                                var fileURL = URL.createObjectURL(file);
                                var a = document.createElement('a');
                                a.href = fileURL;
                                a.target = '_blank';
                                a.download = 'TotalBusiness.csv';
                                document.body.appendChild(a);
                                a.click();
                            }).error(function (data, status, headers) {
                                //TODO when WS error
                            });
                        });
                },

                CompanyDetailsList: function CompanyDetailsList(companyNameId) {
                    return $q(function (resolve, reject) {
                        $http({
                          
                             url: 'api/Advisor/CompanyDetailsRep/' + companyNameId,
                            method: 'POST',
                            params: {},
                            headers: {
                                'Content-type': 'text/csv',
                                'Content-Disposition': 'attachment;filename=CompanyDetails.csv',
                            },
                            responseType: 'arraybuffer'
                        }).success(function (data, status, headers) {
                            // TODO when WS success
                            var file = new Blob([data], {
                                type: 'application/csv'
                            });
                            //trick to download store a file having its URL
                            var fileURL = URL.createObjectURL(file);
                            var a = document.createElement('a');
                            a.href = fileURL;
                            a.target = '_blank';
                            a.download = 'CompanyDetails.csv';
                            document.body.appendChild(a);
                            a.click();
                        }).error(function (data, status, headers) {
                            //TODO when WS error
                        });
                    });
                },

                companyAdvisers: function companyAdvisers() {
                    return $q(function (resolve, reject) {
                        $http({
                            url: 'api/Advisor/CompanyDetailsAdvisorsRep',
                            method: 'POST',
                            params: {},
                            headers: {
                                'Content-type': 'text/csv',
                                'Content-Disposition': 'attachment;filename=companyAdvisersList.csv',
                            },
                            responseType: 'arraybuffer'
                        }).success(function (data, status, headers) {
                            // TODO when WS success
                            var file = new Blob([data], {
                                type: 'application/csv'
                            });
                            //trick to download store a file having its URL
                            var fileURL = URL.createObjectURL(file);
                            var a = document.createElement('a');
                            a.href = fileURL;
                            a.target = '_blank';
                            a.download = 'companyAdvisersList.csv';
                            document.body.appendChild(a);
                            a.click();
                        }).error(function (data, status, headers) {
                            //TODO when WS error
                        });
                    });
                },

                companyKeyIndividuals: function companyKeyIndividuals() {
                    return $q(function (resolve, reject) {
                        $http({
                            url: 'api/Advisor/CompanyDetailsKeyIndividualsRep',
                            method: 'POST',
                            params: {},
                            headers: {
                                'Content-type': 'text/csv',
                                'Content-Disposition': 'attachment;filename=companyKeyIndividualsList.csv',
                            },
                            responseType: 'arraybuffer'
                        }).success(function (data, status, headers) {
                            // TODO when WS success
                            var file = new Blob([data], {
                                type: 'application/csv'
                            });
                            //trick to download store a file having its URL
                            var fileURL = URL.createObjectURL(file);
                            var a = document.createElement('a');
                            a.href = fileURL;
                            a.target = '_blank';
                            a.download = 'companyKeyIndividualsList.csv';
                            document.body.appendChild(a);
                            a.click();
                        }).error(function (data, status, headers) {
                            //TODO when WS error
                        });
                    });
                },

                companyLicenses: function companyLicenses() {
                    return $q(function (resolve, reject) {
                        $http({
                            url: 'api/Advisor/CompanyDetailsLicences',
                            method: 'POST',
                            params: {},
                            headers: {
                                'Content-type': 'text/csv',
                                'Content-Disposition': 'attachment;filename=companyLicenses.csv',
                            },
                            responseType: 'arraybuffer'
                        }).success(function (data, status, headers) {
                            // TODO when WS success
                            var file = new Blob([data], {
                                type: 'application/csv'
                            });
                            //trick to download store a file having its URL
                            var fileURL = URL.createObjectURL(file);
                            var a = document.createElement('a');
                            a.href = fileURL;
                            a.target = '_blank';
                            a.download = 'companyLicenses.csv';
                            document.body.appendChild(a);
                            a.click();
                        }).error(function (data, status, headers) {
                            //TODO when WS error
                        });
                    });
                },

                
                companyDocuments: function companyDocuments(advisorCompanyDocumentsId) {
                    return $q(function (resolve, reject) {
                        $http({
                        url: 'api/Advisor/CompanyDetailsDocuments/' + advisorCompanyDocumentsId,
                            method: 'POST',
                            params: {},
                            headers: {
                                'Content-type': 'text/csv',
                                'Content-Disposition': 'attachment;filename=advisorCompanyDocuments.csv',
                            },
                            responseType: 'arraybuffer'
                        }).success(function (data, status, headers) {
                            // TODO when WS success
                            var file = new Blob([data], {
                                type: 'application/csv'
                            });
                            //trick to download store a file having its URL
                            var fileURL = URL.createObjectURL(file);
                            var a = document.createElement('a');
                            a.href = fileURL;
                            a.target = '_blank';
                            a.download = 'advisorCompanyDocuments.csv';
                            document.body.appendChild(a);
                            a.click();
                        }).error(function (data, status, headers) {
                            //TODO when WS error
                        });
                    });
                },

                GetProvinces: function GetProvinces() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Provinces/').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetCountries: function GetCountries() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Countries/').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAllAdvisors: function GetAllAdvisors() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisors/All').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },
                
                GetAllAdvisorsTypeTwo: function GetAllAdvisorsTypeTwo() {
                        return $q(function (resolve, reject) {
                            $http.get('api/Advisors/typeTwo/All').then(function (d) {
                                resolve(d);
                            }, function (d) {
                                reject(d);
                            });
                        });
                },

                exceptionListDelete: function exceptionListDelete(Id) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Advisor/'+Id+'/ExceptionListDelete').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorStatuses: function GetAdvisorStatuses() {
                    return $q(function (resolve, reject) {
                        $http.get('api/AdvisorStatus').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetApplicationStatuses: function GetApplicationStatuses() {
                    return $q(function (resolve, reject) {
                        $http.get('api/ApplicationStatus').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetAdvisorSupplier: function GetAdvisorSupplier() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Supplier/Basic').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetUnmatchedCommissions: function GetUnmatchedCommissions() {
                    return $q(function (resolve, reject) {
                        $http.get('api/unmatchedCommisions/All').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },
                
                GetAllClients: function GetAllClients() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Contacts').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                ClientToCommision: function ClientToCommision(clientId, commisionStatementId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Clients/' + clientId + '/CommissionStatement/' + commisionStatementId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                AdvisorToCommision: function AdvisorToCommision(advisorId, commisionStatementId) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Advisor/' + advisorId + '/CommissionStatement/' + commisionStatementId).then(function (d) {
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
                    var commissionService = $injector.get('commissionService');
                    return config;
                },
                'responseError': function (rejection) {
                    return $q.reject(rejection);
                }
            };
        }]);
    }]);
})();
