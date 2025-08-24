using System.Collections.Generic;

namespace SkyVuEngine.Core.Powerups
{
	public class ObjectProfile
	{
		public Dictionary<string, float> profile;

		public ObjectProfile()
		{
			profile = new Dictionary<string, float>();
		}

		public ObjectProfile(Dictionary<string, float> profile)
		{
			this.profile = profile;
		}

		public void ApplyPowerups(Dictionary<string, float> powerup)
		{
			if (powerup == null)
			{
				return;
			}
			foreach (KeyValuePair<string, float> item in powerup)
			{
				if (profile.ContainsKey(item.Key))
				{
					profile[item.Key] = profile[item.Key] * item.Value;
				}
			}
		}
	}
}
