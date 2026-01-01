using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.SysMenu
{
    public class SysMenuCreateModel
    {
        public string Name { get; set; } = null!;
        public string? ParentMenu { get; set; }
        public string Note { get; set; } = null!;
        public string? Icon { get; set; }
        public int? IsActive { get; set; }
    }
}

