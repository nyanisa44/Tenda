using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TendaAdvisors.Controllers;

namespace Debugger
{
    class Program
    {
        static void Main(string[] args)
        {
            AdvisorsController ac = new AdvisorsController();
            ac.AdvisorToCommision(1, 1);
        }
    }
}
