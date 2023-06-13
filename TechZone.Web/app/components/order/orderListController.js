(function (app) {
    'use strict';

    app.controller('orderListController', orderListController);

    orderListController.$inject = ['$scope', 'apiService', 'notificationService', '$ngBootbox','$filter'];

    function orderListController($scope, apiService, notificationService, $ngBootbox, $filter) {
        $scope.loading = true;
        $scope.data = [];
        $scope.page = 0;
        $scope.pageCount = 0;
        $scope.search = search;
        $scope.clearSearch = clearSearch;
        $scope.deleteItem = deleteItem;
        $scope.exportPdf = exportPdf;

        function exportPdf(orderId) {
            var config = {
                params: {
                    id: orderId
                }
            }
            apiService.get('/api/statistic/exportPdf', config, function (response) {
                if (response.status = 200) {
                    window.location.href = response.data.Message;
                }
            }, function (error) {
                notificationService.displayError(error);

            });
        }
        function deleteItem(id) {
            $ngBootbox.confirm('Bạn có chắc muốn xóa không?')
                .then(function () {
                    var config = {
                        params: {
                            id: id
                        }
                    }
                    apiService.del('/api/statistic/delete/order', config, function () {
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
            //var listId = [];
            //$.each($scope.selected, function (i, item) {
            //    listId.push(item.ID);
            //});
            //var config = {
            //    params: {
            //        checkedList: JSON.stringify(listId)
            var listId = [];
            $.each($scope.selected, function (i, item) {
                listId.push(item.ID);
            });
            var config = {
                params: {
                    checkedList: JSON.stringify(listId)
                }
             }
                apiService.del('api/statistic/deletemulti/order', config, function (result) {
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
                    return elem.ID == item.ID;
                });
                if (!data.Status) {
                    apiService.put('api/statistic/update', data, function (result) {
                        search();
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
        function selectAll() {
            if ($scope.isAll === false) {
                angular.forEach($scope.data, function (item) {
                    item.checked = true;
                });
                $scope.isAll = true;
            } else {
                angular.forEach($scope.data, function (item) {
                    item.checked = false;
                });
                $scope.isAll = false;
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

        function search(page) {
            page = page || 0;

            $scope.loading = true;
            var config = {
                params: {
                    keyword: $scope.filterExpression||'',
                    page: page,
                    pageSize: 10
                }
            }

            apiService.get('api/statistic/getorder', config, dataLoadCompleted, dataLoadFailed);
        }

        function dataLoadCompleted(result) {
            $scope.data = result.data.Items;
            $scope.page = result.data.Page;
            $scope.pagesCount = result.data.TotalPages;
            $scope.totalCount = result.data.TotalCount;
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