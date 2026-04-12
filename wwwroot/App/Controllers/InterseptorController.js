app.factory('AuthInterceptor', ['$http', '$window', '$q', '$location',
    function ($http, $window, $q, $location) {
        var storage = $window.localStorage;
        var cachedToken = null;
        return {
            // Retrieve current access token
            getToken: function () {
                if (!cachedToken) {
                    cachedToken = storage.getItem('token');
                }
                return cachedToken;
            },

            // Request a new token using a stored refresh token
            refreshToken: function () {
                var refreshToken = storage.getItem('refreshToken');

                if (!refreshToken) {
                    return $q.reject("No refresh token available");
                }

                // Post to your refresh endpoint
                // skipAuthorization: true prevents interceptor loops if using a library like angular-jwt
                return $http.post('/api/auth/refresh', { refresh_token: refreshToken })
                    .then(function (response) {
                        var newToken = response.data.data.access_token;
                        storage.setItem('token', newToken);
                        cachedToken = newToken;
                        return newToken;
                    });
            },

            // Clean up storage on logout or failed refresh
            logout: function () {
                cachedToken = null;
                storage.removeItem('token');
                storage.removeItem('refreshToken');
                storage.removeItem('userInfo');
                // Redirect to login page or broadcast logout event
                $location.path("/login");
            },

            // Check if a token exists
            isAuthenticated: function () {
                return !!this.getToken();
            }
        };
}]);