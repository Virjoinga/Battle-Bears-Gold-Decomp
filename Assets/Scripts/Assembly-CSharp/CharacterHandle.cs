using UnityEngine;

public class CharacterHandle : MonoBehaviour
{
	[SerializeField]
	private Material _skin;

	[SerializeField]
	private Renderer[] _renderers;

	[SerializeField]
	private Material _teamMaterial;

	[SerializeField]
	private Renderer[] _teamRenderers;

	public Material Skin
	{
		get
		{
			return _skin;
		}
		set
		{
			_skin = value;
			Renderer[] renderers = _renderers;
			foreach (Renderer renderer in renderers)
			{
				renderer.material = _skin;
			}
		}
	}

	public Renderer[] Renderers
	{
		get
		{
			return _renderers;
		}
	}

	public Material TeamMaterial
	{
		get
		{
			return _teamMaterial;
		}
		set
		{
			_teamMaterial = value;
			Renderer[] teamRenderers = _teamRenderers;
			foreach (Renderer renderer in teamRenderers)
			{
				renderer.sharedMaterial = _teamMaterial;
			}
		}
	}

	public Renderer[] TeamRenderers
	{
		get
		{
			return _teamRenderers;
		}
	}
}
