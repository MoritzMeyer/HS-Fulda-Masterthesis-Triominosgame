using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace GraphSuite
{
    public static class StringExtensions
    {
        public static string GetVertexValue(this string str)
        {
            return str.Replace("_", "");
        }

        public static int GetTileValue(this string str)
        {
            return str.Split('_').Select(v => Int32.Parse(v)).Sum();
        }

        public static string Reverse(this string str)
        {
            char[] charArray = str.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static bool IsBlueSide(this string str)
        {
            string value = str.GetVertexValue();
            int i = Convert.ToInt32(value[0]);
            int j = Convert.ToInt32(value[1]);

            return i == j;
        }

        public static bool IsGreenSide(this string str)
        {
            string value = str.GetVertexValue();
            int i = Convert.ToInt32(value[0]);
            int j = Convert.ToInt32(value[1]);

            return i < j;
        }

        public static bool IsRedSide(this string str)
        {
            string value = str.GetVertexValue();
            int i = Convert.ToInt32(value[0]);
            int j = Convert.ToInt32(value[1]);

            return i > j;
        }

        public static VertexColor GetVertexColor(this string str)
        {
            if (str.IsGreenSide())
            {
                return VertexColor.Green;
            }

            if (str.IsBlueSide())
            {
                return VertexColor.Blue;
            }

            if (str.IsRedSide())
            {
                return VertexColor.Red;
            }

            throw new ArgumentException("Cannot determin VertexColor");
        }
    }
}
