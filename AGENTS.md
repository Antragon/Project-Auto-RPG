# Project Agent Guidance

## Dependency resolution

- Scene roots implement `ISceneRoot` and expose dependencies through `Get<T>(object? serviceKey = null)`.
- Nodes can access the current scene root through the `NodeExtensions.Root` extension property.
- Resolve global or scene-registered dependencies from nodes with `this.Root.Get<T>()`.
- Pass `serviceKey` to `Get<T>` when resolving a keyed service.

## Class dependency style

- Declare dependencies together at the top of the class, before methods and other implementation details.
- Resolve dependencies lazily with a field-backed expression-bodied property:

  ```csharp
  private IconRepository IconRepository => field ??= this.Root.Get<IconRepository>();
  ```

- Use the dependency property throughout the class instead of resolving the same service repeatedly.

## Code quality

- Keep UI controls focused on one responsibility; put slot-specific behavior in the slot control.
- Avoid parallel arrays and repeated index-based switches when a cohesive type can own that data.
- Review changed classes for duplication and event cleanup before finishing.
- Prefer clear code over abstractions added only to reduce a few repeated lines.

## Design principles

- Apply SRP by giving each class one cohesive responsibility and a clear reason to change; move behavior to the component that owns it.
- Apply Open/Closed (OCP) when a second concrete variation exists; avoid speculative extension points.
- Preserve base-type contracts (LSP) when using inheritance; prefer composition when behaviors vary independently.
- Apply Interface Segregation (ISP) by keeping interfaces small and shaped around their consumers.
- Apply Dependency Inversion (DIP) at real architectural boundaries or where alternate implementations improve isolation; avoid adding an interface for every class.
- Keep domain rules independent from presentation and infrastructure where the existing architecture supports that boundary.
- Share code when behavior and meaning match; do not combine superficially similar logic.
- Keep state changes and invariants inside their owning model or control, and expose only the operations callers need.
- Prefer the simplest design that supports the current requirements; add layers, factories, and configuration only when they solve a concrete need.

## Interaction style

- Use reasonable defaults and proceed without asking for routine confirmation.
- Ask questions only when blocked, when a decision materially changes the outcome, or before destructive or external actions.
- Infer minor implementation details from the repository and the user's request.

## Testing

- Do not run Godot for testing unless the user explicitly asks for it.
