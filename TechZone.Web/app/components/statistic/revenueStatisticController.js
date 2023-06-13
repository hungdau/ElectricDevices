(function (app) {
    app.controller('revenueStatisticController', revenueStatisticController);

    revenueStatisticController.$inject = ['$scope', 'apiService', 'notificationService', '$filter'];

    function revenueStatisticController($scope, apiService, notificationService, $filter) {
        $scope.tabledata = [];
        $scope.labels = [];
        $scope.series = ['Doanh số', 'Lợi nhuận'];
        $scope.chartdata = [];

        $scope.inputDate = true;
        $scope.inputMonth = false;
        $scope.name = 'Ngày';

        $scope.date = {
            startDate: new Date(new Date().getFullYear(), new Date().getMonth(), 1),
            endDate: new Date()
        };
        $scope.month = {
            startMonth: new Date(new Date().getFullYear(), 0),
            endMonth: new Date()
        };

        $scope.revenue = function () {
            if ($scope.inputDate) {
                getStatisticByDate();
            }
            else {
                getStatisticByMonth();
            }
        }

        $scope.getDailyStats = function () {
            $scope.name = 'Ngày';
            $scope.inputMonth = false;
            $scope.inputDate = true;
            getStatisticByDate();
        }

        //Method to get monthly stats data
        $scope.getMonthlyStats = function () {
            $scope.name = 'Tháng';
            $scope.inputDate = false;
            $scope.inputMonth = true;
            getStatisticByMonth();
        }

        function getStatisticByDate() {
            var config = {
                param: {
                    fromDate: formatDate($scope.date.startDate),
                    toDate: formatDate($scope.date.endDate)
                }
            }
            apiService.get('api/statistic/getrevenuebydate?fromDate=' + config.param.fromDate + "&toDate=" + config.param.toDate,
                null, dataLoadCompleted, dataLoadFailed);
        }
        function getStatisticByMonth() {
            var config = {
                param: {
                    fromMonth: formatDate($scope.month.startMonth),
                    toMonth: formatDate($scope.month.endMonth)
                }
            }
            apiService.get('api/statistic/getrevenuebymonth?fromMonth=' + config.param.fromMonth + "&toMonth=" + config.param.toMonth,
                null, dataLoadCompleted, dataLoadFailed);
        }

        function dataLoadCompleted(response) {
            $scope.tabledata = response.data;
            var labels = [];
            var chartData = [];
            var revenues = [];
            var benefits = [];
            $.each(response.data, function (i, item) {
                if ($scope.inputDate) {
                    labels.push($filter('date')(item.Date, 'dd/MM/yyyy'));
                }
                else {
                    labels.push(item.Month);
                }
                revenues.push(item.Revenues);
                benefits.push(item.Benefit);
            });
            chartData.push(revenues);
            chartData.push(benefits);

            $scope.chartdata = chartData;
            $scope.labels = labels;
        }

        function dataLoadFailed(response) {
            notificationService.displayError('Không thể tải dữ liệu');
        }

        getStatisticByDate();

        function formatDate(dateString) {
            var dateObj = new Date(dateString);
            var day = dateObj.getDate();
            var month = dateObj.getMonth() + 1;
            var year = dateObj.getFullYear();
            var formattedDate = (month < 10 ? '0' + month : month) + '/' +
                (day < 10 ? '0' + day : day) + '/' + year;
            return formattedDate;
        }
    }
})(angular.module('techzone.statistics'));