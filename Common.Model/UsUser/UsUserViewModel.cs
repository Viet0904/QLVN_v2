using Common.Model.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.UsUser
{
    public class UsUserViewModel : BaseViewModel
    {
        public string Id { get; set; } = null!;
        public string GroupId { get; set; } = null!;
        public string? GroupName { get; set; }
        public string Name { get; set; } = null!;
        public Nullable<int> Gender { get; set; }
        public string UserName { get; set; } = null!;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CMND { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }
        public string? Theme { get; set; }
        public string? Note { get; set; }
    }
}
