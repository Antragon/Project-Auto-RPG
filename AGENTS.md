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

## Node lookup

- Prefer `NodeExtensions` typed traversal helpers such as `GetAncestor<T>()`, `GetSibling<T>()`, `GetChildOfType<T>()`, and `GetChildrenOfType<T>()` whenever possible.
- Prefer retrieving nodes by type over `GetNode<T>("Name/Path")` magic strings when both express the lookup clearly. Use name paths when node names distinguish multiple nodes of the same type or define a meaningful scene contract.

## Interaction style

- Use reasonable defaults and proceed without asking for routine confirmation.
- Ask questions only when blocked, when a decision materially changes the outcome, or before destructive or external actions.
- Infer minor implementation details from the repository and the user's request.
- Do not tell the user that Godot was not started or run unless they ask.

## Testing

- Do not run Godot for testing unless the user explicitly asks for it.
