using UnityEngine;

public class WeaponTestScript : MonoBehaviour
{
	private GameObject characterObj;

	private PlayerController playerController;

	private PlayerLoadout playerLoadout;

	public string characterName = "Oliver";

	public string skinName = string.Empty;

	public string primaryWeaponName = string.Empty;

	public string secondaryWeaponName = string.Empty;

	public string meleeWeaponName = string.Empty;

	public string specialItemName = "JumpBoots";

	public string equipmentOneName = string.Empty;

	public string equipmentTwoName = string.Empty;

	public bool blueTeam = true;

	private void Start()
	{
		Bootloader.Instance.InTutorial = true;
		Object.Instantiate(Resources.Load("Characters/" + characterName + "/" + characterName), new Vector3(0f, 100f, 0f), Quaternion.identity);
		playerController = Object.FindObjectOfType(typeof(PlayerController)) as PlayerController;
		characterObj = playerController.gameObject;
		CharacterHandle component = characterObj.GetComponent<CharacterHandle>();
		playerLoadout = new PlayerLoadout();
		playerLoadout.model = ServiceManager.Instance.GetItemByName(characterName);
		playerLoadout.skin = ServiceManager.Instance.GetItemByName(skinName);
		playerLoadout.primary = ServiceManager.Instance.GetItemByName(primaryWeaponName);
		playerLoadout.secondary = ServiceManager.Instance.GetItemByName(secondaryWeaponName);
		playerLoadout.melee = ServiceManager.Instance.GetItemByName(meleeWeaponName);
		playerLoadout.special = ServiceManager.Instance.GetItemByName(specialItemName);
		playerLoadout.equipment1 = ServiceManager.Instance.GetItemByName(equipmentOneName);
		playerLoadout.equipment2 = ServiceManager.Instance.GetItemByName(equipmentTwoName);
		playerController.specialItemPrefab = Resources.Load("Specials/" + playerLoadout.special.name) as GameObject;
		LoadoutManager.Instance.CurrentLoadout = playerLoadout;
		Object.Destroy(characterObj.GetComponent<CapsuleCollider>());
		characterObj.transform.Find("playerName/name").GetComponent<TextMesh>().text = string.Empty;
		if (!skinName.Equals(string.Empty))
		{
			string text = null;
			if (skinName.Contains("|"))
			{
				int num = skinName.IndexOf("|");
				text = skinName.Substring(num + 1);
				skinName = skinName.Substring(0, num);
			}
			else
			{
				text = skinName + "_damaged";
			}
			string text2 = ((!blueTeam) ? "_red" : "_blue");
			string text3 = BBRQuality.SkinQuality + "/";
			string text4 = "Characters/" + characterName + "/Skins/" + text3 + skinName + "/normal" + text2;
			Material material = Resources.Load("Skins/Default/" + BBRQuality.SkinQuality + "/normal") as Material;
			if (material != null)
			{
				component.Skin = Object.Instantiate(material) as Material;
			}
			Texture2D texture2D = Resources.Load("Characters/" + playerLoadout.model.name + "/Skins/" + BBRQuality.TextureQuality + "/" + skinName + text2) as Texture2D;
			if (texture2D != null)
			{
				component.Skin.mainTexture = texture2D;
			}
			Material cloakMaterial = Resources.Load("Materials/Characters/Cloaking/cloaking" + text2) as Material;
			PlayerDamageReceiver playerDamageReceiver = characterObj.GetComponentInChildren(typeof(PlayerDamageReceiver)) as PlayerDamageReceiver;
			if (playerDamageReceiver != null)
			{
				playerDamageReceiver.normalMaterial = material;
				playerDamageReceiver.cloakMaterial = cloakMaterial;
				playerDamageReceiver.hitMaterial = new Material(material);
				playerDamageReceiver.hitMaterial.color = Color.white;
			}
		}
		Material teamMaterial = Resources.Load((!blueTeam) ? "goggleMaterialBlue" : "goggleMaterialRed") as Material;
		component.TeamMaterial = teamMaterial;
		playerController.WeaponManager.PrimaryWeaponPrefab = Resources.Load("Characters/" + characterName + "/PrimaryWeapons/" + primaryWeaponName) as GameObject;
		playerController.WeaponManager.SecondaryWeaponPrefab = Resources.Load("Characters/" + characterName + "/SecondaryWeapons/" + secondaryWeaponName) as GameObject;
		playerController.WeaponManager.MeleeWeaponPrefab = Resources.Load("Characters/" + characterName + "/MeleeWeapons/" + meleeWeaponName) as GameObject;
		playerController.WeaponManager.OnSetWeapon(0);
		playerController.Performer.PlayerController = playerController;
		Component[] componentsInChildren = characterObj.GetComponentsInChildren(typeof(LOD));
		Component[] array = componentsInChildren;
		foreach (Component obj in array)
		{
			Object.Destroy(obj);
		}
		CameraFacingBillboard component2 = playerController.statusEffectMount.GetComponent<CameraFacingBillboard>();
		if (component2 != null)
		{
			Object.Destroy(component2);
		}
		playerController.DamageReceiver.isRemote = false;
		HUD.Instance.PlayerController = playerController;
		Component componentInChildren = characterObj.GetComponentInChildren(typeof(Camera));
		HUD.Instance.PlayerCamera = componentInChildren.transform;
		characterObj.AddComponent(typeof(AudioListener));
		playerController.isRemote = false;
		playerController.WeaponManager.isRemote = false;
		Object.Destroy(characterObj.transform.Find("gogglesHighlight").gameObject);
		Object.Destroy(characterObj.transform.Find("playerHighlight").gameObject);
		Transform transform = characterObj.transform.Find("playerName");
		if (transform != null)
		{
			Object.Destroy(transform.gameObject);
		}
		playerController.OnSetTeam(blueTeam ? Team.BLUE : Team.RED);
		playerController.OnPostCreate();
		playerController.DamageReceiver.OnPostCreate();
		SoundManager.Instance.setMusicVolume(0f);
		HUD.Instance.PlayerController = playerController;
		HUD.Instance.PlayerController.BodyAnimator.isDisabled = true;
	}
}
