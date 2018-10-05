using KD.Navien.WaterBoilerMat.Models;
using Microsoft.Toolkit.Uwp.Connectivity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace KD.Navien.WaterBoilerMat.Universal.Models
{
	public class BluetoothGattServiceUwp : BindableBase, IBluetoothGattService
	{
		public event EventHandler GattCharacteristicsUpdated;

		public string UUID => gattDeviceService.UUID;

		public string Name => gattDeviceService.Name;

		public List<IBluetoothGattCharacteristic> GattCharacteristics
		{
			get => gattCharacteristics;
			private set => SetProperty(ref gattCharacteristics, value);
		}
		private List<IBluetoothGattCharacteristic> gattCharacteristics = new List<IBluetoothGattCharacteristic>();

		private ObservableGattDeviceService gattDeviceService;


		public BluetoothGattServiceUwp(ObservableGattDeviceService gattDeviceService)
		{
			this.gattDeviceService = gattDeviceService;

			Initialize();
		}
		public BluetoothGattServiceUwp(GattDeviceService gattDeviceService) : this(new ObservableGattDeviceService(gattDeviceService))
		{

		}
		private void Initialize()
		{
            //gattDeviceService.Service.Session.SessionStatusChanged += Session_SessionStatusChanged;

            gattDeviceService.PropertyChanged += (s, e) => RaisePropertyChanged(e.PropertyName);
			gattDeviceService.Characteristics.CollectionChanged += Characteristics_CollectionChanged;
		}

        private void Session_SessionStatusChanged(GattSession sender, GattSessionStatusChangedEventArgs args)
        {
            Debug.WriteLine($"SessionStatusChanged. Status=[{args.Status}]");
        }

        private void Characteristics_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			GattCharacteristics = gattDeviceService.Characteristics.Select(C => new BluetoothGattCharacteristicUwp(C)).ToList<IBluetoothGattCharacteristic>();
			GattCharacteristicsUpdated?.Invoke(this, EventArgs.Empty);
		}
	}
}
