"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

connection.on("ReceiveNotification", function (notification) {
    // Update badge count
    var badge = document.getElementById("notificationBadge");
    var currentCount = parseInt(badge.innerText) || 0;
    badge.innerText = currentCount + 1;
    badge.classList.remove("d-none");

    // Add to list
    var list = document.getElementById("notificationList");
    if (list.innerHTML.includes("Không có thông báo mới")) {
        list.innerHTML = "";
    }

    var newItem = `
        <li class="notification-item p-3 border-bottom bg-light">
            <a href="${notification.link || notification.Link || '#'}" class="text-decoration-none text-dark d-block">
                <div class="fw-bold small">${notification.title || notification.Title}</div>
                <div class="text-muted small">${notification.message || notification.Message}</div>
                <div class="text-end" style="font-size: 0.7rem;">Vừa xong</div>
            </a>
        </li>
    `;
    list.insertAdjacentHTML('afterbegin', newItem);

    // Show toast if library exists (Toastr or similar)
    if (window.toastr) {
        toastr.info(notification.message, notification.title);
    }
});

connection.start().then(function () {
    console.log("SignalR Connected");
}).catch(function (err) {
    return console.error(err.toString());
});

// Load existing notifications
$(document).ready(function () {
    loadNotifications();

    $("#markAllRead").click(function () {
        $.post("/Notification/MarkAllAsRead", function () {
            $("#notificationBadge").addClass("d-none").text("0");
            $(".notification-item").removeClass("bg-light");
        });
    });
});

function loadNotifications() {
    $.get("/Notification/GetLatest", function (data) {
        var list = document.getElementById("notificationList");
        var badge = document.getElementById("notificationBadge");

        if (data && data.length > 0) {
            if (list) list.innerHTML = "";
            var unreadCount = 0;
            data.forEach(function (n) {
                if (!n.isRead && !n.IsRead) unreadCount++;
                var item = `
                    <li class="notification-item p-3 border-bottom ${n.isRead || n.IsRead ? '' : 'bg-light'}">
                        <a href="${n.link || n.Link || '#'}" class="text-decoration-none text-dark d-block">
                            <div class="fw-bold small">${n.title || n.Title}</div>
                            <div class="text-muted small text-truncate">${n.message || n.Message}</div>
                            <div class="text-end text-muted" style="font-size: 0.7rem;">${new Date(n.createdAt || n.CreatedAt).toLocaleDateString()}</div>
                        </a>
                    </li>
                `;
                if (list) list.insertAdjacentHTML('beforeend', item);
            });

            if (unreadCount > 0 && badge) {
                badge.innerText = unreadCount;
                badge.classList.remove("d-none");
            }
        }
    });
}
