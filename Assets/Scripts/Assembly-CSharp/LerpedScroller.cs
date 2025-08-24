using UnityEngine;

public class LerpedScroller : AnimatedScroller
{
	[SerializeField]
	private GameObject _emptyContainerPrefab;

	private static readonly float _containerBaseScale = 1f;

	private static readonly float _containerSelectedScale = 1.15f;

	private static readonly int _slotDistanceDiff = -50;

	private static readonly float _zOffset;

	private static readonly string _slotPrefix = "slot";

	private static readonly string _upArrowName = "upArrow";

	private static readonly string _downArrowName = "downArrow";

	private float _startingTime = -1f;

	public override void Awake()
	{
		if (myAnimation == null)
		{
			myAnimation = base.GetComponent<Animation>();
		}
		if (myCollider == null)
		{
			myCollider = base.GetComponent<Collider>();
		}
		if (_emptyContainerPrefab == null)
		{
			Debug.LogWarning("Could not create lerped scroller, container prefab was null!");
			return;
		}
		if (mainButton != null)
		{
			mainButton.gameObject.SetActive(true);
		}
		MoveEverySlotByValueAndScale(0f);
	}

	public override void Update()
	{
		bool flag = false;
		if (Input.touchCount > 0)
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				Ray ray = guiCamera.ScreenPointToRay(Input.GetTouch(i).position);
				RaycastHit hitInfo;
				if (myCollider.Raycast(ray, out hitInfo, 500f))
				{
					flag = true;
					float deltaTime = Input.GetTouch(i).deltaTime;
					if (deltaTime <= 0f)
					{
						deltaTime = 1f;
					}
					if (isHorizontal)
					{
						if (Mathf.Abs(Input.GetTouch(i).deltaPosition.x) > 5f && mainButton != null)
						{
							mainButton.gameObject.SetActive(false);
						}
						float num = AnimatedScroller.FixTouchDelta(Input.GetTouch(i)).x * Time.deltaTime * (defaultWidth / (float)Screen.currentResolution.width) * 25f;
						MoveEverySlotByValueAndScale(num);
						snapToButton();
						if (num > 1f && mainButton != null)
						{
							mainButton.gameObject.SetActive(false);
						}
					}
					else
					{
						if (Mathf.Abs(Input.GetTouch(i).deltaPosition.y) > 5f && mainButton != null)
						{
							mainButton.gameObject.SetActive(false);
						}
						float num2 = AnimatedScroller.FixTouchDelta(Input.GetTouch(i)).y * Time.deltaTime * (defaultWidth / (float)Screen.currentResolution.height) * 25f;
						MoveEverySlotByValueAndScale(num2);
						snapToButton();
						if (num2 > 1f && mainButton != null)
						{
							mainButton.gameObject.SetActive(false);
						}
					}
				}
				else
				{
					snapToButton();
				}
			}
		}
		else
		{
			didScroll = false;
		}
		if (slots.Count <= 0 || flag)
		{
			return;
		}
		if (isHorizontal)
		{
			if (slots[currentIndex].transform.position.x != base.transform.position.x)
			{
				float num3 = Mathf.Lerp(slots[currentIndex].transform.position.x, base.transform.position.x, Time.deltaTime * scrollSpeed);
				float delta = num3 - slots[currentIndex].transform.position.x;
				MoveEverySlotByValueAndScale(delta);
			}
		}
		else if (slots[currentIndex].transform.position.y != base.transform.position.y)
		{
			float num4 = Mathf.Lerp(slots[currentIndex].transform.position.y, base.transform.position.y, Time.deltaTime * scrollSpeed);
			float delta2 = num4 - slots[currentIndex].transform.position.y;
			MoveEverySlotByValueAndScale(delta2);
		}
	}

	protected override void snapToButton()
	{
		int num = 0;
		float num2 = float.PositiveInfinity;
		for (int i = 0; i < slots.Count && (numberShowing == -1 || slots.Count - numberShowing >= i); i++)
		{
			float num3 = float.PositiveInfinity;
			num3 = ((!isHorizontal) ? Mathf.Abs(slots[i].position.y - base.transform.position.y) : Mathf.Abs(slots[i].position.x - base.transform.position.x));
			if (num3 < num2)
			{
				num2 = num3;
				num = i;
			}
		}
		currentIndex = num;
		if (linkedButton != null)
		{
			linkedButton.name = scrollerName + currentIndex;
			linkedButton.enable();
			for (int j = 0; j < linkButtonDisables.Length; j++)
			{
				if (currentIndex == linkButtonDisables[j])
				{
					linkedButton.disable();
					break;
				}
			}
		}
		if (mainButton != null)
		{
			mainButton.gameObject.SetActive(true);
		}
	}

	public override void OnReset()
	{
		buttonList.Clear();
	}

	protected override void recalculate()
	{
		currentIndex = 0;
		MoveEverySlotByValueAndScale(0f);
	}

	public override void OnGUIButtonClicked(GUIButton b)
	{
		if (b.name == _upArrowName)
		{
			if (currentIndex > 0)
			{
				currentIndex--;
			}
			return;
		}
		if (b.name == _downArrowName)
		{
			if (currentIndex < buttonList.Count - 1)
			{
				currentIndex++;
			}
			return;
		}
		if (b.name == "mainButton")
		{
			b.name = scrollerName + currentIndex;
			SendMessageUpwards("OnGUIButtonClicked", b, SendMessageOptions.DontRequireReceiver);
			b.name = "mainButton";
			return;
		}
		int num = -1;
		for (int i = 0; i < buttonList.Count; i++)
		{
			if (b == buttonList[i])
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			currentIndex = num;
		}
	}

	private void AddEmptySlot()
	{
		GameObject gameObject = new GameObject(_slotPrefix + slots.Count);
		gameObject.transform.parent = base.transform;
		if (isHorizontal)
		{
			gameObject.transform.localPosition = new Vector3(_slotDistanceDiff * slots.Count, 0f, _zOffset);
		}
		else
		{
			gameObject.transform.localPosition = new Vector3(0f, _slotDistanceDiff * slots.Count, _zOffset);
		}
		gameObject.transform.localRotation = Quaternion.identity;
		slots.Add(gameObject.transform);
	}

	public override void addButton(GUIButton b)
	{
		AddEmptySlot();
		base.addButton(b);
	}

	private void MoveEverySlotByValueAndScale(float delta)
	{
		foreach (Transform slot in slots)
		{
			Vector3 position = slot.position;
			float num = 1f;
			if (isHorizontal)
			{
				position.x += delta;
				slot.position = position;
				num = _containerBaseScale + Mathf.Clamp(1f - Mathf.Abs((position.x - base.transform.position.x) / (float)_slotDistanceDiff), 0f, 1f) * (_containerSelectedScale - _containerBaseScale);
			}
			else
			{
				position.y += delta;
				slot.position = position;
				num = _containerBaseScale + Mathf.Clamp(1f - Mathf.Abs((position.y - base.transform.position.y) / (float)_slotDistanceDiff), 0f, 1f) * (_containerSelectedScale - _containerBaseScale);
			}
			slot.localScale = new Vector3(num, num, num);
		}
	}
}
