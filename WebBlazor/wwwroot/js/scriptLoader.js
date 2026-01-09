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

    // Hàm nạp script thủ công trả về Promise
    loadScript: function (scriptPath) {
        return new Promise((resolve, reject) => {
            // Nếu script đã tồn tại, xóa bỏ để nạp lại cái mới (ép buộc re-init)
            const existingScript = document.querySelector(`script[src="${scriptPath}"]`);
            if (existingScript) {
                existingScript.remove();
            }

            const script = document.createElement('script');
            script.src = scriptPath + "?v=" + new Date().getTime(); // Anti-cache
            script.type = 'text/javascript';
            script.async = false; // Quan trọng: Đảm bảo thứ tự thực thi

            script.onload = () => resolve(scriptPath);
            script.onerror = () => reject(new Error(`Could not load script: ${scriptPath}`));

            document.body.appendChild(script);
        });
    },
    // Hàm tổng hợp để load toàn bộ template
    initAdminLayout: async function () {
        try {
            // Load THỨ TỰ (Sequentially) - Thằng sau phụ thuộc thằng trước
            await this.loadScript('js/pcoded.min.js');
            await this.loadScript('js/vartical-layout.min.js');
            await this.loadScript('js/script.js');

            console.log("Admin Template Scripts Re-initialized!");
            return true;
        } catch (e) {
            console.error("Template Init Error:", e);
            return false;
        }
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


// Load các script của template admin
window.loadTemplateScripts = async () => {
    try {
        await $.getScript('js/pcoded.min.js');
        await $.getScript('js/vartical-layout.min.js');
        await $.getScript('js/script.js');
        console.log("Scripts loaded successfully");
    } catch (e) {
        console.error("Error loading scripts:", e);
    }
};