using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tizen;
namespace KD.Navien.WaterBoilerMat.Tizen.Logging
{
	public class TizenLogger : ILoggerFacade
	{
		public void Log(string message, Category category, Priority priority)
		{
			switch (category)
			{
				case Category.Debug:
					global::Tizen.Log.Debug(nameof(TizenLogger), message);
					break;
				case Category.Info:
					global::Tizen.Log.Info(nameof(TizenLogger), message);
					break;
				case Category.Warn:
					global::Tizen.Log.Warn(nameof(TizenLogger), message);
					break;
				case Category.Exception:
					global::Tizen.Log.Error(nameof(TizenLogger), message);
					break;
			}
		}
	}
}
