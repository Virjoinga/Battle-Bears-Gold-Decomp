using System.Collections;
using UnityEngine;

public class BombTimer : MonoBehaviour
{
	private Renderer timeRenderer;

	private Shader[] shaders;

	public Renderer[] transparentRenderers;

	private ParticleSystem particles;

	private Transform myTransform;

	private Shader transparentShader;

	public Team team;

	private bool canSeeBomb = true;

	private void Awake()
	{
		myTransform = base.transform;
		transparentShader = Shader.Find("CPX_Custom/Mobile/VertexLit Transparent No Z GLSL");
		shaders = new Shader[transparentRenderers.Length];
		for (int i = 0; i < transparentRenderers.Length; i++)
		{
			shaders[i] = transparentRenderers[i].material.shader;
		}
		timeRenderer = myTransform.Find("timerRing").renderer;
		particles = myTransform.Find("pulse").gameObject.GetComponent<ParticleSystem>();
		timeRenderer.enabled = false;
		StartCoroutine(UpdateTimeLeftRatio());
	}

	public void OnStartPulse()
	{
		StartCoroutine(periodicPulse());
	}

	private IEnumerator periodicPulse()
	{
		while (true)
		{
			particles.Emit(1);
			if (Camera.mainCamera != null)
			{
				Vector3 cameraPos = Camera.mainCamera.transform.position;
				float distance = Vector3.Distance(cameraPos, myTransform.position);
				float maxSize = 1000f;
				float minSize = 150f;
				particles.startSize = minSize + distance / 2000f * (maxSize - minSize);
			}
			yield return new WaitForSeconds(2f);
		}
	}

	private IEnumerator UpdateTimeLeftRatio()
	{
		while (CTFManager.Instance != null && timeRenderer != null)
		{
			int timeLeft2 = 0;
			timeLeft2 = ((team != 0) ? CTFManager.Instance.BlueTimeLeft : CTFManager.Instance.RedTimeLeft);
			if (timeLeft2 > 0)
			{
				if (!timeRenderer.enabled)
				{
					timeRenderer.enabled = true;
				}
				float timeLeftRatio = (float)timeLeft2 / (float)CTFManager.Instance.EXPLODE_TIME;
				float maxValue = 0.51f;
				timeRenderer.material.mainTextureOffset = new Vector2(maxValue - maxValue * timeLeftRatio, 0f);
			}
			yield return new WaitForSeconds(1f);
		}
	}

	private void checkCameraVisibility()
	{
		Vector3 position = Camera.mainCamera.transform.position;
		Vector3 vector = myTransform.position - position;
		Ray ray = new Ray(position, vector.normalized);
		LayerMask layerMask = 1 << LayerMask.NameToLayer("Wall");
		if (Physics.Raycast(ray, vector.magnitude, layerMask))
		{
			if (canSeeBomb)
			{
				canSeeBomb = false;
				for (int i = 0; i < transparentRenderers.Length; i++)
				{
					transparentRenderers[i].material.shader = transparentShader;
				}
			}
		}
		else if (!canSeeBomb)
		{
			canSeeBomb = true;
			for (int j = 0; j < transparentRenderers.Length; j++)
			{
				transparentRenderers[j].material.shader = shaders[j];
			}
		}
	}
}
