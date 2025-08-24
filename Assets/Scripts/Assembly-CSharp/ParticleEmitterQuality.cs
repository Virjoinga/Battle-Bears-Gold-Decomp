using UnityEngine;

public class ParticleEmitterQuality : MonoBehaviour
{
	[SerializeField]
	private float _ultraPercent = 1f;

	[SerializeField]
	private float _highPercent = 1f;

	[SerializeField]
	private float _mediumPercent = 0.8f;

	[SerializeField]
	private float _lowPercent = 0.5f;

	[SerializeField]
	private float _lowestPercent = 0.25f;

	private float _selectedPercent;

	private void Awake()
	{
		switch (BBRQuality.Current)
		{
		case QualitySetting.ULTRA:
			_selectedPercent = _ultraPercent;
			break;
		case QualitySetting.HIGH:
			_selectedPercent = _highPercent;
			break;
		case QualitySetting.MED:
			_selectedPercent = _mediumPercent;
			break;
		case QualitySetting.LOW:
			_selectedPercent = _lowPercent;
			break;
		case QualitySetting.LOWEST:
			_selectedPercent = _lowestPercent;
			break;
		}
		if (base.particleSystem != null)
		{
			base.particleSystem.emissionRate *= _selectedPercent;
		}
	}
}
