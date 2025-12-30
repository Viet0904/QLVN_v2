window.dataTableInstances = {};
window.selectedUserRows = {};
window.blazorInstance = null;

// Register Blazor instance
window.registerBlazorInstance = function (instance) {
    window.blazorInstance = instance;
    console.log('✅ Blazor instance registered');
};

// ==========================================
// USER DATA TABLE - FULL FEATURES
// ==========================================
window.initUserDataTable = function (selector) {
    console.log('🚀 Initializing UserDataTable:', selector);

    if ($.fn.DataTable.isDataTable(selector)) {
        console.log('⚠️ Destroying existing DataTable');
        $(selector).DataTable().destroy();
    }

    // Clean up old elements
    $('.dt-column-dropdown').remove();
    $('.colvis-dropdown-custom').remove();
    $('.dt-custom-toolbar').remove();

    // Xóa localStorage cũ để đảm bảo mặc định là 10 khi mới vào trang
    // Chỉ lưu lại nếu user thực sự thay đổi
    var currentPageSize = localStorage.getItem('userTablePageSize');
    if (!currentPageSize || !['10', '25', '50', '100'].includes(currentPageSize)) {
        localStorage.removeItem('userTablePageSize');
    }

    var columnNames = [
        'Id', 'Nhóm', 'Tên', 'Giới tính', 'Tên đăng nhập',
        'Email', 'Điện thoại', 'CMND/CCCD', 'Địa chỉ', 'Hình ảnh',
        'Ghi chú', 'Trạng thái', 'Ngày tạo', 'Người tạo', 'Ngày cập nhật', 'Người cập nhật',
        'Hành động'
    ];

    var defaultHiddenColumns = [3, 7, 8, 9, 10, 13, 14, 15];

    const table = $(selector).DataTable({
        responsive: false,
        searching: true,
        ordering: true,
        info: false,
        paging: false,
        lengthChange: false,
        scrollY: 'calc(100vh - 320px)', // Sử dụng calc thay vì auto
        scrollX: true,
        scrollCollapse: true, //true để table co lại khi ít dữ liệu
        autoWidth: false,
        fixedColumns: false,
        deferRender: true,

        select: {
            style: 'multi',
            selector: 'td:not(:last-child)',
            info: false
        },

        pageLength: 10,
        pagingType: "full_numbers",
        order: [[0, "asc"], [1, "asc"], [2, "asc"]],

        layout: {
            topStart: null,
            topEnd: null,
            bottomStart: null,
            bottomEnd: null
        },

        language: {
            zeroRecords: "Không tìm thấy dữ liệu phù hợp",
            emptyTable: "Không có dữ liệu",
            paginate: {
                first: '«',
                last: '»',
                next: '›',
                previous: '‹'
            }
        },

        columnDefs: [
            { orderable: false, targets: -1 },
            { visible: false, targets: defaultHiddenColumns },
            { width: '80px', targets: 0 },
            { width: '120px', targets: -1 }
        ],

        drawCallback: function (settings) {
            console.log('📊 DataTable drawn');
            setTimeout(function () {
                bindAllRowEvents(selector);
            }, 50);
        },

        initComplete: function () {
            console.log('✅ DataTable initialized');
            var api = this.api();
            var wrapper = $(api.table().container());
            var totalColumns = api.columns().nodes().length;

            createCustomToolbar(api, wrapper, columnNames, totalColumns);

            // Mặc định dropdown là 10 - sẽ được sync lại bởi updateUserDataTableData
            wrapper.find('.dt-page-length').val('10');
            console.log('📊 Initial dropdown set to 10 (will be synced from server)');

            api.columns().every(function (index) {
                var column = this;
                var header = $(column.header());

                if (index === totalColumns - 1) return;
                if (header.find('.dt-column-menu').length > 0) return;

                createColumnMenu(column, header, index, api);
            });

            $(document).off('click.dtUserMenu').on('click.dtUserMenu', function (e) {
                var $target = $(e.target);

                if ($target.closest('.dt-column-dropdown').length > 0) {
                    return;
                }

                if ($target.closest('.colvis-dropdown-custom').length > 0) {
                    return;
                }

                if ($target.closest('.dt-column-menu').length > 0) {
                    return;
                }

                if ($target.closest('.colvis-btn-custom').length > 0) {
                    return;
                }

                $('.dt-column-dropdown').hide();
                $('.colvis-dropdown-custom').hide();
            });

            $(window).off('scroll.dtUserMenu');

            setTimeout(function () {
                api.columns.adjust();
            }, 150);
        }
    });

    window.dataTableInstances[selector] = table;
    window.selectedUserRows[selector] = [];

    table.on('select deselect', function (e, dt, type, indexes) {
        if (type === 'row') {
            const selectedRows = table.rows({ selected: true }).nodes();
            window.selectedUserRows[selector] = [];

            $(selectedRows).each(function () {
                const userId = $(this).data('user-id');
                if (userId) {
                    window.selectedUserRows[selector].push(userId);
                }
            });

            console.log('✅ Selected rows:', window.selectedUserRows[selector]);
        }
    });

    // RESPONSIVE: Handle window resize - chỉ cần adjust columns
    var resizeTimeout;
    $(window).off('resize.dtUserResize').on('resize.dtUserResize', function() {
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(function() {
            
            table.columns.adjust();
        }, 250);
    });

    console.log('✅ UserDataTable initialized successfully');
    return table;
};

