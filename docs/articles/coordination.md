---
uid: coordination
---

# Coordination

Some widgets need to coordinate their state with each other: a set of radio buttons where exactly one is selected, paired sliders that share a property value, a panel of combo boxes drawing from the same items list. The interactive operators in `Bonsai.ImGui` each represent a single widget instance, so any state shared across instances has to be expressed in the workflow itself.

This article covers the subject-based pattern for two-way coordination, using a radio button group as the worked example. The same pattern applies to any group of operators that share a writable property.

## Radio button group

The [`RadioButton`] operator represents a single button. It exposes two integer properties:

- `ButtonValue` is the value this button represents within its group, set once at design time.
- `SelectedValue` is the current selection across the group. The button highlights when `SelectedValue` equals `ButtonValue`, and on click writes its `ButtonValue` back to `SelectedValue` and emits the new selection.

Forming a group means keeping `SelectedValue` synchronized across every instance. When one button is clicked it updates its own `SelectedValue`, but the other buttons need to learn about the change so their highlight updates on the next frame.

The synchronization is expressed by a [`BehaviorSubject`] that owns the current selection. Each [`RadioButton`] reads the latest value into `SelectedValue` before drawing, and writes back into the subject when clicked:

:::workflow
![A radio button group sharing a selection through a BehaviorSubject](../workflows/coordination-radio-group.bonsai)
:::

Each button is connected the same way:

1. A [`SubscribeSubject`] reads the current value of the shared selection.
2. A [`PropertyMapping`] writes that value into `SelectedValue` before the next draw.
3. A [`SubscribeSubject`] on `Frame` emits notifications each frame so the button draws and reports clicks.
4. A [`MulticastSubject`] writes the click output back into the shared selection.

The [`BehaviorSubject`] holds the current selection so any new observer, including a button instance that is added later or unhidden, immediately sees the latest value rather than waiting for the next click.

## One-way alternatives

When the shared state only flows in one direction, for example when an external source determines the selection and the buttons are read-only indicators, an [externalized property](xref:expressions-externalizedmapping) can replace the subject. Externalizing `SelectedValue` lifts it to the parent workflow level, where a single value can be set across all buttons at once. Clicks still emit on the buttons, but the click does not propagate through an externalized property to the other buttons in the same group.

> [!TIP]
> The same pattern fits any configuration property shared across instances, such as the `Min`/`Max` range of paired sliders or the items list of multiple combo boxes.

Use the subject pattern when buttons should remain interactive and writes should propagate; use an externalized property when the buttons are read-only and the configuration comes from outside the group.

## Coordinating inside an operator

The coordination in this article is expressed in the workflow graph. An alternative is to express it inside a single operator authored in C#, where the iteration over buttons happens in code. See [Custom controls](xref:custom-controls) for a worked example of a `RadioButtonGroup` operator that draws the whole group from one node.

Compose the coordination from existing operators when it is dynamic, when the buttons are scattered across the workflow, or when each instance needs to be configurable from the property grid. Author a custom operator when the coordination is fixed, lives next to a single piece of state, and would otherwise require explicit connections for every instance.

<!-- Reference Style Links -->
[`RadioButton`]: xref:Bonsai.ImGui.RadioButtonBuilder
[`BehaviorSubject`]: xref:Bonsai.Reactive.BehaviorSubject
[`SubscribeSubject`]: xref:Bonsai.Expressions.SubscribeSubject
[`MulticastSubject`]: xref:Bonsai.Expressions.MulticastSubject
[`PropertyMapping`]: xref:Bonsai.Expressions.PropertyMappingBuilder
