app.controller('LogoutController', [
    '$scope', '$http', '$location', '$window',
    function ($scope, $http, $location, $window) {

        var storage = $window.localStorage;

        $scope.logout = function () {

            var refreshToken = storage.getItem('refreshToken');
            var token = storage.getItem('token');

            if (refreshToken && token) {

                var headres = {
                    Refreshtoken: refreshToken,
                    Authorization: "Bearer " + token
                }

                $http.post('/api/Auth/logout', null, { headers: headres })
                    .then(function (response) {
                        storage.removeItem('token');
                        storage.removeItem('refreshToken');
                        storage.removeItem('userInfo');
                        $location.path("/login");
                    })
                    .catch(function () {
                        storage.removeItem('token');
                        storage.removeItem('refreshToken');
                        storage.removeItem('userInfo');
                        $location.path("/login");
                    });
            } else {
                storage.removeItem('token');
                storage.removeItem('refreshToken');
                storage.removeItem('userInfo');
                $location.path("/login");
            }
        };
    }
]);