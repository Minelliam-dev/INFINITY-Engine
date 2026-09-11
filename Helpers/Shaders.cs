using OpenTK.Graphics.GL;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

public class Shader
{
    int Handle;

    int FragmentShader;
    int VertexShader;

    private readonly Dictionary<string, int> _uniformLocations = new Dictionary<string, int>();

    private bool disposedValue = false;

    public Shader(string vertexPath, string fragmentPath, bool Empty=false)
    {
        
        if (Empty) return;

        //Get the shaders from the seperate files in the shaders directory
        string VertexShaderSource = File.ReadAllText(vertexPath);
        string FragmentShaderSource = File.ReadAllText(fragmentPath);


        //Create the shaders and set their contents to the shader sources
        
        //Generate the Vertex shader
        VertexShader = GL.CreateShader(ShaderType.VertexShader);
        //Set the shader source to the file provided trough vertexPath
        GL.ShaderSource(VertexShader, VertexShaderSource);

        //Generate the Fragment shader
        FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
        //Set the shader source to fragmentShaderSource generated from the file path
        GL.ShaderSource(FragmentShader, FragmentShaderSource);


        //Compile the shaders and check for errors
        GL.CompileShader(VertexShader);

        //Check for errors while compiling
        GL.GetShader(VertexShader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            //Output the error to the console if it failed
            string infoLog = GL.GetShaderInfoLog(VertexShader);
            Console.WriteLine(infoLog);
        }

        //do the same thing for the fragment shader
        GL.CompileShader(FragmentShader);

        GL.GetShader(FragmentShader, ShaderParameter.CompileStatus, out int success2);
        if (success2 == 0)
        {
            string infoLog = GL.GetShaderInfoLog(FragmentShader);
            Console.WriteLine(infoLog);
        }


        //Link the shaders to the GPU while checking for errors
        
        //Create the compute shader that will run on the GPU
        Handle = GL.CreateProgram();

        //Attach the shaders to the compute shader we created
        GL.AttachShader(Handle, VertexShader);
        GL.AttachShader(Handle, FragmentShader);

        //Link the compute shader to the GPU for execution
        GL.LinkProgram(Handle);

        //Check for errors
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success3);
        if (success3 == 0)
        {
            //Print the error to the console
            string infoLog = GL.GetProgramInfoLog(Handle);
            Console.WriteLine(infoLog);
        }


        //Detach and delete the unused copies of the shaders
        GL.DetachShader(Handle, VertexShader);
        GL.DetachShader(Handle, FragmentShader);
        GL.DeleteShader(FragmentShader);
        GL.DeleteShader(VertexShader);


        //initialize the uniform location variable
        _uniformLocations = new Dictionary<string, int>();

        //get the number of uniforms
        GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

        //
        for (var i = 0; i < numberOfUniforms; i++)
        {
            // get the name of this uniform,
            var key = GL.GetActiveUniform(Handle, i, out _, out _);

            // get the location,
            var location = GL.GetUniformLocation(Handle, key);
        
            // and then add it to the dictionary.
            _uniformLocations.Add(key, location);
        }

    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    //Delete the shader handle after program execution
    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            //delete the compute shader containing the vertex/fragment shaders
            GL.DeleteProgram(Handle);

            //set the disposedValue to true so that we don't delete the same shader twice
            disposedValue = true;
        }
    }

    ~Shader()
    {
        //If the shader handle has not been deleted, output a warning to the console
        if (disposedValue == false)
        {
            Console.WriteLine("GPU Resource leak! Did you forget to call Dispose()?");
        }
    }


    //The function to delete the shader handle from outside this class
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public int GetAttribLocation(string attribName)
    {
        return GL.GetAttribLocation(Handle, attribName);
    }

    public void SetMatrix4(string name, Matrix4 data)
    {
        GL.UseProgram(Handle);
        GL.UniformMatrix4(_uniformLocations[name], true, ref data);
    }

    public void SetInt(string name, int value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        GL.Uniform1(location, value);
    }
    
    public void SetFloat(string name, float value)
    {
        int location = GL.GetUniformLocation(Handle, name);
        GL.Uniform1(location, value);
    }
    
    public void SetVector3(string name, Vector3 value)
    {
        int location = GL.GetUniformLocation(Handle, name);
    
        GL.Uniform3(
            location,
            value.X,
            value.Y,
            value.Z
        );
    }
}