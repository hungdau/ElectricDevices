(function (app) {
    app.filter('statusFilter', function () {
        return function (input) {
            if (input == true)
                return 'Kích hoạt';
            else
                return 'Khóa';
        }
    });
    app.filter('orderFilter', function () {
        return function (input) {
            if (input == true)
                return 'Đã giao';
            else
                return 'Chưa giao';
        }
    });
})(angular.module('techzone.common'));