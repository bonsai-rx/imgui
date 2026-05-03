using System;
using System.ComponentModel;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws a group of three floating-point drag controls
/// and generates a sequence of notifications whenever the current value changes.
/// </summary>
[Description("Draws a group of three floating-point drag controls and generates a sequence of notifications whenever the current value changes.")]
public class DragFloat3Builder : DragFloatBase<Vector3>
{
    /// <summary>
    /// Gets or sets the initial value of the drag control.
    /// </summary>
    [Description("The initial value of the drag control.")]
    [TypeConverter(typeof(NumericRecordConverter))]
    public Vector3 InitialValue { get; set; }

    /// <inheritdoc/>
    protected override IObservable<Vector3> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<Vector3>(observer =>
        {
            var value = InitialValue;
            observer.OnNext(value);
            var label = $"##{Name ?? nameof(ImGui.DragFloat3)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.DragFloat3(label, ref value, Speed, Min, Max, Format, Flags))
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
