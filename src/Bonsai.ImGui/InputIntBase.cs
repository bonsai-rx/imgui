using Hexa.NET.ImGui;
using System.ComponentModel;

namespace Bonsai.ImGui;

/// <summary>
/// Provides an abstract base class for integer input controls.
/// </summary>
public abstract class InputIntBase<TResult> : ControlBuilder<TResult>
{
    private ImGuiInputTextFlags flags;

    /// <summary>
    /// Gets or sets the configuration flags for the input control.
    /// </summary>
    /// <exception cref="System.ArgumentException">
    /// Thrown when <paramref name="value"/> contains a flag that is not
    /// supported by numeric input controls.
    /// </exception>
    [TypeConverter(typeof(NumericInputFlagsConverter))]
    [Description("The configuration flags for the input control.")]
    public ImGuiInputTextFlags Flags
    {
        get => flags;
        set
        {
            Validate.NumericInputFlags(value);
            flags = value;
        }
    }
}
