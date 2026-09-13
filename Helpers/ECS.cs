using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace ECS
{
    public class Mesh
    {
        public int VBO;
        public int EBO;
        public int VAO;
        public int IndexCount;

        public float[] Vertices = [];
        public int[] Indices = [];

        public Texture texture = new Texture();
        public int TextureHandle;

        public string path;
        public string texturePath;

        public void LoadMesh(float[] vertices, int[] indices)
        {
            //texture.StartImageStuff();
            texture.Load(texturePath);
            TextureHandle = texture.Handle;

            Vertices = vertices;
            Indices = indices;

            IndexCount = Indices.Length;

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                Vertices.Length * sizeof(float),
                Vertices,
                BufferUsageHint.StaticDraw
            );

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                Indices.Length * sizeof(int),
                Indices,
                BufferUsageHint.StaticDraw
            );



            GL.VertexAttribPointer(
                0,
                3,
                VertexAttribPointerType.Float,
                false,
                5 * sizeof(float),
                0
            );

            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(
                1,
                2,
                VertexAttribPointerType.Float,
                false,
                5 * sizeof(float),
                3 * sizeof(float)
            );

            GL.EnableVertexAttribArray(1);
        }
        
        public Mesh(string FilePath, string TexturePath)
        {
            path = FilePath;

            texturePath = TexturePath;

            if (FilePath == "§") return;

            //texture.StartImageStuff();
            texture.Load(TexturePath);
            TextureHandle = texture.Handle;

            OBJLoader model = new OBJLoader(FilePath);

            model.GetModel(out Vertices, out Indices);

            IndexCount = Indices.Length;

            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(
                BufferTarget.ArrayBuffer,
                Vertices.Length * sizeof(float),
                Vertices,
                BufferUsageHint.StaticDraw
            );

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(
                BufferTarget.ElementArrayBuffer,
                Indices.Length * sizeof(int),
                Indices,
                BufferUsageHint.StaticDraw
            );



            GL.VertexAttribPointer(
                0,
                3,
                VertexAttribPointerType.Float,
                false,
                5 * sizeof(float),
                0
            );

            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(
                1,
                2,
                VertexAttribPointerType.Float,
                false,
                5 * sizeof(float),
                3 * sizeof(float)
            );

            GL.EnableVertexAttribArray(1);
        }
    }

    public class Model
    {
        public bool Enabled = false;

        public Shader shaders;
        
        public Vector3 position;
        public Vector3 rotation = new Vector3(0, 0, 0);
        public float scale = 1f;

        public Scene parentScene;
        public bool IsTransparent;

        public bool IsStatic;

        public Mesh mesh;
        
        public Model(Mesh Mesh, Vector3 Position, Scene ParentScene, Shader shader, bool Transparent=false, bool Static=false)
        {   
            position = Position;
            IsStatic = Static;
            parentScene = ParentScene;

            shaders = shader;

            IsTransparent = Transparent;

            mesh = Mesh;
        
            ParentScene.Models.Add(this);
        }
    }

    public class Scene(Window window)
    {
        public Window window = window;
        public List<Model> Models = new List<Model>();
        public List<PointLight> Lights = new List<PointLight>();

        Shader BaseShader = new Shader("./Engine/Shaders/Vertex.glsl", "./Engine/Shaders/Base-Fragment.glsl");

        public Action UpdateFunction = Debug.None;
        public Action StartFunction = Debug.None;

        bool FirstIteration = true;
        public bool CombineStaticObjects = true;

        public void Load()
        {
            List<Model> models = new List<Model>();
            
            for (int i=0; i<Models.Count; i++)
            {
                
                if (!CombineStaticObjects)
                {
                    Models[i].Enabled = true;
                    window.Models.Add(Models[i]);
                }
                else
                {
                    if (Models[i].IsStatic)
                    {
                        models.Add(Models[i]);
                        continue;
                    }
                    else
                    {
                        Models[i].Enabled = true;
                        window.Models.Add(Models[i]);
                    }
                }
            }

            if (CombineStaticObjects && models.Count > 0)
            {
                List<float> vertices = new();
                List<int> indices = new();

                foreach (Model model in models)
                {
                    int vertexOffset = vertices.Count / 5;

                    // Add transformed vertices
                    for (int v = 0; v < model.mesh.Vertices.Length; v += 5)
                    {
                        Vector3 vertex = new Vector3(
                            model.mesh.Vertices[v],
                            model.mesh.Vertices[v + 1],
                            model.mesh.Vertices[v + 2]
                        );

                        vertex *= model.scale;

                        Matrix3 rotation =
                            Matrix3.CreateRotationX(model.rotation.X) *
                            Matrix3.CreateRotationY(model.rotation.Y) *
                            Matrix3.CreateRotationZ(model.rotation.Z);

                        vertex = rotation * vertex;

                        vertex += model.position;

                        vertices.Add(vertex.X);
                        vertices.Add(vertex.Y);
                        vertices.Add(vertex.Z);

                        // UV
                        vertices.Add(model.mesh.Vertices[v + 3]);
                        vertices.Add(model.mesh.Vertices[v + 4]);
                    }

                    // Add corrected indices
                    for (int n = 0; n < model.mesh.Indices.Length; n++)
                    {
                        indices.Add(model.mesh.Indices[n] + vertexOffset);
                    }
                }

                Mesh OutMesh = new Mesh("§", models[0].mesh.texturePath);

                

                Model outModel = new Model(
                    OutMesh,
                    Vector3.Zero,
                    this,
                    Models[0].shaders
                );

                OutMesh.LoadMesh(
                    vertices.ToArray(),
                    indices.ToArray()
                );

                outModel.Enabled = true;

                window.Models.Add(outModel);
            }

            for (int i=0; i<Lights.Count; i++)
            {
                window.lighting.Add(Lights[i].position, Lights[i].radius, Lights[i].color);
            }
        }

        public void UnLoad()
        {
            for (int i=0; i<Models.Count; i++)
            {
                Models[i].Enabled = false;

                window.Models.Remove(Models[i]);
            }

            for (int i=0; i<Lights.Count; i++)
            {
                window.lighting = new Lighting();
            }
        }

        public void Update()
        {
            if (UpdateFunction != Debug.None && !FirstIteration) UpdateFunction();

            if (FirstIteration && StartFunction != Debug.None) StartFunction();

            FirstIteration = false;
        }
    }
}