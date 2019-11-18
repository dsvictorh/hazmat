app.directive('grid', ['$rootScope', '$filter', '$timeout', '$q', function ($rootScope, $filter, $timeout, $q) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            filter: '=?',
            config: '=',
            actionIcon: '@?',
        },
        link: function (scope, element, attrs) {
            scope.grid = scope.config;
            var paginationItemsSize = 5;
            var lowMiddle = Math.floor(paginationItemsSize / 2);
            var highMiddle = Math.ceil(paginationItemsSize / 2);
            var canceler;
            var filterInterrupt;

            if (scope.grid.sortDirection == null) {
                scope.grid.sortDirection = true;
            }

            if (typeof scope.grid.rowFunctionLock !== 'function') {
                scope.grid.rowFunctionLock = function () {
                    return false;
                }
            }

            scope.$on('loadItems', function () {
                if (!scope.loading) {
                    scope.loading = true;
                    canceler = $q.defer();
                    var data = {
                        page: scope.grid.selectedPage,
                        pageSize: scope.grid.pageSize,
                        sortColumn: scope.grid.sortColumn,
                        sortDirection: scope.grid.sortDirection ? 'ASC' : 'DESC',
                    };
                    for (var prop in scope.filter) {
                        if (!data[prop]) {
                            data[prop] = scope.filter[prop];
                        }
                    }
                    scope.grid.loadFunction(data, canceler.promise).then(function (response) {
                        var data = angular.fromJson(response.data);
                        scope.grid.rows = data.Rows;
                        scope.grid.totalCount = data.TotalCount;
                        scope.grid.totalPages = data.Pages;

                        var pageArray = [];
                        //The start of the pages to show needs to vary according to the pagination size and it's current position
                        //Any number lesser than the size divided by 2 rounding up represents the point after the middle 
                        //(example: 7 being the size 7/2 is 3.5, rounded up to 4 and in the pages 1 2 3 [4] 5 6 7 we can see 4 is in the middle).
                        //After that middle the items need to be moved from the center which is the selected page minus the size divided by 2 rounding down
                        //(example: 7 being the size 7/2 is 3.5, rounded down to 3 and in page 5 minus 3 is 2. so starting from 2 and having seven numbers we have 2 3 4 [5] 6 7 8, having 5 as the selected page in the center of 7 other numbers)
                        //Just as the first numbers shouldn't move until the middle is past, the last numbers past the middle shouldn't move. We need to substract from the selected page the result of the size minus the result of
                        //the total pages minus the selected page minus 1.
                        //(example: 7 being the size and in page 9 we substract: selectedPage - (paginationSize - (totalPages - selectedPage) - 1) : 9 - (7 - (10 - 9) - 1) : 9 - 5 = 4. Starting from 4 to a size of 7 pages
                        //we have the following list of pages 4 5 6 7 8 [9] 10 with 9 as the seleceted not being in the center but still showing the 7 sized pagination list.
                        //In order to know which of the items are the last in the page past the middle we need to see if substracting the selected page from the total pages would be a number lesser than the number of items on each side of the center
                        //(example: 7 being the size 1 2 3 [4] 5 6 7 we see that in order to be in the middle there has to be 3 items on both sides. 3 being the middle rounded down tells us this: 7 / 2 is 3.5 rounded down is 3. If the number is lesser than 3 there can be no middle:
                        //So totalPages - selectedPage has to be less than the middle rounded down. Given page 8 of 10 pages 10-8 = 2 which is less than 3 therefore a number past the middle and therefore needs to not attempt to move to the middle when selected).
                        var start = scope.grid.selectedPage > highMiddle ?
                            (scope.grid.totalPages - scope.grid.selectedPage < lowMiddle ?
                                scope.grid.selectedPage - (paginationItemsSize - (scope.grid.totalPages - scope.grid.selectedPage) - 1) :
                                scope.grid.selectedPage - lowMiddle)
                            : 1
                        scope.grid.pages = pageArray;
                        for (var i = start; i <= data.Pages; i++) {
                            if (pageArray.length == paginationItemsSize) break;
                            pageArray.push(i);
                        }
                    }, function (error) {
                        $rootScope.handleMessages(angular.fromJson(error.data).Messages);
                    }).finally(function () {
                        scope.filterChanged = false;
                        scope.loading = false;
                        canceler.resolve();
                        canceler = null;
                    });
                }
            });

            scope.$watchGroup(['grid.selectedPage', 'grid.sortColumn', 'grid.sortDirection'], function () {
                if (canceler) canceler.resolve();

                scope.refresh();
            });

            scope.$watch('filter', function () {
                if (canceler) canceler.resolve();
                if (filterInterrupt)
                {
                    $timeout.cancel(filterInterrupt);
                    filterInterrupt = null;
                }
                
                filterInterrupt = $timeout(function () {
                    filterInterrupt = null;

                    scope.filterChanged = true;
                    scope.refresh(true);
                }, 600);
            }, true);

            scope.filterColumn = function (filter, value, filterCriteria) {
                if (!filter) return value;
                var params = [];
                params.push(value);

                if (filterCriteria) {
                    params = params.concat(filterCriteria);
                }

                //Not having a clear view of how many items a filter can have
                //We use apply to convert the concatenated array being the value to filter the first value
                //and the rest being all the filter criterias into a dynamic parameter function call of filter
                return $filter(filter).apply(null, params);
            }

            scope.changePage = function (page) {
                if (!scope.loading && page > 0 && page <= scope.grid.totalPages) {
                    scope.grid.selectedPage = page;
                }
            }
            
            scope.unwrapProperty = function (row, key, propertyStructure) {
                var prop = row;
                if (propertyStructure != null) {
                   if(!Array.isArray(propertyStructure)){
                        console.error('propertyStructure of field' + key + 'is not of type Array');
                        return null;
                    }else{
                        for (var i = 0; i < propertyStructure.length; i++) {
                            if (prop[propertyStructure[i]]) {
                                prop = prop[propertyStructure[i]];
                            }
                        }

                        return prop;
                    }
                }

                return prop[key];
            }

            scope.sort = function (column) {
                scope.grid.sortDirection = scope.grid.sortColumn == column ? !scope.grid.sortDirection : true;
                scope.grid.sortColumn = column;
            }

            scope.refresh = function (reset) {
                if (reset) {
                    scope.grid.selectedPage = 1;
                }
                scope.$emit('loadItems');
            }            

            scope.refresh(true);
        },
        templateUrl: '/assets/js/directives/templates/grid.html'
    }
}]);