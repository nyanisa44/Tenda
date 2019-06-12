tendaApp.controller('advisorsController', ['$scope', 'clientsService', 'advisorService', '$state', function ($scope, clientsService,advisorService, $state) {

    $scope.goToNewAdvisorPage = function goToNewAdvisorPage() {
        $state.go('registerAdvisor');
    };

    // FIELDS
    $scope.contact = {};
    $scope.advisors = [];
    $scope.advisorId = 0;
    $scope.advisorsAdmin = [];
    $scope.advisorsKey = [];
    $scope.advisorsDir = [];
    $scope.isAdvisorContact = false;
    $scope.isAdminContact = false;
    $scope.isKeyContact = false;
    $scope.isDirContact = false;

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

    // SEARCH FIELDS
    $scope.doSearch = function doSearch(searchWhere, adviserTypeId) {
        if (typeof searchWhere === 'string' && searchWhere.trim().length >= 4) {
            $scope.getContacts(searchWhere, adviserTypeId);
        }
    };

    // LOAD ADVISORS
    advisorService.GetAdvisorsAdvisor().then(function succeeded(advisorAdvisor) {
        if (advisorAdvisor.data.length === 0) {
            return;
        }
        $scope.advisors = advisorAdvisor.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // SEARCH QUERIES
    $scope.getContacts = function getContacts(searchQuery, adviserTypeId) {
        clientsService.GetAdvisorContact(searchQuery).then(function succeeded(contact) {
            $scope.contact = contact.data[0];

            for (var i = 0; i < $scope.advisors.length; i++) {
                if ($scope.advisors[i].id === $scope.contact.id) {
                    $scope.GetAdvisorIdOfAdvisorType($scope.contact.id, adviserTypeId);
                    break;
                }
            }
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    // ITEM CLICKED
    $scope.advisorClickedGetId = function advisorClickedGetId(id) {
        $state.go('advisor', { 'advisor': id });
    };

    $scope.dirClickedGetId = function dirClickedGetId(id) {
        $state.go('advisor', { 'advisor': id });
    };

    $scope.adminClickedGetId = function adminClickedGetId(id) {
        $state.go('advisor', { 'advisor': id });
    };

    $scope.keyClickedGetId = function keyClickedGetId(id) {
        $state.go('advisor', { 'advisor': id });
    };

    $scope.advisorClicked = function advisorClicked(id) {
        $state.go('advisor', { 'advisor': id });
    };

    // DELETING
    $scope.deleteAdvisor = function deleteAdvisor(advisorId) {
        advisorService.DeleteAdvisor(advisorId)
            .then(function succeeded(success) {
                alert("Deleted");
            },
            function failure(errormessage) {
                alert("Not Deleted");
            });
    };

    $scope.getAdvisorId = function getAdvisorId(id) {
        advisorService.GetAdvisorId(id)
            .then(function success(response) {
                $scope.advisorIdPast = response.data;
            },
            function failure(error) {
                console.error(error);
            });
    };

    $scope.GetAdvisorIdOfAdvisorType = function GetAdvisorIdOfAdvisorType(id, adviserTypeId) {
        $scope.isAdvisorContact = false;
        $scope.isAdminContact = false;
        $scope.isKeyContact = false;
        $scope.isDirContact = false;
        advisorService.GetAdvisorIdOfAdvisorType(id, adviserTypeId)
            .then(function success(response) {
                $scope.advisorId = response.data;

                if ($scope.advisorId !== 0 && adviserTypeId === 1) {
                    $scope.isKeyContact = true;
                }
                if ($scope.advisorId !== 0 && adviserTypeId === 2) {
                    $scope.isAdvisorContact = true;
                }
                if ($scope.advisorId !== 0 && adviserTypeId === 6) {
                    $scope.isAdminContact = true;
                }
                if ($scope.advisorId !== 0 && adviserTypeId === 7) {
                    $scope.isDirContact = true;
                }
            },
            function failure(error) {
                console.error(error);
            });
    };
}]);