// ==========================================
// CREATE CUSTOM TOOLBAR
// ==========================================
function createCustomToolbar(api, wrapper, columnNames, totalColumns) {
    var toolbarHtml = `
        <div class="dt-custom-toolbar" style="
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 15px 0;
            margin-bottom: 15px;
            border-bottom: 1px solid #e0e0e0;
        ">
            <div class="dt-toolbar-left" style="display: flex; gap: 15px; align-items: center;">
                <div class="colvis-wrapper" style="position: relative;">
                    <button type="button" class="btn btn-primary btn-sm colvis-btn-custom" style="
                        display: flex;
                        align-items: center;
                        gap: 8px;
                        background-color: #01a9ac;
                        border-color: #01a9ac;
                    ">
                        <i class="feather icon-columns"></i>
                        <span>Cột Hiển Thị</span>
                        <i class="feather icon-chevron-down" style="font-size: 12px;"></i>
                    </button>
                    <div class="colvis-dropdown-custom" style="
                        position: absolute;
                        top: 100%;
                        left: 0;
                        margin-top: 5px;
                        background: #ffffff;
                        border: 1px solid #ddd;
                        border-radius: 6px;
                        box-shadow: 0 4px 12px rgba(0,0,0,0.15);
                        z-index: 9999;
                        min-width: 240px;
                        max-height: 400px;
                        overflow-y: auto;
                        display: none;
                    ">
                        <div class="colvis-item colvis-show-all" style="
                            padding: 12px 16px;
                            cursor: pointer;
                            border-bottom: 2px solid #e9ecef;
                            display: flex;
                            align-items: center;
                            gap: 10px;
                            font-weight: 600;
                            color: #01a9ac;
                            background: #f8f9fa;
                        ">
                            <i class="feather icon-eye" style="color: #01a9ac;"></i>
                            <span>Hiện tất cả</span>
                        </div>
                        <div class="colvis-columns-list"></div>
                    </div>
                </div>
                <div class="dt-length-wrapper" style="display: flex; gap: 8px; align-items: center;">
                    <select class="form-select form-select-sm dt-page-length" style="width: auto;">
                        <option value="10" selected>10</option>
                        <option value="25">25</option>
                        <option value="50">50</option>
                        <option value="100">100</option>
                    </select>
                    <span style="font-size: 14px; color: #666;">bản ghi</span>
                </div>
                <button type="button" class="btn btn-success btn-sm dt-add-new-btn" id="btnAddNewUser" style="
                    display: flex;
                    align-items: center;
                    gap: 8px;
                    white-space: nowrap;
                ">
                    <i class="feather icon-plus"></i>
                    <span>Thêm Mới</span>
                </button>
            </div>
            <div class="dt-toolbar-right" style="display: flex; gap: 15px; align-items: center;">
                <div class="dt-search-wrapper" style="display: flex; gap: 8px; align-items: center;">
                    <label style="margin: 0; font-size: 14px; color: #666;">Tìm kiếm:</label>
                    <input type="search" class="form-control form-control-sm dt-custom-search" 
                        placeholder="Nhập từ khóa..." style="width: 250px;">
                </div>
            </div>
        </div>
    `;

    wrapper.prepend(toolbarHtml);

    var columnsList = wrapper.find('.colvis-columns-list');
    for (var i = 0; i < totalColumns - 1; i++) {
        var isVisible = api.column(i).visible();
        var itemHtml = `
            <div class="colvis-item colvis-column-toggle" data-column="${i}" style="
                padding: 10px 16px;
                cursor: pointer;
                display: flex;
                align-items: center;
                gap: 12px;
                transition: background 0.2s;
                background: #ffffff;
                border-bottom: 1px solid #f0f0f0;
            ">
                <span class="colvis-check" style="
                    width: 20px;
                    height: 20px;
                    border: 2px solid #01a9ac;
                    border-radius: 4px;
                    display: inline-flex;
                    align-items: center;
                    justify-content: center;
                    font-size: 14px;
                    color: #01a9ac;
                    font-weight: bold;
                    background: #ffffff;
                    flex-shrink: 0;
                ">${isVisible ? '✓' : ''}</span>
                <span class="colvis-name" style="font-size: 14px; color: #333333; font-weight: 500;">${columnNames[i]}</span>
            </div>
        `;
        columnsList.append(itemHtml);
    }

    columnsList.on('mouseenter', '.colvis-column-toggle', function () {
        $(this).css('background', '#f0f7ff');
    }).on('mouseleave', '.colvis-column-toggle', function () {
        $(this).css('background', '#ffffff');
    });

    wrapper.find('.colvis-btn-custom').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
        $('.dt-column-dropdown').hide();
        var dropdown = wrapper.find('.colvis-dropdown-custom');
        if (dropdown.is(':visible')) {
            dropdown.hide();
        } else {
            dropdown.show();
        }
    });

    wrapper.find('.colvis-column-toggle').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();

        var colIdx = $(this).data('column');
        var column = api.column(colIdx);
        var currentVisibility = column.visible();

        column.visible(!currentVisibility);

        var check = $(this).find('.colvis-check');
        check.text(!currentVisibility ? '✓' : '');

        console.log('✅ Column visibility toggled:', columnNames[colIdx], !currentVisibility);
    });

    wrapper.find('.colvis-show-all').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();

        for (var i = 0; i < totalColumns - 1; i++) {
            api.column(i).visible(true);
        }
        wrapper.find('.colvis-column-toggle .colvis-check').text('✓');
        wrapper.find('.colvis-dropdown-custom').hide();

        console.log('✅ All columns shown');
    });

    // Page length change
    var isChangingPageSize = false;
    wrapper.find('.dt-page-length').off('change').on('change', function (e) {
        e.stopPropagation();
        e.preventDefault();

        if (isChangingPageSize) {
            console.log('⚠️ Page size change in progress, skipping');
            return;
        }

        var pageSize = parseInt($(this).val(), 10);

        if (isNaN(pageSize) || pageSize <= 0) {
            pageSize = 10;
        }

        console.log('📊 Page size selected:', pageSize);

        localStorage.setItem('userTablePageSize', pageSize.toString());

        if (window.blazorInstance) {
            isChangingPageSize = true;
            console.log('📊 Calling Blazor ChangePageSize:', pageSize);
            window.blazorInstance.invokeMethodAsync('ChangePageSize', pageSize)
                .then(function () {
                    console.log('✅ Page size changed successfully to:', pageSize);
                    isChangingPageSize = false;
                })
                .catch(function (err) {
                    console.error('❌ Page size change error:', err);
                    isChangingPageSize = false;
                });
        } else {
            console.warn('⚠️ Blazor instance not found');
        }
    });

    // Search với debounce
    var searchTimeout;
    var isSearching = false;
    wrapper.find('.dt-custom-search').on('input', function (e) {
        e.stopPropagation();
        var searchValue = $(this).val();

        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(function () {
            if (isSearching) {
                console.log('⚠️ Search in progress, skipping');
                return;
            }

            if (window.blazorInstance) {
                isSearching = true;
                console.log('🔍 Searching:', searchValue);
                window.blazorInstance.invokeMethodAsync('SearchUsers', searchValue || '')
                    .then(function () {
                        console.log('✅ Search completed');
                        isSearching = false;
                    })
                    .catch(function (err) {
                        console.error('❌ Search error:', err);
                        isSearching = false;
                    });
            }
        }, 500);
    });

    // Add new button
    wrapper.find('#btnAddNewUser').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (window.blazorInstance) {
            window.blazorInstance.invokeMethodAsync('OpenAddModal')
                .then(function () { console.log('✅ Add modal opened'); })
                .catch(function (err) { console.error('❌ Add modal error:', err); });
        }
    });
}

