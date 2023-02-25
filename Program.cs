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
            AdaptiveCurveFix();
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
        static void AdaptiveCurveFix()
        {
            string keyName = "adaptivecurve.3dm";
            var doc = File3dm.Read(keyName);
            var objects = doc.Objects;
            var polygons = new List<Polygon>();
            foreach (var obj in objects)
            {
                var brep = (rg.Brep)obj.Geometry;
                var guidePolygon = brep.Faces[0].OuterLoop.To3dCurve().ToPolygon().ForceZAxisOrientation();

                IList<Vector3> vertices = brep.Faces[0].GetBrepEdgeVertices();
                Polygon poly = GuidedPolygonFromVertices(guidePolygon, vertices);
                polygons.Add(poly);
            }
            Console.WriteLine($"{polygons.Count}");
            var newfile = new File3dm();
            foreach (var p in polygons)
            {
                newfile.Objects.AddPolyline(p.ToRgPolyline());
            }
            newfile.Write($"{System.Guid.NewGuid()}.3dm",7);
        }

        private static Polygon GuidedPolygonFromVertices(Polygon guidePolygon, IList<Vector3> vertices)
        {
            IList<(Vector3 point, double param)> pairs = new List<(Vector3 point, double param)>();
            for (int i = 0; i < vertices.Count; i++)
            {
                Vector3 closestPoint;
                var dist = vertices[i].PolygonDistanceTo(guidePolygon, out closestPoint);
                // Console.WriteLine(dist);
                var param = guidePolygon.PolygonGetParameterAt(closestPoint, out var seg);
                var param2 = guidePolygon.PolygonGetParameterAt3(vertices[i], out var seg2);
                pairs.Add((vertices[i], param2));
                Console.WriteLine($"dist: {dist}, param: {param}, seg: {seg}");
                Console.WriteLine($"dist: {dist}, param: {param2}, seg: {seg2}");
            }
            var pts = pairs.OrderBy(p => p.param).Select(p => p.point).ToList();
            var poly = new Polygon(pts);
            return poly;
        }
    }
}
