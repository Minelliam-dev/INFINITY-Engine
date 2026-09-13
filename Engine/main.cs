using ECS;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

//this class only exists because i did not want to make the window class messy, 
//making this class messy instead. So i will not comment these variables
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

    public bool ClearScreen = true;

}

//I thought naming it Game was not very fit for a game engine so i renamed it to Window
public class Window : GameWindow
{
    //----------Variables----------
    
    //Variables is just my solution to make this class less disgusting
    Variables variables = new Variables(0, 0);
    
    //Vertices is just here in case there is no model provided
    public float[] vertices =
    {
        //Position          Texture coordinates
         0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
         0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
        -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
        -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left
    };
    //Indices is just here in case there is no model provided
    public int[] indices = {
        0, 1, 3,
        1, 2, 3
    };
    
    //Store the camera
    public Camera camera;

    //The FPS and deltatime variables
    public float FPS = 0;
    public float DeltaTime = 0;

    //A list of models to be rendered
    public List<Model> Models = new List<Model>();

    //the current scene to be rendered
    public Scene CurrentScene;

    //Whether the provided textures should be rendered using the nearest or linear filter
    public bool UseNearest = true;

    //Lighting is just a class providing a basic list of PointLights with some convinient functions
    public Lighting lighting = new Lighting();

    //A variable to choose between multiple pre made (fragment) shaders 
    public int ShaderID = 0;

    //The background color
    Vector3 BackgroundColor = new Vector3();


    //----------Variables----------


    //Reference the pre-made window class provided by OpenTK to create a window
    public Window(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { ClientSize = (width, height), Title = title }) 
    { 
        //Set the windowsize variable to calculate the camera FOV
        variables.WindowSize = new Vector2(width, height); 

        //Create a scene in case program.cs does not provide one
        CurrentScene = new Scene(this);

        //Initialize the camera
        camera = new Camera(new Vector3(-4, 0, 0), 60f, this);
    }
    
    public void ChangeScene(Scene NewScene)
    {
        //return if the new scene is the current scene
        if (NewScene == CurrentScene) return;
        
        
        //Unload the old scene
        CurrentScene.UnLoad();
        
        //Set the current scene, to the new scene
        CurrentScene = NewScene;
        
        //Load the new scene
        CurrentScene.Load();
    }
    
    //Runs every frame
    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        //Update the FPS variable based on DeltaTime
        FPS = 1f / (float)e.Time;

        //Update the windows DeltaTime variable using the FrameArguments passed in from OpenGL
        DeltaTime = (float)e.Time;

        //Update the current scene specified in the window class
        CurrentScene.Update();

        //Update the camera
        camera.Update();
        
        //Let the window do it's thing
        base.OnUpdateFrame(e);
    }

    //Also runs every frame but also renders to the screen
    protected override void OnRenderFrame(FrameEventArgs e)
    {
        //OpenGL stuff
        base.OnRenderFrame(e);

        //set the backround
        if (variables.ClearScreen) GL.Clear(ClearBufferMask.ColorBufferBit);

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
        //render the vertex data
        variables.shaders.Use();

        //Set the model matrix to account for transformations and rotations
        Providematrices(Object);

        //Set the current texture to the objects specified texture
        Object.mesh.texture.UseWithoutLoad(Object.mesh.TextureHandle);
        
        //Set the next object to be rendered 
        GL.BindVertexArray(Object.mesh.VAO);

        //draw the current object
        GL.DrawElements(
            PrimitiveType.Triangles,
            Object.mesh.IndexCount,
            DrawElementsType.UnsignedInt,
            0
        );
    }
    void Render()
    {
        //Reset the screen
        if (variables.ClearScreen) GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        //Create a list of models that have the IsTransparent flag enabled
        List<Model> TransparentModels = new List<Model>();
        
        //Loop through all the objects
        for (int i=0; i<Models.Count; i++)
        {
            //Check if the current object is enabled
            if (Models[i].Enabled)
            {
                
                //If the object is transparent, skip it and put it into the list we created,
                //otherwise render it
                if (!Models[i].IsTransparent) RenderObject(Models[i]);
                else
                {
                    //add the model to the list of transparent objects
                    TransparentModels.Add(Models[i]);
                }
            }
        }

        //loop trough all the transparent objects to be rendered after all the other props
        for (int i=0; i<TransparentModels.Count; i++)
        {   
            //render the objects in the list in reverse order
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
        GL.ClearColor(BackgroundColor.X, BackgroundColor.Y, BackgroundColor.Z, 1f);
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
        //Create the texture and apply it
        Texture texture = new Texture();
        texture.Use("./Engine/Debug/Missing_texture.png");
        
        //Set the texture to repeat
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

        //check if you want to use the nearest filter
        if (UseNearest) 
        {
            //Set both texture parameters to nearest
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Nearest);
        }
        else 
        {
            //Set both texture parameters to Linear
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }
    }

    void Providematrices(Model CurrentModel)
    {
        //Generate a new Model and view matrix for object transformation
        Matrix4 model = Matrix4.CreateScale(CurrentModel.scale) * Matrix4.CreateRotationX(MathHelper.DegreesToRadians(CurrentModel.rotation.X)) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(CurrentModel.rotation.Y)) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(CurrentModel.rotation.Z)) * Matrix4.CreateTranslation(CurrentModel.position);
        variables.view = camera.view;

        //send the matrices to the shaders
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
        if (ShaderID == 0) //Normal
        {
            variables.shaders = new Shader("./Engine/Shaders/Vertex.glsl", "./Engine/Shaders/Base-Fragment.glsl");
        }
        if (ShaderID == 1) //Toon shader
        {
            variables.shaders = new Shader("./Engine/Shaders/Vertex.glsl", "./Engine/Shaders/Toon-Fragment.glsl");
        }
        if (ShaderID == 2) //Normal but without lighting
        {
            variables.shaders = new Shader("./Engine/Shaders/Vertex.glsl", "./Engine/Shaders/NoLighting-Fragment.glsl");
        }
        if (ShaderID == 3) //Grayscale version
        {
            variables.shaders = new Shader("./Engine/Shaders/Vertex.glsl", "./Engine/Shaders/Grayscale-Fragment.glsl");
        }
        if (ShaderID == 4) //Inverted colors
        {
            variables.shaders = new Shader("./Engine/Shaders/Vertex.glsl", "./Engine/Shaders/Inverted-Fragment.glsl");
        }

        //create the vertex array object
        CreateVAO();

        //Create the EBO to use both indicies and verticies
        CreateEBO();

        //enable depth testing
        GL.Enable(EnableCap.DepthTest);

        //enable blending
        GL.Enable(EnableCap.Blend);

        //Enable alpha textures
        GL.BlendFunc(
            BlendingFactor.SrcAlpha,
            BlendingFactor.OneMinusSrcAlpha
        );

        //Create the matrices
        variables.model = Matrix4.CreateScale(1f);
        variables.view = camera.view;
        variables.projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(camera.FOV), variables.WindowSize.X / variables.WindowSize.Y, 0.1f, 100.0f);

        //Give the matrices to the shaders
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
