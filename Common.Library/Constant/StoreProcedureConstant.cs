using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Library.Constant
{
    public class StoreProcedureConstant
    {
        public struct UserPermission
        {
            public struct InsertOrUpdate
            {
                public const string Name = "SpIU_UserPermission";
                public const string PrJson = "@Json";
            }
            public struct Read
            {
                public const string Name = "SpR_UserPermission";
                public const string PrUserId = "@UserId";
            }
        }
        public struct TheReader
        {
            public struct InsertOrDelete
            {
                public const string Name = "SpIUD_TheReader";
                public const string PrJson = "@Json";
            }
        }
    }
}
