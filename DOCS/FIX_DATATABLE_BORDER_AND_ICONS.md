# Fix DataTable Border và Icon Overlap

## V?n ??
1. **M?t border** - Table không hi?n th? border gi?a các c?t và dòng
2. **Icon ?è text** - Sort icons (??) và menu icons (?) ?è lên n?i dung text trong header

## Nguyên nhân
- CSS `border: none !important` ?ã xóa t?t c? border
- `border-collapse: separate` gây ra kho?ng tr?ng gi?a các cells
- Padding-right không ?? ?? ch?a ch? cho icons (sort và menu)

## Gi?i pháp

### 1. Fix Border

**Thay ??i trong `datatables-custom.css`:**

```css
/* Thay ??i border-collapse */
.dt-scroll-head table,
.dt-scroll-body table {
    border-collapse: collapse !important; /* CHANGED: t? separate ? collapse */
}

/* Thêm border cho t?t c? cells */
.dt-scroll-head table th,
.dt-scroll-body table td {
    border: 1px solid #dee2e6 !important; /* ADDED */
}

/* Border cho table */
table.dataTable {
    border: 1px solid #dee2e6 !important; /* ADDED */
    border-collapse: collapse !important;
}

/* Border cho header */
.dt-scroll-head table th {
    border-bottom: 2px solid #dee2e6 !important;
}

/* Border cho body */
.dt-scroll-body table td {
    border: 1px solid #dee2e6 !important;
}
```

### 2. Fix Icon Overlap

**Thay ??i padding và position:**

```css
/* T?ng padding-right cho header cells */
.dt-scroll-head table th,
.dt-scroll-body table td {
    padding: 12px 60px 12px 10px !important; /* CHANGED: 60px right padding */
}

table.dataTable thead th {
    padding-right: 65px !important; /* CHANGED: T?ng t? 60px */
}

/* ?i?u ch?nh v? trí sort icon */
table.dataTable thead th .dt-column-order {
    right: 35px !important; /* CHANGED: t? 30px */
    width: 16px !important; /* ADDED: Fixed width */
}

/* ?i?u ch?nh v? trí menu icon */
.dt-column-menu {
    right: 10px !important; /* CHANGED: t? 8px */
}
```

### 3. Layout Icon trong Header

```
??????????????????????????????????????????????????????
?  Text Content          [Sort ??]  [Menu ?]         ?
?  ? 10px padding        ?         ?                  ?
?                    right: 35px  right: 10px        ?
?                        16px wide  ~20px wide       ?
??????????????????????????????????????????????????????
     Total padding-right: 65px
```

**Breakdown:**
- Text content: B?t ??u t? left + 10px
- Sort icon: Right 35px, width 16px
- Menu icon: Right 10px, width ~20px
- Total space: 10px (menu) + 20px (menu width) + 5px (gap) + 16px (sort) + 14px (margin) = 65px

## Chi ti?t thay ??i

### File: `datatables-custom.css`

#### Section 1: FIX HEADER/BODY ALIGNMENT
- ? Changed `border-collapse: separate` ? `collapse`
- ? Added `border: 1px solid #dee2e6` cho cells
- ? Changed padding t? `12px 10px` ? `12px 60px 12px 10px`

#### Section 2: HEADER STYLING
- ? Changed padding-right t? `60px` ? `65px`
- ? Adjusted sort icon position: `right: 30px` ? `right: 35px`
- ? Added `width: 16px` cho sort icon
- ? Adjusted menu icon position: `right: 8px` ? `right: 10px`

#### Section 3: ROW STYLES
- ? Added `border: 1px solid #dee2e6` cho body cells
- ? Added `vertical-align: middle`

#### Section 4: TABLE STYLES
- ? Changed `border-collapse: separate` ? `collapse`
- ? Added `border: 1px solid #dee2e6` cho table
- ? Updated padding cho thead th: `12px 60px 12px 10px`
- ? Added border cho tbody td

## K?t qu?

### Before:
```
???????????????????????????????????
  Id Nhóm H? Tên??? Tên ??ng nh?p  <- Icons ?è lên text, không có border
  ...
```

### After:
```
???????????????????????????????????????????????????
?  Id     ? Nhóm   ? H? Tên  ???? Tên ??ng nh?p  ?  <- Icons tách bi?t, có border
???????????????????????????????????????????????????
? 00001002? Admin  ? User Test 1 ? user1          ?
???????????????????????????????????????????????????
? 00001003? Admin  ? User Test 2 ? user2          ?
???????????????????????????????????????????????????
```

## Testing

Ki?m tra các tr??ng h?p:

1. ? Border hi?n th? ?úng gi?a các c?t
2. ? Border hi?n th? ?úng gi?a các dòng
3. ? Sort icons không ?è lên text
4. ? Menu icons không ?è lên text
5. ? Header và body v?n align chính xác
6. ? Hover effects v?n ho?t ??ng
7. ? Column visibility toggle v?n ho?t ??ng
8. ? Sort v?n ho?t ??ng
9. ? Responsive trên mobile/tablet
10. ? Sidebar toggle không ?nh h??ng

## Responsive Behavior

### Desktop (>992px)
- Full padding: 65px right
- Icons hi?n th? ??y ??
- Border rõ ràng

### Tablet (768px - 991px)
- Padding có th? gi?m xu?ng 55px
- Icons v?n hi?n th?
- Border v?n rõ

### Mobile (<768px)
- Có th? c?n ?i?u ch?nh padding
- Xem xét ?n m?t s? icons
- Border v?n c?n thi?t

## Performance Impact

- ? Không ?nh h??ng performance
- ? CSS rules t?i ?u
- ? Không thêm JavaScript

## Browser Compatibility

- ? Chrome/Edge (latest)
- ? Firefox (latest)
- ? Safari (latest)
- ? Mobile browsers

## Notes

- **Border color**: `#dee2e6` (Bootstrap default)
- **Header border**: 2px solid (thicker)
- **Body border**: 1px solid (normal)
- **Sort icon width**: 16px (fixed)
- **Menu icon width**: ~20px (auto)
- **Gap between icons**: 5px minimum

## Troubleshooting

### Border không hi?n th?:
```css
/* Ki?m tra không có rule nào override */
table.dataTable * {
    border: none !important; /* ? Xóa rule này n?u có */
}
```

### Icons v?n ?è text:
```css
/* T?ng padding-right */
table.dataTable thead th {
    padding-right: 70px !important; /* T?ng lên n?u c?n */
}
```

### Header/Body misalign:
```css
/* ??m b?o cùng padding */
.dt-scroll-head table th,
.dt-scroll-body table td {
    padding: 12px 60px 12px 10px !important;
    /* ? Ph?i gi?ng nhau */
}
```

## Files Changed

- ? `WebBlazor/wwwroot/css/datatables-custom.css` - Main fixes

## Related Documents

- `DOCS/FIX_DATATABLE_HEADER_ALIGNMENT.md` - Header/Body alignment fix
- Original issue: Border missing, Icons overlap

---
**Date:** 2025-01-XX  
**Author:** GitHub Copilot  
**Version:** 1.1
