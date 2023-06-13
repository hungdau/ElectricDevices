/// <reference path="../../../assets/admin/libs/angular/angular.js" />

(function () {
    angular.module('techzone.manufacturer', ['techzone.common']).config(config);

    config.$inject = ['$stateProvider', '$urlRouterProvider'];

    function config($stateProvider, $urlRouterProvider) {

        $stateProvider.state('manufacturer', {
            url: "/manufacturer/:id",
            parent: 'base',
            templateUrl: "/app/components/manufacturer/manufacturerListView.html",
            controller: "manufacturerListController"
        }).state('add_manufacturer', {
            url: "/add_manufacturer/:id",
            parent: 'base',
            templateUrl: "/app/components/manufacturer/manufacturerAddView.html",
            controller: "manufacturerAddController"
        }).state('edit_manufacturer', {
            url: "/edit_manufacturer/:idCategory/:idManufacturer",
            parent: 'base',
            templateUrl: "/app/components/manufacturer/manufacturerEditView.html",
            controller: "manufacturerEditController"
        });
    }
})();