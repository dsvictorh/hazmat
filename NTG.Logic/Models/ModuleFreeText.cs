﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTG.Logic.Models
{
    public partial class ModuleFreeText
    {
        protected override bool IsNew()
        {
            return Id == 0;
        }
    }
}
