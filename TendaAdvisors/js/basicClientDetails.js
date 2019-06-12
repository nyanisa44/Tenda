tendaApp.controller('basicClientDetailsController', ['$scope', 'clientsService', 'applicationService', 'detailsService', 'advisorService', '$state', function ($scope, clientsService, applicationService, detailsService, advisorService, $state) {
    // If this is true, the layout will look more like a new application than an application Detail.
    $scope.isNewApplication = true;
    $scope.incorrectEntry = false;
    $scope.cantEdit = false;
    $scope.notAdvisorEditing = true;
 
    $scope.isNewApplication_check = false;
    
    $scope.notNewApplication_check = false;
    // Editing is technically always on in a new Application, but it will toggle in the Application Detail
    $scope.editingApplicationDetail =false;
    $scope.contact = { "addresses": [{ "id": 0 }] };
    $scope.registerItem = 100;
    // Start with 1 product and 1 supplier and 1 application document
    $scope.application = {
        "products":
        [{
            "id": 0, "supplier": { "id": 0 }
        }],
        "client":
        {
            "addresses": [{ "id": 0 }]
        },
        "advisor":
        {
            "id": 0
        }
    };

    $scope.suppliers = {};
    $scope.products = [];
    $scope.selectedSupplier = 0;
    $scope.selectedProduct = 0;
    $scope.newFakeDocument =
    {
        "title": "New Document Title",
        "location": "No file chosen"
    };
    $scope.AddressTypes = {};
    $scope.Province = {};
    $scope.Country = {};
    $scope.ErrorIdNumber = {};

    if ($state.params.contact === null) {
        $state.go('clients');
        return;
    }

    $scope.getContact = function getContact() {
        return clientsService.GetContactClients($state.params.contact).then(
            function succeeded(contact) {
                $scope.contact = contact.data;
            },
            function failure(errormessage) {
                console.log(errormessage);
                $scope.contact = null;
            }
        );
    };

   $scope.getContact();

    $scope.checkCharaters = function checkCharaters(characters) {
        if (!/^[a-zA-Z ]+$/.test(characters) || characters.length > 50) {
            $scope.incorrectEntry = true;
        }
        else {
            $scope.incorrectEntry = false;
        }
    };

    // Checks the ID Number of the user then changes it if it's valid
    $scope.advisorTextChangeId = function advisorTextChangeId(IdNum) {
        if ($scope.validateSAIDNumber(IdNum)) {
            $scope.ErrorIdNumber.style = { "color": 'black' };
        }
        else {
            $scope.ErrorIdNumber.style = { "color": "red" };
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

    //Start the editing process:
    $scope.editAdvisorPageAndSave = function editAdvisorPageAndSave(save) {
        $scope.notAdvisorEditing = !$scope.notAdvisorEditing;
        $scope.editingApplicationDetail = !$scope.editingApplicationDetail;
        $scope.origAdvisor = angular.copy($scope.contact);
        if (save) {
            for (var a = 0; a < $scope.TitlesTemp.length; a++) {
                if ($scope.TitlesTemp[a].name === $scope.contact.contactTitle_Name) {
                    $scope.contact.ContactTitle_Id = $scope.TitlesTemp[a].id;
                    break;
                }
            }

            for (var b = 0; b < $scope.AddressTypesTemp.length; b++) {
                if ($scope.AddressTypesTemp[b].name === $scope.contact.addressType) {
                    $scope.contact.addressTyper_Id = $scope.AddressTypesTemp[b].id;
                    break;
                }
            }

            for (var c = 0; c < $scope.ProvinceTemp.length; c++) {
                if ($scope.ProvinceTemp[c].name === $scope.contact.province) {
                    $scope.contact.province_Id = $scope.ProvinceTemp[c].id;
                    break;
                }
            }

            for (var d = 0; d < $scope.CountryTemp.length; d++) {
                if ($scope.CountryTemp[d].name === $scope.contact.country) {
                    $scope.contact.country_id = $scope.CountryTemp[d].id;
                    break;
                }
            }

            clientsService.PutContactClient($scope.contact.contactId, $scope.contact)
                .then(function succeeded(success) {
                    console.log(success);
                }, function failure(errormessage) {
                    console.log(errormessage);
                });
        }
    };

    // Cancel Edit
    $scope.cancelEdit = function () {
        $scope.contact = angular.copy($scope.origAdvisor);
        $scope.notAdvisorEditing = true;
        $scope.editingApplicationDetail = false;
        $scope.origAdvisor = {};
    };

    // Get Title
    detailsService.GetContactTitles().then(function succeeded(success) {
        $scope.TitlesTemp = success.data;
        $scope.Titles = [];
        success.data.forEach(function (item, index) { $scope.Titles.push(item.name); });
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // Get Address Types:
    detailsService.GetAddressTypes().then(function succeeded(success) {
        $scope.AddressTypesTemp = success.data;
        $scope.AddressTypes = [];
        success.data.forEach(function (item, index) { $scope.AddressTypes.push(item.name); });
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // Get Provinces
    detailsService.GetProvinces().then(function succeeded(success) {
        $scope.ProvinceTemp = success.data;
        $scope.Province = [];
        success.data.forEach(function (item, index) {
            $scope.Province.push(item.name);
        });
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // Get Countries
    detailsService.GetCountries().then(function succeeded(success) {
        $scope.CountryTemp = success.data;
        $scope.Country = [];
        success.data.forEach(function (item, index) {
            $scope.Country.push(item.name);
        });
    }, function failure(errormessage) {
        console.log(errormessage);
    });
}]);
