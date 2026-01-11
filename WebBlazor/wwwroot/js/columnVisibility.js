// Column Visibility Dropdown Handler
window.columnVisibilityHandler = {
    init: function (dotNetHelper) {
        // Close dropdown when clicking outside
        document.addEventListener('click', function (e) {
            const dropdown = document.querySelector('.column-visibility-dropdown');
            const menu = document.querySelector('.column-visibility-menu');
            
            if (dropdown && menu && !dropdown.contains(e.target)) {
                dotNetHelper.invokeMethodAsync('CloseColumnMenu');
            }
        });
    }
};
