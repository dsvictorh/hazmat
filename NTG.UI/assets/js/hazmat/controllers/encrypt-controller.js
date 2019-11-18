app.controller('EncryptController', ['$scope', '$rootScope', 'EncryptService', function ($scope, $rootScope, EncryptService) {
    $scope.text = '';
    $scope.encryptedText = '';

    $scope.encrypt = function () {
        if (!$scope.submitting) {
            $scope.submitting = true;
            EncryptService.encrypt({
                text: $scope.text
            }).then(function (response) {
                var data = angular.fromJson(response.data);
                $scope.encryptedText = data.EncryptedText;

                $rootScope.handleMessages(data.Messages);
            }, function (error) {
                $rootScope.handleMessages(angular.fromJson(error.data).Messages);
            }).finally(function () {
                $scope.submitting = false;
            });
        } 

        return false;
    }
}]);