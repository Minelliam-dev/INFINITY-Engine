using System.Numerics;

//public class OBJLoader(string FilePath)
//{
//    public string Path = FilePath;
//    string[] Lines = [];
//    bool Parsed = false;
//
//    void Parse()
//    {
//        Lines = File.ReadAllLines(Path);
//    }
//
//    public float[] GetVertices()
//    {
//        float[] Output = new float[0];
//        
//        if (!Parsed) Parse();
//
//        List<Vector3> Positions = new List<Vector3>();
//        List<Vector2> UVPositions = new List<Vector2>();
//
//        for (int i=0; i<Lines.Length; i++)
//        {
//            string CurrentLine = Lines[i];
//            
//            if (CurrentLine.StartsWith("v "))
//            {
//                string[] pos = new string[4];
//
//                pos = CurrentLine.Split(" ");
//
//                Vector3 outp = new Vector3(float.Parse(pos[1]), float.Parse(pos[2]), float.Parse(pos[3]));
//                
//                Positions.Add(outp);
//            }
//
//            if (CurrentLine.StartsWith("vt "))
//            {
//                string[] pos = new string[3];
//
//                pos = CurrentLine.Split(" ");
//
//                Vector2 outp = new Vector2(float.Parse(pos[1]), float.Parse(pos[2]));
//                
//                UVPositions.Add(outp);
//            }
//        }
//
//        Output = new float[Positions.Count*5];
//        
//        for (int i=0; i<Positions.Count; i++)
//        {
//            int ci = i*5;
//            
//            Output[ci + 0] = Positions[i].X;
//            Output[ci + 1] = Positions[i].Y;
//            Output[ci + 2] = Positions[i].Z;
//
//            Output[ci + 3] = -.5f;
//            Output[ci + 4] = 1;
//        }
//        
//        return Output;
//    }
//
//    public int[] Getindices()
//    {
//        List<int> Output = new List<int>();
//        
//        if (!Parsed) Parse();
//
//        for (int i=0; i<Lines.Length; i++)
//        {
//            string CurrentLine = Lines[i];
//            
//            if (CurrentLine.StartsWith("f "))
//            {
//                string[] pos = CurrentLine.Split(" ");
//
//                for (int p=1; p<pos.Length; p++)
//                {
//                    Output.Add(int.Parse(pos[p].Split("/")[0])-1);
//                    Output.Add(int.Parse(pos[p].Split("/")[1])-1);
//                    Output.Add(int.Parse(pos[p].Split("/")[2])-1);
//                }
//
//                
//            }
//        }
//        
//        return Output.ToArray();
//    }
//}


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