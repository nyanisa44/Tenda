tendaApp.controller('profileController', ['$scope', 'advisorService', 'detailsService', '$state', function ($scope, advisorService,detailsService, $state) {
    $scope.emailRegex = /^\S+@\S+$/;

   if ($state.params.advisor === null) {
        $state.go('profile');
        return;
    }

   $scope.advisor = {};
   $scope.origAdvisor = {};
   $scope.cantEdit = false;
   $scope.notAdvisorEditing = true;

   advisorService.GetAdvisorProfile($state.params.advisor).then(function succeeded(advisor) {
        $scope.advisor = advisor.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    //Client Text Changed Event:
    $scope.advisorTextChange = function advisorTextChange(theText) {

    };

    //Start the editing process:
    $scope.editAdvisorPageAndSave = function editAdvisorPageAndSave(save) {
        // Save is when you stop editing.
        $scope.notAdvisorEditing = !$scope.notAdvisorEditing;
        $scope.origAdvisor = angular.copy($scope.advisor);

        if (save) {
            for (var i = 0; i < $scope.TitlesTemp.length; i++) {
                if ($scope.TitlesTemp[i].name === $scope.advisor.contactTitle_Name) {
                    $scope.advisor.ContactTitle_Id = $scope.TitlesTemp[i].id;
                    break;
                }
            }

            advisorService.PutAdvisorProfile($scope.advisor.id, $scope.advisor).then(function succeeded(success) {
                console.log(success);
            }, function failure(errormessage) {
                console.log(errormessage);
                });
            $scope.cantEdit = true;
        }
        $scope.cantEdit = true;
    };

    $scope.checkCharaters = function checkCharaters(characters) {
        if (!/^[a-zA-Z ]+$/.test(characters) || characters.length > 50) {
            $scope.incorrectEntry = true;
        }
        else {
            $scope.incorrectEntry = false;
        }
    };

    //Get Title
    detailsService.GetContactTitles().then(function succeeded(success) {
        $scope.TitlesTemp = success.data;
        $scope.Titles = [];
        success.data.forEach(function (item, index) { $scope.Titles.push(item.name); });
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    $scope.cancelEdit = function () {
        $scope.advisor = angular.copy($scope.origAdvisor);
        $scope.origAdvisor = {};
        $scope.notAdvisorEditing = true;
    };
}]);
