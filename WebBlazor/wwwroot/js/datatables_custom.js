window.dataTableInstances = {};
window.dataTableConfigs = {};
window.selectedRows = {};
window.blazorInstance = null;

// Khởi tạo DataTable chung cho tất cả các trang
window.initGenericDataTable = function (selector, config) {
    // console.log('🚀 Initializing GenericDataTable:', selector, config);
    if ($.fn.DataTable.isDataTable(selector)) {
        $(selector).DataTable().destroy();
    }

    config = config || {};
    var columnNames = config.columnNames || [];
    var scrollY = config.scrollY || 'calc(100vh - 320px)';
    
    // Xóa các dropdown cũ liên quan đến selector này
    $('.dt-column-dropdown[data-table="' + selector + '"]').remove();

    const tableOptions = {
        responsive: false,
        searching: config.searching !== undefined ? config.searching : true,
        ordering: true,
        info: config.info !== undefined ? config.info : true, // Bật info để hiện số bản ghi khi dùng server-side
        paging: config.paging !== undefined ? config.paging : true, // Bật paging mặc định của DataTables
        lengthChange: false,
        scrollY: scrollY,
        scrollX: true,
        scrollCollapse: true,
        autoWidth: false,
        deferRender: true, // Chỉ render những hàng thực sự hiển thị
        serverSide: config.serverSide || false,
        processing: config.serverSide || false,
        order: config.defaultOrder || [[0, "asc"]],
        // Tối ưu hóa render
        pageLength: config.pageLength || 10,
        pageLength: config.pageLength || 10,
        dom: config.dom || 'rtip', // Rút gọn DOM để render nhanh hơn nếu cần
        layout: { topStart: null, topEnd: null, bottomStart: null, bottomEnd: null },
        language: {
            zeroRecords: "Không tìm thấy dữ liệu phù hợp",
            emptyTable: "Không có dữ liệu",
            processing: '<div class="spinner-border text-primary" role="status"><span class="visually-hidden">Loading...</span></div>',
            info: "Hiển thị từ _START_ đến _END_ trong _TOTAL_ bản ghi",
            infoEmpty: "Hiển thị 0 đến 0 trong 0 bản ghi",
            infoFiltered: "(lọc từ _MAX_ bản ghi)",
            paginate: { first: '«', last: '»', next: '›', previous: '‹' }
        },
        columnDefs: config.columnDefs || [
            { orderable: false, targets: -1 }
        ],
        drawCallback: function (settings) {
            setTimeout(function () {
                if (config.bindEvents) {
                    window[config.bindEvents](selector);
                }
            }, 50);
        },
        initComplete: function () {
            var api = this.api();
            var wrapper = $(api.table().container());
            var totalColumns = api.columns().nodes().length;

            createCustomToolbar(api, wrapper, columnNames, totalColumns, config.addBtnId);

            api.columns().every(function (index) {
                var column = this;
                var header = $(column.header());
                if (index === totalColumns - 1) return;
                createColumnMenu(column, header, index, api, selector);
            });

            setTimeout(function () { api.columns.adjust(); }, 150);
        }
    };

    // Nếu cấu hình serverSide, xử lý ajax callback qua Blazor
    if (config.serverSide) {
        tableOptions.ajax = function (data, callback, settings) {
            if (window.blazorInstance) {
                // console.log('📡 DataTables requesting server-side data:', data);
                window.blazorInstance.invokeMethodAsync('LoadServerSideData', data)
                    .then(result => {
                        // console.log('✅ Server-side data received:', result);
                        // DataTables expect: { draw, recordsTotal, recordsFiltered, data }
                        // My C# PaginatedResponse provides: RecordsTotal, RecordsFiltered, Data (via Items)
                        
                        // Chuyển đổi format dữ liệu nếu cần
                        var mappedData = [];
                        if (result && result.data) {
                            var columns = config.columns || [];
                            result.data.forEach(function (item) {
                                var rowData = [];
                                columns.forEach(function(col) {
                                    var val = item[col];
                                    if (val === undefined) {
                                        var lower = col.charAt(0).toLowerCase() + col.slice(1);
                                        val = item[lower];
                                    }
                                    if (val === undefined) {
                                        var upper = col.charAt(0).toUpperCase() + col.slice(1);
                                        val = item[upper];
                                    }
                                    
                                    // Xử lý renderers
                                    if (config.columnRenderers && config.columnRenderers[col]) {
                                        val = window[config.columnRenderers[col]](val, item);
                                    } else if (col.toLowerCase() === 'rowstatus') {
                                        val = (val === 1 || val === "1") 
                                            ? '<span class="badge bg-success">Hoạt động</span>' 
                                            : '<span class="badge bg-danger">Ngừng hoạt động</span>';
                                    } else if (col.toLowerCase().includes('date') || col.toLowerCase().includes('at')) {
                                        val = formatDateTime(val);
                                    }
                                    rowData.push(val === undefined || val === null ? "" : val);
                                });

                                if (config.actionRenderer) {
                                    rowData.push(window[config.actionRenderer](item.id || item.Id || item));
                                }
                                mappedData.push(rowData);
                            });
                        }

                        callback({
                            draw: data.draw,
                            recordsTotal: result.recordsTotal || 0,
                            recordsFiltered: result.recordsFiltered || 0,
                            data: mappedData
                        });

                        // Highlight rows if needed after redraw
                        setTimeout(() => {
                            if (config.idField) {
                                var $rows = $(selector).find('tbody tr');
                                result.data.forEach((item, idx) => {
                                    var id = item[config.idField] || item[config.idField.charAt(0).toLowerCase() + config.idField.slice(1)];
                                    $($rows[idx]).attr('data-id', id);
                                    if (config.idDataAttr) $($rows[idx]).attr(config.idDataAttr, id);
                                });
                            }
                        }, 50);
                    })
                    .catch(err => {
                        console.error('❌ Error loading server-side data:', err);
                        callback({
                            draw: data.draw,
                            recordsTotal: 0,
                            recordsFiltered: 0,
                            data: []
                        });
                    });
            } else {
                console.warn('⚠️ Blazor instance not registered for server-side pagination');
                callback({ draw: data.draw, recordsTotal: 0, recordsFiltered: 0, data: [] });
            }
        };
    }

    const table = $(selector).DataTable(tableOptions);

    window.dataTableInstances[selector] = table;
    window.dataTableConfigs[selector] = config;
    
    // Sidebar Observer để auto resize
    setupSidebarToggleObserver(selector);

    return table;
};

