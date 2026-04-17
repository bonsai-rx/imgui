using System.ComponentModel;

namespace Bonsai.ImGui;

/// <summary>
/// Provides an abstract base class for floating-point slider controls.
/// </summary>
public abstract class SliderFloatBase<TResult> : ControlBuilderBase<TResult>
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
}
