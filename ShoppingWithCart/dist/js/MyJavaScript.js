GetCartItems();


function GetCartItems() {
    $.ajax({
        url: "/Home/GetCartItems",
        data: JSON.stringify("1"),
        type: "POST",
        dataType: "json",
        contentType: "application/json",

        success: function (data) {
            console.log("GetCartItems success =", data);
            SetCartItemCount(data.cartItemsCount);
        },
        error: function (data) {
            console.log("GetCartItems error =", data);
        }
    });
}

function AddToCart(id) {
    AddedToCartNotification();
    console.log("AddToCart ProductId =", id);
    var obj = { id: id };
    $.ajax({
        url: "/Home/AddToCart",
        data: JSON.stringify(obj),
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            console.log("AddToCart success =", data);
            console.log("AddToCart productExistenceStatus =", data.productStatus);
            SetCartItemCount(data.cartItemsCount);

            if (data.productStatus == true) {
                NotifyProductAlreadyExists();
            }
            else {
                NotifyProductAdded();
            }
        },
        error: function (data) {
            console.log("AddToCart error =", data);
        }
    });
}

function SetCartItemCount(count) {
    console.log("SetCartItemCount count =", count);
    $("#cartItemCount").html('');
    $("#cartItemCount").append(count);
}

function RemoveFromCart(id) {
    console.log("RemoveFromCart id =", id);
    var obj = { id: id };
    $.ajax({
        url: "/Home/RemoveFromCart",
        data: JSON.stringify(obj),
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            console.log("RemoveFromCart success =", data);
            SetCartItemCount(data.cartItemsCount);
        },
        error: function (data) {
            console.log("AddToCart error =", data);
        }
    });
}

function NotifyProductAlreadyExists() {
    var notification = document.querySelector('.mdl-js-snackbar');
    notification.MaterialSnackbar.showSnackbar(
        {
            message: 'PRODUCT IS ALREADY IN THE CART'
        }
    );
}

function AddedToCartNotification() {
    console.log("AddedToCartNotification");
    var notification = document.querySelector('.mdl-js-snackbar');
    notification.MaterialSnackbar.showSnackbar(
        {
            message: 'ADDING TO CART...',
            timeout: 800
        }
    );
}

function NotifyProductAdded() {
    console.log("NotifyProductAdded");
    var notification = document.querySelector('.mdl-js-snackbar');
    var handler = function (event) {
        console.log("Handler Worked");
        window.location.href = "/Home/Cart";
    };
    var data = {
        message: 'PRODUCT ADDED...',
        timeout: 5000,
        actionHandler: handler,
        actionText: 'CART'
    };
    notification.MaterialSnackbar.showSnackbar(data);
}
 