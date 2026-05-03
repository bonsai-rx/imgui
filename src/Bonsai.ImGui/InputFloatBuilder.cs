using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws a floating-point input control and generates
/// a sequence of notifications whenever the current value changes.
/// </summary>
[Description("Draws a floating-point input control and generates a sequence of notifications whenever the current value changes.")]
public class InputFloatBuilder : InputFloatBase<float>
{
    /// <summary>
    /// Gets or sets the initial value of the input control.
    /// </summary>
    [Description("The initial value of the input control.")]
    public float InitialValue { get; set; }

    /// <summary>
    /// Gets or sets the step value used by the increment/decrement buttons.
    /// </summary>
    /// <remarks>
    /// A step value of zero hides the increment/decrement buttons.
    /// Stepping is mouse-driven only; there is no equivalent keyboard shortcut.
    /// </remarks>
    [Description("The step value used by the increment/decrement buttons.")]
    public float Step { get; set; }

    /// <summary>
    /// Gets or sets the accelerated step value applied when the step buttons are Ctrl-clicked.
    /// </summary>
    [Description("The accelerated step value applied when the step buttons are Ctrl-clicked.")]
    public float StepFast { get; set; }

    /// <inheritdoc/>
    protected override IObservable<float> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<float>(observer =>
        {
            var value = InitialValue;
            observer.OnNext(value);
            var label = $"##{Name ?? nameof(ImGui.InputFloat)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.InputFloat(label, ref value, Step, StepFast, Format, Flags))
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
