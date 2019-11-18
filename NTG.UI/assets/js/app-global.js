app.config(['$compileProvider', '$httpProvider', '$qProvider', function ($compileProvider, $httpProvider, $qProvider) {
    $compileProvider.aHrefSanitizationWhitelist(/^\s*(https?|ftp|mailto|file|javascript):/);
    $qProvider.errorOnUnhandledRejections(false);
    $httpProvider.interceptors.push(function ($rootScope, $q) {
        return {
            request: function (config) {
                config.timeout = 30000;
                return config;
            },
            responseError: function (rejection) {
                switch (rejection.status) {
                    case 408:
                        $rootScope.handleMessages({ Global: [{ MessageType: 'Error', Text: 'Server took too long to response. Please try again later or contact support' }] });
                        break;
                    case -1:
                        $rootScope.handleMessages({ Global: [{ MessageType: 'Error', Text: 'Could not connect to server. Please try again later or contact support' }] });
                }
                return $q.reject(rejection);
            }
        }
    });
}]).run(['$rootScope', '$timeout', 'LoggedUserService', function ($rootScope, $timeout, LoggedUserService) {
    $rootScope.userDetails = {};

    $rootScope.getQueryVariable = function (variable) {
        var query = window.location.search.substring(1);
        var vars = query.split("&");
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split("=");
            if (decodeURIComponent(pair[0]) == variable) {
                return decodeURIComponent(pair[1]);
            }
        }
    }

    $rootScope.handleMessages = function (messages) {
        $rootScope.messages = $rootScope.messages || {};
        for (var message in messages) {
            $rootScope.messages[message.toLowerCase()] = messages[message];
        }
    }

    $rootScope.openModal = function (template, size, app, model, closeFunction, confirmCloseMessage) {
        $rootScope.modalTemplate = '/assets/modal-templates/' + app + '/' + template + '.html';
        $rootScope.modalModel = model;
        $rootScope.modalFunction = closeFunction;
        $rootScope.modalCloseMessage = confirmCloseMessage;
        $rootScope.modalSize = size;
    }

    $rootScope.closeModal = function (executeCloseFunction) {
        var close = true;
        if(executeCloseFunction && (typeof $rootScope.modalFunction === 'function')){
            $rootScope.modalFunction();
        } else if ($rootScope.modalCloseMessage) {
            close = confirm($rootScope.modalCloseMessage);
        }

        if (close) {
            $rootScope.modalTemplate = null;
            $rootScope.modalModel = null;
            $rootScope.modalFunction = null;
            $rootScope.modalCloseMessage = null;
            $rootScope.modalSize = null;
        }
    }

    $rootScope.currentYear = new Date().getFullYear();
    $rootScope.$watchCollection('messages.global', function () { 
        if ($rootScope.messages && $rootScope.messages.global) {
            if ($rootScope.messages.global.length) {
                $timeout(function () {
                    if ($rootScope.messages.global) {
                        $rootScope.messages.global[$rootScope.messages.global.length - 1].remove = true;
                        $timeout(function () {
                            if ($rootScope.messages.global) {
                                $rootScope.messages.global.pop();
                            }
                        }, 300);
                    }
                }, 5000);
            } else {
                $rootScope.messages.global = null;
            }
        }
    });

    $rootScope.getUserDetails = function () {
        LoggedUserService.userDetails()
        .then(function (response) {
            var data = angular.fromJson(response.data);
            $rootScope.userDetails = data;
            $rootScope.userDetails.fullName = data.FirstName + ' ' + data.LastName;
            $rootScope.userDetails.isAdmin = data.Role == 'Admin';
            $rootScope.handleMessages(data.Messages);
            $rootScope.$broadcast('userDetailsLoaded', $rootScope.userDetails);
            $rootScope.$broadcast('loadPages');
        }, function (error) {
            $rootScope.handleMessages(angular.fromJson(error.data).Messages);
            $rootScope.$broadcast('userDetailsFailed');
        });
    }

    $rootScope.getUserDetails();
}]);

app.factory('LoggedUserService', ['$http', function ($http) {
    var service = {};

    service.userDetails = function () {
        return $http({
            url: '/hazmat/user/details',
            method: 'GET',
            dataType: 'json',
        });
    };

    return {
        userDetails: service.userDetails,
    }
}]);

app.filter('imageResize', function () {
    return function (input, width, height) {
        var resize = '/image/upload/';

        if (width){
            resize += 'w_' + width;
        }

        if (height) {
            resize += (width ? ',' : '')  + 'h_' + height;
        }

        resize += '/';

        return input ? input.replace('/image/upload/', resize) : '';
    };
});

app.directive('focusableObjects', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            $(element).find('[focus-object]').each(function () {
                $(this).attr('tabindex', '0');
            });

            $(element).on('mouseup click', '[focus-object], a', function (e) {
                if (attrs.preventBubble)
                    e.stopPropagation();

                $(this).blur();
            });

            $(element).on('keyup', '[focus-object], a', function (e) {
                if (e.keyCode == 13 || e.keyCode == 32) {
                    if (attrs.preventBubble)
                        e.stopPropagation();

                    $(this).trigger('click');
                }
            });
        }
    }
});

app.directive('includeReplace', function () {
    return {
        require: 'ngInclude',
        restrict: 'A',
        link: function (scope, element, attrs) {
            element.replaceWith(element.children());
        }
    };
});

$(window).on('load', function () {
    $('body').addClass('ready');
    setTimeout(function () {
        $('body').addClass('stop-loading');
    }, 1500);
});