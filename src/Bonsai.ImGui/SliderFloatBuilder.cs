using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws a floating-point slider control and generates
/// a sequence of notifications whenever the current value changes.
/// </summary>
[Description("Draws a floating-point slider control and generates a sequence of notifications whenever the current value changes.")]
public class SliderFloatBuilder : SliderFloatBase<float>
{
    /// <summary>
    /// Gets or sets the initial value of the slider.
    /// </summary>
    [Description("The initial value of the slider.")]
    public float InitialValue { get; set; }

    /// <inheritdoc/>
    protected override IObservable<float> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<float>(observer =>
        {
            var min = Min;
            var max = Max;
            var value = InitialValue;
            var label = $"##{Name ?? nameof(ImGui.SliderFloat)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.SliderFloat(label, ref value, min, max))
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
