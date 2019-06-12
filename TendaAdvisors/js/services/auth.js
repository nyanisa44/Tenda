(function () {
    var service;

    tendaApp.factory('authService', ['$http', '$q', '$localStorage', function ($http, $q, $localStorage) {
        function formTransform(obj) {
            var str = [];
            for (var p in obj) str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
            return str.join("&");
        }

        if (!service) {
            service = {
                logout: function logout() {
                    delete $localStorage.currentuser;
                },

                authorizeRequest: function authorizeRequest(config) {
                    var cu = $localStorage.currentuser;
                    if (cu) {
                        if (!config.headers) config.headers = {};
                        config.headers.Authorization = 'Bearer ' + cu.token.access_token;
                    }
                    return config;
                },

                login: function login(username, password) {
                    return $q(function (resolve, reject) {
                        $http({
                            method: 'POST',
                            url: 'api/token',
                            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                            transformRequest: formTransform,
                            data: {
                                'grant_type': 'client_credentials',
                                'client_id': username,
                                'client_secret': password
                            }
                        }).then(function success(response) {

                            if (!response.data || !response.data.access_token) {
                                reject("There was an unknown error during the authentication process. Please try again later");
                                return;
                            }

                            $localStorage.currentuser = {
                                token: response.data,
                                username: username
                            };

                            //return nothing, just say success.
                            resolve();
                        }, function failure(response) {
                            if (response.data) {
                                if (response.data.error === "invalid_client") {
                                    reject("Invalid credentials were supplied");
                                    return;
                                }
                            }
                            reject("There was an unknown error during the authentication process. Please try again later");
                        });
                    });
                },

                GetApplications: function GetApplications() {
                    return $q(function (resolve, reject) {
                        $http({
                            method: 'GET',
                            url: 'api/applications/',
                            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                            transformRequest: formTransform
                        }).then(function success(response) {

                            if (!response.data) {
                                reject("No applications were returned. Please try again later");
                                return;
                            }

                            resolve(response.data);
                        }, function failure(response) {
                            if (response.data) {
                                if (response.data.error == "invalid_client") {
                                    reject("Invalid credentials were supplied");
                                    return;
                                }
                            }
                            reject("There was an unknown error during the get application process. Please try again later");
                        });
                    });
                },

                GetApplication: function GetApplication(id) {
                    return $q(function (resolve, reject) {
                        $http({
                            method: 'GET',
                            url: 'api/applications/' + id,
                            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                            transformRequest: formTransform
                        }).then(function success(response) {

                            if (!response.data) {
                                reject("No applications were returned. Please try again later");
                                return;
                            }

                            resolve(response.data);
                        }, function failure(response) {
                            if (response.data) {
                                if (response.data.error == "invalid_client") {
                                    reject("Invalid credentials were supplied");
                                    return;
                                }
                            }
                            reject("There was an unknown error during the get application process. Please try again later");
                        })
                    });
                },

                GetAdvisers: function GetAdvisers(id) {
                    return $q(function (resolve, reject) {
                        $http({
                            method: 'GET',
                            url: 'api/Advisors',
                            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                            transformRequest: formTransform
                        }).then(function success(response) {

                            if (!response.data) {
                                reject("No applications were returned. Please try again later");
                                return;
                            }

                            resolve(response.data);
                        }, function failure(response) {
                            if (response.data) {
                                if (response.data.error == "invalid_client") {
                                    reject("Invalid credentials were supplied");
                                    return;
                                }
                            }
                            reject("There was an unknown error during the get application process. Please try again later");
                        })
                    });
                },

                //id is an application id, and application is an application:
                PutApplication: function PutApplication(id, application) {
                    /*return $q(function (resolve, reject) {
                        $http({
                            method: 'PUT',
                            url: 'api/Applications/' + id,
                            headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
                            data: application,
                            transformRequest: formTransform
                        }).then(function success(response) {

                            if (!response.data) {
                                reject("No applications were returned. Please try again later");
                                return;
                            }

                            resolve(response.data);
                        }, function failure(response) {
                            if (response.data) {
                                if (response.data.error == "invalid_client") {
                                    reject("Invalid credentials were supplied");
                                    return;
                                }
                            }
                            reject("There was an unknown error during the get application process. Please try again later");
                        })
                    });*/

                    $.ajax({
                        url: 'api/Applications/' + id,
                        type: "PUT",
                        data: JSON.stringify(application),
                        contentType: "application/json"
                    });
                },

                //A whole advisor was created in json
                CreateAdvisor: function CreateAdvisor(advisor) {
                    $.ajax({
                        url: 'api/Advisor',
                        type: "POST",
                        data: JSON.stringify(advisor),
                        contentType: "application/json"
                    });
                }
            };

            Object.defineProperty(service, "currentuser", {
                get: function () { var u = $localStorage.currentuser; return u && u.username; },
                enumerable: true,
                configurable: true
            })
        }

        return service;
    }]);


    tendaApp.config(['$httpProvider', function ($httpProvider) {

        $httpProvider.interceptors.push(['$q', '$injector', function ($q, $injector) {
            return {
                'request': function (config) {

                    var authService = $injector.get('authService');
                    return authService.authorizeRequest(config);
                },

                'responseError': function (rejection) {
                    //TODO: handle authorization fail due to token expiry.
                    // do something on error
                    //if (canRecover(rejection)) {
                    //    return responseOrNewPromise
                    //}
                    return $q.reject(rejection);
                }
            };
        }]);
    }]);

})();