app.directive('expandableMenuItem', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            url: '@?',
            icon: '@',
            label: '@?',
            submenu: '=?',
            action: '&?'
        },
        templateUrl: '/assets/js/directives/templates/expandable-menu-item.html'
    }
});