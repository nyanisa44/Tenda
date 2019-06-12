(function () {
    'use strict';

    angular
        .module('app')
        .controller('annualTaxBracket', annualTaxBracket);

    annualTaxBracket.$inject = ['$scope']; 

    function annualTaxBracket($scope) {
        $scope.title = 'annualTaxBracket';

        activate();

        function activate() { }
    }
})();
