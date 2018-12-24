GetCartItems().done(function (data) {
    SetCartItemCount(data.cartItemsCount);
    
});
UpdateTotalAmount();


function GetCartItems() {
    
    return $.ajax({
        url: "/Home/GetCartItems",
        data: JSON.stringify("1"),
        type: "POST",
        dataType: "json",
        contentType: "application/json",

        success: function (data) {
            console.log("GetCartItems success =", data);
        },
        error: function (data) {
            console.log("GetCartItems error =", data);
        }

    });
}

function AjaxCallPost() {

}

function AddToCart(id) {
    AddingToCartNotification();
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
    $("#cartItemCount").html(count);
}

function RemoveFromCart(id) {
    RemoveSlideUpAnimation(id);
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
            NotifyProductRemoved();
        },
        error: function (data) {
            console.log("AddToCart error =", data);
        }
    });
    console.log("After Ajax");
    UpdateTotalAmount();
}


function NotifyProductAlreadyExists() {
    var notification = document.querySelector('.mdl-js-snackbar');
    notification.MaterialSnackbar.showSnackbar(
        {
            message: 'PRODUCT IS ALREADY IN THE CART, QUANTITY INCREASED'
        }
    );
}

function AddingToCartNotification() {
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

function NotifyProductRemoved() {
    var notification = document.querySelector('.mdl-js-snackbar');
    notification.MaterialSnackbar.showSnackbar(
        {
            message: 'PRODUCT REMOVED..'
        }
    );
}

function RemoveSlideUpAnimation(id) {
    console.log("RemoveSlideUpAnimation id =", id);
    //var string = "#cartitembox_" + id;
    $("#cartitembox_" + id).slideUp(1000);
    $("#cartAmountitembox_" + id).slideUp(1000);
    
}

function DeleteCart() {
    $.ajax({
        url: "/Home/DeleteCart",
        data: JSON.stringify("1"),
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            console.log("DeleteCart success =", data);
        },
        error: function (data) {
            console.log("DeleteCart error =", data);
        }
    });
}

function UpdateTotalAmount() {
    $.ajax({
        url: "/Home/UpdateTotalAmount",
        success: function (data) {
            console.log("UpdateTotalAmount data =", data);
            $("#totalAmount").html(data.amount);
        },
        error: function (data) {

        }
    })
    
}