using UnityEngine;

public class TextBlock : MonoBehaviour
{
	private TextMesh ourTextMesh;

	private Collider myCollider;

	public bool heightClamp;

	private Transform myTransform;

	public string startText = string.Empty;

	private FontMetrics metric;

	private float originalCharacterSize;

	private void Awake()
	{
		myTransform = base.transform;
		myCollider = base.GetComponent<Collider>();
		if (startText != string.Empty)
		{
			OnSetText(startText, string.Empty);
		}
	}

	public void OnSetText(string text, string color = "")
	{
		setText(text, color);
	}

	private void setText(string s, string color)
	{
		if (ourTextMesh == null)
		{
			ourTextMesh = GetComponent(typeof(TextMesh)) as TextMesh;
			if (ourTextMesh == null)
			{
				Debug.LogError("could not find textmesh");
				return;
			}
			if (!string.IsNullOrEmpty(color) && ourTextMesh.GetComponent<Renderer>() != null)
			{
				ourTextMesh.GetComponent<Renderer>().material = Resources.Load("Materials/All/HELVETICA_" + color) as Material;
			}
		}
		metric = new FontMetrics(ourTextMesh, ourTextMesh.characterSize);
		Vector3 eulerAngles = myTransform.eulerAngles;
		myTransform.eulerAngles = Vector3.zero;
		ourTextMesh.text = metric.Format(s, myCollider.bounds, 0, myTransform, heightClamp);
		myTransform.eulerAngles = eulerAngles;
		if (!heightClamp)
		{
			Object.DestroyImmediate(myCollider);
			base.gameObject.AddComponent(typeof(BoxCollider));
		}
	}
}
