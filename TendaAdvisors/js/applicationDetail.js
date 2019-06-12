tendaApp.controller('applicationDetailController', ['$scope', 'applicationService', 'detailsService', 'advisorService', 'clientsService', '$state', function ($scope, applicationService, detailsService, advisorService, clientsService, $state) {

    if ($state.params.application === null) {
        $state.go('application');
        return;
    }

    var documentCount = 0;
    $scope.noEditing = true;
    // If isNewApplication is true, the layout will look more like a new application than an application Detail.
    $scope.isNewApplication = false;
    $scope.editingApplicationDetail = false;
    $scope.noEditing = true;
    $scope.incorrectEntry = false;
    $scope.viewAdvisorSelected = false;
    $scope.CurrentAppStatus = null;
    $scope.CurrentAppType = null;
    $scope.ProductList = [];
    $scope.CurrentProduct = null;
    $scope.OldAdvisor = null;
    $scope.ApplicationAdvisorHistor = null;
    $scope.ApplicationAdvisorEditHistory = null;
    $scope.updateEffectiveStartDateAvisorHistory = false;
    $scope.updateEffectiveEndDateAvisorHistory = false;
    $scope.appTypes = null;
    $scope.advisorChange = false;
    $scope.gotAdvisor = false;
    $scope.effectiveStartDateUpdated = null;
    $scope.adviserUpdateCheck = false;

    $scope.newAdvisor = {
        firstName: '',
        lastName: '',
        idNumber: '',
        id: 0
    };

    $scope.getApplicationData = function () {
        applicationService.GetApplication($state.params.application).then(function succeeded(application) {
            if (application.length === 0) {
                console.log("An application was not loaded");
                return;
            }

            $scope.application = application;
            $scope.application.products = $scope.application.product_Id;
            $scope.application.product = $scope.application.product_Id;
            $scope.application.applicationType = {};
            $scope.application.applicationType.id = $scope.application.applicationType_Id;
            $scope.application.memberNumber = application.memberNumber;
            $scope.application.supplierId = application.supplierId;
            $scope.advisorChange = false;
            $scope.gotAdvisor = true;

            clientsService.GetContact(application.client_Id)
                .then(function succeeded(success) {
                    $scope.application.client = success.data;
                });

            $scope.getApplicationAdvisorHistory();
            $scope.getApplicationAdvisorEditHistory();

            applicationService.GetSuppliersByApplication(application.id)
                .then(function succeeded(success) {
                    $scope.application.suppliers = success.data;
                });

            // Get all products vaild for application supplier
            applicationService.GetSupplierProducts(application.supplierId).then(function succeeded(success) {
                $scope.ProductList = success.data;

                for (var i = 0; i < $scope.ProductList.length; i++) {
                    if ($scope.ProductList[i].id === application.product_Id) {
                        $scope.CurrentProduct = $scope.ProductList[i];
                    }
                }
            });

            // Get Application status
            applicationService.GetApplicationStatuses()
                .then(function succeeded(success) {
                    $scope.appStatus = success.data;

                    // Initialse CurrentAppStatus
                    $scope.CurrentAppStatus = { id: 0, status: "" };
                    // Set CurrentAppStatus for return data
                    for (var i = 0; i < $scope.appStatus.length; i++) {
                        if ($scope.appStatus[i].id === $scope.application.applicationStatus_Id) {
                            $scope.CurrentAppStatus = $scope.appStatus[i];
                        }
                    }
                }, function failure(errormessage) {
                    console.log(errormessage);
                });

            // Get Application Types
            applicationService.GetApplicationTypes()
                .then(function succeeded(success) {
                    $scope.appTypes = success.data;
                    // Initialse CurrentappTypes
                    $scope.CurrentAppType = { id: 0, title: "" };
                    // Set CurrentappTypes for return data
                    for (var i = 0; i < $scope.appTypes.length; i++) {
                        if ($scope.appTypes[i].id === $scope.application.applicationType_Id) {
                            $scope.CurrentAppType = $scope.appTypes[i];
                        }
                    }
                }, function failure(errormessage) {
                    console.log(errormessage);
                });

            applicationService.GetApplicationDocumentsByApplicationId(application.id)
                .then(function succeeded(success) {
                    application.applicationDocuments = success.data;
                    if (application.applicationDocuments !== null) {
                        var appDoc = null;
                        for (var i = 0; i < application.applicationDocuments.length; i++) {
                            appDoc = application.applicationDocuments[i];
                            appDoc.validFromDate = new Date(appDoc.validFromDate);
                            appDoc.validToDate = new Date(appDoc.validToDate);

                            if (application.applicationType_Id === 1) {
                                if (appDoc.documentTypeId === 37 && appDoc.uploaded === true) {
                                    //alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }
                                if (appDoc.documentTypeId === 38 && appDoc.uploaded === true) {
                                    // alert("BANK DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }
                                if (appDoc.documentTypeId === 39 && appDoc.uploaded === true) {
                                    // alert("BROKER NOTE DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 3) {
                                    //alert("New application complete")
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }
                            if (application.applicationType_Id === 6) {
                                if (appDoc.documentTypeId === 37 && appDoc.uploaded === true) {
                                    //alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }
                                if (appDoc.documentTypeId === 38 && appDoc.uploaded === true) {
                                    // alert("BANK DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }
                                if (appDoc.documentTypeId === 39 && appDoc.uploaded === true) {
                                    // alert("BROKER NOTE DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 3) {
                                    //alert("New application-Solidarity complete")
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }

                            if (application.applicationType_Id === 2) {
                                if (appDoc.documentTypeId === 7 && appDoc.uploaded === true) {
                                    //alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 1) {
                                    //alert("Broker complete")
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }
                            if (application.applicationType_Id === 7) {
                                if (appDoc.documentTypeId === 7 && appDoc.uploaded === true) {
                                    //alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 1) {
                                    //alert("Broker-Solidarity complete")
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }

                            if (application.applicationType_Id === 3) {
                                if (appDoc.documentTypeId === 40 && appDoc.uploaded === true) {
                                    //alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 1) {
                                    //alert("Group appointment complete")
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }

                            if (application.applicationType_Id === 4) {
                                if (appDoc.documentTypeId === 37 && appDoc.uploaded === true) {
                                    //alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 1) {
                                    //alert("Incomplete complete")
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }


                            if (application.applicationType_Id === 5) {
                                if (appDoc.documentTypeId === 42 && appDoc.uploaded === true) {
                                    //alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 1) {
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }
                            appDoc = null;
                        }

                        //set CurrentAppStatus for documents
                        for (var a = 0; a < $scope.appStatus.length; a++) {
                            if ($scope.appStatus[a].id === $scope.application.applicationStatus_Id) {
                                $scope.updateApplicationStatus($scope.appStatus[a]);
                            }
                        }
                    }
                }, function failure(errormessage) {
                    console.log(errormessage);
                    application.applicationDocuments = null;
                    documentCount = 0;
                });
            $scope.newSupplier = $scope.suppliers[0];
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    },

    $scope.getApplicationData();

    $scope.getApplicationAdvisorHistory = function getApplicationAdvisorHistory() {
        applicationService.GetApplicationAdvisorHistory($scope.application.id).then(function succeeded(success) {
            $scope.ApplicationAdvisorHistor = success.data;
            if (success.data.count !== 0) {
                if ($scope.ApplicationAdvisorHistor[success.data.length - 1].dateStarted !== null) {
                    $scope.effectiveStartDate =
                        $scope.ApplicationAdvisorHistor[success.data.length - 1].dateStarted.substring(0, $scope.ApplicationAdvisorHistor[success.data.length - 1].dateStarted.indexOf("T"));
                }
                if ($scope.ApplicationAdvisorHistor[success.data.length - 1].dateEnded !== null) {
                    $scope.effectiveEndDate =
                        $scope.ApplicationAdvisorHistor[success.data.length - 1].dateEnded.substring(0, $scope.ApplicationAdvisorHistor[success.data.length - 1].dateEnded.indexOf("T"));
                }

                $scope.ApplicationAdvisorHistor.forEach(function (item) {
                    if (item.dateStarted !== null) {
                        item.dateStarted = item.dateStarted.substring(0, item.dateStarted.indexOf("T"));
                    }
                    if (item.dateEnded !== null) {
                        item.dateEnded = item.dateEnded.substring(0, item.dateEnded.indexOf("T"));
                    }
                });
            }
        });
    },

        $scope.getApplicationAdvisorEditHistory = function getApplicationAdvisorEditHistory() {
            applicationService.getApplicationAdvisorEditHistory($scope.application.id).then(function succeeded(success) {
                $scope.applicationAdvisorEditHistory = success.data;
                if (success.data.count !== 0) {
                    $scope.applicationAdvisorEditHistory.forEach(function (item) {
                        if (item.dateEdited !== null || typeof item.dateEdited !== "undefined") {
                            item.dateEdited = item.dateEdited.substring(0, item.dateEdited.indexOf("T"));
                        }
                    });
                }
            });
        },

        $scope.doSearchAdviser = function doSearchAdviser(searchWhereAdviser) {
            if (!!searchWhereAdviser && searchWhereAdviser.trim().length >= 4) {
                $scope.getContactsAdvisers(searchWhereAdviser);
            }
        },

        $scope.doSearchForNewAdviser = function doSearchForNewAdviser(seach) {
            if (!!seach && seach.trim().length >= 4) {
                $scope.getNewContactsAdvisers(seach);
            }
        };

        // get Advisor
        $scope.getContactsAdvisers = function getContactsAdvisers(searchQuery) {
            if (searchQuery !== "") {
                $scope.gotAdvisor = false;
                advisorService.GetSimilarAdvisor(searchQuery).then(function succeeded(contact) {
                    $scope.contact = contact.data[0];

                    if ($scope.application.advisor_Id === $scope.contact.advisor_Id) {
                        $scope.gotAdvisor = true;
                    }
                    else {
                        $scope.advisorChange = true;
                    }

                    $scope.application.advisor.contact = $scope.contact;
                    $scope.application.advisor.firstName = $scope.contact.name;
                    $scope.application.advisor.lastName = $scope.contact.lastName;
                    $scope.application.advisor.idNumber = $scope.contact.idNumber;
                    $scope.application.advisor.id = $scope.contact.advisor_Id;

                }, function failure(errormessage) {
                    $scope.advisorChange = false;
                    console.log(errormessage);
                });
            }
    };

    $scope.getNewContactsAdvisers = function getNewContactsAdvisers(searchQuery) {
        if (searchQuery !== "") {
            advisorService.GetSimilarAdvisor(searchQuery).then(function succeeded(newContact) {
                $scope.newAdvisor.firstName = newContact.data[0].name;
                $scope.newAdvisor.lastName = newContact.data[0].lastName;
                $scope.newAdvisor.idNumber = newContact.data[0].idNumber;
                $scope.newAdvisor.id = newContact.data[0].advisor_Id;

            }, function failure(errormessage) {
                $scope.advisorChange = false;
                console.log(errormessage);
            });
        }
    };

    // ADVISOR
    advisorService.GetAdvisorsAdvisorTypeAdvisor().then(function succeeded(advisorAdvisor) {
        if (advisorAdvisor.data.length === 0) {
            return;
        }
        $scope.advisors = advisorAdvisor.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });


    $scope.AppClicked = function AppClicked(id) {
        applicationService.GetApplicationDocumentFile(id)
            .then(function succeeded(success) {
            },
                function failure(errormessage) {
                    console.log(errormessage);
                }
            );
    };

    $scope.uploadDocument = function uploadDocument(applicationID, productID, docID, doc, submitted) {
        // Use the productID (productID) and the applicationID ($scope.application.id)
        // to submit the document to the ApplicationDocumentController
        if (submitted !== true) {
            submitted = false;
        }

        applicationService.PostApplicationDocument(applicationID, productID, docID, doc, submitted).then(function succeeded(success) {
            doc.uploaded = true;
            doc = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
            doc.notUploaded = true;
        });
    };

    $scope.putApplicationStatus = function putApplicationStatus(applicationId, appliationStatusId) {
        applicationService.PutApplicationStatus(applicationId, appliationStatusId).then(function succeeded(success) {
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.uploadDocument = function uploadDocument(applicationID, productID, docID, doc, submitted) {
        //Use the productID (productID) and the applicationID ($scope.application.id) to submit the document to the ApplicationDocumentController (api/ApplicationDocuments/5/Product/2)

        if (submitted !== true) {
            submitted = false;
        }

        $scope.checkCharaters = function checkCharaters(characters) {
            console.log('advisorTextChange');
            if (!/^[a-zA-Z ]+$/.test(characters) || characters.length > 50) {
                $scope.incorrectEntry = true;
            }
            else {
                $scope.incorrectEntry = false;
            }
        };

        applicationService.PostApplicationDocument(applicationID, productID, docID, doc, submitted).then(function succeeded(success) {
            doc.uploaded = true;
            doc = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
            doc.notUploaded = true;
        });
    };

    $scope.registerItem = 0;
    //Start with 1 product and 1 supplier and 1 application document
    $scope.application = {
        applicationStatus_Id: 0,
        applicationType_Id: 0,
        client_Id: 0,
        deleted: false,
        id: 0,
        memberNumber: "",
        product_Id: 0,
        product_Supplier_Name: "",
        supplierId: 0
    };

    $scope.suppliers = [];
    $scope.products = [];
    $scope.selectedSupplier = 0;
    $scope.selectedProduct = 0;

    $scope.newFakeDocument = { "title": "New Document Title", "location": "No file chosen" };

    $scope.foundContacts = [];
    $scope.saId = "";
    $scope.AddressTypes = {};
    $scope.Province = {};
    $scope.Country = {};


    $scope.gotoPage = function gotoPage(page) {

        //This should submit if it's coming from the first page:
        if (page === 1 && $scope.registerItem === 0) {
            $scope.postOrSaveApplication();
        }
        else if (page === 2 && $scope.registerItem === 1) {
            // When the user is coming from the second step (1) of the registration process (going to the third), 
            // it should submit the products as a list and an application ID should be supplied:
            applicationService.PutApplicationProducts($scope.application.id, $scope.application).then(function succeeded(success) {
                $scope.registerItem = page;
            }, function failure(errormessage) {
                console.log(errormessage);
            });
        }
        else if (page === 3 && $scope.registerItem === 2) {
            // The user is coming from the second step and going to the third where documents can be uploaded:
            $scope.registerItem = page;
        }
        else if (page === 4) {
            // Submit the page
            $scope.registerItem = page;
            // Since everything is already saved, nothing really gets submitted, so We'll just take the user to the application detail of that page:
            $state.go('applicationDetail', { 'application': $scope.application.id });
        }
        else {
            // Maybe go back to previous page
            $scope.registerItem = page;
            $state.go('applicationDetail');
        }
    };

    $scope.goBackToApplications = function goBackToApplications() {
        $state.go('application');
    };

    applicationService.GetSuppliers().then(function succeeded(success) {
        $scope.suppliers = success.data;
        $scope.newSupplier = $scope.suppliers[0];
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // on appstatus select change
    $scope.updateApplicationStatus = function (item) {
        $scope.CurrentAppStatus = item;
        $scope.application.applicationStatus_Id = $scope.CurrentAppStatus.id;
    };

    // Application Types
    // on appTypes select change
    $scope.updateApplicationType = function (item) {
        $scope.CurrentAppType = item;
        $scope.application.applicationType_Id = $scope.CurrentAppType.id;
    };

    // Get Address Types:
    detailsService.GetAddressTypes().then(function succeeded(success) {
        $scope.AddressTypes = success.data;
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

    // When the selection changes, this updates:
    $scope.updateProducts = function updateProducts(id) {
        // This happens because the products change as you select a supplier, so the user should be stopped from going forward if this is 0
        // Ajax to the server and GET the products related to the supplier:
        applicationService.GetSupplierProducts(id).then(function succeeded(success) {
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
                    $scope.products[$scope.products.length - 1].supplier = { "id": id };
                }
            }
            $scope.ProductList = $scope.products;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    //Fetch contacts based on exissting ID number entered
    $scope.getUsersBySAId = function (saId) {
        applicationService.GetUsersBySAId(saId).then(
            function (success) {
                $scope.foundContacts = success.data;
                if (!$scope.oldClient) {
                    $scope.oldClient = $scope.application.client;
                }
                $scope.application.client = success.data[0];
            }, function (error) {
                $scope.errorMessages.push("Could not load users");
            });
    };

    $scope.showProducts = function (supplier, product) {
        return supplier.id === product.supplier.id;
    };

    $scope.updateProduct = function (data) {
        $scope.CurrentProduct = data;
        $scope.application.product_Id = data.id;
        $scope.application.product_Supplier_Name = data.name;
        $scope.application.products = data.id;
    };

    $scope.deleteProduct = function deleteProduct(product, productIndex, applicationId) {
        $scope.application.products.splice(productIndex, 1);
    };

    $scope.addAnotherAddress = function addAnotherAddress() {
        if (typeof $scope.application.client.addresses === "undefined")
            $scope.application.client.addresses = [];
        $scope.application.client.addresses.push({ "id": 0 });
    };

    $scope.saveApplication = function () {
        if ($scope.application) {
            applicationService.PutApplicationProducts($scope.application.id, $scope.application);
        }
        else {
            $scope.errormessage = "No application selected";
        }
    };

    $scope.addApplication = function addApplication(newSupplier) {
        var application = $scope.application;
        application.suppliers = application.suppliers || [];
        application.products = application.products || [];
        application.products.push({ supplierId: newSupplier.id });
        var supplier = application.suppliers.length > 0 ? undefined : newSupplier;

        for (var s in application.suppliers) {
            supplier = application.suppliers[s];
            if (supplier && supplier.id === newSupplier.id) {
                supplier = undefined;
                break;
            }
        }

        supplier && application.suppliers.push(newSupplier);

        if (typeof $scope.application.applicationSuppliers === "undefined")
            $scope.application.applicationSuppliers = [];
        var addMe = true;
        if (newSupplier) {
            for (var r = 0; r < $scope.application.applicationSuppliers.length; r++) {
                if ($scope.application.applicationSuppliers[r].supplierId === newSupplier.id) {
                    addMe = false;
                }
            }
            if (addMe === true)
                $scope.application.applicationSuppliers.push({
                    "applicationId": $scope.application.id,
                    "supplierId": newSupplier.id,
                    "memberNumber": ""
                });
        }
    };

    function filterSupplier(element, index, array) {
        return element.id === this.id ? element : false;
    }

    //Select a picture icon to fit the document:
    $scope.getDocumentPic = function getDocumentPic(pic) {
        return "img/picIcon.png";
    };

    //Receives a product json object which is an object of $scope.application
    $scope.openAddDocument = function openAddDocument(application) {
        //The applicationDocuments is an array, and if it's not defined as an array, then you should make it an array:
        if (typeof application.applicationDocuments === "undefined") {
            application.applicationDocuments = [];
        }

        application.applicationDocuments.push({ "title": "", "location": "", "ValidFromDate": function () { return new Date(Date.now()); }, "ValidToDate": function () { return new Date(Date.now()); } });
    };

    $scope.getDocumentsByApplication = function getDocumentsByApplication(applicationId) {
        applicationService.GetDocumentsByApplication(applicationId).then(function succeeded(success) {
            console.log(success);
            $scope.application.products.forEach(function (product) {
            });
            return true;
        }, function failure(errormessage) {
            console.log(errormessage);
            return false;
        });
    };

    //The following name change function is used since angular does not bind with the input type="file":
    $scope.fileNameChanged = function (ele) {
        $scope.newFakeDocument.location = ele.files[0].name;
    };

    //Save/Update a document of an application (expects a document object):
    $scope.saveDocument = function saveDocument(applicationId, applicaionDocument) {
        applicationService.PutApplicationDocument(applicationId, applicaionDocument).then(
            function succeeded(success) {
            }, function failure(errormessage) {
                $scope.errormessage = errormessage;
                console.log(errormessage);
                return false;
            });
    };

    //Delete a document of an application and client:
    $scope.deleteDocument = function deleteDocument(id) {
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

    // Get Title
    detailsService.GetContactTitles().then(function succeeded(success) {
        $scope.Titles = success.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });


    $scope.editingApplicationDetail = false;
    $scope.editOrSave = function editOrSave(save) {
        $scope.editingApplicationDetail = !$scope.editingApplicationDetail;
        if (!save) {
            $scope.advisorChange = false;
            if ($scope.oldClient) {
                $scope.application.client = $scope.oldClient;
            }
            return;
        }

        $scope.postOrSaveApplication();
        $scope.getApplicationAdvisorHistory();
        $scope.getApplicationAdvisorEditHistory();
    };

    $scope.editOrSaveforMemberNumber = function editOrSaveforMemberNumber(save) {
        $scope.editingApplicationDetail = !$scope.editingApplicationDetail;
        advisorService.GetSuppliersById($scope.application.supplierId).then(function succeeded(success) {
            $scope.application.suppliers[0] = success.data;
        });

        if (!save) {
            if ($scope.oldClient) {
                $scope.application.client = $scope.oldClient;
            }
            return;
        }

        $scope.postOrSaveMemberNumber();
    };

    $scope.seeAdvisor = function seeAdvisor() {
        $scope.viewAdvisorSelected = true;
        $scope.getAdvisors($scope.paging);

    };

    $scope.postOrSaveApplication = function postOrSaveApplication() {
        var newApplicationObject = {};

        if ($scope.application) {
            newApplicationObject = $scope.application;
            newApplicationObject.suppliers = null;
        }

        advisorService.GetAdvisorByApplicationId($scope.application.id).then(function succeeded(success) {
            var advisorIdForHistory = 0;
            
            if ($scope.gotAdvisor) {
                advisorIdForHistory = $scope.application.advisor.id;
            }
            else {
                advisorIdForHistory = $scope.application.advisor.contact.advisor_Id;
            }
            $scope.OldAdvisor = success.data;

            if ($scope.adviserUpdateCheck) {
                // put Advisor history
                // post new history entry
                // post edit history 
                // update old advisor end date and new start date  

                applicationService.PutApplication($scope.application.id, newApplicationObject).then(function succeeded() {
                    advisorService.PutApplicationAdvisorHistory(
                        $scope.application.id,
                        $scope.effectiveStartDate,
                        $scope.effectiveEndDatePreviousAdvisor,
                        $scope.application.advisor.id);

                    $scope.application.modifiedDate = $scope.effectiveStartDateNewAdvisor;

                    applicationService.postApplicationAdvisorEditHistory($scope.application.id);
                }, function failure(errormessage) {
                    alert("Error: Posting advisor application history");
                    console.log(errormessage);
                });
            }
            else {
                if ($scope.updateEffectiveStartDateAvisorHistory && $scope.updateEffectiveEndDateAvisorHistory) {
                    // Update applicationAdvisorHistory's effective date 
                    if ($scope.effectiveEndDate) {
                        advisorService.PutApplicationAdvisorHistory($scope.application.id, $scope.effectiveStartDate, $scope.effectiveEndDate, advisorIdForHistory);
                    }
                    else {
                        advisorService.PutApplicationAdvisorHistory($scope.application.id, $scope.effectiveStartDate, Date.now(), advisorIdForHistory);
                    }
                }
                else if ($scope.updateEffectiveStartDateAvisorHistory) {
                    if ($scope.effectiveEndDate) {
                        advisorService.PutApplicationAdvisorHistory($scope.application.id, $scope.effectiveStartDateUpdated, $scope.effectiveEndDate, advisorIdForHistory);
                    }
                    else {
                        advisorService.PutApplicationAdvisorHistory($scope.application.id, $scope.effectiveStartDateUpdated, null, advisorIdForHistory);
                    }
                }
                else if ($scope.effectiveEndDate) {
                    advisorService.PutApplicationAdvisorHistory($scope.application.id, $scope.effectiveStartDate, $scope.effectiveEndDate, advisorIdForHistory);
                }

                applicationService.PutApplication($scope.application.id, newApplicationObject).then(function succeeded(success) {
                    if ($scope.application.advisor_Id !== $scope.OldAdvisor) {
                        advisorService.PostApplicationAdvisorHisotry(
                            $scope.application.id,
                            $scope.application.advisor.id,
                            $scope.OldAdvisor,
                            $scope.effectiveStartDate,
                            $scope.effectiveEndDate).then(function succeeded(success) {

                            }, function failure(errormessage) {
                                console.log(errormessage);
                            });
                        applicationService.postApplicationAdvisorEditHistory($scope.application.id);
                    }
                }, function failure(errormessage) {
                    alert("Error: Posting advisor application history");
                    console.log(errormessage);
                });

                applicationService.postApplicationAdvisorEditHistory($scope.application.id);
            }
        }, function failure(errormessage) {
            alert("Error: Getting advisor id");
            console.log(errormessage);
        });
    };

    $scope.updateEffectiveStartDate = function updateEffectiveStartDate(effectiveStartdate) {
        $scope.effectiveStartDateUpdated = effectiveStartdate;
        $scope.updateEffectiveStartDateAvisorHistory = true;
    };

    $scope.updateEffectiveEndDate = function updateEffectiveEndDate(effectiveEndDate) {
        $scope.effectiveEndDate = effectiveEndDate;
        $scope.updateEffectiveEndDateAvisorHistory = true;
    };

    $scope.updateEffectiveStartDateNewAdvisor = function updateEffectiveStartDateNewAdvisor(effectiveStartdate) {
        $scope.effectiveStartDateNewAdvisor = effectiveStartdate;
    };

    $scope.updateEffectiveEndDatePreviousAdvisor = function updateEffectiveEndDatePreviousAdvisor(effectiveEndDate) {
        $scope.effectiveEndDatePreviousAdvisor = effectiveEndDate;
    };

    $scope.postOrSaveMemberNumber = function postOrSaveMemberNumber() {
        applicationService.PutApplicationProducts($scope.application.id, $scope.application).then(function succeeded(success) {
            console.log(success);
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.deleteApplicationDocuments = function deleteApplicationDocuments(id, AppId, DocType) {
        applicationService.deleteApplicationDocuments(id, AppId, DocType).then(function succeeded(result) {
            applicationService.GetApplication($state.params.application).then(function succeeded(application) {
                applicationService.GetApplicationDocumentsByApplicationId(AppId).then(function succeeded(success) {
                    application.applicationDocuments = success.data;

                    if (application.applicationDocuments !== null) {
                        var appDoc = null;
                        for (var i = 0; i < application.applicationDocuments.length; i++) {
                            appDoc = application.applicationDocuments[i];
                            appDoc.validFromDate = new Date(appDoc.validFromDate);
                            appDoc.validToDate = new Date(appDoc.validToDate);

                            if (application.applicationType_Id === 1) {
                                if (appDoc.documentTypeId === 37 && appDoc.uploaded === true) {
                                    // alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }
                                if (appDoc.documentTypeId === 38 && appDoc.uploaded === true) {
                                    // alert("BANK DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }
                                if (appDoc.documentTypeId === 39 && appDoc.uploaded === true) {
                                    // alert("BROKER NOTE DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 3) {
                                    // alert("New application complete")
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }
                            if (application.applicationType_Id === 6) {
                                if (appDoc.documentTypeId === 37 && appDoc.uploaded === true) {
                                    // alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }
                                if (appDoc.documentTypeId === 38 && appDoc.uploaded === true) {
                                    // alert("BANK DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }
                                if (appDoc.documentTypeId === 39 && appDoc.uploaded === true) {
                                    // alert("BROKER NOTE DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 3) {
                                    // alert("New application-Solidarity complete")
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }

                            if (application.applicationType_Id === 2) {
                                if (appDoc.documentTypeId === 7 && appDoc.uploaded === true) {
                                    // alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 1) {
                                    // alert("Broker complete")
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }
                            if (application.applicationType_Id === 7) {
                                if (appDoc.documentTypeId === 7 && appDoc.uploaded === true) {
                                    // alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 1) {
                                    // alert("Broker-Solidarity complete")
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }

                            if (application.applicationType_Id === 3) {
                                if (appDoc.documentTypeId === 40 && appDoc.uploaded === true) {
                                    // alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 1) {
                                    // alert("Group appointment complete")
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }

                            if (application.applicationType_Id === 4) {
                                if (appDoc.documentTypeId === 37 && appDoc.uploaded === true) {
                                    // alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 1) {
                                    // alert("Incomplete complete")
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }

                            if (application.applicationType_Id === 5) {
                                if (appDoc.documentTypeId === 42 && appDoc.uploaded === true) {
                                    // alert("CMS DOC IS THERE");
                                    documentCount = documentCount + 1;
                                }

                                if (documentCount >= 1) {
                                    // alert("Voice logged complete")
                                    application.applicationStatus_Id = 6;
                                    $scope.putApplicationStatus(application.id, application.applicationStatus_Id);
                                }
                            }
                            appDoc = null;
                        }

                        // set CurrentAppStatus for documents
                        for (var a = 0; a < $scope.appStatus.length; a++) {
                            if ($scope.appStatus[a].id === $scope.application.applicationStatus_Id) {
                                $scope.updateApplicationStatus($scope.appStatus[a]);
                            }
                        }
                    }
                    $scope.application = application;
                }, function failure(errormessage) {
                    console.log(errormessage);
                    application.applicationDocuments = null;
                    documentCount = 0;
                });
            });
            alert("Deleted");
        },
        function failure(errormessage) {
            console.log(errormessage);
            alert("Not Deleted , add new document to delete this.");
        });
    };

    $scope.setAdvisorChange = function setAdvisorChange(changeIt) {
        $scope.advisorChange = changeIt;
    };

    $scope.updateAdvisorDetails = function updateAdvisorDetails() {
        if ($scope.advisorChange) {
            $scope.application.advisor.contact = $scope.newAdvisor.contact;
            $scope.application.advisor.firstName = $scope.newAdvisor.firstName;
            $scope.application.advisor.lastName = $scope.newAdvisor.lastName;
            $scope.application.advisor.idNumber = $scope.newAdvisor.idNumber;
            $scope.application.advisor.id = $scope.newAdvisor.id;
            $scope.adviserUpdateCheck = true;
            $scope.advisorChange = false;
        }
    };
}]);