// Cập nhật dữ liệu cho Generic DataTable
window.updateGenericDataTableData = function (selector, paginatedData) {
    console.time('DataTableRender:' + selector);
    var table = window.dataTableInstances[selector];
    if (!table) return;

    try {
        var config = window.dataTableConfigs[selector] || {};
        var pageSize = paginatedData?.pageSize || 10;
        var wrapper = $(table.table().container());
        
        // Sync page length dropdown
        var dropdown = wrapper.find('.dt-page-length');
        if (dropdown.length > 0) dropdown.val(pageSize.toString());

        table.clear();
        
        if (paginatedData && paginatedData.items) {
            var columns = config.columns || [];
            paginatedData.items.forEach(function (item) {
                var rowData = [];
                columns.forEach(function(col) {
                    var val = item[col];
                    if (val === undefined) {
                        var lower = col.charAt(0).toLowerCase() + col.slice(1);
                        val = item[lower];
                    }
                    if (val === undefined) {
                        var upper = col.charAt(0).toUpperCase() + col.slice(1);
                        val = item[upper];
                    }
                    
                    // Xử lý các kiểu hiển thị đặc biệt
                    if (config.columnRenderers && config.columnRenderers[col]) {
                        val = window[config.columnRenderers[col]](val, item);
                    } else if (col.toLowerCase() === 'rowstatus') {
                        val = (val === 1 || val === "1") 
                            ? '<span class="badge bg-success">Hoạt động</span>' 
                            : '<span class="badge bg-danger">Ngừng hoạt động</span>';
                    } else if (col.toLowerCase().includes('date') || col.toLowerCase().includes('at')) {
                        val = formatDateTime(val);
                    }
                    
                    rowData.push(val === undefined || val === null ? "" : val);
                });

                // Add Actions column if configured
                if (config.actionRenderer) {
                    // FIX: Pass ID directly, not the whole object, to avoid [object Object] in string concatenation
                    var itemId = item.id || item.Id || (item[config.idField] ? item[config.idField] : null);
                    if (!itemId && config.idField) {
                         itemId = item[config.idField.charAt(0).toLowerCase() + config.idField.slice(1)];
                    }
                    rowData.push(window[config.actionRenderer](itemId));
                }

                var rowNode = table.row.add(rowData).node();
                if (rowNode && config.idField) {
                    var id = item[config.idField] || item[config.idField.charAt(0).toLowerCase() + config.idField.slice(1)];
                    $(rowNode).attr('data-id', id);
                    if (config.idDataAttr) $(rowNode).attr(config.idDataAttr, id);
                }
            });
        }

        table.draw(false);
        // FIX: Adjust columns to prevent header misalignment
        setTimeout(function() {
            table.columns.adjust();
        }, 50);
        if (config.bindEvents) {
            setTimeout(() => window[config.bindEvents](selector), 100);
        }
    } catch (e) {
        console.error('Error updating generic table:', e);
    } finally {
        console.timeEnd('DataTableRender:' + selector);
    }
};

