
(function () {
    var service;

    tendaApp.factory('clientsService', ['$http', '$q', '$localStorage', function ($http, $q, $localStorage) {
        function formTransform(obj) {
            var str = [];
            for (var p in obj) str.push(encodeURIComponent(p) + "=" + encodeURIComponent(obj[p]));
            return str.join("&");
        }

        if (!service) {
            service = {
                PostContact: function PostContact(client) {
                    return $q(function (resolve, reject) {
                        $http.post('api/Contacts', client).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetClientsByAdvisorId: function GetClientsByAdvisorId(advisorId, paging) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Clients/' + advisorId + '/Advisor/' + paging.config.pageIndex + '/' + paging.config.pageIndex).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },

                GetCompanies: function GetCompanies(nameSearch) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Companies/' + nameSearch).then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },

                GetContacts: function GetContacts() {
                    return $q(function (resolve, reject) {
                        $http.get('api/Contacts').then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },

                GetBasicContactCompany: function GetBasicContactCompany(nameSearch) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Contacts/Basic/company/'+nameSearch).then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },

                GetContact: function GetContact(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Contacts/'+id).then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },

                GetContactClients: function GetContactClients(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/ContactsClients/' + id).then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },
                
                PutContactClient: function PutContactClient(id,contact) {
                    return $q(function (resolve, reject) {
                        $http.put('api/ContactsClients/put/' + id, contact).then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },

                GetContactDetails: function GetContactDetails(id) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Contacts/Details/' + id).then(function (d) {
                            resolve(d);
                        }, function (d) {
                            reject(d);
                        });
                    });
                },
                
                PutContact: function PutContact(id, client) {
                    return $q(function (resolve, reject) {
                        $http.put('api/Contacts/' + id, client).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    }); 
                },
                
                GetBasicContact: function GetBasicContact(name) {
                        return $q(function (resolve, reject) {
                            $http.get('api/Contacts/Basic/' + name).then(function (co) {
                                resolve(co);
                            }, function (co) {
                                reject(co);
                            });
                        });
                },

                GetAdvisorContact: function GetAdvisorContact(name) {
                    return $q(function (resolve, reject) {
                        $http.get('api/Contacts/Advisor/' + name).then(function (co) {
                            resolve(co);
                        }, function (co) {
                            reject(co);
                        });
                    });
                },

                GetContactTypes: function GetContactTypes() {
                    return $q(function (resolve, reject) {
                        $http.get('api/ContactTypes').then(function (ct) {
                            resolve(ct);
                        }, function (ct) {
                            reject(ct);
                        });
                    });
                },

                GetAdvisorTypes: function GetAdvisorTypes() {
                    return $q(function (resolve, reject) {
                        $http.get('api/AdvisorTypes').then(function (at) {
                            resolve(at);
                        }, function (at) {
                            reject(at);
                        });
                    });
                },

                PutContactMemberNumber: function PutContactMemberNumber(contactId, contactMemberId) {
                    return $q(function (resolve, reject) {
                        $http.put('api/Contacts/UpdateMemberNumber/' + contactId + '/' + contactMemberId).then(function (a) {
                            resolve(a);
                        }, function (a) {
                            reject(a);
                        });
                    });
                }
            }; //End Service

            Object.defineProperty(service, "currentuser", {
                get: function () {
                    var u = $localStorage.currentuser; return u && u.username;
                },
                enumerable: true,
                configurable: true
            });
        }

        return service;
    }]);


    tendaApp.config(['$httpProvider', function ($httpProvider) {

        $httpProvider.interceptors.push(['$q', '$injector', function ($q, $injector) {
            return {
                'request': function (config) {

                    var advisorService = $injector.get('advisorService');
                    return config;
                },

                'responseError': function (rejection) {
                    return $q.reject(rejection);
                }
            };
        }]);
    }]);

})();