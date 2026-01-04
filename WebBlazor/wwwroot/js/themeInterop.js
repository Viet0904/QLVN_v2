window.themeInterop = {
    // Lấy chiều cao của header để tính toán margin-top
    _getHeaderHeightPx: function (header) {
        if (!header) return 0;
        let h = header.getBoundingClientRect().height || header.offsetHeight || 0;
        if (typeof $ !== 'undefined' && $.fn && $.fn.jquery) {
            try {
                const jq = $(header).outerHeight();
                if (jq && jq > h) h = jq;
            } catch (e) { /* ignore */ }
        }
        return Math.ceil(h);
    },

    // Loại bỏ margin-top inline để CSS controls layout.
    _removeMainContainerInlineMargin: function () {
        const mainContainer = document.querySelector('.pcoded-main-container');
        if (!mainContainer) return;
        try {
            // Loại bỏ margin-top inline
            mainContainer.style.removeProperty('margin-top');
        } catch (e) {
            
            mainContainer.style.marginTop = '';
        }
    },

    // Cập nhật margin-top cho main-container
    updateHeaderMargin: function () {
        const header = document.querySelector('.pcoded-header');
        const pcoded = document.getElementById('pcoded');
        const mainContainer = document.querySelector('.pcoded-main-container');

        if (!header || !mainContainer || !pcoded) return;

        const isFixedHeader = pcoded.classList.contains('pcoded-fixed-header');

        // Tính toán chiều cao của header
        const headerHeight = this._getHeaderHeightPx(header);

        // Loại bỏ margin-top inline
        this._removeMainContainerInlineMargin();
    },

    // Đảm bảo observer được khởi tạo
    _ensureMarginObserver: function () {
        if (this._observerInitialized) return;
        this._observerInitialized = true;

        try {
            const mainContainer = document.querySelector('.pcoded-main-container');
            if (!mainContainer || !window.MutationObserver) return;

            // Tạo observer để theo dõi thay đổi style của main-container
            const observer = new MutationObserver((mutations) => {
                for (const m of mutations) {
                    if (m.type === 'attributes' && m.attributeName === 'style') {
                        // Đợi 40ms để cho các script khác viết style vào main-container
                        setTimeout(() => {
                            this._removeMainContainerInlineMargin();
                        }, 40);
                        break;
                    }
                }
            });
            // Theo dõi thay đổi style của main-container
            observer.observe(mainContainer, { attributes: true, attributeFilter: ['style'] });
        } catch (e) {
            // ignore
        }
    },

    // Khởi tạo lại tabs
    reinitializeTabs: function () {
        if (typeof $ !== 'undefined' && $.fn.tab) {
            $('.nav-tabs a[data-toggle="tab"]').off('click').on('click', function (e) {
                e.preventDefault();
                $(this).tab('show');
            });
            $('.nav-tabs .nav-link.active').tab('show');
            console.log('Tabs reinitialized');
        }
    },

    // Đặt sidebar cố định
    setSidebarFixed: function (isFixed) {
        const navbar = document.querySelector('.pcoded-navbar');
        const inner = document.querySelector('.pcoded-inner-navbar');
        if (!navbar || !inner) {
            console.warn('Navbar or inner navbar not found');
            return;
        }
        if (isFixed) {
            navbar.setAttribute('pcoded-navbar-position', 'fixed');
            navbar.style.position = 'fixed';
            navbar.style.top = '';
            inner.style.overflowY = 'auto';
            inner.style.height = 'calc(100vh - 80px)';
        } else {
            navbar.setAttribute('pcoded-navbar-position', 'absolute');
            navbar.style.position = 'absolute';
            navbar.style.top = '';
            inner.style.overflowY = '';
            inner.style.height = '';
        }
    },

    // Đặt header cố định
    setHeaderFixed: function (isFixed) {
        const header = document.querySelector('.pcoded-header');
        const navbar = document.querySelector('.pcoded-navbar');
        const pcoded = document.getElementById('pcoded');

        if (!header || !pcoded) return;

        if (isFixed) {
            pcoded.classList.add('pcoded-fixed-header');
            header.setAttribute('pcoded-header-position', 'fixed');
            if (navbar) navbar.setAttribute('pcoded-header-position', 'fixed');
            header.style.position = ''; // Clear inline position
        } else {
            pcoded.classList.remove('pcoded-fixed-header');
            header.setAttribute('pcoded-header-position', 'relative');
            if (navbar) navbar.setAttribute('pcoded-header-position', 'relative');
            header.style.position = ''; // Clear inline position


            // Khi header = relative và sidebar = fixed, giữ navbar absolute nếu cần thiết
            try {
                if (navbar) {
                    const navbarPosition = navbar.getAttribute('pcoded-navbar-position');
                    if (navbarPosition === 'fixed') {
                        navbar.style.position = 'absolute';
                    }
                }
            } catch (e) { /* ignore */ }
        }

        // Loại bỏ margin-top inline
        this._removeMainContainerInlineMargin();
        
        // Đảm bảo observer hoạt động để chống lại các script khác cố gắng thêm margin
        this._ensureMarginObserver();
    },

    // Đặt loại menu
    setMenuType: function (type) {
        const pcoded = document.getElementById('pcoded');
        if (!pcoded) return;
        pcoded.setAttribute('nav-type', type);
        pcoded.offsetHeight;
    },

    // Lấy thông tin về chủ đề hiện tại
    getCurrentTheme: function () {
        const pcoded = document.getElementById('pcoded');
        const sidebarEffect = document.getElementById('vertical-menu-effect')?.value || 'shrink';
        const borderStyle = document.getElementById('vertical-border-style')?.value || 'none';
        const dropdownIcon = document.getElementById('vertical-dropdown-icon')?.value || 'style1';
        const subItemIcon = document.getElementById('vertical-subitem-icon')?.value || 'style1';

        const navbar = document.querySelector('.pcoded-navbar');
        const logo = document.querySelector('.navbar-logo');
        const header = document.querySelector('.pcoded-header');
        const navLabel = document.querySelector('.pcoded-navigatio-lavel');

        const mainLayout = navbar?.getAttribute('navbar-theme') || 'theme1';
        const headerBrandColor = logo?.getAttribute('logo-theme') || 'theme1';
        const headerColor = header?.getAttribute('header-theme') || 'theme1';
        const activeLinkColor = navbar?.getAttribute('active-item-theme') || 'theme1';
        const menuCaptionColor = navLabel?.getAttribute('menu-title-theme') || 'theme5';
        const menuType = pcoded?.getAttribute('nav-type') || 'st6';

        // console.log('getCurrentTheme - Reading from DOM attributes:', {
        //     mainLayout,
        //     headerBrandColor,
        //     headerColor,
        //     activeLinkColor,
        //     menuCaptionColor
        // });

        return {
            isFixedSidebar: true,
            isFixedHeader: true,
            mainLayout: mainLayout,
            menuType: menuType,
            sidebarEffect: sidebarEffect,
            borderStyle: borderStyle,
            dropdownIcon: dropdownIcon,
            subItemIcon: subItemIcon,
            headerBrandColor: headerBrandColor,
            headerColor: headerColor,
            activeLinkColor: activeLinkColor,
            menuCaptionColor: menuCaptionColor
        };
    },

    // Áp dụng các thuộc tính chủ đề
    applyThemeAttributes: function (settings) {
        // console.log('Applying theme attributes:', settings);
        if (typeof $ === 'undefined') {
            // console.warn('jQuery not ready, retrying...');
            setTimeout(() => this.applyThemeAttributes(settings), 300);
            return;
        }
        // Áp dụng các thuộc tính chủ đề
        try {
            const $navbar = $('.pcoded-navbar');
            const $logo = $('.navbar-logo');
            const $header = $('.pcoded-header');
            const $navLabel = $('.pcoded-navigatio-lavel');

            $navbar.removeAttr('navbar-theme');
            $navbar.removeAttr('active-item-theme');
            $logo.removeAttr('logo-theme');
            $header.removeAttr('header-theme');
            $navLabel.removeAttr('menu-title-theme');

            $logo.attr('logo-theme', settings.headerBrandColor || 'theme1');
            $logo.css('z-index', '1030');
            $logo.css('position', 'relative');

            $header.attr('header-theme', settings.headerColor || 'theme1');
            $header.css('z-index', '1029');
            $header.css('position', 'relative');

            $navbar.attr('navbar-theme', settings.mainLayout || 'theme1');
            $navbar.attr('active-item-theme', settings.activeLinkColor || 'theme1');
            $navLabel.attr('menu-title-theme', settings.menuCaptionColor || 'theme5');

            const sidebarEffectSelect = document.getElementById('vertical-menu-effect');
            if (sidebarEffectSelect) {
                sidebarEffectSelect.value = settings.sidebarEffect || 'shrink';
                sidebarEffectSelect.dispatchEvent(new Event('change'));
            }

            const borderStyleSelect = document.getElementById('vertical-border-style');
            if (borderStyleSelect) {
                borderStyleSelect.value = settings.borderStyle || 'none';
                borderStyleSelect.dispatchEvent(new Event('change'));
            }

            const dropdownIconSelect = document.getElementById('vertical-dropdown-icon');
            if (dropdownIconSelect) {
                dropdownIconSelect.value = settings.dropdownIcon || 'style1';
                dropdownIconSelect.dispatchEvent(new Event('change'));
            }

            const subItemIconSelect = document.getElementById('vertical-subitem-icon');
            if (subItemIconSelect) {
                subItemIconSelect.value = settings.subItemIcon || 'style1';
                subItemIconSelect.dispatchEvent(new Event('change'));
            }

            this.highlightActiveColorButtons(settings);

            $navbar[0].offsetHeight;
            $logo[0].offsetHeight;
            $header[0].offsetHeight;

            // Đảm bảo không để lại margin inline
            setTimeout(() => {
                this._removeMainContainerInlineMargin();
            }, 60);

        } catch (e) {
            // ignore
        }
    },

    // Đánh dấu các nút màu hoạt động
    highlightActiveColorButtons: function (settings) {
        try {
            const $mainLayout = $(`.navbar-theme[navbar-theme="${settings.mainLayout}"]`);
            const $logo = $(`.logo-theme[logo-theme="${settings.headerBrandColor}"]`);
            const $header = $(`.header-theme[header-theme="${settings.headerColor}"]`);
            const $activeItem = $(`.active-item-theme[active-item-theme="${settings.activeLinkColor}"]`);
            const $menuCaption = $(`.leftheader-theme[lheader-theme="${settings.menuCaptionColor}"]`);

            setTimeout(() => { if ($mainLayout.length > 0) $mainLayout.trigger('click'); }, 100);
            setTimeout(() => { if ($logo.length > 0) $logo.trigger('click'); }, 200);
            setTimeout(() => { if ($header.length > 0) $header.trigger('click'); }, 300);
            setTimeout(() => { if ($activeItem.length > 0) $activeItem.trigger('click'); }, 400);
            setTimeout(() => { if ($menuCaption.length > 0) $menuCaption.trigger('click'); }, 500);

        } catch (e) { /* ignore */ }
    },

    // Áp dụng toàn bộ chủ đề
    applyFullTheme: function (settings) {
        this.setSidebarFixed(settings.isFixedSidebar);
        this.setHeaderFixed(settings.isFixedHeader);
        this.setMenuType(settings.menuType);

        setTimeout(() => {
            if (typeof $ === 'undefined') {
                setTimeout(() => this.applyFullTheme(settings), 200);
                return;
            }

            const sidebarEffectSelect = document.getElementById('vertical-menu-effect');
            if (sidebarEffectSelect) {
                sidebarEffectSelect.value = settings.sidebarEffect || 'shrink';
                sidebarEffectSelect.dispatchEvent(new Event('change'));
            }

            const borderStyleSelect = document.getElementById('vertical-border-style');
            if (borderStyleSelect) {
                borderStyleSelect.value = settings.borderStyle || 'none';
                borderStyleSelect.dispatchEvent(new Event('change'));
            }

            const dropdownIconSelect = document.getElementById('vertical-dropdown-icon');
            if (dropdownIconSelect) {
                dropdownIconSelect.value = settings.dropdownIcon || 'style1';
                dropdownIconSelect.dispatchEvent(new Event('change'));
            }

            const subItemIconSelect = document.getElementById('vertical-subitem-icon');
            if (subItemIconSelect) {
                subItemIconSelect.value = settings.subItemIcon || 'style1';
                subItemIconSelect.dispatchEvent(new Event('change'));
            }

            this.applyThemeClass('.navbar-theme', 'navbar-theme', settings.mainLayout || 'theme1');
            this.applyThemeClass('.logo-theme', 'logo-theme', settings.headerBrandColor || 'theme1');
            this.applyThemeClass('.header-theme', 'header-theme', settings.headerColor || 'theme1');
            this.applyThemeClass('.active-item-theme', 'active-item-theme', settings.activeLinkColor || 'theme1');
            this.applyThemeClass('.leftheader-theme', 'lheader-theme', settings.menuCaptionColor || 'theme5');
        }, 200);
    },

    // Áp dụng các thuộc tính chủ đề
    applyThemeClass: function (selector, attributeName, themeValue) {
        if (typeof $ === 'undefined') {
            // console.error('jQuery not available for applyThemeClass');
            return;
        }

        const elements = document.querySelectorAll(selector);
        let applied = false;

        elements.forEach(el => {
            el.classList.remove('active');
            if (el.getAttribute(attributeName) === themeValue) {
                el.classList.add('active');
                applied = true;
            }
        });

        try {
            if (selector === '.navbar-theme') {
                $('.pcoded-navbar').attr('navbar-theme', themeValue);
            } else if (selector === '.logo-theme') {
                $('.navbar-logo').attr('logo-theme', themeValue);
            } else if (selector === '.header-theme') {
                $('.pcoded-header').attr('header-theme', themeValue);
                $('.navbar-logo').attr('logo-theme', themeValue);
            } else if (selector === '.active-item-theme') {
                $('.pcoded-navbar').attr('active-item-theme', themeValue);
            } else if (selector === '.leftheader-theme') {
                $('.pcoded-navigatio-lavel').attr('menu-title-theme', themeValue);
            }
        } catch (e) {
            // console.error('Error applying theme class:', e);
        }

        if (!applied) {
            // console.warn(`Theme value "${themeValue}" not found for selector "${selector}"`);
        }
    },

    // Khởi tạo lại toàn bộ layout khi chuyển trang (Fix lỗi vỡ giao diện)
    initializeLayout: function () {
        if (typeof $ === 'undefined') return;
        try {
            if (typeof $.fn.pcodedmenu === 'function') {
                $("#pcoded").pcodedmenu({
                    verticalMenuplacement: 'left',
                    itemClick: true,
                    itemPrefixStyle: 'l2',
                    TextBoxSearch: true,
                    activeMenuitemPic: true,
                    FixedHeaderPosition: true,
                    FixedNavbarPosition: true,
                    VerticalSubMenuItemIconStyle: 'style1',
                    VerticalNavigationView: 'view1',
                    verticalSubItemClick: true,
                    TitleAttributes: "{ '" + "Title" + "': '" + "value" + "' }",
                    SubMenuAttributes: "{ '" + "SubMenu" + "': '" + "value" + "' }"
                });
            }
            if (typeof window.InitPcoded === 'function') window.InitPcoded();
            this.updateHeaderMargin();
            $('.mobile-menu').off('click').on('click', function() {
                $('#mobile-collapse').trigger('click');
            });
        } catch (e) {
            console.error('Error in initializeLayout:', e);
        }

        // Đảm bảo click sự kiện được bind lại cho nút thu nhỏ sidebar
        setTimeout(function() {
            try {
                $('#mobile-collapse').off('click').on('click', function(e) {
                    $('#pcoded').toggleClass('pcoded-m-side');
                    e.stopPropagation();
                });
                
                // Bind lại cho nút Setting Theme (bánh răng)
                $('.theme-loader').fadeOut('slow', function() {
                    $(this).remove();
                });
                
                if (typeof window.InitThemeSettings === 'function') window.InitThemeSettings();
            } catch (err) {}
        }, 500);
    }
};