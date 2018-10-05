using System;
using System.Collections.Generic;
using System.Text;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IPairingList
    {
        string this[string key] { get; set; }

        bool Contains(string address);

        void Add(string address, string uniqueID);

        bool Remove(string address);

        void Clear();
    }
}
