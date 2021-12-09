using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkryazTools.Enum
{
    public enum FilterRuleEnum
    {
        [Description("Equal To")]
        EqualTo,
        [Description("Not Equal To")]
        NotEqualTo,
        [Description("Contains")]
        Contains,
        [Description("Does Not Contains")]
        DoesNotContains,
        [Description("Begins With")]
        BeginsWith,
        [Description("Does Not Begins With")]
        DoesNotBeginsWith,
        [Description("Ends With")]
        EndsWith,
        [Description("Does Not Ends With")]
        DoesNotEndsWith
    }
}
