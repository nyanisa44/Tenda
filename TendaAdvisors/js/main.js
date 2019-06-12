// <reference path="main.js" />

var tendaApp = angular.module('tendaApp', ['ui.router', 'ngStorage', 'angular-loading-bar']);
//tendaApp = angular.module('tendaApp', ['angular-loading-bar', 'ngAnimate'])

var currentPage = 0;


tendaApp.config(['cfpLoadingBarProvider', function (cfpLoadingBarProvider) {
    cfpLoadingBarProvider.includeSpinner = false;

}]);
//configure routing
//we're using ui-router. see https://git.io/vKlAd
tendaApp.config(['$stateProvider', '$locationProvider', function ($stateProvider, $locationProvider) {

    $stateProvider
         .state('about', {
             url: '/',
             templateUrl: 'templates/about.html',
             controller: 'aboutController'
         })
        .state('company', {
            url: '/company',
            templateUrl: 'templates/company.html',
            controller: 'companyController'
        })
         .state('companyDetail', {
             url: '/company/companyDetail',
             templateUrl: 'templates/companyDetail.html',
             controller: 'companyDetailController',
             params: { 'advisor': null }
         })
        .state('dashboard', {
            url: '/dashboard',
            templateUrl: 'templates/dashboard.html',
            controller: 'dashboardController'
        })
        .state('login', {
            url: '/login',
            templateUrl: 'templates/login.html',
            controller: 'loginController'
        })
        .state('application', {
            url: '/application',
            templateUrl: 'templates/application.html',
            controller: 'applicationController'
        })
        .state('commission', {
            url: '/commission',
            templateUrl: 'templates/commission.html',
            controller: 'commissionController'
        })
        .state('applicationDetail', {
            url: '/application/applicationDetail/',
            views: {
                '': {
                    templateUrl: 'templates/applicationDetail.html'
                },
                'clientDetails@applicationDetail': {
                    templateUrl: 'templates/newApplication/clientDetails.html'
                },
                'products@applicationDetail': {
                    templateUrl: 'templates/newApplication/products.html'
                },
                'documents@applicationDetail': {
                    templateUrl: 'templates/newApplication/documents.html'
                },
                'queries@applicationDetail': {
                    templateUrl: 'templates/queries.html',
                    controller: 'queriesController'
                }
            },
            params: { 'application': null }
        })
        .state('newApplication', {
            url: '/application/newApplication',
            views: {
                '': {
                    templateUrl: 'templates/newApplication.html'
                },
                'checkExistingClient@newApplication': {
                    templateUrl: 'templates/newApplication/checkExistingClient.html'
                },
                'clientDetails@newApplication': {
                    templateUrl: 'templates/newApplication/clientDetails.html'
                },
                'products@newApplication': {
                    templateUrl: 'templates/newApplication/products.html'
                },
                'documents@newApplication': {
                    templateUrl: 'templates/newApplication/documents.html'
                },
                'summary@newApplication': {
                    templateUrl: 'templates/newApplication/summary.html'
                }
            }
        })
        .state('registerAdvisor', {
            url: '/registerAdvisor',
            templateUrl: 'templates/registerAdvisor.html',
            controller: 'registerAdvisorController'
          
    
        })
          .state('registerCompany', {
              url: '/registerCompany',
              templateUrl: 'templates/registerCompany.html',
              controller: 'registerCompanyController'

          })
        .state('advisor', {
            url: '/advisor/:advisorId/:section',
            templateUrl: 'templates/advisor.html',
            controller: 'advisorController',
            views: {
                '': {
                    templateUrl: 'templates/advisor.html'
                },
                'detail@advisor': {
                    templateUrl: 'templates/advisor/detail.html'
                }
            },
            params: { 'advisor': null }
        })
        .state('advisors', {
            url: '/advisors',
            templateUrl: 'templates/advisors.html',
            controller: 'advisorsController'
        })
        .state('profile', {
            url: '/profile',
            templateUrl: 'templates/profile.html',
            controller: 'profileController',
            params: { 'advisor': 0 }
        })
        .state('clients', {
            url: '/clients',
            templateUrl: 'templates/clients.html',
            controller: 'clientsController'

        })
         .state('newClient', {
             url: '/newClient',
             templateUrl: 'templates/newClient.html',
             controller: 'newClient'

         })
        .state('basicClientDetails', {
            url: '/basicClientDetails',
            templateUrl: 'templates/basicClientDetails.html',
            controller: 'basicClientDetailsController',
            params: { 'contact': null }

        })
        .state('admin', {
            url: '/admin/',
            views: {
                '': {
                    templateUrl: 'templates/admin.html'
                },
                'import@admin': {
                    templateUrl: 'templates/admin/import.html'
                },
                'process@admin': {
                    templateUrl: 'templates/admin/process.html'
                },
                'suppliersProducts@admin': {
                    templateUrl: 'templates/admin/suppliersProducts.html'
                },
                'settings@admin': {
                    templateUrl: 'templates/admin/settings.html'
                },
                'users@admin': {
                    templateUrl: 'templates/admin/users.html'
                }
            },
            params: { 'admin': null }
        });

    $locationProvider.html5Mode(true);
}]);

