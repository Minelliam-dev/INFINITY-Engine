using ECS;
using OpenTK.Mathematics;

public class Sprite() : Model()
{
    
    public Sprite(Window win, Vector2 position, string texture, Scene targetScene, Shader shader) : this()
    {
        base.CustomInit(true, win, texture, new Vector3(position.X, position.Y, 0),targetScene, shader);

        base.is2D = true;
        base.scale = new Vector3(100);

        base.position = new Vector3(0, 0, 0);

        base.rotation.Y = 45f;
        
    }
}
