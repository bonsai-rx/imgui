using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws an integer slider control and generates
/// a sequence of notifications whenever the current value changes.
/// </summary>
[Description("Draws an integer slider control and generates a sequence of notifications whenever the current value changes.")]
public class SliderIntBuilder : SliderIntBase<int>
{
    /// <summary>
    /// Gets or sets the initial value of the slider.
    /// </summary>
    [Description("The initial value of the slider.")]
    public int InitialValue { get; set; }

    /// <inheritdoc/>
    protected override IObservable<int> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<int>(observer =>
        {
            var min = Min;
            var max = Max;
            var value = InitialValue;
            observer.OnNext(value);
            var label = $"##{Name ?? nameof(ImGui.SliderInt)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.SliderInt(label, ref value, min, max))
                        observer.OnNext(value);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
