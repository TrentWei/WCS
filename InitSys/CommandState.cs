using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirle.ASRS
{
    public class CommandState
    {
        public const string Inital = "0";
        public const string Start = "1";
        public const string CompletedWaitPost = "7";
        public const string CancelWaitPost = "8";
        public const string Completed = "9";
        public const string Cancel = "D";
        public const string PostFail = "E";

        private CommandState()
        {
        }
    }
}
