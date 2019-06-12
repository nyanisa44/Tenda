tendaApp.controller('companyController', ['$scope', 'clientsService', 'advisorService', 'applicationService', '$state', '$stateParams', function ($scope, clientsService, advisorService, applicationService, $state, $stateParams) {

    $scope.advisors = { "contact": { "addresses": [{ "id": 0 }] } };

    $scope.doSearch = function doSearch(searchWhere) {
        if (!!searchWhere && searchWhere.trim().length >= 4)
            $scope.getContacts(searchWhere);
    };

    $scope.getContacts = function getContacts(searchQuery) {
        clientsService.GetBasicContactCompany(searchQuery).then(function succeeded(contact) {
            $scope.contact = contact.data;
            $scope.advisor.contact = $scope.contact[0];
            $scope.isAdvisorContact = false;
            for (var i = 0; i < $scope.advisors.length; i++) {
                if ($scope.advisors[i].id === $scope.advisor.contact.id) {
                    $scope.isAdvisorContact = true;
                    break;
                }
            }

        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    // Gets advisers of Adviser type Company (id=5)
    advisorService.GetAdvisorsCompany().then(function succeeded(advisorCompanies) {
        console.log(advisorCompanies);
        if (advisorCompanies.data.length === 0) {
            console.log("There are no companies loaded");
            return;
        }
        // Advisor contacts are returned
        $scope.advisors = advisorCompanies.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // Go to company detail page
    $scope.advisorClickedGetId = function advisorClickedGetId(id) {
        // Go to individual company detail:
        advisorService.GetAdvisorCompanyId(id).then(function succeeded(advisorCompanies) {
            $scope.advisorsId = advisorCompanies.data;
            $state.go('companyDetail', { 'advisor': $scope.advisorsId });
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    // Register a new Company
    $scope.goToAdvisorsPage = function goToAdvisorsPage() {
        $state.go('registerCompany');
    };

    // Delete
    $scope.deleteAdvisor = function deleteAdvisor(advisorId) {
        advisorService.GetAdvisorCompanyId(advisorId).then(function succeeded(advisorCompanies) {
            $scope.advisorsId = advisorCompanies.data;
            $scope.deleteCompany($scope.advisorsId);
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.deleteCompany = function deleteCompany(advisorId) {
        advisorService.DeleteAdvisor(advisorId)
            .then(function succeeded(success) {
                $state.go('company');
            },
                function failure(errormessage) {
                    console.log(errormessage);
                    alert("Not Deleted");
                });
    };
}]);
