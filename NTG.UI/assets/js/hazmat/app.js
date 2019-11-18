var app = angular.module('hazmat', ['ngWYSIWYG', 'datePicker']).config(['$provide', function ($provide) {
    $provide.decorator('mFormatFilter', function () {
        return function newFilter(m, format, tz) {
            if (!(moment.isMoment(m))) {
                return '';
            }
            return tz ? moment.tz(m, tz).format(format) : m.format(format);
        };
    });
}]);

app.controller('LayoutController', ['$scope', '$rootScope', 'PageService', function ($scope, $rootScope, PageService) {
    $scope.menuExpanded = false;
    $scope.userMenuExpanded = false;

    $scope.$on('loadPages', function () {
        PageService.getPages().then(function (response) {
            var data = angular.fromJson(response.data);
            $scope.pages = angular.copy(data.Pages);
            for (var i = 0; i < $scope.pages.length; i++) {
                $scope.pages[i].url = $scope.pages[i].Url;
                $scope.pages[i].label = $scope.pages[i].Name;
                $scope.pages[i].delete = false;
                $scope.pages[i].locked = $scope.pages[i].AdminLocked && !$rootScope.userDetails.isAdmin;
            }
            $scope.pages.unshift({ url: '/hazmat/page/create', label: 'New Page' });
            $scope.pages.unshift({ action: function () { $rootScope.openModal('pages-order', 'medium', 'hazmat', data.Pages) }, label: 'Page Order' });       
        }, function (error) {
            $rootScope.handleMessages(angular.fromJson(error.data).Messages);
        });
    });

    $scope.toggleMenu = function (e, forceMode) {
        e.stopPropagation();
        $scope.menuExpanded = forceMode != null ? forceMode : !$scope.menuExpanded;
        if (!$scope.menuExpanded) {
            angular.element(e.target).parents('ul').find('li').removeClass('active');
        }
    }

    $scope.toggleSubmenu = function (e) {
        $scope.menuExpanded = true;
        angular.element(e.target).parents('li').toggleClass('active');
    }

    $scope.showUserMenu = function (force) {
        $scope.userMenuExpanded = force != null  ? force : !$scope.userMenuExpanded;
    }
}]);

app.controller('PageListController', ['$scope', '$rootScope', 'PageService', function ($scope, $rootScope, PageService) {
    $scope.savePageList = function (pageList) {
        if (!$scope.submitting && confirm('Deleted pages cannot be recovered. Are you sure you want to continue?')) {
            $scope.submitting = true;
            PageService.saveList({
                pageActions: pageList
            }).then(function (response) {
                var data = angular.fromJson(response.data);
                if (data.Success) {
                    $rootScope.$broadcast('loadPages');
                    for (var i = 0; i < data.PageActions.length; i++) {
                        if (data.PageActions[i].Delete) {
                            var index = $scope.model.findIndex(function (item) { return item.Id === data.PageActions[i].Id });
                            $scope.model.splice(index, 1);
                        }
                    }
                }

                $scope.submitting = false;
                $rootScope.handleMessages(data.Messages);
            }, function (error) {
                $scope.submitting = false;
                $rootScope.handleMessages(angular.fromJson(error.data).Messages);
            });
        }

        return false;
    }

    $scope.toggleDeletePage = function (page) {
        if (page.Position != 1 && (!page.AdminLocked || $rootScope.userDetails.isAdmin)) {
            $scope.deletePage = page;
            $scope.deletePage.delete = !$scope.deletePage.delete;
            delete $scope.deletePage;
        }
    }
}]);

app.factory('MediaService', ['$http', function ($http) {
    var service = {};

    service.getImages = function (folderStructure, tags) {
        return $http({
            url: '/hazmat/media/images/folder?folderStructure=' + folderStructure + '&tags=' + tags,
            method: 'GET',
        });
    };

    service.uploadImage = function (data) {
        return $http({
            url: '/hazmat/media/images/upload',
            method: 'POST',
            headers: {
                'Content-Type': undefined
            },
            data: data,
            transformRequest: function (data) {
                var formData = new FormData();
                angular.forEach(data, function (value, key) {
                    if (value != null) {
                        formData.append(key, value);
                    }
                });

                return formData;
            }
        });
    };

    service.deleteImage = function (data) {
        return $http({
            url: '/hazmat/media/images/delete',
            method: 'POST',
            dataType: 'json',
            data: data,
        });
    };

    return {
        getImages: service.getImages,
        uploadImage: service.uploadImage,
        deleteImage: service.deleteImage,
    }
}]);