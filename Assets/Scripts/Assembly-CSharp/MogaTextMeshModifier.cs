using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class MogaTextMeshModifier : MonoBehaviour
{
	public string appendedText;

	private void Start()
	{
		if (MogaController.Instance.connection == 1)
		{
			GetComponent<TextMesh>().text += appendedText;
		}
	}
}
