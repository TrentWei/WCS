using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirle.ASRS
{
    public class BufferData
    {
        private Buffer[] arrBuffer = new Buffer[0];

        public Buffer this[int bufferIndex]
        {
            get { return arrBuffer[bufferIndex]; }
        }

        public int _BufferCount
        {
            get { return arrBuffer.Length; }
        }

        public BufferData()
        {
        }

        public BufferData(List<string> bufferList, Dictionary<string, string> bufferAddressMAP)
        {
            arrBuffer = new Buffer[bufferList.Count];
            for(int i = 0; i < bufferList.Count; i++)
            {
                string strBuffer = bufferList[i];
                if(bufferAddressMAP.ContainsKey(strBuffer))
                    arrBuffer[i] = new Buffer(strBuffer, bufferAddressMAP[strBuffer]);
            }
        }
    }
}
