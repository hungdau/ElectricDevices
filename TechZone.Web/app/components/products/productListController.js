(function (app) {
    app.controller('productListController', productListController);

    productListController.$inject = ['$scope', 'apiService', 'notificationService', '$ngBootbox', '$filter', '$stateParams'];

    function productListController($scope, apiService, notificationService, $ngBootbox, $filter, $stateParams) {
        $scope.products = [];
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.getProducts = getProducts;
        $scope.keyword = '';
        $scope.loading = true;
        $scope.count = 0;
        $scope.search = search;
        $scope.deleteProduct = deleteProduct;
        $scope.selectAll = selectAll;
        $scope.deleteMultiple = deleteMultiple;
        $scope.exportExcel = exportExcel;
        $scope.exportPdf = exportPdf;

        function exportExcel() {
            var config = {
                params: {
                    filter: $scope.keyword
                }
            }
            apiService.get('/api/product/exportXls', config, function (response) {
                if (response.status = 200) {
                    window.location.href = response.data.Message;
                }
            }, function (error) {
                notificationService.displayError(error);

            });
        }

        function exportPdf(productId) {
            var config = {
                params: {
                    id: productId
                }
            }
            apiService.get('/api/product/exportPdf', config, function (response) {
                if (response.status = 200) {
                    window.location.href = response.data.Message;
                }
            }, function (error) {
                notificationService.displayError(error);

            });
        }

        function deleteMultiple() {
            $ngBootbox.confirm('Bạn có chắc muốn xóa ' + $scope.count + ' bản ghi này không?').then(function () {
                var listId = [];
                $.each($scope.selected, function (i, item) {
                    /*listId.push(item.Product.ID);*/
                    listId.push(item.ID);
                });
                var config = {
                    params: {
                        checkedProducts: JSON.stringify(listId)
                    }
                }
                apiService.del('api/product/deletemulti', config, function (result) {
                    notificationService.displaySuccess('Xóa thành công ' + $scope.count + ' bản ghi.');
                    search();
                }, function (error) {
                    notificationService.displayError('Xóa không thành công');
                });
            });
        }

        $scope.isAll = false;
        function selectAll() {
            if ($scope.isAll === false) {
                angular.forEach($scope.products, function (item) {
                    item.checked = true;
                });
                $scope.isAll = true;
            } else {
                angular.forEach($scope.products, function (item) {
                    item.checked = false;
                });
                $scope.isAll = false;
            }
        }

        $scope.$watch("products", function (n, o) {
            var checked = $filter("filter")(n, { checked: true });
            $scope.count = checked.length;
            if (checked.length) {
                $scope.selected = checked;
                $('#btnDelete').removeAttr('disabled');
            } else {
                $('#btnDelete').attr('disabled', 'disabled');
            }
        }, true);

        function deleteProduct(id) {
            $ngBootbox.confirm('Bạn có chắc muốn xóa?').then(function () {
                var config = {
                    params: {
                        id: id
                    }
                }
                apiService.del('api/product/delete', config, function () {
                    notificationService.displaySuccess('Xóa thành công');
                    search();
                }, function () {
                    notificationService.displayError('Xóa không thành công');
                })
            });
        }

        function search() {
            getProducts();
        }

        function getProducts(page) {
            page = page || 0;
            var config = {
                params: {
                    keyword: $scope.keyword,
                    categoryId: $stateParams.categoryId || 0,
                    page: page,
                    pageSize: 20
                }
            }
            $scope.loading = true;
            apiService.get('/api/product/getall', config, function (result) {
                $scope.products = result.data.Items;
                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
            }, function () {
                console.log('Load product failed.');
                $scope.loading = false;
            });
        }

        $scope.getProducts();
    }
})(angular.module('techzone.products'));