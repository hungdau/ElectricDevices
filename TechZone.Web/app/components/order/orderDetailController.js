(function (app) {
    'use strict';

    app.controller('orderDetailController', orderDetailController);

    orderDetailController.$inject = ['$scope', 'apiService', 'notificationService', '$ngBootbox', '$stateParams', '$filter'];

    function orderDetailController($scope, apiService, notificationService, $ngBootbox, $stateParams, $filter) {
        $scope.loading = true;
        $scope.data = [];
        $scope.page = 0;
        $scope.pageCount = 0;
        $scope.search = search;
        $scope.clearSearch = clearSearch;
        $scope.deleteItem = deleteItem;

        function deleteItem(id) {
            $ngBootbox.confirm('Bạn có chắc muốn xóa?')
                .then(function () {
                    var config = {
                        params: {
                            id: id
                        }
                    }
                    apiService.del('/api/statistic/delete/orderdetail', config, function () {
                        notificationService.displaySuccess('Đã xóa thành công.');
                        search();
                    },
                        function () {
                            notificationService.displayError('Xóa không thành công.');
                        });
                });
        }

        $scope.deleteMultiple = deleteMultiple;

        function deleteMultiple() {
            $ngBootbox.confirm('Bạn có chắc muốn xóa ' + $scope.count + ' bản ghi này không?').then(function () {
                var listId = [];
                $.each($scope.selected, function (i, item) {
                    listId.push(item.ProductID);
                });
                var config = {
                    params: {
                        checkedList: JSON.stringify(listId)
                    }
                }
                apiService.del('api/statistic/deletemulti/orderdetail', config, function (result) {
                    notificationService.displaySuccess('Xóa thành công ' + result.data + ' bản ghi.');
                    search();
                }, function (error) {
                    notificationService.displayError('Xóa không thành công');
                });
            });
        }

        $scope.delivery = delivery;
        function delivery() {
            var status = true;
            $.each($scope.selected, function (i, item) {
                var data = $scope.data.find(function (elem) {
                    return elem.ProductID == item.ProductID;
                });
                if (!data.IsDelivery) {
                    data.IsDelivery = true;
                    apiService.put('api/statistic/update', data, function (result) {
                    }, function (error) {
                        status = false;
                    });
                }
            });
            if (status) {
                notificationService.displaySuccess('Giao hàng thành công');
                search();
            } else {
                console.log("Có lỗi xảy ra");
            }
        }

        $scope.selectAll = selectAll;
        $scope.isAll = false;
        $scope.isCheckAll = false;
        function selectAll() {
            if ($scope.isAll === false) {
                angular.forEach($scope.data, function (item) {
                    item.checked = true;
                });
                $scope.isAll = true;
                $scope.isCheckAll = true;
            } else {
                angular.forEach($scope.data, function (item) {
                    item.checked = false;
                });
                $scope.isAll = false;
                $scope.isCheckAll = false;
            }
        }

        $scope.$watch("data", function (n, o) {
            var checked = $filter("filter")(n, { checked: true });
            if (checked.length) {
                $scope.selected = checked;
                $('#btnDelete').removeAttr('disabled');
                $('#btnDelivery').removeAttr('disabled');
            } else {
                $('#btnDelete').attr('disabled', 'disabled');
                $('#btnDelivery').attr('disabled', 'disabled');
            }
        }, true);

        function search() {
            $scope.isCheckAll = false;
            $scope.loading = true;
            apiService.get('api/statistic/getorderdetail/' + $stateParams.id, null, dataLoadCompleted, dataLoadFailed);
        }

        function dataLoadCompleted(result) {
            $scope.data = result.data;
            $scope.loading = false;

            if ($scope.filterExpression && $scope.filterExpression.length) {
                notificationService.displayInfo(result.data.Items.length + ' items found');
            }
        }
        function dataLoadFailed(response) {
            notificationService.displayError(response.data);
        }

        function clearSearch() {
            $scope.filterExpression = '';
            search();
        }

        $scope.search();
    }
})(angular.module('techzone.order'));