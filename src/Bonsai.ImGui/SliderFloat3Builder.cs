using System;
using System.ComponentModel;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws a group of three floating-point sliders and
/// generates a sequence of notifications whenever the current value changes.
/// </summary>
[Description("Draws a group of three floating-point sliders and generates a sequence of notifications whenever the current value changes.")]
public class SliderFloat3Builder : SliderFloatBase<Vector3>
{
    /// <summary>
    /// Gets or sets the initial value of the slider.
    /// </summary>
    [Description("The initial value of the slider.")]
    [TypeConverter(typeof(NumericRecordConverter))]
    public Vector3 InitialValue { get; set; }

    /// <inheritdoc/>
    protected override IObservable<Vector3> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<Vector3>(observer =>
        {
            var min = Min;
            var max = Max;
            var value = InitialValue;
            observer.OnNext(value);
            var label = $"##{Name ?? nameof(ImGui.SliderFloat3)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.SliderFloat3(label, ref value, min, max))
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
