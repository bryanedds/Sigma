using System;
using System.Collections.Generic;
using System.Linq;

namespace Sigma
{
    /// <summary>
    /// Math functions implemented in Sigma.
    /// </summary>
    public static class SigmaMath
    {
        public static PointI Min(PointI point, PointI point2)
        {
            return new PointI(
                Math.Min(point.X, point2.X),
                Math.Min(point.Y, point2.Y));
        }

        public static PointI Max(PointI point, PointI point2)
        {
            return new PointI(
                Math.Max(point.X, point2.X),
                Math.Max(point.Y, point2.Y));
        }

        public static List<PointI> GetCells(PointI extent)
        {
            return GetCells(PointI.Zero, extent);
        }

        public static List<PointI> GetCells(PointI origin, PointI extent)
        {
            return
                (from x in Enumerable.Range(origin.X, extent.X)
                 from y in Enumerable.Range(origin.Y, extent.Y)
                 select new PointI(x, y))
                .ToList();
        }

        public static bool IsPointInBounds(PointI point, PointI origin, PointI extent)
        {
            var corner = origin + extent;
            return
                point.X >= origin.X &&
                point.X < corner.X &&
                point.Y >= origin.Y &&
                point.Y < corner.Y;
        }

        public static bool IsBoundsInBounds(PointI origin, PointI extent, PointI origin2, PointI extent2)
        {
            var corner = origin + extent;
            var corner2 = origin2 + extent2;
            return
                origin.X < corner2.X &&
                corner.X > origin2.X &&
                origin.Y < corner.Y &&
                corner.Y > origin2.Y;
        }
    }
}
