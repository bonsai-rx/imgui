using Hexa.NET.ImGui;
using System.ComponentModel;

namespace Bonsai.ImGui;

/// <summary>
/// Provides an abstract base class for integer drag controls.
/// </summary>
public abstract class DragIntBase<TResult> : ControlBuilder<TResult>
{
    private string format;
    private ImGuiSliderFlags flags;

    /// <summary>
    /// Gets or sets the rate at which the value changes when dragging,
    /// in value units per pixel.
    /// </summary>
    [Description("The rate at which the value changes when dragging, in value units per pixel.")]
    public float Speed { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the lower limit of values in the drag control.
    /// </summary>
    /// <remarks>
    /// When <see cref="Min"/> and <see cref="Max"/> are equal, the drag is
    /// unbounded and the value can vary freely.
    /// </remarks>
    [Description("The lower limit of values in the drag control.")]
    public int Min { get; set; }

    /// <summary>
    /// Gets or sets the upper limit of values in the drag control.
    /// </summary>
    [Description("The upper limit of values in the drag control.")]
    public int Max { get; set; }

    /// <summary>
    /// Gets or sets the format string used to display the value.
    /// </summary>
    [Description("The format string used to display the value.")]
    public string Format
    {
        get => format;
        set => format = string.IsNullOrEmpty(value) ? null : value;
    }

    /// <summary>
    /// Gets or sets the configuration flags for the drag control.
    /// </summary>
    /// <exception cref="System.ArgumentException">
    /// Thrown when <paramref name="value"/> contains a flag that is not
    /// supported by drag controls.
    /// </exception>
    [TypeConverter(typeof(DragFlagsConverter))]
    [Description("The configuration flags for the drag control.")]
    public ImGuiSliderFlags Flags
    {
        get => flags;
        set
        {
            Validate.DragFlags(value);
            flags = value;
        }
    }
}
