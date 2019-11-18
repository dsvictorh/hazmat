app.directive('inputText', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            hide: '=?',
            required: '=?',
            errors: '=?',
            warnings: '=?',
            panel: '=?',
            isPassword: '=',
            disable: '=?',
            halfField: '@?',
            inputId: '@',
            label: '@',
            maxLength: '@',
            change: '&?'
        },
        link: function (scope, element, attrs) {
            scope.$watch('model', function (newValue, oldValue) {
                if (scope.errors) {
                    scope.errors = null;
                }

                if (scope.hide) {
                    scope.model = null;
                }

                if (newValue != oldValue && typeof scope.change === 'function')
                    scope.change();
            });

            scope.$watch('hide', function () {
                if (scope.hide) {
                    scope.model = null;
                }
            });
        },
        templateUrl: '/assets/js/directives/templates/input-text.html'
    }
});

app.directive('inputTextarea', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            hide: '=?',
            required: '=?',
            errors: '=?',
            warnings: '=?',
            panel: '=?',
            isPassword: '=',
            disable: '=?',
            halfField: '@?',
            inputId: '@',
            label: '@',
            maxLength: '@',
            change: '&?'
        },
        link: function (scope, element, attrs) {
            scope.$watch('model', function (newValue, oldValue) {
                if (scope.errors) {
                    scope.errors = null;
                }

                if (scope.hide) {
                    scope.model = null;
                }

                if (newValue != oldValue && typeof scope.change === 'function')
                    scope.change();
            });

            scope.$watch('hide', function () {
                if (scope.hide) {
                    scope.model = null;
                }
            });
        },
        templateUrl: '/assets/js/directives/templates/input-textarea.html'
    }
});


app.directive('inputInfoField', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            hide: '=?',
            errors: '=?',
            warnings: '=?',
            flipBold: '=?',
            panel: '=?',
            halfField: '@?',
            label: '@',
        },
        link: function (scope, element, attrs) {
            scope.$watch('model', function () {
                if (scope.errors) {
                    scope.errors = null;
                }
            });
        },
        templateUrl: '/assets/js/directives/templates/input-info-field.html'
    }
});

app.directive('inputDatePicker', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            hide: '=?',
            required: '=?',
            errors: '=?',
            warnings: '=?',
            panel: '=?',
            disable: '=?',
            inputId: '@',
            format: '@?',
            halfField: '@?',
            label: '@',
            minView: '@?',
            maxView: '@?',
            defaultView: '@?',
            change: '&?'
        },
        link: function (scope, element, attrs) {
            scope.format = scope.format || 'MM/DD/YYYY HH:mm';
            scope.minView = scope.minView || 'minutes';
            scope.maxView = scope.maxView || 'year';
            scope.defaultView = scope.defaultView || 'date';
            scope.dateModel = {
                date: scope.model ? moment(scope.model) : null,
            };

            scope.$watch('model', function (newValue, oldValue) {
                if (scope.errors) {
                    scope.errors = null;
                }

                if (scope.hide) {
                    scope.model = null;
                }

                if (newValue != oldValue && typeof scope.change === 'function')
                    scope.change();
            });

            scope.changeDate = function (modelName, newDate) {
                scope.model = newDate.toDate();
            }

            scope.clearDate = function () {
                scope.dateModel.date = '';
                scope.model = null;
            }

            scope.$watch('hide', function () {
                if (scope.hide) {
                    scope.model = null;
                }
            });

            element.on('keypress', 'input', function (e) {
                e.preventDefault();
            });
        },
        templateUrl: '/assets/js/directives/templates/input-date-picker.html'
    }
});


app.directive('inputDropdown', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            errors: '=?',
            warnings: '=?',
            panel: '=?',
            hide: '=?',
            options: '=',
            required: '=?',
            disable: '=?',
            halfField: '@?',
            inputId: '@',
            label: '@', 
            display: '@',
            value: '@',
            change: '&?'            
        },
        link: function (scope, element, attrs) {
            scope.$watch('model', function (newValue, oldValue) {
                if (scope.errors) {
                    scope.errors = null;
                }

                if (scope.hide) {
                    scope.model = null;
                }

                if (newValue != oldValue && typeof scope.change === 'function')
                    scope.change();
            });

            scope.$watch('hide', function () {
                if (scope.hide) {
                    scope.model = null;
                }
            });
        },
        templateUrl: '/assets/js/directives/templates/input-dropdown.html'
    }
});

app.directive('inputCheck', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            inputId: '@',
            label: '@',
            tooltip: '@',
            halfField: '@?',
            warnings: '=?',
            panel: '=?',
            hide: '=?',
            change: '&?'
        },
        link: function (scope, element, attrs) {
            scope.$watch('model', function (newValue, oldValue) {
                if (scope.hide) {
                    scope.model = null;
                }

                if (newValue != oldValue && typeof scope.change === 'function')
                    scope.change();
            });

            scope.$watch('hide', function () {
                if (scope.hide) {
                    scope.model = null;
                }
            });
        },
        templateUrl: '/assets/js/directives/templates/input-check.html'
    }
});

