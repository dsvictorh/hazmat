app.controller('UserController', ['$scope', '$rootScope', 'UserService', function ($scope, $rootScope, UserService) {
    $scope.filter = {
        User: null,
        Name: null,
        RoleId: null,
        Status: null,
    };

    $scope.warnings = {
        email: [
            {
                text: 'Removing your user from the Admin role will restrict you from further accesing Admin only features',
                applies: function (value) { return value == $rootScope.userDetails.Email }
            },
        ],
        role: [
            {
                text: 'Changing other hazmat users to Admin will remove your ability to edit it further',
                applies: function (value) {
                    if ($scope.roles) {
                        var admin = $scope.roles.find(function (item) { return item.Name == 'Admin' });
                    }
                    
                    return admin && value == admin.Id;
                }
            },
        ]
    }

    $scope.gridConfig = {
        pageSize: 15,
        actionIcon: 'edit',
        sortColumn: 'Email',
        enableAddButton: true,
        rowFunctionLock: function (row) {
            return row.Role.Name == 'Admin' && row.Email != $rootScope.userDetails.Email;
        },
        loadFunction: function(data, timeout){
            return UserService.getUsers(data, 'admin', timeout);
        },
        rowFunction: function (row, refresh) {
            if (!row) {
                row = {
                    RoleId: $scope.roles[0].Id
                }
            }
            $rootScope.openModal('user-details', 'small', 'hazmat', angular.copy(row), refresh, 'All unsaved changes will be lost. Are you sure you want to close the form?');
        },
        fields: {
            Email: {
                sortable: true,
                label: 'Email',
                cardViewTitle: true,
                columnSize: 2,
            },
            FullName: {
                sortable: true,
                label: 'Name',
                columnSize: 1.5,
                sortValue: 'FirstName'
            },
            Role: {
                sortable: true,
                label: 'Role',
                propertyStructure: ['Role', 'Name'],
                sortValue: 'Role.Name'
            },
            Active: {
                sortable: true,
                label: 'Active',
                textCenter: true,
                check: true,
            },
        }
    }

    $scope.saveUser = function (user) {
        if (!$scope.submitting) {
            $scope.submitting = true;
            UserService.saveUser({
                Id: user.Id,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Password: user.Password,
                VerifyPassword: user.VerifyPassword,
                Active: user.Active,
                RoleId: user.RoleId
            }, 'admin', user.Email).then(function (response) {
                var data = angular.fromJson(response.data);
                $rootScope.handleMessages(data.Messages);

                if (data.Success) {
                    user = data;
                    $rootScope.closeModal(true);
                }
            }, function (error) {
                $rootScope.handleMessages(angular.fromJson(error.data).Messages);
            }).finally(function () {
                $scope.submitting = false;
            });
        }
    }

    UserService.getRoles().then(function (response) {
        var data = angular.fromJson(response.data);
        $scope.roles = data.Roles;
        $scope.filterRoles = angular.copy($scope.roles);
        $scope.filterRoles.unshift({ Id: null, Name: 'All' });
    }, function (error) {
        $rootScope.handleMessages(angular.fromJson(error.data).Messages);
    })
}]);