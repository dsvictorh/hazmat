app.controller('PageController', ['$scope', '$rootScope', '$window', '$filter', 'PageService', function ($scope, $rootScope, $window, $filter, PageService) {
    var page;
    var original;
    $scope.model = {};
    $scope.modules = [];

    //Dropdown options
    $scope.pageModuleStates = [{ value: 0, label: 'Published' }, { value: 1, label: 'Preview' }, { value: 2, label: 'Preview Hide' }];
    $scope.moduleTypes = [
        { label: 'Boxes', value: 'Boxes' },
        { label: 'Callout', value: 'Callout' },
        { label: 'Free Text', value: 'FreeText' },
        { label: 'Profile Cards', value: 'ProfileCards' },
        { label: 'Simple Cards', value: 'SimpleCards' },
        { label: 'Gallery', value: 'Gallery' },
        { label: 'Promo', value: 'Promo' }];
    $scope.newModule = {};

    function init() {
        //The modules and information are loaded according to the url. The page's name defines the URL by converting the name to lowercase and replacing spaces for dashes
        page = window.location.pathname.split('/').filter(function (item) { return item; })[2];
        if (page != 'create') {
            PageService.getModules(page).then(function (response) {
                //The original page data is kept in case the admin wants to undo all changes
                original = angular.fromJson(response.data);
                $scope.model = angular.copy(original);
                delete $scope.model.Modules;

                original.Modules = $filter('orderBy')(original.Modules, 'Position');
                for (var i = 0; i < original.Modules.length; i++) {
                    var moduleName = original.Modules[i].Info.ModuleType.Name.toLowerCase();
                    //Each module has a specific template to edit its data. The name of this template needs to reflect the same name of the module's type
                    original.Modules[i].ModuleTemplate = '/assets/js/hazmat/page-module-forms/' + moduleName + '.html';
                    original.Modules[i].Expanded = false;
                    original.Modules[i].Delete = false;
                    initModule(moduleName, original.Modules[i]);
                }

                //The modules for editting are kept on a separate collection so changes to them can be undone
                //However, since each module has it's own save and cancel action we can't reset them when the page basic info is reset
                //therefore a separate array is used rather than using the original vs. scope model reset
                $scope.modules = angular.copy(original.Modules);
                $scope.pageName = $scope.model.Page.Name;
                $scope.success = true;
            }, function (error) {
                $rootScope.handleMessages(angular.fromJson(error.data).Messages);
            }).finally(function () {
                $scope.pageLoaded = true;
            });
        } else {
            original = {
                Page: {
                    Name: null,
                    Visible: false,
                    InMenu: false,
                }
            };
            $scope.pageLoaded = true;
            $scope.success = true;
        }
    }

    $scope.resetPage = function () {
        if (confirm('All unsaved changes to the page will be lost. Are you sure you want to cancel?')) {
            angular.merge($scope.model, original);
            delete $scope.model.Modules;
            //Even though each module handles its own data individually, its position is a collective information handled by the page.
            //That way when changes on one of them are undone we do not risk failling to reset any other module's position previously changed
            //The page's basic info editting includes the module's positions and therefore if the page info is reset so are the positions of existing modules
            for (var i = 0; i < $scope.modules.length; i++) {
                var module = getOriginalModule($scope.modules[i].Info.PageModuleId);
                if (module) {
                    $scope.modules[i].Info.Position = module.Info.Position;
                    $scope.modules[i].Delete = module.Delete;
                }        
            }
        }
    }

    $scope.resetNewModule = function (responseReset) {
        if (responseReset || confirm('All unsaved changes to the module will be lost. Are you sure you want to cancel?')) {
            $scope.newModule.moduleType = null;
            delete $scope.newModule.Details;
            delete $scope.newModule.Info;
            delete $scope.newModule.ModuleTemplate;
        }
    }

    $scope.savePageChanges = function () {
        var moduleActions = [];
        if (!$scope.submitting) {
            $scope.submitting = true;
            for (var i = 0; i < $scope.modules.length; i++) {
                moduleActions.push({ Id: $scope.modules[i].Info.Id, Position: $scope.modules[i].Info.Position, Delete: $scope.modules[i].Delete });
            }
            
            PageService.save(page, {
                id: $scope.model.Page.Id,
                name: $scope.model.Page.Name,
                heroImage: $scope.model.Page.HeroImage,
                mobileHeroImage: $scope.model.Page.MobileHeroImage,
                inMenu: $scope.model.Page.InMenu,
                inBottomMenu: $scope.model.Page.InBottomMenu,
                visible: $scope.model.Page.Visible,
                adminLocked: $rootScope.userDetails.isAdmin ? $scope.model.Page.AdminLocked : null,
                moduleActions: moduleActions,
            }).then(function (response) {
                var data = angular.fromJson(response.data);
                $rootScope.handleMessages(data.Messages);

                if (data.Success) {
                    //Since this form handles which page to load according to its URL (page name), in case a page name has occurred
                    //the response will detail the new url for this page and therefore reload the page
                    if (data.RedirectTo) {
                        $window.location.href = '/hazmat/page/' + data.RedirectTo;
                    } else {
                        $scope.model.Id = data.Id;
                        //Once the page has been successfully saved the positions on the modules become permanent and the original
                        //data needs to reflect as such
                        for (var i = 0; i < $scope.modules.length; i++) {
                            if ($scope.modules[i].Delete) {
                                var originalIndex = original.Modules.findIndex(function (item) { return item.Info.PageModuleId === $scope.modules[i].Info.PageModuleId });
                                if (originalIndex >= 0) {
                                    $scope.modules.splice(i, 1);
                                    original.Modules.splice(originalIndex, 1);
                                    i--;
                                }
                            } else {
                                var module = getOriginalModule($scope.modules[i].Info.PageModuleId);
                                if (module) {
                                    var action = data.ModuleActions.find(function(item){ return item.Id == module.Info.PageModuleId });
                                    if (action) {
                                        $scope.modules[i].Info.Position = action.Position;
                                        module.Info.Position = action.Position;
                                    }
                                }
                            }
                        }
                        angular.merge(original, $scope.model);
                    }                    
                }
            }, function (error) {
                $rootScope.handleMessages(angular.fromJson(error.data).Messages);
            }).finally(function () {
                $scope.submitting = false;
            });
        }

        return false;
    }

    $scope.resetModule = function (module) {
        if(!$scope.moduleReset){
            if (confirm('All unsaved changes to the module will be lost. Are you sure you want to cancel?')) {
                //The module's position is handled by the page info rather by than itself
                //as it is collective information between modules, therefore resetting the module's content
                //should not reset the temporary position.
                //We also don't want resetting the module to cause it to collapse
                var position = module.Info.Position;
                var expanded = module.Expanded;
                $scope.moduleReset = module;
                angular.copy(getOriginalModule($scope.moduleReset.Info.PageModuleId), $scope.moduleReset);
                $scope.moduleReset.Info.Position = position;
                $scope.moduleReset.Expanded = expanded;
                delete $scope.moduleReset;
            }
        }
    }

    $scope.saveModule = function (moduleName, module) {
        if (!$scope.submitting) {
            $scope.submitting = true;
            moduleName = moduleName.toLowerCase();
            PageService.saveModule(page, moduleName, getModuleData(moduleName, module))
            .then(function (response) {
                var data = angular.fromJson(response.data);
                $rootScope.handleMessages(data.Messages);

                if (data.Success) {
                    $scope.module = module;
                    setOriginalModule(moduleName, $scope.module, data);
                    delete $scope.module;
                }
            }, function (error) {
                $rootScope.handleMessages(angular.fromJson(error.data).Messages);
            }).finally(function () {
                $scope.submitting = false;
            });
        }
    }

    $scope.addModuleSubitem = function (collection, defaultValues) {
        //This function copies any subitem from a module and sets the necessary default values. 
        //It then proceeds to insert it into its respective collection
        //Example: used to create a new box in the "Boxes" module
        if (collection) {
            collection.push(defaultValues);
        } else {
            console.error('collection cant be null')
        }
    }

    $scope.toggleExpandModule = function (module) {
        if (!$scope.moduleExpand) {
            $scope.moduleExpand = module;
            $scope.moduleExpand.Expanded = !$scope.moduleExpand.Expanded;
            delete $scope.moduleExpand;
        }
    }

    $scope.deleteItem = function (index,  collection, actions) {
        if (!$scope.collection) {
            $scope.collection = collection;

            //When togglePropName is populated the property specified is to be reversed in value (true to false, false to true)
            if (actions.togglePropName) {
                $scope.item = $scope.collection[index];
                $scope.item[actions.togglePropName] = $scope.item.hasOwnProperty(actions.togglePropName) ? !$scope.item[actions.togglePropName] : true;
                delete $scope.item;
            }

            //When removeItem is set to true the item is to be removed from the collection
            if (actions.removeItem) {
                $scope.collection.splice(index, 1);
            }

            //When positionUpdatePropName is populated, the item is set to update the property specified according to its position on the final array
            if (actions.updatePositionPropName) {
                for (var i = 0; i < $scope.collection.length; i++) {
                    if ($scope.collection[i].hasOwnProperty(actions.updatePositionPropName)) {
                        $scope.collection[i][actions.updatePositionPropName] = i + 1;
                    }
                }
            }
            
            delete $scope.collection;
        }
    }

    function getOriginalModule(pageModuleId) {
        return original.Modules.find(function (item) { return item.Info.Id == pageModuleId });
    }

    function setOriginalModule(moduleName, module, data) {
        var originalIndex = original.Modules.findIndex(function (item) { return item.Info.Id == module.Info.Id });
        setModuleData(moduleName, module, data);

        //If the module is not found it's because it's a newly added module and has to be inserted on all collections
        if (originalIndex >= 0) {
            original.Modules[originalIndex] = angular.copy(module);
        } else {
            $scope.modules.push(angular.copy(module));
            original.Modules.push(angular.copy(module));
            $scope.resetNewModule(true);
        }

        
    }

    //Module specific functions
    $scope.$watch('newModule.moduleType', function () {
        if ($scope.newModule.moduleType) {
            $scope.newModule.Details = {};
            $scope.newModule.Info = {
                State: 1,
                ModuleType: { Name: $scope.newModule.moduleType },
                LootBoxTop: 0,
                LootBoxLeft: 0,
            };
            var moduleName = $scope.newModule.Info.ModuleType.Name.toLowerCase()
            $scope.newModule.ModuleTemplate = '/assets/js/hazmat/page-module-forms/' + moduleName + '.html';

            switch (moduleName) {
                case 'boxes':
                    $scope.newModule.Details.ModuleBoxesBox = [{ Color: 'green', Position: 1, IsModified: true }];
                    break;
                case 'simplecards':
                    $scope.newModule.Details.ModuleSimpleCardsCards = [{ Position: 1, IsModified: true }];
                    break;
                case 'profilecards':
                    $scope.newModule.Details.ModuleProfileCardsCards = [{ Position: 1, IsModified: true, ModuleProfileCardsCardLinks: [] }];
                    break
                case 'gallery':
                    $scope.newModule.Details.ModuleGalleryImages = [{ Position: 1, IsModified: true, ModuleGalleryImageLinks: [] }];
                    break;
            }
        }
    });

    function getModuleData(moduleName, module) {
        var data = {
            Id: module.Details.Id,
            State: module.Info.State,
            Title: module.Info.Title,
            BlueTitle: module.Info.BlueTitle,
            Theme: module.Info.Theme,
            TransparentBackground: module.Info.TransparentBackground,
            LootBox: module.Info.LootBox,
            LootBoxTop: module.Info.LootBoxTop,
            LootBoxLeft: module.Info.LootBoxLeft,
        };

        switch (moduleName) {
            case 'callout':
                data.GreenTitlePrefix = module.Details.GreenTitlePrefix;
                data.Subtitle = module.Details.Subtitle;
                data.ButtonText = module.Details.ButtonText;
                data.ButtonLink = module.Details.ButtonLink;
                break;
            case 'boxes':
                data.Text = module.Details.Text;
                data.Boxes = module.Details.ModuleBoxesBox;
                break;
            case 'simplecards':
                data.Cards = module.Details.ModuleSimpleCardsCards;
                break;
            case 'profilecards':
                data.Cards = module.Details.ModuleProfileCardsCards;
                break;
            case 'freetext':
                data.Text = module.Details.Text;
                break;
            case 'gallery':
                data.Images = module.Details.ModuleGalleryImages;
                break;
            case 'promo':
                data.Image = module.Details.Image;
                data.Text = module.Details.Text;
                data.ButtonText = module.Details.ButtonText;
                data.ButtonLink = module.Details.ButtonLink;
                data.ImageRight = module.Details.ImageRight;
                data.TitleTop = module.Details.TitleTop;
                break;
        }

        return data;
    }


    function setModuleData(moduleName, module, data) {
        module.Details.Id = data.Id;
        module.Info.Id = data.PageModuleId;

        if (data.Position) {
            module.Info.Position = data.Position;
        }

        switch (moduleName) {
            case 'boxes':
                module.Details.ModuleBoxesBox = data.Boxes;
                break
            case 'simplecards':
                module.Details.ModuleSimpleCardsCards = data.Cards;
                break;
            case 'profilecards':
                module.Details.ModuleProfileCardsCards = data.Cards;
                break;
            case 'gallery':
                module.Details.ModuleGalleryImages = data.Images;
                break;
        }
    }

    function initModule(moduleName, module) {
        switch (moduleName) {
            case 'boxes':
                module.Details.ModuleBoxesBox = $filter('orderBy')(module.Details.ModuleBoxesBox, 'Position');
                break;
            case 'simplecards':
                module.Details.ModuleSimpleCardsCards = $filter('orderBy')(module.Details.ModuleSimpleCardsCards, 'Position');
                break;
            case 'profilecards':
                module.Details.ModuleProfileCardsCards = $filter('orderBy')(module.Details.ModuleProfileCardsCards, 'Position');
                for (var i = 0; i < module.Details.ModuleProfileCardsCards.length; i++) {
                    module.Details.ModuleProfileCardsCards[i].ModuleProfileCardsCardLinks = $filter('orderBy')(module.Details.ModuleProfileCardsCards[i].ModuleProfileCardsCardLinks, 'Position');
                }
                break;
            case 'gallery':
                module.Details.ModuleGalleryImages = $filter('orderBy')(module.Details.ModuleGalleryImages, 'Position');
                for (var i = 0; i < module.Details.ModuleGalleryImages.length; i++) {
                    module.Details.ModuleGalleryImages[i].ModuleGalleryImageLinks = $filter('orderBy')(module.Details.ModuleGalleryImages[i].ModuleGalleryImageLinks, 'Position');
                }
                break;
        }
    }


    init();
}]);