tendaApp.run(['$rootScope', '$state', 'authService', function ($rootScope, $state, authService) {

    $rootScope.$on('$stateChangeStart', function (event, toState, toParams) {
        var loggedIn = !!authService.currentuser;

        $rootScope.Supervisor = JSON.parse(sessionStorage.getItem("Supervisor"));
        $rootScope.AdvisorLog = JSON.parse(sessionStorage.getItem("AdvisorLog"));
        $rootScope.LoggedIn = JSON.parse(sessionStorage.getItem("LoggedIn"));

        if (sessionStorage.getItem("ID") !== "undefined")
        {
            $rootScope.UserID = JSON.parse(sessionStorage.getItem("ID"));
        }
        //if the user is not logged in go to login page instead
        if (!$rootScope.LoggedIn) {
            if (toState.controller !== "loginController")
            {
                event.preventDefault();
                $state.go('login');
                currentPage = 5;//login state
            }
        }
        else if (toState.controller === "dashboardController") {
            currentPage = 0;//dashboard state
        }
        else if (toState.controller === "applicationController") {
            currentPage = 1;//application state
        }
        else if (toState.controller === "applicationDetailController") {
            //currentPage = 1;//application state
        }
        else if (toState.controller === "newApplicationController") {
            currentPage = 1;//application state
        }
        
        else if (toState.controller === "newClient") {
            currentPage = 13;//application state
        }

        else if (toState.controller === "basicClientDetailsController") {
            currentPage = 14;//application state
        }
        else if (toState.controller === "registerAdvisorController") {//Just make sure about this
            //currentPage = 1;//register advisor state
        }
        else if (toState.controller === "commissionController") {
            currentPage = 2;//commission state
        }
        else if (toState.controller === "profileController") {
            currentPage = 3;//profile state
        }
        else if (toState.controller === "advisorsController") {
            currentPage = 6;//advisors list page
        }
        else if (toState.controller === "adminController") {
            currentPage = 7;//admin state
        }
        else if (toState.controller === "advisorController") {
            currentPage = 8;//advisor detail page
        }
        else if (toState.controller === "clientsController") {
            currentPage = 9;//advisor detail page
        }
        else if (toState.controller === "aboutController") {
            currentPage = 10;//advisor detail page
        }
        else if (toState.controller === "companyController") {
            currentPage = 11;//advisor detail page
        }
        else if (toState.controller === "registerCompanyController") {//Just make sure about this
            //currentPage = 1;//register advisor state
        }
        else if (toState.controller === "companyDetailController") {//Just make sure about this
            //currentPage = 1;//register advisor state
        }
    });
}]);