// ==========================================
// CREATE COLUMN MENU - THÊM FILTER VỚI NHIỀU KIỂU
// ==========================================
function createColumnMenu(column, header, index, api) {
    // Sử dụng Unicode ☰ thay vì feather icon để đảm bảo hiển thị
    var menuBtn = $('<span class="dt-column-menu" title="Menu">☰</span>');

    // Khởi tạo dropdown 
    var dropdown = $(`
    <div class="dt-column-dropdown" data-column-index="${index}">
        <div class="dt-dropdown-item dt-sort-asc">
            <i class="feather icon-arrow-up"></i>
            <span>Sắp xếp tăng dần</span>
        </div>
        <div class="dt-dropdown-item dt-sort-desc">
            <i class="feather icon-arrow-down"></i>
            <span>Sắp xếp giảm dần</span>
        </div>
        <div class="dt-dropdown-divider"></div>
        <div class="dt-dropdown-section">
            <label class="dt-dropdown-label">Kiểu lọc</label>
            <select class="dt-filter-type form-control form-control-sm">
                <option value="contains">Chứa</option>
                <option value="equals">Bằng</option>
                <option value="startswith">Bắt đầu bằng</option>
                <option value="endswith">Kết thúc bằng</option>
            </select>
        </div>
        <div class="dt-dropdown-filter">
            <input type="text" class="dt-filter-input form-control form-control-sm" placeholder="Nhập giá trị lọc...">
        </div>
        <div class="dt-dropdown-item dt-clear-filter">
            <i class="feather icon-x-circle"></i>
            <span>Xóa bộ lọc</span>
        </div>
        <div class="dt-dropdown-divider"></div>
        <div class="dt-dropdown-item dt-hide-column">
            <i class="feather icon-eye-off"></i>
            <span>Ẩn cột này</span>
        </div>
    </div>
`); 

    header.append(menuBtn);
    $('body').append(dropdown);

    dropdown.find('.dt-dropdown-item').on('mouseenter', function () {
        $(this).css('background', '#f5f5f5');
    }).on('mouseleave', function () {
        $(this).css('background', 'white');
    });

    function positionDropdown() {
        var btnOffset = menuBtn.offset();
        var btnHeight = menuBtn.outerHeight();
        var dropdownWidth = dropdown.outerWidth();
        var left = btnOffset.left - dropdownWidth + menuBtn.outerWidth();

        dropdown.css({
            top: btnOffset.top + btnHeight + 5,
            left: Math.max(10, left),
            display: 'block'
        });
    }

    menuBtn.on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();

        $('.dt-column-dropdown').not(dropdown).hide();
        $('.colvis-dropdown-custom').hide();

        if (dropdown.is(':visible')) {
            dropdown.hide();
        } else {
            positionDropdown();
        }
    });

    dropdown.find('.dt-sort-asc').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
        column.order('asc').draw(false);
        dropdown.hide();
        console.log('✅ Sorted ascending');
    });

    dropdown.find('.dt-sort-desc').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
        column.order('desc').draw(false);
        dropdown.hide();
        console.log('✅ Sorted descending');
    });

    // FILTER INPUT - Không đóng dropdown khi typing
    var filterInput = dropdown.find('.dt-filter-input');
    var filterType = dropdown.find('.dt-filter-type');
    
    filterInput.on('click', function(e) {
        e.stopPropagation();
    });

    filterType.on('click change', function(e) {
        e.stopPropagation();
    });

    // Apply filter với regex dựa trên loại
    function applyFilter() {
        var searchValue = filterInput.val();
        var type = filterType.val();
        
        if (!searchValue) {
            column.search('').draw();
            return;
        }

        var regex;
        switch(type) {
            case 'equals':
                regex = '^' + $.fn.dataTable.util.escapeRegex(searchValue) + '$';
                break;
            case 'startswith':
                regex = '^' + $.fn.dataTable.util.escapeRegex(searchValue);
                break;
            case 'endswith':
                regex = $.fn.dataTable.util.escapeRegex(searchValue) + '$';
                break;
            case 'contains':
            default:
                regex = $.fn.dataTable.util.escapeRegex(searchValue);
                break;
        }

        column.search(regex, true, false).draw();
        console.log('🔍 Filtering column', index, 'with type:', type, 'value:', searchValue);
    }

    filterInput.on('keyup', function(e) {
        e.stopPropagation();
        applyFilter();
    });

    filterType.on('change', function(e) {
        e.stopPropagation();
        applyFilter();
    });

    // CLEAR FILTER
    dropdown.find('.dt-clear-filter').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
        filterInput.val('');
        filterType.val('contains');
        column.search('').draw();
        console.log('✅ Filter cleared for column', index);
    });

    dropdown.find('.dt-hide-column').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
        column.visible(false);
        dropdown.hide();
        console.log('✅ Column hidden');
    });

    dropdown.on('click mousedown', function(e) {
        e.stopPropagation();
    });
}

