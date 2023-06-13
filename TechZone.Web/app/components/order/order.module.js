/// <reference path="../../../assets/admin/libs/angular/angular.js" />

(function () {
    angular.module('techzone.order', ['techzone.common']).config(config);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('order', {
                url: "/order",
                parent: 'base',
                templateUrl: "/app/components/order/orderListView.html",
                controller: "orderListController"
            }).state('order_detail', {
                url: "/order_detail/:id",
                parent: 'base',
                templateUrl: "/app/components/order/orderDetailView.html",
                controller: "orderDetailController"
            });
    }
})();