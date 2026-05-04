using Hexa.NET.ImGui;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws an angle slider control and generates
/// a sequence of notifications whenever the current value changes.
/// </summary>
/// <remarks>
/// The emitted value is in radians, while the slider bounds and display
/// format are expressed in degrees for ergonomics.
/// </remarks>
[Description("Draws an angle slider control and generates a sequence of notifications whenever the current value changes.")]
public class SliderAngleBuilder : ControlBuilder<float>
{
    private string format;
    private ImGuiSliderFlags flags;

    /// <summary>
    /// Gets or sets the initial value of the slider, in radians.
    /// </summary>
    [Description("The initial value of the slider, in radians.")]
    public float InitialValue { get; set; }

    /// <summary>
    /// Gets or sets the lower limit of values, in degrees.
    /// </summary>
    [Description("The lower limit of values, in degrees.")]
    public float MinDegrees { get; set; } = -360.0f;

    /// <summary>
    /// Gets or sets the upper limit of values, in degrees.
    /// </summary>
    [Description("The upper limit of values, in degrees.")]
    public float MaxDegrees { get; set; } = 360.0f;

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

    /// <inheritdoc/>
    protected override IObservable<float> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<float>(observer =>
        {
            var value = InitialValue;
            observer.OnNext(value);
            var label = $"##{Name ?? nameof(ImGui.SliderAngle)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.SliderAngle(label, ref value, MinDegrees, MaxDegrees, Format, Flags))
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
