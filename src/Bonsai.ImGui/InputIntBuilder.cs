using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws an integer input control and generates
/// a sequence of notifications whenever the current value changes.
/// </summary>
[Description("Draws an integer input control and generates a sequence of notifications whenever the current value changes.")]
public class InputIntBuilder : InputIntBase<int>
{
    /// <summary>
    /// Gets or sets the initial value of the input control.
    /// </summary>
    [Description("The initial value of the input control.")]
    public int InitialValue { get; set; }

    /// <summary>
    /// Gets or sets the step value used by the increment/decrement buttons.
    /// </summary>
    /// <remarks>
    /// A step value of zero hides the increment/decrement buttons.
    /// Stepping is mouse-driven only; there is no equivalent keyboard shortcut.
    /// </remarks>
    [Description("The step value used by the increment/decrement buttons.")]
    public int Step { get; set; } = 1;

    /// <summary>
    /// Gets or sets the accelerated step value applied when the step buttons are Ctrl-clicked.
    /// </summary>
    [Description("The accelerated step value applied when the step buttons are Ctrl-clicked.")]
    public int StepFast { get; set; } = 100;

    /// <inheritdoc/>
    protected override IObservable<int> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<int>(observer =>
        {
            var value = InitialValue;
            observer.OnNext(value);
            var label = $"##{Name ?? nameof(ImGui.InputInt)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.InputInt(label, ref value, Step, StepFast, Flags))
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
