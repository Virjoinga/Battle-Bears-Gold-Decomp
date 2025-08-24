using System;
using System.Collections;
using TapjoyUnity;
using UnityEngine;

public class TapjoyAd
{
	private MonoBehaviour _monoBehaviour;

	private AdType _type;

	private bool _showWhenFetched;

	private bool _contentFetched;

	private bool _isFetchingContent;

	private TJPlacement _placement;

	public AdType Type
	{
		get
		{
			return _type;
		}
	}

	private event Action<TapjoyAd> _placementCompleted;

	public event Action<TapjoyAd> PlacementCompleted
	{
		add
		{
			this._placementCompleted = (Action<TapjoyAd>)Delegate.Combine(this._placementCompleted, value);
		}
		remove
		{
			this._placementCompleted = (Action<TapjoyAd>)Delegate.Remove(this._placementCompleted, value);
		}
	}

	public TapjoyAd(AdType type, MonoBehaviour mono)
	{
		_type = type;
		_monoBehaviour = mono;
		_placement = TJPlacement.CreatePlacement(type.ToString());
		TJPlacement.OnRequestSuccess += HandleContentRequestSucceeded;
		TJPlacement.OnRequestFailure += HandleRequestFailure;
		TJPlacement.OnContentDismiss += HandleContentDismissed;
	}

	private void RaisePlacementCompleted()
	{
		if (this._placementCompleted != null)
		{
			this._placementCompleted(this);
		}
	}

	private void HandleContentRequestSucceeded(TJPlacement placement)
	{
		if (placement == _placement)
		{
			_isFetchingContent = false;
			_contentFetched = true;
			if (_showWhenFetched)
			{
				TryShowFetchedContent();
			}
		}
	}

	private void TryShowFetchedContent()
	{
		if (_placement.IsContentAvailable())
		{
			_monoBehaviour.StartCoroutine(ShowAdRoutine());
			return;
		}
		Debug.LogWarning("No content available for Tapjoy placement " + _type);
		RaisePlacementCompleted();
	}

	private void HandleRequestFailure(TJPlacement placement, string error)
	{
		if (placement == _placement)
		{
			Debug.LogError("Requesting content for Tapjoy placement " + _type.ToString() + " failed with error:");
			Debug.LogError(error);
			RaisePlacementCompleted();
		}
	}

	private void HandleContentDismissed(TJPlacement placement)
	{
		if (placement == _placement)
		{
			RaisePlacementCompleted();
		}
	}

	private IEnumerator ShowAdRoutine()
	{
		while (!_placement.IsContentReady())
		{
			yield return null;
		}
		_placement.ShowContent();
	}

	public void Fetch(bool showWhenFetched)
	{
		_showWhenFetched = showWhenFetched;
		_isFetchingContent = true;
		_placement.RequestContent();
	}

	public void Show()
	{
		if (_isFetchingContent)
		{
			_showWhenFetched = true;
		}
		else if (_contentFetched)
		{
			TryShowFetchedContent();
		}
		else
		{
			Fetch(true);
		}
	}
}
