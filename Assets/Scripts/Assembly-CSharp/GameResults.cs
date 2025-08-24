public class GameResults
{
	public string errorString;

	public int oldXP = -1;

	public int baseXP = -1;

	public int bonusXP = -1;

	public int killXP = -1;

	public int extraXP = -1;

	public int oldJoules = -1;

	public int baseJoules = -1;

	public int killJoules = -1;

	public int mapJoules = -1;

	public int bonusJoules = -1;

	public float oldSkill = -1f;

	public float newSkill = -1f;

	public bool enoughEnergy;

	public float SkillDelta
	{
		get
		{
			return newSkill - oldSkill;
		}
	}
}
