var app = angular.module('ntg', []);

app.controller('LayoutController', ['$scope', '$rootScope', 'LootBoxService', function ($scope, $rootScope, LootBoxService) {
    $scope.menuActive = false;

    function init() {
        LootBoxService.getInfo().then(function (response) {
            var data = angular.fromJson(response.data);
            $rootScope.lootBox = {
                imageUrl: data.ImageUrl,
                title: data.Title,
                text: data.Text,
            };
        });
    }

    $scope.toggleMenu = function (e, forceMode) {
        e.stopPropagation();
        $scope.menuActive = forceMode != null ? forceMode : !$scope.menuActive;
    }

    $scope.claimLootBox = function (model) {
        if (!$scope.submitting) {
            $scope.submitting = true;
            LootBoxService.claim(model).then(function (response) {
                var data = angular.fromJson(response.data);
                $rootScope.handleMessages(data.Messages);

                if(data.Success || data.Claimed){
                    $rootScope.closeModal(true);
                }
            }, function (error) {
                $rootScope.handleMessages(angular.fromJson(error.data).Messages);
            }).finally(function () {
                $scope.submitting = false;
            });
        }
    }

    $scope.showLootBoxForm = function (pageModuleId, e) {
        if ($rootScope.lootBox) {
            $rootScope.openModal('loot-box-form', 'small', 'ntg', { pageModuleId: pageModuleId }, function () {
                $(e.target).remove();
            }, 'Prize will not be claimed. Are you sure you want to close this form?');
        }
    }

    init();
}]);

app.factory('LootBoxService', ['$http', function ($http) {
    var service = {};

    service.claim = function (data) {
        return $http({
            url: '/loot-box/claim',
            method: 'POST',
            dataType: 'json',
            data: data,
        });
    };

    service.getInfo = function () {
        return $http({
            url: '/loot-box/info',
            method: 'GET',
            dataType: 'json',
        });
    };

    return {
        claim: service.claim,
        getInfo: service.getInfo,
    }
}]);