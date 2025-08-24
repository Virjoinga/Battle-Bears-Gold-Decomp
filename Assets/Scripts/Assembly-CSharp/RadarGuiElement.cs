using System;
using System.Collections.Generic;
using UnityEngine;

public class RadarGuiElement : MonoBehaviour, GUIComponent, RenderedGUIComponent, UpdatedGUIComponent
{
	public static RadarGuiElement Instance;

	private Dictionary<string, RadarBlip> _radarBlips;

	private Dictionary<string, RadarBlip> _fadingBlips;

	private List<string> _blipsToRemove;

	private List<string> _fadingBlipsToRemove;

	private Texture2D _blipTexture;

	private AnimatedSprite _radarBase;

	private Rect _radarBaseRect;

	private float _blipWorldRadiusSqr;

	private PlayerGUI _playerGUI;

	[SerializeField]
	private float _renderRadius = 1000f;

	private float _renderRadiusSqr;

	[SerializeField]
	private float _radarScreenRadius = 75f;

	[SerializeField]
	private string _radarTexturePath = "Textures/GUI/radarBaseSheet";

	private string _blipAnimationName = "blip";

	[SerializeField]
	private int _blipAnimNumberOfFrames = 8;

	[SerializeField]
	private float _blipAnimTimeBetweenFrames = 0.03f;

	[SerializeField]
	private float _blipAnimRestartPauseTime = 2f;

	[SerializeField]
	private Vector2 _blipFrameSize = new Vector2(16f, 16f);

	[SerializeField]
	private Vector2 _radarBaseFrameSize = new Vector2(256f, 256f);

	private Vector2 _radarScreenPosition;

	public PlayerController LocalPlayerController { get; set; }

	public Rect RadarBaseRect
	{
		get
		{
			return _radarBaseRect;
		}
	}

	public float RenderDepth { get; set; }

	public float RenderRadius
	{
		get
		{
			return _renderRadius;
		}
		set
		{
			_renderRadius = value;
			_renderRadiusSqr = _renderRadius * _renderRadius;
			_blipWorldRadiusSqr = (_blipFrameSize / _radarScreenRadius * _renderRadius).sqrMagnitude;
		}
	}

	private void Start()
	{
		Instance = this;
		_renderRadiusSqr = _renderRadius * _renderRadius;
		_radarBlips = new Dictionary<string, RadarBlip>();
		_fadingBlips = new Dictionary<string, RadarBlip>();
		_blipsToRemove = new List<string>();
		_fadingBlipsToRemove = new List<string>();
		_radarBase = new AnimatedSprite(Resources.Load(_radarTexturePath) as Texture2D, _radarBaseFrameSize);
		_radarBase.CreateAnimation(_blipAnimationName, Vector2.zero, _blipAnimNumberOfFrames, _blipAnimTimeBetweenFrames, WrapMode.Loop, _blipAnimRestartPauseTime);
		_radarBase.Play(_blipAnimationName);
		_blipWorldRadiusSqr = (_blipFrameSize / _radarScreenRadius * _renderRadius).sqrMagnitude;
	}

	public void AddTo(PlayerGUI gui)
	{
		gui.AddRenderedComponent(RenderDepth, this);
		gui.AddUpdatedComponent(this);
		_playerGUI = gui;
		PositionGUI();
	}

	public void RemoveFrom(PlayerGUI gui)
	{
		gui.RemoveRenderedComponent(this);
		gui.RemoveUpdatedComponent(this);
		_playerGUI = null;
	}

	public void RenderGUI()
	{
		if (!(HUD.Instance.currentPauseMenu == null) || !(HUD.Instance.currentStatsOverlay == null) || !(LocalPlayerController != null))
		{
			return;
		}
		_radarBase.Draw(_radarBaseRect, Preferences.Instance.HUDColor);
		foreach (RadarBlip value in _fadingBlips.Values)
		{
			value.Draw(_radarScreenPosition, _radarScreenRadius, _blipFrameSize);
		}
		foreach (RadarBlip value2 in _radarBlips.Values)
		{
			value2.Draw(_radarScreenPosition, _radarScreenRadius, _blipFrameSize);
		}
	}

	public void UpdateGUI(float deltaTime)
	{
		if (HUD.Instance.PlayerController != null && CustomLayoutController.Instance.IsOpen)
		{
			PositionGUI();
		}
		_radarBase.Update();
		if (LocalPlayerController != null)
		{
			UpdateBlipList();
			UpdateBlipPositions();
		}
	}

