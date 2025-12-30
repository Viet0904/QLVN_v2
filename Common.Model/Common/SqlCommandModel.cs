using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.Common
{
    public class SqlCommandModel
    {
        public string CommandText { get; set; } = null!;

        public CommandType CommandType { get; set; }

        public IDictionary<string, object> Parameters { get; set; } = null!;
    }
}
