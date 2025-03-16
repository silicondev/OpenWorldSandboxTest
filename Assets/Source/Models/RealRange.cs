using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Source.Models
{
    public class RealRange
    {
        public int Start { get; }
        public int End { get; }

        public RealRange(int start, int end)
        {
            Start = start;
            End = end;
        }
    }
}
