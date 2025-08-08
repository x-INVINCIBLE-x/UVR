namespace LunaWolfStudios.ScriptableSheets.Samples.RPG
{
	[System.Flags]
	public enum Abilities
	{
		None = 0,
		Jump = 1 << 0,
		DoubleJump = 1 << 1,
		Dash = 1 << 2,
		Glide = 1 << 3,
		WallClimb = 1 << 4,
		Teleport = 1 << 5,
		Invisibility = 1 << 6,
		Fly = 1 << 7,
	}
}
