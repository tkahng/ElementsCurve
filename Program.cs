using Elements;
using Rhino.FileIO;
using Rhino.DocObjects;
using rg = Rhino.Geometry;
using System.Collections.Generic;
using Elements.Geometry;
using HyRhi;

namespace ElementsCurve
{
    class Program
    {
        static void Main()
        {
            AdaptiveCurve();
        }

        static void AdaptiveCurve()
        {
            string keyName = "adaptivecurve.3dm";
            var doc = File3dm.Read(keyName);
            var objects = doc.Objects;
            foreach (var obj in objects)
            {
                var brep  = (rg.Brep)obj.Geometry;
                var guidePolygon = brep.Faces[0].OuterLoop.To3dCurve().ToPolygon();

                IList<Vector3> vertices = brep.Faces[0].GetBrepEdgeVertices();
                for (int i = 0; i < vertices.Count; i++)
                {
                    Vector3 closestPoint;
                    vertices[i].DistanceTo(guidePolygon, out closestPoint);
                    var param = guidePolygon.GetParameterAt(closestPoint);
                    Console.WriteLine(param);
                }
            }
        }
    }
}
