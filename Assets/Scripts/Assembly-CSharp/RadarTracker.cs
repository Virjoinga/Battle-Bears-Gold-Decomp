using UnityEngine;

public class RadarTracker : MonoBehaviour
{
	private static readonly Vector3 EPSILON = Vector3.one;

	private readonly float _visibleDuration = 0.5f;

	private float _lastVisibleStartTime;

	private Vector3 _lastPosition;

	private bool _visibleOnRadar;

	private Color _blipColor = Color.red;

	protected string _blipTexturePath = "Textures/GUI/radarBlip";

	private Texture2D _blipTexture;

	public DamageReceiver CloakHandle { get; set; }

	public Vector3 LastPosition
	{
		get
		{
			return _lastPosition;
		}
		set
		{
			_lastPosition = value;
		}
	}

	public bool VisibleOnRadar
	{
		get
		{
			return _visibleOnRadar;
		}
	}

	public Color BlipColor
	{
		get
		{
			return _blipColor;
		}
		set
		{
			_blipColor = value;
		}
	}

	public Texture2D BlipTexture
	{
		get
		{
			return _blipTexture;
		}
		set
		{
			_blipTexture = value;
		}
	}

	public void SetVisible()
	{
		_visibleOnRadar = true;
		_lastVisibleStartTime = Time.fixedTime;
	}

	protected virtual void Awake()
	{
		RadarTrackerManager.Instance.AddRadarTracker(this);
		_blipTexture = Resources.Load(_blipTexturePath) as Texture2D;
	}

	protected virtual void Update()
	{
		Vector3 position = base.transform.position;
		if ((CloakHandle == null || !CloakHandle.IsCloaking) && (position - _lastPosition).sqrMagnitude > EPSILON.sqrMagnitude)
		{
			SetVisible();
		}
		else if (_visibleOnRadar && Time.fixedTime - _lastVisibleStartTime > _visibleDuration)
		{
			_visibleOnRadar = false;
		}
		_lastPosition = position;
	}

	private void OnDestroy()
	{
		RadarTrackerManager.Instance.RemoveRadarTracker(this);
	}
}
