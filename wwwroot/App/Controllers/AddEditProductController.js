app.service('sharedData', function () {
    this.selectedProduct = {
        id: null,
        nama: null,
        desc: null,
        harga: null
    };
});


app.controller('AddEditProductController', [
    '$scope', '$http', '$location', '$window', 'CONFIG', 'sharedData',
    function ($scope, $http, $location, $window, CONFIG, sharedData) {

        let storage = $window.localStorage;

        const path = "/api/Product/AddProduct";
        const updateProductPath = "/api/Product/UpdateProduct";

        $scope.product = sharedData.selectedProduct;

        $scope.addEditProduct = function () {

            let refreshToken = storage.getItem('refreshToken');
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
                Refreshtoken: refreshToken,
                Authorization: "Bearer " + token
            }

            let payload = {
                Name: $scope.product.nama,
                Description: $scope.product.desc,
                Price: $scope.product.harga
            }

            let apiPath = path;

            if ($scope.product.id != null) {
                payload.Id = $scope.product.id.toString();
                apiPath = updateProductPath;
            }

            $http.post(apiPath, payload, { headers: headres })
                .then(function (response) {
                    if (response.data.message.toLowerCase().includes("success")) {
                        $scope.product = null;

                        if ($scope.product.id != null) {
                            $window.alert('Update product Success');
                        } else {
                            $window.alert('Add product Success');   
                        }

                    } else {
                        $scope.errorMessage = response.data.message;
                    }
                })
                .catch(function (error) {
                    $scope.errorMessage = error.data.message || "An error occurred during login.";
                });

        }
    }
]);