// ==========================================
// BIND ROW EVENTS
// ==========================================
function bindAllRowEvents(selector) {
    var $table = $(selector);
    if (!$table.length) return;

    console.log('🔗 Binding row events for:', selector);

    $table.find('tbody tr').each(function () {
        bindRowActionEvents(this);
    });
}

function bindRowActionEvents(rowNode) {
    if (!rowNode) return;

    var $row = $(rowNode);

    // Unbind trước để tránh duplicate
    $row.find('.btn-edit-user').off('click');
    $row.find('.btn-delete-user').off('click');

    $row.find('.btn-edit-user').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var userId = $(this).data('user-id') || $row.data('user-id');
        if (userId && window.blazorInstance) {
            console.log('✏️ Edit user:', userId);
            window.blazorInstance.invokeMethodAsync('OpenEditModalById', userId.toString())
                .catch(function (err) { console.error('❌ Edit error:', err); });
        } else {
            console.warn('⚠️ Cannot edit: userId or blazorInstance missing', userId, !!window.blazorInstance);
        }
    });

    $row.find('.btn-delete-user').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var userId = $(this).data('user-id') || $row.data('user-id');
        if (userId && window.blazorInstance) {
            console.log('🗑️ Delete user:', userId);
            window.blazorInstance.invokeMethodAsync('OpenDeleteModalById', userId.toString())
                .catch(function (err) { console.error('❌ Delete error:', err); });
        } else {
            console.warn('⚠️ Cannot delete: userId or blazorInstance missing', userId, !!window.blazorInstance);
        }
    });
}

