using System;
using System.Collections.Generic;
using System.Text;

namespace GraphSuite
{
    public static class StringExtensions
    {
        public static string GetVertexValue(this String str)
        {
            return str.Replace("_", "");
        }
    }
}
