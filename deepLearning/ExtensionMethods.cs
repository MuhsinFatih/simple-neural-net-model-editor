using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deepLearning
{

    public static class MyExtensions
    {

        public static bool IsInteger(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

    }
}

