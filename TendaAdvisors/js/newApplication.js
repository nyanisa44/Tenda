tendaApp.controller('newApplicationController', ['$scope', 'clientsService', 'applicationService', 'detailsService', 'advisorService', '$state', function ($scope, clientsService, applicationService, detailsService, advisorService, $state) {

    $scope.isNewApplication = true;
    $scope.isNewApplication_check = false;
    $scope.incorrectEntry = false;
    // If notNewApplication_check is true, the layout will look more like a new application than an application Detail.
    $scope.notNewApplication_check = false;
    $scope.editingApplicationDetail = true;
    // Editing is technically always on in a new Application, but it will toggle in the Application Detail
    $scope.showDocumentUploads = false;
    $scope.contacts = {};
    $scope.contact = [];
    $scope.duplicateIdNumbers = false;
    $scope.duplicateMemberId = false;
    $scope.selectedItem = {};
    $scope.item = {};
    $scope.registerItem = 100;
    $scope.selectAdvisor = false;
    $scope.isAdvisorContact = false;
    // Start with 1 product and 1 supplier and 1 application document
    $scope.application = {};
    $scope.application.product = { "id": 0, "supplier": { "id": 0 } };
    $scope.application.client = { "id": 0 };
    $scope.application.advisor = {};
    $scope.CurrentAppType = null;

    $scope.suppliers = {};
    $scope.products = [];
    $scope.noProductSelected = true;

    $scope.newFakeDocument = { "title": "New Document Title", "location": "No file chosen" };

    $scope.AddressTypes = {};
    $scope.Province = {};
    $scope.Country = {};

    // The reason it is default to true so that the
    // error message only shows if it becomes a problem
    $scope.duplicateSupplier = false;
    $scope.supplierDuplicateError = '';
    $scope.duplicateSupplierClass = '';

    $scope.click = function (item) {
        $scope.selectedItem = item;
    };

    $scope.doSearch = function doSearch(searchWhere) {
        if (!!searchWhere && searchWhere.trim().length >= 4) {
            $scope.getContacts(searchWhere);
        }
    };

    $scope.doSearchAdviser = function doSearchAdviser(searchWhereAdviser) {
        if (!!searchWhereAdviser && searchWhereAdviser.trim().length >= 4) {
            $scope.getContactsAdvisers(searchWhereAdviser);
        }
    };

    $scope.doSearchSupplier = function doSearchSupplier(searchWhereSupplier) {
        if (!!searchWhereSupplier && searchWhereSupplier.trim().length >= 4) {
            $scope.getContactsSupplier(searchWhereSupplier);
        }
    };

    $scope.doSearchProducts = function doSearchProducts(searchWhereProducts, supplierID) {
        if (!!searchWhereProducts && searchWhereProducts.trim().lengsth >= 4) {
            $scope.getContactsProduct(searchWhereProducts, supplierID);
        }
    };

    // Get advisor
    $scope.getContactsAdvisers = function getContactsAdvisers(searchQuery) {
        if (searchQuery !== "") {
            advisorService.GetSimilarAdvisor(searchQuery)
                .then(function succeeded(contact) {
                    $scope.contact = contact.data;
                    $scope.gotAdvisor = false;
                    $scope.application.advisor.contact = $scope.contact[0];
                    $scope.application.advisor.firstName = $scope.contact[0].name;
                    $scope.application.advisor.lastName = $scope.contact[0].lastName;
                    $scope.application.advisor.idNumber = $scope.contact[0].idNumber;
                    $scope.application.advisor.memberId = $scope.contact[0].memberId;
                    $scope.application.advisor.id = $scope.contact[0].advisor_Id;
                    $scope.gotAdvisor = true;
                    $scope.application.advisor.contact.firstName = $scope.contact[0].name;
                }, function failure(errormessage) {
                    console.log(errormessage);
                });
        }
    };

    // Get supplier
    $scope.getContactsProduct = function getContactsProduct(searchQuery, supplierID) {
        if (searchQuery !== "") {
            applicationService.GetBasicProduct(searchQuery, supplierID).then(function succeeded(product) {
                $scope.product = product.data;
                $scope.application.product = $scope.product[0];
            }, function failure(errormessage) {
                console.log(errormessage);
            });
        }
    };

    // Get products
    $scope.getContactsSupplier = function getContactsSupplier(searchQuery) {
        if (searchQuery !== "") {
            applicationService.GetBasicSupplier(searchQuery).then(function succeeded(supplier) {
                $scope.supplier = supplier.data;
                $scope.application.product.supplier = $scope.supplier[0];
            }, function failure(errormessage) {
                console.log(errormessage);
            });
        }
    };

    // Get clients
    $scope.getContacts = function getContacts(searchQuery) {
        clientsService.GetBasicContact(searchQuery).then(function succeeded(contact) {
            $scope.contact = contact.data;
            if ($scope.contact !== null) {
                $scope.application.client = $scope.contact[0];
            }
            else {
                $scope.application.client = null;
            }
        }, function failure(errormessage) {
            console.log(errormessage);
            $scope.application.client = null;
        });
    };

    $scope.gotToNewClientPage = function () {
        $scope.addAnotherAddress();
        $scope.gotoPage(0);
    };

    // Advisor
    advisorService.GetAdvisorsAdvisorTypeAdvisor().then(function succeeded(advisorAdvisor) {
        if (advisorAdvisor.data.length === 0) {
            return;
        }
        $scope.advisors = advisorAdvisor.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    $scope.checkCharaters = function checkCharaters(characters) {
        if (!/^[a-zA-Z ]+$/.test(characters) || characters.length > 50) {
            $scope.incorrectEntry = true;
        }
        else {
            $scope.incorrectEntry = false;
        }
    };

    $scope.updateEffectiveStartDate = function updateEffectiveStartDate(startDate) {
        // Modified date is being manipulated to get the start date for the entry
        // Needs to be ModifiedDate since that is what is used when updating as well
        $scope.application.ModifiedDate = startDate;
    };

    $scope.gotoPage = function gotoPage(page) {

        if (page === 1 && $scope.registerItem === 0) {
            /*
                Note:
                At the moment when submitting an application, it's adding a new supplier the whole time (And the suppliers should be static),
                so to make things easier for the backend (and to lighten the load of a POST or PUT), I'm going to take out the suppliers out
                of the $scope.application before a PUT or a POST:
            */

            if ($scope.application.id === null || $scope.application.id === 0) {
                applicationService.PostApplication($scope.application).then(function succeeded(success) {
                    $scope.application.id = success.data.id;
                    console.log(" Post Applicaiton return: ", success.data);
                    // If successful, go to next page
                    $scope.registerItem = page;
                }, function failure(errormessage) {
                    $scope.application.client.errorMessage = "The client could not be saved. Please make sure all the required inputs have been filled in.";
                    console.log(errormessage);
                });
            }
            else {
                // Do a PUT to save the application's details: 
                applicationService.PutApplication($scope.application.id, $scope.application).then(function succeeded(success) {
                    // If successful, go to next page
                    $scope.registerItem = page;
                }, function failure(errormessage) {
                    console.log(errormessage);
                });
            }
        }
        else if (page === 2 && $scope.registerItem === 1) {
            // When the user is coming from the second step (1) of the registration process (going to the third),
            // it should submit the products as a list and an application ID should be supplied:

            // The member number is lined up incorrectly this will now allow for the client's details to be updated
            $scope.application.client.memberId = $scope.application.memberNumber;

            applicationService.PutApplicationProducts($scope.application.id, $scope.application)
                .then(function succeeded(success) {
                    // Just check if this was successful or not. If it's successful, then technically that application's products were successfully linked.
                    /*
                        Once the ajax call is done, I did the following for convenience in the next steps:
                        1.I matched all of the $scope.products elements onto $scope.application.products so that all the data is present:
                        2.Then I matched $scope.suppliers to $scope.application.products[*].suppliers
                    */

                    // 1.
                    for (var c = 0; c < $scope.products.length; c++) {
                        if ($scope.application.product_Id === $scope.products[c].id) {
                            $scope.application.product.id = $scope.products[c].id;
                            $scope.application.product.name = $scope.products[c].name;
                        }
                    }

                    //2.
                    for (var d = 0; d < $scope.suppliers.length; d++) {
                        if ($scope.application.supplierId === $scope.suppliers[d].id) {
                            $scope.application.product.supplier.id = $scope.suppliers[d].id;
                            $scope.application.product.supplier.name = $scope.suppliers[d].name;
                        }
                    }
                    // If successful, go to next page
                    $scope.registerItem = page;
                }, function failure(errormessage) {
                    console.log(errormessage);
                });

            if ($scope.application.client.memberId !== null && typeof $scope.application.client.memberId !== "undefined") {
                clientsService.PutContactMemberNumber($scope.application.client.id, $scope.application.client.memberId)
                    .then(function succeeded(success) {
                    }, function failure(errormessage) {
                        console.log(errormessage);
                    });
            }
        }
        else if (page === 3 && $scope.registerItem === 2) {
            // The user is coming from the second step and going to the third where documents can be uploaded:
            // Go to next page
            $scope.registerItem = page;
        }
        else if (page === 4) {
            applicationService.PutApplication($scope.application.id, $scope.application)
                .then(function succeeded(success) {
                    // If successful, go to next page
                    $scope.registerItem = page;
                }, function failure(errormessage) {
                    console.log(errormessage);
                });

            // Submit the page
            $scope.registerItem = page;

            //Since everything is already saved, nothing really gets submitted, so We'll just take the user to the application detail of that page:
            $state.go('application');
        }
        else if (page === 5) {
            // Submit the page
            $scope.registerItem = page;

            // Since everything is already saved, nothing really gets submitted, so We'll just take the user to the application detail of that page:
            $state.go('application');
        }
        else if (page === 0) {
            // Submit the page
            $scope.registerItem = page;
        }
        else if (page === 1 && $scope.registerItem === 100) {
            if ($scope.application.id === null || $scope.application.id === 0 || typeof $scope.application.id === 'undefined') {
                $scope.application.clientId = $scope.application.client.id;
                applicationService.PostApplication($scope.application)
                    .then(function succeeded(success) {
                        $scope.application.id = success.data.id;
                        // If successful, go to next page
                        $scope.registerItem = page;
                    }, function failure(errormessage) {
                        $scope.application.client.errorMessage = "The client could not be saved. Please make sure all the required inputs have been filled in.";
                        console.log(errormessage);
                    });
            }
            else {
                // Do a PUT to save the application's details:
                applicationService.PutApplication($scope.application.id, $scope.application)
                    .then(function succeeded(success) {
                        // If successful, go to next page
                        $scope.registerItem = page;
                    }, function failure(errormessage) {
                        console.log(errormessage);
                    });
            }
        }
        else {
            // Go back to previous page
            $scope.registerItem = page;
        }
    };

    $scope.newClient = function newClient() {
        $scope.isNewApplication_check = true;
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

    $scope.oldClient = function oldClient() {
        $scope.notNewApplication_check = true;
    };

    // Get title
    detailsService.GetContactTitles().then(function succeeded(success) {
        $scope.Titles = success.data;

    }, function failure(errormessage) {
        console.log(errormessage);
    });

    $scope.goBackToApplications = function goBackToApplications() {
        $state.go('application');
    };

    $scope.getApplicationData = function () {
        // Application Types
        applicationService.GetApplicationTypes()
            .then(function succeeded(success) {
                $scope.appTypes = success.data;
                $scope.application.applicationType = $scope.appTypes[0];
                $scope.application.applicationType_Id = $scope.application.applicationType.id;
                $scope.CurrentAppType = { id: 0, title: "" };
                for (var i = 0; i < $scope.appTypes.length; i++) {
                    if ($scope.appTypes[i].id === $scope.application.applicationType_Id) {
                        $scope.CurrentAppType = $scope.appTypes[i];
                    }
                }
            }, function failure(errormessage) {
                console.log(errormessage);
            });
    };

    $scope.getApplicationData();

    // Supplier issues here
    applicationService.GetSuppliers().then(function succeeded(success) {
        $scope.suppliers = success.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    applicationService.GetApplicationStatuses().then(function succeeded(success) {
        $scope.appStatus = success.data;
        $scope.application.applicationStatus = $scope.appStatus[0];
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // Get Address Types:
    detailsService.GetAddressTypes().then(function succeeded(success) {
        $scope.AddressTypes = success.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // Get provinces
    detailsService.GetProvinces().then(function succeeded(success) {
        $scope.Province = success.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // Get countries
    detailsService.GetCountries().then(function succeeded(success) {
        $scope.Country = success.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // When the selection changes, this updates:
    $scope.updateProducts = function updateProducts(supplierId) {
        if (typeof $scope.application.applicationSuppliers === "undefined") {
            $scope.application.applicationSuppliers = [];
        }

        applicationService.checkClientDuplicateSupplier($scope.application.clientId, supplierId).then(
            function succeeded(success) {
                $scope.duplicateSupplier = success.data;

                if ($scope.duplicateSupplier) {
                    $scope.supplierDuplicateError = 'This client has an application with choosen supplier';
                    $scope.duplicateSupplierClass = 'ng-invalid-border';
                }
                else {
                    $scope.supplierDuplicateError = '';
                    $scope.duplicateSupplierClass = '';
                }
            },
            function failure(errorMessage) {
                console.log("Check duplicate supplier error: ", errorMessage);
            }
        );

        var addMe = true;
        if ($scope.application.applicationSuppliers.length > 0) {
            for (var r = 0; r < $scope.application.applicationSuppliers.length; r++) {
                if ($scope.application.applicationSuppliers[r].supplierId === supplierId) {
                    addMe = false;
                }
            }
        }

        if (addMe === true) {
            // check on
            $scope.application.applicationSuppliers.push({ "applicationId": $scope.application.id, "supplierId": supplierId, "memberNumber": $scope.application.memberNumber });
        }

        // Ajax to the server and GET the products related to the supplier:
        applicationService.GetSupplierProducts(supplierId)
            .then(function succeeded(success) {
                for (var r = 0; r < success.data.length; r++) {
                    var canAdd = true;
                    for (var k = 0; k < $scope.products.length; k++) {
                        if (success.data[r].id === $scope.products[k].id) {
                            canAdd = false;
                        }
                    }
                    if (canAdd) {
                        if (typeof $scope.products === "undefined") {
                            $scope.products = [];
                        }

                        $scope.products.push(success.data[r]);
                        $scope.products[$scope.products.length - 1].supplier = { "id": supplierId };
                    }
                }
            }, function failure(errormessage) {
                console.log(errormessage);
            });
    };

    $scope.productChange = function productChange(productId) {
        if (productId !== 0 && productId !== null) {
            $scope.noProductSelected = false;
        }
        else {
            $scope.noProductSelected = true;
        }
    };

    $scope.addAnotherAddress = function addAnotherAddress() {
        if (typeof $scope.application.client.addresses === "undefined") {
            $scope.application.client.addresses = [];
        }

        $scope.application.client.addresses.push({ "id": 0 });
    };

    $scope.addApplication = function addApplication(supplierId) {
        if (typeof $scope.application.product === "undefined") {
            $scope.application.product = { "id": 0, "supplier": { "id": 0 } };
        }
    };

    // Select a picture icon to fit the document:
    $scope.getDocumentPic = function getDocumentPic(pic) {
        return "img/picIcon.png";
    };

    // Receives a product json object which is an object of $scope.application
    $scope.openAddDocument = function openAddDocument(product) {
        // The applicationDocuments is an array, and if it's not defined as an array, then you should make it an array:
        if (typeof product.applicationDocuments === "undefined") {
            product.applicationDocuments = [];
        }

        product.applicationDocuments.push({
            "title": "", "location": "", "ValidFromDate": function () {
                return new Date(Date.now());
            }, "ValidToDate": function () { return new Date(Date.now()); }
        });
    };

    $scope.uploadDocument = function uploadDocument(applicationID, productID, docID, doc, submitted) {
        // Use the productID (productID) and the applicationID ($scope.application.id) to submit the document to the ApplicationDocumentController (api/ApplicationDocuments/5/Product/2)

        if (submitted !== true) {
            submitted = false;
        }

        applicationService.PostApplicationDocument(applicationID, productID, docID, doc, submitted)
            .then(function succeeded(success) {
                doc.uploaded = true;
                doc = success.data;
            }, function failure(errormessage) {
                console.log(errormessage);
                doc.notUploaded = true;
            });
    };

    // The following name change function is used since angular does not bind with the input type="file":
    $scope.fileNameChanged = function (ele) {
        $scope.newFakeDocument.location = ele.files[0].name;
    };

    // Delete a document of an application and client:
    $scope.deleteDocument = function deleteDocument(id) {
        // TODO: this will change once the database works again:
        for (var i = 0; i < $scope.application.applicationDocuments.length; i++) {
            if ($scope.application.applicationDocuments[i] !== null && $scope.application.applicationDocuments[i].id === id) {
                $scope.application.applicationDocuments.splice(i, 1);
                i = $scope.application.applicationDocuments.length;
            }
        }
    };

    $scope.deleteAddress = function deleteAddress(index) {
        $scope.application.client.addresses.splice(index, 1);
    };

    $scope.checkIfProductsValid = function checkIfProductsValid(products) {
        for (var i = 0; i < products.length; i++) {
            if (products[i].id === null || products[i].id === "" || typeof products[i].id === "undefined") {
                return false;
            }
        }
        return true;
    };

    $scope.getAddressType = function getAddressType(id) {
        if (typeof $scope.AddressTypes === "undefined" || $scope.AddressTypes === null) {
            return "";
        }
        else {
            for (var i = 0; i < $scope.AddressTypes.length; i++) {
                if ($scope.AddressTypes[i].id === id) {
                    return $scope.AddressTypes[i].name;
                }
            }
        }
    };

    $scope.getProvince = function getProvince(id) {
        if (typeof $scope.Province === "undefined" || $scope.Province === null) {
            return "";
        }
        else {
            for (var i = 0; i < $scope.Province.length; i++) {
                if ($scope.Province[i].id === id) {
                    return $scope.Province[i].name;
                }
            }
        }
    };


    $scope.getContactsDb = function getContactsDb() {
        clientsService.GetContacts().then(function succeeded(success) {
            $scope.contacts = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.getContactsDb();

    $scope.getCountry = function getCountry(id) {
        if (typeof $scope.Country === "undefined" || $scope.Country === null) {
            return "";
        }
        else {
            for (var i = 0; i < $scope.Country.length; i++) {
                if ($scope.Country[i].id === id) {
                    return $scope.Country[i].name;
                }
            }
        }
    };
}]);
