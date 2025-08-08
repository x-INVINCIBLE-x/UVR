using UnityEngine;

namespace LunaWolfStudios.ScriptableSheets.Samples.ComponentPresets
{
	[System.Serializable]
	public class RigidbodyPreset : AbstractComponentPreset<Rigidbody>, IComponentPreset
	{
		// Remake RigidbodyConstraints enum because Unity's isn't tagged as a System.Flag for serialization.
		[System.Flags]
		public enum RigidbodyConstraints
		{
			None = 0,
			FreezePositionX = 2,
			FreezePositionY = 4,
			FreezePositionZ = 8,
			FreezePosition = 14,
			FreezeRotationX = 16,
			FreezeRotationY = 32,
			FreezeRotationZ = 64,
			FreezeRotation = 112,
			FreezeAll = 126
		}

		[SerializeField]
		private float m_Mass;
		public float Mass { get => m_Mass; set => m_Mass = value; }

		[SerializeField]
		private float m_Drag;
		public float Drag { get => m_Drag; set => m_Drag = value; }

		[SerializeField]
		private float m_AngularDrag;
		public float AngularDrag { get => m_AngularDrag; set => m_AngularDrag = value; }

		[SerializeField]
		private Vector3 m_CenterOfMass;
		public Vector3 CenterOfMass { get => m_CenterOfMass; set => m_CenterOfMass = value; }

		[SerializeField]
		private Vector3 m_InertiaTensor;
		public Vector3 InertiaTensor { get => m_InertiaTensor; set => m_InertiaTensor = value; }

		[SerializeField]
		private Quaternion m_InertiaTensorRotation;
		public Quaternion InertiaTensorRotation { get => m_InertiaTensorRotation; set => m_InertiaTensorRotation = value; }

		[SerializeField]
		private bool m_UseGravity;
		public bool UseGravity { get => m_UseGravity; set => m_UseGravity = value; }

		[SerializeField]
		private bool m_IsKinematic;
		public bool IsKinematic { get => m_IsKinematic; set => m_IsKinematic = value; }

		[SerializeField]
		private RigidbodyInterpolation m_Interpolation;
		public RigidbodyInterpolation Interpolation { get => m_Interpolation; set => m_Interpolation = value; }

		[SerializeField]
		private CollisionDetectionMode m_CollisionDetectionMode;
		public CollisionDetectionMode CollisionDetectionMode { get => m_CollisionDetectionMode; set => m_CollisionDetectionMode = value; }

		[SerializeField]
		private RigidbodyConstraints m_Constraints;
		public RigidbodyConstraints Constraints { get => m_Constraints; set => m_Constraints = value; }

#if UNITY_2022_2_OR_NEWER

		[SerializeField]
		private LayerMask m_IncludeLayers;
		public LayerMask IncludeLayers { get => m_IncludeLayers; set => m_IncludeLayers = value; }

		[SerializeField]
		private LayerMask m_ExcludeLayers;
		public LayerMask ExcludeLayers { get => m_ExcludeLayers; set => m_ExcludeLayers = value; }
#endif

		protected override void Apply(Rigidbody obj)
		{
			obj.mass = m_Mass;
#if UNITY_6000_0_OR_NEWER
			obj.linearDamping = m_Drag;
			obj.angularDamping = m_AngularDrag;
#else
			obj.drag = m_Drag;
			obj.angularDrag = m_AngularDrag;
#endif
			obj.centerOfMass = m_CenterOfMass;
			obj.inertiaTensor = m_InertiaTensor;
			obj.inertiaTensorRotation = m_InertiaTensorRotation;
			obj.useGravity = m_UseGravity;
			obj.isKinematic = m_IsKinematic;
			obj.interpolation = m_Interpolation;
			obj.collisionDetectionMode = m_CollisionDetectionMode;
			obj.constraints = (UnityEngine.RigidbodyConstraints) m_Constraints;
#if UNITY_2022_2_OR_NEWER
			obj.includeLayers = m_IncludeLayers;
			obj.excludeLayers = m_ExcludeLayers;
#endif
		}
	}
}
