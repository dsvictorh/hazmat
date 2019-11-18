app.factory('LogsService', ['$http', function ($http) {
    var service = {};

    service.errorsLog = function (data, timeout) {
        return $http({
            url: '/hazmat/logs/error',
            method: 'POST',
            dataType: 'json',
            data: data,
            timeout: timeout
        });
    };

    service.securityLog = function (data, timeout) {
        return $http({
            url: '/hazmat/logs/security',
            method: 'POST',
            dataType: 'json',
            data: data,
            timeout: timeout
        });
    };

    service.siteLog = function (data, timeout) {
        return $http({
            url: '/hazmat/logs/site',
            method: 'POST',
            dataType: 'json',
            data: data,
            timeout: timeout
        });
    };

    return {
        errorsLog: service.errorsLog,
        securityLog: service.securityLog,
        siteLog: service.siteLog,
    }
}]);