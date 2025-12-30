window.adminAssetManager = {
    // Danh sách các ID của thẻ CSS admin để dễ quản lý 
    loadedCssIds: [],

    loadCss: function (href) {
        // Kiểm tra nếu đã có rồi thì không thêm nữa
        if (document.querySelector(`link[href="${href}"]`)) return;

        var link = document.createElement("link");
        link.rel = "stylesheet";
        link.href = href;
        // Gán ID để sau này tìm xóa
        link.id = "admin-css-" + (Math.random().toString(36).substr(2, 9));
        document.head.appendChild(link);
        this.loadedCssIds.push(link.id);
    },

    loadScript: function (src) {
        return new Promise((resolve, reject) => {
            if (document.querySelector(`script[src="${src}"]`)) {
                resolve();
                return;
            }

            var script = document.createElement("script");
            script.src = src;
            script.type = "text/javascript";
            script.async = false; // Đảm bảo load theo thứ tự nếu gọi nhiều file
            script.onload = resolve;
            script.onerror = reject;
            document.body.appendChild(script);
        });
    },

    unloadAdminCss: function () {
        this.loadedCssIds.forEach(id => {
            var el = document.getElementById(id);
            if (el) el.remove();
        });
        this.loadedCssIds = [];
    },

    // --- TẮT LOADING ---
    hideLoader: function () {
        $('.theme-loader').fadeOut('slow', function () {
            $(this).remove();
        });
    },
};