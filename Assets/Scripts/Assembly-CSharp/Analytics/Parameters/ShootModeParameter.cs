using System;

namespace Analytics.Parameters
{
	public class ShootModeParameter : StringParameter
	{
		public enum Mode
		{
			BUTTON = 0,
			DOUBLE_TAP = 1,
			DUAL_JOYSTICK = 2
		}

		protected override AnalyticsParameter _parameter
		{
			get
			{
				return AnalyticsParameter.shootMode;
			}
		}

		public ShootModeParameter(ShootMode mode)
			: base(FromShootMode(mode).ToString())
		{
		}

		private static Mode FromShootMode(ShootMode mode)
		{
			switch (mode)
			{
			case ShootMode.doubleTap:
				return Mode.DOUBLE_TAP;
			case ShootMode.shootButton:
			case ShootMode.keyboardAndMouse:
				return Mode.BUTTON;
			case ShootMode.dualJoystick:
				return Mode.DUAL_JOYSTICK;
			default:
				throw new Exception("No shoot mode defined for " + mode);
			}
		}
	}
}
