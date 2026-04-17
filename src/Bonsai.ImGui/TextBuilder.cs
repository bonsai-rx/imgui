using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws a text label and generates
/// a sequence of notifications with the label text.
/// </summary>
[Description("Draws a text label and generates a sequence of notifications with the label text.")]
public class TextBuilder : TextControlBuilderBase<string>
{
    /// <inheritdoc/>
    protected override IObservable<string> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<string>(observer =>
        {
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    var text = Text;
                    if (Visible)
                    {
                        ImGui.Text(text);
                        observer.OnNext(text);
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
