using KD.Navien.WaterBoilerMat.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using System.Threading;

namespace KD.Navien.WaterBoilerMat.Universal.Models
{
	public class BluetoothGattServiceUwp : BindableBase, IBluetoothGattService
	{
		public event EventHandler GattCharacteristicsUpdated;

		public string UUID => _gattDeviceService.Uuid.ToString();

        public string Name => Utils.ConvertUuidToName(_gattDeviceService.Uuid);

        public List<IBluetoothGattCharacteristic> GattCharacteristics
		{
			get => gattCharacteristics;
			private set => SetProperty(ref gattCharacteristics, value);
		}
		private List<IBluetoothGattCharacteristic> gattCharacteristics = new List<IBluetoothGattCharacteristic>();

        #region Fields

        private bool _disposed;
        private readonly GattDeviceService _gattDeviceService;

        #endregion

        #region Constructors & Initialize & Dispose, Destructors

        public BluetoothGattServiceUwp(GattDeviceService gattDeviceService)
		{
            _gattDeviceService = gattDeviceService;

            Initialize();
        }
		private Task Initialize()
		{
            return GetAllCharacteristics();
        }

        ~BluetoothGattServiceUwp()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            try
            {
                if (disposing)
                {/* You need to clean up external resources managed by the .NET Framework at here. */
                    _gattDeviceService.Dispose();
                }

                try { /* You need to clean up external resources didn't managed by the .NET Framework at here. */ }
                catch { }
                finally
                {
                    _disposed = true;
                }
            }
            finally { }
        }

        #endregion

        /// <summary>
        /// Gets all the characteristics of this service
        /// </summary>
        /// <returns>The status of the communication with the GATT device.</returns>
        private async Task<GattCommunicationStatus> GetAllCharacteristics()
        {
            var tokenSource = new CancellationTokenSource(5000);
            var getCharacteristicsTask = await Task.Run(() => _gattDeviceService.GetCharacteristicsAsync(BluetoothCacheMode.Uncached), tokenSource.Token);

            GattCharacteristicsResult result = null;
            result = await getCharacteristicsTask;

            if (result.Status == GattCommunicationStatus.Success)
            {
                GattCharacteristics.AddRange(result.Characteristics.Select(C => new BluetoothGattCharacteristicUwp(C)));
            }

            GattCharacteristicsUpdated?.Invoke(this, EventArgs.Empty);

            return result.Status;
        }
    }
}
