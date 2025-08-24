using UnityEngine;

public class RadarBlip
{
	private Vector2 _radarPosition;

	private GameObject _owningObject;

	private RadarTracker _tracker;

	private string _name;

	private AnimatedSprite _blipSprite;

	private Rect _drawPos;

	private Color _tintColor;

	private PlayerGUI _playerGUI;

	private static string _fadeInAnimName = "fadeIn";

	private static Vector2 _fadeInAnimStartPosition = new Vector2(0f, 0f);

	private static int _fadeInAnimNumberOfFrames = 4;

	private static float _fadeInAnimTimeBetweenFrames = 0.05f;

	private static string _fadeOutAnimName = "fadeOut";

	private static Vector2 _fadeOutAnimStartPosition = new Vector2(48f, 0f);

	private static int _fadeOutAnimNumberOfFrames = 4;

	private static float _fadeOutAnimTimeBetweenFrames = 0.05f;

	private static int _fadeOutAnimXDir = -1;

	private static int _fadeOutAnimYDir = 1;

	public Vector2 RadarPosition
	{
		get
		{
			return _radarPosition;
		}
		set
		{
			_radarPosition = value;
		}
	}

	public GameObject OwningObject
	{
		get
		{
			return _owningObject;
		}
	}

	public RadarTracker Tracker
	{
		get
		{
			return _tracker;
		}
	}

	public RadarBlip(GameObject owningObject, RadarTracker tracker, Color blipColor, Texture2D blipTexture, PlayerController localPlayer)
		: this(owningObject, tracker, blipColor, blipTexture, localPlayer, new Vector2(blipTexture.width, blipTexture.height))
	{
	}

	public RadarBlip(GameObject owningObject, RadarTracker tracker, Color blipColor, Texture2D blipTexture, PlayerController localPlayer, Vector2 spriteSize)
	{
		_name = owningObject.name;
		_owningObject = owningObject;
		_tracker = tracker;
		_tintColor = blipColor;
		_blipSprite = new AnimatedSprite(blipTexture, spriteSize);
		_blipSprite.CreateAnimation(_fadeInAnimName, _fadeInAnimStartPosition, _fadeInAnimNumberOfFrames, _fadeInAnimTimeBetweenFrames, WrapMode.Once, 0f);
		_blipSprite.CreateAnimation(_fadeOutAnimName, _fadeOutAnimStartPosition, _fadeOutAnimNumberOfFrames, _fadeOutAnimTimeBetweenFrames, WrapMode.Once, 0f, _fadeOutAnimXDir, _fadeOutAnimYDir);
		_blipSprite.Play(_fadeInAnimName);
		_playerGUI = localPlayer.PlayersGUI;
	}

	public Vector3 GetWorldPosition()
	{
		return _owningObject.transform.position;
	}

	public void PlayFadeOut()
	{
		_blipSprite.Play(_fadeOutAnimName, RemoveFromRadar);
	}

	public void RemoveFromRadar()
	{
		RadarGuiElement.Instance.RemoveFadingBlip(_name);
	}

	public void Update()
	{
		_blipSprite.Update();
	}

	public void Draw(Vector2 radarScreenPosition, float radarScreenRadius, Vector2 size)
	{
		float num = radarScreenRadius * _playerGUI.SmallestRatio;
		float num2 = radarScreenPosition.x + num + _radarPosition.x * num;
		float num3 = radarScreenPosition.y + num + _radarPosition.y * num;
		float num4 = size.x * _playerGUI.SmallestRatio;
		float num5 = size.y * _playerGUI.SmallestRatio;
		_blipSprite.Draw(new Rect(num2 - num4 / 2f, num3 - num5 / 2f, num4, num5), _tintColor);
	}

	public override string ToString()
	{
		return _name;
	}
}
