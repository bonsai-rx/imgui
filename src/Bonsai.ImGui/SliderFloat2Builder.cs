using OpenTK;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;
using SNVector2 = System.Numerics.Vector2;

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
            var initialValue = InitialValue;
            var value = new SNVector2(initialValue.X, initialValue.Y);
            var label = $"##{Name ?? nameof(ImGui.SliderFloat2)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.SliderFloat2(label, ref value, min, max))
                        observer.OnNext(new(value.X, value.Y));
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
