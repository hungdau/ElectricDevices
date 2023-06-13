(function (app) {
    app.controller('productImportController', productImportController);

    productImportController.$inject = ['apiService', '$http', 'authenticationService', '$scope', 'notificationService', '$state', 'commonService'];

    function productImportController(apiService, $http, authenticationService, $scope, notificationService, $state, commonService) {
        $scope.files = [];
        $scope.product = {};
        $scope.ImportProduct = ImportProduct;
        $scope.loadManufacturer = loadManufacturer;

        $scope.$on("fileSelected", function (event, args) {
            $scope.$apply(function () {
                $scope.files.push(args.file);
            });
        });

        function ImportProduct() {
            authenticationService.setHeader();
            $http({
                method: 'POST',
                url: "/api/product/import",
                headers: { 'Content-Type': undefined },
                transformRequest: function (data) {
                    var formData = new FormData();
                    formData.append("categoryId", angular.toJson(data.categoryId));
                    for (var i = 0; i < data.files.length; i++) {
                        formData.append('file' + i, data.files[i]);
                    }
                    return formData;
                },
                data: { categoryId: $scope.product.CategoryID, files: $scope.files }
            }).then(function (result, status, headers, config) {
                notificationService.displaySuccess(result.data);
                $state.go('products');
            },
                function (data, status, headers, config) {
                    notificationService.displayError(data);
                });
        }

        function loadProductCategory() {
            var config = {
                params: {
                    keyword: '',
                    page: 0,
                    pageSize: 100
                }
            }
            apiService.get('api/productcategory/getallProductCategory', config, function (result) {
                $scope.productCategories = result.data.Items;
            }, function () { });
        }

        function loadManufacturer() {
            $scope.product.CategoryID = null;
            var config = {
                params: {
                    keyword: '',
                    page: 0,
                    pageSize: 100
                }
            }
            apiService.get('api/productcategory/getallManufacturer/' + $scope.product.ProductCategoryID, config, function (result) {
                $scope.productManufacturer = result.data.Items;
            }, function () { });
        }

        loadProductCategory();
    }
})(angular.module('techzone.products'));