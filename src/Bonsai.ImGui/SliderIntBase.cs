using System.ComponentModel;

namespace Bonsai.ImGui;

/// <summary>
/// Provides an abstract base class for integer slider controls.
/// </summary>
public abstract class SliderIntBase<TResult> : ControlBuilderBase<TResult>
{
    /// <summary>
    /// Gets or sets the lower limit of values in the slider.
    /// </summary>
    [Description("The lower limit of values in the slider.")]
    public int Min { get; set; }

    /// <summary>
    /// Gets or sets the upper limit of values in the slider.
    /// </summary>
    [Description("The upper limit of values in the slider.")]
    public int Max { get; set; } = 100;
}
