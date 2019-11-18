app.factory('LoginService', ['$http', function ($http) {
    var service = {};

    service.login = function (credentials) {
        return $http({
            url: '/hazmat/login?returnUrl',
            method: 'POST',
            dataType: 'json',
            data: credentials
        });
    };

    return {
        login: service.login,
    }
}]);