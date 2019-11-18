app.controller('SiteLogController', ['$scope', '$rootScope', 'LogsService', function ($scope, $rootScope, LogsService) {
    $scope.filter = {
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
        loadFunction: LogsService.siteLog,
        rowFunction: function (row) {
            $rootScope.openModal('site-log-details', 'medium', 'hazmat', row);
        },
        fields: {
            User: {
                sortable: true,
                label: 'User',
                cardViewTitle: true,
                propertyStructure: ['Admin', 'Email'],
            },
            Date: {
                sortable: true,
                label: 'Date',
                filter: 'date',
                filterCriteria: ['MM/dd/yyyy hh:mma Z'],
                columnSize: 1.1,
            },
            Action: {
                sortable: true,
                label: 'Action',
                columnSize: 1.5,
                tooltip: true,
            },
            Page: {
                sortable: true,
                label: 'Page',
                tooltip: true,
            },
        }
    }
}]);