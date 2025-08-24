using System.Collections;
using UnityEngine;

public class TutorialSection : MonoBehaviour
{
	public GameObject sign;

	public GameObject blocker;

	public TutorialDamageReceiver[] targets;

	public GameObject sectionText;

	private void Awake()
	{
		if (blocker == null)
		{
			base.enabled = false;
		}
	}

	private void OnTriggerEnter(Collider c)
	{
		if (sectionText != null)
		{
			sectionText.transform.parent.gameObject.SetActive(false);
			sectionText.SetActive(true);
		}
		if (sign != null)
		{
			sign.animation["Popup"].speed = 0.5f;
			sign.animation.Play("Popup");
		}
		Object.Destroy(base.collider);
		Tutorial.Instance.OnNextSection();
		if (sign == null)
		{
			Tutorial.Instance.OnFinished();
		}
	}

	private void Update()
	{
		bool flag = true;
		for (int i = 0; i < targets.Length; i++)
		{
			if (targets[i] != null)
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			StartCoroutine(delayedCloseGate());
			base.enabled = false;
		}
	}

	private IEnumerator delayedCloseGate()
	{
		blocker.animation.Play("gateClose");
		yield return new WaitForSeconds(blocker.animation["gateClose"].length);
		Object.Destroy(blocker);
	}
}
