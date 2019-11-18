app.directive('inputWysiwyg', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            required: '=?',
            inputId: '@',
            label: '@',
            halfField: '@?',
            panel: '=?',
            hide: '=?',
            showImage: '=?',
            showColor: '=?',
            showPrint: '=?',
            restrictToParagraph: '=?',
            errors: '=?',
            warnings: '=?',
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
        templateUrl: '/assets/js/directives/templates/input-wysiwyg.html'
    }
});

app.directive('inputImagePicker', ['$rootScope', '$filter', '$http', 'MediaService', function ($rootScope, $filter, $http, MediaService) {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            folder: '=',
            tags: '=?',
            errors: '=?',
            warnings: '=?',
            hide: '=?',
            panel: '=?',
            hideList: '=?',
            resize: '=?',
            required: '=?',
            compactRecommend: '=?',
            halfField: '@?',
            inputId: '@',
            label: '@',
            recommendSize: '@?',
            recommendRatio: '@?',
            change: '&?'
        },
        link: function (scope, element, attrs) {
            if (!scope.hideList) {
                MediaService.getImages(scope.folder, scope.tags && scope.tags.length ? scope.tags.toString() : null).then(function (response) {
                    scope.images = $filter('orderBy')(angular.fromJson(response.data).Images, '-Created');
                    if (scope.model) {
                        var found = false;
                        for (var i = 0; i < scope.images.length; i++) {
                            if (scope.images[i].Image == scope.model) { 
                                var selected = scope.images[i];
                                scope.images[i] = scope.images[0];
                                scope.images[0] = selected;
                                found = true;
                                break;
                            }
                        }
                        if (scope.model && !found) {
                            scope.model = null;
                        }
                    }
                }, function (error) {
                    $rootScope.handleMessages(angular.fromJson(error.data).Messages);
                }).finally(function () {
                    scope.loaded = true;
                });
            } else {
                scope.loaded = true;
            }

            scope.$watch('model', function (newValue, oldValue) {6
                if (scope.errors) {
                    scope.errors = null;
                }

                if (scope.hide || (scope.images && !scope.images.find(function(item){ return item.Image == scope.model}))) {
                    scope.model = null;
                }

                if (scope.model) {
                    $http.get(scope.model).then(null, function () {
                        scope.model = null;
                    });
                }

                if (newValue != oldValue && typeof scope.change === 'function')
                    scope.change();
            });

            scope.$watch('hide', function () {
                if (scope.hide) {
                    scope.model = null;
                }
            });

            scope.$watchCollection('images', function () {
                element.find('ul').animate({ scrollLeft: 0 });
            });

            scope.deleteImage = function (image, index) {
                if (confirm('Deleted images cannot be restored. Are you sure you want to delete this image?')) {
                    MediaService.deleteImage({ Url: image.Image, PublicId: image.PublicId }).then(function (response) {
                        var data = angular.fromJson(response.data);
                        if (data.Success) {
                            scope.images.splice(index, 1);

                            if (scope.model == image.Image)
                            {
                                scope.model = null;
                            }
                        }

                        $rootScope.handleMessages(data.Messages);
                    }, function (error) {
                        $rootScope.handleMessages(angular.fromJson(error.data).Messages);
                    });
                }
            }


            element.on('click', 'li:not(.add) img', function (e) {
                if (!scope.uploading) {
                    if (scope.model == angular.element(e.target).attr('data-image')) {
                        scope.model = null;
                    } else {
                        scope.model = angular.element(e.target).attr('data-image');
                    }
                    
                    scope.$apply();
                }  
            });

            element.on('click', '.add, label', function (e) {
                element.find('[type="file"]').click();
            });

            element.find('[type="file"]').on('change', function (e) {
                var file = e.target.files[0];
                if (!scope.uploading && file) {
                    scope.uploading = true;
                    scope.dataUrl = URL.createObjectURL(file);
                    MediaService.uploadImage({ folder: scope.folder, image: file, tags: scope.tags })
                    .then(function (response) {
                        var data = angular.fromJson(response.data);
                        if (data.Success) {
                            if (scope.images) {
                                scope.images.unshift({ Image: data.Url, PublicId: data.PublicId });
                            }

                            scope.model = data.Url;
                        }

                        $rootScope.handleMessages(data.Messages);
                    }, function (error) {
                        $rootScope.handleMessages(angular.fromJson(error.data).Messages);
                    }).finally(function () {
                        scope.dataUrl = null;
                        element.find('[type="file"]')
                        element.wrap('<form>').closest('form').get(0).reset();
                        element.unwrap();
                        scope.uploading = false;
                    });
                }   
            });
        },
        templateUrl: '/assets/js/directives/templates/input-image-picker.html'
    }
}]);

