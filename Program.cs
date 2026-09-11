using ECS;
using OpenTK.Mathematics;

Window window = new Window(500, 500, "New, New game engine");

window.VSync = OpenTK.Windowing.Common.VSyncMode.Off;
window.CursorState = OpenTK.Windowing.Common.CursorState.Grabbed;

Scene TestScene = new Scene(window);

Model model  = new Model("./Engine/Prefabs/Shapes/Cube.obj", new Vector3(0, 0, 0), TestScene, "./Engine/Debug/Debug-01.png");
Model model2 = new Model("./Engine/Prefabs/Shapes/Cone.obj", new Vector3(3, 0, 0), TestScene, "./Engine/Debug/Debug-02.png");
Model model3 = new Model("./Engine/Prefabs/Shapes/Cylinder.obj", new Vector3(6, 0, 0), TestScene, "./Engine/Debug/Debug-03.png");
Model model4 = new Model("./Engine/Prefabs/Shapes/Plane.obj", new Vector3(9, 0, 0), TestScene, "./Engine/Debug/Debug-04.png");
Model model5 = new Model("./Engine/Prefabs/Shapes/Sphere.obj", new Vector3(12, 0, 0), TestScene, "./Engine/Debug/Debug-01.png");
Model model6 = new Model("./Engine/Prefabs/Shapes/Torus.obj", new Vector3(15, 0, 0), TestScene, "./Engine/Debug/Debug-02.png");

TestScene.Load();

// Test lights
window.lighting.Add(
    new Vector3(0, 0, 1),
    4f
);

window.lighting.Add(
    new Vector3(2, 0, 0),
    2f
);

window.Run();