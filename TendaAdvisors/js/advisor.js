tendaApp.controller('advisorController', ['$scope', 'clientsService', 'advisorService', 'detailsService', 'applicationService', '$state', '$stateParams', function ($scope, clientsService, advisorService, detailsService, applicationService, $state, $stateParams) {
    $scope.emailRegex = /^\S+@\S+$/;
    $scope.registerItem = 0;

    // REDIRECTS
    if ($state.params.advisor === null) {
        $state.go('advisors');
        return;
    }

    if ($scope.registerItem === null || $scope.registerItem === "undefined") {
        $scope.registerItem = 0;
    }

    $scope.gotoPage = function gotoPage(page) {
        if (page === 0) {
            $scope.registerItem = page;
        }
        else if (page === 1) {
            $scope.registerItem = page;
        }
        else if (page === 2) {
            $scope.registerItem = page;
        }
    };

    $scope.putAdviserStatus = function putAdviserStatus(advisorId, advisorStatusId) {
        advisorService.PutAdvisorStatus(advisorId, advisorStatusId).then(function succeeded(success) {
        }, function failure(errormessage) {
        });
    };

    $scope.doSearchUnderSupervision = function doSearchUnderSupervision(searchWhere) {
        if (!!searchWhere && searchWhere.trim().length >= 4)
            $scope.getContactsUnderSupervision(searchWhere);
    };

    $scope.getContactsUnderSupervision = function getContactsUnderSupervision(searchQuery) {
        clientsService.GetBasicContact(searchQuery).then(function succeeded(contact) {
            $scope.Basiccontacts = contact.data;
            for (var i = 0; i < $scope.advisorsNotAdvisorTypeComapany.length; i++) {

                if ($scope.advisorsNotAdvisorTypeComapany[i].id === $scope.Basiccontacts[0].id) {
                    $scope.selectedAdvisor = $scope.Basiccontacts[0];
                    $scope.getAdvisorId($scope.selectedAdvisor.id);
                    $scope.isAdvisorContact = true;
                    break;
                }
            }
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.getAdvisorId = function getAdvisorId(id) {
        advisorService.GetAdvisorIdTypeAdvisor(id).then(function success(response) {
            $scope.advisorIdPast = response.data;
        },
            function failure(error) {
                console.error(error);
            });
    };


    advisorService.GetAdvisorsNotAdvisorTypeComapany().then(function succeeded(success) {
        $scope.advisorsNotAdvisorTypeComapany = success.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // FIELDS
    var documentCount = 0;
    $scope.advisor = {};
    $scope.advisors = [];
    $scope.licenses = [];
    $scope.documents = [];
    $scope.allLicences = [];
    $scope.AddressTypes = [];
    $scope.Provinces = [];
    $scope.Countries = [];
    $scope.AddressType = null;
    $scope.origAdvisor = {};
    $scope.origLicenses = [];
    $scope.origDocuments = [];
    $scope.suppliers = [];
    $scope.notAdvisorEditing = true;
    $scope.incorrectEntry = false;
    $scope.viewLicenses = false;
    $scope.advisorTypes = [];
    $scope.documentTypeList = [];
    $scope.advisor2LicenceType = {};
    $scope.item = "";
    $scope.advisorRole = [{ name: "Admin", checked: false }, { name: "Adviser", checked: false }];
    $scope.currentSupplier = null;

    // PAGE FIELD INITIALIZATION
    // GET ADVISOR TYPES
    advisorService.GetAdvisorTypes().then(function succeeded(advisorTypes) {
        $scope.advisorTypes = [];
        advisorTypes.data.forEach(function (item, index) {
            $scope.advisorTypes.push(item.title);
        });
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // GET ADVISOR STATUSES
    advisorService.GetAdvisorStatuses().then(function succeeded(advisorStatuses) {
        $scope.advisorStatuses = [];
        advisorStatuses.data.forEach(function (item, index) { $scope.advisorStatuses.push(item.name); });
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // GET TITLES
    detailsService.GetContactTitles().then(function succeeded(success) {
        $scope.Titles = [];
        success.data.forEach(function (item, index) { $scope.Titles.push(item.name); });
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // GET BANK NAMES
    detailsService.GetBankName().then(function succeeded(success) {
        $scope.Banks = [];
        success.data.forEach(function (item, index) { $scope.Banks.push(item.name); });
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // GET BANK CODES
    detailsService.GetBankBranchCodes().then(function succeeded(success) {
        $scope.BankCode = [];
        success.data.forEach(function (item, index) { $scope.BankCode.push(item.name); });
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // GET DOCUMENTS TYPES
    advisorService.GetDocumentTypes($state.params.advisor).then(function succeeded(documentTypes) {
        $scope.documentTypeList = documentTypes.data;
    },
    function failure(errormessage) {
        console.error(errormessage);
    });

    // GET ADVISORS
    advisorService.GetAdvisorsAdvisor().then(function success(success) {
        $scope.advisors = success.data;
    },
    function failure(error) {
        console.error(error);
    });

    //// ADRESS DETAILS
    detailsService.GetAddressTypes().then(function succeded(success) {
        success.data.forEach(function (item, index) {
            $scope.AddressTypes.push(item.name);
        });
    }, function failure(errormessage) {
        console.log(errormessage);
    });


    detailsService.GetProvinces().then(function succeded(success) {
        success.data.forEach(function (item, index) {
            $scope.Provinces.push(item.name);
        });
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    detailsService.GetCountries().then(function succeded(success) {
        success.data.forEach(function (item, index) {
            $scope.Countries.push(item.name);
        });
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    // ADVISOR FIELDS INITIALIZATION
    // ADVISOR DETAILS
    advisorService.GetAdvisorDetails($state.params.advisor).then(function success(result) {
        $scope.advisor = result.data;
        if ($scope.advisor.effectiveStartDate !== null) {
            $scope.advisor.effectiveStartDate = $scope.advisor
                .effectiveStartDate.substring(0, $scope.advisor.effectiveStartDate.indexOf("T"));
        }
        else {
            $scope.advisor.effectiveStartDate = "";
        }

        if ($scope.advisor.effectiveEndDate !== null) {
            $scope.advisor.effectiveEndDate = $scope.advisor.effectiveEndDate.substring(0, $scope.advisor.effectiveEndDate.indexOf("T"));
        }
        else {
            $scope.advisor.effectiveEndDate = "";
        }

        $scope.item = $scope.advisor.isAdmin ? "Admin" : "Adviser";
    },
    function failure(error) {
        console.error(error);
    });


    $scope.getAdvisorLicenses = function getAdvisorLicenses() {
        advisorService.GetAdvisorLicenses($state.params.advisor).then(function success(result) {
            $scope.advisor2LicenceType = result.data;
            $scope.advisor2LicenceType.forEach(function (item, index) {
                item.validCommissionFromDate = item.validCommissionFromDate.substring(0, item.validCommissionFromDate.indexOf("T"));
                if (item.validCommissionToDate !== null) {
                    item.validCommissionToDate = item.validCommissionToDate.substring(0, item.validCommissionToDate.indexOf("T"));
                }

                item.validFromDate = item.validFromDate.substring(0, item.validFromDate.indexOf("T"));
                item.validToDate = item.validToDate.substring(0, item.validToDate.indexOf("T"));
            });
        },
            function failure(error) {
                console.error(error);
            });
    };

    advisorService.GetAdvisorLicenses($state.params.advisor).then(function success(result) {
        $scope.advisor2LicenceType = result.data;
        $scope.advisor2LicenceType.forEach(function (item, index) {
            item.validCommissionFromDate = item.validCommissionFromDate.substring(0, item.validCommissionFromDate.indexOf("T"));
            if (item.validCommissionToDate !== null) {
                item.validCommissionToDate = item.validCommissionToDate.substring(0, item.validCommissionToDate.indexOf("T"));
            }

            item.validFromDate = item.validFromDate.substring(0, item.validFromDate.indexOf("T"));
            item.validToDate = item.validToDate.substring(0, item.validToDate.indexOf("T"));

        });
    },
    function failure(error) {
        console.error(error);
    });

    // ADVISOR DOCUMENTS
    advisorService.GetCurrentAdvisorDocuments($state.params.advisor).then(function success(result) {
        $scope.documents = result.data;
        $scope.documents.forEach(function (item, index) {
            switch (item.documentTypeId) {
                case 10: case 13: case 18: documentCount++; break;
            }
        });

        if (documentCount >= 3) {
            $scope.advisor.advisorStatusName = "Complete";
            $scope.putAdviserStatus($state.params.advisor, 5);
        }
    }, function failure(error) {
        console.error(error);
    });

    $scope.getAdvisorLicenseTypes = function getAdvisorLicenseTypes() {
        advisorService.GetAdvisorShareUnderSupervision($state.params.advisor).then(function succeeded(success) {
            $scope.advisor2LicenceType = success.data;
            $scope.getSupervisors();
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.getAdvisorSupervision = function getAdvisorSupervision(advisorId) {
        $scope.GetAdvisor = function GetAdvisor(advisorId) {
            advisorService.GetAdvisor(advisorId).then(function succeeded(success) {
                $scope.advisorId = advisorId.data;
            }, function failure(errormessage) {
                console.log(errormessage);
            });
        };
    };

    // ENABLE EDIT AND SAVE
    $scope.editAdvisorPageAndSave = function editAdvisorPageAndSave(save) {
        $scope.notAdvisorEditing = !$scope.notAdvisorEditing;
        $scope.advisor.province_Id = $scope.advisor.province_Id;
        $scope.advisor.country_id = $scope.advisor.country_id;
        $scope.advisor.addressType_Id = $scope.advisor.addressType_Id;
        $scope.origAdvisor = angular.copy($scope.advisor);

        if (save) {
            $scope.advisor.isAdmin = document.getElementById("input-Admin").checked;
            advisorService.PostAdvisorDetails($scope.advisor).then(function success(result) {
            }, function failure(error) {
                console.log(error);
            });
        }
    };

    // DISABLE EDIT
    $scope.cancelEdit = function cancelEdit() {
        $scope.advisor = angular.copy($scope.origAdvisor);
        $scope.licenses = angular.copy($scope.origLicenses);
        $scope.documents = angular.copy($scope.origDocuments);

        $scope.origAdvisor = {};
        $scope.origLicenses = {};
        $scope.origDocuments = {};

        $scope.notAdvisorEditing = true;
    };

    // SAVE LICENSES
    $scope.saveCommisionSplit = function saveCommisionSplit(comSplit, supervisorId) {
        var myDate = new Date();
        myDate.getUTCDate();

        if (comSplit.underSupervision !== true) {
            comSplit.underSupervision = false;
        }

        if (comSplit.advisorShare === null || comSplit.advisorShare === "") {
            comSplit.advisorShare = 0;
        }

        if (comSplit.advisorSupervision === null || comSplit.advisorSupervision === "") {
            comSplit.advisorSupervision = 53;
        }

        if (comSplit.validSupervisionFromDate === null || comSplit.validSupervisionFromDate === "") {
            comSplit.validSupervisionFromDate = null;
        }

        if (comSplit.validSupervisionToDate === null || comSplit.validSupervisionToDate === "") {
            comSplit.validSupervisionToDate = null;
        }

        if (comSplit.validCommissionFromDate === null || comSplit.validCommissionFromDate === "") {
            comSplit.validCommissionFromDate = myDate;
        }

        if (comSplit.validCommissionToDate === null || comSplit.validCommissionToDate === "") {
            comSplit.validCommissionToDate = null;
        }

        if (supervisorId !== undefined) {
            comSplit.supervisorId = supervisorId.id;
        }

        advisorService.PostAdvisorShareObject(comSplit).then(function succeeded(advisor) {
            comSplit.uploaded = true;
            comSplit.notUploaded = false;
            $scope.getAdvisorLicenses();
        }, function failure(errormessage) {
            comSplit.notUploaded = true;
            comSplit.uploaded = false;
        });
    };

    $scope.PostAdvisorShareOnSupplier = function (product, id, licenseID, supplierName, productName, advisorShare,
        validCommissionFromDate, validCommissionToDate, underSupervision,
        advisorSupervision, validSupervisionFromDate, validSupervisionToDate) {
        var myDate = new Date();
        myDate.getUTCDate();

        if (underSupervision !== true) {
            underSupervision = false;
        }

        if (advisorShare === null) {
            advisorShare = 0;
        }

        if (advisorSupervision === null) {
            advisorSupervision = 59;
        }

        if (validSupervisionFromDate === null) {
            validSupervisionFromDate = myDate;

        }
        if (validSupervisionToDate === null) {
            validSupervisionToDate = null;
        }

        if (validCommissionFromDate === null) {
            validCommissionFromDate = myDate;
        }
        if (validCommissionToDate === null) {
            validCommissionToDate = null;
        }

        $scope.AdvisorShareObject = {};
        $scope.AdvisorShareObject.advisorId = $scope.advisor.advisorId;
        $scope.AdvisorShareObject.LicenseTypeId = licenseID;
        $scope.AdvisorShareObject.supplier = supplierName;
        $scope.AdvisorShareObject.product = productName;
        $scope.AdvisorShareObject.Share = advisorShare;
        $scope.AdvisorShareObject.underSupervision = underSupervision;
        $scope.AdvisorShareObject.supervisorId = advisorSupervision;
        $scope.AdvisorShareObject.validCommissionFromDate = validCommissionFromDate;
        $scope.AdvisorShareObject.validCommissionToDate = validCommissionToDate;
        $scope.AdvisorShareObject.ValidFromDate = validSupervisionFromDate;
        $scope.AdvisorShareObject.ValidToDate = validSupervisionToDate;

        $scope.savedProduct = false;
        advisorService.PostAdvisorShareObject($scope.AdvisorShareObject).then
            (
            function succeeded(advisor) {
                product.uploaded = true;
                product.notUploaded = false;
                $scope.savedProduct = true;
                console.log("Succeeded putting advisor share " + JSON.stringify(advisor));
                $scope.getAdvisorLicenses();
            },
            function failure(errormessage) {
                product.notUploaded = true;
                product.uploaded = false;
                $scope.savedProduct = false;
                console.log("failed putting advisor share" + errormessage);
            }
            );
    };

    // SAVE DOCUMENTS
    $scope.editDocumentsPageAndSave = function editDocumentsPageAndSave(save) {
        
    };

    $scope.checkCharaters = function checkCharaters(characters) {
        if (!/^[a-zA-Z ]+$/.test(characters) || characters.length > 50) {
            $scope.incorrectEntry = true;
        }
        else {
            $scope.incorrectEntry = false;
        }
    };

    $scope.checkTaxDirective = function checkTaxDirective(characters) {
        if (!!characters && !/^[0-9.,]+$/.test(characters)) {
            $scope.incorrectEntryTax = true;
        }
        else {
            $scope.incorrectEntryTax = false;
        }
    };

    
    $scope.getAdvisor = function getAdvisor() {
        return advisorService.GetAdvisor($state.params.advisor).then(
            function succeeded(advisor) {
                if (advisor.data && advisor.data.advisorDocuments) {
                    advisor.data.advisorDocuments.forEach(function (document) {
                        if (document && document.documentType === null && document.documentTypeId !== null) {
                            document.documentType = { id: document.documentTypeId };
                        }
                        if (document.documentType.id === 10 && document.uploaded === true) {
                            documentCount = documentCount + 1;
                        }
                        if (document.documentType.id === 13 && document.uploaded === true) {
                            documentCount = documentCount + 1;
                        }
                        if (document.documentType.id === 18 && document.uploaded === true) {
                            documentCount = documentCount + 1;
                        }
                    });
                }

                if (documentCount >= 3) {
                    advisor.data.advisorStatus.id = 5;
                    $scope.putAdviserStatus(advisor.data.id, advisor.data.advisorStatus.id);
                }

                if (advisor.data.user.isAdmin === false) {
                    $scope.selectedRole.item = "Advisor";
                    $scope.advisorRole[0].checked = false;
                    $scope.advisorRole[1].checked = true;
                    $scope.advisorRole.isAdvisor = true;
                    $scope.advisorRole.isAdmin = false;

                }

                if (advisor.data.user.isAdmin === true) {
                    $scope.selectedRole.item = "Admin";
                    $scope.advisorRole[0].checked = true;
                    $scope.advisorRole[1].checked = false;
                    $scope.advisorRole.isAdmin = true;
                    $scope.advisorRole.isAdvisor = false;
                }

                //Advisor Status Check
                $scope.advisor = advisor.data;
            },
            function failure(errormessage) {
                console.log(errormessage);
                $scope.advisor = null;
            }
        );
    };

    $scope.paging = {
        config: { pageIndex: 1, pageSize: 100 },
        pageSizes: [1, 2, 10, 20, 50, 100],
        totalRows: 0,
        pages: [],
        updatePages: function () {
            this.pages = [];

            for (var i = 1; i <= this.totalRows / (this.totalRows < this.config.pageSize ? this.totalRows : this.config.pageSize); i++) {
                this.pages.push(i);
            }

            if (this.config.pageIndex > this.pages.length) {
                this.config.pageIndex = this.pages.length;
            }

            return this.pages;
        }
    };

    $scope.getAdvisors = function (paging) {
        advisorService.GetAdvisors(paging.config).then(function succeeded(dto) {
            paging.totalRows = dto.count;
            $scope.advisors = dto.advisors;
            paging.updatePages();
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };


    $scope.getSupervisors = function getSupervisor() {
        for (var i = 0; i < $scope.advisor2LicenceType.length; ++i) {
            (function (index) {
                clientsService.GetContactDetails($scope.advisor2LicenceType[index].advisor).then(function success(result) {
                    $scope.advisor2LicenceType[index].advisor = result.data;
                },
                function failure(result) {
                    $scope.advisor2LicenceType[index].advisor = {};
                });
            })(i);
        }
    };
    
    //Get application
    $scope.getApplications = function getApplications() {
        applicationService.GetApplications().then(
            function succeeded(applications) {
                if (applications.length === 0) {
                    return;
                }
                $scope.applications = applications;
            }, function failure(errormessage) {
                console.log(errormessage);
            });
    };


    $scope.applicationClicked = function applicationClicked(id) {
        $state.go('applicationDetail', { 'application': id });
    };

    $scope.AdvisorAppClicked = function AdvisorAppClicked(id) {
        advisorService.GetAdvisorDocumentFile(id).then(function succeeded(success) {
        },
            function failure(errormessage) {
                console.log(errormessage);
            }
        );
    };

    //When the selection changes, this updates:
    $scope.sendAdviserMemberCode = function sendAdviserMemberCode(advisorId, supplierId) {
        var code = document.getElementById('newId').value;
        var advisorSupplierCode = { "advisorId": advisorId, "supplierId": supplierId, "advisorCode": code };

        advisorService.PostSendAdviserMemberCode(advisorSupplierCode).then(function succeeded(success) {
        },
            function failure(errormessage) {
                console.log(errormessage);
            });
    };
   
    $scope.editAdviserMemberCode = function editAdviserMemberCode(advisorId, supplierId) {
        var code = document.getElementById('editId').value;
        var advisorSupplierCode = { "advisorId": advisorId, "supplierId": supplierId, "advisorCode": code };

        advisorService.PutAdvisorSupplierCode(advisorId, supplierId, advisorSupplierCode).then(function succeeded(success) {
        },
            function failure(errormessage) {
                console.log(errormessage);
            });
    };
    
    $scope.goToAdvisorsPage = function goToAdvisorsPage() {
        $state.go('advisors');
    };

    //Upload Documents
    $scope.checkPercentage = function checkPercentage(product, shareAmount) {
        if (!(shareAmount >= 0 && shareAmount < 100)) {
            product.incorrectEntry = true;
        }
        else {
            product.incorrectEntry = false;
        }
    };

    $scope.uploadDocument = function uploadDocument(advisorId, documentTypeID, document, submitted) {
        if (submitted !== true) {
            submitted = false;
        }

        advisorService.PostAdvisorDocument(advisorId, documentTypeID, document, submitted).then(function succeeded(success) {
            document.uploaded = true;
            document = success.data;
        },
            function failure(errormessage) {
                console.log(errormessage);
                document.notUploaded = true;
            });
    };
    
    $scope.checkRightAmount = function checkRightAmount(allowance) {
        if (!/^[0-9.,]+$/.test(allowance)) {
            $scope.allowanceIncorrect = true;
        }
        else {
            $scope.allowanceIncorrect = false;
        }
    };

    $scope.getSuppliers = function getSuppliers() {
        advisorService.GetSuppliers().then(function succeeded(success) {
            $scope.suppliers = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    $scope.changeAdvisorSuppliers = function changeAdvisorSuppliers() {
        $scope.getSuppliers();
        $scope.availableProducts = null;
        $scope.gotoPage(2);
    };

    $scope.getProducts = function getProducts(supplier) {
        $scope.currentSupplier = supplier;
        $scope.fetchProductsByLicenses(supplier.products[0].licenseType.id, supplier.id);
    };

    $scope.fetchProductsByLicenses = function fetchSuppliersByLicenses(licenseList, id) {
        advisorService.FetchProductsByLicenseId(licenseList, id).then(
            function succeeded(products) {
                products.data.forEach(function (item, index) { item.name = ""; });
                $scope.availableProducts = products.data;
                return products.data;
            },
            function failure(errormessage) {
                console.log(errormessage);
            }
        );
    };

    $scope.deleteAdviDoc = function deleteAdviDoc(id, AdviId, DocType) {
        advisorService.deleteAdviDoc(id, AdviId, DocType).then(function succeeded(success) {
            advisorService.GetCurrentAdvisorDocuments(AdviId).then(function succeeded(result) {
                $scope.documents = result.data;
                $scope.documents.forEach(function (item, index) {
                    switch (item.documentTypeId) {
                        case 10: case 13: case 18: documentCount++; break;
                    }
                });

                if (documentCount >= 3) {
                    $scope.advisor.advisorStatusName = "Complete";
                    $scope.putAdviserStatus($state.params.advisor, 5);
                }
                alert("Deleted");
            }, function failure(error) {
                console.error(error);
            });
        },
            function failure(errormessage) {
                console.log(errormessage);
                alert("Not Deleted , add new document to delete this.");
            });
    };
}]);
