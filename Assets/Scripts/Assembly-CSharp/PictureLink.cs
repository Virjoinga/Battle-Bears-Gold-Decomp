using System;
using UnityEngine;

[Serializable]
public class PictureLink
{
	public string name;

	[HideInInspector]
	public string imageURL;

	public Texture2D picture;

	protected string _url;

	public virtual string URL
	{
		get
		{
			return _url;
		}
		set
		{
			_url = value;
		}
	}

	public PictureLink(string url, string name, string imageURL)
	{
		if (!string.IsNullOrEmpty(url))
		{
			_url = url;
		}
		this.name = name;
		this.imageURL = imageURL;
	}
}
