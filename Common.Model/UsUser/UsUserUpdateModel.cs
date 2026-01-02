using Common.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.UsUser
{
    public class UsUserUpdateModel : BaseViewModel
    {
        public string Id { get; set; } = null!;
        public string GroupId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Name { get; set; } = null!;
        public Nullable<int> Gender { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CMND { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }
        public string? Note { get; set; }
        public string? Password { get; set; }
        public bool? IsChangePassword { get; set; }
    }
}
