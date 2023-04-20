// Call this to register your module to main application
var moduleName = 'VirtoCommerce.TaskManagement';

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .config(['$stateProvider',
        function ($stateProvider) {

        }
    ])
    .run(['platformWebApp.permissionScopeResolver',
        function (scopeResolver) {
            var taskToAllScope = {
                type: 'TaskToMeScope',
                title: 'Tasks only to me'
            };
            scopeResolver.register(taskToAllScope);

            var taskToMyOrganizationScope = {
                type: 'TaskToMyOrganizationScope',
                title: 'Tasks to all in my organization'
            };
            scopeResolver.register(taskToMyOrganizationScope);
        }
    ]);
