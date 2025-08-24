using UnityEngine;

public class Skin
{
	private string _highBluePath;

	private Material _highBlue;

	private string _highRedPath;

	private Material _highRed;

	private string _lowBluePath;

	private Material _lowBlue;

	private string _lowRedPath;

	private Material _lowRed;

	public string Name { get; private set; }

	public Material HighBlue
	{
		get
		{
			if (_highBlue == null)
			{
				_highBlue = Resources.Load(_highBluePath) as Material;
			}
			return _highBlue;
		}
	}

	public Material HighRed
	{
		get
		{
			if (_highRed == null)
			{
				_highRed = Resources.Load(_highRedPath) as Material;
			}
			return _highRed;
		}
	}

	public Material LowBlue
	{
		get
		{
			if (_lowBlue == null)
			{
				_lowBlue = Resources.Load(_lowBluePath) as Material;
			}
			return _lowBlue;
		}
	}

	public Material LowRed
	{
		get
		{
			if (_lowRed == null)
			{
				_lowRed = Resources.Load(_lowRedPath) as Material;
			}
			return _lowRed;
		}
	}

	public Skin(string name, string highBlue, string highRed, string lowBlue, string lowRed)
	{
		Name = name.Substring(0, 1).ToUpper() + name.Substring(1);
		_highBluePath = highBlue;
		_highRedPath = highRed;
		_lowBluePath = lowBlue;
		_lowRedPath = lowRed;
	}
}
