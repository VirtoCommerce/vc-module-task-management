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
            var taskAssignToAllScope = {
                type: 'TaskAssignToMeScope',
                title: 'Assign tasks only to me'
            };
            scopeResolver.register(taskAssignToAllScope);

            var taskAssignToMyOrganizationScope = {
                type: 'TaskAssignToMyOrganizationScope',
                title: 'Assign tasks to all in my organization'
            };
            scopeResolver.register(taskAssignToMyOrganizationScope);
        }
    ]);
