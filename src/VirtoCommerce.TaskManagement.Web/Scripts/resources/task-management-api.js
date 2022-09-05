angular.module('TaskManagement')
    .factory('TaskManagement.webApi', ['$resource', function ($resource) {
        return $resource('api/task-management');
    }]);