// ==========================================
// DESTROY
// ==========================================
window.destroyUserDataTable = function (selector) {
    console.log('🗑️ Destroying UserDataTable:', selector);

    if (window.dataTableInstances[selector]) {
        $(document).off('click.dtUserMenu');
        $(window).off('scroll.dtUserMenu resize.dtUserMenu resize.dtUserResize');
        $('.dt-column-dropdown').remove();
        $('.colvis-dropdown-custom').remove();
        $('.dt-custom-toolbar').remove();

        window.dataTableInstances[selector].destroy();
        delete window.dataTableInstances[selector];
        delete window.selectedUserRows[selector];

        console.log('✅ DataTable destroyed');
    }
};

window.reinitUserDataTable = function (selector) {
    window.destroyUserDataTable(selector);
    window.initUserDataTable(selector);
};

// ==========================================
// HELPER FUNCTIONS
// ==========================================
window.getSelectedUserIds = function (selector) {
    return window.selectedUserRows[selector] || [];
};

window.clearTableSelection = function (selector) {
    if (window.dataTableInstances[selector]) {
        window.dataTableInstances[selector].rows().deselect();
        window.selectedUserRows[selector] = [];
    }
};

// ==========================================
// UPDATE DATA - Core function - FIXED cho search empty
// ==========================================
window.updateUserDataTableData = function (selector, paginatedData) {
    var table = window.dataTableInstances[selector];
    if (!table) {
        console.warn('⚠️ DataTable not found for selector:', selector);
        return;
    }

    try {
        var itemCount = paginatedData?.items?.length || 0;
        var pageSize = paginatedData?.pageSize || 10;
        console.log('📊 Updating DataTable with', itemCount, 'items, pageSize:', pageSize);

        // Sync dropdown với pageSize từ server
        var wrapper = $(table.table().container());
        var dropdown = wrapper.find('.dt-page-length');
        if (dropdown.length > 0 && dropdown.val() != pageSize.toString()) {
            dropdown.val(pageSize.toString());
            console.log('📊 Synced dropdown to:', pageSize);
        }

        // Lưu scroll position
        var scrollBody = $(selector).closest('.dt-scroll').find('.dt-scroll-body');
        var scrollTop = scrollBody.scrollTop();
        var scrollLeft = scrollBody.scrollLeft();

        // Clear data hiện tại
        table.clear();

        if (paginatedData && paginatedData.items && paginatedData.items.length > 0) {
            paginatedData.items.forEach(function (user) {
                // Xử lý cả camelCase và PascalCase từ JSON serialization
                var userId = user.id || user.Id || '';
                var groupId = user.groupId || user.GroupId || '';
                var name = user.name || user.Name || '';
                var gender = user.gender !== undefined ? user.gender : (user.Gender !== undefined ? user.Gender : null);
                var userName = user.userName || user.UserName || '';
                var email = user.email || user.Email || '';
                var phone = user.phone || user.Phone || '';
                var cmnd = user.cmnd || user.Cmnd || '';
                var address = user.address || user.Address || '';
                var image = user.image || user.Image || '';
                var note = user.note || user.Note || '';
                var rowStatus = user.rowStatus !== undefined ? user.rowStatus : (user.RowStatus !== undefined ? user.RowStatus : 1);
                var createdAt = user.createdAt || user.CreatedAt || '';
                var createdBy = user.createdBy || user.CreatedBy || '';
                var updatedAt = user.updatedAt || user.UpdatedAt || '';
                var updatedBy = user.updatedBy || user.UpdatedBy || '';
                
                var rowData = [
                    userId,
                    groupId,
                    name,
                    getUserGenderBadge(gender),
                    userName,
                    email,
                    phone,
                    cmnd,
                    address,
                    getUserImageHtml(image),
                    note,
                    getUserStatusBadge(rowStatus),
                    formatDateTime(createdAt),
                    createdBy,
                    formatDateTime(updatedAt),
                    updatedBy,
                    getUserActionButtons(userId)
                ];

                // Add row và set data-user-id
                var rowNode = table.row.add(rowData).node();
                if (rowNode && userId) {
                    $(rowNode).attr('data-user-id', userId);
                }
            });
        }

        // Draw table - false để giữ vị trí
        table.draw(false);

        // Restore scroll và bind events
        setTimeout(function () {
            scrollBody.scrollTop(scrollTop);
            scrollBody.scrollLeft(scrollLeft);

            // Bind events SAU khi draw xong
            bindAllRowEvents(selector);
            
            console.log('✅ DataTable data updated, items:', itemCount);
        }, 100);

    } catch (error) {
        console.error('❌ Error updating DataTable data:', error);
    }
};

