app.factory('SiteSettingsService', ['$http', function ($http) {
    var service = {};

    service.get = function () {
        return $http({
            url: '/hazmat/site-settings/info',
            method: 'GET',
            dataType: 'json',
        });
    };

    service.save = function (data) {
        return $http({
            url: '/hazmat/site-settings/save',
            method: 'POST',
            dataType: 'json',
            data: data
        });
    };

    return {
        get: service.get,
        save: service.save
    }
}]);