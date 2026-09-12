using ECS;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

Window window = new Window(500, 500, "New, New game engine");

window.VSync = OpenTK.Windowing.Common.VSyncMode.Off;
window.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;

Scene TestScene = new Scene(window);
Scene SecondScene = new Scene(window);

Variables variables = new Variables(0, 0);

TestScene.StartFunction = Start;
TestScene.UpdateFunction = Update;

SecondScene.UpdateFunction = Update;

for (int i=0; i<(6); i++)
{
    Model model  = new Model("./Engine/Prefabs/Shapes/Cube.obj", new Vector3(0, 0, (i*3)), TestScene, "./Engine/Debug/Debug-01.png");
    Model model2 = new Model("./Engine/Prefabs/Shapes/Cone.obj", new Vector3(3, 0, (i*3)), TestScene, "./Engine/Debug/Debug-02.png");
    Model model3 = new Model("./Engine/Prefabs/Shapes/Cylinder.obj", new Vector3(6, 0, (i*3)), TestScene, "./Engine/Debug/Debug-03.png");
    Model model4 = new Model("./Engine/Prefabs/Shapes/Plane.obj", new Vector3(9, 0, (i*3)), TestScene, "./Engine/Debug/Debug-04.png", true);
    Model model5 = new Model("./Engine/Prefabs/Shapes/Sphere.obj", new Vector3(12, 0, (i*3)), TestScene, "./Engine/Debug/Debug-01.png");
    Model model6 = new Model("./Engine/Prefabs/Shapes/Torus.obj", new Vector3(15, 0, (i*3)), TestScene, "./Engine/Debug/Debug-02.png");
}

new Model("./Engine/Prefabs/Shapes/Ocean.obj", new Vector3(30, 0, -30), SecondScene, "./Engine/Debug/Debug-02.png");

TestScene.Lights.Add(new PointLight(new Vector3(-2, 0, 0), 5f, new Vector3(0, 3, 0)));
TestScene.Lights.Add(new PointLight(new Vector3(17, 2, 0), 5f, new Vector3(3, 0, 0)));
TestScene.Lights.Add(new PointLight(new Vector3(0, 15, 0), 5f, new Vector3(0, 0, 3)));

SecondScene.Lights.Add(new PointLight(new Vector3(-2, 0, 0), 5f, new Vector3(0, 3, 0)));
SecondScene.Lights.Add(new PointLight(new Vector3(17, 2, 0), 5f, new Vector3(3, 0, 0)));
SecondScene.Lights.Add(new PointLight(new Vector3(0, 15, 0), 5f, new Vector3(0, 0, 3)));

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
}