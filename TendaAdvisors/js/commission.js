

tendaApp.controller('commissionController', ['$scope', 'applicationService', 'authService', 'commissionService', 'clientsService', 'advisorService', function ($scope, applicationService, authService, commissionService, clientsService, advisorService) {
    $scope.advisorId = 0;
    $scope.advisorList = [];
    $scope.membersList = [];
    $scope.setMemberList = false;
    $scope.membersCommissionList = [];
    $scope.advisorStatusList = [];
    $scope.clientList = [];
    $scope.advisorChoice = null;
    $scope.commisionStatementId = 0;
    $scope.commissionList = [];
    $scope.advisorTotal = 0;
    $scope.companyTotal = 0;
    $scope.commissionInclVATTotal = 0;
    $scope.commissionExclVATTotal = 0;
    $scope.advisorTaxTotal = 0;
    $scope.AdvisorTaxRate = 0;
    $scope.setAdvisor = false;
    $scope.setExceptionList = false;
    $scope.closeExceptionList = false;
    $scope.applicationStatusList = [];
    $scope.member = [];
    $scope.contacts = [];

    var fromDate = new Date(Date.now() - 2592000000);
    var toDate = new Date(Date.now());


    $scope.paging = {
        config: { pageIndex: 1, pageSize: 10 },
        pageSizes: [1, 2, 10, 20, 50, 100],
        totalRows: 0,
        pages: [],
        updatePages: function () {
            this.pages = [];

            for (var i = 1; i <= this.totalRows / (this.totalRows < this.config.pageSize ? this.totalRows : this.config.pageSize); i++) {
                this.pages.push(i);
            }

            if (this.config.pageIndex > this.pages.length) {
                this.config.pageIndex = this.pages.length;
            }

            return this.pages;
        }
    };

    $scope.paging.config["dateFrom"] = fromDate.yyyymmdd('-');
    $scope.paging.config["dateTo"] = toDate.yyyymmdd('-');


    $scope.getTotals = function () {
        $scope.advisorTotal = 0;
        $scope.companyTotal = 0;
        $scope.commissionInclVATTotal = 0;
        $scope.commissionExclVATTotal = 0;
        $scope.advisorTaxTotal = 0;
        $scope.AdvisorTaxRate = 0;

        for (var i = 0; i < $scope.commissionList.length; i++) {
            var commision = $scope.commissionList[i];
            $scope.advisorTotal += commision.advisorCommission;
            $scope.companyTotal += commision.companyCommission;
            $scope.commissionInclVATTotal += commision.commissionInclVAT;
            $scope.commissionExclVATTotal += commision.commissionExclVAT;
            $scope.advisorTaxTotal += commision.advisorTax;
            $scope.AdvisorTaxRate = commision.advisorTaxRate;
        }
    };

    $scope.getCommissionList = function getCommissionList(advisorId, config) {
        $scope.commissionList = ["0"];
        commissionService.GetCommissionList(advisorId, config).then(function succeeded(success) {
            $scope.commissionList = success.data;
            $scope.getTotals();
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.doSearch = function doSearch(searchWhere) {
        if (!!searchWhere && searchWhere.trim().length >= 4)
            $scope.getContacts(searchWhere);
    };

    $scope.getContacts = function getContacts(searchQuery) {
        clientsService.GetBasicContact(searchQuery).then(function succeeded(contact) {
            $scope.contact = contact.data;
            $scope.member = $scope.contact[0];
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.EmailCommissions = function EmailCommissions(advisorId, dateFrom, dateTo) {
        commissionService.EmailCommissions(advisorId, dateFrom, dateTo).then(function succeeded(success) {
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.getCommissionList($scope.advisorId, $scope.paging.config);

    $scope.getAdvisors = function getAdvisors() {
        commissionService.GetAllAdvisors().then(
            function succeeded(success) {
                $scope.advisorList = success.data;
            }, function failure(errormessage) {
                console.log(errormessage);
            }
        );
    };

    $scope.getAdvisors();

    $scope.getAdvisorsTypeTwo = function getAdvisorsTypeTwo() {
        commissionService.GetAllAdvisorsTypeTwo().then(
            function succeeded(success) {
                $scope.advisorListTypeTwo = success.data;
            }, function failure(errormessage) {
                console.log(errormessage);
            }
        );
    };
    $scope.getAdvisorsTypeTwo();


    //STATUS
    $scope.GetAdvisorStatuses = function GetAdvisorStatuses() {
        commissionService.GetAdvisorStatuses().then(
            function succeeded(success) {
                $scope.advisorStatusList = success.data;
            }, function failure(errormessage) {
                console.log(errormessage);
            }
        );
    };
    $scope.GetAdvisorStatuses();


    $scope.GetApplicationStatuses = function GetApplicationStatuses() {
        commissionService.GetApplicationStatuses().then(
            function succeeded(success) {
                $scope.applicationStatusList = success.data;
            }, function failure(errormessage) {
                console.log(errormessage);
            }
        );
    };
    $scope.GetApplicationStatuses();

    //SUPPLIER
    $scope.GetAdvisorSupplier = function GetAdvisorSupplier() {
        commissionService.GetAdvisorSupplier().then(
            function succeeded(success) {
                $scope.advisorSupplierList = success.data;
            }, function failure(errormessage) {
                console.log(errormessage);
            }
        );
    };
    $scope.GetAdvisorSupplier();

    $scope.GetAdvisorsCompany = function GetAdvisorsCompany() {
        advisorService.GetAdvisorsCompany().then(
            function succeeded(success) {
                $scope.advisorCompany = success.data;
            }, function failure(errormessage) {
                console.log(errormessage);
            }
        );
    };
    $scope.GetAdvisorsCompany();

    $scope.addAdvisor = function addAdvisor() {
        $scope.setAdvisor = true;
    };

    $scope.viewExceptionList = function viewExceptionList() {
        commissionService.GetUnmatchedCommissions().then(function succeeded(success) {
            $scope.expectionList = success.data;
            $scope.setExceptionList = true;
            $scope.closeExceptionList = true;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.viewMemberCommission = function viewMemberCommission(memberId, dateFromMem, dateToMem) {

        commissionService.GetMemberCommission(memberId, dateFromMem, dateToMem).then(function succeeded(success) {
            $scope.membersCommissionList = success.data;
            $scope.membersCommissionList.forEach(function (item, index) {
                item.transactionDate = item.transactionDate.substring(0, item.transactionDate.indexOf("T"));
                item.commAmmount = 'R' + item.commAmmount.toFixed(2);
            });

            $scope.setMemberList = true;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };


    $scope.closeExceptionListFunc = function closeExceptionListFunc() {
        $scope.setExceptionList = false;
        $scope.closeExceptionList = false;
    };

    $scope.linkAdvisor = function linkAdvisor(advisorId, commisionStatementId) {
        commissionService.AdvisorToCommision(advisorId, commisionStatementId).then(function succeeded(success) {
            $scope.getCommissionList($scope.advisorId, $scope.paging.config);
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.linkAdvisor($scope.advisorId, $scope.commisionStatementId);

    $scope.linkClient = function linkClient(clientId, commisionStatementId) {
        commissionService.ClientToCommision(clientId, commisionStatementId).then(function succeeded(success) {
            $scope.getCommissionList($scope.advisorId, $scope.paging.config);
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };
    
    $scope.linkClient($scope.clientId, $scope.commisionStatementId);

    $scope.exportCommision = function exportCommision(advisorId, config) {
        commissionService.GetCommissionReport(advisorId, config).then(function succeeded(success) {
            $scope.commissionList = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.exceptionList = function exceptionList() {
        commissionService.GetExceptionReport().then(function succeeded(success) {
            $scope.exceptionListData = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.commissionSummaryListVIP = function commissionSummaryListVIP(dateFromVip, dateToVip) {
        commissionService.commissionSummaryListVIP(dateFromVip, dateToVip).then(function succeeded(success) {
            $scope.commissionVIP = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    applicationService.GetDashboard().then(function succeeded(success) {
        $scope.dashboard = success;
        advisorService.GetAdvisorDocuments();
        //Need to find a better place to do this
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    

    $scope.representativeList = function representativeList() {
        commissionService.representativeList().then(function succeeded(success) {
            $scope.representativeListData = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    
    $scope.applicationsPerRepresentativeList = function applicationsPerRepresentativeList(advisorSelectId, advisorStatusId, advisorSupplierId, dateFromSelect, dateToSelect) {
        commissionService.applicationsPerRepresentativeList(advisorSelectId, advisorStatusId, advisorSupplierId, dateFromSelect, dateToSelect).then(function succeeded(success) {
            $scope.applicationsPerRepresentativeData = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.TotalBusiness = function TotalBusiness(advisorTotalBusinessSupplierId, dateFromTotalBusinessSelect, dateToTotalBusinessSelect) {
        commissionService.TotalBusiness(advisorTotalBusinessSupplierId, dateFromTotalBusinessSelect, dateToTotalBusinessSelect).then(function succeeded(success) {
            $scope.TotalBusinessData = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    

    $scope.applicationsExport = function applicationsExport(dateFromTotalBusinessApps, dateToTotalBusinessApps) {
        commissionService.applicationsExport(dateFromTotalBusinessApps, dateToTotalBusinessApps).then(function succeeded(success) {
            $scope.TotalBusinessAppData = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    

    $scope.CompanyDetailsList = function CompanyDetailsList(companyNameId) {
        commissionService.CompanyDetailsList(companyNameId).then(function succeeded(success) {
            $scope.companyNameData = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };


    $scope.companyAdvisers = function companyAdvisers() {
        commissionService.companyAdvisers().then(function succeeded(success) {
            $scope.companyAdvisersData = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };
    
    
    $scope.companyKeyIndividuals = function companyKeyIndividuals() {
        commissionService.companyKeyIndividuals().then(function succeeded(success) {
            $scope.companyKeyIndividualsData = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    
    
    $scope.companyLicenses = function companyLicenses() {
        commissionService.companyLicenses().then(function succeeded(success) {
            $scope.companyLicensesData = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    
    $scope.companyDocuments = function companyDocuments(advisorCompanyDocumentsId) {
        commissionService.companyDocuments(advisorCompanyDocumentsId).then(function succeeded(success) {
            $scope.companyDocumentsData = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.exportCommisionForAdvisor = function exportCommisionForAdvisor(advisorCommissionId, dateFromCommissionForAdvisor, dateToCommissionForAdvisor) {
        commissionService.exportCommisionForAdvisor(advisorCommissionId, dateFromCommissionForAdvisor, dateToCommissionForAdvisor).then(function succeeded(success) {
            $scope.exportCommisionForAdvisorData = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.exceptionListDelete = function exceptionListDelete(Id) {
        commissionService.exceptionListDelete(Id).then(function succeeded(success) {
            $scope.exceptionListDeleteData = success.data;
            $scope.viewExceptionList();
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };
}]);
