using Kaizen.MircoORM.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaizen.MircoORM
{
    public class DataFactory
    {
        public static IDataService DataService => new MSSQL();
    }
}
