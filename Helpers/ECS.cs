using OpenTK.Mathematics;

namespace ECS
{
    public class Model
    {
        public bool Enabled = false;
        
        public Vector3 position;
        public Vector3 rotation = new Vector3(0, 0, 0);
        public float scale = 1f;
        public string path;

        public string texturePath;
        public Texture texture;

        public float[] Vertices = [];
        public int[] Indices = [];

        public int TextureHandle;

        public bool IsTransparent;

        public Model(string Path, Vector3 Position, Scene ParentScene, string TexturePath, bool Transparent=false)
        {   
            position = Position;
            path = Path;

            IsTransparent = Transparent;

            texturePath = TexturePath;

            texture = new Texture();

            //texture.StartImageStuff();
            texture.Load(TexturePath);
            TextureHandle = texture.Handle;

            OBJLoader model = new OBJLoader(Path);

            model.GetModel(out Vertices, out Indices);
        
            ParentScene.Models.Add(this);
        }
    }

    public class Scene(Window window)
    {
        public Window window = window;
        public List<Model> Models = new List<Model>();
        public List<PointLight> Lights = new List<PointLight>();

        public Action UpdateFunction = Debug.None;
        public Action StartFunction = Debug.None;

        bool FirstIteration = true;

        public void Load()
        {
            for (int i=0; i<Models.Count; i++)
            {
                Models[i].Enabled = true;
                window.Models.Add(Models[i]);
            }

            for (int i=0; i<Lights.Count; i++)
            {
                window.lighting.Add(Lights[i].Position, Lights[i].Radius);
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