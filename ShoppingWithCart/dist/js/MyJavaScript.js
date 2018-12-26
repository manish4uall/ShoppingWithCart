GetCartItems().done(function (data) {
    SetCartItemCount(data.cartItemsCount);
    
});
UpdateTotalCart();


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
            console.log("AddToCart currentItemCount =", data.currentItemCount);
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
            if (data.cartItemsCount == 0) {
                $("#cartPage").html('');
                $("#cartPage").html('<h3> Your Cart is Empty</h3>');
            }
        },
        error: function (data) {
            console.log("AddToCart error =", data);
        },
        complete: function (data) {
            console.log("RemoveFromCart complete =", data);
            UpdateTotalCart();
            
            
        }
    });
    console.log("After Ajax");
    
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

function NotifyByParameter(data) {
    var notification = document.querySelector('.mdl-js-snackbar');
    notification.MaterialSnackbar.showSnackbar(
        {
            message: data
        }
    );
}

function RemoveSlideUpAnimation(id) {
    console.log("RemoveSlideUpAnimation id =", id);
    //var string = "#cartitembox_" + id;
    $("#cartitembox_" + id).slideUp(1000);
    $("#cartAmountitembox_" + id).slideUp(1000);
    
}

function RemoveFadeOutAnimation(id) {
    console.log("RemoveFadeOutAnimation id =", id);
    //var string = "#cartitembox_" + id;
    $("#cartitembox_" + id).fadeOut(1000);
    $("#cartAmountitembox_" + id).fadeOut(1000);

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

function UpdateTotalCart(id) {
    console.log("UpdateTotalCart ProductId =", id);
    var obj = { id: id };
    $.ajax({
        data: JSON.stringify(obj),
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        url: "/Home/UpdateTotalCart",
        success: function (data) {
            console.log("UpdateTotalCart data =", data);
            
            $("#ItemQuantity_" + id).html(data.itemQuantity);
            $("#totalAmount").html(data.amount.toLocaleString('en'));
            $("#inputQuantitybox_"+id).val(data.itemQuantity);
            if (data.itemTotalPrice != undefined) {
                $("#ItemPrice_" + id).html(data.itemTotalPrice.toLocaleString('en'));
            }

            
            
        },
        error: function (data) {

        }
    })
    
}

function IncreaseProductAmount(id) {
    
    console.log("IncreaseProductAmount ProductId =", id);
    var obj = { id: id };
    $.ajax({
        url: "/Home/AddToCart",
        data: JSON.stringify(obj),
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            console.log("IncreaseProductAmount success =", data);
            UpdateTotalCart(id);
            //NotifyByParameter("Quantity =" + data.currentItemCount);
            console.log("IncreaseProductAmount currentItemCount =", data.currentItemCount);
        },
        error: function (data) {
            console.log("IncreaseProductAmount error =", data);
        }
    });
}

function DecreaseProductAmount(id) {

    console.log("DecreaseProductAmount ProductId =", id);
    var obj = { id: id };
    $.ajax({
        url: "/Home/DecreaseProductAmount",
        data: JSON.stringify(obj),
        type: "POST",
        dataType: "json",
        contentType: "application/json",
        success: function (data) {
            console.log("DecreaseProductAmount success =", data);
            UpdateTotalCart(id);
            //NotifyByParameter("Quantity =" + data.currentItemCount);
            console.log("DecreaseProductAmount currentItemCount =", data.currentItemCount);
            console.log("DecreaseProductAmount cartItemsCount =", data.cartItemsCount);
            SetCartItemCount(data.cartItemsCount);
            if (data.currentItemCount==0) {
                RemoveFadeOutAnimation(id);
            }
            if (data.cartItemsCount == 0) {
                $("#cartPage").html('');
                $("#cartPage").html('<h3> Your Cart is Empty</h3>');
            }
        },
        error: function (data) {
            console.log("DecreaseProductAmount error =", data);
        }
    });
}