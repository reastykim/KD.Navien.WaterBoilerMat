using System;
using KD.Navien.WaterBoilerMat.Services;
using KD.Navien.WaterBoilerMat.Tizen.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Diagnostics;
using Prism.Logging;

[assembly: Dependency(typeof(BluetoothService))]
namespace KD.Navien.WaterBoilerMat.Tizen.Services
{
	public class BluetoothService : IBluetoothService
	{
		private ILoggerFacade logger;

		public BluetoothService(ILoggerFacade logger)
		{
			this.logger = logger;
		}

		public Task ScanAsync(int timeoutMilliseconds)
		{
			logger?.Log($"Call ScanAsync({timeoutMilliseconds})", Category.Debug, Priority.Medium);
			return Task.CompletedTask;
		}
	}
}