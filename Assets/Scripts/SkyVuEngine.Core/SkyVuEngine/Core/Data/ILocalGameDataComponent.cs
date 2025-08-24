namespace SkyVuEngine.Core.Data
{
	public interface ILocalGameDataComponent
	{
		string LabelName { get; set; }

		bool SaveData(object writer, DataType dataType);

		bool LoadData(string key, object reader, DataType dataType);
	}
}
