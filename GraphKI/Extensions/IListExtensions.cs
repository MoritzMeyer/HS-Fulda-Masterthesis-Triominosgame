using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphKI.Extensions
{
    public static class IListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            for (int i = 0; i < list.Count - 1; ++i)
            {
                int rand = UnityEngine.Random.Range(i, list.Count);
                T temp = list[i];
                list[i] = list[rand];
                list[rand] = temp;
            }
        }
    }
}
