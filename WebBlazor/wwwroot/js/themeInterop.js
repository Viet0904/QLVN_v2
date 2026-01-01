window.themeInterop = {
    
    // NEW: Reinitialize Bootstrap tabs (fix issue #1)
    reinitializeTabs: function() {
        if (typeof $ !== 'undefined' && $.fn.tab) {
            // Force reinitialize all tabs
            $('.nav-tabs a[data-toggle="tab"]').off('click').on('click', function(e) {
                e.preventDefault();
                $(this).tab('show');
            });
            
            // Ensure active tab is shown
            $('.nav-tabs .nav-link.active').tab('show');
            
            console.log('Tabs reinitialized');
        }
    },
    
    setSidebarFixed: function (isFixed) {
        const navbar = document.querySelector('.pcoded-navbar');
        const inner = document.querySelector('.pcoded-inner-navbar');

        if (!navbar || !inner) {
            console.warn('Navbar or inner navbar not found');
            return;
        }

        //console.log('setSidebarFixed được gọi, isFixed:', isFixed);

        if (isFixed) {
            // === FIXED SIDEBAR ===
            navbar.setAttribute('pcoded-navbar-position', 'fixed');
            navbar.style.position = 'fixed';
            navbar.style.top = ''; // Clear any top value

            inner.style.overflowY = 'auto';
            inner.style.height = 'calc(100vh - 80px)';

            //console.log('Navbar set to FIXED');
        } else {
            // === ABSOLUTE SIDEBAR ===
            navbar.setAttribute('pcoded-navbar-position', 'absolute');
            navbar.style.position = 'absolute';
            navbar.style.top = ''; // Clear any top value

            inner.style.overflowY = '';
            inner.style.height = '';

            //console.log('Navbar set to ABSOLUTE');
        }
    },

    setHeaderFixed: function (isFixed) {
        const header = document.querySelector('.pcoded-header');
        const navbar = document.querySelector('.pcoded-navbar');
        const mainContainer = document.querySelector('.pcoded-main-container');
        const pcoded = document.getElementById('pcoded');

        if (!header || !mainContainer || !pcoded) {
            //console.error('Header, mainContainer hoặc pcoded không tìm thấy!');
            return;
        }

        console.log('setHeaderFixed được gọi, isFixed:', isFixed);

        if (isFixed) {
            // === FIXED HEADER ===
            pcoded.classList.add('pcoded-fixed-header');
            header.setAttribute('pcoded-header-position', 'fixed');
            
            if (navbar) {
                navbar.setAttribute('pcoded-header-position', 'fixed');
            }
            
            // Tính chiều cao header
            let headerHeight;
            if (typeof $ !== 'undefined' && $.fn.jquery) {
                headerHeight = $(header).outerHeight();
            } else {
                headerHeight = header.offsetHeight;
            }
            
            console.log('Header height:', headerHeight);
            
            // Set margin-top cho main-container
            mainContainer.style.marginTop = headerHeight + 'px';
            //console.log('Set margin-top:', headerHeight + 'px');
            
        } else {
            // === RELATIVE HEADER ===
            pcoded.classList.remove('pcoded-fixed-header');
            header.setAttribute('pcoded-header-position', 'relative');
            
            if (navbar) {
                navbar.setAttribute('pcoded-header-position', 'relative');
            }
            
            // Reset margin-top về '0px'
            mainContainer.style.marginTop = '0px';
            //console.log('Reset margin-top về 0px');
            
            // QUAN TRỌNG: Khi header = relative, phải re-check sidebar position
            // Nếu sidebar = fixed, navbar phải là absolute khi scroll = 0
            const navbarPosition = navbar.getAttribute('pcoded-navbar-position');
            if (navbarPosition === 'fixed') {
                // Force re-apply sidebar fixed để đảm bảo navbar position đúng
                const isSidebarFixed = pcoded.classList.contains('pcoded-fixed-sidebar');
                if (isSidebarFixed) {
                    navbar.style.position = 'absolute'; // Khi header relative, navbar fixed phải là absolute
                    //console.log('Navbar position reset to absolute (sidebar fixed + header relative)');
                }
            }
        }
    },

    setMenuType: function (type) {
        const pcoded = document.getElementById('pcoded');
        if (!pcoded) return;

        pcoded.setAttribute('nav-type', type);
        pcoded.offsetHeight;
    },

    // Lấy toàn bộ theme settings hiện tại từ UI
    getCurrentTheme: function () {
        const pcoded = document.getElementById('pcoded');
        
        // Lấy giá trị từ các select boxes
        const sidebarEffect = document.getElementById('vertical-menu-effect')?.value || 'shrink';
        const borderStyle = document.getElementById('vertical-border-style')?.value || 'none';
        const dropdownIcon = document.getElementById('vertical-dropdown-icon')?.value || 'style1';
        const subItemIcon = document.getElementById('vertical-subitem-icon')?.value || 'style1';

        // QUAN TRỌNG: Lấy theme từ ATTRIBUTES của elements chính, KHÔNG phải từ buttons
        // Vì buttons có thể không có class 'active' do CSS không có style
        const navbar = document.querySelector('.pcoded-navbar');
        const logo = document.querySelector('.navbar-logo');
        const header = document.querySelector('.pcoded-header');
        const navLabel = document.querySelector('.pcoded-navigatio-lavel');

        const mainLayout = navbar?.getAttribute('navbar-theme') || 'theme1';
        const headerBrandColor = logo?.getAttribute('logo-theme') || 'theme1';
        const headerColor = header?.getAttribute('header-theme') || 'theme1';
        const activeLinkColor = navbar?.getAttribute('active-item-theme') || 'theme1';
        const menuCaptionColor = navLabel?.getAttribute('menu-title-theme') || 'theme5';

        // Lấy menu type từ pcoded attribute
        const menuType = pcoded?.getAttribute('nav-type') || 'st6';
        
        console.log('getCurrentTheme - Reading from DOM attributes:', {
            mainLayout,
            headerBrandColor,
            headerColor,
            activeLinkColor,
            menuCaptionColor
        });

        return {
            isFixedSidebar: true, // Sẽ được override từ C#
            isFixedHeader: true,  // Sẽ được override từ C#
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

    // Apply ATTRIBUTES ONLY - không cần color elements
    // FIX Issue #3: Apply colors in correct order with proper z-index
    applyThemeAttributes: function (settings) {
        console.log('Applying theme attributes:', settings);

        // Wait for jQuery
        if (typeof $ === 'undefined') {
            console.warn('jQuery not ready, retrying...');
            setTimeout(() => this.applyThemeAttributes(settings), 300);
            return;
        }

        try {
            // Apply attributes trực tiếp - KHÔNG cần color elements
            const $navbar = $('.pcoded-navbar');
            const $logo = $('.navbar-logo');
            const $header = $('.pcoded-header');
            const $navLabel = $('.pcoded-navigatio-lavel');

            // Remove old attributes first - TỪNG CÁI MỘT
            $navbar.removeAttr('navbar-theme');
            $navbar.removeAttr('active-item-theme');
            $logo.removeAttr('logo-theme');
            $header.removeAttr('header-theme');
            $navLabel.removeAttr('menu-title-theme');

            // FIX Issue #3: Apply với z-index phù hợp
            // Thứ tự: Logo -> Header -> Navbar -> Active -> Caption
            
            // 1. Logo (Header Brand) - PHẢI CÓ Z-INDEX CAO NHẤT
            $logo.attr('logo-theme', settings.headerBrandColor || 'theme1');
            $logo.css('z-index', '1030'); // Higher than header
            $logo.css('position', 'relative');
            
            // 2. Header - Z-INDEX THẤP HƠN LOGO
            $header.attr('header-theme', settings.headerColor || 'theme1');
            $header.css('z-index', '1029');
            $header.css('position', 'relative');
            
            // 3. Navbar theme
            $navbar.attr('navbar-theme', settings.mainLayout || 'theme1');
            
            // 4. Active link color
            $navbar.attr('active-item-theme', settings.activeLinkColor || 'theme1');
            
            // 5. Menu caption
            $navLabel.attr('menu-title-theme', settings.menuCaptionColor || 'theme5');

            // Apply select boxes (PHẢI ĐẶT TRƯỚC khi highlight buttons)
            const sidebarEffectSelect = document.getElementById('vertical-menu-effect');
            if (sidebarEffectSelect) {
                sidebarEffectSelect.value = settings.sidebarEffect || 'shrink';
                sidebarEffectSelect.dispatchEvent(new Event('change'));
                console.log('Applied sidebarEffect:', settings.sidebarEffect);
            }

            const borderStyleSelect = document.getElementById('vertical-border-style');
            if (borderStyleSelect) {
                borderStyleSelect.value = settings.borderStyle || 'none';
                borderStyleSelect.dispatchEvent(new Event('change'));
                console.log('Applied borderStyle:', settings.borderStyle);
            }

            const dropdownIconSelect = document.getElementById('vertical-dropdown-icon');
            if (dropdownIconSelect) {
                dropdownIconSelect.value = settings.dropdownIcon || 'style1';
                dropdownIconSelect.dispatchEvent(new Event('change'));
                console.log('Applied dropdownIcon:', settings.dropdownIcon);
            }

            const subItemIconSelect = document.getElementById('vertical-subitem-icon');
            if (subItemIconSelect) {
                subItemIconSelect.value = settings.subItemIcon || 'style1';
                subItemIconSelect.dispatchEvent(new Event('change'));
                console.log('Applied subItemIcon:', settings.subItemIcon);
            }

            // QUAN TRỌNG: Highlight các color buttons tương ứng
            this.highlightActiveColorButtons(settings);

            // Force repaint
            $navbar[0].offsetHeight;
            $logo[0].offsetHeight;
            $header[0].offsetHeight;

            console.log('Theme attributes applied:', {
                navbar: settings.mainLayout,
                logo: settings.headerBrandColor,
                header: settings.headerColor,
                activeItem: settings.activeLinkColor,
                menuCaption: settings.menuCaptionColor,
                sidebarEffect: settings.sidebarEffect,
                borderStyle: settings.borderStyle,
                dropdownIcon: settings.dropdownIcon,
                subItemIcon: settings.subItemIcon
            });

            // Verify attributes
            console.log('Verify - Logo attr:', $logo.attr('logo-theme'));
            console.log('Verify - Header attr:', $header.attr('header-theme'));
            console.log('Verify - Navbar attr:', $navbar.attr('navbar-theme'));
            
            // Verify computed styles
            if ($logo[0]) {
                const logoStyles = window.getComputedStyle($logo[0]);
                console.log('Verify - Logo background:', logoStyles.backgroundColor);
                console.log('Verify - Logo z-index:', logoStyles.zIndex);
            }
            if ($header[0]) {
                const headerStyles = window.getComputedStyle($header[0]);
                console.log('Verify - Header background:', headerStyles.backgroundColor);
                console.log('Verify - Header z-index:', headerStyles.zIndex);
            }
            if ($navbar[0]) {
                const navbarStyles = window.getComputedStyle($navbar[0]);
                console.log('Verify - Navbar background:', navbarStyles.backgroundColor);
            }
        } catch (e) {
            //console.error('Error applying theme attributes:', e);
        }
    },

    // Highlight active color buttons based on settings
    // KHÔNG dùng class 'active' vì CSS không có style cho nó
    // Thay vào đó, TRIGGER CLICK để script.js tự động apply
    highlightActiveColorButtons: function(settings) {
        try {
            console.log('Highlighting buttons with settings:', settings);
            
            // Tìm các buttons tương ứng
            const $mainLayout = $(`.navbar-theme[navbar-theme="${settings.mainLayout}"]`);
            const $logo = $(`.logo-theme[logo-theme="${settings.headerBrandColor}"]`);
            const $header = $(`.header-theme[header-theme="${settings.headerColor}"]`);
            const $activeItem = $(`.active-item-theme[active-item-theme="${settings.activeLinkColor}"]`);
            const $menuCaption = $(`.leftheader-theme[lheader-theme="${settings.menuCaptionColor}"]`);

            console.log('Found buttons:', {
                mainLayout: $mainLayout.length,
                logo: $logo.length,
                header: $header.length,
                activeItem: $activeItem.length,
                menuCaption: $menuCaption.length
            });

            // TRIGGER CLICK để script.js tự động apply màu
            // Đợi một chút giữa các click để tránh conflict
            setTimeout(() => {
                if ($mainLayout.length > 0) {
                    $mainLayout.trigger('click');
                    //console.log('Clicked mainLayout button');
                }
            }, 100);

            setTimeout(() => {
                if ($logo.length > 0) {
                    $logo.trigger('click');
                    //console.log('Clicked logo button');
                }
            }, 200);

            setTimeout(() => {
                if ($header.length > 0) {
                    $header.trigger('click');
                    //console.log('Clicked header button');
                }
            }, 300);

            setTimeout(() => {
                if ($activeItem.length > 0) {
                    $activeItem.trigger('click');
                    //console.log('Clicked activeItem button');
                }
            }, 400);

            setTimeout(() => {
                if ($menuCaption.length > 0) {
                    $menuCaption.trigger('click');
                    //console.log('Clicked menuCaption button');
                }
            }, 500);

        } catch (e) {
            //console.error('Error highlighting color buttons:', e);
        }
    },

    // Apply toàn bộ theme settings vào UI (khi SettingTheme panel mở)
    applyFullTheme: function (settings) {
        //console.log('Applying full theme:', settings);

        
        // 1. Sidebar TRƯỚC (để set navbar position)
        this.setSidebarFixed(settings.isFixedSidebar);
        
        // 2. Header SAU (để không override navbar position)
        this.setHeaderFixed(settings.isFixedHeader);
        
        // 3. Menu Type
        this.setMenuType(settings.menuType);

        // 4. Apply colors - CHỜ jQuery ready
        setTimeout(() => {
            // Check jQuery available
            if (typeof $ === 'undefined') {
                //console.warn('jQuery not loaded yet, retrying...');
                setTimeout(() => this.applyFullTheme(settings), 200);
                return;
            }

            // Apply select boxes
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

            // Apply main layout theme
            this.applyThemeClass('.navbar-theme', 'navbar-theme', settings.mainLayout || 'theme1');

            // Apply color themes
            this.applyThemeClass('.logo-theme', 'logo-theme', settings.headerBrandColor || 'theme1');
            this.applyThemeClass('.header-theme', 'header-theme', settings.headerColor || 'theme1');
            this.applyThemeClass('.active-item-theme', 'active-item-theme', settings.activeLinkColor || 'theme1');
            this.applyThemeClass('.leftheader-theme', 'lheader-theme', settings.menuCaptionColor || 'theme5');

            //console.log('Theme applied successfully');
        }, 200);
    },

    // Helper function để apply theme class
    applyThemeClass: function (selector, attributeName, themeValue) {
        // Check jQuery available
        if (typeof $ === 'undefined') {
            console.error('jQuery not available for applyThemeClass');
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
        
        // Apply theme trực tiếp vào các elements chính

        try {
            if (selector === '.navbar-theme') {
                $('.pcoded-navbar').attr('navbar-theme', themeValue);
                console.log('Applied navbar-theme:', themeValue);
            } else if (selector === '.logo-theme') {
                $('.navbar-logo').attr('logo-theme', themeValue);
                console.log('Applied logo-theme:', themeValue);
            } else if (selector === '.header-theme') {
                $('.pcoded-header').attr('header-theme', themeValue);
                $('.navbar-logo').attr('logo-theme', themeValue); // Header theme cũng apply cho logo
                console.log('Applied header-theme:', themeValue);
            } else if (selector === '.active-item-theme') {
                $('.pcoded-navbar').attr('active-item-theme', themeValue);
                console.log('Applied active-item-theme:', themeValue);
            } else if (selector === '.leftheader-theme') {
                $('.pcoded-navigatio-lavel').attr('menu-title-theme', themeValue);
                console.log('Applied menu-title-theme:', themeValue);
            }
        } catch (e) {
            console.error('Error applying theme class:', e);
        }
        
        if (!applied) {
            console.warn(`Theme value "${themeValue}" not found for selector "${selector}"`);
        }
    }
};
