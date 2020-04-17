using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserApp
{
    interface IUpdatable
    {
        List<Threat> RefreshTableWithThreats(out string refreshedInfo);
    }
}
