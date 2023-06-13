(function (app) {
    app.controller('manufacturerEditController', manufacturerEditController);

    manufacturerEditController.$inject = ['apiService', '$scope', 'notificationService', '$state', '$stateParams', 'commonService'];

    function manufacturerEditController(apiService, $scope, notificationService, $state, $stateParams, commonService) {
        $scope.manufacturer = {
            CreatedDate: new Date(),
            Status: true,
        }

        $scope.UpdateManufacturer = UpdateManufacturer;
        $scope.GetSeoTitle = GetSeoTitle;
        $scope.productCategoryId = $stateParams.idCategory;

        function GetSeoTitle() {
            $scope.manufacturer.Alias = commonService.getSeoTitle($scope.manufacturer.Name);
        }

        function loadManufacturerDetail() {
            apiService.get('api/productCategory/getbyid/' + $stateParams.idManufacturer, null, function (result) {
                $scope.manufacturer = result.data;
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }

        function UpdateManufacturer() {
            apiService.put('api/productcategory/update', $scope.manufacturer,
                function (result) {
                    notificationService.displaySuccess(result.data.Name + ' đã được cập nhật.');
                    $state.go('manufacturer', { id: $stateParams.idCategory });
                }, function (error) {
                    notificationService.displayError('Cập nhật không thành công.');
                });
        }
        function loadParentCategory() {
            apiService.get('api/productcategory/getallparents', null, function (result) {
                $scope.parentCategories = commonService.getTree(result.data, "ID", "ParentID");
            }, function () {
                console.log('Cannot get list parent');
            });
        }

        function getProductCategory() {
            apiService.get('/api/productcategory/getbyid/' + $stateParams.idCategory, null, function (result) {
                $scope.productCategory = result.data.Name;
            }, function () { });
        }
        getProductCategory();
        loadParentCategory();
        loadManufacturerDetail();
    }

})(angular.module('techzone.manufacturer'));