using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Systems
{
    public struct Location : IEquatable<Location>
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Location(int x = 0, int y = 0, int z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3 ToVector3() =>
            new Vector3(X, Y, Z);

        public Location AddX(int a) => new Location(this.X + a, this.Y, this.Z);
        public Location AddY(int a) => new Location(this.X , this.Y + a, this.Z);
        public Location AddZ(int a) => new Location(this.X , this.Y, this.Z + a);

        public bool Equals(Location other) => this.X == other.X && this.Y == other.Y && this.Z == other.Z;
        public static Location operator +(Location a) => a;
        public static Location operator -(Location a) => new Location(-a.X, -a.Y, -a.Z);
        public static Location operator +(Location a, Location b) => new Location(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Location operator -(Location a, Location b) => new Location(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Location operator *(Location a, Location b) => new Location(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        public static Location operator /(Location a, Location b) => new Location(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        public static Location operator +(Location a, int b) => new Location(a.X + b, a.Y + b, a.Z + b);
        public static Location operator -(Location a, int b) => new Location(a.X - b, a.Y - b, a.Z - b);
        public static Location operator *(Location a, int b) => new Location(a.X * b, a.Y * b, a.Z * b);
        public static Location operator /(Location a, int b) => new Location(a.X / b, a.Y / b, a.Z / b);
        public static bool operator ==(Location a, Location b) => a.Equals(b);
        public static bool operator !=(Location a, Location b) => !a.Equals(b);
    }
}
