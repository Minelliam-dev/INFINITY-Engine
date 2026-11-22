using ECS;
using OpenTK.Mathematics;

public class Model
{
    public bool Enabled = false;

    public Shader shaders;
        
    public OpenTK.Mathematics.Vector3 position;
    public Vector3 rotation = new Vector3(0, 0, 0);
    public Vector3 scale = new Vector3(1);

    public Scene parentScene;
    public bool IsTransparent;

    public bool is2D = false;

    public bool IsStatic;

    public Mesh mesh;
    public BoxCollider collision;
        
    public Model(Window win, Mesh Mesh, OpenTK.Mathematics.Vector3 Position, Scene ParentScene, Shader shader, bool Transparent=false, bool Static=false)
    {   
        position = Position;
        IsStatic = Static;
        parentScene = ParentScene;

        collision = new BoxCollider(Mesh.Vertices, this, win);

        shaders = shader;

        IsTransparent = Transparent;

        mesh = Mesh;
    
        ParentScene.Models.Add(this);
    }

    public Model(bool IsSprite, Window win, string Texture, Vector3 Position, Scene ParentScene, Shader shader)
    {
        if (IsSprite == false)
        {
            throw new Exception("Ur a moron");
        }
        
        Mesh meshPlane = new Mesh("../Prefabs/shapes/plane.obj", Texture);
        
        position = Position;
        IsStatic = false;
        parentScene = ParentScene;

        collision = new BoxCollider(meshPlane.Vertices, this, win);

        shaders = shader;

        IsTransparent = true;

        mesh = meshPlane;
    
        ParentScene.Models.Add(this);
    }

    public Model()
    {
        
    }


    public void CustomInit(bool IsSprite, Window win, string Texture, Vector3 Position, Scene ParentScene, Shader shader)
    {
        if (IsSprite == false)
        {
            throw new Exception("Ur a moron");
        }
        
        Mesh meshPlane = new Mesh("./Engine/Prefabs/Shapes/Plane.obj", Texture);
        
        position = Position;
        IsStatic = false;
        parentScene = ParentScene;

        collision = new BoxCollider(meshPlane.Vertices, this, win);

        shaders = shader;

        IsTransparent = true;

        mesh = meshPlane;
    
        ParentScene.Models.Add(this);
    }
}