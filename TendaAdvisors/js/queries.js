
tendaApp.controller('queriesController', ['$scope', 'applicationService', '$state', '$window', function ($scope, applicationService, $state,$window) {

    if ($state.params.application == null) {

        $state.go('applicationDetail');
        return;

   }

  
   $scope.notQueryEditing = -1;
   $scope.addQueryButtonClicked = false;
   $scope.tempQueryCount = 0;
   $scope.tempNewQueryCount = 0;
   $scope.applicationid = 0;
   $scope.queryid = 0;
   $scope.refreshCheck = 0;
   $scope.clickedSave = true;
   $scope.queryCount =0;
  

    //getQuerys
   applicationService.GetQuerysByApplication($state.params.application).then(function succeeded(queries) {
        //console.log(queries.data);
        if (queries.data.length == 0) {
            console.log("There are no Queries");
            return;
        }
        $scope.queries = queries.data;
        $scope.applicationid = $scope.queries[0].application_Id;
        $scope.tempQueryCount = $scope.queries.length;
    }, function failure(errormessage) {
        console.log(errormessage);
    });
    
    //getQuerys
    applicationService.GetQueryType().then(function succeeded(queryTypes) {
        //console.log(queries.data);
        if (queryTypes.length == 0) {
            console.log("There are no Queries");
            return;
        }

        $scope.queryTypes = queryTypes.data;

    }, function failure(errormessage) {
        console.log(errormessage);
    });

    $scope.addQuery = function addQuery() {

        if ($scope.clickedSave==false)
        {
            $scope.clickedSave = !$scope.clickedSave;
        }
       
        if (typeof $scope.queries == "undefined") {
            $scope.queries = [];
            
        }

            $scope.addQueryButtonClicked = true;
            $scope.queryCount = ($scope.queries.length > 0) ? $scope.queries[$scope.queries.length - 1].id : 0;
            $scope.queryCount = $scope.queryCount + 1;
            $scope.queries.push({ id: $scope.queryCount });
            $scope.notQueryEditing = $scope.queries.length;

      
        //$scope.tempQueryCount = $scope.tempQueryCount + 1;
    }

    $scope.goBackToClientDetail = function goBackToClientDetail() {
        $state.go('newApplication');
    }


    //delete Query

    $scope.deleteQuery = function deleteQuery(del,index,queryId) {//Save is when you stop editing.
        //$scope.notQueryEditing = !$scope.notQueryEditing;

        
        if (del) {
            //$scope.tempQueryCount = $scope.tempQueryCount -1;
            
            applicationService.DeleteQuery(queryId).then(function succeeded(success) {
                $scope.queries.splice(index, 1);
                console.log(success);
                //var paramsSatte=$state.params.application;
                //$state.go('applicationDetail')
               
                return;
                
            }, function failure(errormessage) {
                console.log(errormessage);
            });

            //Response.Redirect("templates/queries.html", false);

            //$state.go('queries');
            //location.reload(true);
        }
    }


    $scope.cancelQuery = function cancelQuery(index) {

        $scope.queries.splice(index, 1);

    }


    //Save New Query:
    $scope.saveNewQuery = function saveNewQuery(save) {//Save is when you stop editing.
        if (save) {
            $scope.clickedSave = !$scope.clickedSave;
            var query = $scope.queries[$scope.queries.length - 1];

            if ($scope.applicationid!=0)
            {

                query.application = { "id": $scope.applicationid };
            }

        else{
                query.application = { "id": $state.params.application };
            }
          
            applicationService.PostQuery(query).then(function succeeded(success) {
                console.log(success);
                
            }, function failure(errormessage) {
                console.log(errormessage);
            });
      
        }
    }

    $scope.gotoPage = function gotoPage(page) {

        if (page == 1 && $scope.registerItem == 0) {
            $scope.registerItem = page;
        }
        else if (page == 2 && $scope.registerItem == 1) {
            $scope.registerItem = page;
        }
        else if (page == 3 && $scope.registerItem == 2) {
            $scope.registerItem = page;
            $state.go('admin', page);
        }
        else if (page == 4) {
            $scope.registerItem = page;
        }
        else {
            $scope.registerItem = page;
            console.log("gotopage " + page);
        }
    }

}]);

