app.controller('ListProductController', [
    '$scope', '$http', '$location', '$window', 'CONFIG', '$rootScope',
    function ($scope, $http, $location, $window, CONFIG, $rootScope) {
        $scope.products = [];
        $scope.loadingStatus = false;
        $scope.page = 1;
        $scope.lastPage = false
        $scope.namaProduct = null;

        let storage = $window.localStorage;

        const path = "/api/Product/GetListProduct";

        $scope.searchPage = function () {
            $scope.page = 1;
            $scope.lastPage = false;
            $scope.getProducts();
        }

        $scope.getProducts = function () {

            let refreshToken = storage.getItem('refreshToken');
            let token = storage.getItem('token');

            let headres = {
                Refreshtoken: refreshToken,
                Authorization: "Bearer " + token
            }

            let payload = {
                page : $scope.page.toString(),
                pageSize : 10
            }

            if ($scope.namaProduct != null && $scope.namaProduct.toString() != "") {
                payload.Name = $scope.namaProduct.toString()
            }

            $scope.loadingStatus = true;
            $http.post(path, payload, { headers: headres })
                .then(function (response) {
                    let getMessage = response.data.message;
                    $scope.loadingStatus = false;
                    if (getMessage.toLowerCase().includes("success")) {
                        $scope.products = response.data.data;

                        if ($scope.products.length > 0) {
                            if ($scope.products.length < 10) {
                                $scope.lastPage = true;
                            } else {
                                $scope.lastPage = false;
                            }
                        } else {
                            $scope.lastPage = true;
                        }
                    } else {
                        $location.alert(response.data.message);
                    }
                    $scope.loadingStatus = false;
                })
                .catch(function (error) {
                    $location.alert(error.data.message);
                });
            
        }

        $scope.prevPage = function () {
            $scope.page = $scope.page - 1;
            console.log($scope.page);
            $scope.getProducts();
        }

        $scope.nextPage = function () {
            $scope.page = $scope.page + 1;
            console.log($scope.page);
            $scope.getProducts();
        }

        $scope.getSelectedItem = function (item) {
            //sharedData.setValue(
            //    {
            //        id: 1,
            //        nama: "asd",
            //        desc: "asd",
            //        harga: 500
            //    }
            //);
            $rootScope.$broadcast('editProductValue', item);
        }

        $scope.deleteSelectedItem = function (itemID) {
            $scope.page = 1;
            $scope.lastPage = false;
            $scope.getProducts();
        }


        $scope.getProducts();
    }
])