	public void PositionGUI()
	{
		_radarScreenPosition.x = GUIPositionController.Instance.RadarPercentLocation.x * (float)Screen.width;
		_radarScreenPosition.y = GUIPositionController.Instance.RadarPercentLocation.y * (float)Screen.height;
		if (_playerGUI != null)
		{
			_radarBaseRect = new Rect(_radarScreenPosition.x, _radarScreenPosition.y, _radarScreenRadius * 2f * _playerGUI.SmallestRatio, _radarScreenRadius * 2f * _playerGUI.SmallestRatio);
		}
	}

	private void UpdateBlipList()
	{
		foreach (RadarBlip value2 in _radarBlips.Values)
		{
			if (value2.Tracker == null || value2.OwningObject == null)
			{
				RemoveBlip(value2.ToString());
			}
			else if (value2.OwningObject != null && LocalPlayerController != null)
			{
				if (!InRangeOfRadar(value2.GetWorldPosition()) || !value2.Tracker.VisibleOnRadar)
				{
					RemoveBlip(value2.ToString());
				}
				else
				{
					value2.Update();
				}
			}
		}
		RemoveScheduledBlipsAndAddToFade();
		UpdateFadingBlips();
		RemoveScheduledFadingBlips();
		List<RadarTracker> trackers = RadarTrackerManager.Instance.Trackers;
		if (trackers == null)
		{
			return;
		}
		foreach (RadarTracker item in trackers)
		{
			if (item.VisibleOnRadar && item.gameObject != null && !_radarBlips.ContainsKey(item.gameObject.name) && InRangeOfRadar(item.transform.position))
			{
				RadarBlip value = new RadarBlip(item.gameObject, item, item.BlipColor, item.BlipTexture, LocalPlayerController, _blipFrameSize);
				_fadingBlips.Remove(item.gameObject.name);
				_radarBlips.Add(item.gameObject.name, value);
			}
		}
	}

	private void UpdateFadingBlips()
	{
		foreach (RadarBlip value in _fadingBlips.Values)
		{
			value.Update();
		}
	}

	public void RemoveBlip(string name)
	{
		_blipsToRemove.Add(name);
	}

	private void RemoveScheduledBlips()
	{
		foreach (string item in _blipsToRemove)
		{
			_radarBlips.Remove(item);
		}
		_blipsToRemove.Clear();
	}

	private void RemoveScheduledBlipsAndAddToFade()
	{
		foreach (string item in _blipsToRemove)
		{
			if (_radarBlips.ContainsKey(item))
			{
				RadarBlip radarBlip = _radarBlips[item];
				radarBlip.PlayFadeOut();
				if (radarBlip.OwningObject != null)
				{
					_fadingBlips.Remove(radarBlip.ToString());
					_fadingBlips.Add(radarBlip.ToString(), radarBlip);
				}
			}
			_radarBlips.Remove(item);
		}
		_blipsToRemove.Clear();
	}

	public void RemoveFadingBlip(string name)
	{
		_fadingBlipsToRemove.Add(name);
	}

	private void RemoveScheduledFadingBlips()
	{
		foreach (string item in _fadingBlipsToRemove)
		{
			_fadingBlips.Remove(item);
		}
		_fadingBlipsToRemove.Clear();
	}

	private void UpdateBlipPositions()
	{
		if (_radarBlips.Count <= 0)
		{
			return;
		}
		Vector2 vector = new Vector2(LocalPlayerController.transform.forward.x, LocalPlayerController.transform.forward.z);
		float num = Vector2.Angle(Vector2.up, vector);
		if (Vector3.Cross(Vector2.up, vector).z > 0f)
		{
			num = 360f - num;
		}
		num *= (float)Math.PI / 180f;
		foreach (string key in _radarBlips.Keys)
		{
			RadarBlip radarBlip = _radarBlips[key];
			if (radarBlip.OwningObject != null)
			{
				radarBlip.RadarPosition = GetRadarPosition(num, LocalPlayerController.transform.position, radarBlip.GetWorldPosition());
			}
			else
			{
				RemoveBlip(key);
			}
		}
	}

	private Vector2 GetRadarPosition(float radianTheta, Vector3 localPlayerPosition, Vector3 playerWorldPosition)
	{
		Vector2 vector = new Vector2((localPlayerPosition.x - playerWorldPosition.x) / _renderRadius, (localPlayerPosition.z - playerWorldPosition.z) / _renderRadius);
		float num = Mathf.Cos(radianTheta);
		float num2 = Mathf.Sin(radianTheta);
		float num3 = vector.x * num - vector.y * num2;
		float y = vector.x * num2 + vector.y * num;
		return new Vector2(0f - num3, y);
	}

	private bool InRangeOfRadar(Vector3 worldObjectPosition)
	{
		return (LocalPlayerController.transform.position - worldObjectPosition).sqrMagnitude + _blipWorldRadiusSqr < _renderRadiusSqr;
	}
}
