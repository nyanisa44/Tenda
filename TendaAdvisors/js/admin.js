tendaApp.controller('adminController', ['$scope', 'adminService', '$state', 'advisorService', function ($scope, adminService, $state, advisorService) {
    $scope.registerItem = 0;
    $scope.importTypeChoice = { id: 0 };
    $scope.importCompanyChoice = { id: 0 };
    $scope.uploads = {
        file: {
            id: 0,
            fileName: "",
            CompanyId: 0,
            ImportTypeId: $scope.importTypeChoice.id
        }
    };
    $scope.suppliers = {
        default: "",
        supplierList: []
    };
    $scope.licenseTypes = [];
    $scope.licenseCategories = [];
    $scope.selectedProductID = [];
    $scope.FunctionResult = null;
    $scope.ProcessError = "";
    $scope.isValidRun = false;
    $scope.dataLoaded = true;
    $scope.processStatus = "";
    $scope.importErrorMessage = "";
    // extra drop down when uploading a member list boolean check to show or not 
    $scope.applicationWithMemberListUploadCheck = false;
    $scope.supplierDefault = "";

    $scope.gotoPage = function gotoPage(page) {
        $scope.applicationWithMemberListUploadCheck = false;
        if (page === 1 && $scope.registerItem === 0) {
            $scope.registerItem = page;
        }
        else if (page === 2 && $scope.registerItem === 1) {
            $scope.registerItem = page;
        }
        else if (page === 3 && $scope.registerItem === 2) {
            $scope.registerItem = page;
        }
        else if (page === 4) {
            $scope.registerItem = page;
            $state.go('admin', { 'import': page });
        }
        else {
            $scope.registerItem = page;
        }
    };

    $scope.clearfieldMap = function (file) {
        file.fieldMap.mappingData = [];
    };

    $scope.goToAdmin = function goToAdmin() {
        $state.go('admin');
    };

    $scope.getImportTypes = function getImportTypes() {
        $scope.importErrorMessage = "";
        adminService.GetImportTypes().then(function succeeded(success) {
            $scope.importTypes = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };
    // Run on load
    $scope.getImportTypes();

    $scope.getImportCompanies = function getImportCompanies() {
        adminService.GetImportCompanies().then(function succeeded(success) {
            $scope.importCompanies = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    //Run on load
    $scope.getImportCompanies();

    $scope.getFilesToImport = function getFilesToImport(id) {
        if (id === 2) {
            adminService.MemberListExceptionFile().then(function succeeded(success) {
                var header = success.headers('Content-Disposition');
                var fileName = header.split("=")[1].replace(/\"/gi, '');

                var blob = new Blob([success.data],
                    { type: 'application/vnd.ms-excel;charset=UTF-8' });
                var objectUrl = (window.URL || window.webkitURL).createObjectURL(blob);
                var link = angular.element('<a/>');
                link.attr({
                    href: objectUrl,
                    download: fileName
                })[0].click();
                alert("Exception member list downloaded successfully");
            }, function failure(errormessage) {
                alert("Error processing member exception list");
                $scope.importedFiles = [];
            });
        }
        else {
            adminService.GetImportFileListByImportTypeId(id).then(function succeeded(success) {
                $scope.importedFiles = success.data;
                $scope.importedFiles.forEach(function (file) {
                    file.fieldMap = angular.fromJson(file.fieldMap);
                    if (file.fieldMap !== null) {
                        file.fieldMap.mappingData = [];
                    }
                });
            }, function failure(errormessage) {
                console.log("import file error: ", errormessage);
                $scope.importedFiles = [];
            });
        }
    };

    $scope.approveImport = function approveImport(Id, SupplierId, FromDate, ToDate) {
        if (FromDate === null || ToDate === null || typeof FromDate === "undefined" || typeof ToDate === "undefined") {
            $scope.ProcessError = "You need to add approval dates.";
            return;
        }
        
        for (var i = 0; i < $scope.importedFiles.length; i++) {
            if ($scope.importedFiles[i].id === Id) {
                $scope.importedFiles[i].status = "Started Approving";
            }
        }

        adminService.ApproveImport(Id, SupplierId, FromDate, ToDate)
            .then(function succeeded(success) {
                $scope.setImportFileStatus();
                $scope.getFilesToImport(1);
            }, function failure(errormessage) {
                console.log("Import files Approve Failed: ", errormessage);
            });
    };

    $scope.setImportFileStatus = function setImportFileStatus() {
        adminService.GetImportFileListByImportTypeId($scope.importTypeChoice.id)
            .then(function succeeded(success) {
                for (var i = 0; i < success.data.length; i++) {
                    $scope.importedFiles[i].status = success.data[i].status;
                }
            }, function failure(errorMessage) {
                console.log("Set Import Status error: ", errorMessage);
            });
    };

    $scope.updateFile = function updateFile(callingObject) {
        $scope.applicationWithMemberListUploadCheck = false;
        if (callingObject["companyId"]) {
            $scope.uploads.file.CompanyId = callingObject.companyId;
        } else if (callingObject["imporTypeId"]) {
            $scope.uploads.file.ImportTypeId = callingObject.imporTypeId;
            if ($scope.uploads.file.ImportTypeId === 2) {
                $scope.applicationWithMemberListUploadCheck = true;
                var confirmedClearMemberExceptionList = confirm("Would you like to clear the Exception Member List first?");
                if (confirmedClearMemberExceptionList) {
                    adminService.MemberListExceptionTableTruncation()
                        .then(function succeeded(success) { }
                            , function failure(errormessage) {
                                alert(`Error clearing the member exception list \n Error: ${errormessage}`);
                                $scope.importedFiles = [];
                            });
                }
            }
        }
    };

    $scope.uploadImport = function uploadImport(file) {
        adminService.PostFileImport(file).then(function succeeded(success) {
            alert("File was successfully uploaded");
            $scope.applicationWithMemberListUploadCheck = false;
            $scope.importErrorMessage = "";
            $scope.gotoPage(1);
            return { message: "File uploaded to server successfully.", success: true };
        }, function failure(errormessage) {
            alert("File failed to upload.");
            console.log(errormessage);
            $scope.importErrorMessage = errormessage.data.message;
            return { errormessage: "File could not be uploaded to server.", error: true };
        });
    };

    $scope.refreshUnmatched = function refreshUnmatched() {
        $scope.processStatus = "Started Refreshing";
        adminService.refreshUnmatched().then(function succeeded(success) {
            $scope.processStatus = "Finnished Refreshing";
        }, function failure(errormessage) {
            console.log("Refresh unmatched commissions Failed: ", errormessage);
        });
    };

    $scope.putFieldMap = function (id, fieldMap, actionUrl) {
        adminService.PutFieldMap(id, fieldMap).then(
            function succeed(success) {
                $scope.processImport(id, actionUrl);
            },
            function failure(errormessage) {
                console.log("Could not save FieldMap saved.", errormessage);
            }
        );
    };

    $scope.putProcessAction = function putProcessAction(id, productId) {
        $scope.processStatus = "Started processing";
        adminService.PutProcessAction(id, productId).then(function succeeded(success) {
            $scope.getFilesToImport(1);
            $scope.setImportFileStatus();
            $scope.processStatus = "Processing complete";
            return { message: "ImportedFile processed successfully.", success: true };
        },
            function failure(errormessage) {
                console.log(errormessage);
                return { errormessage: "ImportedFile processing: failed to contact server.", error: true };
            }
        );
    };

    $scope.processImport = function processImport(id, SupplierId, datefrom, dateto) {
        if (datefrom === null || dateto === null) {
            $scope.ProcessError = "You need to add process dates.";
            return;
        } 

        adminService.CheckComissionRun(SupplierId, datefrom, dateto).then(function succeeded() {
            $scope.isValidRun = true;
            if ($scope.isValidRun) {
                $scope.putProcessAction(id, SupplierId);
            }
        }, function failure(errormessage) {
            $scope.ProcessError = "The Commission run has already been done for this month";
            $scope.isValidRun = false;
        });
    };

    $scope.getSuppliers = function getSuppliers() {
        adminService.GetSuppliers().then(
            function succeeded(success) {
                $scope.suppliers.supplierList = success.data;
                $scope.suppliers.default = success.data[0].name;
            }, function failure(errormessage) {
                console.log(errormessage);
                return { errormessage: "Could not fetch suppliers.", error: true };
            });
    };

    $scope.getSuppliers();

    $scope.getLicenseCategories = function getLicenseCategories() {
        adminService.GetLicenseCategories().then(
            function succeeded(success) {
                $scope.licenseCategories = success.data;
                $scope.suppliers.forEach(function (supplier) {
                    supplier.product.forEach(function (product) {
                        product.licenseCategory = $scope.licenseCategoriesfind;
                    });
                });
                return { message: "Fetched list of files to be imported.", success: true };
            }, function failure(errormessage) {
                console.log(errormessage);
                return { errormessage: "Could not fetch LicenseCategories.", error: true };
            });
    };

    $scope.getLicenseTypes = function getLicenseTypes() {
        adminService.GetLicenseTypes().then(
            function succeeded(success) {
                $scope.licenseTypes = success.data;
            }, function failure(errormessage) {
                console.log(errormessage);
                return { errormessage: "Could not fetch LicenseTypes.", error: true };
            });
    };

    $scope.getLicenseTypes();

    $scope.advisors = {};
    $scope.advisor = {};
    $scope.advisorId = "";
    $scope.advisorShare = "";
    $scope.licenses = [];
    $scope.licenceId = "";
    $scope.selectedLicense = null;
    $scope.products = [];
    $scope.setting = {};

    $scope.putAdvisorShare = function putAdvisorShare() {
        advisorService.PutAdvisorShare($scope.advisorId, $scope.licenseId, $scope.advisorShare)
            .then(function succeeded(advisor) {
            },
            function failure(errormessage) {
                console.log("failed putting advisor share" + errormessage);
            });
    };

    $scope.getAdvisorLicenseTypes = function getAdvisorLicenseTypes() {
        return adminService.GetAdvisorLicenseTypes($scope.advisorId).then(
            function succeeded(advisor) {
                $scope.licenses = advisor.data;
            },
            function failure(errormessage) {
                console.log("failed getting advisor license types" + errormessage);
                $scope.advisor = null;
            }
        );
    };

    $scope.putSystemSettings = function putSystemSettings(setting) {
        adminService.PutSystemSettings(setting.systemSettingId, setting).then(
            function succeeded(advisor) {
            },
            function failure(errormessage) {
                console.log("failed putting system setting" + errormessage);
            });
    };

    $scope.getAdvisorLicenseTypesByID = function getAdvisorLicenseTypesByID(selectedAdvisorId, componentIndex) {
        $scope.licenses[componentIndex] = [];
        $scope.products[componentIndex] = [];
        $scope.selectedProductID[componentIndex] = null;
        return adminService.GetAdvisorLicenseTypes(selectedAdvisorId).then(
            function succeeded(advisor) {
                $scope.licenses[componentIndex] = advisor.data;
            },
            function failure(errormessage) {
                console.log("failed getting advisor license types" + errormessage);
                $scope.advisor = null;
            }
        );
    };

    $scope.getAdvisorLicensedProducts = function getAdvisorLicensedProducts(selectedAdvisorId, licenseId, componentIndex) {
        $scope.selectedProductID[componentIndex] = null;
        return adminService.GetAdvisorLicensedProducts(selectedAdvisorId, licenseId).then(
            function succeeded(advisor) {
                $scope.products[componentIndex] = advisor.data;
            },
            function failure(errormessage) {
                console.log("failed getting advisor license type products" + errormessage);
                $scope.advisor = null;
            }
        );
    };

    $scope.onProductSelected = function onProductSelected(id, componentIndex) {
        $scope.selectedProductID[componentIndex] = id;
    };

    $scope.getAdvisor = function getAdvisor() {
        return advisorService.GetAdvisor($scope.advisorId).then(
            function succeeded(advisor) {
            },
            function failure(errormessage) {
                console.log(errormessage);
                $scope.advisor = null;
            }
        );
    };

    $scope.onAdvisorSelected = function () {
        for (var k in $scope.advisors) {
            var advisor = $scope.advisors[k];
            if (advisor.id === $scope.advisorId) {
                $scope.getAdvisorLicenseTypes();
            }
        }
    };

    $scope.onLicenseSelected = function onLicenseSelected() {
        for (var l in $scope.licenses) {
            var license = $scope.licenses[l];
            if (license.licenseTypeId === $scope.licenseId) {
                $scope.advisorShare = license.share;
            }
        }
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

    $scope.getAdvisors($scope.paging);

    $scope.updateFileOwner = function updateFileOwner(fileId, advisorId) {
        adminService.UpdateFileOwner(fileId, advisorId);
    };

    $scope.addProduct = function addProduct(supplier) {
        if (supplier) {
            var product = {
                "new": true
            };

            if (Array.isArray(supplier.products)) {
                supplier.products.push(product);
            } else {
                supplier.products = [product];
            }

            return true;
        } else {
            return false;
        }
    };

    $scope.addSupplier = function addSupplier() {
        var supplier = { "id": null, "name": null, "deleted": false, "products": [], "licenses": null };
        $scope.suppliers.supplierList.push(supplier);
    };

    $scope.saveProduct = function saveProduct(product, supplierId) {
        adminService.AddProductToSupplier(product, supplierId).then(
            function succeeded(success) {
                product = success.data;
            }, function failure(errormessage) {
                console.log(errormessage);
                return { errormessage: "Could not fetch LicenseTypes.", error: true };
            });
    };

    $scope.saveSupplier = function saveSupplier(supplier) {
        adminService.SaveSupplier(supplier).then(
            function succeeded(success) {
                supplier = success.data;
            }, function failure(errormessage) {
                console.log(errormessage);
                return { errormessage: "Could not fetch LicenseTypes.", error: true };
            });
    };

    $scope.selectedUser = null;
    $scope.users = [];
    $scope.systemsettings = [];
    $scope.errorMessages = [];

    $scope.initUserAdmin = function () {
        $scope.errorMessages = [];
        $scope.getUsers();
        $scope.newUser = { isAdmin: true };
    };

    $scope.getUsers = function () {
        adminService.getUsers().then(
            function (success) {
                $scope.users = success.data;
                $scope.selectedUser = null;
            }, function (error) {
                $scope.errorMessages.push("Could not load users");
            });
    };

    $scope.updateUser = function (user) {
        adminService.UpdateUser(user).then(
            function (success) {
                $scope.newUser = {
                    isAdmin: true
                };
                $scope.selectedUser = null;
                $scope.users.push(success.data);
            }, function (error) {
                $scope.errorMessages.push("Could not update user");
            });
    };

    $scope.hasUserSelected = function () {
        return typeof $scope.selectedUser !== 'undefined' && $scope.selectedUser !== null;
    };

    $scope.passwordsMatch = function () {
        if ($scope.selectedUser.passwordChanged) {
            return $scope.selectedUser.password === $scope.selectedUser.confirmPassword;
        }

        return true;
    };

    //New user created from admin section , could perhaps use update?
    $scope.newUser = function (user) {
        adminService.UpdateUser(user).then(
            function (success) {
                $scope.users.push(success.data);
            }, function (error) {
                $scope.errorMessages.push("Could not update user");
            });
    };

    $scope.getSystemSettings = function () {
        adminService.GetSystemSettings().then(
            function (success) {
                $scope.systemsettings = success.data;
            }, function (error) {
                $scope.errorMessages.push("Could not load system settings");
            });
    };

    $scope.getSystemSettings();

    // When the selection changes, this updates:
    $scope.updateProducts = function updateProducts(id) {
        if (typeof $scope.suppliers.supplierList === "undefined") {
            $scope.suppliers.supplierList = [];
        }
        else {
            // Not going to change the model because there is no need to change the db. 
            // instead manipulating the file.id field for supplierId to be passed 
            // it is currently only used under a if statement to check importType == 1
            // so this will be only as the supplier id used when importType == 2
            $scope.uploads.file.id = id;
        }
    };

    $scope.clearCommissionExceptions = function clearCommissionExceptions() {
        var confirmedClearCommissionExceptionList = confirm("Would you like to clear the commission exception list first?");
        if (confirmedClearCommissionExceptionList) {
            adminService.CommissionExceptionTableTruncation().then(
                function succeeded(success) {
                    alert('unMatchedCommissions table cleared successfully');
                },
                function failure(errormessage) {
                    alert(`Error clearing the commission exception list \n Error: ${errormessage}`);
                    $scope.importedFiles = [];
                });
        }
    };
}]);

