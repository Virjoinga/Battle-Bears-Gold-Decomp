using SkyVu.Common.Enums;

namespace SkyVuEngine.Core.Data.Packages
{
	public abstract class BasePackageItem
	{
		public PackageItemTypes PackageItemType { get; set; }

		public BasePackageItem(PackageItemTypes packageItemType)
		{
			PackageItemType = packageItemType;
		}

		public virtual bool Load(string json)
		{
			return false;
		}
	}
}
