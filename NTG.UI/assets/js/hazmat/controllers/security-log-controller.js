app.controller('SecurityLogController', ['$scope', '$rootScope', 'LogsService', function ($scope, $rootScope, LogsService) {
    $scope.filter = {
        IPAddress: null,
        UserEmail: null,
        Action: null,
        DateFrom: null,
        DateTo: null,
    };

    $scope.gridConfig = {
        pageSize: 15,
        actionIcon: 'view',
        sortColumn: 'Date',
        sortDirection: false,
        loadFunction: LogsService.securityLog,
        rowFunction: function (row) {
            $rootScope.openModal('security-log-details', 'medium', 'hazmat', row);
        },
        fields: {
            UserEmail: {
                sortable: true,
                label: 'User',
                cardViewTitle: true,
            },
            Date: {
                sortable: true,
                label: 'Date',
                filter: 'date',
                filterCriteria: ['MM/dd/yyyy hh:mma Z']
            },
            Action: {
                sortable: true,
                label: 'Action',
                columnSize: 2,
                tooltip: true,
            },
        }
    }
}]);