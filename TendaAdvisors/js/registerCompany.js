tendaApp.controller('registerCompanyController', ['$scope', 'advisorService', 'detailsService', '$state', function ($scope, advisorService,detailsService, $state) {
    $scope.registerItem = 0;//Start at the beginning
    $scope.advisor = { "advisorDocuments": [{ "documentTypeId": 0 }], "licenses": [], "contact": { "addresses": [{ "id": 0 }] } };
    $scope.uploadSucces = "Not Uploaded";
    $scope.companies = [];
    $scope.editingAdvisorDetail = true;
    $scope.uploaded = false;
    $scope.notUploaded = false;
    $scope.cantEdit = true;

    $scope.advisorRole = {
        isAdvisor: false,
        isAdmin: false
    };
    $scope.actualNewAdvisor = false;
    //Licenses
    $scope.advisorTypes = [];
    $scope.licenseCategories = [];
    $scope.licenseTypes = [];
    $scope.chosenLicenseCategoryId = 0;
    $scope.chosenLicenseTypeId = 0;
    $scope.advisor.CmsCode = "BR";
    $scope.advisor.Company = { "id": 1 };
    $scope.advisor.advisorType = { "id": 2 };
    $scope.advisor.advisorStatus = { "id": 2 };
    $scope.advisor.contact = { "ContactType": { "id": 1 }, "addresses": [{ "addressType": { "id": 3 } }] };
    //$scope.advisor.Contact = $scope.advisor.contact; //Why are there two? case sensitive...
    $scope.AddressTypes = {};
    $scope.Province = {};
    $scope.Country = {};
    $scope.Addresses = {};

    $scope.selectedRole = [];
    $scope.advisorRole = [
        {
            name: "Admin",
            checked: false
        },
        {
            name: "Advisor",
            checked: false
        }
    ];


    $scope.gotoPage = function gotoPage(page) {

        if (page === 0 ) {
            $scope.registerItem = page;
            $scope.actualNewAdvisor = true;
        } else if (page === 1) {
            $scope.registerItem = page;

         
                    $scope.advisor.User.IsAdmin = true;
       
            advisorService.CreateAdvisor($scope.advisor).then(function succeeded(companies) {
                console.log(companies);
                $scope.advisor = companies.data;
            }, function failure(errormessage) {
                console.log(errormessage);
                
            });
            //$scope.fetchSuppliersByLicenses($scope.advisor.licenses);
        } else if (page === 2) {
            $state.go('company');

        } 
    }

    $scope.addLicenseToAdvisor = function (advisor, licenseId) {
        var licenseFound = false;
        for (id in advisor.licenses) {
            if (advisor.licenses[id].id == licenseId) {
                licenseFound = true;
            }
        }

        if (!licenseFound) {
            for (id in $scope.licenseTypes) {
                if ($scope.licenseTypes[id].id == licenseId) {
                    advisor.licenses.push($scope.licenseTypes[id]);
                }
            }
        }
        console.log(advisor.licenses);
    }

    $scope.doSearch = function doSearch(searchWhere) {
        console.log("Search for companies containing: " + searchWhere);
        if (!!searchWhere && searchWhere.trim().length >= 4)
            $scope.getCompanies(searchWhere);
    }

    //Get Address Types:
    detailsService.GetAddressTypes().then(function succeeded(success) {
        console.log(success);

        $scope.AddressTypes = success.data;

    }, function failure(errormessage) {
        console.log(errormessage);
    });

    //Get Provinces
    detailsService.GetProvinces().then(function succeeded(success) {
        console.log(success);
        $scope.Province = success.data;

    }, function failure(errormessage) {
        console.log(errormessage);
    });

    //check FirstName characters
    $scope.checkCharaters = function checkCharaters(characters) {
        if (!/^[a-zA-Z ]+$/.test(characters) || characters.length > 50) {
            $scope.incorrectEntry = true;
        }
        else {
            $scope.incorrectEntry = false;
        }


    }

    //Get Countries
    detailsService.GetCountries().then(function succeeded(success) {
        console.log(success);

        $scope.Country = success.data;

    }, function failure(errormessage) {
        console.log(errormessage);
    });

  





    //Select a picture icon to fit the document:
    $scope.getDocumentPic = function getDocumentPic(pic) {
        if (pic == "JPEG")
            return "img/picIcon.png";

        return "img/picIcon.png";
    }

    $scope.addAnotherAddress = function addAnotherAddress() {

        if (typeof $scope.advisor.contact.addresses == "undefined")
            $scope.advisor.contact.addresses = [];
        $scope.advisor.contact.addresses.push({ "id": 0 });

    }

    $scope.deleteAddress = function deleteAddress(index) {

        $scope.advisor.contact.addresses.splice(index, 1);

    }


    $scope.uploadDocument = function uploadDocument(advisorId, documentTypeID, document, submitted) {

        //advisorId, documentTypeId and base64 file data (api/AdvisorDocuments/' + advisorId + '/DocumentTypeId/' + documentId)

        if (submitted != true) {
            submitted = false;
        }

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

    //when you want to leave


    $scope.gotoAdvisor = function gotoAdvisor() {
        var result = confirm('Are you sure you want to exit adding a new Advisor?');

        if (result == true) {
            $state.go('advisors');
        }
    }


    //getContactTypes
    advisorService.GetAdvisorTypes().then(function succeeded(advisorTypes) {
        //console.log(queries.data);
        if (advisorTypes.length === 0) {
            console.log("There are no Queries");
            return;
        }

        $scope.advisorTypes = advisorTypes.data;
        $scope.advisor.advisorType = $scope.advisorTypes[4];
    }, function failure(errormessage) {
        console.log(errormessage);
    });


}]);