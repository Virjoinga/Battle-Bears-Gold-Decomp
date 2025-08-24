namespace SkyVu.Common.JsonParser
{
	internal delegate object ImporterFunc(object input);
	public delegate TValue ImporterFunc<TJson, TValue>(TJson input);
}
