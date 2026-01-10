# Fix L?ch Header và Body DataTable

## V?n ??
Khi s? d?ng DataTables v?i `scrollY` và `scrollX`, header và body c?a table b? l?ch c?t do:
- DataTables t?o 2 b?ng riêng bi?t (m?t cho header, m?t cho body)
- Scrollbar vertical làm thay ??i width c?a body table
- Tính toán width không chính xác gi?a 2 b?ng

## Gi?i pháp ?ã implement

### 1. JavaScript - datatables_custom.js
**Thay ??i:**
- T?t `scrollCollapse: false` ?? tránh thay ??i height không ??ng b?
- Thêm `forceAdjust()` function ?? ?i?u ch?nh columns nhi?u l?n
- G?i `columns.adjust()` nhi?u l?n sau m?i thay ??i d? li?u:
  - Sau 50ms
  - Sau 150ms  
  - Sau 300ms

**Lý do:** DOM c?n th?i gian ?? render, m?t l?n adjust không ??

### 2. JavaScript - datatable-header-sync.js (M?I)
**File m?i t?o ?? sync chính xác width gi?a header và body:**

```javascript
// Sync width t?ng cell gi?a header và body
window.syncDataTableHeaderWidth = function(selector) {
    var headerCells = headerTable.find('thead th');
    var bodyCells = bodyTable.find('tbody tr:first td');
    
    headerCells.each(function(index) {
        var width = bodyCells.eq(index).outerWidth();
        $(this).css({
            'width': width + 'px',
            'min-width': width + 'px',
            'max-width': width + 'px'
        });
    });
}

// Force sync nhi?u l?n
window.forceDataTableSync = function(selector) {
    table.columns.adjust();
    setTimeout(() => window.syncDataTableHeaderWidth(selector), 50);
    setTimeout(() => window.syncDataTableHeaderWidth(selector), 150);
    setTimeout(() => window.syncDataTableHeaderWidth(selector), 300);
}
```

**Features:**
- Auto-sync khi window resize
- Auto-sync horizontal scroll gi?a header và body
- Tính toán chính xác width t?ng cell

### 3. CSS - datatables-custom.css
**Thay ??i quan tr?ng:**

```css
/* CRITICAL: Fixed layout ?? ??ng b? width */
.dt-scroll-head table,
.dt-scroll-body table {
    table-layout: fixed !important;
    border-collapse: separate !important;
    border-spacing: 0 !important;
}

/* ??ng b? padding gi?a header và body */
.dt-scroll-head table th,
.dt-scroll-body table td {
    box-sizing: border-box !important;
    padding: 12px 10px !important;
    overflow: hidden;
    text-overflow: ellipsis;
}

/* Always show vertical scrollbar ?? width nh?t quán */
.dt-scroll-body {
    overflow-y: scroll !important;
    overflow-x: auto !important;
}

/* Hide horizontal scroll ? header */
.dt-scroll-head {
    overflow: hidden !important;
    overflow-x: hidden !important;
}
```

**Lý do:**
- `table-layout: fixed` ??m b?o columns có cùng width algorithm
- `overflow-y: scroll` luôn hi?n scrollbar ?? width không thay ??i
- `box-sizing: border-box` tính padding vào width
- ??ng b? border và spacing

### 4. Razor Component - UserList.razor
**Thay ??i:**

```csharp
private async Task RefreshData()
{
    paginatedUsers = await UserService.GetPaginatedUsersAsync(paginationRequest);
    await RefreshDataTable();
    StateHasChanged();

    await Task.Delay(100); // ??i DOM render
    await JS.InvokeVoidAsync("eval", $@"
        if(window.dataTableInstances['#{tableId}'].forceAdjust) {{
            window.dataTableInstances['#{tableId}'].forceAdjust();
        }}
    ");
}
```

**Lý do:** G?i `forceAdjust()` sau khi Blazor render xong

### 5. index.html
**Thêm script m?i:**

```html
<script src="js/datatables_custom.js"></script>
<!-- NEW: DataTable Header/Body Sync Helper -->
<script src="js/datatable-header-sync.js"></script>
<script src="js/data-table-custom.js"></script>
```

**Th? t? quan tr?ng:** Load sau datatables_custom.js, tr??c data-table-custom.js

## Cách ho?t ??ng

1. **Init DataTable:**
   - Kh?i t?o v?i `scrollCollapse: false`
   - G?i `columns.adjust()` nhi?u l?n sau init

2. **Update Data:**
   - Clear và add rows m?i
   - G?i `draw(false)` ?? render
   - G?i `forceDataTableSync()` 3 l?n (50ms, 150ms, 300ms)

3. **Sync Width:**
   - ??c width t? body cells (có data th?t)
   - Set width cho header cells t??ng ?ng
   - Set min-width và max-width ?? lock width

4. **Auto-adjust:**
   - Window resize ? sync l?i t?t c? tables
   - Sidebar toggle ? sync l?i
   - Horizontal scroll ? sync scroll position header/body

## Testing

Sau khi implement, test các tr??ng h?p:

1. ? Load trang l?n ??u
2. ? Thay ??i page size (10, 25, 50, 100)
3. ? Search và filter
4. ? Sort columns
5. ? Hide/show columns
6. ? Resize window
7. ? Toggle sidebar
8. ? Scroll horizontal
9. ? Add/Edit/Delete rows
10. ? Zoom browser (80%, 100%, 125%, 150%)

## L?u ý

- Không xóa các `setTimeout()` - chúng c?n thi?t cho DOM render
- Không t?t `overflow-y: scroll` - nó ??m b?o width nh?t quán
- `table-layout: fixed` là CRITICAL - không ???c thay ??i
- N?u thêm/b?t columns, c?n test l?i alignment

## Performance

- Sync width ch? ch?y khi c?n thi?t (sau update data, resize, sidebar toggle)
- S? d?ng `requestAnimationFrame` ?? t?i ?u render
- Không ?nh h??ng performance v?i b?ng nh? (<1000 rows visible)

## Fallback

N?u `datatable-header-sync.js` không load:
```javascript
if (window.forceDataTableSync) {
    window.forceDataTableSync(selector);
} else {
    table.columns.adjust(); // Fallback
}
```

## Troubleshooting

**V?n còn l?ch m?t chút:**
- Ki?m tra padding c?a th và td có gi?ng nhau không
- Ki?m tra border-width có khác nhau không
- Th? t?ng timeout (50ms ? 100ms, 150ms ? 250ms)

**Header b? crop n?i dung:**
- Ki?m tra `text-overflow: ellipsis` ?ã apply ch?a
- Xem xét t?ng min-width c?a columns

**Scrollbar không ??ng b?:**
- Ki?m tra event listener cho scroll ?ã bind ch?a
- Verify selector ?úng ch?a

## Files Changed

1. ? `WebBlazor/wwwroot/js/datatables_custom.js` - Core logic
2. ? `WebBlazor/wwwroot/js/datatable-header-sync.js` - NEW file
3. ? `WebBlazor/wwwroot/css/datatables-custom.css` - Styling fixes
4. ? `WebBlazor/Pages/User/UserList.razor` - Component update
5. ? `WebBlazor/wwwroot/index.html` - Script reference

## K?t qu?

- ? Header và body table alignment chính xác
- ? Không b? l?ch khi scroll
- ? Responsive v?i m?i screen size
- ? Work v?i sidebar toggle
- ? Performance t?t

---
**Ngày t?o:** 2025-01-XX  
**Tác gi?:** GitHub Copilot  
**Version:** 1.0
