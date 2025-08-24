using System.Collections.Generic;
using System.Linq;
using SkyVuEngine.SkyVuNetwork.Client.Entities;

namespace SkyVuEngine.SkyVuNetwork.Client
{
	public class JsonHelper
	{
		public static string ConvertToJsonArray(IEnumerable<BaseEntity> entities)
		{
			if (entities.Count() == 0)
			{
				return null;
			}
			string text = "[";
			foreach (BaseEntity entity in entities)
			{
				string text2 = entity.Serialize();
				if (text2 != null)
				{
					text = text + text2 + ",";
				}
			}
			text = text.Remove(text.LastIndexOf(","), 1);
			return text + "]";
		}
	}
}
