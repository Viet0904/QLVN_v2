using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Model.Common
{
    public class SQLConnectionStringModel
    {
        public string Ip { get; set; } = null!;

        public string Database { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
