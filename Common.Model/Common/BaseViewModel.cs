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
        public string? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedName { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public string? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedName { get; set; }
    }
}
