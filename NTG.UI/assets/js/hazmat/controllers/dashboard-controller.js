app.controller('DashboardController', ['$scope', '$rootScope', 'LootBoxClaimService', function ($scope, $rootScope, LootBoxClaimService) {
    var dateFrom = new Date();
    var dateTo = new Date();
    dateFrom.setDate(dateTo.getDate() - 7);
    dateFrom.setHours(0, 0, 0, 0);
    $scope.filter = {
        Email: null,
        Name: null,
        DateFrom: dateFrom,
        DateTo: dateTo,
        Redeemed: false,
    };

    $scope.gridConfig = {
        pageSize: 15,
        actionIcon: 'view',
        sortColumn: 'Date',
        sortDirection: false,
        loadFunction: LootBoxClaimService.getLootBoxClaims,
        rowFunction: function (row, refresh) {
            $rootScope.openModal('loot-box-claim-details', 'medium', 'hazmat', row, refresh);
        },
        fields: {
            Email: {
                sortable: true,
                label: 'Email',
                cardViewTitle: true,
            },
            FullName: {
                sortable: true,
                label: 'Name',
                sortValue: 'FirstName'
            },
            Date: {
                sortable: true,
                label: 'Date',
                filter: 'date',
                filterCriteria: ['MM/dd/yyyy hh:mma Z'],
                columnSize: 1.1,
            },
        }
    }

    $scope.saveLootBoxClaim = function (lootBoxClaim) {
        if (!$scope.submitting) {
            $scope.submitting = true;
            LootBoxClaimService.saveLootBoxClaim({
                Id: lootBoxClaim.Id,
                Prize: lootBoxClaim.Prize,
                Redeemed: lootBoxClaim.Redeemed,
            }).then(function (response) {
                var data = angular.fromJson(response.data);
                $rootScope.handleMessages(data.Messages);

                if (data.Success) {
                    lootBoxClaim = data;
                    $rootScope.closeModal(true);
                }
            }, function (error) {
                $rootScope.handleMessages(angular.fromJson(error.data).Messages);
            }).finally(function () {
                $scope.submitting = false;
            });
        }
    }
}]);