using OpenTK.Mathematics;
using ECS;

public class CollisionInfo(bool Colliding, Model CollisionModel, Vector3 CollisionDepth)
{
    public Model CollisionModel = CollisionModel;
    public bool isColliding = Colliding;
    public Vector3 CollisionDepth = CollisionDepth;
}

public class BoxCollider
{
    // LOCAL bounds
    public Vector3 A;
    public Vector3 B;

    Model self;
    Window win;

    public Vector3 Min =>
        A * self.scale + self.position;

    public Vector3 Max =>
        B * self.scale + self.position;

    public BoxCollider(
        float[] vertices,
        Model Self,
        Window Win)
    {
        self = Self;
        win = Win;

        GetBounds(vertices, out A, out B);
    }

    bool AABBCheck(
        Vector3 minA,
        Vector3 maxA,
        Vector3 minB,
        Vector3 maxB)
    {
        return
            minA.X <= maxB.X &&
            maxA.X >= minB.X &&

            minA.Y <= maxB.Y &&
            maxA.Y >= minB.Y &&

            minA.Z <= maxB.Z &&
            maxA.Z >= minB.Z;
    }

    Vector3 GetDepth(Model a, Model b)
    {
        Vector3 minA = a.collision.Min;
        Vector3 maxA = a.collision.Max;

        Vector3 minB = b.collision.Min;
        Vector3 maxB = b.collision.Max;

        return new Vector3(
            MathF.Min(maxA.X, maxB.X) -
            MathF.Max(minA.X, minB.X),

            MathF.Min(maxA.Y, maxB.Y) -
            MathF.Max(minA.Y, minB.Y),

            MathF.Min(maxA.Z, maxB.Z) -
            MathF.Max(minA.Z, minB.Z)
        );
    }

    public bool GetCollision(out CollisionInfo info)
    {
        info = new CollisionInfo(
            false,
            self,
            Vector3.Zero
        );

        foreach (Model other in win.Models)
        {
            if (other == self)
                continue;

            if (other.collision == null)
                continue;

            if (!AABBCheck(
                Min,
                Max,
                other.collision.Min,
                other.collision.Max))
            {
                continue;
            }

            info.isColliding = true;
            info.CollisionModel = other;
            info.CollisionDepth =
                GetDepth(self, other);

            return true;
        }

        return false;
    }

    public static void GetBounds(
        float[] vertices,
        out Vector3 min,
        out Vector3 max)
    {
        min = new Vector3(float.MaxValue);
        max = new Vector3(float.MinValue);

        for (int i = 0; i < vertices.Length; i += 5)
        {
            Vector3 v = new Vector3(
                vertices[i],
                vertices[i + 1],
                vertices[i + 2]
            );

            min = Vector3.ComponentMin(min, v);
            max = Vector3.ComponentMax(max, v);
        }
    }
}