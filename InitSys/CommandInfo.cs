using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirle.ASRS
{
    public class CommandInfo
    {
        public string CommandID;
        public int CommandMode;
        public string IO_Type;
        public string Loaction;
        public string NewLoaction;
        public string StationNo;
        public string Priority;

        public CommandInfo()
        {
            CommandID = string.Empty;
            CommandMode = 0;
            IO_Type = string.Empty;
            Loaction = string.Empty;
            NewLoaction = string.Empty;
            StationNo = string.Empty;
            Priority = string.Empty;
        }
    }
}
