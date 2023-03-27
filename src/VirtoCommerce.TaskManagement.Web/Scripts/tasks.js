// Call this to register your module to main application
var moduleName = 'VirtoCommerce.TaskManagement';

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .config(['$stateProvider',
        function ($stateProvider) {
            //$stateProvider
            //    .state('workspace.TaskManagementState', {
            //        url: '/TaskManagement',
            //        templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
            //        controller: [
            //            'platformWebApp.bladeNavigationService',
            //            function (bladeNavigationService) {
            //                var newBlade = {
            //                    id: 'blade1',
            //                    controller: 'TaskManagement.helloWorldController',
            //                    template: 'Modules/$(VirtoCommerce.TaskManagement)/Scripts/blades/hello-world.html',
            //                    isClosingDisabled: true,
            //                };
            //                bladeNavigationService.showBlade(newBlade);
            //            }
            //        ]
            //    });
        }
    ])
    .run(['platformWebApp.permissionScopeResolver',
        function (scopeResolver) {
            //Register module in main menu
            //var menuItem = {
            //    path: 'browse/TaskManagement',
            //    icon: 'fa fa-cube',
            //    title: 'TaskManagement',
            //    priority: 100,
            //    action: function () { $state.go('workspace.TaskManagementState'); },
            //    permission: 'TaskManagement:access',
            //};
            //mainMenuService.addMenuItem(menuItem);

            var taskAssignToAllScope = {
                type: 'TaskAssignToAllScope',
                title: 'Assign tasks to all'
            };
            scopeResolver.register(taskAssignToAllScope);

            var taskAssignToMyOrganizationScope = {
                type: 'TaskAssignToMyOrganizationScope',
                title: 'Assign tasks to all in my organization'
            };
            scopeResolver.register(taskAssignToMyOrganizationScope);
        }
    ]);
