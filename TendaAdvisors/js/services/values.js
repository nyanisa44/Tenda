
//this is an example service for calling the API
tendaApp.factory('valuesService', ['$q', '$http', 'authService', function ($q, $http, authService) {

    function path(pathbase, arg1, arg2, arg3) {
        var p = 'api/' + pathbase;
        if (arg1) p += '/' + encodeURIComponent(arg1);
        if (arg2) p += '/' + encodeURIComponent(arg2);
        if (arg3) p += '/' + encodeURIComponent(arg3);
        return p;
    }

    return {
        get: function (id) {
            return $http.get(path('values', id)).then(function (response) { return response.data; });
        },

        getAll: function () {
            return $http.get(path('values')).then(function (response) { return response.data; });
        },

        add: function (value) {
            return $http.post(path('values'), { value: value }).then(function (response) { return response.data; });
        },

        update: function (id, value) {
            return $http.put(path('values', id), { value: value }).then(function (response) { return response.data; });
        },

        'delete': function (id) {
            return $http.delete(path('values', id)).then(function (response) { return null; });
        },

        clear: function () {
            return $http.post(path('values/clear')).then(function (response) { return null; });
        }
    }
}]);