using System.Collections;
using UnityEngine;

public class LOD : MonoBehaviour
{
	private enum LODLevel
	{
		LOW = 0,
		MEDIUM = 1,
		HIGH = 2,
		NONE = 3
	}

	public float medDist = 600f;

	public float lowDist = 800f;

	public Mesh high;

	public Mesh med;

	public Mesh low;

	private Transform myTransform;

	private Transform cameraTransform;

	private SkinnedMeshRenderer myRenderer;

	private MeshFilter myFilter;

	private LODLevel lastLODLevel = LODLevel.NONE;

	private void Awake()
	{
		myRenderer = GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
		myFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
	}

	private void Start()
	{
		myTransform = base.transform;
		StartCoroutine(lodChecker());
	}

	public void setHighMesh()
	{
		if (myRenderer != null)
		{
			myRenderer.sharedMesh = null;
			myRenderer.sharedMesh = high;
		}
		else if (myFilter != null)
		{
			myFilter.mesh = high;
		}
	}

	private IEnumerator lodChecker()
	{
		while (HUD.Instance == null || (HUD.Instance != null && HUD.Instance.PlayerCamera == null))
		{
			yield return new WaitForSeconds(0.1f);
		}
		if (!(HUD.Instance != null))
		{
			yield break;
		}
		cameraTransform = HUD.Instance.PlayerCamera;
		while (true)
		{
			if (cameraTransform != null && myTransform != null)
			{
				float dist = Vector3.Distance(cameraTransform.position, myTransform.position);
				if (dist > lowDist && lastLODLevel != 0)
				{
					lastLODLevel = LODLevel.LOW;
					if (myRenderer != null)
					{
						myRenderer.sharedMesh = null;
						myRenderer.sharedMesh = low;
					}
					else if (myFilter != null)
					{
						myFilter.mesh = low;
					}
				}
				else if (dist > medDist && dist < lowDist && lastLODLevel != LODLevel.MEDIUM)
				{
					lastLODLevel = LODLevel.MEDIUM;
					if (myRenderer != null)
					{
						myRenderer.sharedMesh = null;
						myRenderer.sharedMesh = med;
					}
					else if (myFilter != null)
					{
						myFilter.mesh = med;
					}
				}
				else if (dist <= medDist && lastLODLevel != LODLevel.HIGH)
				{
					lastLODLevel = LODLevel.HIGH;
					if (myRenderer != null)
					{
						myRenderer.sharedMesh = null;
						myRenderer.sharedMesh = high;
					}
					else if (myFilter != null)
					{
						myFilter.mesh = high;
					}
				}
			}
			yield return new WaitForSeconds(0.25f);
		}
	}
}
