app.controller('HomeController', [
    '$scope', '$http', '$location', '$window', 'CONFIG',
    function ($scope, $http, $location, $window, CONFIG) {
        $scope.message = "Welcome to Home Page";
    }
]);