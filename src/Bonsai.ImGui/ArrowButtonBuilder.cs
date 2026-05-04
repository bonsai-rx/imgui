using Hexa.NET.ImGui;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws a directional arrow button and generates
/// a sequence of notifications whenever the button is clicked.
/// </summary>
[Description("Draws a directional arrow button and generates a sequence of notifications whenever the button is clicked.")]
public class ArrowButtonBuilder : ControlBuilder<string>
{
    private ImGuiDir direction = ImGuiDir.Right;

    /// <summary>
    /// Gets or sets the direction of the arrow displayed on the button.
    /// </summary>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="value"/> is not a cardinal direction.
    /// </exception>
    [TypeConverter(typeof(ArrowButtonDirectionConverter))]
    [Description("The direction of the arrow displayed on the button.")]
    public ImGuiDir Direction
    {
        get => direction;
        set
        {
            Validate.ArrowButtonDirection(value);
            direction = value;
        }
    }

    /// <inheritdoc/>
    protected override IObservable<string> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<string>(observer =>
        {
            var capturedName = Name;
            var label = $"##{capturedName ?? nameof(ImGui.ArrowButton)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    var direction = Direction;
                    if (Visible && ImGui.ArrowButton(label, direction))
                        observer.OnNext(capturedName ?? direction.ToString());
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
