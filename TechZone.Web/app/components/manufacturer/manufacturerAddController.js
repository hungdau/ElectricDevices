(function (app) {
    app.controller('manufacturerAddController', manufacturerAddController);

    manufacturerAddController.$inject = ['apiService', '$scope', 'notificationService', '$state', 'commonService', '$stateParams'];

    function manufacturerAddController(apiService, $scope, notificationService, $state, commonService, $stateParams) {
        $scope.manufacturer = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.AddManufacturer = AddManufacturer;
        $scope.GetSeoTitle = GetSeoTitle;
        $scope.productCategoryId = $stateParams.id;

        function GetSeoTitle() {
            $scope.manufacturer.Alias = commonService.getSeoTitle($scope.manufacturer.Name);
        }

        function AddManufacturer() {
            apiService.post('api/productcategory/create', $scope.manufacturer,
                function (result) {
                    notificationService.displaySuccess(result.data.Name + ' đã được thêm mới.');
                    $state.go('manufacturer', { id: $stateParams.id });
                }, function (error) {
                    notificationService.displayError('Thêm mới không thành công.');
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
            apiService.get('/api/productcategory/getbyid/' + $stateParams.id, null, function (result) {
                $scope.productCategory = result.data.Name;
            }, function () { });
        }
        getProductCategory();
        loadParentCategory();
    }
})(angular.module('techzone.manufacturer'));