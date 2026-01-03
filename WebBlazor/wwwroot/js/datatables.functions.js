window.DataTableFunctions = {
    renderStatus: function(data) {
        return data === 1 ? '<span class="badge badge-success">Hoạt động</span>' : '<span class="badge badge-danger">Ngừng hoạt động</span>';
    },
    renderActions: function(data, type, row) {
        return '<button class="btn btn-info btn-sm btn-select" data-id="' + data + '" data-name="' + row.name + '"><i class="feather icon-shield"></i></button> ' + 
               '<button class="btn btn-primary btn-sm btn-edit" data-id="' + data + '"><i class="feather icon-edit"></i></button> ' + 
               '<button class="btn btn-danger btn-sm btn-delete" data-id="' + data + '" data-name="' + row.name + '"><i class="feather icon-trash-2"></i></button>';
    }
};

// Global AJAX setup to include Auth Token for all DataTables/jQuery calls
$.ajaxSetup({
    beforeSend: function(xhr) {
        var token = localStorage.getItem('authToken');
        if (token) {
            // Remove quotes if present (Blazored.LocalStorage might add them)
            token = token.replace(/^"(.*)"$/, '$1');
            xhr.setRequestHeader('Authorization', 'Bearer ' + token);
        }
    }
});
