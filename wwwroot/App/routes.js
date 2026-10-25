app.config(['$routeProvider', '$locationProvider', '$httpProvider',
    function ($routeProvider, $locationProvider, $httpProvider) {
        const cacheBust = "?v=" + new Date().getTime();

        $routeProvider
            .when("/", {
                templateUrl: "app/views/home.html" + cacheBust,
            })
            .when("/login", {
                templateUrl: "app/views/login.html" + cacheBust,
            })
            .when("/register", {
                templateUrl: "app/views/register.html" + cacheBust,
            })
            .otherwise({
                redirectTo: "/"
            });

        $locationProvider.html5Mode(true);
        $locationProvider.hashPrefix("!");
        $httpProvider.interceptors.push('AuthInterceptor');
    }
]);

app.run(['$rootScope', '$location', '$window',
    function ($rootScope, $location, $window) {

        $rootScope.location = $location;

        $rootScope.$on("$routeChangeStart", function (event, next, current) {
            let user = $window.localStorage.getItem("userInfo");
            let refreshToken = $window.localStorage.getItem("refreshToken");
            let token = $window.localStorage.getItem("token");

            if (!user) {
                if (!next.templateUrl || (next.templateUrl.indexOf("app/views/login.html") === -1 && next.templateUrl.indexOf("app/views/register.html") === -1)) {
                    $location.path("/login");
                }
            } else {
                if (next.templateUrl && (next.templateUrl.indexOf("app/views/login.html") !== -1 || next.templateUrl.indexOf("app/views/register.html") !== -1)) {
                    $location.path("/");
                }
            }
        });

    }
]);
