using Hexa.NET.ImGui;
using System.ComponentModel;

namespace Bonsai.ImGui;

/// <summary>
/// Provides an abstract base class for floating-point input controls.
/// </summary>
public abstract class InputFloatBase<TResult> : ControlBuilder<TResult>
{
    private string format;
    private ImGuiInputTextFlags flags;

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
