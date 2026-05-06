---
uid: custom-controls
---

# Custom controls

`Bonsai.ImGui` includes operators for the most common Dear ImGui widgets, but ImGui exposes many more. Local scripting extensions can be used to access any ImGui call and build custom interface operators into your workflow, without having to author a separate package. This article walks through the anatomy of a widget operator and applies it to two examples: a display-only progress bar and an interactive radio button group.

This article extends the [Scripting Extensions](xref:scripting-extensions) documentation. If you have not used scripting extensions before, follow that guide first to set up the `Extensions` folder and the recommended C# development tools.

## Anatomy of a widget operator

Every widget operator included with `Bonsai.ImGui` follows a common structure. The operator class exposes a set of properties, defines a `Process` method that returns an [observable](xref:observables), and inside that method submits ImGui calls every frame.

The input to a widget operator is a sequence of frame notifications. The package publishes a `Frame` subject from the top-level ImGui visualizer, and each widget subscribes to it. Each notification on that source corresponds to one ImGui frame: the operator reads its dynamic properties, calls the matching `ImGui.X(...)` function, and emits any events the widget produces.

The split between properties read once at subscribe and properties read every frame matches the [Property lifetime](xref:property-lifetime) categories. As a rule of thumb, anything that contributes to the widget identity is captured at subscribe; anything else is read inside the per-frame handler.

## Set up the project

The script needs access to the ImGui API using [Hexa.NET.ImGui](https://github.com/HexaEngine/Hexa.NET.ImGui.git). Add the following package reference to your `Extensions.csproj` so the script can resolve references to ImGui types:

```xml
<PackageReference Include="Hexa.NET.ImGui" Version="2.2.9" />
```

Make sure the version included in the package reference matches the version used by `Bonsai.ImGui`.

## Example 1: A progress bar

ImGui includes a progress bar widget that is not yet included with `Bonsai.ImGui`. In this example we will show how to wrap this widget into a custom operator. This simple example is useful as this widget is display-only and therefore has no interactions to handle and no widget identity to manage.

Add a new `CSharpCombinator` operator to your workflow and name it `ProgressBar` when prompted. Replace the script body with:

```csharp
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
```

A few points to highlight:

- `Process<TSource>` accepts a render tick stream of any element type and emits each tick unchanged while the operator is `Visible`. The output is gated on visibility so that downstream operators do not run when the bar is not on screen, matching the behavior of other display widgets in the package.
- `Fraction` and `Overlay` are read inside the handler, so changes apply on the next frame.
- There is no `Name` property because `ImGui.ProgressBar` has no associated widget id.

The following workflow uses a [`SliderFloat`] to set `Fraction`:

:::workflow
![A slider sets the fraction of the progress bar](../workflows/custom-controls-progress-bar.bonsai)
:::

Both operators subscribe to the `Frame` subject so they are drawn each frame. A [`PropertyMapping`] writes the slider value into `ProgressBar.Fraction` whenever it changes.

## Example 2: A radio button group

The [`RadioButton`] operator represents a single button. Forming a group from instances of [`RadioButton`] requires keeping a shared selection in sync across them using a subject. When the group is fixed and lives next to a single piece of state, it is often simpler to draw the whole group from one operator. The same strategy is used by [`Combo`] and [`ListBox`].

Add a new `CSharpSource` operator and name it `RadioButtonGroup`:

```csharp
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
```

The implementation shows the binding model used by interactive widgets in this package. `SelectedIndex` is the widget state property: it can be set externally through a property mapping or internally by user interaction. On each frame the operator:

1. Reads `SelectedIndex` into a local.
2. Passes the local by `ref` to every `ImGui.RadioButton` call, so ImGui can mutate it when a button is clicked.
3. If a click is registered, writes the local back to `SelectedIndex` and emits the new index.

The `ref` parameter is the hinge of this pattern. The same property carries external state into the widget through the local at step 1, and carries internal state back out through the assignment and emission at step 3. The slider, checkbox, and radio button operators follow the same pattern.

One other detail worth noting: `idSuffix` is built once, when the source is subscribed, using the captured `Name`. This is the identity portion of each radio label and should not change between frames. The per-frame item text is appended to it, producing labels like `Option A##Choices`. The [Property lifetime](xref:property-lifetime) article covers this distinction in detail.

:::workflow
![A radio button group with three options](../workflows/custom-controls-radio-button-group.bonsai)
:::

## What this example leaves out

To keep the script small a few conventions used elsewhere in the package are skipped:

- Operators in the package normalize an empty `Name` to `null` so that fallback patterns like `Name ?? "default"` behave the same whether the user clears the property grid field or never sets it.
- Most operators validate property values that would otherwise trigger an ImGui assertion. See [Error handling](xref:error-handling) for the broader strategy.
- An `InitialValue` property would let the operator publish the starting selection on subscribe.

The [Property lifetime](xref:property-lifetime) and [Error handling](xref:error-handling) articles cover these design choices in detail.

## When to script and when to compose

Scripted operators are the right tool when:

- The ImGui call you need is not included in the package.
- A coordination pattern would otherwise require explicit connections for every instance, for example multiple subjects, property mappings, and multicasts.
- The operator is project-specific and unlikely to be reused.

Composing existing operators in the workflow is preferable when:

- The pattern is genuinely a graph of independent widgets that share data, best expressed as a subject.
- The configuration changes often and benefits from the property grid on each instance.

If a scripted operator becomes useful across several projects, consider [creating a package](xref:create-package) so others can reference it from the package manager.

<!-- Reference Style Links -->
[`SliderFloat`]: xref:Bonsai.ImGui.SliderFloatBuilder
[`RadioButton`]: xref:Bonsai.ImGui.RadioButtonBuilder
[`Combo`]: xref:Bonsai.ImGui.ComboBuilder
[`ListBox`]: xref:Bonsai.ImGui.ListBoxBuilder
[`PropertyMapping`]: xref:Bonsai.Expressions.PropertyMappingBuilder
