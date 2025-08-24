using UnityEngine;

public class AlwaysOnRadarTracker : RadarTracker
{
	private static readonly string _kothBlipTexturePath = "Textures/GUI/kothPointBlip";

	protected override void Awake()
	{
		base.Awake();
	}

	private void Start()
	{
		base.BlipColor = Color.green;
	}

	protected override void Update()
	{
		SetVisible();
	}
}
