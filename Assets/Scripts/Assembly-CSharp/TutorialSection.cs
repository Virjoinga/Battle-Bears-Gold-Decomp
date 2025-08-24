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
			sign.GetComponent<Animation>()["Popup"].speed = 0.5f;
			sign.GetComponent<Animation>().Play("Popup");
		}
		Object.Destroy(base.GetComponent<Collider>());
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
		blocker.GetComponent<Animation>().Play("gateClose");
		yield return new WaitForSeconds(blocker.GetComponent<Animation>()["gateClose"].length);
		Object.Destroy(blocker);
	}
}
