tendaApp.controller('applicationController', ['$rootScope', '$scope', 'clientsService', 'applicationService', 'advisorService', '$state', function ($rootScope, $scope, clientsService, applicationService, advisorService, $state) {
    $scope.registerItem = 0;
    $scope.foundContacts = [];
    $scope.advisors = [];
    $scope.storeApplicationId = 0;
    $scope.clickedApplication = false;
    $scope.noApps = false;
    $scope.notClient = false;
    $scope.totalApplicationsPerClient = [];

    $scope.gotoPage = function gotoPage(page) {
        if (page === 0 || page === 1 || page === 2 || page === 3 || page === 4 || page === 5) {
            $scope.registerItem = page;
        }
    };

    $scope.DeleteApplication = function DeleteApplication(id) {
        applicationService.DeleteApplication(id)
            .then(function succeeded(success) {
                alert("Deleted");
            },
            function failure(errormessage) {
                console.log(errormessage);
                alert("Not Deleted");
            });
    };

    $scope.getAdivisors = function getAdvisors() {
        advisorService.GetAdvisorsAdvisor()
            .then(function succeeded(advisorAdvisor) {
                if (advisorAdvisor.data.length === 0) {
                    return;
                }
                $scope.advisors = advisorAdvisor.data;
            }, function failure(errormessage) {
                console.log(errormessage);
            });
    };

    $scope.doSearch = function doSearch(searchWhere) {
        //$scope.getAdivisors();
        $scope.getContacts(searchWhere);
        $scope.clickedApplication = false;
        $scope.noApps = false;
        $scope.notClient = false;
    };

    //get Contacts
    $scope.getContacts = function getContacts(searchQuery) {
        clientsService.GetBasicContact(searchQuery).then(function succeeded(contact) {
            clientsService.GetContactDetails(contact.data[0].id).then(function succeeded(contact) {
                $scope.contact = contact.data;
                if ($scope.contact !== null) {
                    $scope.application.client = $scope.contact;
                    var createdDateString = $scope.application.client.createdDate;
                    var modifiedDateString = $scope.application.client.modifiedDate;
                    var createdIndex = createdDateString !== null ? createdDateString.indexOf("T") : 0;
                    var modifiedIndex = modifiedDateString !== null ? modifiedDateString.indexOf("T") : 0;
                    $scope.application.client.createdDate = createdDateString !== null ? createdDateString.substring(0, createdIndex) : '';
                    $scope.application.client.modifiedDate = modifiedDateString !== null ? modifiedDateString.substring(0, modifiedIndex) : '';
                }
                else {
                    $scope.application.client = null;
                }
            }, function failure(errormessage) {
                console.log("Failed getting contact details: ", errormessage);
            });
        }, function failure(errormessage) {
            console.log("Failed to get basic contact details: ", errormessage);
        });
    };

    $scope.AppplicationAppClicked = function AppplicationAppClicked(id) {
        applicationService.GetApplicationDocumentFile(id)
            .then(function succeeded(success) {
            },
            function failure(errormessage) {
                console.log(errormessage);
            }
        );
    };

    $scope.allApplicationsClicked = function allApplicationsClicked() {
        $state.go('application');
    };
    
    $scope.calculateSum = function calculateSum(data, applicationStatus, advisorStatus) {
        if (typeof data === "undefined") {
            return 0;
        }

        var count = 0;
        for (var i = 0; i < data.length; i++) {
            if (data[i].applicationStatus.id === applicationStatus && data[i].advisor.advisorStatus.id === advisorStatus)
                count++;
        }
        return count;
    };
   
    $scope.filterStatus = function filterStatus(status) {
        alert(status);
    };

    $scope.calculateSumExpired = function calculateSumExpired(data) {
        if (typeof data === "undefined") {
            return 0;
        }

        var countExpired = 0;
        for (var i = 0; i < data.length; i++) {
            if (data[i].isExpired === true)
                countExpired++;
        }
        return countExpired;
    };

    //Gets the status colour according to the status ID:
    $scope.getStatusClassColour = function getStatusClassColour(status) {
        if (status === 0)
            return "activityStatusPending";
        else if (status === 1)
            return "activityStatusActive";
        else if (status === 2)
            return "activityStatusInactive";
        else
            return "";
    };

    $scope.GetApplicationDocuments = function GetApplicationDocuments() {
        applicationService.GetApplicationDocuments().then(
            function succeeded(success) {
                if (success.length === 0) {
                    return;
                }

                //Ensure that the expiry date for each document is updated on the view
                for (var i = 0; i < success.data.length; i++) {
                    if (typeof success.data[i] !== 'undefined') {

                        var dateFrom = new Date(success.data[i].validFromDate);
                        var dateTo = new Date(success.data[i].validToDate);

                        if (success.data[i].validFromDate !== null && success.data[i].validToDate !== null) {
                            success.data[i].isExpired = applicationService.AssertIfExpired(new Date(success.data[i].validFromDate), new Date(success.data[i].validToDate));
                            $scope.adjustExpiry(success.data[i].id, success.data[i].isExpired);
                            success.data[i].validFromDate = ' ' + dateFrom.getFullYear() + '/' + dateFrom.getMonth() + '/' + dateFrom.getDate();
                            success.data[i].validToDate = ' ' + dateTo.getFullYear() + '/' + dateTo.getMonth() + '/' + dateTo.getDate();
                        }
                        else if (success.data[i].validFromDate === null && success.data[i].validToDate !== null) {
                            success.data[i].isExpired = applicationService.AssertIfExpired(new Date(success.data[i].validtodate));
                            $scope.adjustExpiry(success.data[i].id, success.data[i].isExpired);
                            success.data[i].validFromDate = ' ' + dateFrom.getFullYear() + '/' + dateFrom.getMonth() + '/' + dateFrom.getDate();
                            success.data[i].validToDate = ' ' + dateTo.getFullYear() + '/' + dateTo.getMonth() + '/' + dateTo.getDate();
                        }
                        else {
                            // data is null, why do anything with it?
                            success.data[i].isExpired = false;
                        }
                    }
                }
                $scope.applicationsDocuments = success.data;
            }, function failure(errormessage) {
                console.log(errormessage);
            });
    };

    $scope.adjustExpiry = function adjustExpiry(docID, status) {
        applicationService.PutchangeIsExpired(docID, status).then(function succeeded(success) {
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.applicationSelected = function applicationSelected(id) {
        $scope.storeApplicationId = 0;
        applicationService.GetApplicationsbyClientId(id).then(
            function succeeded(applications) {
                if (applications.length === 0) {
                    return;
                }

                $scope.applications = applications.data;

                for (var i = 0; i < $scope.applications.length; i++) {
                    if (!$rootScope.AdvisorLog || ($rootScope.AdvisorLog && $scope.applications[i].advisor_Id === $rootScope.UserID)) {
                        $scope.clickedApplication = true;
                        $scope.storeApplicationId = $scope.storeApplicationId + 1;
                        $scope.totalApplicationsPerClient.push($scope.applications[i].id);
                    }
                }

                if ($scope.applications.length === 0) {
                    $scope.noApps = true;
                }

                if ($rootScope.AdvisorLog && !$scope.noApps) {
                    $scope.notClient = true;
                }
            }, function failure(errormessage) {
                console.log(errormessage);
            });
    };

    $scope.applicationClicked = function applicationClicked(id) {
        $state.go('applicationDetail', { 'application': id });
    };

    $scope.goToNewApplicationPage = function goToNewApplicationPage() {
        $state.go('newApplication');
    };
}]);
