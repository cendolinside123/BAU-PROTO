app.controller('HomeController', [
    '$scope', '$http', '$location', '$window',
    function ($scope, $http, $location, $window) {
        $scope.message = "Welcome to Home Page";
    }
]);