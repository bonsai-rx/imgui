---
uid: containers
---

# Containers

Dear ImGui exposes several pairs of `Begin*` / `End*` calls that scope the widgets drawn between them. A table groups its widgets into rows and columns; a child window scrolls its widgets independently of the parent; a tab bar arranges them across selectable tabs. These container constructs all share the same idea: a single call opens the scope, intermediate widget calls run inside it, and a matching call closes it.

`Bonsai.ImGui` currently includes two containers, tables and tab bars. Tables use the [`Table`] and [`TableNextColumn`] operators; tab bars use the [`TabBar`] and [`TabItem`] operators. The same begin/end pattern will apply when other containers are added in future releases.

## Implicit begin/end

The Begin and End calls are not separate operators in the workflow. A single operator handles both: it calls `ImGui.Begin*`, emits a notification to its downstream observers, waits for them to process it, and then calls `ImGui.End*`. Operators connected downstream of this operator therefore run inside the scope.

The workflow does not have an explicit End operator. The end is implicit: once all downstream observers have processed the notification, the operator calls `ImGui.End*`. Observers process the notification in order, so the workflow corresponds line-by-line to the equivalent ImGui code.

## Tables

The [`Table`] operator opens a new table. It exposes the number of columns, the table flags, the outer size, and the inner width for horizontal scrolling. On each frame it emits the table identifier inside its `BeginTable` / `EndTable` scope so that downstream operators run inside the open table.

The [`TableNextColumn`] operator advances to the next column inside an open table, wrapping to the first column of the next row when the last column of the current row is reached. Each cell of the table corresponds to a [`TableNextColumn`] followed by the widgets drawn in that cell.

In the following example, each cell of a 2x2 table contains a different widget type:

:::workflow
![A 2x2 table with a text label, a button, a slider, and a combo box](../workflows/containers-tables.bonsai)
:::

The [`Table`] operator subscribes to the `Frame` subject and emits inside its `BeginTable` / `EndTable` scope. Each cell is a separate branch from [`Table`], composed of a [`TableNextColumn`] followed by the widgets to draw in that cell. Branches are processed in order: each [`TableNextColumn`] advances the ImGui column cursor and emits, the downstream widget operator draws in that cell, and the next branch follows. After the last branch has been processed, the [`Table`] notification handler returns, and `EndTable` is called.

## Tab bars

The [`TabBar`] operator opens a tab bar. On each frame it emits an identifier inside its `BeginTabBar` / `EndTabBar` scope so that downstream operators run inside the open tab bar.

The [`TabItem`] operator opens an individual tab inside the tab bar. Its `Text` property labels the tab. On each frame it emits inside its `BeginTabItem` / `EndTabItem` scope so that downstream operators draw the tab content.

In the following example, a tab bar holds two tabs, each with its own widgets:

:::workflow
![A tab bar with a settings tab and an about tab](../workflows/containers-tabs.bonsai)
:::

The [`TabBar`] operator subscribes to the `Frame` subject and emits inside its `BeginTabBar` / `EndTabBar` scope. Each tab is a separate branch from [`TabBar`], composed of a [`TabItem`] followed by the widgets that render its content. A [`TabItem`] only emits when its tab is the active selection, so widgets in inactive tabs do not run for the frame. After the last branch has been processed, the [`TabBar`] notification handler returns, and `EndTabBar` is called.

## Other containers

The same begin/end framing will apply to the other ImGui container constructs as they are added: child windows, groups, menus, popups, and tree nodes. Each container has its own intermediate operators where applicable. Containers without an intermediate cursor, such as groups, require only the begin operator and its downstream observers.

<!-- Reference Style Links -->
[`Table`]: xref:Bonsai.ImGui.TableBuilder
[`TableNextColumn`]: xref:Bonsai.ImGui.TableNextColumnBuilder
[`TabBar`]: xref:Bonsai.ImGui.TabBarBuilder
[`TabItem`]: xref:Bonsai.ImGui.TabItemBuilder
