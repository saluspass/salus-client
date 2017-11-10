using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ipfs_pswmgr.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static void AssignIfDifferent(this string str, string strNewValue, ISaveableObject parent)
        {
            if(str != strNewValue)
            {
                str = strNewValue;
                parent.Dirty = true;
            }
        }
    }
}
