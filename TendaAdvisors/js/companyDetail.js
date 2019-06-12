tendaApp.controller('companyDetailController', ['$scope', 'detailsService', 'advisorService', 'applicationService', '$state', function ($scope,detailsService, advisorService, applicationService, $state) {

    if ($state.params.advisor == null) {
        $state.go('advisors');
        return;
    }

    if ($scope.registerItem == null) {
        $scope.registerItem = 0;
    }

    $scope.advisor = {};
    $scope.advisorTemp = {  };
    $scope.uploaded = false;
    $scope.notUploaded = false;
    $scope.origAdvisor = {};
    $scope.documentTypeList = [];
    $scope.notAdvisorEditing = true;
    $scope.editingApplicationDetail = false;
    $scope.cantEdit = true;
 

    //Get the details of the adviser (COMPANY)
    $scope.getAdvisor = function getAdvisor() {
        return advisorService.GetAdvisor($state.params.advisor).then(
            function succeeded(advisor) {
                console.log(advisor);
                if (advisor.data && advisor.data.advisorDocuments) {
                    advisor.data.advisorDocuments.forEach(function (document) {
                        if (document && document.documentType == null && document.documentTypeId != null) {
                            document.documentType = { id: document.documentTypeId };
                        }
                    });
                }
                $scope.advisor = advisor.data;
                //$scope.sortedDocs= $scope.advisor.advisorDocuments.sort();
            },
            function failure(errormessage) {
                console.log(errormessage);
                $scope.advisor = null;
            }
        );
    }
    $scope.getAdvisor();


    //GET FOR FSP TYPE 
    advisorService.GetAdvisorTypes().then(function succeeded(advisorTypes) {
        //console.log(queries.data);
        if (advisorTypes.length === 0) {
            console.log("There are no Queries");
            return;
        }

        $scope.advisorTypes = advisorTypes.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    //get License Types
    $scope.getLicenseTypes = function getLicenseTypes(advisorTypeId) {
        advisorService.GetLicenseTypes().then(
            function succeeded(licenseTypes) {
                console.log(licenseTypes);
                $scope.licenseTypes = licenseTypes.data;
            },
            function failure(errormessage) {
                console.log(errormessage);
            }
        );
    }
    $scope.getLicenseTypes($scope.advisor.advisorTypeId);

    //Upload missing documents
 $scope.uploadDocument = function uploadDocument(advisorId, documentTypeID, document, submitted) {

        if (submitted != true) {
            submitted = false;
        }

        //advisorId, documentTypeId and base64 file data (api/AdvisorDocuments/' + advisorId + '/DocumentTypeId/' + documentId)
        advisorService.PostAdvisorDocument(advisorId, documentTypeID, document, submitted)
            .then(function succeeded(success) {
                console.log(success);
                document.uploaded = true;
                document = success.data;
                console.log(document);
            },
           function failure(errormessage) {
               console.log(errormessage);
               document.notUploaded = true;
           });
    }



    //Client Text Changed Event:
 $scope.advisorTextChange = function advisorTextChange(theText) {
     console.log('advisorTextChange');
 }


    //check FirstName characters
    $scope.checkCharaters = function checkCharaters(characters) {
     if (!/^[a-zA-Z ]+$/.test(characters) || characters.length > 50) {
            $scope.incorrectEntry = true;
      }
      else {
            $scope.incorrectEntry = false;
      }

    }


 $scope.advisorClickedAdvisor = function advisorClickedAdvisor(id,addType) {

     //ADVISORIds
    advisorService.GetAdvisorIdOfAdvisorType(id, addType).then(function succeeded(advisorsOfCertainType) {
        console.log(advisorsOfCertainType);
        if (advisorsOfCertainType.data.length == 0) {
            console.log("There are no Advisors loaded");
            return;
        }
        $scope.advisorsOfCertainType = advisorsOfCertainType.data;
        $state.go('advisor', { 'advisor': $scope.advisorsOfCertainType });
    }, function failure(errormessage) {
        console.log(errormessage);
    });

   

 }



 

 $scope.advisorClickedKey = function advisorClickedKey(id,addType) {

     //ADVISORIds
     advisorService.GetAdvisorIdOfAdvisorType(id, addType).then(function succeeded(advisorsOfCertainType) {
         console.log(advisorsOfCertainType);
         if (advisorsOfCertainType.data.length == 0) {
             console.log("There are no Advisors loaded");
             return;
         }
         $scope.advisorsOfCertainType = advisorsOfCertainType.data;
         $state.go('advisor', { 'advisor': $scope.advisorsOfCertainType });
     }, function failure(errormessage) {
         console.log(errormessage);
     });



 }

    //Start the editing process:
 $scope.editAdvisorPageAndSave = function editAdvisorPageAndSave(save) {
     $scope.notAdvisorEditing = !$scope.notAdvisorEditing;
     $scope.editingApplicationDetail = !$scope.editingApplicationDetail;
     $scope.origAdvisor = angular.copy($scope.advisor);


     if (save) {

         for (var i = 0; i < $scope.AddressTypesTemp.length; i++)
         {
             if($scope.AddressTypesTemp[i].name==$scope.advisor.addressType)
             {
                 $scope.advisor.addressTyper_Id = $scope.AddressTypesTemp[i].id;
                 break;
             }
                 
         }

         for (var i = 0; i < $scope.ProvinceTemp.length; i++) {
             if ($scope.ProvinceTemp[i].name == $scope.advisor.province) {
                 $scope.advisor.province_Id = $scope.ProvinceTemp[i].id;
                 break;
             }

         }

         for (var i = 0; i < $scope.CountryTemp.length; i++) {
             if ($scope.CountryTemp[i].name == $scope.advisor.country) {
                 $scope.advisor.country_id = $scope.CountryTemp[i].id;
                 break;
             }

         }
         


        advisorService.PutAdvisor($scope.advisor.id, $scope.advisor).then(function succeeded(success) {
             console.log(success);
         }, function failure(errormessage) {
             console.log(errormessage);
         });
     }
 }

    //Get Address Types:
 detailsService.GetAddressTypes().then(function succeeded(success) {
     console.log(success);

     $scope.AddressTypesTemp = success.data;
     $scope.AddressTypes=[];
     success.data.forEach(function (item, index) { $scope.AddressTypes.push(item.name); });
  
 }, function failure(errormessage) {
     console.log(errormessage);
 });


    //Get Provinces
 detailsService.GetProvinces().then(function succeeded(success) {
     console.log(success);
     $scope.ProvinceTemp = success.data
     $scope.Province = [];
     success.data.forEach(function (item, index) { $scope.Province.push(item.name); });

 }, function failure(errormessage) {
     console.log(errormessage);
 });

    //Get Countries
 detailsService.GetCountries().then(function succeeded(success) {
     console.log(success);

     $scope.CountryTemp = success.data
     $scope.Country = [];
     success.data.forEach(function (item, index) { $scope.Country.push(item.name); });
     

 }, function failure(errormessage) {
     console.log(errormessage);
 });


 $scope.cancelEdit = function () {
     $scope.advisor = angular.copy($scope.origAdvisor);
     $scope.notAdvisorEditing = true;
     $scope.origAdvisor = {};
 };


 $scope.AdvisorAppClicked = function AdvisorAppClicked(id) {
     advisorService.GetAdvisorDocumentFile(id).then(function succeeded(success) {
         console.log(success);
     },
         function failure(errormessage) {
             console.log(errormessage);
 
         }
     );
 }


    //Delete
 $scope.deleteDocument = function deleteDocument(advisorId) {

     advisorService.DeleteAdvisorDocument(advisorId)
         .then(function succeeded(success) {
             console.log(success);
             $state.go('company');

         },
        function failure(errormessage) {
            console.log(errormessage);
            alert("Not Deleted")


        });
 }

   //KEY
    advisorService.GetAdvisorsKey().then(function succeeded(advisorKey) {
        console.log(advisorKey);
        if (advisorKey.data.length == 0) {
            console.log("There are no Keys loaded");
            return;
        }
        $scope.advisorsKey = advisorKey.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    //ADVISOR
    advisorService.GetAdvisorOfAdvisorTypeAdvisor().then(function succeeded(advisorAdvisor) {
        console.log(advisorAdvisor);
        if (advisorAdvisor.data.length == 0) {
            console.log("There are no Advisors loaded");
            return;
        }
        $scope.advisorAdvisor = advisorAdvisor.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    //ADVISORIds
    advisorService.GetAdvisorsAdvisorID().then(function succeeded(adadvisorAdvisorId) {
        console.log(adadvisorAdvisorId);
        if (adadvisorAdvisorId.data.length == 0) {
            console.log("There are no Advisors loaded");
            return;
        }
        $scope.adadvisorAdvisorId = adadvisorAdvisorId.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });


    //ADVISORKeyIds
    advisorService.GetAdvisorsKeyId().then(function succeeded(adadvisorKeyId) {
        console.log(adadvisorKeyId);
        if (adadvisorKeyId.data.length == 0) {
            console.log("There are no Advisors loaded");
            return;
        }
        $scope.adadvisorKeyId = adadvisorKeyId.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });



}]);