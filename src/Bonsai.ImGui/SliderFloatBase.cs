using Hexa.NET.ImGui;
using System.ComponentModel;

namespace Bonsai.ImGui;

/// <summary>
/// Provides an abstract base class for floating-point slider controls.
/// </summary>
public abstract class SliderFloatBase<TResult> : ControlBuilder<TResult>
{
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
    public string Format { get; set; }

    /// <summary>
    /// Gets or sets the configuration flags for the slider control.
    /// </summary>
    [Description("The configuration flags for the slider control.")]
    public ImGuiSliderFlags Flags { get; set; }
}
