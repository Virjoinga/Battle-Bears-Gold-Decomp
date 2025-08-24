using ExitGames.Client.Photon;

public class PlayerParameterModel
{
	private string _socialName;

	private int _team;

	private string _character;

	private string _skin;

	private string _primary;

	private string _secondary;

	private string _melee;

	private string _special;

	private string _equipmentOne;

	private string _equipmentTwo;

	private int _playerId;

	private int _level;

	private int _skill;

	private string _taunt;

	private string _reputationColor;

	public string SocialName
	{
		get
		{
			return _socialName;
		}
	}

	public int PlayerTeam
	{
		get
		{
			return _team;
		}
	}

	public string Character
	{
		get
		{
			return _character;
		}
	}

	public string Skin
	{
		get
		{
			return _skin;
		}
	}

	public string Primary
	{
		get
		{
			return _primary;
		}
	}

	public string Secondary
	{
		get
		{
			return _secondary;
		}
	}

	public string Melee
	{
		get
		{
			return _melee;
		}
	}

	public string Special
	{
		get
		{
			return _special;
		}
	}

	public string EquipmentOne
	{
		get
		{
			return _equipmentOne;
		}
	}

	public string EquipmentTwo
	{
		get
		{
			return _equipmentTwo;
		}
	}

	public int PlayerId
	{
		get
		{
			return _playerId;
		}
	}

	public int Level
	{
		get
		{
			return _level;
		}
	}

	public int Skill
	{
		get
		{
			return _skill;
		}
	}

	public string Taunt
	{
		get
		{
			return _taunt;
		}
	}

	public string ReputationColor
	{
		get
		{
			return _reputationColor;
		}
	}

	public PlayerParameterModel(Hashtable userParameters)
	{
		_socialName = (string)userParameters[(byte)84];
		_team = ((!userParameters.ContainsKey((byte)86)) ? (-1) : ((int)userParameters[(byte)86]));
		_character = (string)userParameters[(byte)87];
		_skin = (string)userParameters[(byte)88];
		_primary = (string)userParameters[(byte)89];
		_secondary = (string)userParameters[(byte)90];
		_melee = (string)userParameters[(byte)91];
		_special = (string)userParameters[(byte)92];
		_equipmentOne = (string)userParameters[(byte)93];
		_equipmentTwo = (string)userParameters[(byte)104];
		_playerId = ((!userParameters.ContainsKey((byte)106)) ? (-1) : ((int)userParameters[(byte)106]));
		_level = ((!userParameters.ContainsKey((byte)107)) ? (-1) : ((int)userParameters[(byte)107]));
		_skill = ((!userParameters.ContainsKey((byte)108)) ? (-1) : ((int)userParameters[(byte)108]));
		_taunt = (string)userParameters[(byte)109];
		_reputationColor = (string)userParameters[(byte)110];
	}

	public override string ToString()
	{
		return string.Format("SocialName={0}\n PlayerTeam={1}\n Character={2}\n Skin={3}\nPrimary={4}\n Secondary={5}\n Melee={6}\n Special={7}\n EquipmentOne={8}\n EquipmentTwo={9}\nPlayerId={10}\n Level={11}\n Skill={12}\n Taunt={13}\n ReputationColor={14}", SocialName, PlayerTeam, Character, Skin, Primary, Secondary, Melee, Special, EquipmentOne, EquipmentTwo, PlayerId, Level, Skill, Taunt, ReputationColor);
	}
}
