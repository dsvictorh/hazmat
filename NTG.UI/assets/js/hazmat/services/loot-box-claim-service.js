app.factory('LootBoxClaimService', ['$http', function ($http) {
    var service = {};

    service.getLootBoxClaims = function (data, timeout) {
        return $http({
            url: '/hazmat/loot-box-claim/list',
            method: 'POST',
            dataType: 'json',
            data: data,
            timeout: timeout
        });
    };

    service.saveLootBoxClaim = function (data) {
        return $http({
            url: '/hazmat/loot-box-claim/save',
            method: 'POST',
            dataType: 'json',
            data: data,
        });
    };

    return {
        getLootBoxClaims: service.getLootBoxClaims,
        saveLootBoxClaim: service.saveLootBoxClaim,
    }
}]);