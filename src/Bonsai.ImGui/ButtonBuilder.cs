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
            var capturedName = Name;
            var capturedText = Text;
            var idSuffix = $"##{capturedName ?? nameof(ImGui.Button)}";
            var label = capturedText + idSuffix;
            var output = capturedName ?? capturedText;
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    var text = Text;
                    if (text != capturedText)
                    {
                        capturedText = text;
                        label = text + idSuffix;
                        output = capturedName ?? text;
                    }
                    if (Visible && ImGui.Button(label))
                        observer.OnNext(output);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
