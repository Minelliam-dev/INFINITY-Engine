using OpenTK.Mathematics;
public class PointLight(Vector3 Position, float Radius)
{
    public Vector3 Position = Position;
    public float Radius = Radius;
}

public class Lighting
{
    const int MaxLights = 32;

    public List<PointLight> Lights = [];

    public void Add(Vector3 position, float Radius)
    {
        if (Lights.Count >= MaxLights)
            return;

        Lights.Add(new PointLight(position, Radius));
    }

    public void Upload(Shader shader)
    {
        int count = Math.Min(Lights.Count, MaxLights);

        shader.SetInt("lightCount", count);

        for (int i = 0; i < count; i++)
        {
            PointLight light = Lights[i];

            shader.SetVector3(
                $"lightPositions[{i}]",
                light.Position
            );

            shader.SetFloat(
                $"lightStrengths[{i}]",
                light.Radius
            );
        }
    }
}