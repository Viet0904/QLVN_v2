// Bootstrap-Growl Notification Manager
window.notificationManager = {
    show: function (message, type, position, align, delay, icon) {
        // Map type để sử dụng với bootstrap-growl
        const typeMap = {
            'success': {
                cssClass: 'alert alert-success',
                bootstrapGrowlType: 'success'
            },
            'danger': {
                cssClass: 'alert alert-danger',
                bootstrapGrowlType: 'danger'
            },
            'warning': {
                cssClass: 'alert alert-warning',
                bootstrapGrowlType: 'warning'
            },
            'info': {
                cssClass: 'alert alert-info',
                bootstrapGrowlType: 'info'
            },
            'primary': {
                cssClass: 'alert alert-primary',
                bootstrapGrowlType: 'primary'
            },
            'inverse': {
                cssClass: 'alert alert-inverse',
                bootstrapGrowlType: 'inverse'
            }
        };

        const notificationType = typeMap[type] || typeMap['info'];

        // Cấu hình vị trí
        const allowDismiss = true;
        const placement = {
            from: position === 'bottom' ? 'bottom' : 'top',
            align: align === 'left' ? 'left' : (align === 'center' ? 'center' : 'right')
        };

        // Tạo HTML nội dung notification
        const iconHtml = icon ? `<i class="${icon}" style="margin-right: 8px;"></i>` : '';
        const content = `${iconHtml}${message}`;

        // Sử dụng bootstrap-growl nếu có
        if (typeof $.growl !== 'undefined') {
            $.growl({
                message: content,
                duration: delay
            }, {
                type: notificationType.bootstrapGrowlType,
                placement: placement,
                offset: { x: 20, y: 85 },
                allow_dismiss: allowDismiss,
                delay: delay,
                animate: {
                    enter: 'animated fadeInRight',
                    exit: 'animated fadeOutRight'
                }
            });
        } else {
            // Fallback: tạo notification tùy chỉnh nếu bootstrap-growl không có
            this.showFallback(message, type, position, align, delay, icon);
        }
    },

    // Shorthand methods cho Blazor gọi trực tiếp
    success: function (message, delay) {
        this.show(message, 'success', 'top', 'right', delay || 3000, 'feather icon-check-circle');
    },

    error: function (message, delay) {
        this.show(message, 'danger', 'top', 'right', delay || 5000, 'feather icon-x-circle');
    },

    warning: function (message, delay) {
        this.show(message, 'warning', 'top', 'right', delay || 4000, 'feather icon-alert-triangle');
    },

    info: function (message, delay) {
        this.show(message, 'info', 'top', 'right', delay || 3000, 'feather icon-info');
    },

    showFallback: function (message, type, position, align, delay, icon) {
        // Fallback khi bootstrap-growl không sẵn có
        const typeMap = {
            'success': { bg: '#dff0d8', border: '#3c763d', color: '#3c763d', iconColor: '#3c763d' },
            'danger': { bg: '#f2dede', border: '#a94442', color: '#a94442', iconColor: '#a94442' },
            'warning': { bg: '#fcf8e3', border: '#8a6d3b', color: '#8a6d3b', iconColor: '#8a6d3b' },
            'info': { bg: '#d9edf7', border: '#31708f', color: '#31708f', iconColor: '#31708f' },
            'primary': { bg: '#d1ecf1', border: '#0c5460', color: '#0c5460', iconColor: '#0c5460' },
            'inverse': { bg: '#f5f5f5', border: '#333', color: '#333', iconColor: '#333' }
        };

        const colors = typeMap[type] || typeMap['info'];
        const notificationId = 'notification-' + Date.now();

        // Xác định vị trí CSS
        const positionY = position === 'bottom' ? 'bottom' : 'top';
        let positionXStyle = '';
        if (align === 'left') {
            positionXStyle = 'left: 20px;';
        } else if (align === 'center') {
            positionXStyle = 'left: 50%; transform: translateX(-50%);';
        } else {
            positionXStyle = 'right: 20px;';
        }

        // Tạo HTML
        const iconHtml = icon ? `<i class="${icon}" style="margin-right: 12px; font-size: 18px;"></i>` : '';
        const notificationHtml = `
            <div id="${notificationId}" class="custom-notification custom-notification-${type}" style="
                position: fixed;
                ${positionY}: 20px;
                ${positionXStyle}
                min-width: 320px;
                max-width: 450px;
                padding: 16px 20px;
                background-color: ${colors.bg};
                border-left: 4px solid ${colors.border};
                color: ${colors.color};
                border-radius: 6px;
                box-shadow: 0 6px 16px rgba(0, 0, 0, 0.15);
                z-index: 99999;
                animation: slideInRight 0.3s ease-out;
                display: flex;
                align-items: center;
                font-size: 14px;
                line-height: 1.5;
                font-weight: 500;
            ">
                <span style="flex: 1; display: flex; align-items: center;">${iconHtml}${message}</span>
                <button type="button" style="
                    background: none;
                    border: none;
                    color: ${colors.color};
                    cursor: pointer;
                    font-size: 20px;
                    padding: 0;
                    margin-left: 12px;
                    opacity: 0.7;
                    transition: opacity 0.2s;
                    line-height: 1;
                " onclick="document.getElementById('${notificationId}').remove();" 
                onmouseover="this.style.opacity='1'" 
                onmouseout="this.style.opacity='0.7'">
                    ×
                </button>
            </div>
        `;

        // Thêm CSS animation nếu chưa có
        if (!document.querySelector('#notification-styles')) {
            const style = document.createElement('style');
            style.id = 'notification-styles';
            style.textContent = `
                @keyframes slideInRight {
                    from {
                        transform: translateX(120%);
                        opacity: 0;
                    }
                    to {
                        transform: translateX(0);
                        opacity: 1;
                    }
                }
                
                @keyframes slideOutRight {
                    from {
                        transform: translateX(0);
                        opacity: 1;
                    }
                    to {
                        transform: translateX(120%);
                        opacity: 0;
                    }
                }
                
                .notification-exit {
                    animation: slideOutRight 0.3s ease-out forwards !important;
                }

                .custom-notification {
                    font-family: 'Open Sans', sans-serif;
                }
            `;
            document.head.appendChild(style);
        }

        // Thêm notification vào body
        document.body.insertAdjacentHTML('beforeend', notificationHtml);
        const notificationEl = document.getElementById(notificationId);

        // Tự động xóa sau delay
        setTimeout(() => {
            if (notificationEl) {
                notificationEl.classList.add('notification-exit');
                setTimeout(() => {
                    if (notificationEl && notificationEl.parentNode) {
                        notificationEl.remove();
                    }
                }, 300);
            }
        }, delay || 3000);
    }
};

