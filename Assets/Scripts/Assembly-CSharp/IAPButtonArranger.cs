using UnityEngine;

public class IAPButtonArranger : MonoBehaviour
{
	[SerializeField]
	protected int _numRows;

	[SerializeField]
	protected int _numColumns;

	[SerializeField]
	protected Transform _upperLeftPosition;

	[SerializeField]
	protected Vector2 _buttonSpacing;

	[SerializeField]
	protected GameObject _iOSButton;

	[SerializeField]
	protected GameObject _androidButton;

	[SerializeField]
	protected GameObject _microsoftButton;

	public IIAPButton[] Buttons { get; set; }

	private void Awake()
	{
		CreateButtonsInGrid();
	}

	protected virtual void CreateButtonsInGrid()
	{
		int num = _numRows * _numColumns;
		Buttons = new IIAPButton[num];
		GameObject gameObject = null;
		GameObject gameObject2 = null;
		gameObject2 = _androidButton;
		for (int i = 0; i < _numRows; i++)
		{
			for (int j = 0; j < _numColumns; j++)
			{
				Vector3 position = _upperLeftPosition.position;
				position.y -= _buttonSpacing.y * (float)i;
				position.x += _buttonSpacing.x * (float)j;
				gameObject = Object.Instantiate(gameObject2) as GameObject;
				gameObject.transform.parent = _upperLeftPosition;
				gameObject.transform.position = position;
				gameObject.transform.rotation = _upperLeftPosition.rotation;
				gameObject.name = gameObject.name.Replace("(Clone)", string.Empty) + "_" + (i + j * _numRows);
				Buttons[i + j * _numRows] = gameObject.GetComponent<AndroidIAPButton>();
			}
		}
	}
}
