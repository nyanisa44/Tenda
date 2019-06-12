
(function () {
    var service;

    tendaApp.factory('detailsService', ['$http', '$q', function ($http, $q) {
        function formTransform(obj) {
            var str = [];
            for (var p in obj) str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
            return str.join("&");
        }

        if (!service) {
            service = {
                GetAddressTypes: function GetAddressTypes() {
                    return $q(function (resolve, reject) {
                        $http.get('api/AddressTypes/').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },
                
                DeleteProductFromApplication: function DeleteProductFromApplication(productId, applicationId) {
                    return $q(function (resolve, reject) {
                        $http.delete('api/Products/'+ productId +'/Application/'+ applicationId).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetProvinces: function GetProvinces() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Provinces/').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },


              
                GetContactTitles: function GetContactTitles() {
                        return $q(function (resolve, reject) {
                            $http.get('api/ContactTitles').then(function (d) {
                                resolve(d);
                            }, function (d) {
                                reject(d);
                            });
                        });
                    },



                GetBankName: function GetBankName() {
                    return $q(function (resolve, reject) {
                        $http.get('api/BankNames').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },


                GetBankBranchCodes: function  () {
                    return $q(function (resolve, reject) {
                        $http.get('api/BankBranchCodes').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

      
                GetCountries: function GetCountries() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Countries/').then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                }
            };
        }

        return service;
    }]);


    tendaApp.config(['$httpProvider', function ($httpProvider) {

        $httpProvider.interceptors.push(['$q', '$injector', function ($q, $injector) {
            return {
                'request': function (config) {

                    var detailsService = $injector.get('detailsService');
                    return config;
                },

                'responseError': function (rejection) {
                    return $q.reject(rejection);
                }
            };
        }]);
    }]);

})();