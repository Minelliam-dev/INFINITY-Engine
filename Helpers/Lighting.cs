using OpenTK.Mathematics;
public class PointLight
{
    public Vector3 position;
    public float radius;
    public Vector3 color;

    public PointLight(Vector3 Position, float Radius, Vector3 Color=new Vector3())
    {
        position = Position;
        radius = Radius;
        
        if (Color != new Vector3())
        {
            color = Color;
        }
        else
        {
            color = new Vector3(1, 1, 1);
        }
    }
}

public class Lighting
{
    const int MaxLights = 64;
    public List<PointLight> Lights = [];

    public void Add(Vector3 position, float Radius, Vector3 Color = new Vector3())
    {
        if (Lights.Count >= MaxLights)
            return;

        Lights.Add(new PointLight(position, Radius, Color));
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
                light.position
            );

            shader.SetVector3(
                $"lightColors[{i}]",
                light.color
            );

            shader.SetFloat(
                $"lightStrengths[{i}]",
                light.radius
            );
        }
    }
}