// Sidebar Observer để tự động điều chỉnh kích thước bảng khi đóng/mở menu
function setupSidebarToggleObserver(selector) {
    try {
        const pcoded = document.getElementById('pcoded');
        if (!pcoded || !window.MutationObserver) return;

        const observer = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.type === 'attributes') {
                    // Khi có sự thay đổi về attribute (thường là pcoded-device-type hoặc các class toggle)
                    setTimeout(function () {
                        const table = window.dataTableInstances[selector];
                        if (table) {
                            table.columns.adjust().draw(false);
                        }
                    }, 300); // Đợi menu animation hoàn tất
                }
            });
        });

        observer.observe(pcoded, {
            attributes: true,
            attributeFilter: ['class', 'pcoded-device-type', 'vertical-nav-type']
        });
    } catch (e) {
        console.warn('Sidebar observer error:', e);
    }
}

// Đăng ký instance của Blazor để giao tiếp với server Blazor mà không cần refresh lại trang 
window.registerBlazorInstance = function (instance) {
    window.blazorInstance = instance;
};

// ==========================================
// USER DATA TABLE - FULL FEATURES
// ==========================================


// ==========================================
// CREATE CUSTOM TOOLBAR
// ==========================================

// Tạo custom toolbar
function createCustomToolbar(api, wrapper, columnNames, totalColumns, addBtnId) {
    var addBtnHtml = addBtnId ? `
        <button type="button" class="btn btn-success btn-sm dt-add-new-btn" id="${addBtnId}" style="
            display: flex;
            align-items: center;
            gap: 8px;
            white-space: nowrap;
        ">
            <i class="feather icon-plus"></i>
            <span>Thêm Mới</span>
        </button>
    ` : '';

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
                ${addBtnHtml}
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
    // Thêm toolbar vào đầu DataTable
    wrapper.prepend(toolbarHtml);

    // Lấy danh sách các cột
    var columnsList = wrapper.find('.colvis-columns-list');
    // Lặp qua các cột
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
    // Cấu hình sự kiện hover cho các menu
    columnsList.on('mouseenter', '.colvis-column-toggle', function () {
        $(this).css('background', '#f0f7ff');
    }).on('mouseleave', '.colvis-column-toggle', function () {
        $(this).css('background', '#ffffff');
    });
    // Cấu hình sự kiện click cho nút Cột Hiển Thị
    wrapper.find('.colvis-btn-custom').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
        // Ẩn các dropdown khác
        $('.dt-column-dropdown').hide();
        var dropdown = wrapper.find('.colvis-dropdown-custom');
        if (dropdown.is(':visible')) {
            dropdown.hide();
        } else {
            dropdown.show();
        }
    });
    // Cấu hình sự kiện click cho các menu
    wrapper.find('.colvis-column-toggle').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();

        var colIdx = $(this).data('column');
        var column = api.column(colIdx);
        var currentVisibility = column.visible();

        column.visible(!currentVisibility);

        var check = $(this).find('.colvis-check');
        check.text(!currentVisibility ? '✓' : '');

        // console.log('✅ Column visibility toggled:', columnNames[colIdx], !currentVisibility);
    });
    // Cấu hình sự kiện click cho nút Hiển Tất Cả
    wrapper.find('.colvis-show-all').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();

        for (var i = 0; i < totalColumns - 1; i++) {
            api.column(i).visible(true);
        }
        wrapper.find('.colvis-column-toggle .colvis-check').text('✓');
        wrapper.find('.colvis-dropdown-custom').hide();

        // console.log('✅ All columns shown');
    });

    // Cấu hình sự kiện thay đổi số lượng bản ghi
    wrapper.find('.dt-page-length').off('change').on('change', function (e) {
        var pageSize = parseInt($(this).val(), 10) || 10;
        localStorage.setItem('userTablePageSize', pageSize.toString());
        
        if (window.blazorInstance) {
            window.blazorInstance.invokeMethodAsync('ChangePageSize', pageSize);
        } else if (api.page.len) {
            api.page.len(pageSize).draw();
        }
    });

    // Cấu hình sự kiện input cho nút tìm kiếm
    var searchTimeout;
    wrapper.find('.dt-custom-search').off('input').on('input', function (e) {
        var searchValue = $(this).val();
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(function () {
            if (window.blazorInstance) {
                // Ưu tiên gọi Blazor để search server-side (toàn cục 50k bản ghi)
                window.blazorInstance.invokeMethodAsync('SearchUsers', searchValue);
            } else {
                api.search(searchValue).draw();
            }
        }, 500);
    });

    // Cấu hình sự kiện click cho nút Thêm Mới
    wrapper.find('#btnAddNewUser').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        if (window.blazorInstance) {
            var eventName = config.addEvent || 'OpenAddModal';
            window.blazorInstance.invokeMethodAsync(eventName)
                .then(function () { console.log(); })
                .catch(function (err) { console.error(); });
        }
    });
}