//all other controllers will reside inside this controller
tendaApp.controller('mainController', ['$scope', '$state', 'authService', 'valuesService', function ($scope, $state, authService, valuesService) {
    $scope.logout = function logout() {
        currentPage = 5;
        authService.logout();

        sessionStorage.setItem("Supervisor", "false");
        sessionStorage.setItem("AdvisorLog", "false");
        sessionStorage.setItem("LoggedIn", "false");
        sessionStorage.setItem("ID", undefined);

        $state.go('login');
        linkClicked();
    };

    $scope.testApi = function testApi() {
        console.log("making api calls");
        //this chains a bunch of api calls
        valuesService.getAll().then(function (data) {
            console.log("first call returned", data);
            valuesService.add('hello').then(function (data) {
                console.log("second call returned", data);
                if (data.id) {
                    valuesService.getAll().then(function (data) {
                        console.log("third call returned", data);
                        valuesService.clear().then(function (data) {
                            console.log("fourth call returned", data);
                            valuesService.getAll().then(function (data) {
                                console.log("fifth call returned", data);
                            });
                        });
                    });
                }
            });
        });
    };

    $scope.refresh = function refresh() {
        $state.reload();
    };

    $scope.dashboard = function dashboard() {
        currentPage = 0;
        $state.go('dashboard');
        linkClicked();
    };

    $scope.application = function application() {
        currentPage = 1;
        $state.go('application');
        linkClicked();
    };

    $scope.commission = function commission() {
        currentPage = 2;
        $state.go('commission');
        linkClicked();
    };

    $scope.registerAdvisor = function registerAdvisor() {
        currentPage = 1;
        $state.go('registerAdvisor');
        linkClicked();
    };

    $scope.registerCompany = function registerCompany() {
        currentPage = 1;
        $state.go('registerCompany');
        linkClicked();
    };

    $scope.profile = function profile() {
        currentPage = 3;
        $state.go('profile');
        linkClicked();
    };

    //advisor detail
    $scope.advisor = function advisor() {
        currentPage = 8;
        $state.go('advisor');
        linkClicked();
    };

    //advisors list
    $scope.advisors = function advisors() {
        currentPage = 6;
        $state.go('advisors');
        linkClicked();
    };

    $scope.admin = function admin() {
        currentPage = 7;
        $state.go('admin');
        linkClicked();
    };

    $scope.about = function about() {
        currentPage = 10;
        $state.go('about');
        linkClicked();
    };

    $scope.company = function company() {
        currentPage = 11;
        $state.go('company');
        linkClicked();
    };

    $scope.companyDetail = function companyDetail() {
        currentPage = 1;
        $state.go('companyDetail');
        linkClicked();
    };

    //clients list
    $scope.clients = function clients() {
        currentPage = 9;
        $state.go('clients');
        linkClicked();
    };

    //clients list
    $scope.newClient = function newClient() {
        currentPage = 13;
        $state.go('newClient');
        linkClicked();
    };

    $scope.basicClientDetails = function basicClientDetails() {
        currentPage = 14;
        $state.go('basicClientDetails');
        linkClicked();
    };

    $scope.currentSelectedPage = function currentSelectedPage(n) {
        if (currentPage === n)
            return "currentSelectedPage";
        return "";
    };
}]);


