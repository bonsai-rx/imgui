using Bonsai;
using Hexa.NET.ImGui;
using System;
using System.ComponentModel;
using System.Numerics;
using System.Reactive;
using System.Reactive.Linq;

[Combinator]
[Description("Draws a progress bar showing a value between 0 and 1.")]
public class ProgressBar
{
    public ProgressBar()
    {
        Visible = true;
    }

    public bool Visible { get; set; }

    public float Fraction { get; set; }

    public string Overlay { get; set; }

    public IObservable<TSource> Process<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<TSource>(observer =>
        {
            var sourceObserver = Observer.Create<TSource>(
                value =>
                {
                    if (Visible)
                    {
                        ImGui.ProgressBar(Fraction, default(Vector2), Overlay);
                        observer.OnNext(value);
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
