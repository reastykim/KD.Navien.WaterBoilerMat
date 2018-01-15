﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace KD.Navien.WaterBoilerMat.Models
{
    public interface IBluetoothGattService
    {
		string UUID { get; }
		string Name { get; }

		ObservableCollection<IBluetoothGattCharacteristic> GattCharacteristics { get; }
	}
}
