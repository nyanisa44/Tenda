

tendaApp.controller('registerAdvisorController', ['$scope', 'advisorService', 'detailsService', 'clientsService', '$state', function ($scope, advisorService, detailsService, clientsService, $state) {

    //window.location.reload(true);
    $scope.registerItem = 6;//Start at the beginning
    /*"documentTypeId":0*/
    /*$scope.application = { "products": [{ "id": 0, "supplier": { "id": 0 } }], "client": { "addresses": [{ "id": 0 }] } };*/
    $scope.advisor = { "advisorDocuments": [{ "documentTypeId": 0 }], "licenses": [], "contact": { "addresses": [{ "id": 0 }] } };
    $scope.advisorShareUnderSupervision = { "AdvisorId": {  }, "LicenseTypeId": {}, "supplier": {}, "product": {}, "Share": {}, "underSupervision": {}, "Advisor": {}, "validCommissionFromDate": {}, "validCommissionToDate": {}, "ValidFromDate": {}, "ValidToDate": {} };
    $scope.uploadSucces = "Not Uploaded";
    $scope.uploaded = false;
    $scope.notUploaded = false;
    $scope.addAllowanceStatus = false;
    $scope.uploadedAllowance = false;
    $scope.notUploadedAllowance = false;
    $scope.actualNewAdvisor = false;
    $scope.cantEdit = true;
    $scope.isAdvisorContact = false;

    $scope.BasiccontactsSupervisor = null;
 
   // $scope.incorrectEntry = false;
    $scope.companies = [];
    $scope.editingAdvisorDetail = true;
    $scope.incorrectEntry = false;
    $scope.duplicateId = false;

    //Licenses
    $scope.advisorTypes = [];
    $scope.licenseCategories = [];
    $scope.licenseTypes = [];
    $scope.chosenLicenseCategoryId = 0;
    $scope.chosenLicenseTypeId = 0;
    $scope.advisor.fsbCode = "44680";
    $scope.advisor.cmsCode = "BR"; 
    $scope.advisor.company = { "id": 1 };
    
    $scope.advisor.advisorType = { "id": 2 };
    $scope.advisor.bankName = { "id": 2 };
    $scope.advisor.branchCode = { "id": 2 };
    $scope.advisor.advisorStatus = { "id": 2 };
    $scope.advisor.contact = { "contactType": { "id": 1 }, "addresses": [{ "addressType": { "id": 3 }, "country": { "id": 1 } }], "contactTitle": { "id": 1 } };
    //Default role is admin
    $scope.advisorRole = {
        isAdvisor: false,
        isAdmin: false
    };
    //$scope.advisor.advisorRole = "Admin";
    //$scope.advisor.Contact = $scope.advisor.contact; //Why are there two? case sensitive...
    $scope.AddressTypes = {};
    $scope.Province = {};
    $scope.Country = {};
    $scope.Titles = {};
    $scope.Addresses = {};
    $scope.advisors = {};

    $scope.notNewApplication_check = false;

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

        if (page === 0) {

            $scope.registerItem = page;
            $scope.actualNewAdvisor = true;
        }
        if (page === 6|| page === 1) {
            $scope.registerItem = page;
        } else if ($scope.advisor.licenses && $scope.advisor.licenses.length > 0  && page == 3) {
            $scope.registerItem = page;
            $scope.suppliers = $scope.getSuppliers();
            if (typeof $scope.advisorRole.isAdvisor != undefined && $scope.advisorRole.isAdvisor) {
             
                if ($scope.actualNewAdvisor == true)
                {
                    $scope.advisor.User.isAdmin = false;
                }
                else {

                    $scope.advisor.User.isAdmin = false;
                }
                
            } else {

                if ($scope.actualNewAdvisor == true)
                {
                     $scope.advisor.User.isAdmin = true;
                }
                else {

                    $scope.advisor.User.isAdmin = true;
                }
             
            }

            advisorService.CreateAdvisor($scope.advisor).then(function succeeded(companies) {
                console.log("Companies: ", companies);
                $scope.advisor = companies.data;
            }, function failure(errormessage) {
                console.log(errormessage);
                $scope.registerItem = page;
            });
          
            //$scope.fetchSuppliersByLicenses($scope.advisor.licenses);
        }
        else if(page == 3)
        {
            $scope.registerItem = page;
            //$scope.suppliers = $scope.fetchSupplierByLicenses($scope.advisor.licenses);
            
            if (typeof $scope.advisorRole.isAdvisor != undefined && $scope.advisorRole.isAdvisor) {

                if ($scope.actualNewAdvisor == true) {
                    $scope.advisor.User.isAdmin = false;
                }
                else {

                    $scope.advisor.User.isAdmin = false;
                }

            } else {

                if ($scope.actualNewAdvisor == true) {
                    $scope.advisor.User.isAdmin = true;
                }
                else {

                    $scope.advisor.User.isAdmin = true;
                }

            }
            advisorService.CreateAdvisor($scope.advisor).then(function succeeded(companies) {
                $scope.advisor = companies.data;
            }, function failure(errormessage) {
                console.log(errormessage);
                $scope.registerItem = page;
            });
        }

     else if (page == 7 || page == 4) {
            $scope.registerItem = page;
           
        } else if ($scope.advisor && page == 5) {
            $state.go('advisors');
            
        } else {
            $scope.registerItem = page;
        }
    }

    $scope.addAllowance = function addAllowance() {

        $scope.addAllowanceStatus = true;
    }

    $scope.checkPercentage=function  checkPercentage(product,shareAmount)
    {
        if (!(shareAmount >= 0 && shareAmount < 100)) {
            product.incorrectEntry = true;
        }
        else
        {
            product.incorrectEntry = false;
        }
    }
    


    $scope.PostAdvisorShare = function PostAdvisorShare(product,id, licenseID, supplierName, productName, advisorShare, validCommissionFromDate, validCommissionToDate, underSupervision, advisorSupervision, validSupervisionFromDate, validSupervisionToDate) {
        console.log('putting advisor share');

        if (underSupervision != true)
        {
            underSupervision = false;
        }
        if (advisorShare == null)
        {
            advisorShare = 0;
        }


        if (advisorSupervision == null) {
            advisorSupervision = 59;
        }

        if (validSupervisionFromDate == null) {
            var myDate = new Date();
            //11/24/2016 12:00:00 AM
            myDate.getUTCDate();
            validSupervisionFromDate = null;
           
        }
        if (validSupervisionToDate == null) {
            var myDate = new Date();
            myDate.getUTCDate();
            validSupervisionToDate = null;
        }

        if (validCommissionFromDate == null) {
            var myDate = new Date();
            //11/24/2016 12:00:00 AM
            myDate.getUTCDate();
            validCommissionFromDate = null;

        }
        if (validCommissionToDate == null) {
            var myDate = new Date();
            myDate.getUTCDate();
            validCommissionToDate = null;
        }
               
        $scope.advisorShareUnderSupervision.advisorId = $scope.advisor.id;
        $scope.advisorShareUnderSupervision.LicenseTypeId = licenseID;
        $scope.advisorShareUnderSupervision.supplier = supplierName;
        $scope.advisorShareUnderSupervision.product = productName;
        $scope.advisorShareUnderSupervision.Share = advisorShare;
        $scope.advisorShareUnderSupervision.underSupervision = underSupervision;
        $scope.advisorShareUnderSupervision.supervisorId = advisorSupervision;
        $scope.advisorShareUnderSupervision.validCommissionFromDate = validCommissionFromDate;
        $scope.advisorShareUnderSupervision.validCommissionToDate = validCommissionToDate;
        $scope.advisorShareUnderSupervision.ValidFromDate = validSupervisionFromDate;
        $scope.advisorShareUnderSupervision.ValidToDate = validSupervisionToDate;
        
        $scope.savedProduct = false;
        advisorService.PostAdvisorShareObject($scope.advisorShareUnderSupervision).then
            (
            function succeeded(advisor) {
                product.uploaded = true;
                product.notUploaded = false;
                $scope.savedProduct = true;
                console.log("Succeeded putting advisor share " + JSON.stringify(advisor));

            },
            function failure(errormessage) {
                product.notUploaded = true;
                product.uploaded = false;
                $scope.savedProduct = false;
                console.log("failed putting advisor share" + errormessage);

            }
            );
    }

    $scope.checkCharaters= function checkCharaters(characters)
    {
        if (!/^[a-zA-Z ]+$/.test(characters) || characters.length > 50)
        {
            $scope.incorrectEntry = true;
        }
        else {
            $scope.incorrectEntry = false;
        }

   
    }

    $scope.checkRightAmount = function checkRightAmount(allowance) {
        if (!/^[0-9.,]+$/.test(allowance)) {
            $scope.allowanceIncorrect = true;
        }
        else {
            $scope.allowanceIncorrect = false;
        }
    }


    

    $scope.checkCharatersLast = function checkCharatersLast(characters) {
        if (!/^[a-zA-Z ]+$/.test(characters)) {
            $scope.incorrectEntryLastName = true;
        }
        else {
            $scope.incorrectEntryLastName= false;
        }
    }

    $scope.checkTaxDirective = function checkTaxDirective(characters) {
        if (characters === undefined || characters === "") {
            $scope.incorrectEntryTax = false;
        }
        else {
            if (!/^[0-9.,]+$/.test(characters)) {
                $scope.incorrectEntryTax = true;
            }
            else {
                $scope.incorrectEntryTax = false;
            }
        }
    };
    
    $scope.checkDuplicateIds = function checkDuplicateIds(idNum) {
        if (idNum === undefined) {
            $scope.duplicateId = false;
        }
        else {
            for (var i = 0; i < $scope.advisorsNotAdvisorTypeComapanyShowId.length; i++) {
                if ($scope.advisorsNotAdvisorTypeComapanyShowId[i].idNumber === idNum) {
                    $scope.duplicateId = true;
                    break;
                }
                else {
                    $scope.duplicateId = false;
                }
            }
        }
    };
    
    $scope.putAdvisorAllowance = function putAdvisorAllowance(id,advisorAllowance)
    {
        console.log($scope);

        if (advisorAllowance == null) {
            advisorAllowance = 0;
        }

        advisorService.putAdvisorAllowance(id,advisorAllowance).then
       (
       function succeeded(advisor) {
           $scope.uploadedAllowance = true;
           $scope.notUploadedAllowance = false;
           console.log("Succeeded putting advisor share " + JSON.stringify(advisor));

       },
       function failure(errormessage) {
           $scope.notUploadedAllowance = true;
           $scope.uploadedAllowance = false;
           console.log("failed putting advisor share" + errormessage);

       }
       );

    }



    $scope.addLicenseToAdvisor = function (advisor, licenseId) {
        var licenseFound = false;
        for (id in advisor.licenses) {
            if (advisor.licenses[id].id == licenseId) {
                licenseFound = true;
            }
        }

        if(!licenseFound) {
            for (id in $scope.licenseTypes) {
                if ($scope.licenseTypes[id].id == licenseId) {
                    advisor.licenses.push($scope.licenseTypes[id]);
                }
            }
        }
        console.log(advisor.licenses);
    }


    /**
    * This function received am advisor object along 
    * with the license object to be deleted.
    */
    $scope.deleteLicence = function (advisor, license) {
        var licenseFound = false;

        //Ensure that the license is in the list before
        //attempting to delete.
        var licenseIndex = advisor.licenses.indexOf(license);
        if (licenseIndex != -1) {
            advisor.licenses.splice(licenseIndex, 1);
        }

        console.log(advisor.licenses);
    }




    $scope.doSearchSuper = function doSearchSuper(searchWhere) {
        console.log("Search for contact containing: " + searchWhere);
        if (!!searchWhere && searchWhere.trim().length >= 4)
            $scope.getContactsSuper(searchWhere);
    }

    $scope.getContactsSuper = function getContacts(searchQuery) {
        clientsService.GetBasicContact(searchQuery).then(function succeeded(contact) {
            console.log(contact);

            $scope.BasiccontactsSupervisor = contact.data;

            $scope.advisorSupervisor.contact = $scope.BasiccontactsSupervisor[0];





        }, function failure(errormessage) {
            console.log(errormessage);
        });
    }






    $scope.doSearch = function doSearch(searchWhere) {
        console.log("Search for contact containing: " + searchWhere);
        if (!!searchWhere && searchWhere.trim().length >= 4)
            $scope.getContacts(searchWhere);
    }

    $scope.doSearchUnderSupervision = function doSearchUnderSupervision(searchWhere) {
        console.log("Search for contact containing: " + searchWhere);
        if (!!searchWhere && searchWhere.trim().length >= 4)
            $scope.getContactsUnderSupervision(searchWhere);
    }

    $scope.getContactsUnderSupervision = function getContactsUnderSupervision(searchQuery) {
        clientsService.GetBasicContact(searchQuery).then(function succeeded(contact) {
            console.log(contact);

            $scope.Basiccontacts = contact.data;
            for (var i = 0; i < $scope.advisorsNotAdvisorTypeComapany.length; i++) {

                if ($scope.advisorsNotAdvisorTypeComapany[i].id == $scope.Basiccontacts[0].id) {
                    $scope.advisorSupervisor = $scope.Basiccontacts[0];
                    $scope.getAdvisorId($scope.advisorSupervisor.id);
                    //$scope.getAdvisorLogin($scope.advisor.contact.id);
                    $scope.isAdvisorContact = true;
                    break;

                }

            }

        }, function failure(errormessage) {
            console.log(errormessage);
        });
    }
    

    $scope.getAdvisorId = function getAdvisorId(id) {
        advisorService.GetAdvisorIdTypeAdvisor(id).then(function success(response) {
            $scope.advisorIdPast = response.data;
        },
        function failure(error) {
            console.error(error);
        });
    }


    $scope.getContacts = function getContacts(searchQuery) {
        clientsService.GetBasicContact(searchQuery).then(function succeeded(contact) {
            console.log(contact);

            $scope.Basiccontacts = contact.data;
          for (var i = 0; i < $scope.advisorsNotAdvisorTypeComapany.length; i++) {

                    if ($scope.advisorsNotAdvisorTypeComapany[i].id == $scope.Basiccontacts[0].id)
                    {
                        $scope.advisor.contact = $scope.Basiccontacts[0];
                        getUser($scope.advisor.contact.id);
                        $scope.getAdvisorLogin($scope.advisor.contact.id);
                        $scope.isAdvisorContact = true;
                        break;

                    }
            
                }
        
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    }


    $scope.getAdvisorLogin = function getAdvisorLogin(id) {

        for (var i = 0; i < $scope.advisors.length; i++) {
            if ($scope.advisors[i].contact.id == id) {
                $scope.advisorType = $scope.advisors[i].advisorType.id;

                $scope.advisor.user = $scope.advisors[i].user;
                if ($scope.advisor.user.isAdmin==true)
                {
                    $scope.advisorRole.isAdmin = true;
                    $scope.advisorRole.isAdvisor = false;
                }
                if ($scope.advisor.user.isAdmin == false) {

                    $scope.advisorRole.isAdvisor = true;
                    $scope.advisorRole.isAdmin = false;
                
                }


            }
        }
    }





    $scope.oldClient = function oldClient() {

        $scope.notNewApplication_check = true;
    }

 

    //Get Suppliers:
    
    $scope.getSuppliers = function getSuppliers() {
        advisorService.GetSuppliers().then(function succeeded(success) {
            console.log(success);

            $scope.suppliers = success.data;


        }, function failure(errormessage) {
            console.log(errormessage);
        });
    };

    function getUser(id)
    {
        advisorService.GetUser(id).then(function success(result) {
            $scope.advisor.User = result.data;
        }, function failure(error) {
            console.errror(error);
        });
    }
    
    advisorService.GetAdvisorsNotAdvisorTypeComapanyDisplayIdNumber()
        .then(function succeeded(success) {
            console.log(success);
            $scope.advisorsNotAdvisorTypeComapanyShowId = success.data;
        }, function failure(errormessage) {
            console.log(errormessage);
        });

    advisorService.GetAdvisorsNotAdvisorTypeComapany().then(function succeeded(success) {
        console.log(success);
        $scope.advisorsNotAdvisorTypeComapany = success.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });


    $scope.getProducts = function getProducts(id) {
        $scope.availableProducts = $scope.fetchProductsByLicenses($scope.advisor.licenses, id);
    };
 
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


    //Get Title
    detailsService.GetContactTitles().then(function succeeded(success) {
        console.log(success);

        $scope.Titles = success.data;

    }, function failure(errormessage) {
        console.log(errormessage);
    });

    //Get Banks
    detailsService.GetBankName().then(function succeeded(success) {
        console.log(success);

        $scope.Banks = success.data;

    }, function failure(errormessage) {
        console.log(errormessage);
    });

    //Get Bank Codes
    detailsService.GetBankBranchCodes().then(function succeeded(success) {
        console.log(success);

        $scope.BankCode = success.data;

    }, function failure(errormessage) {
        console.log(errormessage);
    });





    //Get Countries
    detailsService.GetCountries().then(function succeeded(success) {
        console.log(success);

        $scope.Country = success.data;

    }, function failure(errormessage) {
        console.log(errormessage);
    });
    
    //getCompanies
    $scope.getCompanies = function getCompanies(searchQuery) {
        advisorService.GetCompanies(searchQuery).then(function succeeded(companies) {
            console.log(companies);
            $scope.companies = companies.data;
            $scope.advisor.Company = $scope.companies[0];
        }, function failure(errormessage) {
            console.log(errormessage);
        });
    }
    $scope.getCompanies("");

    //get Suppliers & Products (for certain licenses only)
    $scope.fetchProductsByLicenses = function fetchSuppliersByLicenses(licenseList,id) {
        advisorService.FetchProductsByLicenses(licenseList,id).then(
            function succeeded(products) {
                console.log(products);
                products.data.forEach(function (item, index) { item.name = ""; });
                $scope.availableProducts = products.data;
                return products.data;
            },
            function failure(errormessage) {
                console.log(errormessage);
            }
        );
    }
    $scope.fetchSupplierByLicenses = function fetchSuppliersByLicenses(licenseList) {
        advisorService.FetchSuppliersByLicenses(licenseList).then(
            function succeeded(suppliers) {
                console.log(suppliers);
                $scope.suppliers = suppliers.data;
                return suppliers.data;
            },
            function failure(errormessage) {
                console.log(errormessage);
            }
        );
    }
    


 
    //Fetch Suppliers

   

    //get License Types
    $scope.getLicenseTypes = function getLicenseTypes(advisorTypeId) {
        advisorService.GetLicenseTypes(advisorTypeId).then(
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




    
    //AdvisorDocuments
    advisorService.GetAdvisorDocuments().then(function succeeded(advisorDocument) {
        //console.log(queries.data);
        if (advisorDocument.length == 0) {
            console.log("There are no Queries");
            return;
        }

        $scope.advisorDocument = advisorDocument.data;

    }, function failure(errormessage) {
        console.log(errormessage);
    });




    //get License Categories and License Types
    $scope.getLicenseCategories = function getLicenseCategories(advisorTypeId) {
        advisorService.GetLicenseCategories(advisorTypeId).then(
            function succeeded(licenseCategories) {
                console.log(licenseCategories.data);
                $scope.licenseCategories = licenseCategories.data;
            },
            function failure(errormessage) {
                console.log(errormessage);
            }
        );
    }
    $scope.getLicenseCategories($scope.advisor.advisorTypeId);




    //getContactTypes
    advisorService.GetContactTypes().then(function succeeded(contactTypes) {
        //console.log(queries.data);
        if (contactTypes.length === 0) {
            console.log("There are no Queries");
            return;
        }

        $scope.contactTypes = contactTypes.data;

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

    $scope.uploadDocument = function uploadDocument(advisorId, documentTypeID, document,submitted) {
       
        //advisorId, documentTypeId and base64 file data (api/AdvisorDocuments/' + advisorId + '/DocumentTypeId/' + documentId)

        if (submitted != true)
        {
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


    $scope.gotoAdvisor = function gotoAdvisor()
    {
        var result=confirm('Are you sure you want to exit adding a new Advisor?');

        if (result==true)
        {
            $state.go('advisors');
        }
    }
    
   
    //getContactTypes
    advisorService.GetAdvisorTypes().then(function succeeded(advisorTypes)
        {
        //console.log(queries.data);
        if (advisorTypes.length === 0) {
            console.log("There are no Queries");
            return;
        }

        $scope.advisorTypes = advisorTypes.data;
        $scope.advisor.advisorType = $scope.advisorTypes[1];
    }, function failure(errormessage) {
        console.log(errormessage);
    });

    advisorService.GetAdvisorStatuses().then(function succeeded(advisorStatuses) {
        //console.log(queries.data);
        if (advisorStatuses.length === 0) {
            console.log("There are no Statuses");
            return;
        }

        $scope.advisorStatuses = advisorStatuses.data;
    }, function failure(errormessage) {
        console.log(errormessage);
    });


 

}]);



Date.prototype.yyyymmdd = function (padding, time) {
    time = time ? time : false;
    var mm = this.getMonth() + 1; // getMonth() is zero-based
    var dd = this.getDate();

    var dateString = [this.getFullYear(), (mm < 10 ? '0' : '') + mm, (dd < 10 ? '0' : '') + dd].join(padding); // padding
    if (time) {
        dateString += " " + this.getHours() + ":" + this.getMinutes();
    }
    return dateString;
};
var today = new Date(Date.parse("2016/07/01", true));

