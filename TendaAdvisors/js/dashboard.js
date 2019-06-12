tendaApp.controller('dashboardController', ['$scope', 'applicationService', 'advisorService', 'clientsService', '$state', function ($scope, applicationService, advisorService, clientsService, $state) {

    $scope.AdvisorDocuments = {};
    $scope.moveToCompleteSuccess = false;
    //Change between pages
    $scope.registerItem = 0;
    $scope.gotoPage = function gotoPage(page) {
        if (page === 0 ||
            page === 1 ||
            page === 2 ||
            page === 3 ||
            page === 4 ||
            page === 5) {
            $scope.registerItem = page;
        }
        else {
            $scope.registerItem = page;
            $state.go('dashboard');
        }
    };

    $scope.viewAll = function viewAll() {
        console.log('ViewAll');
    };

    //DONE ONLY SHOWS ADVISORS IN DASHBOARD-SERVER CHANGE,SETS WHO LOGGED IN ETC.
    applicationService.GetDashboard().then(function succeeded(success) {
        $scope.dashboard = success;
        getAdvisorDocuments();
        //Need to find a better place to do this

    }, function failure(errormessage) {
        console.log(errormessage);
    });


    //COMPLETE APPLICATIONS AND ADVISORS FOR THAT APPLICATION
    applicationService.GetCompleteApplications().then(function succeeded(applicationComplete) {

        if (applicationComplete.data.length === 0) {
            return;
        }

        if ($scope.dashboard.advisor.id !== null) {

            applicationService.GetCompleteApplicationsForAdvisor($scope.dashboard.advisor.id).then(function succeeded(applicationComplete) {

                if (applicationComplete.data.length === 0) {
                    return;
                }

                $scope.applicationCompleteForAdvisor = applicationComplete.data;
                $scope.applicationCompleteCountForAdvisor = applicationComplete.data.length;
            }, function failure(errormessage) {
                console.log(errormessage);
            });
        }

        $scope.applicationComplete = applicationComplete.data;
        $scope.applicationCompleteCount = applicationComplete.data.length;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    //Pending APPLICATIONS AND ADVISORS FOR THAT APPLICATION
    applicationService.GetPendingApplications().then(function succeeded(applicationPending) {

        if (applicationPending.data.length === 0) {
            return;
        }
        $scope.applicationPending = applicationPending.data;
        $scope.applicationPendingCount = applicationPending.data.length;
        if ($scope.dashboard.advisor.id !== null) {
            applicationService.GetPendingApplicationsForAdvisor($scope.dashboard.advisor.id).then(function succeeded(applicationPending) {

                if (applicationPending.data.length === 0) {
                    return;
                }

                $scope.applicationPendingForAdvisor = applicationPending.data;
                $scope.applicationPendingCountForAdvisor = applicationPending.data.length;
            }, function failure(errormessage) {
                console.log(errormessage);
            });
        }
    }, function failure(errormessage) {
        console.log(errormessage);
    });


    //New APPLICATIONS AND ADVISORS FOR THAT APPLICATION
    applicationService.GetNewApplications().then(function succeeded(applicationNew) {
        if (applicationNew.data.length === 0) {
            return;
        }
        $scope.applicationNew = applicationNew.data;
        $scope.applicationNewCount = applicationNew.data.length;
        if ($scope.dashboard.advisor.id !== null) {
            applicationService.GetNewApplicationsForAdvisor($scope.dashboard.advisor.id).then(function succeeded(applicationNew) {

                if (applicationNew.data.length === 0) {
                    return;
                }
                $scope.applicationNewForAdvisor = applicationNew.data;
                $scope.applicationNewCountForAdvisor = applicationNew.data.length;
            }, function failure(errormessage) {
                console.log(errormessage);
            });
        }
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    function getAdvisorDocuments() {
        advisorService.GetAdvisorDocuments().then(function succeeded(success) {
            $scope.AdvisorDocuments = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });

        advisorService.GetAdvisorDocumentsForAdvisor($scope.dashboard.advisor.id).then(function succeeded(AdviExpired) {
            $scope.AdvisorDocumentsForAdvisor = AdviExpired.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    }

    $scope.calculateSumExpired = function calculateSumExpired(data) {
        if (typeof data === "undefined") {
            return 0;
        }

        var countExpired = 0;
        for (var i = 0; i < data.length; i++) {
            if (data[i].expired)
                countExpired++;
        }
        return countExpired;
    },

    $scope.calculateSumExpiredAdvisor = function calculateSumExpiredAdvisor(data) {
        if (typeof data === "undefined") {
            return 0;
        }

        var countExpired = 0;
        for (var i = 0; i < data.length; i++) {
            if (data[i].expired)
                countExpired++;
        }
        return countExpired;
    },

    $scope.adjustExpiry = function adjustExpiry(docID, status) {
        advisorService.PutchangeIsExpired(docID, status).then(function succeeded(success) {
            console.log(success);
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    },

    $scope.applicationClicked = function applicationClicked(id) {
        //Go to individual application detail:
        $state.go('applicationDetail', { 'application': id });
    },

    $scope.applicationCompleted = function applicationCompleted(applicationId, applicationTypeId) {
        // Application status complete === 6
        applicationService.GetApplicationDocumentsByApplicationId(applicationId).then(function succeeded(success) {
            var applicationDocuments = success.data;
            if (applicationDocuments !== null) {
                var appDoc = null;
                var hasApplicationForm = false;
                var hasAdviceRecord = false;
                var hasDisclosureDocument = false;
                var hasBrokerNote = false;
                var hasGroupAppointment = false;
                var hasVoiceLoggedScript = false;
                var applicationCompleteStatus = 6;
                var elementId = 'card' + applicationId;
                var moveToCompleteError = 'Cannot move to complete, missing the following document(s): ';
                var moveToCompleteSuccessMessage = 'Successfully moved';

                if (applicationDocuments.length === 0) {
                    document.getElementById(elementId).innerHTML = 'Application has no uploaded documents'; 
                    $scope.moveToCompleteSuccess = false;
                }

                if (applicationTypeId === 1) {
                    for (var a = 0; a < applicationDocuments.length; a++) {
                        appDoc = applicationDocuments[a];

                        if (appDoc.documentTypeId === 37) {
                            hasApplicationForm = true;
                        }
                        else if (appDoc.documentTypeId === 38) {
                            hasAdviceRecord = true;
                        }
                        else if (appDoc.documentTypeId === 39) {
                            hasDisclosureDocument = true;
                        }
                    }

                    if (hasApplicationForm && hasAdviceRecord && hasDisclosureDocument) {
                        applicationService.PutApplicationStatus(applicationId, applicationCompleteStatus);
                        $scope.moveToCompleteSuccess = true;
                    }
                    else {
                        moveToCompleteError += hasApplicationForm ? '' : `-- Application form \n`;
                        moveToCompleteError += hasAdviceRecord ? '' : `-- Advice record  \n`;
                        moveToCompleteError += hasDisclosureDocument ? '' : `-- Disclosure document \n`;
                        $scope.moveToCompleteSuccess = false;
                    }
                }
                else if (applicationTypeId === 2) {
                    for (var b = 0; b < applicationDocuments.length; b++) {
                        appDoc = applicationDocuments[b];

                        if (appDoc.documentTypeId === 7) {
                            hasBrokerNote = true;
                        }
                    }

                    if (hasBrokerNote) {
                        $scope.putApplicationStatus(applicationId, applicationCompleteStatus);
                        $scope.moveToCompleteSuccess = true;
                    }
                    else {
                        $scope.moveToCompleteError += `-- Broker Note \n`;
                        document.getElementById(elementId).innerHTML = moveToCompleteError;
                        $scope.moveToCompleteSuccess = false;
                    }
                }
                else if (applicationTypeId === 3) {
                    for (var c = 0; c < applicationDocuments.length; c++) {
                        appDoc = applicationDocuments[c];

                        if (appDoc.documentTypeId === 40) {
                            hasGroupAppointment = true;
                        }
                    }

                    if (hasGroupAppointment) {
                        $scope.putApplicationStatus(applicationId, applicationCompleteStatus);
                        $scope.moveToCompleteSuccess = true;
                    }
                    else {
                        $scope.moveToCompleteError += `-- Group Appointment \n`;
                        document.getElementById(elementId).innerHTML = moveToCompleteError;
                        $scope.moveToCompleteSuccess = false;
                    }
                } 
                else if (applicationTypeId === 4) {
                    for (var d = 0; d < applicationDocuments.length; d++) {
                        appDoc = applicationDocuments[d];

                        if (appDoc.documentTypeId === 37) {
                            hasApplicationForm = true;
                        }

                        if (appDoc.documentTypeId === 40) {
                            hasGroupAppointment = true;
                        }
                    }

                    if (hasApplicationForm && hasGroupAppointment) {
                        $scope.putApplicationStatus(applicationId, applicationCompleteStatus);
                        $scope.moveToCompleteSuccess = true;
                    }
                    else {
                        $scope.moveToCompleteError += hasApplicationForm ? '' : `-- Application Form \n`;
                        $scope.moveToCompleteError += hasGroupAppointment ? '' : `-- Group Appointment \n`;
                        document.getElementById(elementId).innerHTML = moveToCompleteError;
                        $scope.moveToCompleteSuccess = false;
                    }
                }
                else if (applicationTypeId === 5) {
                    for (var e = 0; e < applicationDocuments.length; e++) {
                        appDoc = applicationDocuments[e];

                        if (appDoc.documentTypeId === 42) {
                            hasVoiceLoggedScript = true;
                        }
                    }
                    
                    if (hasVoiceLoggedScript) {
                        $scope.putApplicationStatus(applicationId, applicationCompleteStatus);
                        $scope.moveToCompleteSuccess = true;
                    }
                    else {
                        $scope.moveToCompleteError += `-- Voice logged script \n`;
                        document.getElementById(elementId).innerHTML = moveToCompleteError;
                        $scope.moveToCompleteSuccess = false;
                    }
                }
                else if (applicationTypeId === 6) {
                    for (var f = 0; f < applicationDocuments.length; f++) {
                        appDoc = applicationDocuments[f];

                        if (appDoc.documentTypeId === 37) {
                            hasApplicationForm = true;
                        }

                        if (appDoc.documentTypeId === 38) {
                            hasAdviceRecord = true;
                        }

                        if (appDoc.documentTypeId === 39) {
                            hasDisclosureDocument = true;
                        }
                    }

                    if (hasApplicationForm && hasAdviceRecord && hasDisclosureDocument) {
                        applicationService.putApplicationStatus(applicationId, applicationCompleteStatus);
                        $scope.moveToCompleteSuccess = true;
                    }
                    else {
                        moveToCompleteError += hasApplicationForm ? '' : `-- Application form \n`;
                        moveToCompleteError += hasAdviceRecord ? '' : `-- Advice record  \n`;
                        moveToCompleteError += hasDisclosureDocument ? '' : `-- Disclosure document \n`;
                        document.getElementById(elementId).innerHTML = moveToCompleteError;
                        $scope.moveToCompleteSuccess = false;
                    }
                }
                else if (applicationTypeId === 7) {
                    for (var g = 0; g < applicationDocuments.length; g++) {
                        appDoc = applicationDocuments[g];

                        if (appDoc.documentTypeId === 7) {
                            hasBrokerNote = true;
                        }
                    }

                    if (hasBrokerNote) {
                        $scope.putApplicationStatus(applicationId, applicationCompleteStatus);
                        $scope.moveToCompleteSuccess = true;
                    }
                    else {
                        $scope.moveToCompleteError += `-- Broker Note \n`;
                        document.getElementById(elementId).innerHTML = moveToCompleteError;
                        $scope.moveToCompleteSuccess = false;
                    }
                }

                if ($scope.moveToCompleteSuccess) {
                    document.getElementById(elementId).innerHTML = moveToCompleteSuccessMessage;
                    // Reload the new documents
                    applicationService.GetNewApplications().then(function succeeded(applicationNew) {
                        if (applicationNew.data.length === 0) {
                            return;
                        }
                        $scope.applicationNew = applicationNew.data;
                        $scope.applicationNewCount = applicationNew.data.length;
                        if ($scope.dashboard.advisor.id !== null) {
                            applicationService.GetNewApplicationsForAdvisor($scope.dashboard.advisor.id).then(function succeeded(applicationNew) {

                                if (applicationNew.data.length === 0) {
                                    return;
                                }
                                $scope.applicationNewForAdvisor = applicationNew.data;
                                $scope.applicationNewCountForAdvisor = applicationNew.data.length;
                            }, function failure(errormessage) {
                                console.log(errormessage);
                            });
                        }
                    }, function failure(errormessage) {
                        console.log(errormessage);
                    });
                }
                else {
                    document.getElementById(elementId).innerHTML = moveToCompleteError;
                }
            }
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    },

    $scope.applicationRemoved = function applicationRemoved(id) {
        applicationService.DeleteApplication(id).then(function succeeded(success) {
            if (success) {
                // Reload the new documents
                applicationService.GetNewApplications().then(function succeeded(applicationNew) {
                    if (applicationNew.data.length === 0) {
                        return;
                    }
                    $scope.applicationNew = applicationNew.data;
                    $scope.applicationNewCount = applicationNew.data.length;
                    if ($scope.dashboard.advisor.id !== null) {
                        applicationService.GetNewApplicationsForAdvisor($scope.dashboard.advisor.id).then(function succeeded(applicationNew) {

                            if (applicationNew.data.length === 0) {
                                return;
                            }
                            $scope.applicationNewForAdvisor = applicationNew.data;
                            $scope.applicationNewCountForAdvisor = applicationNew.data.length;
                        }, function failure(errormessage) {
                            console.log(errormessage);
                        });
                    }
                }, function failure(errormessage) {
                    console.log(errormessage);
                });
            }
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    },

    $scope.calculateSum = function calculateSum(data, applicationStatus, advisorStatus) {
        if (typeof data === "undefined") {
            return 0;
        }

        var count = 0;
        for (var i = 0; i < data.length; i++) {
            if (data[i].applicationStatus.id === applicationStatus && data[i].advisor.advisorStatus.id === advisorStatus) {
                count++;
            }
        }
        return count;
    },


    $scope.AdvisorAppClicked = function AdvisorAppClicked(id) {
        advisorService.GetAdvisorDocumentFile(id).then(function succeeded(success) {
            console.log(success);
        }, function failure(errormessage) {
            console.log(errormessage);
        }
        );
    },

    $scope.filterStatus = function filterStatus(status) {
        alert(status);
    },

    $scope.allApplicationsClicked = function allApplicationsClicked() {
        $state.go('application');
    };
}]);
