using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTG.Logic.Models
{
    public partial class SiteLog
    {
        protected override bool IsNew()
        {
            return Id == 0;
        }

        public SiteLog StripAdminPassword()
        {
            if (this.Admin != null)
            {
                this.Admin.Password = null;
            }

            return this;
        }
    }
}
