using KD.Navien.WaterBoilerMat.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace KD.Navien.WaterBoilerMat.Universal.Models
{
    public class PairingList : IPairingList
    {
        #region Properties

        public int Count => pairingListLocalSettings.Values.Count;

        public string this[string key]
        {
            get => pairingListLocalSettings.Values[key] as string;
            set => pairingListLocalSettings.Values[key] = value;
        }

        #endregion

        #region Fields

        private readonly ApplicationDataContainer pairingListLocalSettings
            = ApplicationData.Current.RoamingSettings.CreateContainer(nameof(PairingList), ApplicationDataCreateDisposition.Always);

        #endregion

        public bool Contains(string address) => pairingListLocalSettings.Values.ContainsKey(address);

        public void Add(string address, string uniqueID)
        {
            pairingListLocalSettings.Values.Add(address, uniqueID);
        }

        public bool Remove(string address)
        {
            return pairingListLocalSettings.Values.Remove(address);
        }

        public void Clear()
        {
            pairingListLocalSettings.Values.Clear();
        }
    }
}
