using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class GrowVerticalToChildrenHeightFromVerticalLayoutGroupSelfLayoutElement : MonoBehaviour, ILayoutSelfController, ILayoutController
{
	[SerializeField]
	private VerticalLayoutGroup _verticalLayoutGroup;

	public void SetLayoutHorizontal()
	{
	}

	private void Update()
	{
		SetLayoutVertical();
	}

	public void SetLayoutVertical()
	{
		if (base.transform.childCount < 1)
		{
			return;
		}
		_verticalLayoutGroup = _verticalLayoutGroup ?? GetComponent<VerticalLayoutGroup>();
		float num = 0f;
		for (int i = 0; i < base.transform.childCount; i++)
		{
			RectTransform rectTransform = base.transform.GetChild(i) as RectTransform;
			if (rectTransform != null && rectTransform.gameObject.activeInHierarchy)
			{
				num += rectTransform.rect.height + _verticalLayoutGroup.spacing;
			}
		}
		(base.transform as RectTransform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, num);
	}

	private void Reset()
	{
		_verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
	}
}
