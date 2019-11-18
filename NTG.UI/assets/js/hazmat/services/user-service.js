app.factory('UserService', ['$http', function ($http) {
    var service = {};

    service.getUsers = function (data, userType, timeout) {
        return $http({
            url: '/hazmat/user/' + userType + '/list',
            method: 'POST',
            dataType: 'json',
            data: data,
            timeout: timeout
        });
    };

    service.saveUser = function (data, userType, user) {
        data.User = user;
        return $http({
            url: '/hazmat/user/' + userType + '/' + (data.Id ? 'save' : 'create'),
            method: 'POST',
            dataType: 'json',
            data: data,
        });
    };

    service.getRoles = function () {
        return $http({
            url: '/hazmat/user/admin/roles',
            method: 'GET',
            dataType: 'json',
        });
    };

    return {
        getUsers: service.getUsers,
        saveUser: service.saveUser,
        getRoles: service.getRoles,
    }
}]);