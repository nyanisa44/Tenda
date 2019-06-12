tendaApp.controller('aboutController', ['$scope','advisorService', 'applicationService', '$state', function ($scope, advisorService, applicationService, $state) {

    applicationService.GetDashboard().then(function succeeded(success) {
        $scope.dashboard = success;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

}]);