app.controller('AccountController', ['$scope', '$rootScope', '$window', '$timeout', 'UserService', function ($scope, $rootScope, $window, $timeout, UserService) {
    function init() {
        $scope.$on('userDetailsLoaded', function (event, args) {
            $scope.model = angular.copy(args);
            $scope.success = true;
        });

        $scope.$on('userDetailsFailed', function (event, args) {
            $scope.success = false;
        });
    }

    $scope.save = function () {
        if (!$scope.submitting) {
            $scope.submitting = true;
            UserService.saveUser({
                Id: $scope.model.Id,
                Email: $scope.model.Email,
                FirstName: $scope.model.FirstName,
                LastName: $scope.model.LastName,
                Password: $scope.model.Password,
                VerifyPassword: $scope.model.VerifyPassword
            }, 'admin').then(function (response) {
                var data = angular.fromJson(response.data);
                $rootScope.handleMessages(data.Messages);

                if (data.Success) {
                    if (data.Relog) {
                        $timeout(function () {
                            $window.location.href = '/hazmat';
                        }, 2000);
                        return;
                    }
                        
                    $scope.model = data;
                    $rootScope.getUserDetails();
                }
            }, function (error) {
                $rootScope.handleMessages(angular.fromJson(error.data).Messages);
            }).finally(function () {
                $scope.submitting = false;
            });
        }
        
    }

    $scope.reset = function () {
        if (confirm('All unsaved changes will be lost. Are you sure you want to cancel?')) {
            $scope.model = angular.copy($rootScope.userDetails);
        }
    }

    init();
}]);