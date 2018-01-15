using KD.Navien.WaterBoilerMat.Models;
using Microsoft.Toolkit.Uwp.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KD.Navien.WaterBoilerMat.UWP.Models
{
	public class WaterBoilerMatDeviceUwp : WaterBoilerMatDevice
	{
		#region Properties

		public override string Name => device?.Name;
		public override string Address => device?.BluetoothAddressAsString;

		public override ObservableCollection<IBluetoothGattService> Services
		{
			get { return services ?? (services = new ObservableCollection<IBluetoothGattService>()); }
		}
		private ObservableCollection<IBluetoothGattService> services;

		#endregion

		#region Fields

		private ObservableBluetoothLEDevice device;

		#endregion

		#region Constructors & Initialize

		public WaterBoilerMatDeviceUwp(ObservableBluetoothLEDevice device)
		{
			this.device = device;

			Initialize();
		}

		private void Initialize()
		{
			device.PropertyChanged += (s, e) => RaisePropertyChanged(e.PropertyName);
			device.Services.CollectionChanged += Services_CollectionChanged;
		}
		private void Services_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Reset:
					Services.Clear();
					break;
				case NotifyCollectionChangedAction.Add:
					foreach (var item in e.NewItems.OfType<ObservableGattDeviceService>().Select(S => new BluetoothGattServiceUwp(S)))
					{
						Services.Add(item);
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach (var item in e.NewItems.OfType<ObservableGattDeviceService>().Select(S => new BluetoothGattServiceUwp(S)))
					{
						Services.Remove(item);
					}
					break;
			}
		}

		#endregion

		public override Task ConnectAsync()
		{
			return device.ConnectAsync();
		}
	}
}
