using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GFramework
{
    public class Point
    {
        public float x;
        public float y;
    }

    public static class ConvexHull
    {
        public static List<Point> FindConvexPolygon(List<Point> points)
        {
            List<Point> top = new List<Point>();
            List<Point> bottom = new List<Point>();

            IEnumerable<Point> finalTop;
            IEnumerable<Point> finalBottom;

            points.Sort((x, y) =>
            {
                return (int)(x.x - y.x);
            });

            float deltaX = points.Last().x - points.First().x;
            float deltaY = points.Last().y - points.First().y;
            float denominator = (Mathf.Pow(deltaX, 2) + Mathf.Pow(deltaY, 2));

            for (int i = 1; i < points.Count - 1; i++)
                if (MinimumDistanceBetweenPointAndLine2D(points.First(), points.Last(), points[i], deltaX, deltaY, denominator))
                    bottom.Add(points[i]);
                else
                    top.Add(points[i]);

            top.Insert(0, points.First());
            top.Add(points.Last());
            bottom.Insert(0, points.First());
            bottom.Add(points.Last());

            finalTop = ConvexHullCore(top, true);
            finalBottom = ConvexHullCore(bottom, false);

            return finalTop.Union(finalBottom.Reverse()).ToList();
        }

        private static IEnumerable<Point> ConvexHullCore(List<Point> points, bool isTop)
        {
            List<Point> result = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                result.Add(points[i]);
                if (result.Count > 2 && !IsAngleConvex(result, isTop))
                {
                    result.Remove(result[result.Count - 2]);
                    result = ConvexHullCore(result, isTop).ToList();
                }
            }
            return result;
        }

        private static bool IsAngleConvex(List<Point> result, bool isTop)
        {
            Point lastPoint = result.Last();
            Point middlePoint = result[result.Count - 2];
            Point firstPoint = result[result.Count - 3];

            float firstAngle = Mathf.Atan2(middlePoint.y - firstPoint.y, middlePoint.x - firstPoint.x) * 180.0f / Mathf.PI;
            float secondAngle = Mathf.Atan2(lastPoint.y - middlePoint.y, lastPoint.x - middlePoint.x) * 180.0f / Mathf.PI;

            return isTop ? secondAngle < firstAngle : secondAngle > firstAngle;
        }

        private static bool MinimumDistanceBetweenPointAndLine2D(Point P1, Point P2, Point P3, float deltaX, float deltaY, float denominator)
        {
            float u = ((P3.x - P1.x) * deltaX + (P3.y - P1.y) * deltaY) / denominator;
            //float x = P1.X + u * deltaX;
            float y = P1.y + u * deltaY;
            return y - P3.y > 0;
        }
    }
}