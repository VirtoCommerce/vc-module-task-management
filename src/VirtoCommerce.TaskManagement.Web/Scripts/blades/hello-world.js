angular.module('TaskManagement')
    .controller('TaskManagement.helloWorldController', ['$scope', 'TaskManagement.webApi', function ($scope, api) {
        var blade = $scope.blade;
        blade.title = 'TaskManagement';

        blade.refresh = function () {
            api.get(function (data) {
                blade.title = 'TaskManagement.blades.hello-world.title';
                blade.data = data.result;
                blade.isLoading = false;
            });
        };

        blade.refresh();
    }]);
