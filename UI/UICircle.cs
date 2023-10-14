// DestroyOnBurnup.cs
// 
// Author: Max Jackman
// Email:  max.jackman@outlook.com
// Date:   October 6, 2022

using UnityEngine;
using UnityEngine.UI;

namespace Zop.Unity
{
	/// <summary>
	/// A UI circle that can be drawn without a sprite.
	/// See: https://forum.unity.com/threads/draw-circles-or-primitives-on-the-new-ui-canvas.272488/
	/// </summary>
	public class UICircle : MaskableGraphic
	{
		[SerializeField]
		private Texture _Texture;
		[Range(0, 1)]
		[SerializeField]
		private float _FillAmount = 1;
		public bool FillCenter = false;
		public int Border = 1;
		[Range(0, 360)]
		public int Segments = 360;

		public override Texture mainTexture { get { return (_Texture != null) ? (_Texture) : (s_WhiteTexture); } }
		public Texture Texture
		{
			get { return _Texture; }
			set
			{
				if (_Texture != value)
				{
					_Texture = value;
					SetVerticesDirty();
					SetMaterialDirty();
				}
			}
		}
		public float FillAmount
		{
			get { return _FillAmount; }
			set
			{
				if (_FillAmount != value)
				{
					_FillAmount = value;
					SetVerticesDirty();
				}
			}
		}

		private static readonly UIVertex[] vertices = new UIVertex[4];
		private static readonly Vector2[] positions = new Vector2[4];
		private static readonly Vector2[] uvs = new Vector2[4];

		/// <summary>
		/// Initialize.
		/// </summary>
		static UICircle()
		{
			uvs[0] = new Vector2(0, 1);
			uvs[1] = new Vector2(1, 1);
			uvs[2] = new Vector2(1, 0);
			uvs[3] = new Vector2(0, 0);
		}

		/// <summary>
		/// Callback function when a UI element needs to generate vertices. Fills the vertex buffer data.
		/// </summary>
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
			float outer = rectTransform.pivot.x * rectTransform.rect.width;
			float inner = rectTransform.pivot.x * rectTransform.rect.width - Border;
			float degrees = 360.0f / Segments;
			Vector2 prevX = new Vector2(outer * Mathf.Cos(0), outer * Mathf.Sin(0));
			Vector2 prevY = new Vector2(inner * Mathf.Cos(0), inner * Mathf.Sin(0));

			// Add each triangle.
			int end = (int)((Segments + 1) * this._FillAmount);
			for (int i = 0; i < end - 1; i++)
			{
				float rad = Mathf.Deg2Rad * ((i + 1) * degrees);
				float cos = Mathf.Cos(rad);
				float sin = Mathf.Sin(rad);
				positions[0] = prevX;
				positions[1] = new Vector2(outer * cos, outer * sin);

				// Inner vertices.
				if (FillCenter)
				{
					positions[2] = Vector2.zero;
					positions[3] = Vector2.zero;
				}
				else
				{
					positions[2] = new Vector2(inner * cos, inner * sin);
					positions[3] = prevY;
				}

				// Apply
				for (int j = 0; j < 4; j++)
				{
					vertices[j].color = color;
					vertices[j].position = positions[j];
					vertices[j].uv0 = uvs[j];
				}
				int index = vh.currentVertCount;
				vh.AddVert(vertices[0]);
				vh.AddVert(vertices[1]);
				vh.AddVert(vertices[2]);
				vh.AddTriangle(index, index + 2, index + 1);
				if (!FillCenter)
				{
					vh.AddVert(vertices[3]);
					vh.AddTriangle(index, index + 3, index + 2);
				}

				// Next
				prevX = positions[1];
				prevY = positions[2];
			}
		}
	}
}