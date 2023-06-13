(function (app) {
    app.controller('manufacturerListController', manufacturerListController);

    manufacturerListController.$inject = ['$scope', 'apiService', 'notificationService', '$ngBootbox', '$filter', '$stateParams'];

    function manufacturerListController($scope, apiService, notificationService, $ngBootbox, $filter, $stateParams) {
        $scope.manufacturer = [];
        $scope.page = 0;
        $scope.pagesCount = 0;
        $scope.getManufacturer = getManufacturer;
        $scope.productCategoryId = $stateParams.id;
        $scope.keyword = '';
        $scope.loading = true;

        $scope.search = search;

        $scope.deleteManufacturer = deleteManufacturer;

        $scope.selectAll = selectAll;

        $scope.deleteMultiple = deleteMultiple;

        function deleteMultiple() {
            var listId = [];
            $.each($scope.selected, function (i, item) {
                listId.push(item.ID);
            });
            var config = {
                params: {
                    checkedProductCategories: JSON.stringify(listId)
                }
            }
            apiService.del('api/productcategory/deletemulti', config, function (result) {
                notificationService.displaySuccess('Xóa thành công ' + result.data + ' bản ghi.');
                search();
            }, function (error) {
                notificationService.displayError('Xóa không thành công');
            });
        }

        $scope.isAll = false;
        function selectAll() {
            if ($scope.isAll === false) {
                angular.forEach($scope.manufacturer, function (item) {
                    item.checked = true;
                });
                $scope.isAll = true;
            } else {
                angular.forEach($scope.manufacturer, function (item) {
                    item.checked = false;
                });
                $scope.isAll = false;
            }
        }

        $scope.$watch("manufacturer", function (n, o) {
            var checked = $filter("filter")(n, { checked: true });
            if (checked.length) {
                $scope.selected = checked;
                $('#btnDelete').removeAttr('disabled');
            } else {
                $('#btnDelete').attr('disabled', 'disabled');
            }
        }, true);

        function deleteManufacturer(id) {
            $ngBootbox.confirm('Bạn có chắc muốn xóa?').then(function () {
                var config = {
                    params: {
                        id: id
                    }
                }
                apiService.del('api/productcategory/delete', config, function () {
                    notificationService.displaySuccess('Xóa thành công');
                    search();
                }, function () {
                    notificationService.displayError('Xóa không thành công');
                })
            });
        }

        function search() {
            getManufacturer();
        }

        function getManufacturer(page) {
            page = page || 0;
            var config = {
                params: {
                    keyword: $scope.keyword,
                    page: page,
                    pageSize: 20
                }
            }
            $scope.loading = true;
            apiService.get('/api/productcategory/getallManufacturer/' + $stateParams.id, config, function (result) {
                if (result.data.TotalCount == 0) {
                    notificationService.displayWarning('Không có bản ghi nào được tìm thấy.');
                }
                $scope.manufacturer = result.data.Items;

                $scope.page = result.data.Page;
                $scope.pagesCount = result.data.TotalPages;
                $scope.totalCount = result.data.TotalCount;
                $scope.loading = false;
            }, function () {
                $scope.loading = false;
            });
        }
        function getProductCategory() {
            apiService.get('/api/productcategory/getbyid/' + $stateParams.id, null, function (result) {
                $scope.productCategory = result.data.Name;
            }, function () { });
        }
        getProductCategory();
        $scope.getManufacturer();
    }
})(angular.module('techzone.manufacturer'));