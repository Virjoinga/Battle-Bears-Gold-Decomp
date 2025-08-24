using SkyVu.Common.Enums;
using SkyVu.Common.JsonParser;

namespace SkyVuEngine.Core.Data.Packages
{
	public class EnemyItemPackage : BasePackageItem
	{
		public string Name { get; set; }

		public EnemyItemPackage(PackageItemTypes packageItemType)
			: base(packageItemType)
		{
		}

		public override bool Load(string json)
		{
			try
			{
				if (json == null)
				{
					return false;
				}
				JsonReader jsonReader = new JsonReader(json);
				jsonReader.Read();
				if (jsonReader.Token != JsonToken.ObjectStart)
				{
					return false;
				}
				while (jsonReader.Read() && jsonReader.Token != JsonToken.ObjectEnd)
				{
					string text = (string)jsonReader.Value;
					if (text != null && text == "name")
					{
						jsonReader.Read();
						Name = (string)jsonReader.Value;
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
