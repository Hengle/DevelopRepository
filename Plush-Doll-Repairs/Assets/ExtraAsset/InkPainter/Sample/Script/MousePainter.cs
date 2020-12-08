using UnityEngine;
using System.Collections;

namespace Es.InkPainter.Sample
{
	public class MousePainter : MonoBehaviour
	{
		/// <summary>
		/// Types of methods used to paint.
		/// </summary>
		[System.Serializable]
		private enum UseMethodType
		{
			RaycastHitInfo,
			WorldPoint,
			NearestSurfacePoint,
			DirectUV,
		}

		[SerializeField]
		private Brush brush;

		[SerializeField]
		private UseMethodType useMethodType = UseMethodType.RaycastHitInfo;

		[SerializeField]
		bool erase = false;

        public RenderTexture RT4;

        IEnumerator Start()
        {

            RT4 = new RenderTexture(4, 4, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            RT4.Create();

            while (true)
            {

                yield return new WaitForEndOfFrame();


                RenderTexture.active = RT4;
                var Tex4 = new Texture2D(4, 4, TextureFormat.ARGB32, false);
                Tex4.ReadPixels(new Rect(0, 0, 4, 4), 0, 0);
                Tex4.Apply();

                yield return new WaitForSeconds(0.01f);

                Color scoresColor = new Color(0, 0, 0, 0);
                scoresColor += Tex4.GetPixel(0, 0);
                scoresColor += Tex4.GetPixel(0, 1);
                scoresColor += Tex4.GetPixel(0, 2);
                scoresColor += Tex4.GetPixel(0, 3);

                yield return new WaitForSeconds(0.01f);

                scoresColor += Tex4.GetPixel(1, 0);
                scoresColor += Tex4.GetPixel(1, 1);
                scoresColor += Tex4.GetPixel(1, 2);
                scoresColor += Tex4.GetPixel(1, 3);

                yield return new WaitForSeconds(0.01f);

                scoresColor += Tex4.GetPixel(2, 0);
                scoresColor += Tex4.GetPixel(2, 1);
                scoresColor += Tex4.GetPixel(2, 2);
                scoresColor += Tex4.GetPixel(2, 3);

                yield return new WaitForSeconds(0.01f);

                scoresColor += Tex4.GetPixel(3, 0);
                scoresColor += Tex4.GetPixel(3, 1);
                scoresColor += Tex4.GetPixel(3, 2);
                scoresColor += Tex4.GetPixel(3, 3);

                Debug.Log(scoresColor);


                yield return new WaitForSeconds(1.0f);

            }

        }

        private void Update()
		{
            
            if (Input.GetMouseButton(0))
			{
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				bool success = true;
				RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
				{
                    var paintObject = hitInfo.transform.GetComponent<InkCanvas>();
					if(paintObject != null)
						switch(useMethodType)
						{
							case UseMethodType.RaycastHitInfo:
								success = erase ? paintObject.Erase(brush, hitInfo) : paintObject.Paint(brush, hitInfo);
								break;

							case UseMethodType.WorldPoint:
								success = erase ? paintObject.Erase(brush, hitInfo.point) : paintObject.Paint(brush, hitInfo.point);
								break;

							case UseMethodType.NearestSurfacePoint:
								success = erase ? paintObject.EraseNearestTriangleSurface(brush, hitInfo.point) : paintObject.PaintNearestTriangleSurface(brush, hitInfo.point);
								break;

							case UseMethodType.DirectUV:
								if(!(hitInfo.collider is MeshCollider))
									Debug.LogWarning("Raycast may be unexpected if you do not use MeshCollider.");
								success = erase ? paintObject.EraseUVDirect(brush, hitInfo.textureCoord) : paintObject.PaintUVDirect(brush, hitInfo.textureCoord);
								break;
						}
					if(!success)
						Debug.LogError("Failed to paint.");
				}
			}
		}

		public void OnGUI()
		{
			if(GUILayout.Button("Reset"))
			{
				foreach(var canvas in FindObjectsOfType<InkCanvas>())
					canvas.ResetPaint();
			}
		}
	}
}