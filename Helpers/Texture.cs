using OpenTK.Graphics.OpenGL4;
using StbImageSharp;

//A class that handles texture loading and allocating
public class Texture
{
    //Texture handle
    public int Handle;

    //Image data
    ImageResult ResultOfLoading = new ImageResult();

    //A bool deciding if the image should be flipped vertically
    public bool MirrorY = false;


    //i could have choosen a better name but i did not want to change it
    public void StartImageStuff()
    {
        //Generate the base texture
        Handle = GL.GenTexture();
    }
    
    public ImageResult Load(string Path)
    {
        //if MirrorY is true, mirror the image on the Y axis
        if (MirrorY) StbImage.stbi_set_flip_vertically_on_load(1);

        //Load the image data
        ImageResult image = ImageResult.FromStream(File.OpenRead(Path), ColorComponents.RedGreenBlueAlpha);

        //store the loaded image for later
        ResultOfLoading = image;

        //return the image data
        return image;
    }

    public void Use(string Path)
    {
        //Load the image
        ImageResult image = Load(Path);
        
        
        //Select the texture slot 0
        GL.ActiveTexture(TextureUnit.Texture0);

        //Bind the texture
        GL.BindTexture(TextureTarget.Texture2D, Handle);

        //Upload the image data to the selected texture slot
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
    }

    //This function exists so that the render loop does not load the image thousands of times per second
    public void UseWithoutLoad(int handle)
    {
        //Bind the texture
        GL.BindTexture(TextureTarget.Texture2D, handle);

        //Set the image data
        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, ResultOfLoading.Width, ResultOfLoading.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, ResultOfLoading.Data);
    }
}