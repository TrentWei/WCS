using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirle.ASRS
{
    public class CraneMode
    {
        public const string StoreIn = "1";
        public const string StoreOut = "2";
        public const string StationToStation = "4";
        public const string LoactionToLoaction = "5";
        public const string Move = "6";
        public const string Scan = "7";
        public const string PickUp = "8";
        public const string Deposite = "9";
        public const string ReturnHP = "10";

        private CraneMode()
        {
        }
    }
}
