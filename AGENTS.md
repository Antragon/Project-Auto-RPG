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

## Interaction style

- Use reasonable defaults and proceed without asking for routine confirmation.
- Ask questions only when blocked, when a decision materially changes the outcome, or before destructive or external actions.
- Infer minor implementation details from the repository and the user's request.