app.directive('inputIconPicker', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            model: '=',
            colorModel: '=',
            errors: '=?',
            warnings: '=?',
            hide: '=?',
            panel: '=?',
            required: '=?',
            disable: '=?',
            halfField: '@?',
            inputId: '@',
            label: '@',
            change: '&?'
        },
        link: function (scope, element, attrs) {
            //List is generated from executing a console code into font awesome's cheatsheet page
            scope.icons = [{ "code": "&#xf26e;", "name": "500px" }, { "code": "&#xf2b9;", "name": "address book" }, { "code": "&#xf2ba;", "name": "address book o" }, { "code": "&#xf2bb;", "name": "address card" }, { "code": "&#xf2bc;", "name": "address card o" }, { "code": "&#xf042;", "name": "adjust" }, { "code": "&#xf170;", "name": "adn" }, { "code": "&#xf037;", "name": "align center" }, { "code": "&#xf039;", "name": "align justify" }, { "code": "&#xf036;", "name": "align left" }, { "code": "&#xf038;", "name": "align right" }, { "code": "&#xf270;", "name": "amazon" }, { "code": "&#xf0f9;", "name": "ambulance" }, { "code": "&#xf2a3;", "name": "american sign language interpreting" }, { "code": "&#xf13d;", "name": "anchor" }, { "code": "&#xf17b;", "name": "android" }, { "code": "&#xf209;", "name": "angellist" }, { "code": "&#xf103;", "name": "angle double down" }, { "code": "&#xf100;", "name": "angle double left" }, { "code": "&#xf101;", "name": "angle double right" }, { "code": "&#xf102;", "name": "angle double up" }, { "code": "&#xf107;", "name": "angle down" }, { "code": "&#xf104;", "name": "angle left" }, { "code": "&#xf105;", "name": "angle right" }, { "code": "&#xf106;", "name": "angle up" }, { "code": "&#xf179;", "name": "apple" }, { "code": "&#xf187;", "name": "archive" }, { "code": "&#xf1fe;", "name": "area chart" }, { "code": "&#xf0ab;", "name": "arrow circle down" }, { "code": "&#xf0a8;", "name": "arrow circle left" }, { "code": "&#xf01a;", "name": "arrow circle o down" }, { "code": "&#xf190;", "name": "arrow circle o left" }, { "code": "&#xf18e;", "name": "arrow circle o right" }, { "code": "&#xf01b;", "name": "arrow circle o up" }, { "code": "&#xf0a9;", "name": "arrow circle right" }, { "code": "&#xf0aa;", "name": "arrow circle up" }, { "code": "&#xf063;", "name": "arrow down" }, { "code": "&#xf060;", "name": "arrow left" }, { "code": "&#xf061;", "name": "arrow right" }, { "code": "&#xf062;", "name": "arrow up" }, { "code": "&#xf047;", "name": "arrows" }, { "code": "&#xf0b2;", "name": "arrows alt" }, { "code": "&#xf07e;", "name": "arrows h" }, { "code": "&#xf07d;", "name": "arrows v" }, { "code": "&#xf2a2;", "name": "assistive listening systems" }, { "code": "&#xf069;", "name": "asterisk" }, { "code": "&#xf1fa;", "name": "at" }, { "code": "&#xf29e;", "name": "audio description" }, { "code": "&#xf04a;", "name": "backward" }, { "code": "&#xf24e;", "name": "balance scale" }, { "code": "&#xf05e;", "name": "ban" }, { "code": "&#xf2d5;", "name": "bandcamp" }, { "code": "&#xf080;", "name": "bar chart" }, { "code": "&#xf02a;", "name": "barcode" }, { "code": "&#xf0c9;", "name": "bars" }, { "code": "&#xf2cd;", "name": "bath" }, { "code": "&#xf244;", "name": "battery empty" }, { "code": "&#xf240;", "name": "battery full" }, { "code": "&#xf242;", "name": "battery half" }, { "code": "&#xf243;", "name": "battery quarter" }, { "code": "&#xf241;", "name": "battery three quarters" }, { "code": "&#xf236;", "name": "bed" }, { "code": "&#xf0fc;", "name": "beer" }, { "code": "&#xf1b4;", "name": "behance" }, { "code": "&#xf1b5;", "name": "behance square" }, { "code": "&#xf0f3;", "name": "bell" }, { "code": "&#xf0a2;", "name": "bell o" }, { "code": "&#xf1f6;", "name": "bell slash" }, { "code": "&#xf1f7;", "name": "bell slash o" }, { "code": "&#xf206;", "name": "bicycle" }, { "code": "&#xf1e5;", "name": "binoculars" }, { "code": "&#xf1fd;", "name": "birthday cake" }, { "code": "&#xf171;", "name": "bitbucket" }, { "code": "&#xf172;", "name": "bitbucket square" }, { "code": "&#xf27e;", "name": "black tie" }, { "code": "&#xf29d;", "name": "blind" }, { "code": "&#xf293;", "name": "bluetooth" }, { "code": "&#xf294;", "name": "bluetooth b" }, { "code": "&#xf032;", "name": "bold" }, { "code": "&#xf0e7;", "name": "bolt" }, { "code": "&#xf1e2;", "name": "bomb" }, { "code": "&#xf02d;", "name": "book" }, { "code": "&#xf02e;", "name": "bookmark" }, { "code": "&#xf097;", "name": "bookmark o" }, { "code": "&#xf2a1;", "name": "braille" }, { "code": "&#xf0b1;", "name": "briefcase" }, { "code": "&#xf15a;", "name": "btc" }, { "code": "&#xf188;", "name": "bug" }, { "code": "&#xf1ad;", "name": "building" }, { "code": "&#xf0f7;", "name": "building o" }, { "code": "&#xf0a1;", "name": "bullhorn" }, { "code": "&#xf140;", "name": "bullseye" }, { "code": "&#xf207;", "name": "bus" }, { "code": "&#xf20d;", "name": "buysellads" }, { "code": "&#xf1ec;", "name": "calculator" }, { "code": "&#xf073;", "name": "calendar" }, { "code": "&#xf274;", "name": "calendar check o" }, { "code": "&#xf272;", "name": "calendar minus o" }, { "code": "&#xf133;", "name": "calendar o" }, { "code": "&#xf271;", "name": "calendar plus o" }, { "code": "&#xf273;", "name": "calendar times o" }, { "code": "&#xf030;", "name": "camera" }, { "code": "&#xf083;", "name": "camera retro" }, { "code": "&#xf1b9;", "name": "car" }, { "code": "&#xf0d7;", "name": "caret down" }, { "code": "&#xf0d9;", "name": "caret left" }, { "code": "&#xf0da;", "name": "caret right" }, { "code": "&#xf150;", "name": "caret square o down" }, { "code": "&#xf191;", "name": "caret square o left" }, { "code": "&#xf152;", "name": "caret square o right" }, { "code": "&#xf151;", "name": "caret square o up" }, { "code": "&#xf0d8;", "name": "caret up" }, { "code": "&#xf218;", "name": "cart arrow down" }, { "code": "&#xf217;", "name": "cart plus" }, { "code": "&#xf20a;", "name": "cc" }, { "code": "&#xf1f3;", "name": "cc amex" }, { "code": "&#xf24c;", "name": "cc diners club" }, { "code": "&#xf1f2;", "name": "cc discover" }, { "code": "&#xf24b;", "name": "cc jcb" }, { "code": "&#xf1f1;", "name": "cc mastercard" }, { "code": "&#xf1f4;", "name": "cc paypal" }, { "code": "&#xf1f5;", "name": "cc stripe" }, { "code": "&#xf1f0;", "name": "cc visa" }, { "code": "&#xf0a3;", "name": "certificate" }, { "code": "&#xf127;", "name": "chain broken" }, { "code": "&#xf00c;", "name": "check" }, { "code": "&#xf058;", "name": "check circle" }, { "code": "&#xf05d;", "name": "check circle o" }, { "code": "&#xf14a;", "name": "check square" }, { "code": "&#xf046;", "name": "check square o" }, { "code": "&#xf13a;", "name": "chevron circle down" }, { "code": "&#xf137;", "name": "chevron circle left" }, { "code": "&#xf138;", "name": "chevron circle right" }, { "code": "&#xf139;", "name": "chevron circle up" }, { "code": "&#xf078;", "name": "chevron down" }, { "code": "&#xf053;", "name": "chevron left" }, { "code": "&#xf054;", "name": "chevron right" }, { "code": "&#xf077;", "name": "chevron up" }, { "code": "&#xf1ae;", "name": "child" }, { "code": "&#xf268;", "name": "chrome" }, { "code": "&#xf111;", "name": "circle" }, { "code": "&#xf10c;", "name": "circle o" }, { "code": "&#xf1ce;", "name": "circle o notch" }, { "code": "&#xf1db;", "name": "circle thin" }, { "code": "&#xf0ea;", "name": "clipboard" }, { "code": "&#xf017;", "name": "clock o" }, { "code": "&#xf24d;", "name": "clone" }, { "code": "&#xf0c2;", "name": "cloud" }, { "code": "&#xf0ed;", "name": "cloud download" }, { "code": "&#xf0ee;", "name": "cloud upload" }, { "code": "&#xf121;", "name": "code" }, { "code": "&#xf126;", "name": "code fork" }, { "code": "&#xf1cb;", "name": "codepen" }, { "code": "&#xf284;", "name": "codiepie" }, { "code": "&#xf0f4;", "name": "coffee" }, { "code": "&#xf013;", "name": "cog" }, { "code": "&#xf085;", "name": "cogs" }, { "code": "&#xf0db;", "name": "columns" }, { "code": "&#xf075;", "name": "comment" }, { "code": "&#xf0e5;", "name": "comment o" }, { "code": "&#xf27a;", "name": "commenting" }, { "code": "&#xf27b;", "name": "commenting o" }, { "code": "&#xf086;", "name": "comments" }, { "code": "&#xf0e6;", "name": "comments o" }, { "code": "&#xf14e;", "name": "compass" }, { "code": "&#xf066;", "name": "compress" }, { "code": "&#xf20e;", "name": "connectdevelop" }, { "code": "&#xf26d;", "name": "contao" }, { "code": "&#xf1f9;", "name": "copyright" }, { "code": "&#xf25e;", "name": "creative commons" }, { "code": "&#xf09d;", "name": "credit card" }, { "code": "&#xf283;", "name": "credit card alt" }, { "code": "&#xf125;", "name": "crop" }, { "code": "&#xf05b;", "name": "crosshairs" }, { "code": "&#xf13c;", "name": "css3" }, { "code": "&#xf1b2;", "name": "cube" }, { "code": "&#xf1b3;", "name": "cubes" }, { "code": "&#xf0f5;", "name": "cutlery" }, { "code": "&#xf210;", "name": "dashcube" }, { "code": "&#xf1c0;", "name": "database" }, { "code": "&#xf2a4;", "name": "deaf" }, { "code": "&#xf1a5;", "name": "delicious" }, { "code": "&#xf108;", "name": "desktop" }, { "code": "&#xf1bd;", "name": "deviantart" }, { "code": "&#xf219;", "name": "diamond" }, { "code": "&#xf1a6;", "name": "digg" }, { "code": "&#xf192;", "name": "dot circle o" }, { "code": "&#xf019;", "name": "download" }, { "code": "&#xf17d;", "name": "dribbble" }, { "code": "&#xf16b;", "name": "dropbox" }, { "code": "&#xf1a9;", "name": "drupal" }, { "code": "&#xf282;", "name": "edge" }, { "code": "&#xf2da;", "name": "eercast" }, { "code": "&#xf052;", "name": "eject" }, { "code": "&#xf141;", "name": "ellipsis h" }, { "code": "&#xf142;", "name": "ellipsis v" }, { "code": "&#xf1d1;", "name": "empire" }, { "code": "&#xf0e0;", "name": "envelope" }, { "code": "&#xf003;", "name": "envelope o" }, { "code": "&#xf2b6;", "name": "envelope open" }, { "code": "&#xf2b7;", "name": "envelope open o" }, { "code": "&#xf199;", "name": "envelope square" }, { "code": "&#xf299;", "name": "envira" }, { "code": "&#xf12d;", "name": "eraser" }, { "code": "&#xf2d7;", "name": "etsy" }, { "code": "&#xf153;", "name": "eur" }, { "code": "&#xf0ec;", "name": "exchange" }, { "code": "&#xf12a;", "name": "exclamation" }, { "code": "&#xf06a;", "name": "exclamation circle" }, { "code": "&#xf071;", "name": "exclamation triangle" }, { "code": "&#xf065;", "name": "expand" }, { "code": "&#xf23e;", "name": "expeditedssl" }, { "code": "&#xf08e;", "name": "external link" }, { "code": "&#xf14c;", "name": "external link square" }, { "code": "&#xf06e;", "name": "eye" }, { "code": "&#xf070;", "name": "eye slash" }, { "code": "&#xf1fb;", "name": "eyedropper" }, { "code": "&#xf09a;", "name": "facebook" }, { "code": "&#xf230;", "name": "facebook official" }, { "code": "&#xf082;", "name": "facebook square" }, { "code": "&#xf049;", "name": "fast backward" }, { "code": "&#xf050;", "name": "fast forward" }, { "code": "&#xf1ac;", "name": "fax" }, { "code": "&#xf182;", "name": "female" }, { "code": "&#xf0fb;", "name": "fighter jet" }, { "code": "&#xf15b;", "name": "file" }, { "code": "&#xf1c6;", "name": "file archive o" }, { "code": "&#xf1c7;", "name": "file audio o" }, { "code": "&#xf1c9;", "name": "file code o" }, { "code": "&#xf1c3;", "name": "file excel o" }, { "code": "&#xf1c5;", "name": "file image o" }, { "code": "&#xf016;", "name": "file o" }, { "code": "&#xf1c1;", "name": "file pdf o" }, { "code": "&#xf1c4;", "name": "file powerpoint o" }, { "code": "&#xf15c;", "name": "file text" }, { "code": "&#xf0f6;", "name": "file text o" }, { "code": "&#xf1c8;", "name": "file video o" }, { "code": "&#xf1c2;", "name": "file word o" }, { "code": "&#xf0c5;", "name": "files o" }, { "code": "&#xf008;", "name": "film" }, { "code": "&#xf0b0;", "name": "filter" }, { "code": "&#xf06d;", "name": "fire" }, { "code": "&#xf134;", "name": "fire extinguisher" }, { "code": "&#xf269;", "name": "firefox" }, { "code": "&#xf2b0;", "name": "first order" }, { "code": "&#xf024;", "name": "flag" }, { "code": "&#xf11e;", "name": "flag checkered" }, { "code": "&#xf11d;", "name": "flag o" }, { "code": "&#xf0c3;", "name": "flask" }, { "code": "&#xf16e;", "name": "flickr" }, { "code": "&#xf0c7;", "name": "floppy o" }, { "code": "&#xf07b;", "name": "folder" }, { "code": "&#xf114;", "name": "folder o" }, { "code": "&#xf07c;", "name": "folder open" }, { "code": "&#xf115;", "name": "folder open o" }, { "code": "&#xf031;", "name": "font" }, { "code": "&#xf2b4;", "name": "font awesome" }, { "code": "&#xf280;", "name": "fonticons" }, { "code": "&#xf286;", "name": "fort awesome" }, { "code": "&#xf211;", "name": "forumbee" }, { "code": "&#xf04e;", "name": "forward" }, { "code": "&#xf180;", "name": "foursquare" }, { "code": "&#xf2c5;", "name": "free code camp" }, { "code": "&#xf119;", "name": "frown o" }, { "code": "&#xf1e3;", "name": "futbol o" }, { "code": "&#xf11b;", "name": "gamepad" }, { "code": "&#xf0e3;", "name": "gavel" }, { "code": "&#xf154;", "name": "gbp" }, { "code": "&#xf22d;", "name": "genderless" }, { "code": "&#xf265;", "name": "get pocket" }, { "code": "&#xf260;", "name": "gg" }, { "code": "&#xf261;", "name": "gg circle" }, { "code": "&#xf06b;", "name": "gift" }, { "code": "&#xf1d3;", "name": "git" }, { "code": "&#xf1d2;", "name": "git square" }, { "code": "&#xf09b;", "name": "github" }, { "code": "&#xf113;", "name": "github alt" }, { "code": "&#xf092;", "name": "github square" }, { "code": "&#xf296;", "name": "gitlab" }, { "code": "&#xf000;", "name": "glass" }, { "code": "&#xf2a5;", "name": "glide" }, { "code": "&#xf2a6;", "name": "glide g" }, { "code": "&#xf0ac;", "name": "globe" }, { "code": "&#xf1a0;", "name": "google" }, { "code": "&#xf0d5;", "name": "google plus" }, { "code": "&#xf2b3;", "name": "google plus official" }, { "code": "&#xf0d4;", "name": "google plus square" }, { "code": "&#xf1ee;", "name": "google wallet" }, { "code": "&#xf19d;", "name": "graduation cap" }, { "code": "&#xf184;", "name": "gratipay" }, { "code": "&#xf2d6;", "name": "grav" }, { "code": "&#xf0fd;", "name": "h square" }, { "code": "&#xf1d4;", "name": "hacker news" }, { "code": "&#xf258;", "name": "hand lizard o" }, { "code": "&#xf0a7;", "name": "hand o down" }, { "code": "&#xf0a5;", "name": "hand o left" }, { "code": "&#xf0a4;", "name": "hand o right" }, { "code": "&#xf0a6;", "name": "hand o up" }, { "code": "&#xf256;", "name": "hand paper o" }, { "code": "&#xf25b;", "name": "hand peace o" }, { "code": "&#xf25a;", "name": "hand pointer o" }, { "code": "&#xf255;", "name": "hand rock o" }, { "code": "&#xf257;", "name": "hand scissors o" }, { "code": "&#xf259;", "name": "hand spock o" }, { "code": "&#xf2b5;", "name": "handshake o" }, { "code": "&#xf292;", "name": "hashtag" }, { "code": "&#xf0a0;", "name": "hdd o" }, { "code": "&#xf1dc;", "name": "header" }, { "code": "&#xf025;", "name": "headphones" }, { "code": "&#xf004;", "name": "heart" }, { "code": "&#xf08a;", "name": "heart o" }, { "code": "&#xf21e;", "name": "heartbeat" }, { "code": "&#xf1da;", "name": "history" }, { "code": "&#xf015;", "name": "home" }, { "code": "&#xf0f8;", "name": "hospital o" }, { "code": "&#xf254;", "name": "hourglass" }, { "code": "&#xf253;", "name": "hourglass end" }, { "code": "&#xf252;", "name": "hourglass half" }, { "code": "&#xf250;", "name": "hourglass o" }, { "code": "&#xf251;", "name": "hourglass start" }, { "code": "&#xf27c;", "name": "houzz" }, { "code": "&#xf13b;", "name": "html5" }, { "code": "&#xf246;", "name": "i cursor" }, { "code": "&#xf2c1;", "name": "id badge" }, { "code": "&#xf2c2;", "name": "id card" }, { "code": "&#xf2c3;", "name": "id card o" }, { "code": "&#xf20b;", "name": "ils" }, { "code": "&#xf2d8;", "name": "imdb" }, { "code": "&#xf01c;", "name": "inbox" }, { "code": "&#xf03c;", "name": "indent" }, { "code": "&#xf275;", "name": "industry" }, { "code": "&#xf129;", "name": "info" }, { "code": "&#xf05a;", "name": "info circle" }, { "code": "&#xf156;", "name": "inr" }, { "code": "&#xf16d;", "name": "instagram" }, { "code": "&#xf26b;", "name": "internet explorer" }, { "code": "&#xf208;", "name": "ioxhost" }, { "code": "&#xf033;", "name": "italic" }, { "code": "&#xf1aa;", "name": "joomla" }, { "code": "&#xf157;", "name": "jpy" }, { "code": "&#xf1cc;", "name": "jsfiddle" }, { "code": "&#xf084;", "name": "key" }, { "code": "&#xf11c;", "name": "keyboard o" }, { "code": "&#xf159;", "name": "krw" }, { "code": "&#xf1ab;", "name": "language" }, { "code": "&#xf109;", "name": "laptop" }, { "code": "&#xf202;", "name": "lastfm" }, { "code": "&#xf203;", "name": "lastfm square" }, { "code": "&#xf06c;", "name": "leaf" }, { "code": "&#xf212;", "name": "leanpub" }, { "code": "&#xf094;", "name": "lemon o" }, { "code": "&#xf149;", "name": "level down" }, { "code": "&#xf148;", "name": "level up" }, { "code": "&#xf1cd;", "name": "life ring" }, { "code": "&#xf0eb;", "name": "lightbulb o" }, { "code": "&#xf201;", "name": "line chart" }, { "code": "&#xf0c1;", "name": "link" }, { "code": "&#xf0e1;", "name": "linkedin" }, { "code": "&#xf08c;", "name": "linkedin square" }, { "code": "&#xf2b8;", "name": "linode" }, { "code": "&#xf17c;", "name": "linux" }, { "code": "&#xf03a;", "name": "list" }, { "code": "&#xf022;", "name": "list alt" }, { "code": "&#xf0cb;", "name": "list ol" }, { "code": "&#xf0ca;", "name": "list ul" }, { "code": "&#xf124;", "name": "location arrow" }, { "code": "&#xf023;", "name": "lock" }, { "code": "&#xf175;", "name": "long arrow down" }, { "code": "&#xf177;", "name": "long arrow left" }, { "code": "&#xf178;", "name": "long arrow right" }, { "code": "&#xf176;", "name": "long arrow up" }, { "code": "&#xf2a8;", "name": "low vision" }, { "code": "&#xf0d0;", "name": "magic" }, { "code": "&#xf076;", "name": "magnet" }, { "code": "&#xf183;", "name": "male" }, { "code": "&#xf279;", "name": "map" }, { "code": "&#xf041;", "name": "map marker" }, { "code": "&#xf278;", "name": "map o" }, { "code": "&#xf276;", "name": "map pin" }, { "code": "&#xf277;", "name": "map signs" }, { "code": "&#xf222;", "name": "mars" }, { "code": "&#xf227;", "name": "mars double" }, { "code": "&#xf229;", "name": "mars stroke" }, { "code": "&#xf22b;", "name": "mars stroke h" }, { "code": "&#xf22a;", "name": "mars stroke v" }, { "code": "&#xf136;", "name": "maxcdn" }, { "code": "&#xf20c;", "name": "meanpath" }, { "code": "&#xf23a;", "name": "medium" }, { "code": "&#xf0fa;", "name": "medkit" }, { "code": "&#xf2e0;", "name": "meetup" }, { "code": "&#xf11a;", "name": "meh o" }, { "code": "&#xf223;", "name": "mercury" }, { "code": "&#xf2db;", "name": "microchip" }, { "code": "&#xf130;", "name": "microphone" }, { "code": "&#xf131;", "name": "microphone slash" }, { "code": "&#xf068;", "name": "minus" }, { "code": "&#xf056;", "name": "minus circle" }, { "code": "&#xf146;", "name": "minus square" }, { "code": "&#xf147;", "name": "minus square o" }, { "code": "&#xf289;", "name": "mixcloud" }, { "code": "&#xf10b;", "name": "mobile" }, { "code": "&#xf285;", "name": "modx" }, { "code": "&#xf0d6;", "name": "money" }, { "code": "&#xf186;", "name": "moon o" }, { "code": "&#xf21c;", "name": "motorcycle" }, { "code": "&#xf245;", "name": "mouse pointer" }, { "code": "&#xf001;", "name": "music" }, { "code": "&#xf22c;", "name": "neuter" }, { "code": "&#xf1ea;", "name": "newspaper o" }, { "code": "&#xf247;", "name": "object group" }, { "code": "&#xf248;", "name": "object ungroup" }, { "code": "&#xf263;", "name": "odnoklassniki" }, { "code": "&#xf264;", "name": "odnoklassniki square" }, { "code": "&#xf23d;", "name": "opencart" }, { "code": "&#xf19b;", "name": "openid" }, { "code": "&#xf26a;", "name": "opera" }, { "code": "&#xf23c;", "name": "optin monster" }, { "code": "&#xf03b;", "name": "outdent" }, { "code": "&#xf18c;", "name": "pagelines" }, { "code": "&#xf1fc;", "name": "paint brush" }, { "code": "&#xf1d8;", "name": "paper plane" }, { "code": "&#xf1d9;", "name": "paper plane o" }, { "code": "&#xf0c6;", "name": "paperclip" }, { "code": "&#xf1dd;", "name": "paragraph" }, { "code": "&#xf04c;", "name": "pause" }, { "code": "&#xf28b;", "name": "pause circle" }, { "code": "&#xf28c;", "name": "pause circle o" }, { "code": "&#xf1b0;", "name": "paw" }, { "code": "&#xf1ed;", "name": "paypal" }, { "code": "&#xf040;", "name": "pencil" }, { "code": "&#xf14b;", "name": "pencil square" }, { "code": "&#xf044;", "name": "pencil square o" }, { "code": "&#xf295;", "name": "percent" }, { "code": "&#xf095;", "name": "phone" }, { "code": "&#xf098;", "name": "phone square" }, { "code": "&#xf03e;", "name": "picture o" }, { "code": "&#xf200;", "name": "pie chart" }, { "code": "&#xf2ae;", "name": "pied piper" }, { "code": "&#xf1a8;", "name": "pied piper alt" }, { "code": "&#xf1a7;", "name": "pied piper pp" }, { "code": "&#xf0d2;", "name": "pinterest" }, { "code": "&#xf231;", "name": "pinterest p" }, { "code": "&#xf0d3;", "name": "pinterest square" }, { "code": "&#xf072;", "name": "plane" }, { "code": "&#xf04b;", "name": "play" }, { "code": "&#xf144;", "name": "play circle" }, { "code": "&#xf01d;", "name": "play circle o" }, { "code": "&#xf1e6;", "name": "plug" }, { "code": "&#xf067;", "name": "plus" }, { "code": "&#xf055;", "name": "plus circle" }, { "code": "&#xf0fe;", "name": "plus square" }, { "code": "&#xf196;", "name": "plus square o" }, { "code": "&#xf2ce;", "name": "podcast" }, { "code": "&#xf011;", "name": "power off" }, { "code": "&#xf02f;", "name": "print" }, { "code": "&#xf288;", "name": "product hunt" }, { "code": "&#xf12e;", "name": "puzzle piece" }, { "code": "&#xf1d6;", "name": "qq" }, { "code": "&#xf029;", "name": "qrcode" }, { "code": "&#xf128;", "name": "question" }, { "code": "&#xf059;", "name": "question circle" }, { "code": "&#xf29c;", "name": "question circle o" }, { "code": "&#xf2c4;", "name": "quora" }, { "code": "&#xf10d;", "name": "quote left" }, { "code": "&#xf10e;", "name": "quote right" }, { "code": "&#xf074;", "name": "random" }, { "code": "&#xf2d9;", "name": "ravelry" }, { "code": "&#xf1d0;", "name": "rebel" }, { "code": "&#xf1b8;", "name": "recycle" }, { "code": "&#xf1a1;", "name": "reddit" }, { "code": "&#xf281;", "name": "reddit alien" }, { "code": "&#xf1a2;", "name": "reddit square" }, { "code": "&#xf021;", "name": "refresh" }, { "code": "&#xf25d;", "name": "registered" }, { "code": "&#xf18b;", "name": "renren" }, { "code": "&#xf01e;", "name": "repeat" }, { "code": "&#xf112;", "name": "reply" }, { "code": "&#xf122;", "name": "reply all" }, { "code": "&#xf079;", "name": "retweet" }, { "code": "&#xf018;", "name": "road" }, { "code": "&#xf135;", "name": "rocket" }, { "code": "&#xf09e;", "name": "rss" }, { "code": "&#xf143;", "name": "rss square" }, { "code": "&#xf158;", "name": "rub" }, { "code": "&#xf267;", "name": "safari" }, { "code": "&#xf0c4;", "name": "scissors" }, { "code": "&#xf28a;", "name": "scribd" }, { "code": "&#xf002;", "name": "search" }, { "code": "&#xf010;", "name": "search minus" }, { "code": "&#xf00e;", "name": "search plus" }, { "code": "&#xf213;", "name": "sellsy" }, { "code": "&#xf233;", "name": "server" }, { "code": "&#xf064;", "name": "share" }, { "code": "&#xf1e0;", "name": "share alt" }, { "code": "&#xf1e1;", "name": "share alt square" }, { "code": "&#xf14d;", "name": "share square" }, { "code": "&#xf045;", "name": "share square o" }, { "code": "&#xf132;", "name": "shield" }, { "code": "&#xf21a;", "name": "ship" }, { "code": "&#xf214;", "name": "shirtsinbulk" }, { "code": "&#xf290;", "name": "shopping bag" }, { "code": "&#xf291;", "name": "shopping basket" }, { "code": "&#xf07a;", "name": "shopping cart" }, { "code": "&#xf2cc;", "name": "shower" }, { "code": "&#xf090;", "name": "sign in" }, { "code": "&#xf2a7;", "name": "sign language" }, { "code": "&#xf08b;", "name": "sign out" }, { "code": "&#xf012;", "name": "signal" }, { "code": "&#xf215;", "name": "simplybuilt" }, { "code": "&#xf0e8;", "name": "sitemap" }, { "code": "&#xf216;", "name": "skyatlas" }, { "code": "&#xf17e;", "name": "skype" }, { "code": "&#xf198;", "name": "slack" }, { "code": "&#xf1de;", "name": "sliders" }, { "code": "&#xf1e7;", "name": "slideshare" }, { "code": "&#xf118;", "name": "smile o" }, { "code": "&#xf2ab;", "name": "snapchat" }, { "code": "&#xf2ac;", "name": "snapchat ghost" }, { "code": "&#xf2ad;", "name": "snapchat square" }, { "code": "&#xf2dc;", "name": "snowflake o" }, { "code": "&#xf0dc;", "name": "sort" }, { "code": "&#xf15d;", "name": "sort alpha asc" }, { "code": "&#xf15e;", "name": "sort alpha desc" }, { "code": "&#xf160;", "name": "sort amount asc" }, { "code": "&#xf161;", "name": "sort amount desc" }, { "code": "&#xf0de;", "name": "sort asc" }, { "code": "&#xf0dd;", "name": "sort desc" }, { "code": "&#xf162;", "name": "sort numeric asc" }, { "code": "&#xf163;", "name": "sort numeric desc" }, { "code": "&#xf1be;", "name": "soundcloud" }, { "code": "&#xf197;", "name": "space shuttle" }, { "code": "&#xf110;", "name": "spinner" }, { "code": "&#xf1b1;", "name": "spoon" }, { "code": "&#xf1bc;", "name": "spotify" }, { "code": "&#xf0c8;", "name": "square" }, { "code": "&#xf096;", "name": "square o" }, { "code": "&#xf18d;", "name": "stack exchange" }, { "code": "&#xf16c;", "name": "stack overflow" }, { "code": "&#xf005;", "name": "star" }, { "code": "&#xf089;", "name": "star half" }, { "code": "&#xf123;", "name": "star half o" }, { "code": "&#xf006;", "name": "star o" }, { "code": "&#xf1b6;", "name": "steam" }, { "code": "&#xf1b7;", "name": "steam square" }, { "code": "&#xf048;", "name": "step backward" }, { "code": "&#xf051;", "name": "step forward" }, { "code": "&#xf0f1;", "name": "stethoscope" }, { "code": "&#xf249;", "name": "sticky note" }, { "code": "&#xf24a;", "name": "sticky note o" }, { "code": "&#xf04d;", "name": "stop" }, { "code": "&#xf28d;", "name": "stop circle" }, { "code": "&#xf28e;", "name": "stop circle o" }, { "code": "&#xf21d;", "name": "street view" }, { "code": "&#xf0cc;", "name": "strikethrough" }, { "code": "&#xf1a4;", "name": "stumbleupon" }, { "code": "&#xf1a3;", "name": "stumbleupon circle" }, { "code": "&#xf12c;", "name": "subscript" }, { "code": "&#xf239;", "name": "subway" }, { "code": "&#xf0f2;", "name": "suitcase" }, { "code": "&#xf185;", "name": "sun o" }, { "code": "&#xf2dd;", "name": "superpowers" }, { "code": "&#xf12b;", "name": "superscript" }, { "code": "&#xf0ce;", "name": "table" }, { "code": "&#xf10a;", "name": "tablet" }, { "code": "&#xf0e4;", "name": "tachometer" }, { "code": "&#xf02b;", "name": "tag" }, { "code": "&#xf02c;", "name": "tags" }, { "code": "&#xf0ae;", "name": "tasks" }, { "code": "&#xf1ba;", "name": "taxi" }, { "code": "&#xf2c6;", "name": "telegram" }, { "code": "&#xf26c;", "name": "television" }, { "code": "&#xf1d5;", "name": "tencent weibo" }, { "code": "&#xf120;", "name": "terminal" }, { "code": "&#xf034;", "name": "text height" }, { "code": "&#xf035;", "name": "text width" }, { "code": "&#xf00a;", "name": "th" }, { "code": "&#xf009;", "name": "th large" }, { "code": "&#xf00b;", "name": "th list" }, { "code": "&#xf2b2;", "name": "themeisle" }, { "code": "&#xf2cb;", "name": "thermometer empty" }, { "code": "&#xf2c7;", "name": "thermometer full" }, { "code": "&#xf2c9;", "name": "thermometer half" }, { "code": "&#xf2ca;", "name": "thermometer quarter" }, { "code": "&#xf2c8;", "name": "thermometer three quarters" }, { "code": "&#xf08d;", "name": "thumb tack" }, { "code": "&#xf165;", "name": "thumbs down" }, { "code": "&#xf088;", "name": "thumbs o down" }, { "code": "&#xf087;", "name": "thumbs o up" }, { "code": "&#xf164;", "name": "thumbs up" }, { "code": "&#xf145;", "name": "ticket" }, { "code": "&#xf00d;", "name": "times" }, { "code": "&#xf057;", "name": "times circle" }, { "code": "&#xf05c;", "name": "times circle o" }, { "code": "&#xf043;", "name": "tint" }, { "code": "&#xf204;", "name": "toggle off" }, { "code": "&#xf205;", "name": "toggle on" }, { "code": "&#xf25c;", "name": "trademark" }, { "code": "&#xf238;", "name": "train" }, { "code": "&#xf224;", "name": "transgender" }, { "code": "&#xf225;", "name": "transgender alt" }, { "code": "&#xf1f8;", "name": "trash" }, { "code": "&#xf014;", "name": "trash o" }, { "code": "&#xf1bb;", "name": "tree" }, { "code": "&#xf181;", "name": "trello" }, { "code": "&#xf262;", "name": "tripadvisor" }, { "code": "&#xf091;", "name": "trophy" }, { "code": "&#xf0d1;", "name": "truck" }, { "code": "&#xf195;", "name": "try" }, { "code": "&#xf1e4;", "name": "tty" }, { "code": "&#xf173;", "name": "tumblr" }, { "code": "&#xf174;", "name": "tumblr square" }, { "code": "&#xf1e8;", "name": "twitch" }, { "code": "&#xf099;", "name": "twitter" }, { "code": "&#xf081;", "name": "twitter square" }, { "code": "&#xf0e9;", "name": "umbrella" }, { "code": "&#xf0cd;", "name": "underline" }, { "code": "&#xf0e2;", "name": "undo" }, { "code": "&#xf29a;", "name": "universal access" }, { "code": "&#xf19c;", "name": "university" }, { "code": "&#xf09c;", "name": "unlock" }, { "code": "&#xf13e;", "name": "unlock alt" }, { "code": "&#xf093;", "name": "upload" }, { "code": "&#xf287;", "name": "usb" }, { "code": "&#xf155;", "name": "usd" }, { "code": "&#xf007;", "name": "user" }, { "code": "&#xf2bd;", "name": "user circle" }, { "code": "&#xf2be;", "name": "user circle o" }, { "code": "&#xf0f0;", "name": "user md" }, { "code": "&#xf2c0;", "name": "user o" }, { "code": "&#xf234;", "name": "user plus" }, { "code": "&#xf21b;", "name": "user secret" }, { "code": "&#xf235;", "name": "user times" }, { "code": "&#xf0c0;", "name": "users" }, { "code": "&#xf221;", "name": "venus" }, { "code": "&#xf226;", "name": "venus double" }, { "code": "&#xf228;", "name": "venus mars" }, { "code": "&#xf237;", "name": "viacoin" }, { "code": "&#xf2a9;", "name": "viadeo" }, { "code": "&#xf2aa;", "name": "viadeo square" }, { "code": "&#xf03d;", "name": "video camera" }, { "code": "&#xf27d;", "name": "vimeo" }, { "code": "&#xf194;", "name": "vimeo square" }, { "code": "&#xf1ca;", "name": "vine" }, { "code": "&#xf189;", "name": "vk" }, { "code": "&#xf2a0;", "name": "volume control phone" }, { "code": "&#xf027;", "name": "volume down" }, { "code": "&#xf026;", "name": "volume off" }, { "code": "&#xf028;", "name": "volume up" }, { "code": "&#xf18a;", "name": "weibo" }, { "code": "&#xf1d7;", "name": "weixin" }, { "code": "&#xf232;", "name": "whatsapp" }, { "code": "&#xf193;", "name": "wheelchair" }, { "code": "&#xf29b;", "name": "wheelchair alt" }, { "code": "&#xf1eb;", "name": "wifi" }, { "code": "&#xf266;", "name": "wikipedia w" }, { "code": "&#xf2d3;", "name": "window close" }, { "code": "&#xf2d4;", "name": "window close o" }, { "code": "&#xf2d0;", "name": "window maximize" }, { "code": "&#xf2d1;", "name": "window minimize" }, { "code": "&#xf2d2;", "name": "window restore" }, { "code": "&#xf17a;", "name": "windows" }, { "code": "&#xf19a;", "name": "wordpress" }, { "code": "&#xf297;", "name": "wpbeginner" }, { "code": "&#xf2de;", "name": "wpexplorer" }, { "code": "&#xf298;", "name": "wpforms" }, { "code": "&#xf0ad;", "name": "wrench" }, { "code": "&#xf168;", "name": "xing" }, { "code": "&#xf169;", "name": "xing square" }, { "code": "&#xf23b;", "name": "y combinator" }, { "code": "&#xf19e;", "name": "yahoo" }, { "code": "&#xf1e9;", "name": "yelp" }, { "code": "&#xf2b1;", "name": "yoast" }, { "code": "&#xf167;", "name": "youtube" }, { "code": "&#xf16a;", "name": "youtube play" }, { "code": "&#xf166;", "name": "youtube square" }]
            scope.search = '';
            scope.show = false;
            scope.chooseIcon = function (code) {
                scope.model = code;
                scope.show = false;
            }

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
        templateUrl: '/assets/js/directives/templates/input-icon-picker.html'
    }
});