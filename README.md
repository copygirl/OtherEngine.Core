OtherEngine CORE
================

This is the core of OtherEngine, a custom very early in-development [Entity Component System](1)
game engine written in C# with the aim of being very modular and moddable. Rendering will most
likely be done using MonoGame for cross-platform support.


Engine Structure
----------------

- [GameEntity](2): Container object which can have any number of components, one of each type.
- [GameComponent](3): Small pieces of data or a flags attached to entities.
- [GameSystem](4): Ideally takes care of a small portion of engine or game logic.
- [IGameEvent](5): Messages passed to systems which are listening for specific event types.

**Entities** would include anything that could be considered a game object and in some cases
even more. For example: Players, creatures, plants and other traditional game objects, items
(whether in-world or owned by another entity), materials, worlds, chunks, biomes, structure
types, textures, models, ...

**Components** would include most (if not all) pieces of data that could be associated with
entities. For example: ID, name, location, color, durability, health, equipment, status effects,
player data, list of chunks (or other child-entities), ...

**Systems** would control and handle a small selection of components and events, so that
ideally, any system could be disabled (or suspended) without crippling the whole game, or
replacing them with a different system. For example: Managing movement, physics, controls,
rendering, networking, world generation, administation, debugging, ...

```
// TODO: Work some more on this README
```


[1]: http://en.wikipedia.org/wiki/Entity_component_system
[2]: https://github.com/OtherEngine/OtherEngine.Core/blob/master/Data/GameEntity.cs
[3]: https://github.com/OtherEngine/OtherEngine.Core/blob/master/Data/GameComponent.cs
[4]: https://github.com/OtherEngine/OtherEngine.Core/blob/master/Systems/GameSystem.cs
[5]: https://github.com/OtherEngine/OtherEngine.Core/blob/master/Events/IGameEvent.cs
[6]: https://github.com/OtherEngine/OtherEngine.Core/blob/master/Systems/ComponentTrackerSystem.cs
[7]: https://github.com/OtherEngine/OtherEngine.Core/blob/master/Data/GameData.cs
