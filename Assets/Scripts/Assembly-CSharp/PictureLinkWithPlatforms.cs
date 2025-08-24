using System;

[Serializable]
public class PictureLinkWithPlatforms : PictureLink
{
	public string androidURL;

	public string iosURL;

	public string win8URL;

	public override string URL
	{
		get
		{
			if (!string.IsNullOrEmpty(_url))
			{
				return _url;
			}
			return androidURL;
		}
		set
		{
			base.URL = value;
		}
	}

	public PictureLinkWithPlatforms(string url, string name, string imageURL)
		: base(url, name, imageURL)
	{
	}
}