// ==========================================
// ROW ANIMATIONS - FAST VERSION - tìm row ngay trong DataTable
// ==========================================
window.addUserRowSmooth = function (selector, userId) {
    console.log('🎬 addUserRowSmooth called for:', userId);
    
    var targetUserId = String(userId);
    var table = window.dataTableInstances[selector];
    
    if (!table) {
        console.warn('⚠️ DataTable not found');
        return;
    }
    
    // Tìm row trực tiếp trong DataTable API - nhanh hơn DOM selector
    var foundRow = null;
    table.rows().every(function() {
        var rowNode = this.node();
        if (rowNode) {
            var rowId = $(rowNode).attr('data-user-id');
            // So sánh với cột đầu tiên (ID) nếu không có attr
            if (!rowId) {
                var rowData = this.data();
                if (rowData && rowData[0] == targetUserId) {
                    $(rowNode).attr('data-user-id', targetUserId);
                    rowId = targetUserId;
                }
            }
            if (rowId == targetUserId) {
                foundRow = $(rowNode);
                return false; // break
            }
        }
    });
    
    if (foundRow && foundRow.length > 0) {
        console.log('✅ Found row for ADD animation immediately');
        applyAddHighlight(foundRow, selector);
    } else {
        // Fallback: thử tìm bằng DOM sau 50ms
        setTimeout(function() {
            var $row = $(selector).find('tbody tr').filter(function() {
                return $(this).find('td:first').text().trim() == targetUserId ||
                       $(this).attr('data-user-id') == targetUserId;
            });
            if ($row.length > 0) {
                $row.attr('data-user-id', targetUserId);
                applyAddHighlight($row, selector);
            } else {
                console.warn('⚠️ Row not found for ADD userId:', targetUserId);
            }
        }, 50);
    }
};

