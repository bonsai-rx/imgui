using Hexa.NET.ImGui;
using System;
using System.ComponentModel;

namespace Bonsai.ImGui;

/// <summary>
/// Provides an abstract base class for floating-point slider controls.
/// </summary>
public abstract class SliderFloatBase<TResult> : ControlBuilder<TResult>
{
    private string format;
    private ImGuiSliderFlags flags;

    /// <summary>
    /// Gets or sets the lower limit of values in the slider.
    /// </summary>
    [Description("The lower limit of values in the slider.")]
    public float Min { get; set; }

    /// <summary>
    /// Gets or sets the upper limit of values in the slider.
    /// </summary>
    [Description("The upper limit of values in the slider.")]
    public float Max { get; set; } = 1;

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
    /// Gets or sets the configuration flags for the slider control.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="value"/> contains a flag that is not
    /// supported by slider controls.
    /// </exception>
    [TypeConverter(typeof(SliderFlagsConverter))]
    [Description("The configuration flags for the slider control.")]
    public ImGuiSliderFlags Flags
    {
        get => flags;
        set
        {
            Validate.SliderFlags(value);
            flags = value;
        }
    }
}
