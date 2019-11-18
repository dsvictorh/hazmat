app.controller('LoginController', ['$scope', '$rootScope', '$window', 'LoginService', function ($scope, $rootScope, $window, LoginService) {
    $scope.email = '';
    $scope.password = '';
    $scope.persist = false;

    $scope.login = function () {
        if (!$scope.submitting) {
            $scope.submitting = true;
            LoginService.login({
                email: $scope.email,
                password: $scope.password,
                persist: $scope.persist,
                returnUrl: $rootScope.getQueryVariable('returnUrl'),
            }).then(function (response) {
                var data = angular.fromJson(response.data);
                if (data.Success) {
                    $window.location.href = data.ReturnUrl;
                    return;
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
}]);