window.updateUserRowSmooth = function (selector, userId) {
    console.log('🎬 updateUserRowSmooth called for:', userId);
    
    var targetUserId = String(userId);
    var table = window.dataTableInstances[selector];
    
    if (!table) {
        console.warn('⚠️ DataTable not found');
        return;
    }
    
    // Tìm row trực tiếp trong DataTable API
    var foundRow = null;
    table.rows().every(function() {
        var rowNode = this.node();
        if (rowNode) {
            var rowId = $(rowNode).attr('data-user-id');
            if (!rowId) {
                var rowData = this.data();
                if (rowData && rowData[0] == targetUserId) {
                    $(rowNode).attr('data-user-id', targetUserId);
                    rowId = targetUserId;
                }
            }
            if (rowId == targetUserId) {
                foundRow = $(rowNode);
                return false;
            }
        }
    });
    
    if (foundRow && foundRow.length > 0) {
        console.log('✅ Found row for UPDATE animation immediately');
        applyUpdateHighlight(foundRow, selector);
    } else {
        // Fallback
        setTimeout(function() {
            var $row = $(selector).find('tbody tr').filter(function() {
                return $(this).find('td:first').text().trim() == targetUserId ||
                       $(this).attr('data-user-id') == targetUserId;
            });
            if ($row.length > 0) {
                $row.attr('data-user-id', targetUserId);
                applyUpdateHighlight($row, selector);
            } else {
                console.warn('⚠️ Row not found for UPDATE userId:', targetUserId);
            }
        }, 50);
    }
};

window.deleteUserRowSmooth = function(selector, userId) {
    console.log('🎬 deleteUserRowSmooth called for:', userId);
    
    var targetUserId = String(userId);
    
    // Tìm bằng attr hoặc text trong cột đầu tiên
    var $row = $(selector).find('tbody tr').filter(function() {
        return $(this).attr('data-user-id') == targetUserId ||
               $(this).find('td:first').text().trim() == targetUserId;
    });
    
    if ($row.length > 0) {
        console.log('✅ Animating delete for userId:', targetUserId);
        
        $row.css({
            'transition': 'all 0.4s ease-out',
            'position': 'relative',
            'z-index': '10',
            'box-shadow': '0 0 15px rgba(220, 53, 69, 0.5)'
        });
        $row.find('td').css({
            'transition': 'background-color 0.3s ease-out',
            'background-color': '#f8d7da'
        });
        
        setTimeout(function() {
            $row.css({
                'opacity': '0.5',
                'transform': 'scale(0.98) translateX(10px)'
            });
        }, 150);
        
        setTimeout(function() {
            $row.css({
                'opacity': '0.2',
                'transform': 'scale(0.95) translateX(20px)'
            });
        }, 300);
    } else {
        console.warn('⚠️ Row not found for delete userId:', targetUserId);
    }
};

// Helper function cho ADD highlight
function applyAddHighlight($row, selector) {
    // Scroll đến row
    var scrollBody = $(selector).closest('.dt-scroll').find('.dt-scroll-body');
    if (scrollBody.length > 0) {
        var rowOffset = $row.position().top;
        var scrollHeight = scrollBody.height();
        if (rowOffset > scrollHeight || rowOffset < 0) {
            scrollBody.animate({ scrollTop: scrollBody.scrollTop() + rowOffset - 50 }, 300);
        }
    }
    
    // Reset trước
    $row.css({
        'transition': 'none',
        'background-color': '',
        'box-shadow': '',
        'transform': ''
    });
    $row.find('td').css({
        'transition': 'none',
        'background-color': ''
    });
    
    // Force reflow
    $row[0].offsetHeight;
    
    // Apply highlight effect - màu xanh lá cho add
    $row.css({
        'transition': 'all 0.3s ease-in-out',
        'position': 'relative',
        'z-index': '10',
        'box-shadow': '0 0 20px rgba(40, 167, 69, 0.6)'
    });
    $row.find('td').css({
        'transition': 'background-color 0.3s ease-in-out',
        'background-color': '#d4edda'
    });
    
    // Pulse animation
    var pulseCount = 0;
    var pulseInterval = setInterval(function() {
        pulseCount++;
        if (pulseCount % 2 === 0) {
            $row.find('td').css('background-color', '#d4edda');
        } else {
            $row.find('td').css('background-color', '#c3e6cb');
        }
        if (pulseCount >= 4) {
            clearInterval(pulseInterval);
            $row.find('td').css('background-color', '#d4edda');
        }
    }, 300);
    
    // Remove effect after delay
    setTimeout(function () {
        $row.css({
            'transition': 'all 0.5s ease-out',
            'box-shadow': 'none',
            'position': '',
            'z-index': ''
        });
        $row.find('td').css({
            'transition': 'background-color 0.5s ease-out',
            'background-color': ''
        });
    }, 2500);
}