// ==========================================
// CREATE COLUMN MENU - THÊM FILTER VỚI NHIỀU KIỂU
// ==========================================
// Tạo menu cho cột
function createColumnMenu(column, header, index, api, selector) {
    var menuBtn = $('<span class="dt-column-menu" title="Menu">☰</span>');
    var dropdown = $(`
    <div class="dt-column-dropdown" data-column-index="${index}" data-table="${selector}">
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
    // Tạo vị trí dropdown
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
    // xử lý sự kiện click cho nút menu
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
    // xử lý sự kiện click cho nút sắp xếp
    dropdown.find('.dt-sort-asc').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
        column.order('asc').draw(false);
        dropdown.hide();
        console.log('✅ Sorted ascending');
    });
    // xử lý sự kiện click cho nút sắp xếp giảm dần
    dropdown.find('.dt-sort-desc').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
        column.order('desc').draw(false);
        dropdown.hide();
        console.log('✅ Sorted descending');
    });
    // xử lý sự kiện click cho nút xóa bộ lọc
    dropdown.find('.dt-clear-filter').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
        column.search('').draw();
        dropdown.hide();
        console.log('✅ Clear filter');
    });
    // xử lý sự kiện click cho nút ẩn cột
    dropdown.find('.dt-hide-column').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
        column.visible(false);
        dropdown.hide();
        console.log('✅ Hide column');
    });
    // xử lý sự kiện click cho nút lọc
    dropdown.find('.dt-filter-input').on('input', function (e) {
        e.stopPropagation();
        e.preventDefault();
        applyFilter();
    });
   
    // FILTER INPUT - Không đóng dropdown khi typing
    var filterInput = dropdown.find('.dt-filter-input');
    var filterType = dropdown.find('.dt-filter-type');
    // xử lý sự kiện click cho nút lọc
    filterInput.on('click', function(e) {
        e.stopPropagation();
    });
    // xử lý sự kiện click cho nút lọc
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
        // console.log('🔍 Filtering column', index, 'with type:', type, 'value:', searchValue);
    }
    // xử lý sự kiện khi typing
    filterInput.on('keyup', function(e) {
        e.stopPropagation();
        applyFilter();
    });
    // xử lý sự kiện khi thay đổi loại lọc
    filterType.on('change', function(e) {
        e.stopPropagation();
        applyFilter();
    });
    // xử lý sự kiện khi click vào nút xóa bộ lọc
    dropdown.find('.dt-clear-filter').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
        filterInput.val('');
        filterType.val('contains');
        column.search('').draw();
        // console.log('✅ Filter cleared for column', index);
    });
    // xử lý sự kiện khi click vào nút ẩn cột
    dropdown.find('.dt-hide-column').on('click', function (e) {
        e.stopPropagation();
        e.preventDefault();
        column.visible(false);
        dropdown.hide();
        console.log('✅ Column hidden');
    });
    // xử lý sự kiện khi click vào dropdown
    dropdown.on('click mousedown', function(e) {
        e.stopPropagation();
    });
}

// ==========================================
// BIND ROW EVENTS
// ==========================================
// 
function bindAllRowEvents(selector) {
    var $table = $(selector);
    if (!$table.length) return;

    // console.log('🔗 Binding row events for:', selector);

    $table.find('tbody tr').each(function () {
        bindRowActionEvents(this);
    });
}
// xử lý sự kiện khi click vào hàng
function bindRowActionEvents(rowNode) {
    if (!rowNode) return;

    var $row = $(rowNode);

    // xử lý sự kiện khi click vào nút sửa
    $row.find('.btn-edit-user').off('click');
    $row.find('.btn-delete-user').off('click');

    // xử lý sự kiện khi click vào nút sửa
    $row.find('.btn-edit-user').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var userId = $(this).data('user-id') || $row.data('user-id');
        if (userId && window.blazorInstance) {
            // console.log('✏️ Edit user:', userId);
            window.blazorInstance.invokeMethodAsync('OpenEditModalById', userId.toString())
                .catch(function (err) {  });
        } else {
            // console.warn('⚠️ Cannot edit: userId or blazorInstance missing', userId, !!window.blazorInstance);
        }
    });
    // xử lý sự kiện khi click vào nút xóa
    $row.find('.btn-delete-user').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        var userId = $(this).data('user-id') || $row.data('user-id');
        if (userId && window.blazorInstance) {
            // console.log('🗑️ Delete user:', userId);
            window.blazorInstance.invokeMethodAsync('OpenDeleteModalById', userId.toString())
                .catch(function (err) {  });
        } else {
            // console.warn('⚠️ Cannot delete: userId or blazorInstance missing', userId, !!window.blazorInstance);
        }
    });
}

// ==========================================
// DESTROY
// ==========================================

// hủy datatable
// hủy datatable chung
window.destroyGenericDataTable = function (selector) {
    if (window.dataTableInstances[selector]) {
        $(document).off('click.dtMenu');
        $(window).off('scroll.dtMenu resize.dtMenu');
        $('.dt-column-dropdown[data-table="' + selector + '"]').remove();
        
        window.dataTableInstances[selector].destroy();
        delete window.dataTableInstances[selector];
        delete window.dataTableConfigs[selector];
        delete window.selectedRows[selector];
    }
};

window.destroyUserDataTable = window.destroyGenericDataTable; // Backward compatibility if needed

// ==========================================
// HELPER FUNCTIONS
// ==========================================

// Xóa các hàm user-specific dư thừa nếu đã có generic

// ==========================================
// UPDATE DATA - Core function
// ==========================================


// ==========================================
// HIGHLIGHT ROWS
// ==========================================

// highlight row khi thêm, cập nhật, xóa
window.addDataTableRowSmooth = function (selector, rowId, idAttrName = 'data-id') {
    applyGenericHighlight(selector, rowId, 'add', idAttrName);
};

// highlight row khi cập nhật
window.updateDataTableRowSmooth = function (selector, rowId, idAttrName = 'data-id') {
    applyGenericHighlight(selector, rowId, 'update', idAttrName);
};

// highlight row khi xóa
window.deleteDataTableRowSmooth = function (selector, rowId, idAttrName = 'data-id') {
    var targetId = String(rowId);
    var $row = $(selector).find('tbody tr').filter(function() {
        return $(this).attr(idAttrName) == targetId ||
               $(this).find('td:first').text().trim() == targetId;
    });
    
    if ($row.length > 0) {
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
    }
};

// highlight row khi thêm, cập nhật, xóa
function applyGenericHighlight(selector, rowId, type, idAttrName) {
    var targetId = String(rowId);
    var table = window.dataTableInstances[selector];
    if (!table) return;

    var foundRow = null;
    table.rows().every(function() {
        var rowNode = this.node();
        if (rowNode) {
            var actualId = $(rowNode).attr(idAttrName);
            if (!actualId && this.data()[0] == targetId) {
                $(rowNode).attr(idAttrName, targetId);
                actualId = targetId;
            }
            if (actualId == targetId) {
                foundRow = $(rowNode);
                return false;
            }
        }
    });

    if (!foundRow || foundRow.length === 0) {
        foundRow = $(selector).find('tbody tr').filter(function() {
            return $(this).attr(idAttrName) == targetId || $(this).find('td:first').text().trim() == targetId;
        });
    }

    if (foundRow && foundRow.length > 0) {
        if (type === 'add') applyAddHighlight(foundRow, selector);
        else applyUpdateHighlight(foundRow, selector);
    }
}

// bí danh cho các function
window.addUserRowSmooth = (s, id) => window.addDataTableRowSmooth(s, id, 'data-user-id');
window.updateUserRowSmooth = (s, id) => window.updateDataTableRowSmooth(s, id, 'data-user-id');
window.deleteUserRowSmooth = (s, id) => window.deleteDataTableRowSmooth(s, id, 'data-user-id');

// highlight row khi thêm
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

// highlight row khi cập nhật 
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
// Các hàm hỗ trợ render cell 
// ==========================================
// render gender
function getUserGenderBadge(gender) {
    if (gender === 1) {
        return '<span class="badge bg-primary">Nam</span>';
    } else if (gender === 0) {
        return '<span class="badge bg-info">Nữ</span>';
    } else {
        return '<span class="badge bg-secondary">Không xác định</span>';
    }
}

// render status
function getUserStatusBadge(rowStatus) {
    if (rowStatus === 1) {
        return '<span class="badge bg-success">Hoạt động</span>';
    } else {
        return '<span class="badge bg-danger">Ngừng hoạt động</span>';
    }
}

// render image
function getUserImageHtml(image) {
    if (image) {
        return '<img src="' + image + '" alt="Avatar" style="width: 40px; height: 40px; border-radius: 50%;" />';
    } else {
        return '<span class="text-muted">-</span>';
    }
}

// render action
function getUserActionButtons(userId) {
    return '<button class="btn btn-sm btn-warning me-1 btn-edit-user" data-user-id="' + userId + '" title="Chỉnh sửa">' +
        '<i class="feather icon-edit"></i></button>' +
        '<button class="btn btn-sm btn-danger btn-delete-user" data-user-id="' + userId + '" title="Xóa">' +
        '<i class="feather icon-trash-2"></i></button>';
}

// render date time
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

// render group action
window.getGroupActionButtons = function(item) {
    var id = item.id || item.Id;
    var name = item.name || item.Name;
    return `
        <div class="btn-group">
            <button class="btn btn-primary btn-sm btn-edit-group" data-id="${id}" title="Sửa"><i class="feather icon-edit"></i></button>
            <button class="btn btn-danger btn-sm btn-delete-group" data-id="${id}" data-name="${name}" title="Xóa"><i class="feather icon-trash-2"></i></button>
        </div>
    `;
};