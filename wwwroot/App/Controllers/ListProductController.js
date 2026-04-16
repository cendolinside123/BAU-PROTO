app.controller('ListProductController', [
    '$scope', '$http', '$location', '$window', 'CONFIG', '$rootScope', '$mdDialog',
    function ($scope, $http, $location, $window, CONFIG, $rootScope, $mdDialog) {
        $scope.products = [];
        $scope.loadingStatus = false;
        $scope.deleteLoadingStatus = false;
        $scope.page = 1;
        $scope.lastPage = false
        $scope.namaProduct = null;

        let storage = $window.localStorage;

        const path = "/api/Product/GetListProduct";
        const pathDelete = "/api/Product/DeleteProduct";

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
                        $window.alert(response.data.message);
                    }
                    
                })
                .catch(function (error) {
                    $scope.loadingStatus = false;
                    $window.alert(error.data.message);
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
            $rootScope.$broadcast('editProductValue', item);
        }

        $scope.deleteSelectedItem = function (itemID) {
            var ctrl = this;
            if ($window.confirm("Are you sure you want to delete this?")) {
                let refreshToken = storage.getItem('refreshToken');
                let token = storage.getItem('token');

                let headres = {
                    Refreshtoken: refreshToken,
                    Authorization: "Bearer " + token
                }


                let payload = {
                    Id: itemID.toString()
                }

                $scope.loadingDialog(ctrl)

                $http.post(pathDelete, payload, { headers: headres })
                    .then(function (response) {
                        let getMessage = response.data.message;
                        $mdDialog.hide();
                        if (getMessage.toLowerCase().includes("success")) {
                            $scope.products = response.data.data;
                            $window.alert("delete berhasil");
                            $scope.searchPage();
                            
                        } else {
                            $window.alert(response.data.message);
                        }
                    })
                    .catch(function (error) {
                        $mdDialog.hide();
                        $window.alert(error.data.message);
                    });

            } else {
                // Logic for "Cancel" clicked
            }

        }

        $scope.loadingDialog = function (ev) {
            $mdDialog.show({
                contentElement: '#myDialog',
                template: '<md-dialog>' +
                    '  <md-dialog-content><h1>Loading...</h1></md-dialog-content>' +
                    '</md-dialog>',
                parent: angular.element(document.body),
                targetEvent: ev,
            });
        };


        $scope.getProducts();
    }
])