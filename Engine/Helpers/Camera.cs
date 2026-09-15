using OpenTK.Mathematics;

public class Camera(Vector3 Position, float FOV, Window window)
{
    public Vector3 Position = Position;
    public float FOV = FOV;
    public Vector3 Target;
    public Vector3 Direction;
    Vector3 GlobalUp = Vector3.UnitY;
    public Vector3 right;
    public Matrix4 view;
    public Vector3 up;
    public Vector3 front;
    public Window window = window;


    public void Update()
    {   
        front = Vector3.Normalize(Target - Position);

        right = Vector3.Normalize(Vector3.Cross(front, GlobalUp));

        up = Vector3.Normalize(Vector3.Cross(right, front));

        view = Matrix4.LookAt(Position, Target, up);

        //Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(FOV), window.ClientSize.X / window.ClientSize.Y, 0.01f, 100f);
    }
}