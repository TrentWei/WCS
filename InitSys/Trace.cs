using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirle.ASRS
{
    public class Trace
    {
        public const string Inital = "0";

        public const string StoreIn_GetStoreInCommandAndWritePLC = "11";
        public const string StoreIn_CrateCraneCommand = "12";
        public const string StoreIn_CraneCommandFinish = "13";

        public const string StoreOut_GetStoreOutCommandAndWritePLC = "21";
        public const string StoreOut_CrateCraneCommand = "22";
        public const string StoreOut_CraneCommandFinish = "23";

        public const string LoactionToLoaction_CrateCraneCommand = "51";
        public const string LoactionToLoaction_CraneCommandFinish = "52";

        private Trace()
        {
        }
    }
}
