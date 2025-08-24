using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
	private int currentTutorialStep;

	private List<float> stepCompletionTimes = new List<float>();

	private static Tutorial instance;

	private float lastCompletionTime;

	private PlayerController playerController;

	private GameObject characterObj;

	public static Tutorial Instance
	{
		get
		{
			return instance;
		}
	}

	private void Awake()
	{
		Bootloader.Instance.permitItemModifiers = false;
		if (instance == null)
		{
			instance = this;
		}
	}

	private void Start()
	{
		lastCompletionTime = Time.time;
		playerController = Object.FindObjectOfType(typeof(PlayerController)) as PlayerController;
		CharacterHandle component = playerController.gameObject.GetComponent<CharacterHandle>();
		if (component != null)
		{
			Material original = Resources.Load("Skins/Default/" + BBRQuality.SkinQuality + "/normal") as Material;
			component.Skin = Object.Instantiate(original) as Material;
			Texture2D texture2D = Resources.Load("Characters/Oliver/Skins/" + BBRQuality.TextureQuality + "/brown_red") as Texture2D;
			if (texture2D != null)
			{
				component.Skin.mainTexture = texture2D;
			}
		}
		characterObj = playerController.gameObject;
		string text = "Oliver";
		string text2 = "AR";
		string text3 = "rocket_bearzooka";
		string text4 = "Standard_krikket";
		playerController.WeaponManager.PrimaryWeaponPrefab = Resources.Load("Characters/" + text + "/PrimaryWeapons/" + text2) as GameObject;
		playerController.WeaponManager.SecondaryWeaponPrefab = Resources.Load("Characters/" + text + "/SecondaryWeapons/" + text3) as GameObject;
		playerController.WeaponManager.MeleeWeaponPrefab = Resources.Load("Characters/" + text + "/MeleeWeapons/" + text4) as GameObject;
		playerController.specialItemPrefab = Resources.Load("Specials/JumpBoots") as GameObject;
		CapsuleCollider obj = characterObj.GetComponent(typeof(CapsuleCollider)) as CapsuleCollider;
		Object.Destroy(obj);
		Component[] componentsInChildren = characterObj.GetComponentsInChildren(typeof(LOD));
		Component[] array = componentsInChildren;
		foreach (Component obj2 in array)
		{
			Object.Destroy(obj2);
		}
		playerController.DamageReceiver.isRemote = false;
		HUD.Instance.PlayerController = playerController;
		Component componentInChildren = characterObj.GetComponentInChildren(typeof(Camera));
		HUD.Instance.PlayerCamera = componentInChildren.transform;
		HUD.Instance.OnPlayTutorialAnimation(true);
		characterObj.AddComponent(typeof(AudioListener));
		playerController.isRemote = false;
		playerController.OnSetTeam(Team.RED);
		playerController.WeaponManager.isRemote = false;
		playerController.OnPostCreate();
		playerController.DamageReceiver.OnPostCreate();
		playerController.DamageReceiver.isInvincible = true;
		MogaPopUpHandler.ShowTutorial();
		HUD.Instance.PlayerController = playerController;
	}

	public void OnFinished()
	{
		for (int i = 0; i < stepCompletionTimes.Count; i++)
		{
		}
		Bootloader.Instance.unlockAchievement(GameCenterIDDictionaries.Achievements["TUTORIAL_DONE"]);
		if (!LoginManager.offlineMode)
		{
			Application.LoadLevel("MainMenu");
		}
		else
		{
			Application.LoadLevel("Login");
		}
	}

	public void QuittingEarly()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("last_event_accomplished", currentTutorialStep.ToString());
	}

	public void OnNextSection()
	{
		float item = Time.time - lastCompletionTime;
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("step", currentTutorialStep.ToString());
		dictionary.Add("time", item.ToString());
		stepCompletionTimes.Add(item);
		lastCompletionTime = Time.time;
		currentTutorialStep++;
	}
}
