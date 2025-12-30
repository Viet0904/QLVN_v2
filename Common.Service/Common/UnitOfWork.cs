using Common.Model.Common;
using Common.Library.Helper;
namespace Common.Service.Common
{
    public class UnitOfWork
    {
        private static SQLConnectionStringModel? clientConnectionString = null;
        private static SQLConnectionStringModel? serverConnectionString = null;

        private static DataProvider? _ins = null;

        public static SQLConnectionStringModel SetClientConnectionString
        {
            set
            {
                clientConnectionString = value;
                _ins = new DataProvider(clientConnectionString);
            }
        }

        public static SQLConnectionStringModel ServerConnectionString
        {
            set
            {
                serverConnectionString = value;
            }
            get { return serverConnectionString; }
        }

        public static DataProvider Ins
        {
            get
            {
                if (clientConnectionString == null)
                    throw new Exception("Cannot connect to database.");

                if (_ins == null) _ins = new DataProvider(clientConnectionString);
                return _ins;
            }
        }
    }
}
