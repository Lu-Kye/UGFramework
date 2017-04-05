using UnityEngine;
using System;

namespace UGFramework.Utility
{
	public enum MeshType
	{
		NULL,
		POLYGON_PLANE,
	}

	[ExecuteInEditMode]
	public abstract class AbstractMesh<T> : MonoBehaviour
		where T : AbstractMeshConfig
	{
		public T Config { get; set; }

		protected MeshFilter meshFilter;
		protected MeshCollider meshCollider;
		protected MeshRenderer meshRenderer;

		protected virtual void Awake()
		{
			this.meshFilter = gameObject.GetComponent<MeshFilter>();
			if (this.meshFilter == null)
			{
				this.meshFilter = gameObject.AddComponent<MeshFilter>();
			}

			this.meshCollider = gameObject.GetComponent<MeshCollider>();
			if (this.meshCollider == null)
			{
				this.meshCollider = gameObject.AddComponent<MeshCollider>();
			}

			this.meshRenderer = gameObject.GetComponent<MeshRenderer>();
			if (this.meshRenderer == null)
			{
				this.meshRenderer = gameObject.AddComponent<MeshRenderer>();
			}
		}

		protected virtual void Start()
		{
			InitMesh();
			meshCollider.sharedMesh = meshFilter.sharedMesh;

			InitMaterial();
		}

		protected virtual void InitMesh()
		{
		}

		protected virtual void InitMaterial()
		{
			this.meshRenderer.material = Config.Material;	
		}
	}

	public abstract class AbstractMeshConfig
	{
		public virtual MeshType Type 
		{ 
			get
			{
				return MeshType.NULL;
			} 
		}
		public Material Material { get; set; }
	}

	public abstract class AbstractMeshFactory<T1, T2>
		where T1 : AbstractMesh<T2>
		where T2 : AbstractMeshConfig
	{
		public virtual T1 Create(T2 meshConfig)
		{
			if (meshConfig.Type == MeshType.NULL)
			{
				throw new Exception("MeshType can not be null!");
			}
			return null;
		}
	}
}
