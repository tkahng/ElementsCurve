using Elements;
using Rhino.FileIO;
using Rhino.DocObjects;
using rg = Rhino.Geometry;
using System.Collections.Generic;
using Elements.Geometry;
using HyRhi;
using System.Linq;
using System;

namespace ElementsCurve
{
    class Program
    {
        static void Main()
        {
#if DEBUG
            // TestParam();
            AdaptiveCurve();
            // var r = Demo();
            // System.Console.Write(r);
#endif
        }


        static void AdaptiveCurve()
        {
            string keyName = "adaptivecurve.3dm";
            // var newFile = new File3dm();
            var doc = File3dm.Read(keyName);
            var objects = doc.Objects;
            var polygons = new List<Polygon>();
            var points = new List<rg.Point3d>();
            var v3s = new List<Vector3>();
            foreach (var obj in objects)
            {
                var brep  = (rg.Brep)obj.Geometry;
                var guidePolygon = brep.Faces[0].OuterLoop.To3dCurve().ToPolygon();

                IList<Vector3> vertices = brep.Faces[0].GetBrepEdgeVertices();
                var pairList = new List<PointTPair>();
                for (int i = 0; i < vertices.Count; i++)
                {
                    Vector3 closestPoint;
                    vertices[i].DistanceTo(guidePolygon, out closestPoint);
                    var param = guidePolygon.GetParameterAt(closestPoint);
                    Console.WriteLine(param);
                    pairList.Add(new PointTPair() { Point = closestPoint, Parameter = param });
                }
                var orderedpairlist = pairList.OrderBy(p => p.Parameter).ToList();
                var ptList = orderedpairlist.Select(s => s.Point).ToList();

                // for (int i = 0; i < orderedpairlist.Count; i++)
                // {
                //     var data = new ObjectAttributes();
                //     data.SetUserString("order", $"{i}");
                //     data.SetUserString("param", $"{orderedpairlist[i].Parameter}");
                //     // newFile.Objects.AddPoint(vertices[i].ToRgPoint(), data);
                // }


                var polygon = new Polygon(ptList);
                // newFile.Objects.AddPolyline(polygon.ToRgPolyline());
                // polygons.Add(polygon);
            }
            // polyline = new rg.Polyline(v3s.Select(s => s.ToRgPoint()));

            // newFile.Write($"{System.Guid.NewGuid()}.3dm", 7);
        }
    }
}
