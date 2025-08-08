using Newtonsoft.Json;

namespace LunaWolfStudiosEditor.ScriptableSheets.Tables
{
	public class UnityObjectWrapper
	{
		[JsonProperty("guid")]
		public string Guid;

		[JsonProperty("localId")]
		public long LocalId;

		[JsonProperty("type")]
		public int Type;

		[JsonProperty("instanceID")]
		public int InstanceId;

		public override string ToString()
		{
			return $"guid: '{Guid}' localId: '{LocalId}' type: '{Type}' instanceID: '{InstanceId}'";
		}
	}
}
