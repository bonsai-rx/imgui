using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws a button control and generates
/// a sequence of notifications whenever the button is clicked.
/// </summary>
[Description("Draws a button control and generates a sequence of notifications whenever the button is clicked.")]
public class ButtonBuilder : TextControlBuilder<string>
{
    /// <inheritdoc/>
    protected override IObservable<string> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<string>(observer =>
        {
            var name = Name ?? Text;
            var label = $"{Text}##{Name ?? nameof(ImGui.Button)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.Button(label))
                        observer.OnNext(name);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
