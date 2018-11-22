using System;
using System.Collections.Generic;
using System.Text;

namespace KD.Navien.WaterBoilerMat
{
    public class CommunicationFailException : Exception
    {
        public CommunicationFailException(string message) : base(message)
        {

        }
    }
}
