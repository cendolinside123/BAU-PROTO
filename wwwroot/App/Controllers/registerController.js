app.controller('RegisterController', [
    '$scope', '$http', '$location', '$window', 'CONFIG',
    function ($scope, $http, $location, $window, CONFIG) {

        const path = "/api/Auth/register";
        const configPath = "/api/Configuration/configPrivate";

        //const keyString = CONFIG.VITE_keyFront;
        //const ivString = CONFIG.VITE_IVFront;

        $scope.loadingStatus = false;

        $scope.message = "Welcome to Register Page";

        $scope.register = function () {
            $scope.errorMessage = "";
            if (!$scope.user) {
                $scope.errorMessage = "Please complete form registration.";
                return;
            }

            if ($scope.user.email == null || $scope.user.password == null || $scope.user.role == null) {
                $scope.errorMessage = "Please enter email, password, and role.";
                return;
            }
            $scope.loadingStatus = true;

            $http
                .post(configPath, null)
                .then(function (response) {
                    // Handle success
                    const keyString = response.data.s0VZ;
                    const ivString = response.data.svy;
                    const key = CryptoJS.enc.Utf8.parse(keyString);
                    const iv = CryptoJS.enc.Utf8.parse(ivString);
                    const encrypted = CryptoJS.AES.encrypt($scope.user.password, key, {
                        iv: iv,
                        mode: CryptoJS.mode.CBC,
                        padding: CryptoJS.pad.Pkcs7
                    });

                    let payload = {
                        Email: $scope.user.email,
                        Password: encrypted.toString(),
                        Role: $scope.user.role
                    }

                    $http
                        .post(path, payload)
                        .then(function (response) {
                            if (response.data.message === "register success") {
                                $window.alert('Register Success');
                                $location.path("/login"); // Redirect to dashboard or another page
                            } else {
                                $scope.errorMessage = response.data.message;
                            }
                            $scope.loadingStatus = false;
                        })
                        .catch(function (error) {
                            $scope.loadingStatus = false;
                            $scope.errorMessage = error.data.message || "An error occurred during login.";
                        });
                })
                .catch(function (error) {
                    $scope.errorMessage = error.data.message || "An error occurred during login.";
                    $scope.loadingStatus = false;
                });
        };

    }
]);