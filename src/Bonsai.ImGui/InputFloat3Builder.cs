using System;
using System.ComponentModel;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws a group of three floating-point input controls
/// and generates a sequence of notifications whenever the current value changes.
/// </summary>
[Description("Draws a group of three floating-point input controls and generates a sequence of notifications whenever the current value changes.")]
public class InputFloat3Builder : InputFloatBase<Vector3>
{
    /// <summary>
    /// Gets or sets the initial value of the input control.
    /// </summary>
    [Description("The initial value of the input control.")]
    [TypeConverter(typeof(NumericRecordConverter))]
    public Vector3 InitialValue { get; set; }

    /// <inheritdoc/>
    protected override IObservable<Vector3> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<Vector3>(observer =>
        {
            var value = InitialValue;
            observer.OnNext(value);
            var label = $"##{Name ?? nameof(ImGui.InputFloat3)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.InputFloat3(label, ref value, Format, Flags))
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
