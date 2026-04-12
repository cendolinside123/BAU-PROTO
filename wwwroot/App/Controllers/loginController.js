app.controller('LoginController', [
    '$scope', '$http', '$location', '$window',
    function ($scope, $http, $location, $window) {

        const path = "/api/Auth/login";

        const keyString = "YmFobGlsZ29ibG9r";
        const ivString = "cGlnYWltb255ZXQ=";

        $scope.message = "Welcome to Login Page";


        $scope.login = function () {
            $scope.errorMessage = "";
            if (!$scope.user) {
                $scope.errorMessage = "Please enter username and password.";
                return;
            }

            if ($scope.user.email == null || $scope.user.password == null) {
                $scope.errorMessage = "Please enter email or password.";
                return;
            }

            const key = CryptoJS.enc.Utf8.parse(keyString);
            const iv = CryptoJS.enc.Utf8.parse(ivString);
            const encrypted = CryptoJS.AES.encrypt($scope.user.password, key, {
                iv: iv,
                mode: CryptoJS.mode.CBC,
                padding: CryptoJS.pad.Pkcs7
            });

            let payload = {
                Email: $scope.user.email,
                Password: encrypted.toString()
            }


            $http
                .post(path, payload)
                .then(function (response) {
                    // Handle success
                    if (response.data.message === "login success") {
                        // Perform actions on successful login
                        $window.localStorage.setItem("token", response.data.data.access_token);
                        $window.localStorage.setItem("refreshToken", response.data.data.refresh_token);
                        $window.localStorage.setItem("userInfo", JSON.stringify(response.data.data.user_info));
                        $location.path("/"); // Redirect to dashboard or another page
                    } else {
                        $scope.errorMessage = response.data.message;
                    }
                })
                .catch(function (error) {
                    $scope.errorMessage = error.data.message || "An error occurred during login.";
                });
        };
    }
]);