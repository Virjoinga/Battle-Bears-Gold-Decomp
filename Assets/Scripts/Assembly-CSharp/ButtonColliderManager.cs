using UnityEngine;

public class ButtonColliderManager : MonoBehaviour
{
	public ButtonGroup[] buttonGroups;

	public void SetGroupActive(string groupName)
	{
		ButtonGroup[] array = buttonGroups;
		foreach (ButtonGroup buttonGroup in array)
		{
			if (groupName == buttonGroup.buttonGroupName)
			{
				Collider[] buttonColliders = buttonGroup.buttonColliders;
				foreach (Collider collider in buttonColliders)
				{
					if (collider == null)
					{
						Debug.LogError("Null collider found in: " + buttonGroup.buttonGroupName);
					}
					else
					{
						collider.enabled = true;
					}
				}
				continue;
			}
			Collider[] buttonColliders2 = buttonGroup.buttonColliders;
			foreach (Collider collider2 in buttonColliders2)
			{
				if (collider2 == null)
				{
					Debug.LogError("Null collider found in: " + buttonGroup.buttonGroupName);
				}
				else
				{
					collider2.enabled = false;
				}
			}
		}
	}
}
