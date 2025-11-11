using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace prrprr_projekt_oop.Forms
{
    public class Circle
    {
        private Vector2 pos;
        private float radius;
        public Vector2 Pos { get => pos; }
        public float Radius { get => radius; }
        public void ChangePos(Vector2 pos)
        {
            this.pos = pos;
        }

        public Circle(Vector2 pos, float radius)
        {
            this.pos = pos;
            this.radius = radius;
        }

        public bool ContainsPoint(Point point)
        {
            Vector2 temp = new Vector2(point.X - pos.X, point.Y - pos.Y);
            return temp.Length() <= radius;
        }


        // Intersect for just Recatangle
        public bool Intersects(Rectangle rectangle)
        {

            Point[] corners =
            [
                new Point(rectangle.Left, rectangle.Top),
                new Point(rectangle.Right, rectangle.Top),
                new Point(rectangle.Right, rectangle.Bottom),
                new Point(rectangle.Left, rectangle.Bottom)
            ];

            foreach (Point corner in corners)
            {
                if (ContainsPoint(corner))
                    return true;
            }

            if (pos.X - radius > rectangle.Right || pos.X + radius < rectangle.Left)
                return false;

            if (pos.Y - radius > rectangle.Bottom || pos.Y + radius < rectangle.Top)
                return false;

            return true;
        }

        // Intersect for just Circle
        public bool Intersects(Circle circle)
        {

            return Vector2.Distance(circle.pos, pos) < radius + circle.Radius;

        }

        public bool Intersects(Vector2[] polygon)
        {
            // Check for intersection with polygon edges
            for (int i = 0; i < polygon.Length; i++)
            {
                Vector2 start = polygon[i];
                Vector2 end = polygon[(i + 1) % polygon.Length];

                if (LineIntersectsCircle(start, end, pos, Radius))
                    return true;
            }

            // Check if the circle's center is inside the polygon
            if (PointInPolygon(pos, polygon))
                return true;

            return false;
        }

        private bool LineIntersectsCircle(Vector2 a, Vector2 b, Vector2 center, float radius)
        {
            Vector2 ab = b - a;
            Vector2 ac = center - a;

            float abLengthSquared = ab.LengthSquared();
            float projection = Vector2.Dot(ac, ab) / abLengthSquared;
            projection = MathHelper.Clamp(projection, 0f, 1f);

            Vector2 closestPoint = a + projection * ab;
            float distanceSquared = Vector2.DistanceSquared(center, closestPoint);

            return distanceSquared <= radius * radius;
        }

        private bool PointInPolygon(Vector2 point, Vector2[] polygon)
        {
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if (((polygon[i].Y > point.Y) != (polygon[j].Y > point.Y)) &&
                    (point.X < (polygon[j].X - polygon[i].X) *
                    (point.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X))
                {
                    inside = !inside;
                }
            }
            return inside;
        }

        public void DrawCircle(Color color, SpriteBatch _spriteBatch, Texture2D texture)
        {
            int r = (int)radius;
            int cx = (int)pos.X;
            int cy = (int)pos.Y;

            int thickness = 1; // thickness of the outline

            for (int x = -r; x <= r; x++)
            {
                for (int y = -r; y <= r; y++)
                {
                    if (x * x + y * y <= r * r) // Check if inside circle
                    {
                        _spriteBatch.Draw(texture, new Vector2(cx + x, cy + y), color);
                        if (x * x + y * y >= ((r - thickness) * (r - thickness)))
                        {
                            _spriteBatch.Draw(texture, new Vector2(cx + x, cy + y), new Color(0, 0, 0));
                        }
                    }
                }
            }
        }
    }
}