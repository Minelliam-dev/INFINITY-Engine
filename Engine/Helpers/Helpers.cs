using System.Numerics;

public class Helpers
{
    public static string IntArrayToString(int[] Array)
    {
        string Output = "";

        for (int i=0; i<Array.Length; i++)
        {
            Output += (Array[i].ToString() + " ");
        }

        return Output;
    }

    public static string FloatArrayToString(float[] Array)
    {
        string Output = "";

        for (int i=0; i<Array.Length; i++)
        {
            Output += (Array[i].ToString() + " ");
        }

        return Output;
    }

    public static string StringArrayToString(String[] Array)
    {
        string Output = "";

        for (int i=0; i<Array.Length; i++)
        {
            Output += (Array[i].ToString() + " ");
        }

        return Output;
    }

    static public Vector3 PixelToGLPos(Vector3 pos, int width, int height)
        {
            return new Vector3(
                (pos.X / width) * 2f - 1f,
                1f - (pos.Y / height) * 2f,
                pos.Z / 10000
            );
        } 

    public static Vector3 GLPosToPixel(Vector3 pos, int width, int height)
    {
        return new Vector3(
            ((pos.X + 1f) * 0.5f) * width,
            ((1f - pos.Y) * 0.5f) * height,
            pos.Z / 10000
        );
    }

    public static OpenTK.Mathematics.Vector3 ToOpenTKVector(System.Numerics.Vector3 inp)
    {
        return new OpenTK.Mathematics.Vector3(inp.X, inp.Y, inp.Z);
    }

    public static OpenTK.Mathematics.Vector2 ToOpenTKVector(System.Numerics.Vector2 inp)
    {
        return new OpenTK.Mathematics.Vector2(inp.X, inp.Y);
    }

    public static System.Numerics.Vector3 FromOpenTKVector(OpenTK.Mathematics.Vector3 inp)
    {
        return new System.Numerics.Vector3(inp.X, inp.Y, inp.Z);
    }

    public static System.Numerics.Vector2 FromOpenTKVector(OpenTK.Mathematics.Vector2 inp)
    {
        return new System.Numerics.Vector2(inp.X, inp.Y);
    }
}