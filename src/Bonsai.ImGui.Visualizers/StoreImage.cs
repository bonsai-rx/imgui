using Hexa.NET.ImGui;
using OpenCV.Net;
using OpenTK.Graphics.OpenGL4;
using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace Bonsai.ImGui.Visualizers;

/// <summary>
/// Represents an operator that writes each image in the sequence to a texture object
/// and returns the corresponding texture reference.
/// </summary>
[Description("Writes each image in the sequence to a texture object and returns the corresponding texture reference.")]
public class StoreImage : Combinator<IplImage, ImTextureRef>
{
    /// <summary>
    /// Gets or sets a value specifying the internal pixel format of the texture.
    /// </summary>
    [Category("TextureParameter")]
    [Description("Specifies the internal pixel format of the texture.")]
    public PixelInternalFormat InternalFormat { get; set; } = PixelInternalFormat.Rgba;

    /// <summary>
    /// Gets or sets a value specifying wrapping parameters for the column
    /// coordinates of the texture sampler.
    /// </summary>
    [Category("TextureParameter")]
    [Description("Specifies wrapping parameters for the column coordinates of the texture sampler.")]
    public TextureWrapMode WrapS { get; set; } = TextureWrapMode.Repeat;

    /// <summary>
    /// Gets or sets a value specifying wrapping parameters for the row
    /// coordinates of the texture sampler.
    /// </summary>
    [Category("TextureParameter")]
    [Description("Specifies wrapping parameters for the row coordinates of the texture sampler.")]
    public TextureWrapMode WrapT { get; set; } = TextureWrapMode.Repeat;

    /// <summary>
    /// Gets or sets a value specifying the texture minification filter.
    /// </summary>
    [Category("TextureParameter")]
    [Description("Specifies the texture minification filter.")]
    public TextureMinFilter MinFilter { get; set; } = TextureMinFilter.Linear;

    /// <summary>
    /// Gets or sets a value specifying the texture magnification filter.
    /// </summary>
    [Category("TextureParameter")]
    [Description("Specifies the texture magnification filter.")]
    public TextureMagFilter MagFilter { get; set; } = TextureMagFilter.Linear;

    /// <inheritdoc/>
    public unsafe override IObservable<ImTextureRef> Process(IObservable<IplImage> source)
    {
        return Observable.Defer(() =>
        {
            int textureId = default;
            ImTextureID texId = default;
            ImTextureRef texRef = default;
            return source.Select(image =>
            {
                if (image is not null)
                {
                    if (textureId == 0)
                    {
                        GL.GenTextures(1, out textureId);
                        texId = new ImTextureID(textureId);
                        texRef = new ImTextureRef(texId: texId);
                        GL.BindTexture(TextureTarget.Texture2D, textureId);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)WrapS);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)WrapT);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)MinFilter);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)MagFilter);
                    }

                    GL.BindTexture(TextureTarget.Texture2D, textureId);
                    TextureHelper.UpdateTexture(TextureTarget.Texture2D, InternalFormat, image);
                }
                else
                {
                    if (textureId > 0)
                        GL.DeleteTextures(1, ref textureId);
                    textureId = default;
                    texId = default;
                    texRef = default;
                }

                return texRef;
            }).Finally(() =>
            {
                if (textureId > 0)
                    GL.DeleteTextures(1, ref textureId);
            });
        });
    }
}
