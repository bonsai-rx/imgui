---
uid: property-lifetime
---

# Property lifetime

Each operator in `Bonsai.ImGui` follows the same internal pattern: a subscription creates an inner observer that runs once per source notification and renders an ImGui widget. The properties exposed by the operator are used by that inner observer. A property is either read once when the observer is created, or read every time the widget is rendered. That choice determines how the widget responds to property edits over its lifetime.

Operator properties fall into three categories, each with a different read pattern.

## Property categories

| Category | Examples | Read pattern | Why |
| --- | --- | --- | --- |
| **Identity** | `Name` | Cached at subscribe | Forms the ImGui label. Mutating it during rendering resets the widget state. |
| **Initialization** | `InitialValue`, `Capacity` | Cached at subscribe | One-shot by definition. The value describes a starting state or sizes a buffer that cannot be resized. |
| **Dynamic** | `Min`, `Max`, `Format`, `Flags`, `Step`, `Visible` | Read per-frame | Live edits in the property grid apply immediately. |

## Why per-frame reads are safe

ImGui rendering is single-threaded, so there is no race condition between reading a property and submitting its value to ImGui. Per-frame reads are safe without additional synchronization. This also makes properties work safely with the [`InputMapping`] operator. Setting a property in the workflow just before the operator renders gives each subscription its own configuration even though the underlying property is shared across all subscriptions.

## Implementation pattern

The convention inside `Generate` is:

```csharp
return Observable.Create<TResult>(observer =>
{
    // Identity and initialization: captured here
    var label = $"##{Name ?? nameof(ImGui.SliderFloat)}";
    var value = InitialValue;
    observer.OnNext(value);

    var sourceObserver = Observer.Create<TSource>(
        _ =>
        {
            // Dynamic: read directly
            if (Visible && ImGui.SliderFloat(label, ref value, Min, Max, Format, Flags))
                observer.OnNext(value);
        },
        observer.OnError,
        observer.OnCompleted);
    return source.SubscribeSafe(sourceObserver);
});
```

> [!TIP]
> When you add a new operator, follow this rule: if a property contributes to the ImGui label or is one-shot, such as an initial value or an allocation parameter, cache it at subscribe. Otherwise, read it per-frame.

<!-- Reference Style Links -->
[`InputMapping`]: xref:Bonsai.Expressions.InputMappingBuilder