app.directive('inputSlider', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            errors: '=?',
            warnings: '=?',
            hide: '=?',
            disable: '=?',
            panel: '=?',
            halfField: '@?',
            prefix: '@?',
            suffix: '@?',
            min: '@?',
            max: '@?',
            step: '@?',
            inputId: '@',
            label: '@',
            change: '&?',
        },
        link: function (scope, element, attrs) {
            var sliderElement = $(element).find('.slider');
            var fill = sliderElement.find('.fill');
            scope.min = scope.min ? parseInt(scope.min) : 0;
            scope.max = scope.max ? parseInt(scope.max) : 100;
            scope.step = scope.step ? parseFloat(scope.step) : 1;

            scope.$watch('model', function (newValue, oldValue) {
                if (scope.errors) {
                    scope.errors = null;
                }

                if (scope.hide) {
                    scope.model = scope.min;
                }

                if (newValue != oldValue) {
                    refreshSlider();

                    if(typeof scope.change === 'function')
                        scope.change();
                }
            });

            scope.$watch('hide', function () {
                if (scope.hide) {
                    scope.model = scope.min;
                }
            });

            scope.$watch('disable', function (newValue) {
                sliderElement.slider({ disabled: newValue});
            });

            if (isNaN(scope.model)) {
                console.error('Model needs to be a number');
                return;
            }

            if (isNaN(scope.min)) {
                console.error('Min value needs to be a number');
                return;
            }

            if (isNaN(scope.max)) {
                console.error('Max value needs to be a number');
                return;
            }

            if (isNaN(scope.step)) {
                console.error('Max value needs to be a number');
                return;
            }

            if (scope.min < 0) {
                console.error('Min value cannot be less than 0');
                return;
            }

            if (scope.max < scope.min) {
                console.error('Max value cannot be less than min value');
                return;
            }

            sliderElement.slider({
                min: scope.min,
                max: scope.max,
                step: scope.step,
                slide: function (e, ui) {
                    scope.model = ui.value;
                    scope.$apply();
                },
            });

            function refreshSlider() {
                sliderElement.slider('value', scope.model);
                fill.width(sliderElement.find('.ui-slider-handle')[0].style.left);
            }

            refreshSlider();
            
        },
        templateUrl: '/assets/js/directives/templates/input-slider.html'
    }
});


app.directive('sortArrows', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            collection: '=',
            index: '=',
            vertical: '=?',
            mobileSmallVertical: '=?',
            propertyParentStructure: '=?',
            disable: '=?',
            sortPropertyName: '@',
            itemName: '@?',
            change: '&?',
            changeProperty: '@?',
        },
        link: function (scope, element, attrs) {
            element.on('click', 'i', function (e) {
                var increase = angular.element(e.target).hasClass('next');
                if (!scope.disable && ((!increase && scope.index > 0) || (increase && scope.index < scope.collection.length - 1))) {
                    var swapIndex = increase ? (scope.index + 1) : (scope.index - 1);
                    var obj = scope.collection[scope.index];
                    var swapObj = scope.collection[swapIndex];

                    if (scope.propertyParentStructure && scope.propertyParentStructure.length) {
                        for (var i = 0; i < scope.propertyParentStructure.length; i++) {
                            if (obj.hasOwnProperty(scope.propertyParentStructure[i]) && swapObj.hasOwnProperty(scope.propertyParentStructure[i])) {
                                obj = obj[scope.propertyParentStructure[i]];
                                swapObj = swapObj[scope.propertyParentStructure[i]];
                            }
                        }
                    }

                    if (obj.hasOwnProperty(scope.sortPropertyName) && swapObj.hasOwnProperty(scope.sortPropertyName)) {
                        var swapValue = obj[scope.sortPropertyName];
                        var swapCollectionObj = scope.collection[scope.index];
                        obj[scope.sortPropertyName] = swapObj[scope.sortPropertyName];
                        swapObj[scope.sortPropertyName] = swapValue;
                        scope.collection[scope.index] = scope.collection[swapIndex];
                        scope.collection[swapIndex] = swapCollectionObj;
                        obj[scope.changeProperty] = true;
                        swapObj[scope.changeProperty] = true;
                        if(typeof scope.change === 'function')
                            scope.change();
                        scope.$apply();
                    }
                }

            });
        },
        templateUrl: '/assets/js/directives/templates/sort-arrows.html'
    }
});

app.directive('submitButton', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            loading: '=?',
            submittingText: '@?',
            text: '@',
        },
        templateUrl: '/assets/js/directives/templates/submit-button.html'
    }
});

app.directive('cancelButton', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            text: '@?',
        },
        templateUrl: '/assets/js/directives/templates/cancel-button.html'
    }
});