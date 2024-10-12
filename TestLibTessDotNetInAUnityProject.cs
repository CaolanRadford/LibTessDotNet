using System.Collections.Generic;
using LibTessDotNet;
using UnityEngine;

namespace LibTessDotNet_ForUnity
{
    
    /// <summary>
    /// Put this on a parent game object with a composite collider, and it will triangulate the collider's paths using LibTessDotNet.
    /// The composite collider test object obviously needs children for this to work, those children need to be set up to use the composite collider, etc, etc.
    /// When you run the scene you should see debug lines drawing the tesselation of the composite collider.
    /// </summary>
    public class TestLibTessDotNetInAUnityProject : MonoBehaviour
    {
        public CompositeCollider2D compositeCollider;

        void Start()
        {
            if (compositeCollider == null)
            {
                Debug.LogError("CompositeCollider2D is not assigned!");
                return;
            }

            // Get paths from the CompositeCollider2D
            List<ContourVertex[]> contours = new List<ContourVertex[]>();
            for (int i = 0; i < compositeCollider.pathCount; i++)
            {
                Vector2[] path = new Vector2[compositeCollider.GetPathPointCount(i)];
                compositeCollider.GetPath(i, path);

                ContourVertex[] contour = new ContourVertex[path.Length];
                for (int j = 0; j < path.Length; j++)
                {
                    // Convert local path point to world point
                    Vector3 worldPoint = compositeCollider.transform.TransformPoint(path[j]);
                    contour[j] = new ContourVertex
                    {
                        Position = new Vec3(worldPoint.x, worldPoint.y, 0)
                    };
                }
                contours.Add(contour);
            }

            // Perform triangulation using LibTessDotNet
            Tess tess = new Tess();
            foreach (var contour in contours)
            {
                tess.AddContour(contour, ContourOrientation.Original);
            }

            tess.Tessellate(WindingRule.NonZero, ElementType.Polygons, 3);

            // Visualize the result
            DrawTriangles(tess);
        }

        void DrawTriangles(Tess tess)
        {
            for (int i = 0; i < tess.ElementCount; i++)
            {
                int idx0 = tess.Elements[i * 3];
                int idx1 = tess.Elements[i * 3 + 1];
                int idx2 = tess.Elements[i * 3 + 2];

                Vector3 v0 = new Vector3(tess.Vertices[idx0].Position.X, tess.Vertices[idx0].Position.Y, 0);
                Vector3 v1 = new Vector3(tess.Vertices[idx1].Position.X, tess.Vertices[idx1].Position.Y, 0);
                Vector3 v2 = new Vector3(tess.Vertices[idx2].Position.X, tess.Vertices[idx2].Position.Y, 0);

                Debug.DrawLine(v0, v1, Color.red, 10f);
                Debug.DrawLine(v1, v2, Color.green, 10f);
                Debug.DrawLine(v2, v0, Color.blue, 10f);
            }
        }
    }
}
