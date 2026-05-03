using Hexa.NET.ImGui;
using System.ComponentModel;

namespace Bonsai.ImGui;

/// <summary>
/// Provides an abstract base class for color edit and color picker controls.
/// </summary>
public abstract class ColorControlBuilderBase<TResult> : ControlBuilder<TResult>
{
    private ImGuiColorEditFlags flags;

    /// <summary>
    /// Gets or sets the configuration flags for the color control.
    /// </summary>
    /// <exception cref="System.ArgumentException">
    /// Thrown when <paramref name="value"/> contains conflicting flags from
    /// the same mutually-exclusive category.
    /// </exception>
    [TypeConverter(typeof(ColorEditFlagsConverter))]
    [Description("The configuration flags for the color control.")]
    public ImGuiColorEditFlags Flags
    {
        get => flags;
        set
        {
            Validate.ColorEditFlags(value);
            flags = value;
        }
    }
}
