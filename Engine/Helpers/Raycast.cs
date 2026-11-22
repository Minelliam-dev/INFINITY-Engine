using System.Numerics;
using ECS;

public struct RayHit
{
    public float Distance;
    public Model HitModel;
    public Vector3 HitPoint;
    public bool Hit;
}

public class Ray
{
    public Window win;

    public Ray(Window window)
    {
        win = window;
    }

    public RayHit Cast(Vector3 Origin, Vector3 Direction)
    {
        return new RayHit();
    }
}