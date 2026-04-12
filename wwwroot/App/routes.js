app.config(function ($routeProvider, $locationProvider, $httpProvider) {

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
});

app.run(function ($rootScope, $location, $window) {

    $rootScope.$on("$routeChangeStart", function (event, next, current) {

        var user = $window.sessionStorage.getItem("user");

        if (!user && (next.templateUrl !== "app/views/login.html" && next.templateUrl !== "app/views/register.html")) {
            $location.path("/login");
        }
    });

});