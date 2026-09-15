using ECS;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

Window window = new Window(500, 500, "New, New game engine");

window.VSync = OpenTK.Windowing.Common.VSyncMode.Off;
window.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;

Scene TestScene = new Scene(window);
Scene SecondScene = new Scene(window);

TestScene.CombineStaticObjects = true;

Variables variables = new Variables(0, 0);

TestScene.StartFunction = Start;
TestScene.UpdateFunction = Update;
SecondScene.UpdateFunction = Update;

Mesh Torus = new Mesh("./Engine/Prefabs/Shapes/Torus.obj", "./Engine/Debug/Debug-01.png");
Mesh Sphere = new Mesh("./Engine/Prefabs/Shapes/Sphere.obj", "./Engine/Debug/Debug-01.png");
Mesh Plane = new Mesh("./Engine/Prefabs/Shapes/Plane.obj", "./Engine/Debug/Debug-01.png");
Mesh Cone = new Mesh("./Engine/Prefabs/Shapes/Cone.obj", "./Engine/Debug/Debug-01.png");
Mesh Cube = new Mesh("./Engine/Prefabs/Shapes/Cube.obj", "./Engine/Debug/Debug-01.png");
Mesh Test = new Mesh("./Engine/Prefabs/Test.obj", "./Engine/Debug/Debug-02.png");
Mesh Cylinder = new Mesh("./Engine/Prefabs/Shapes/Cylinder.obj", "./Engine/Debug/Debug-01.png");

Shader BaseShader = new Shader("./Engine/Shaders/Vertex.glsl", "./Engine/Shaders/Grayscale-Fragment.glsl");
Shader ToonShader = new Shader("./Engine/Shaders/Vertex.glsl", "./Engine/Shaders/Toon-Fragment.glsl");
Shader InvertedShader = new Shader("./Engine/Shaders/Vertex.glsl", "./Engine/Shaders/Inverted-Fragment.glsl");
Shader NoLightShader = new Shader("./Engine/Shaders/Vertex.glsl", "./Engine/Shaders/NoLighting-Fragment.glsl");

Model model = new Model(window, Cube, new Vector3(0, 0, 0), TestScene, BaseShader);

for (int i=0; i<(1); i++)
{
    Model model2 = new Model(window, Torus, new Vector3(3, 0, (i*3)), TestScene, NoLightShader, Static: false);
    Model model3 = new Model(window, Sphere, new Vector3(6, 0, (i*3)), TestScene, ToonShader, Static: false);
    Model model4 = new Model(window, Plane, new Vector3(9, 0, (i*3)), TestScene, InvertedShader, Transparent: true, Static: false);
    Model model5 = new Model(window, Cone, new Vector3(12, 0, (i*3)), TestScene, NoLightShader, Static: false);
    Model model6 = new Model(window, Cube, new Vector3(15, 0, (i*3)), TestScene, BaseShader, Static: false);
}

Model model0 = new Model(window, Test, new Vector3(0, 0, 0), SecondScene, ToonShader);

model0.scale = 2f;

TestScene.Lights.Add(new PointLight(new Vector3(-2, -1, 0), 1f, new Vector3(3, 0, 0)));
TestScene.Lights.Add(new PointLight(new Vector3(2, -1, 0), 1f, new Vector3(0, 3, 0)));
TestScene.Lights.Add(new PointLight(new Vector3(1, 1, 0), 1f, new Vector3(0, 0, 3)));

SecondScene.Lights.Add(new PointLight(new Vector3(-2, -1, 0), 1f, new Vector3(3, 0, 0)));
SecondScene.Lights.Add(new PointLight(new Vector3(2, -1, 0), 1f, new Vector3(0, 3, 0)));
SecondScene.Lights.Add(new PointLight(new Vector3(1, 1, 0), 1f, new Vector3(0, 0, 3)));

window.CurrentScene = TestScene;
window.Run();



void PlayerInput(float Deltatime)
{
    KeyboardState input = window.KeyboardState;
    float speed = 3f * Deltatime;
    
    if (input.IsKeyDown(Keys.W))
    {
        window.camera.Position += window.camera.front * speed; //Forward 
        window.camera.Target += window.camera.front * speed; //Forward 
    }
    if (input.IsKeyDown(Keys.S))
    {
        window.camera.Position -= window.camera.front * speed; //Forward 
        window.camera.Target -= window.camera.front * speed; //Forward 
    }
    if (input.IsKeyDown(Keys.D))
    {
        window.camera.Position += window.camera.right * speed; //Forward 
        window.camera.Target += window.camera.right * speed; //Forward 
    }
    if (input.IsKeyDown(Keys.A))
    {
        window.camera.Position -= window.camera.right * speed; //Forward 
        window.camera.Target -= window.camera.right * speed; //Forward 
    }
    if (input.IsKeyDown(Keys.LeftShift))
    {
        window.camera.Position -= Vector3.UnitY * speed; //Forward 
        window.camera.Target -= Vector3.UnitY * speed; //Forward 
    }
    if (input.IsKeyDown(Keys.Space))
    {
        window.camera.Position += Vector3.UnitY * speed; //Forward 
        window.camera.Target += Vector3.UnitY * speed; //Forward 
    }
    
    Vector2 mouse = window.MouseState.Position;
    float Sensitivity = .2f;
    if (variables.FirstMouseMove)
    {
        variables.lastMousePos = new Vector2(mouse.X, mouse.Y);
        variables.FirstMouseMove = false;
    }
    else
    {
        float deltaX = mouse.X - variables.lastMousePos.X;
        float deltaY = mouse.Y - variables.lastMousePos.Y;
        variables.lastMousePos = new Vector2(mouse.X, mouse.Y);
        variables.yaw += deltaX * Sensitivity;
        
        variables.pitch -= deltaY * Sensitivity;
        variables.pitch = Math.Clamp(variables.pitch, -89f, 89f);
    }
    Vector3 front;
    front.X =
        MathF.Cos(MathHelper.DegreesToRadians(variables.pitch)) *
        MathF.Cos(MathHelper.DegreesToRadians(variables.yaw));
    front.Y =
        MathF.Sin(MathHelper.DegreesToRadians(variables.pitch));
    front.Z =
        MathF.Cos(MathHelper.DegreesToRadians(variables.pitch)) *
        MathF.Sin(MathHelper.DegreesToRadians(variables.yaw));
    front = Vector3.Normalize(front);
    window.camera.Target = window.camera.Position + front;
}

void Start()
{
    return;
}

void Update()
{
    PlayerInput(window.DeltaTime);

    Debug.Log(window.FPS);

    if (window.KeyboardState.IsKeyPressed(Keys.D0))
    {
        window.ChangeScene(SecondScene);
    }

    if (window.KeyboardState.IsKeyPressed(Keys.D1))
    {
        window.ChangeScene(TestScene);
    }

    //TestScene.Models[0].rotation.Y += (20f * window.DeltaTime);
    SecondScene.Models[0].rotation.Y += (20f * window.DeltaTime);

    TestScene.Models[0].position = new Vector3(window.camera.Position.X, window.camera.Position.Y, TestScene.Models[0].position.Z);

    CollisionInfo Info;

    Debug.Log(TestScene.Models[0].collision.GetCollision(out Info));
}
