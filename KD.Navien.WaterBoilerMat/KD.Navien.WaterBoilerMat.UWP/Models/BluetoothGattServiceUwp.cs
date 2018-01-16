using KD.Navien.WaterBoilerMat.Models;
using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace KD.Navien.WaterBoilerMat.UWP.Models
{
	public class BluetoothGattServiceUwp : IBluetoothGattService
	{
		public string UUID => gattDeviceService.UUID;

		public string Name => gattDeviceService.Name;

		public ObservableCollection<IBluetoothGattCharacteristic> GattCharacteristics
		{
			get { return gattCharacteristics ?? (gattCharacteristics = new ObservableCollection<IBluetoothGattCharacteristic>()); }
		}
		private ObservableCollection<IBluetoothGattCharacteristic> gattCharacteristics;

		private ObservableGattDeviceService gattDeviceService;
		

		public BluetoothGattServiceUwp(ObservableGattDeviceService gattDeviceService)
		{
			this.gattDeviceService = gattDeviceService;
			this.gattDeviceService.Characteristics.CollectionChanged += Characteristics_CollectionChanged;
		}
		public BluetoothGattServiceUwp(GattDeviceService gattDeviceService) : this(new ObservableGattDeviceService(gattDeviceService))
		{

		}

		private void Characteristics_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Reset:
					GattCharacteristics.Clear();
					break;
				case NotifyCollectionChangedAction.Add:
					foreach (var item in e.NewItems.OfType<ObservableGattCharacteristics>().Select(C => new BluetoothGattCharacteristicUwp(C)))
					{
						GattCharacteristics.Add(item);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (var item in e.NewItems.OfType<ObservableGattCharacteristics>())
					{
						var existItem = GattCharacteristics.FirstOrDefault(C => C.UUID.Equals(item.UUID));
						GattCharacteristics.Remove(existItem);
					}
					break;
			}
		}
	}
}
