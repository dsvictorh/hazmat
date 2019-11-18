app.controller('ErrorLogController', ['$scope', '$rootScope', 'LogsService', function ($scope, $rootScope, LogsService) {
    $scope.filter = {
        IPAddress: null,
        DateFrom: null,
        DateTo: null,
    };

    $scope.gridConfig = {
        pageSize: 10,
        actionIcon: 'view',
        sortColumn: 'Date',
        sortDirection: false,
        loadFunction: LogsService.errorsLog,
        rowFunction: function (row) {
            $rootScope.openModal('error-log-details', 'medium', 'hazmat', row);
        },
        fields: {
            IPAddress: {
                sortable: true,
                label: 'IP Address',
                cardViewTitle: true,
                columnSize: 0.5,
            },
            Date: {
                sortable: true,
                label: 'Date',
                filter: 'date',
                filterCriteria: ['MM/dd/yyyy hh:mma Z']
            },
            Message: {
                sortable: true,
                label: 'Message',
                columnSize: 1.5,
                tooltip: true,
            },
            InnerMessage: {
                sortable: true,
                label: 'Inner Message',
                columnSize: 1.5,
                tooltip: true,
            },
        }
    }
}]);