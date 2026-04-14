app.factory('AuthInterceptor', ['$window', '$q', '$location', '$injector', 'CONFIG',
    function ($window, $q, $location, $injector, CONFIG) {
        var storage = $window.localStorage;
        var cachedToken = null;

        return {
            request: function (config) {
                // SKIP if URL contains /api/Auth/
                if (config.url.indexOf('/api/Auth/') !== -1) {
                    return config;
                }

                // Skip if only page URL
                if (config.url.indexOf('app/views/') !== -1) {
                    return config;
                }

                var token = storage.getItem('token');
                if (token) {
                    config.headers['Authorization'] = 'Bearer ' + token;
                }
                return config;
            },

            responseError: function (rejection) {
                // SKIP if URL contains /api/Auth/ OR is not a 401
                if (rejection.status !== 401 || rejection.config.url.indexOf('/api/Auth/') !== -1) {
                    return $q.reject(rejection);
                }

                var $http = $injector.get('$http');
                var refreshToken = storage.getItem('refreshToken');
                var token = storage.getItem('token');

                if (!refreshToken) {
                    this.logout();
                    return $q.reject(rejection);
                }

                if (!token) {
                    this.logout();
                    return $q.reject(rejection);
                }

                // Attempt to refresh
                var headres = {
                    Refreshtoken: refreshToken,
                    Authorization: token
                }

                return $http.post('/api/Auth/refresh', null, { headers: headres })
                    .then(function (response) {
                        var newToken = response.data.data.access_token;
                        storage.setItem('token', newToken);

                        // Retry original request with new token
                        rejection.config.headers['Authorization'] = 'Bearer ' + newToken;
                        return $http(rejection.config);
                    })
                    .catch(function () {
                        // If refresh fails, log out
                        cachedToken = null;
                        storage.removeItem('token');
                        storage.removeItem('refreshToken');
                        storage.removeItem('userInfo');
                        $location.path("/login");
                        return $q.reject(rejection);
                    });
            },

            logout: function () {

                var $http = $injector.get('$http');
                var refreshToken = storage.getItem('refreshToken');
                var token = storage.getItem('token');

                if (refreshToken && token) {

                    var headres = {
                        Refreshtoken: refreshToken,
                        Authorization: "Bearer " +token
                    }

                    $http.post('/api/Auth/logout', null, { headers: headres })
                        .then(function (response) {
                            cachedToken = null;
                            storage.removeItem('token');
                            storage.removeItem('refreshToken');
                            storage.removeItem('userInfo');
                            $location.path("/login");
                        })
                        .catch(function () {
                            cachedToken = null;
                            storage.removeItem('token');
                            storage.removeItem('refreshToken');
                            storage.removeItem('userInfo');
                            $location.path("/login");
                        });
                }
            }
        };
    }
]);