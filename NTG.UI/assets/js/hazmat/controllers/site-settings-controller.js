app.controller('SiteSettingsController', ['$scope', '$rootScope', 'SiteSettingsService', function ($scope, $rootScope, SiteSettingsService) {
    $scope.model = {
        layoutExpanded: false,
        lootBoxExpanded: false,
        metaTagsExpanded: false,
        openGraphTagsExpanded: false,
        twitterTagsExpanded: false,
        errorPagesExpanded: false,
        uploadFolder: 'site-setings'
    };

    $scope.warnings = {
        headerHomeImageUrl: [
            {
                text: 'Leaving this field empty will disable the home button on the site\'s header',
                applies: function (value) { return !value; }
            },
        ],
        footerHomeImageUrlLight: [
            {
                text: 'Leaving this field empty will disable the home image on the site\'s footer',
                applies: function (value) { return !value; }
            },
        ],
        footerHomeImageUrlDark: [
            {
                text: 'Leaving this field empty will disable the home image on the site\'s footer',
                applies: function (value) { return !value; }
            },
        ],
        lootBoxImageUrl: [
            {
                text: 'Leaving this field empty will disable the loot box system on the site',
                applies: function (value) { return !value; }
            },
        ],
        metaFaviconImageUrl: [
            {
                text: 'Uploading a non .ico image will cause prevent the icon from working correctly',
                applies: function (value) { return value && !value.endsWith('.ico'); }
            },
        ]
    }

    function init() {
        SiteSettingsService.get().then(function (response) {
            var data = angular.fromJson(response.data);
            $scope.model.settings = data.SiteSettings;
            $scope.success = true;
        }, function (error) {
            $rootScope.handleMessages(angular.fromJson(error.data).Messages);
        }).finally(function () {
            $scope.pageLoaded = true;
        });
    }

    $scope.resetChanges = function () {
        if (confirm('All unsaved changes will be lost. Are you sure you want to cancel?')) {
            init();
        }
    }

    $scope.saveChanges = function () {
        if (!$scope.submitting) {
            $scope.submitting = true;

            SiteSettingsService.save($scope.model.settings).then(function (response) {
                var data = angular.fromJson(response.data);
                $rootScope.handleMessages(data.Messages);

                if (data.Success) {
                    $scope.model.settings = data;
                }
            }, function (error) {
                $rootScope.handleMessages(angular.fromJson(error.data).Messages);
            }).finally(function () {
                $scope.submitting = false;
            });
        }

        return false;
    }

    $scope.toggleExpandModule = function (module) {
        if (typeof($scope.model[module]) === 'boolean') {
            $scope.model[module] = !$scope.model[module];
        }
    }

    init();
}]);