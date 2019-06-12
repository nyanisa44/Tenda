tendaApp.controller('loginController', ['$scope', '$state', 'authService', 'applicationService', function ($scope, $state, auth, applicationService)
{
    // Error messages
    const invalid = "Incorrect Username  or Password";
    const empty = "Username and Password cannot be left empty";
    // Error messages

    // $scope fields
    $scope.Model = new Model(["Username", "Password"], function IsValid()
    {
        return (typeof this.Username === "string" && this.Username.trim().length > 0) &&
            (typeof this.Password === "string" && this.Password.trim().length > 0);
    });
    $scope.ErrorMessage = "";
    $scope.LoggingIn = true;
    // $scope fields

    // $scope methods
    $scope.Login = function Login() {
        if ($scope.Model.IsValid) {
            auth.login($scope.Model.Username, $scope.Model.Password)
                .then(function succeeded(token) {
                    $scope.LoggingIn = true;
                    sessionStorage.setItem("LoggedIn", "true");
                    applicationService.GetDashboard().then(function succeeded(success) {
                        sessionStorage.setItem("Supervisor", success.role + "");
                        sessionStorage.setItem("AdvisorLog", success.advisorRole + "");
                        sessionStorage.setItem("ID", success.id + "");
                        $state.go('about');
                    },
                    function failure(error) {
                        $scope.ErrorMessage = error;
                        console.log("Login: failed by get dashboard");
                        $scope.LoggingIn = false;
                    });
                },
                function failure(error) {
                    $scope.ErrorMessage = invalid;
                    console.log("Login: failed after token");
                    console.log(error);
                    $scope.LoggingIn = false;
                });
        }
        else {
            $scope.ErrorMessage = empty;
            console.log("Login: Model is not valid");
            $scope.LoggingIn = false;
        }
    };
    // $scope methods
}]);
