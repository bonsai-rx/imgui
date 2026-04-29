using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Bonsai.ImPlot;

/// <summary>
/// Provides an abstract base class for common plot element configuration.
/// </summary>
[Combinator]
[DefaultProperty(nameof(Name))]
public abstract class PlotCombinator
{
    /// <summary>
    /// Gets or sets the name of the plot element.
    /// </summary>
    [Category(nameof(CategoryAttribute.Design))]
    [Description("The name of the plot element.")]
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets a value specifying whether the plot element is displayed.
    /// </summary>
    [Category(nameof(CategoryAttribute.Behavior))]
    [Description("Specifies whether the plot element is displayed.")]
    public bool Visible { get; set; } = true;

    internal IObservable<TSource> Process<TSource>(IObservable<TSource> source, Action<string, TSource> plot)
    {
        return Observable.Create<TSource>(observer =>
        {
            var label = Name ?? string.Empty;
            var sourceObserver = Observer.Create<TSource>(
                value =>
                {
                    if (Visible)
                    {
                        plot(label, value);
                        observer.OnNext(value);
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
