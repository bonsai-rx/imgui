using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImGui;
using ImGui = Hexa.NET.ImGui.ImGui;

/// <summary>
/// Represents an operator that draws an input text box and generates
/// a sequence of notifications whenever the text changes.
/// </summary>
[Description("Draws an input text box and generates a sequence of notifications whenever the text changes.")]
public class InputTextBuilder : TextControlBuilder<string>
{
    /// <summary>
    /// Gets or sets the maximum number of characters allowed in the text box.
    /// </summary>
    [Description("The maximum number of characters allowed in the text box.")]
    public ulong Capacity { get; set; } = 1024;

    /// <inheritdoc/>
    protected override IObservable<string> Generate<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<string>(observer =>
        {
            var buf = Text ?? string.Empty;
            observer.OnNext(buf);
            var bufSize = (nuint)Capacity;
            var label = $"##{Name ?? nameof(ImGui.InputText)}";
            var sourceObserver = Observer.Create<TSource>(
                _ =>
                {
                    if (Visible && ImGui.InputText(label, ref buf, bufSize))
                        observer.OnNext(buf);
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
