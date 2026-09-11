using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

public class Texture
{
    public int Handle;

    ImageResult ResultOfLoading = new ImageResult();


    public void StartImageStuff()
    {
        Handle = GL.GenTexture();   
    }
    public ImageResult Load(string Path)
    {
        //StbImage.stbi_set_flip_vertically_on_load(0);

        ImageResult image = ImageResult.FromStream(File.OpenRead(Path), ColorComponents.RedGreenBlueAlpha);

        ResultOfLoading = image;

        return image;
    }

    public void Use(string Path)
    {
        ImageResult image = Load(Path);
        

        GL.ActiveTexture(TextureUnit.Texture0);

        GL.BindTexture(TextureTarget.Texture2D, Handle);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
    }

    public void UseWithoutLoad()
    {
        GL.ActiveTexture(TextureUnit.Texture0);

        GL.BindTexture(TextureTarget.Texture2D, Handle);

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ResultOfLoading.Width, ResultOfLoading.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ResultOfLoading.Data);
    }
}