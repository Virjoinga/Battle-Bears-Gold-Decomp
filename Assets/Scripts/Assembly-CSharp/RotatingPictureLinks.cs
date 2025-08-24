using System.Collections;
using System.Collections.Generic;
using Analytics;
using Analytics.Parameters;
using Analytics.Schemas;
using UnityEngine;

public class RotatingPictureLinks : MonoBehaviour
{
	[SerializeField]
	private PictureLinkWithPlatforms[] _defaultRotatingPictures;

	private PictureLink[] _rotatingPictures;

	[SerializeField]
	private float _timeBetweenPictures = 5f;

	private int _currentPicture;

	private void Start()
	{
		_rotatingPictures = new PictureLink[_defaultRotatingPictures.Length];
		for (int i = 0; i < _defaultRotatingPictures.Length; i++)
		{
			_rotatingPictures[i] = _defaultRotatingPictures[i];
		}
		_defaultRotatingPictures = null;
		if (ServiceManager.Instance != null)
		{
			ServiceManager.Instance.UpdateProperty("pictureRotateTime", ref _timeBetweenPictures);
			PictureLink[] rotatingPictureLinks = ServiceManager.Instance.GetRotatingPictureLinks();
			if (rotatingPictureLinks != null && rotatingPictureLinks.Length > 0)
			{
				PictureLink[] array = rotatingPictureLinks;
				foreach (PictureLink p in array)
				{
					StartCoroutine(grabImage(p));
				}
			}
		}
		NextPicture();
		StartCoroutine(SwapPicture());
	}

	private void NextPicture()
	{
		_currentPicture = ((_currentPicture + 1 < _rotatingPictures.Length) ? (_currentPicture + 1) : 0);
		base.gameObject.GetComponent<Renderer>().material.mainTexture = _rotatingPictures[_currentPicture].picture;
	}

	private IEnumerator SwapPicture()
	{
		while (true)
		{
			yield return new WaitForSeconds(_timeBetweenPictures);
			NextPicture();
		}
	}

	public void OnGUIButtonClicked(GUIButton b)
	{
		if (b.name == "webButton" && !string.IsNullOrEmpty(_rotatingPictures[_currentPicture].URL))
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("url", _rotatingPictures[_currentPicture].URL);
			if (_rotatingPictures[_currentPicture].URL == "http://battlebears.com/store/")
			{
				_rotatingPictures[_currentPicture].URL = "http://sky.vu/bbr2ops";
			}
			EventTracker.TrackEvent(new MainMenuBannerClickedSchema(new MainMenuBannerURLParameter(_rotatingPictures[_currentPicture].URL)));
			Application.OpenURL(_rotatingPictures[_currentPicture].URL);
		}
	}

	private IEnumerator grabImage(PictureLink p)
	{
		if (string.IsNullOrEmpty(p.imageURL))
		{
			for (int j = 0; j < _rotatingPictures.Length; j++)
			{
				if (_rotatingPictures[j].name == p.name)
				{
					_rotatingPictures[j].URL = p.URL;
				}
			}
			yield break;
		}
		yield return new WaitForSeconds(10f);
		if (p == null)
		{
			yield break;
		}
		WWW www = new WWW(p.imageURL);
		yield return www;
		if (www.error != null)
		{
			Debug.LogWarning("WWW download error: " + www.error);
			yield break;
		}
		p.picture = www.texture;
		bool updateFound = false;
		for (int i = 0; i < _rotatingPictures.Length; i++)
		{
			if (_rotatingPictures[i].name == p.name)
			{
				updateFound = true;
				_rotatingPictures[i] = p;
			}
		}
		if (!updateFound)
		{
			AppendPictureLink(p);
		}
	}

	private void AppendPictureLink(PictureLink link)
	{
		PictureLink[] array = new PictureLink[_rotatingPictures.Length + 1];
		for (int i = 0; i < _rotatingPictures.Length; i++)
		{
			array[i] = _rotatingPictures[i];
		}
		array[_rotatingPictures.Length] = link;
		_rotatingPictures = array;
	}
}
