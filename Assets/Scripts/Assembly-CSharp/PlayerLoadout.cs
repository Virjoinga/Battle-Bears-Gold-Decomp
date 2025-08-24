public class PlayerLoadout
{
	public int pid = -1;

	public int loadoutNumber = 1;

	public Item model;

	public Item skin;

	public Item taunt;

	public Item primary;

	public Item secondary;

	public Item special;

	public Item melee;

	public Item equipment1;

	public Item equipment2;

	public PlayerLoadout Clone()
	{
		PlayerLoadout playerLoadout = new PlayerLoadout();
		playerLoadout.pid = pid;
		playerLoadout.loadoutNumber = loadoutNumber;
		playerLoadout.model = model;
		playerLoadout.skin = skin;
		playerLoadout.taunt = taunt;
		playerLoadout.primary = primary;
		playerLoadout.secondary = secondary;
		playerLoadout.special = special;
		playerLoadout.melee = melee;
		playerLoadout.equipment1 = equipment1;
		playerLoadout.equipment2 = equipment2;
		return playerLoadout;
	}
}
