using System;
using UnityEngine;

public class BBRFog : MonoBehaviour, BBRImageEffect
{
	public FogMode fogMode;

	private float CAMERA_NEAR = 0.5f;

	private float CAMERA_FAR = 50f;

	private float CAMERA_FOV = 60f;

	private float CAMERA_ASPECT_RATIO = 1.333333f;

	public float startDistance = 1500f;

	public float globalDensity = 0.05f;

	public float heightScale = 500f;

	public float height = -100f;

	public Color globalFogColor = Color.grey;

	private Material fogMaterial;

	public void ApplySetting(string key, string val)
	{
		key = key.ToLower();
		if (key.Equals("start"))
		{
			startDistance = float.Parse(val);
		}
		else if (key.Equals("density"))
		{
			globalDensity = float.Parse(val);
		}
		else if (key.Equals("height scale"))
		{
			heightScale = float.Parse(val);
		}
		else if (key.Equals("height"))
		{
			height = float.Parse(val);
		}
		else if (key.Equals("color"))
		{
			string[] array = val.Split(',');
			globalFogColor = new Color(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]), 1f);
		}
		else if (key.Equals("mode"))
		{
			if (val.Equals("absolue y and dist"))
			{
				fogMode = FogMode.AbsoluteYAndDistance;
			}
			else if (val.Equals("absolue y"))
			{
				fogMode = FogMode.AbsoluteY;
			}
			else if (val.Equals("dist"))
			{
				fogMode = FogMode.Distance;
			}
			else if (val.Equals("relative y and dist"))
			{
				fogMode = FogMode.RelativeYAndDistance;
			}
		}
	}

	private bool CheckResources()
	{
		if (fogMaterial == null)
		{
			fogMaterial = (Material)Resources.Load("ImageEffects/Materials/Fog");
		}
		if (!fogMaterial.shader.isSupported)
		{
			base.enabled = false;
			return false;
		}
		return true;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (!CheckResources())
		{
			Graphics.Blit(source, destination);
			return;
		}
		CAMERA_NEAR = base.GetComponent<Camera>().nearClipPlane;
		CAMERA_FAR = base.GetComponent<Camera>().farClipPlane;
		CAMERA_FOV = base.GetComponent<Camera>().fieldOfView;
		CAMERA_ASPECT_RATIO = base.GetComponent<Camera>().aspect;
		Matrix4x4 identity = Matrix4x4.identity;
		float num = CAMERA_FOV * 0.5f;
		Vector3 vector = base.GetComponent<Camera>().transform.right * CAMERA_NEAR * Mathf.Tan(num * ((float)Math.PI / 180f)) * CAMERA_ASPECT_RATIO;
		Vector3 vector2 = base.GetComponent<Camera>().transform.up * CAMERA_NEAR * Mathf.Tan(num * ((float)Math.PI / 180f));
		Vector3 vector3 = base.GetComponent<Camera>().transform.forward * CAMERA_NEAR - vector + vector2;
		float num2 = vector3.magnitude * CAMERA_FAR / CAMERA_NEAR;
		vector3.Normalize();
		vector3 *= num2;
		Vector3 vector4 = base.GetComponent<Camera>().transform.forward * CAMERA_NEAR + vector + vector2;
		vector4.Normalize();
		vector4 *= num2;
		Vector3 vector5 = base.GetComponent<Camera>().transform.forward * CAMERA_NEAR + vector - vector2;
		vector5.Normalize();
		vector5 *= num2;
		Vector3 vector6 = base.GetComponent<Camera>().transform.forward * CAMERA_NEAR - vector - vector2;
		vector6.Normalize();
		vector6 *= num2;
		identity.SetRow(0, vector3);
		identity.SetRow(1, vector4);
		identity.SetRow(2, vector5);
		identity.SetRow(3, vector6);
		fogMaterial.SetMatrix("_FrustumCornersWS", identity);
		fogMaterial.SetVector("_CameraWS", base.GetComponent<Camera>().transform.position);
		fogMaterial.SetVector("_StartDistance", new Vector4(1f / startDistance, num2 - startDistance));
		fogMaterial.SetVector("_Y", new Vector4(height, 1f / heightScale));
		fogMaterial.SetFloat("_GlobalDensity", globalDensity * 0.01f);
		fogMaterial.SetColor("_FogColor", globalFogColor);
		CustomGraphicsBlit(source, destination, fogMaterial, (int)fogMode);
	}

	public static void CustomGraphicsBlit(RenderTexture src, RenderTexture dest, Material mat, int pass)
	{
		RenderTexture.active = dest;
		mat.SetPass(pass);
		mat.SetTexture("_MainTex", src);
		Graphics.Blit(src, dest, mat);
		GL.PushMatrix();
		GL.LoadOrtho();
		GL.Begin(7);
		GL.MultiTexCoord2(0, 0f, 0f);
		GL.Vertex3(0f, 0f, 3f);
		GL.MultiTexCoord2(0, 1f, 0f);
		GL.Vertex3(1f, 0f, 2f);
		GL.MultiTexCoord2(0, 1f, 1f);
		GL.Vertex3(1f, 1f, 1f);
		GL.MultiTexCoord2(0, 0f, 1f);
		GL.Vertex3(0f, 1f, 0f);
		GL.End();
		GL.PopMatrix();
	}
}
