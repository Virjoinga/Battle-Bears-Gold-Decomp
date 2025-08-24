using UnityEngine;

public class AutofillWithLogin : MonoBehaviour
{
	public InputField inputField;

	private void Start()
	{
		if (inputField != null && LoginManager.Instance != null)
		{
			inputField.actualString = LoginManager.Instance.loginAttemptUsername;
			inputField.textMesh.text = LoginManager.Instance.loginAttemptUsername;
		}
		Object.Destroy(this);
	}
}
