/**
 * DataTable Header/Body Sync Helper
 * Fix lệch header và body khi sử dụng scrollX và scrollY
 */

// Hàm sync width giữa header và body table
window.syncDataTableHeaderWidth = function(selector) {
    try {
        var table = window.dataTableInstances[selector];
        if (!table) return;

        var wrapper = $(table.table().container());
        var headerTable = wrapper.find('.dt-scroll-head table');
        var bodyTable = wrapper.find('.dt-scroll-body table');

        if (headerTable.length && bodyTable.length) {
            // Force re-calculate width
            headerTable.css('width', '');
            bodyTable.css('width', '');
            
            // Get body table width (this is the actual width with data)
            var bodyWidth = bodyTable.outerWidth();
            
            // Set header table to match body table width
            headerTable.css('width', bodyWidth + 'px');
            
            // Sync each column width
            var headerCells = headerTable.find('thead th');
            var bodyCells = bodyTable.find('tbody tr:first td');
            
            headerCells.each(function(index) {
                var bodyCell = bodyCells.eq(index);
                if (bodyCell.length) {
                    var width = bodyCell.outerWidth();
                    $(this).css({
                        'width': width + 'px',
                        'min-width': width + 'px',
                        'max-width': width + 'px'
                    });
                }
            });
        }
    } catch (e) {
        //console.warn('Error syncing header width:', e);
    }
};

// Hàm thêm tooltip cho header cells
window.addDataTableHeaderTooltips = function (selector) {
    try {
        var table = window.dataTableInstances[selector];
        if (!table) return;

        var wrapper = $(table.table().container());

        // Add tooltips for header cells in both scrollable and normal tables
        wrapper.find('thead th').each(function () {
            var $th = $(this);

            // Skip if already has title
            if ($th.attr('title')) return;

            // Get text content (excluding icons and menu)
            var textNode = $th.contents().filter(function () {
                return this.nodeType === 3; // Text node
            }).text().trim();

            // If no text node, try to get from span or other elements
            if (!textNode) {
                textNode = $th.clone()
                    .children('.dt-column-order, .dt-column-menu')
                    .remove()
                    .end()
                    .text()
                    .trim();
            }

            if (textNode) {
                $th.attr('title', textNode);
            }
        });

        //console.log('✓ Header tooltips added for', selector);
    } catch (e) {
        //console.warn('Error adding header tooltips:', e);
    }
};


// Hàm để force re-sync sau khi có thay đổi
window.forceDataTableSync = function(selector) {
    var table = window.dataTableInstances[selector];
    if (!table) return;
    
    // Multiple adjustments để đảm bảo sync
    table.columns.adjust();
    
    setTimeout(() => {
        window.syncDataTableHeaderWidth(selector);
        window.addDataTableHeaderTooltips(selector); 
        table.columns.adjust();
    }, 50);
    
    setTimeout(() => {
        window.syncDataTableHeaderWidth(selector);
        table.columns.adjust();
    }, 150);
    
    setTimeout(() => {
        window.syncDataTableHeaderWidth(selector);
    }, 300);
};

// Auto-sync khi window resize
$(window).on('resize', function() {
    Object.keys(window.dataTableInstances).forEach(function(selector) {
        window.forceDataTableSync(selector);
    });
});

// Auto-sync khi scroll horizontal
$(document).on('scroll', '.dt-scroll-body', function() {
    var selector = '#' + $(this).closest('.dataTables_wrapper').find('table').attr('id');
    if (window.dataTableInstances[selector]) {
        // Sync scroll position between header and body
        var scrollLeft = $(this).scrollLeft();
        $(this).closest('.dataTables_wrapper').find('.dt-scroll-head').scrollLeft(scrollLeft);
    }
});
