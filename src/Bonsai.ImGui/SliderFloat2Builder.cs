using System;
using System.ComponentModel;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws a group of two floating-point sliders and
/// generates a sequence of notifications whenever the current value changes.
/// </summary>
[Description("Draws a group of two floating-point sliders and generates a sequence of notifications whenever the current value changes.")]
public class SliderFloat2Builder : SliderFloatBase<Vector2>
{
    /// <summary>
    /// Gets or sets the initial value of the slider.
    /// </summary>
    [Description("The initial value of the slider.")]
    [TypeConverter(typeof(NumericRecordConverter))]
    public Vector2 InitialValue { get; set; }

    /// <inheritdoc/>
    protected override IObservable<Vector2> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<Vector2>(observer =>
        {
            var min = Min;
            var max = Max;
            var value = InitialValue;
            var label = $"##{Name ?? nameof(ImGui.SliderFloat2)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.SliderFloat2(label, ref value, min, max))
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
