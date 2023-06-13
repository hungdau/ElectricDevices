(function (app) {
    app.controller('productAddController', productAddController);

    productAddController.$inject = ['apiService', '$scope', 'notificationService', '$state', 'commonService'];

    function productAddController(apiService, $scope, notificationService, $state, commonService) {
        $scope.product = {
            CreatedDate: new Date(),
            Status: true
        }
        $scope.ckeditorOptions = {
            languague: 'vi',
            height: '200px'
        }

        $scope.AddProduct = AddProduct;
        $scope.GetSeoTitle = GetSeoTitle;
        $scope.loadManufacturer = loadManufacturer;

        function GetSeoTitle() {
            $scope.product.Alias = commonService.getSeoTitle($scope.product.Name);
        }

        function AddProduct() {
            $scope.product.MoreImages = JSON.stringify($scope.moreImages);
            if (!$scope.product.CategoryID)
                $scope.product.CategoryID = $scope.product.ProductCategoryID;
            apiService.post('api/product/create', $scope.product,
                function (result) {
                    notificationService.displaySuccess(result.data.Name + ' đã được thêm mới.');
                    $state.go('products');
                }, function (error) {
                    notificationService.displayError('Thêm mới không thành công.');
                });
            console.log($scope.product);
        }
        console.log(JSON.stringify($scope.moreImages));

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

        $scope.moreImages = [];

        $scope.ChooseMoreImage = function () {
            var finder = new CKFinder();
            finder.selectActionFunction = function (fileUrl) {
                $scope.$apply(function () {
                    if ($scope.moreImages.indexOf(fileUrl) === -1) {
                        $scope.moreImages.push(fileUrl);
                    }
                })
            }
            finder.popup();
        }

        loadProductCategory();
    }
})(angular.module('techzone.products'));