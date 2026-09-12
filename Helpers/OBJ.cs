using OpenTK.Mathematics;


//this class is just a mess, i wont bother commenting them because i am propably going to re-write it anyway
public class OBJLoader(string FilePath)
{
    public string Path = FilePath;

    string[] Lines = [];
    bool Parsed = false;

    void Parse()
    {
        Lines = File.ReadAllLines(Path);
        Parsed = true;
    }

    public void GetModel(out float[] vertices, out int[] indices)
    {
        if (!Parsed)
            Parse();

        List<Vector3> positions = new();
        List<Vector2> uvs = new();

        foreach (string line in Lines)
        {
            string[] parts = line.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                continue;

            if (parts[0] == "v")
            {
                positions.Add(new Vector3(
                    float.Parse(parts[1]),
                    float.Parse(parts[2]),
                    float.Parse(parts[3])
                ));
            }

            else if (parts[0] == "vt")
            {
                uvs.Add(new Vector2(
                    float.Parse(parts[1]),
                    float.Parse(parts[2])
                ));
            }
        }

        List<float> outputVertices = new();
        List<int> outputIndices = new();

        foreach (string line in Lines)
        {
            string[] parts = line.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0 || parts[0] != "f")
                continue;

            for (int i = 1; i < parts.Length; i++)
            {
                string[] face = parts[i].Split('/');

                int positionIndex = int.Parse(face[0]) - 1;
                int uvIndex = int.Parse(face[1]) - 1;

                Vector3 position = positions[positionIndex];
                Vector2 uv = uvs[uvIndex];

                outputVertices.Add(position.X);
                outputVertices.Add(position.Y);
                outputVertices.Add(position.Z);

                outputVertices.Add(uv.X);
                outputVertices.Add(uv.Y);

                outputIndices.Add(outputIndices.Count);
            }
        }

        vertices = outputVertices.ToArray();
        indices = outputIndices.ToArray();
    }
}