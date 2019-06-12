tendaApp.controller('clientsController', ['$scope', 'applicationService', 'clientsService', 'advisorService', '$state', '$stateParams', function ($scope, applicationService, clientsService, advisorService, $state, $stateParams) {

    $scope.clients = [];
    $scope.advisors = [];
    $scope.incorrectEntry = false;

    $scope.goToNewClientPage = function goToNewClientPage() {
        $state.go('newClient');
    };

    $scope.doSearch = function doSearch(searchWhere) {
        $scope.getAdivisors();
        if (!!searchWhere && searchWhere.trim().length >= 4) {
            $scope.getContacts(searchWhere);
        }
    };

    $scope.isClient = function isClient(id) {
        for (i = 0; i < $scope.advisors.length; i++) {
            if (id === $scope.advisors[i].id) {
                return false;
            }
        }

        return true;
    };

    $scope.getAdivisors = function getAdvisors() {
        advisorService.GetAdvisorsAdvisor().then(function succeeded(advisorAdvisor) {
            if (advisorAdvisor.data.length === 0) {
                return;
            }
            $scope.advisors = advisorAdvisor.data;

        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.checkDuplicateIds = function checkDuplicateIds(idNum) {
        for (var i = 0; i < $scope.advisors.length; i++) {
            if ($scope.advisors[i].contact.idNumber === idNum) {
                $scope.duplicateId = true;
                break;
            }
            else {
                $scope.duplicateId = false;
            }
        }
    };

    // For Member IDs 
    $scope.checkDuplicateMemberIds = function checkDuplicateMemberIds(memberId) {
        for (var i = 0; i < $scope.advisors.length; i++) {
            if ($scope.advisors[i].contact.memberId === memberId) {
                $scope.duplicateId = true;
                break;
            }
            else {
                $scope.duplicateId = false;
            }
        }
    };

    //getCompanies
    $scope.getContacts = function getContacts(searchQuery) {
        clientsService.GetBasicContact(searchQuery).then(function succeeded(contact) {


            $scope.contact = contact.data;
            $scope.foundClient = false;
            for (var i = 0; i < $scope.notCompanyContacts.length; i++) {
                if ($scope.notCompanyContacts[i].id === $scope.contact[0].id && $scope.isClient($scope.contact[0].id)) {
                    $scope.application.client = $scope.contact[0];
                    $scope.foundClient = true;
                    break;
                }
            }
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    advisorService.GetAdvisorsNotCompany().then(function succeeded(success) {
        $scope.notCompanyContacts = success.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });


    $scope.clientClicked = function clientClicked(id) {
        $state.go('basicClientDetails', { 'contact': id });
    };
}]);
