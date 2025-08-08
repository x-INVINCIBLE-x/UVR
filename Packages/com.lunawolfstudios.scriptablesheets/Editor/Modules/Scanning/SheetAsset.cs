namespace LunaWolfStudiosEditor.ScriptableSheets.Scanning
{
	[System.Flags]
	public enum SheetAsset
	{
		Default = 0,
		ScriptableObject = 1 << 0,
		AnimationClip = 1 << 1,
		AnimatorController = 1 << 2,
		AudioClip = 1 << 3,
		AudioMixer = 1 << 4,
		AvatarMask = 1 << 5,
		Flare = 1 << 6,
		Font = 1 << 7,
		Material = 1 << 8,
		Mesh = 1 << 9,
		PhysicMaterial = 1 << 10,
		PhysicsMaterial2D = 1 << 11,
		Prefab = 1 << 12,
		Scene = 1 << 13,
		Shader = 1 << 14,
		Sprite = 1 << 15,
		TerrainData = 1 << 16,
		TextAsset = 1 << 17,
		Texture = 1 << 18,
		VideoClip = 1 << 19,
	}
}
