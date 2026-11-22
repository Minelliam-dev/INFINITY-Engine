using OpenTK.Mathematics;

public struct RaycastHit(Vector3 Point, float Distance, Model Object)
{
    public Vector3 Point = Point;
    public float Distance = Distance;
    public Model Object = Object;
}

public class Ray
{
    public Vector3 Origin;
    public Vector3 Direction;

    public static bool RayAABB(Ray ray, Vector3 min, Vector3 max, out float distance)
    {
        float tMin = 0f;
        float tMax = float.PositiveInfinity;

        for (int axis = 0; axis < 3; axis++)
        {
            float origin = ray.Origin[axis];
            float direction = ray.Direction[axis];

            if (Math.Abs(direction) < 0.000001f)
            {
                if (origin < min[axis] || origin > max[axis])
                {
                    distance = 0;
                    return false;
                }

                continue;
            }

            float t1 = (min[axis] - origin) / direction;
            float t2 = (max[axis] - origin) / direction;

            float near = Math.Min(t1, t2);
            float far = Math.Max(t1, t2);

            tMin = Math.Max(tMin, near);
            tMax = Math.Min(tMax, far);

            if (tMin > tMax)
            {
                distance = 0;
                return false;
            }
        }

        distance = tMin;
        return true;
    }

    public Ray(Vector3 origin, Vector3 direction)
    {
        Origin = origin;
        Direction = Vector3.Normalize(direction);
    }

    public static Vector3 GetPoint(Vector3 origin, Vector3 direction, float distance)
    {
        return origin + direction * distance;
    }

    public static bool Cast(Window win, Vector3 origin, Vector3 direction, float distance, out RaycastHit hit)
    {
        List<RaycastHit> ModelsHit = new List<RaycastHit>();
        bool Hit = false;
        
        for (int i=0; i<win.Models.Count; i++)
        {
            Model model = win.Models[i];

            float Distance = 0f;

            if (RayAABB(new Ray(origin, direction), model.collision.Min, model.collision.Max, out Distance))
            {
                Vector3 point = GetPoint(origin, direction, Distance);

                ModelsHit.Add(new RaycastHit(point, Distance, model));

                Hit = true;
            }
        }

        ModelsHit.Sort((a, b) =>
            a.Distance.CompareTo(b.Distance)
        );

        if (ModelsHit.Count > 0) hit = ModelsHit[0];
        else hit = new RaycastHit();

        return Hit;
    }
}