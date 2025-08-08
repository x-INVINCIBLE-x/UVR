using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.ComponentPresets
{
	public abstract class AbstractComponentPreset<T> : ScriptableObject, IComponentPreset where T : Object
	{
		protected abstract void Apply(T obj);

		public void Apply(Object obj)
		{
			if (obj == null)
			{
				Debug.LogWarning($"{typeof(T)} is null. Cannot apply settings.");
				return;
			}
			if (obj.GetType() != typeof(T))
			{
				Debug.LogWarning($"Object of type {obj.GetType()} cannot be cast to {typeof(T)}. Cannot apply settings.");
				return;
			}
			Apply((T) obj);
		}
	}
}
