using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTG.Logic
{
    public partial class NTGDB
    {
        public NTGDB(System.Data.IsolationLevel isolationLevel)
            : base("name=NTGDB")
        {
            Database.BeginTransaction(isolationLevel);
        }
    }

    public class NTGDBTransactional : NTGDB
    {
        public NTGDBTransactional() : base(System.Data.IsolationLevel.Snapshot) { }

        public void Commit() {
            Database.CurrentTransaction.Commit();
        }

        public void Rollback()
        {
            Database.CurrentTransaction.Rollback();
        }
    }
}
