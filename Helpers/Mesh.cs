using OpenTK.Graphics.OpenGL;

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