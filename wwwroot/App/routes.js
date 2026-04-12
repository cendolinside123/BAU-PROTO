app.config(['$routeProvider', '$locationProvider', '$httpProvider',
    function ($routeProvider, $locationProvider, $httpProvider) {

        $routeProvider
            .when("/", {
                templateUrl: "app/views/home.html",
            })
            .when("/about", {
                templateUrl: "app/views/about.html",
            })
            .when("/login", {
                templateUrl: "app/views/login.html",
            })
            .when("/register", {
                templateUrl: "app/views/register.html",
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

            if (!user && (next.templateUrl !== "app/views/login.html" && next.templateUrl !== "app/views/register.html")) {
                $location.path("/login");
            }
        });

    }
]);