using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.Common
{
    public class BaseViewModel
    {
        public int RowStatus { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string CreatedDate { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
        public string CreatedName { get; set; } = null!;
        public System.DateTime UpdatedAt { get; set; }
        public string UpdatedDate { get; set; } = null!;
        public string UpdatedBy { get; set; } = null!;
        public string UpdatedName { get; set; } = null!;
    }
}
