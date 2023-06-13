(function (app) {
    app.controller('productEditController', productEditController);

    productEditController.$inject = ['apiService', '$scope', 'notificationService', '$state', 'commonService', '$stateParams'];

    function productEditController(apiService, $scope, notificationService, $state, commonService, $stateParams) {
        $scope.product = {};
        $scope.ckeditorOptions = {
            languague: 'vi',
            height: '200px'
        }

        $scope.UpdateProduct = UpdateProduct;
        $scope.moreImages = [];
        $scope.GetSeoTitle = GetSeoTitle;
        $scope.loadManufacturer = loadManufacturer;

        function GetSeoTitle() {
            $scope.product.Alias = commonService.getSeoTitle($scope.product.Name);
        }

        function loadProductDetail() {
            apiService.get('api/product/getbyid/' + $stateParams.idProduct, null, function (result) {
                $scope.product = result.data;
                $scope.moreImages = JSON.parse($scope.product.MoreImages);
                apiService.get('/api/productcategory/getbyid/' + $scope.product.CategoryID, null, function (respose) {
                    $scope.product.ProductCategoryID = respose.data.ParentID;
                    $scope.product.CategoryID = respose.data.ID;
                }, function () { });
                var config = {
                    params: {
                        keyword: '',
                        page: 0,
                        pageSize: 100
                    }
                }
                apiService.get('api/productcategory/getallManufacturer/' + $stateParams.idCategory, config, function (resultManufacturer) {
                    $scope.productManufacturer = resultManufacturer.data.Items;
                }, function () { });
            }, function (error) {
                notificationService.displayError(error.data);
            });
        }
        function UpdateProduct() {
            $scope.product.MoreImages = JSON.stringify($scope.moreImages);
            if (!$scope.product.CategoryID)
                $scope.product.CategoryID = $scope.product.ProductCategoryID;
            apiService.put('api/product/update', $scope.product,
                function (result) {
                    notificationService.displaySuccess(result.data.Name + ' đã được cập nhật.');
                    /*$state.go('products');*/
                    $state.go('products(categoryId:' + $scope.product.CategoryID + ')');
                }, function (error) {
                    notificationService.displayError('Cập nhật không thành công.');
                });
        }

        function loadProductCategory() {
            var config = {
                params: {
                    keyword: '',
                    page: 0,
                    pageSize: 100
                }
            }
            apiService.get('api/productcategory/getallProductCategory', config, function (result) {
                $scope.productCategories = result.data.Items;
            }, function () { });
        }

        function loadManufacturer() {
            $scope.product.CategoryID = null;
            var config = {
                params: {
                    keyword: '',
                    page: 0,
                    pageSize: 100
                }
            }
            apiService.get('api/productcategory/getallManufacturer/' + $scope.product.ProductCategoryID, config, function (result) {
                $scope.productManufacturer = result.data.Items;
            }, function () { });
        }

        $scope.ChooseImage = function () {
            var finder = new CKFinder();
            finder.selectActionFunction = function (fileUrl) {
                $scope.$apply(function () {
                    $scope.product.Image = fileUrl;
                })
            }
            finder.popup();
        }

        $scope.images = [];

        $scope.ChooseMoreImage = function () {
            var finder = new CKFinder();
            finder.selectActionFunction = function (fileUrl) {
                $scope.$apply(function () {
                    if (!Array.isArray($scope.moreImages)) {
                        $scope.moreImages = [];
                    }
                    if ($scope.moreImages.indexOf(fileUrl) === -1) {
                        $scope.moreImages.push(fileUrl);
                    }
                })

            }
            finder.popup();
        }

        loadProductCategory();
        loadProductDetail();
        loadManufacturer();
    }
})(angular.module('techzone.products'));