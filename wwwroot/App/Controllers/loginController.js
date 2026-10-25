app.controller('LoginController', [
    '$scope', '$http', '$location', '$window', 'CONFIG',
    function ($scope, $http, $location, $window, CONFIG) {

        const path = "/api/Auth/login";
        const configPath = "/api/Configuration/configPrivate";

        //const keyString = CONFIG.VITE_keyFront;
        //const ivString = CONFIG.VITE_IVFront;

        $scope.message = "Welcome to Login Page";

        $scope.loadingStatus = false;

        async function encryptAes256Gcm(plainText, keyString) {
            const encoder = new TextEncoder();
            const keyData = encoder.encode(keyString);
            const plainBytes = encoder.encode(plainText);

            const keyBytesBuffer = await $window.crypto.subtle.digest('SHA-256', keyData);
            const cryptoKey = await $window.crypto.subtle.importKey(
                'raw',
                keyBytesBuffer,
                { name: 'AES-GCM' },
                false,
                ['encrypt']
            );

            const nonceSize = 12;
            const tagSize = 16;
            const nonce = $window.crypto.getRandomValues(new Uint8Array(nonceSize));

            const encryptedBuffer = await $window.crypto.subtle.encrypt(
                { name: 'AES-GCM', iv: nonce, tagLength: tagSize * 8 },
                cryptoKey,
                plainBytes
            );

            const encryptedBytes = new Uint8Array(encryptedBuffer);
            const cipherBytes = encryptedBytes.slice(0, encryptedBytes.length - tagSize);
            const tag = encryptedBytes.slice(encryptedBytes.length - tagSize);

            const totalLength = 4 + nonceSize + 4 + tagSize + cipherBytes.length;
            const result = new Uint8Array(totalLength);
            const view = new DataView(result.buffer);

            view.setInt32(0, nonceSize, true);
            result.set(nonce, 4);
            view.setInt32(4 + nonceSize, tagSize, true);
            result.set(tag, 4 + nonceSize + 4);
            result.set(cipherBytes, 4 + nonceSize + 4 + tagSize);

            let binary = '';
            for (let i = 0; i < result.byteLength; i++) {
                binary += String.fromCharCode(result[i]);
            }
            return $window.btoa(binary);
        }

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
            $scope.loadingStatus = true;

            $http
                .post(configPath, null)
                .then(async function (response) {
                    // Handle success
                    const keyString = response.data.s0VZ;
                    const encryptedPassword = await encryptAes256Gcm($scope.user.password, keyString);

                    let payload = {
                        Email: $scope.user.email,
                        Password: encryptedPassword
                    }

                    return $http
                        .post(path, payload)
                        .then(function (response) {
                            // Handle success
                            if (response.data.message === "login success") {
                                // Perform actions on successful login
                                $window.localStorage.setItem("token", response.data.data.access_token);
                                $window.localStorage.setItem("refreshToken", response.data.data.refresh_token);
                                $window.localStorage.setItem("userInfo", JSON.stringify(response.data.data.user_info));
                                $window.alert('Login Success');
                                $location.path("/"); // Redirect to dashboard or another page
                            } else {
                                $scope.errorMessage = response.data.message;
                            }
                            $scope.loadingStatus = false;
                        })
                        .catch(function (error) {
                            $scope.loadingStatus = false;
                            $scope.errorMessage = error.data?.message || "An error occurred during login.";
                        });
                })
                .catch(function (error) {
                    $scope.$applyAsync(function () {
                        $scope.loadingStatus = false;
                        $scope.errorMessage = error.data?.message || "An error occurred during login.";
                    });
                });
        };
    }
]);