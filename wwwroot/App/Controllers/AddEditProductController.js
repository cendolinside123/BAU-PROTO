
app.controller('AddEditProductController', [
    '$scope', '$http', '$location', '$window', 'CONFIG', '$rootScope',
    function ($scope, $http, $location, $window, CONFIG, $rootScope) {

        let storage = $window.localStorage;

        const path = "/api/Product/AddProduct";
        const updateProductPath = "/api/Product/UpdateProduct";

        $scope.product = {
            id: null,
            nama: null,
            desc: null,
            harga: null
        };
        $scope.loadingStatus = false;

        $scope.$on('editProductValue', function (events, args) {
            console.log(args);
            $scope.$applyAsync(function () {
                $scope.product = angular.copy(args);
            });
        })

        $scope.addEditProduct = function () {

            let token = storage.getItem('token');

            $scope.errorMessage = "";
            if (!$scope.product) {
                $scope.errorMessage = "Please enter product details.";
                return;
            }

            if ($scope.product.nama == null || $scope.product.desc == null || $scope.product.harga == null) {
                $scope.errorMessage = "Please complete input product details.";
                return;
            }

            let headres = {
                Authorization: "Bearer " + token
            }

            let payload = {
                Name: $scope.product.nama,
                Description: $scope.product.desc,
                Price: $scope.product.harga.toString()
            }

            let apiPath = path;

            if ($scope.product.id != null) {
                payload.Id = $scope.product.id.toString();
                apiPath = updateProductPath;
            }
            $scope.loadingStatus = true;
            $http.post(apiPath, payload, { headers: headres })
                .then(function (response) {
                    let getMessage = response.data.message;
                    $scope.loadingStatus = false;
                    if (getMessage.toLowerCase().includes("success")) {
                        
                        if ($scope.product.id != null) {
                            $window.alert('Update product Success');
                        } else {
                            $window.alert('Add product Success');   
                        }
                        $rootScope.$broadcast('reloadList');
                        $scope.product = {
                            id: null,
                            nama: null,
                            desc: null,
                            harga: null
                        };
                    } else {
                        $scope.errorMessage = getMessage;
                    }
                })
                .catch(function (error) {
                    $scope.loadingStatus = false;
                    $scope.errorMessage = error.data.message || "An error occurred during login.";
                });

        }
    }
]);
