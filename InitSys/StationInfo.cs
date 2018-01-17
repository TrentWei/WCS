using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirle.ASRS
{
    public class StationInfo
    {
        public string BufferName;
        public int BufferIndex;
        public string StationName;
        public int StationIndex;
        public string BCRName;

        public StationInfo()
        {
            BufferName = string.Empty;
            BufferIndex = 0;
            StationName = string.Empty;
            StationIndex = 0;
            BCRName = string.Empty;
        }
    }
}
