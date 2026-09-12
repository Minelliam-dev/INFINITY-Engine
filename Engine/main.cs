using ECS;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

class Variables(int VertexBufferObject, int VertexArrayObject)
{
    public int VertexBufferObject = VertexBufferObject;
    public int VertexArrayObject = VertexArrayObject;

    public Shader shaders = new Shader("", "", true);
    public int ElementBufferObject;
    public Vector2 WindowSize;
    public bool FirstMouseMove = true;
    public Vector2 lastMousePos;
    public float pitch = 0;
    public float yaw = 0;

    public Matrix4 model;
    public Matrix4 view;
    public Matrix4 projection;

}

//I thought naming it Game was not very fit for a game engine so i renamed it to Window
public class Window : GameWindow
{
    //----------Variables----------
    Variables variables = new Variables(0, 0);
    public float[] vertices =
    {
        //Position          Texture coordinates
         0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
         0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
        -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
    };
    public int[] indices = {  // note that we start from 0!
        0, 1, 3,   // first triangle
        1, 2, 3    // second triangle
    };
    
    public Camera camera;

    public float FPS = 0;
    public float DeltaTime = 0;
    public float fpsTimer = 0;

    public List<Model> Models = new List<Model>();

    public Scene CurrentScene;

    public Lighting lighting = new Lighting();

    //----------Variables----------


    //Reference the pre-made window class provided by OpenTK to create a window
    public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) 
    { 
        variables.WindowSize = new Vector2(width, height); 

        CurrentScene = new Scene(this);

        camera = new Camera(new Vector3(0, 0, 3f), 60f, this);
    }
    
    public void ChangeScene(Scene NewScene)
    {
        if (NewScene == CurrentScene) return;
        
        CurrentScene.UnLoad();
        CurrentScene = NewScene;
        CurrentScene.Load();
    }
    
    //Runs every frame
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        FPS = 1f / (float)e.Time;

        DeltaTime = (float)e.Time;

        CurrentScene.Update();

        //Update the camera
        camera.Update();
        
        //Let the window do it's thing
        base.OnUpdateFrame(e);
    }

    //Also runs every frame but also renders to the screen
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        base.OnRenderFrame(e);

        //set the backround
        GL.Clear(ClearBufferMask.ColorBufferBit);

        lighting.Upload(variables.shaders);
        
        //render the current frame
        Render();
        
        //Swap between front and back buffers
        SwapBuffers();
    }

    //Runs whenever the window is resized
    protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
    {
        //Resize the window
        base.OnFramebufferResize(e);

        //Set the window size variable to be accurate after resizing
        variables.WindowSize = new Vector2(e.Width, e.Height);

        //change the OpenGL output to also cover the new screenspace
        GL.Viewport(0, 0, e.Width, e.Height);
    }
    
    void RenderObject(Model Object)
    {
        //Control = 1.5
        //No textures = 2.5
        //No shaders = 1.5
        //No models = 2.5
        //No matrices = 1.7

        //render the vertex data
        variables.shaders.Use();

        Providematrices(Object);

        Object.texture.UseWithoutLoad(Object.TextureHandle);
        
        GL.BindVertexArray(Object.VAO);

        GL.DrawElements(
            PrimitiveType.Triangles,
            Object.IndexCount,
            DrawElementsType.UnsignedInt,
            0
        );
    }
    void Render()
    {
        GL.Clear(
            ClearBufferMask.ColorBufferBit |
            ClearBufferMask.DepthBufferBit
        );
        
        List<Model> TransparentModels = new List<Model>();
        
        for (int i=0; i<Models.Count; i++)
        {
            if (Models[i].Enabled)
            {
                if (!Models[i].IsTransparent) RenderObject(Models[i]);
                else
                {
                    TransparentModels.Add(Models[i]);
                }
            }
        }

        for (int i=0; i<TransparentModels.Count; i++)
        {   
            RenderObject(TransparentModels[(TransparentModels.Count-1) - i]);
        }
    }
    void CreateVBO()
    {
        //Create the Vertex buffer
        variables.VertexBufferObject = GL.GenBuffer();

        //Bind the newly created buffer
        GL.BindBuffer(BufferTarget.ArrayBuffer, variables.VertexBufferObject);

        //Upload the vertex data into the buffer
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

        //Set the background color
        GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
    }
    void CreateVAO()
    {
        //Create and bind the Vertex array object (VAO)
        int VertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(VertexArrayObject);

        //Tell OpenGL how to use the Vertex data
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

        int texCoordLocation = variables.shaders.GetAttribLocation("aTexCoord");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

        
        //Enable the bound Vetex array object (VAO)
        GL.EnableVertexAttribArray(0);

        //Make a copy of the VAO for the OnRenderFrame function
        variables.VertexArrayObject = VertexArrayObject;
    }
    void CreateEBO()
    {
        //Create the Element Buffer so that we can use vertecies and indecies to save on VRAM
        variables.ElementBufferObject = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, variables.ElementBufferObject);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);
    }
    void TextureInit()
    {
        bool UseNearest = true;
        
        Texture texture = new Texture();
        texture.Use("./Engine/Debug/Missing_texture.png");
        
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        if (UseNearest) 
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
        }
        else 
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
    }

    void Providematrices(Model CurrentModel)
    {
        Matrix4 model = Matrix4.CreateScale(CurrentModel.scale) * Matrix4.CreateRotationX(CurrentModel.rotation.X) * Matrix4.CreateRotationY(CurrentModel.rotation.Y) * Matrix4.CreateRotationZ(CurrentModel.rotation.Z) * Matrix4.CreateTranslation(CurrentModel.position);
        variables.view = camera.view;

        variables.shaders.SetMatrix4("model", model);
        variables.shaders.SetMatrix4("view", variables.view);
        variables.shaders.SetMatrix4("projection", variables.projection);
    } 
    //Runs once before normal program execution
    protected override void OnLoad()
    {
        //Load the window stuff
        base.OnLoad();

        //Load the current scene
        CurrentScene.Load();
        
        //Create the vertex buffer object
        CreateVBO();

        //Create the vertex and fragment shaders
        variables.shaders = new Shader("./Engine/Shaders/Vertex.glsl", "./Engine/Shaders/Fragment.glsl");

        //create the vertex array object
        CreateVAO();

        //Create the EBO to use both indicies and verticies
        CreateEBO();

        //enable depth testing
        GL.Enable(EnableCap.DepthTest);

        GL.Enable(EnableCap.Blend);

        GL.BlendFunc(
            BlendingFactor.SrcAlpha,
            BlendingFactor.OneMinusSrcAlpha
        );

        variables.model = Matrix4.CreateScale(1f);
        variables.view = camera.view;
        variables.projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(camera.FOV), variables.WindowSize.X / variables.WindowSize.Y, 0.1f, 100.0f);

        variables.shaders.SetMatrix4("view", variables.view);
        variables.shaders.SetMatrix4("model", variables.model);
        variables.shaders.SetMatrix4("projection", variables.projection);

        //initialize texture parameters
        TextureInit();
    }

    protected override void OnUnload()
    {
        //delete the shaders to free up the used VRAM
        variables.shaders.Dispose();
        
        //unload the window
        base.OnUnload();
    }

}
