OtherEngine CORE
================

This is the core of OtherEngine, a custom very early in-development [Entity Component System](1)
game engine written in C# with the aim of being very modular and moddable. Rendering will be done
using [OpenTK](2) using the seperate module [OtherEngine.Core](3).


Engine Structure
----------------

- [Entity](4): Container object which can have any number of components, one of each type.
- [Component](5): Small pieces of data or flags attached to entities.
- [Controller](6): Ideally takes care of a small portion of engine or game logic.
- [Event](7): Messages passed to controllers which are listening for specific event types.

**Entities** would include anything that could be considered a game object and in some cases
even more. For example: Players, creatures, plants and other traditional game objects, items
(whether in-world or owned by another entity), materials, worlds, chunks, biomes, structure
types, textures, models, ...

**Components** would include most (if not all) pieces of data that could be associated with
entities. For example: ID, name, location, color, durability, health, equipment, status effects,
player data, list of chunks (or other child-entities), ...

**Controllers** would control and handle a small selection of components and events, so that
ideally, any controller could be disabled without crippling the whole game, or replacing them
with a different controller. For example: Managing movement, physics, controls, rendering,
networking, world generation, administation, debugging, ...

```
// TODO: Work some more on this README
```


[1]: http://en.wikipedia.org/wiki/Entity_component_system
[2]: http://www.opentk.com/
[3]: https://github.com/OtherEngine/OtherEngine.Graphics
[4]: https://github.com/OtherEngine/OtherEngine.Core/blob/master/Entity.cs
[5]: https://github.com/OtherEngine/OtherEngine.Core/blob/master/Component.cs
[6]: https://github.com/OtherEngine/OtherEngine.Core/blob/master/Controller.cs
[7]: https://github.com/OtherEngine/OtherEngine.Core/blob/master/Event.cs
