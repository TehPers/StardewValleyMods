using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace TehCore.Menus.BoxModel {
    public struct Vector2I {
        public int X { get; }
        public int Y { get; }

        public Vector2I(int x, int y) {
            this.X = x;
            this.Y = y;
        }

        public double Dot(Vector2I other) => this.X * other.X + this.Y * other.Y;

        public double Magnitude() => Math.Sqrt(this.X * this.X + this.Y * this.Y);

        public Vector2I Normalize() => this / this.Magnitude();

        public Vector2I Project(params Vector2I[] space) => this.Project(space as IEnumerable<Vector2I>);
        public Vector2I Project(IEnumerable<Vector2I> space) {
            Vector2I tmpThis = this;
            return space.Aggregate(new Vector2I(0, 0), (current, v) => current + tmpThis.Dot(v) * current.Dot(v) * v);
        }

        public Vector2I Translate(int addX, int addY) => new Vector2I(this.X + addX, this.Y + addY);

        public Vector2 ToVector2() => new Vector2(this.X, this.Y);

        public static Vector2I operator +(Vector2I a, Vector2I b) => new Vector2I(a.X + b.X, a.Y + b.Y);
        public static Vector2I operator -(Vector2I a, Vector2I b) => new Vector2I(a.X - b.X, a.Y - b.Y);
        public static Vector2I operator -(Vector2I vector) => new Vector2I(-vector.X, -vector.Y);
        public static Vector2I operator *(Vector2I vector, double scalar) => new Vector2I((int) (vector.X * scalar), (int) (vector.Y * scalar));
        public static Vector2I operator *(double scalar, Vector2I vector) => new Vector2I((int) (vector.X * scalar), (int) (vector.Y * scalar));
        public static Vector2I operator /(Vector2I vector, double scalar) => new Vector2I((int) (vector.X / scalar), (int) (vector.Y / scalar));
    }
}