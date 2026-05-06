using Bonsai;
using Hexa.NET.ImGui;
using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;

[Combinator]
[Description("Draws a group of radio buttons and emits the selected index when it changes.")]
public class RadioButtonGroup
{
    public RadioButtonGroup()
    {
        Visible = true;
        Items = new string[0];
    }

    public string Name { get; set; }

    public bool Visible { get; set; }

    public string[] Items { get; set; }

    public int SelectedIndex { get; set; }

    public IObservable<int> Process<TSource>(IObservable<TSource> source)
    {
        return Observable.Create<int>(observer =>
        {
            var idSuffix = "##" + (Name ?? "RadioButtonGroup");
            var sourceObserver = Observer.Create<TSource>(
                value =>
                {
                    if (!Visible) return;
                    var items = Items;
                    var selected = SelectedIndex;
                    for (var i = 0; i < items.Length; i++)
                    {
                        if (ImGui.RadioButton(items[i] + idSuffix, ref selected, i))
                        {
                            SelectedIndex = selected;
                            observer.OnNext(selected);
                        }
                    }
                },
                observer.OnError,
                observer.OnCompleted);
            return source.SubscribeSafe(sourceObserver);
        });
    }
}
