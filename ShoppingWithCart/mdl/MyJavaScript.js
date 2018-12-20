
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

function NotifyProductAlreadyExists() {
    var notification = document.querySelector('.mdl-js-snackbar');
    notification.MaterialSnackbar.showSnackbar(
        {
            message: 'Product is Already in the Cart'
        }
    );
}


function AddedToCartNotification() {
    console.log("Inside Notification");
}
 