tendaApp.directive('fileDropzone', function () {
    return {
        restrict: 'A',
        scope: {
            file: '=',
            fileName: '='
        },
        link: function (scope, element, attrs) {
            var checkSize, isTypeValid, processDragOverOrEnter, validMimeTypes;
            processDragOverOrEnter = function (event) {
                if (event !== null) {
                    event.preventDefault();
                }
               
                event.originalEvent.dataTransfer.effectAllowed = 'copy';
                return false;
            };
            validMimeTypes = attrs.fileDropzone;
            checkSize = function (size) {
                var _ref;
                if ((_ref = attrs.maxFileSize === void 0 || _ref === '') || (size / 3072) / 3072 < attrs.maxFileSize) {
                    return true;
                } else {
                    alert("File must be smaller than " + attrs.maxFileSize + " MB");
                    return false;
                }
            };
            isTypeValid = function (type) {
                if ((validMimeTypes === void 0 || validMimeTypes === '') || validMimeTypes.indexOf(type) > -1) {
                    return true;
                } else {
                    alert("Invalid file type.  File must be one of following types " + validMimeTypes);
                    return false;
                }
            };
            element.bind('dragover', processDragOverOrEnter);
            element.bind('dragenter', processDragOverOrEnter);
            return element.bind('drop', function (event) {
                var file, name, reader, size, type;
                if (event !== null) {
                    event.preventDefault();
                }
                reader = new FileReader();
                reader.onload = function (evt) {
                    if (checkSize(size) && isTypeValid(type)) {
                        return scope.$apply(function () {
                            scope.file = evt.target.result;
                            return scope.fileName = name;
                        });
                    }
                };
                file = event.originalEvent.dataTransfer.files[0];
                name = file.name;
                type = file.type;
                size = file.size;
                reader.readAsDataURL(file);
                return false;
            });
        }
    };
});


tendaApp.directive('datepicker', function () {
    return function ($scope, element) {
        element.datepicker({ dateFormat: "yy-mm-dd" });
    };
});

tendaApp.directive('rrValidateSaid', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, mCtrl) {
            function saidValidation(value) {
                console.log('SAID: ' + value);
                var isValid = validateSAIDNumber(value);
                console.log('SAID Valid: ' + isValid);
                if (isValid) {
                    mCtrl.$setValidity('said', true);
                } else {
                    mCtrl.$setValidity('said', false);
                }
                return value;
            }
            mCtrl.$parsers.push(saidValidation);
        }
    };
});

tendaApp.directive('ngMatch', function () {
    return {
        restrict: 'A',
        require: '?ngModel',
        link: function (scope, elem, attrs, ngModel) {
            if (!ngModel) return;

            function validate() {
                // values
                var val1 = ngModel.$viewValue;
                var val2 = scope.$eval(attrs.ngMatch);

                // set validity
                ngModel.$setValidity('ngMatch', !val1 || !val2 || val1 === val2);
            }

            //watch own value and re-validate on change
            scope.$watch(attrs.ngModel, validate);
            scope.$watch(attrs.ngMatch, validate);
        }
    };
});

function linkClicked() {
    $("#bs-example-navbar-collapse-1").slideUp();
}

Date.prototype.yyyymmdd = function (padding) {
    var mm = this.getMonth() + 1; // getMonth() is zero-based
    var dd = this.getDate();

    return [this.getFullYear(), (mm < 10 ? '0' : '') + mm, (dd < 10 ? '0' : '') + dd].join(padding); // padding
};

function validateSAIDNumber (idnum) {
    //http://geekswithblogs.net/willemf/archive/2005/10/30/58561.aspx
    //Left by simon on Apr 28, 2009 5:09 AM
    idnum = idnum.toString().replace(" ", "");
    r = /^\d{10}[0-1]\d{2}$/;
    if (! r.test(idnum)) return false;
    n = idnum;
    p1 = parseInt(n[0],10) + parseInt(n[2],10) + parseInt(n[4],10) + parseInt(n[6],10) + parseInt(n[8],10) + parseInt(n[10],10);
    p2 = (parseInt(n[1] + n[3] + n[5] + n[7] + n[9] + n[11],10) * 2).toString();
    p3 = 0;
    for (i=0; i < p2.length; i++) {
        p3+= parseInt(p2[i]);
    }
    check = 10 - (p1 + p3).toString()[(p1 + p3).toString().length -1];
    check_char = check > 9 ? check.toString()[1] : check.toString();
    if (check_char !== idnum[12]) return false;
    return true;
}
