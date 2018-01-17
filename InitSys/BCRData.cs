using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirle.ASRS
{
    public class BCRData
    {
        private List<BCR> lstBCR = new List<BCR>();

        public BCR this[int index]
        {
            get { return lstBCR[index]; }
        }

        public int _BCRCount
        {
            get { return lstBCR.Count; }
        }

        public BCRData()
        {
        }

        public BCRData(List<BCR> bCRList)
        {
            lstBCR = bCRList;
        }
    }
}
