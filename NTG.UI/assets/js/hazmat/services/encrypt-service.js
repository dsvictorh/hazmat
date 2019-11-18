app.factory('EncryptService', ['$http', function ($http) {
    var service = {};

    service.encrypt = function (data) {
        return $http({
            url: '/hazmat/encrypt',
            method: 'POST',
            dataType: 'json',
            data: data
        });
    };

    return {
        encrypt: service.encrypt
    }
}]);