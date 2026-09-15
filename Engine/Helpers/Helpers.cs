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
}