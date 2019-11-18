app.factory('PageService', ['$http', function ($http) {
    var service = {};

    service.getPages = function () {
        return $http({
            url: '/hazmat/page/list',
            method: 'GET',
            dataType: 'json',
        });
    };

    service.getModules = function (page) {
        return $http({
            url: '/hazmat/page/' + page + '/modules',
            method: 'GET',
            dataType: 'json',
        });
    };

    service.save = function (page, data) {
        return $http({
            url: '/hazmat/page/' + page + '/save',
            method: 'POST',
            dataType: 'json',
            data: data,
        });
    };

    service.saveList = function (data) {
        return $http({
            url: '/hazmat/page/list/save',
            method: 'POST',
            dataType: 'json',
            data: data,
        });
    };

    service.saveModule = function (page, moduleType, data) {
        return $http({
            url: '/hazmat/page/' + page + '/save/' + moduleType,
            method: 'POST',
            dataType: 'json',
            data: data,
        });
    };

    return {
        getModules: service.getModules,
        save: service.save,
        saveModule: service.saveModule,
        getPages: service.getPages,
        saveList: service.saveList,
    }
}]);