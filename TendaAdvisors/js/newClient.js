
tendaApp.controller('newClient', ['$scope', 'clientsService', 'applicationService', 'detailsService', 'advisorService', '$state', function ($scope, clientsService, applicationService, detailsService, advisorService, $state) {

    $scope.isNewApplication = true;
    $scope.isNewApplication_check = false;
    $scope.notNewApplication_check = false; // If this is true, the layout will look more like a new application than an application Detail.
    $scope.editingApplicationDetail = true; // Editing is technically always on in a new Application, but it will toggle in the Application Detail
    $scope.duplicateIdNumbers = false;
    $scope.invalidIdNumber = true;
    $scope.registerItem = 0;
    $scope.application = {
        "products": [{
            "id": 0,
            "supplier": { "id": 0 }
        }],
        "client": {
            "addresses": [{ "id": 0 }]
        },
        "advisor": { "id": 0 }
    }; // Start with 1 product and 1 supplier and 1 application document
    $scope.contact = { "addresses": [{ "id": 0 }]};
    $scope.suppliers = {};
    $scope.products = [];
    $scope.selectedSupplier = 0;
    $scope.selectedProduct = 0;
    $scope.incorrectEntry = false;
    $scope.newFakeDocument = { "title": "New Document Title", "location": "No file chosen" };
    $scope.AddressTypes = {};
    $scope.Province = {};
    $scope.Country = {};
    $scope.Titles = {};

    $scope.gotoPage = function gotoPage(page) {

        if (page === 1 && $scope.registerItem === 0) {
            clientsService.PostContact($scope.contact).then(function succeeded(success) {
                // This might be a bad idea and also not needed
                $scope.contact = success.data;
                //If Successful, go to next page
                $scope.registerItem = page;
                $state.go('clients');

            }, function failure(errormessage) {
                $scope.client.errorMessage = "The client could not be saved. Please make sure all the required inputs have been filled in.";
                console.log(errormessage);
            });
        }
        else {
            $scope.registerItem = page;//Maybe go back to previous page
        }
    };

    $scope.validateSAIDNumber = function validateSAIDNumber(idnum) {
        // http://geekswithblogs.net/willemf/archive/2005/10/30/58561.aspx
        // Left by simon on Apr 28, 2009 5:09 AM
        idnum = idnum.toString().replace(" ", "");
        r = /^\d{10}[0-1]\d{2}$/;
        if (!r.test(idnum)) {
            return false;
        }
        n = idnum;
        p1 = parseInt(n[0], 10) + parseInt(n[2], 10) + parseInt(n[4], 10) + parseInt(n[6], 10) + parseInt(n[8], 10) + parseInt(n[10], 10);
        p2 = (parseInt(n[1] + n[3] + n[5] + n[7] + n[9] + n[11], 10) * 2).toString();
        p3 = 0;
        for (i = 0; i < p2.length; i++) {
            p3 += parseInt(p2[i]);
        }
        check = 10 - (p1 + p3).toString()[(p1 + p3).toString().length - 1];
        check_char = check > 9 ? check.toString()[1] : check.toString();
        if (check_char !== idnum[12]) {
            return false;
        }
        return true;
    };

    $scope.checkDuplicateIds = function checkDuplicateIds(idNum) {
        if (idNum === undefined) {
            $scope.duplicateIdNumbers = false;
        }
        else {
            for (var i = 0; i < $scope.contacts.length; i++) {
                if ($scope.contacts[i].idNumber === idNum) {
                    $scope.duplicateIdNumbers = true;
                    break;
                }
                else {
                    $scope.duplicateIdNumbers = false;
                }
            }
        }
    };

    $scope.validateIdNumber = function validateIdNumber(idNumber) {
        if (idNumber === null || typeof idNumber === 'undefined') {
            $scope.duplicateIdNumber = false;
            $scope.invalidIdNumber = true;
        }
        else {
            $scope.checkDuplicateIds(idNumber);

            if (idNumber.length === 13) {
                $scope.invalidIdNumber = $scope.validateSAIDNumber(idNumber);
            }
            else if (idNumber.length > 13) {
                $scope.invalidIdNumber = true;
            }
            else {
                $scope.invalidIdNumber = false;
            }
        }
    };

    $scope.checkCharaters = function checkCharaters(characters) {
        if (!/^[a-zA-Z ]+$/.test(characters) || characters.length > 50) {
            $scope.incorrectEntry = true;
        }
        else {
            $scope.incorrectEntry = false;
        }
    };

    // Get Address Types:
    detailsService.GetAddressTypes().then(function succeeded(success) {
        $scope.AddressTypes = success.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });


    clientsService.GetContacts().then(function succeeded(success) {
        $scope.contacts = success.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });


    // Get Title
    detailsService.GetContactTitles().then(function succeeded(success) {
        $scope.Titles = success.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // Get Provinces
    detailsService.GetProvinces().then(function succeeded(success) {
        $scope.Province = success.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // Get Countries
    detailsService.GetCountries().then(function succeeded(success) {
        $scope.Country = success.data;
    }, function failure(errormessage) {
        console.log(errormessage);
        });
}]);
