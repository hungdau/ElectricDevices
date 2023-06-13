var cart = {
    init: function () {
        cart.loadData();
        cart.loadData2();
        cart.registerEvent();
    },
    registerEvent: function () {
        $('#frmPayment').validate({
            rules: {
                name: "required",
                address: "required",
                email: {
                    required: true,
                    email: true
                },
                phone: {
                    required: true,
                    number: true
                }
            },
            messages: {
                name: "Tên chưa nhập",
                address: "Địa chỉ chưa nhập",
                email: {
                    required: "Email chưa nhập",
                    email: "Định dạng email chưa đúng"
                },
                phone: {
                    required: "Số điện thoại chưa nhập",
                    number: "Số điện thoại phải là số."
                }
            }
        });
        $('.btnDeleteItem').off('click').on('click', function (e) {
            e.preventDefault();
            var productId = parseInt($(this).data('id'));
            cart.deleteItem(productId);
        });
        $('.txtQuantity').off('input').on('keypress', function (event) {
            if (event.keyCode === 13) {
                cart.quantityLimit($(this));
            }
        }).on('change', function () {
            cart.quantityLimit($(this));
        });
        //    $('#lblTotalOrder').text(numeral(cart.getTotalOrder()).format('0,0'));
        //    $('#lblTotalOrder2').text(numeral(cart.getTotalOrder2()).format('0,0'));
        //    cart.updateAll();
        //});
        $('.btnContinue').off('click').on('click', function (e) {
            e.preventDefault();
            window.location.href = "/";
        });
        $('#btnDeleteAll').off('click').on('click', function (e) {
            e.preventDefault();
            cart.deleteAll();
        });
        $('#btnCheckout').off('click').on('click', function (e) {
            e.preventDefault();
            $('#divCheckout').show();
        });
        $('#chkUserLoginInfo').off('click').on('click', function () {
            if ($(this).prop('checked'))
                cart.getLoginUser();
            else {
                $('#txtName').val('');
                $('#txtAddress').val('');
                $('#txtEmail').val('');
                $('#txtPhone').val('');
            }
        });
        $('#btnCreateOrder').off('click').on('click', function (e) {
            e.preventDefault();
            var isValid = $('#frmPayment').valid();
            if (isValid) {
                cart.createOrder();
            }

        });

    },
    quantityLimit: function (input) {
        var quantity = parseInt(input.val());
        var productid = parseInt(input.data('id'));
        var price = parseFloat(input.data('price'));

        if (isNaN(quantity) || quantity <= 0) {
            quantity = 0;
            alert("Số lượng không hợp lệ");
            input.val(0);
        } else {
            var result = cart.updateAll();
            if (!result.status) {
                quantity = result.quantity;
                input.val(result.quantity);
            }
        }

        var amount = quantity * price;
        $('#amount_' + productid).text(numeral(amount).format('0,0'));
        $('#lblTotalOrder').text(numeral(cart.getTotalOrder()).format('0,0'));
        $('#lblTotalOrder2').text(numeral(cart.getTotalOrder2()).format('0,0'));
        // Kiểm tra tiền hàng trước khi submit form
        $('#btnSubmitOrder').prop('disabled', (cart.getTotalOrder() <= 0));
    },
    getLoginUser: function () {
        $.ajax({
            url: '/ShoppingCart/GetUser',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status) {
                    var user = response.data;
                    $('#txtName').val(user.FullName);
                    $('#txtAddress').val(user.Address);
                    $('#txtEmail').val(user.Email);
                    $('#txtPhone').val(user.PhoneNumber);
                }
            }
        });
    },

    createOrder: function () {
        var order = {
            CustomerName: $('#txtName').val(),
            CustomerAddress: $('#txtAddress').val(),
            CustomerEmail: $('#txtEmail').val(),
            CustomerMobile: $('#txtPhone').val(),
            CustomerMessage: $('#txtMessage').val(),
            PaymentMethod: "Thanh toán tiền mặt",
            Status: false
        }
        $.ajax({
            url: '/ShoppingCart/CreateOrder',
            type: 'POST',
            dataType: 'json',
            data: {
                orderViewModel: JSON.stringify(order)
            },
            success: function (response) {
                if (response.status) {
                    console.log('create order ok');
                    $('#divCheckout').hide();
                    cart.deleteAll();
                    setTimeout(function () {
                        $('#cartContent').html('<div class="giorong">Cảm ơn bạn đã đặt hàng thành công. Chúng tôi sẽ liên hệ sớm nhất.<br/><br/><button class="btn btn-success btnContinue" style="background-color: #40a2f8">Tiếp tục mua hàng</button></div>');
                    }, 500);

                }
            }
        });
    },
    getTotalOrder: function () {
        var listTextBox = $('.txtQuantity');
        var total = 0;
        $.each(listTextBox, function (i, item) {
            var quantity = parseInt($(item).val());
            if (quantity > 0) {
                total += quantity * parseFloat($(item).data('price'));
            }
        });
        return total;
    },
    getTotalOrder2: function () {
        var listTextBox = $('.txtQuantity');
        var total = 0;
        $.each(listTextBox, function (i, item) {
            var quantity = parseInt($(item).val());
            if (quantity > 0) {
                total += quantity * parseFloat($(item).data('price'));
            }
        });
        return total;
    },
    deleteAll: function () {
        $.ajax({
            url: '/ShoppingCart/DeleteAll',
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status) {
                    cart.loadData();
                    cart.loadData2();
                }
            }
        });
    },
    updateAll: function () {
        var result = {};
        var cartList = [];
        $.each($('.txtQuantity'), function (i, item) {
            cartList.push({
                ProductId: $(item).data('id'),
                Quantity: $(item).val()
            });
        });
        $.ajax({
            url: '/ShoppingCart/Update',
            type: 'POST',
            data: {
                cartData: JSON.stringify(cartList)
            },
            dataType: 'json',
            async: false,
            success: function (response) {
                if (response.status) {
                    cart.loadData();
                    cart.loadData2();
                    result.status = true;
                } else {
                    alert("Không đủ số lượng sản phẩm trong kho");
                    $('.txtQuantity[data-id="' + response.productId + '"]').val(response.quantity);
                    result.status = false;
                    result.quantity = response.quantity;
                }
            }
        });
        return result;
    },
    deleteItem: function (productId) {
        $.ajax({
            url: '/ShoppingCart/DeleteItem',
            data: {
                productId: productId
            },
            type: 'POST',
            dataType: 'json',
            success: function (response) {
                if (response.status) {
                    cart.loadData();
                    cart.loadData2();
                }
            }
        });
    },
    loadData: function () {
        $.ajax({
            url: '/ShoppingCart/GetAll',
            type: 'GET',
            dataType: 'json',
            success: function (res) {
                if (res.status) {
                    var template = $('#tplCart').html();
                    var html = '';
                    var data = res.data;
                    $.each(data, function (i, item) {
                        html += Mustache.render(template, {
                            Id: i + 1,
                            ProductId: item.ProductId,
                            ProductName: item.Product.Name,
                            Image: item.Product.Image,
                            Price: item.Product.Price,
                            PriceF: numeral(item.Product.Price).format('0,0'),
                            Quantity: item.Quantity,
                            Amount: numeral(item.Quantity * item.Product.Price).format('0,0')
                        });
                    });

                    $('#cartBody').html(html);

                    if (html == '') {
                        $('#cartContent').html('<div class="giorong"">Không có sản phẩm nào trong giỏ hàng.<br/><br/><button class="btn btn-success btnContinue"  style="background-color: #40a2f8">Mua ngay</button></div>');
                    }
                    $('#lblTotalOrder').text(numeral(cart.getTotalOrder()).format('0,0'));
                    cart.registerEvent();
                }
            }
        });
    },
    loadData2: function () {
        $.ajax({
            url: '/ShoppingCart/GetAll',
            type: 'GET',
            dataType: 'json',
            success: function (res) {
                if (res.status) {
                    var template = $('#tplCart2').html();
                    var html = '';
                    var data = res.data;
                    $.each(data, function (i, item) {
                        html += Mustache.render(template, {
                            Id: i + 1,
                            ProductId: item.ProductId,
                            ProductName: item.Product.Name,
                            Image: item.Product.Image,
                            Price: item.Product.Price,
                            PriceF: numeral(item.Product.Price).format('0,0'),
                            Quantity: item.Quantity,
                            Amount: numeral(item.Quantity * item.Product.Price).format('0,0')
                        });
                    });

                    $('#cartBody2').html(html);
                    $('#lblTotalOrder2').text(numeral(cart.getTotalOrder2()).format('0,0'));
                    cart.registerEvent();
                }
            }
        })
    }
}
cart.init();