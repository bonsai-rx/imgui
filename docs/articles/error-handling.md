---
uid: error-handling
---

# Error handling

ImGui raises a native assertion when it detects invalid usage, such as a flag combination that the operator does not support, or a value outside its expected range. On some platforms, this can appear as an abort dialog that terminates the application. `Bonsai.ImGui` handles invalid usage errors in three ways: managed validation at the property level, the ImGui runtime error recovery, and native assertions for the small set of errors that cannot be recovered from.

## Managed validation

For a small set of known invalid configurations, the property setter throws [`ArgumentException`] directly. For example, assigning the Drag-only `WrapAround` flag to a slider operator raises [`ArgumentException`] at the moment the property is set, before any rendering happens. The error surfaces in the property grid so you can correct it before running the workflow.

Managed validation is currently limited to combinations we know would otherwise trigger native assertions. New cases are added as they are discovered.

## ImGui error recovery

For most other invalid usages, the ImGui error recovery system is enabled by default. When an error is detected at runtime, the application does not terminate. Instead, a red tooltip appears on the offending widget describing the problem, and rendering continues for the rest of the workflow.

## Unrecoverable errors

Some errors are not recoverable. ImGui triggers a native assertion when they occur, which on some platforms appears as an abort dialog and may terminate the application. Buffer overflows are one example.

This is a fundamental limitation of the boundary between native ImGui and the .NET runtime. Native exceptions cannot be propagated as managed exceptions, so these crashes cannot be converted into [`ArgumentException`] or similar managed errors. See [Hexa.NET.ImGui issue #35](https://github.com/HexaEngine/Hexa.NET.ImGui/issues/35) for the upstream discussion.

## Debugging guidance

When you encounter an unexpected error, the following steps usually help isolate the cause:

- Identify which operator is generating the error. The red tooltip points to the widget; the operator is the one drawing it.
- Inspect the property values on that operator. The most common cause of recoverable errors is an invalid combination of flags or a value outside the expected range.
- For native assertions, attaching a debugger with mixed-mode debugging enabled gives a clearer call stack. Setup instructions for Visual Studio are documented in [Hexa.NET.ImGui issue #35](https://github.com/HexaEngine/Hexa.NET.ImGui/issues/35).
- If you encounter a native assertion that you believe could be caught by managed validation at the property level, please [file an issue](https://github.com/bonsai-rx/imgui/issues) so we can add it.

<!-- Reference Style Links -->
[`ArgumentException`]: xref:System.ArgumentException