// Helper function cho UPDATE highlight  
function applyUpdateHighlight($row, selector) {
    // Scroll đến row
    var scrollBody = $(selector).closest('.dt-scroll').find('.dt-scroll-body');
    if (scrollBody.length > 0) {
        var rowOffset = $row.position().top;
        var scrollHeight = scrollBody.height();
        if (rowOffset > scrollHeight || rowOffset < 0) {
            scrollBody.animate({ scrollTop: scrollBody.scrollTop() + rowOffset - 50 }, 300);
        }
    }
    
    // Reset trước
    $row.css({
        'transition': 'none',
        'background-color': '',
        'box-shadow': '',
        'transform': ''
    });
    $row.find('td').css({
        'transition': 'none',
        'background-color': ''
    });
    
    // Force reflow
    $row[0].offsetHeight;
    
    // Apply highlight effect - màu vàng cho update
    $row.css({
        'transition': 'all 0.3s ease-in-out',
        'position': 'relative',
        'z-index': '10',
        'box-shadow': '0 0 20px rgba(255, 193, 7, 0.6)'
    });
    $row.find('td').css({
        'transition': 'background-color 0.3s ease-in-out',
        'background-color': '#fff3cd'
    });
    
    // Pulse animation
    var pulseCount = 0;
    var pulseInterval = setInterval(function() {
        pulseCount++;
        if (pulseCount % 2 === 0) {
            $row.find('td').css('background-color', '#fff3cd');
        } else {
            $row.find('td').css('background-color', '#ffeeba');
        }
        if (pulseCount >= 4) {
            clearInterval(pulseInterval);
            $row.find('td').css('background-color', '#fff3cd');
        }
    }, 250);
    
    // Remove effect after delay
    setTimeout(function () {
        $row.css({
            'transition': 'all 0.5s ease-out',
            'box-shadow': 'none',
            'position': '',
            'z-index': ''
        });
        $row.find('td').css({
            'transition': 'background-color 0.5s ease-out',
            'background-color': ''
        });
    }, 2000);
}

// ==========================================
// HELPER FUNCTIONS FOR ROW RENDERING
// ==========================================
function getUserGenderBadge(gender) {
    if (gender === 1) {
        return '<span class="badge bg-primary">Nam</span>';
    } else if (gender === 0) {
        return '<span class="badge bg-info">Nữ</span>';
    } else {
        return '<span class="badge bg-secondary">Không xác định</span>';
    }
}

function getUserStatusBadge(rowStatus) {
    if (rowStatus === 1) {
        return '<span class="badge bg-success">Hoạt động</span>';
    } else {
        return '<span class="badge bg-danger">Ngừng hoạt động</span>';
    }
}

function getUserImageHtml(image) {
    if (image) {
        return '<img src="' + image + '" alt="Avatar" style="width: 40px; height: 40px; border-radius: 50%;" />';
    } else {
        return '<span class="text-muted">-</span>';
    }
}

function getUserActionButtons(userId) {
    return '<button class="btn btn-sm btn-warning me-1 btn-edit-user" data-user-id="' + userId + '" title="Chỉnh sửa">' +
        '<i class="feather icon-edit"></i></button>' +
        '<button class="btn btn-sm btn-danger btn-delete-user" data-user-id="' + userId + '" title="Xóa">' +
        '<i class="feather icon-trash-2"></i></button>';
}

function formatDateTime(dateString) {
    if (!dateString) return '';

    try {
        var date = new Date(dateString);
        if (isNaN(date.getTime())) return '';

        var day = ('0' + date.getDate()).slice(-2);
        var month = ('0' + (date.getMonth() + 1)).slice(-2);
        var year = date.getFullYear();
        var hours = ('0' + date.getHours()).slice(-2);
        var minutes = ('0' + date.getMinutes()).slice(-2);

        return day + '/' + month + '/' + year + ' ' + hours + ':' + minutes;
    } catch (e) {
        return '';
    }
}