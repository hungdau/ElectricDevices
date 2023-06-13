/// <reference path="../../../assets/admin/libs/angular/angular.js" />

(function () {
    angular.module('techzone.products', ['techzone.common']).config(config);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {
        $stateProvider
            .state('products', {
                url: "/products/:categoryId",
                parent: 'base',
                templateUrl: "/app/components/products/productListView.html",
                controller: "productListController"
            }).state('product_all', {
                url: "/products",
                parent: 'base',
                templateUrl: "/app/components/products/productListView.html",
                controller: "productListController"
            }).state('product_add', {
                url: "/product_add",
                parent: 'base',
                templateUrl: "/app/components/products/productAddView.html",
                controller: "productAddController"
            }).state('product_import', {
                url: "/product_import",
                parent: 'base',
                templateUrl: "/app/components/products/productImportView.html",
                controller: "productImportController"
            }).state('product_edit', {
                url: "/product_edit/:idProduct/:idCategory",
                parent: 'base',
                templateUrl: "/app/components/products/productEditView.html",
                controller: "productEditController"
            });
    }
})();