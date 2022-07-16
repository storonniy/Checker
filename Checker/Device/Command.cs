using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Checker.Steps;

namespace Checker.Devices
{
    public abstract class Command
    {
        public abstract void Do(Step step);
    }
}
