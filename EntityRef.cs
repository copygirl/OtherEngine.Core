using System;

namespace OtherEngine.Core
{
	/// <summary>
	/// Represents and entity that has specific components.
	/// You may use these classes as method parameters and the return type.
	/// 
	/// There are implicit casts available to convert between Entity and
	/// EntityRef types automatically. Converting from an entity ensures
	/// the components exist on the entity.
	/// 
	/// Note that if any of the components are removed from the entity,
	/// the EntityRef is not invalidated - access to the components through
	/// it is still possible. Therefore, references should not be kept.
	/// </summary>
	public abstract class EntityRef
	{
		public Entity Entity { get; private set; }

		protected EntityRef(Entity entity)
		{
			Entity = entity;
		}

		#region Equals and GetHashCode

		public override bool Equals(object obj)
		{
			var @ref = (obj as EntityRef);
			return ((@ref != null) && (Entity == @ref.Entity));
		}

		public override int GetHashCode()
		{
			return Entity.GetHashCode();
		}

		#endregion

		public static implicit operator Entity(EntityRef @ref) { return @ref?.Entity; }
	}


	#region EntityRef<> class definition

	/// <summary>
	/// Represents and entity that has a specific component.
	/// You may use this class as method parameters and the return type.
	/// 
	/// There are implicit casts available to convert between Entity, this
	/// and other EntityRef types automatically. Converting from an entity
	/// ensures the component exists on the entity.
	/// 
	/// Note that if the component is removed from the entity, the
	/// EntityRef is not invalidated - access to the component through it
	/// is still possible. Therefore, references should not be kept.
	/// </summary>
	public class EntityRef<TComponent> : EntityRef
		where TComponent : Component
	{
		public TComponent Component { get; private set; }

		internal EntityRef(Entity entity, TComponent component)
			: base(entity)
		{
			Component = component;
		}

		#region Equals and GetHashCode

		public override bool Equals(object obj)
		{
			var @ref = (obj as EntityRef<TComponent>);
			return ((@ref != null) &&
				(Entity == @ref.Entity) &&
				(Component == @ref.Component));
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() ^ Component.GetHashCode());
		}

		#endregion

		public static implicit operator EntityRef<TComponent>(Entity entity)
		{
			var component = entity.GetOrThrow<TComponent>();
			return new EntityRef<TComponent>(entity, component);
		}
	}

	#endregion

	#region EntityRef<,> class definition

	/// <summary>
	/// Represents and entity that has two specific components.
	/// You may use this class as method parameters and the return type.
	/// 
	/// There are implicit casts available to convert between Entity, this
	/// and other EntityRef types automatically. Converting from an entity
	/// ensures the components exists on the entity.
	/// 
	/// Note that if any of the components are removed from the entity, the
	/// EntityRef is not invalidated - access to the components through it
	/// is still possible. Therefore, references should not be kept.
	/// </summary>
	public class EntityRef<TFirst, TSecond> : EntityRef
		where TFirst : Component
		where TSecond : Component
	{
		public TFirst First { get; private set; }
		public TSecond Second { get; private set; }

		internal EntityRef(Entity entity, TFirst first, TSecond second)
			: base(entity)
		{
			First = first;
			Second = second;
		}

		#region Equals and GetHashCode

		public override bool Equals(object obj)
		{
			var @ref = (obj as EntityRef<TFirst, TSecond>);
			return ((@ref != null) &&
				(Entity == @ref.Entity) &&
				(First == @ref.First) &&
				(Second == @ref.Second));
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() ^ First.GetHashCode() ^ Second.GetHashCode());
		}

		#endregion

		public static implicit operator EntityRef<TFirst, TSecond>(Entity entity)
		{
			var first = entity.GetOrThrow<TFirst>();
			var second = entity.GetOrThrow<TSecond>();
			return new EntityRef<TFirst, TSecond>(entity, first, second);
		}

		#region Implicit cast operators from / to EntityRef<>

		public static implicit operator EntityRef<TFirst, TSecond>(EntityRef<TFirst> @ref)
		{
			var second = @ref.Entity.GetOrThrow<TSecond>();
			return new EntityRef<TFirst, TSecond>(@ref, @ref.Component, second);
		}
		public static implicit operator EntityRef<TFirst, TSecond>(EntityRef<TSecond> @ref)
		{
			var first = @ref.Entity.GetOrThrow<TFirst>();
			return new EntityRef<TFirst, TSecond>(@ref, first, @ref.Component);
		}

		public static implicit operator EntityRef<TFirst>(EntityRef<TFirst, TSecond> @ref)
		{
			return new EntityRef<TFirst>(@ref.Entity, @ref.First);
		}
		public static implicit operator EntityRef<TSecond>(EntityRef<TFirst, TSecond> @ref)
		{
			return new EntityRef<TSecond>(@ref.Entity, @ref.Second);
		}

		#endregion
	}

	#endregion

	#region EntityRef<,,> class definition

	/// <summary>
	/// Represents and entity that has three specific components.
	/// You may use this class as method parameters and the return type.
	/// 
	/// There are implicit casts available to convert between Entity, this
	/// and other EntityRef types automatically. Converting from an entity
	/// ensures the components exists on the entity.
	/// 
	/// Note that if any of the components are removed from the entity, the
	/// EntityRef is not invalidated - access to the components through it
	/// is still possible. Therefore, references should not be kept.
	/// </summary>
	public class EntityRef<TFirst, TSecond, TThird> : EntityRef
		where TFirst : Component
		where TSecond : Component
		where TThird : Component
	{
		public TFirst First { get; private set; }
		public TSecond Second { get; private set; }
		public TThird Third { get; private set; }

		internal EntityRef(Entity entity, TFirst first, TSecond second, TThird third)
			: base(entity)
		{
			First = first;
			Second = second;
			Third = third;
		}

		#region Equals and GetHashCode

		public override bool Equals(object obj)
		{
			var @ref = (obj as EntityRef<TFirst, TSecond>);
			return ((@ref != null) && (Entity == @ref.Entity) &&
				(First == @ref.First) && (Second == @ref.Second));
		}

		public override int GetHashCode()
		{
			return (base.GetHashCode() ^ First.GetHashCode() ^
				Second.GetHashCode() ^ Third.GetHashCode());
		}

		#endregion

		public static implicit operator EntityRef<TFirst, TSecond, TThird>(Entity entity)
		{
			var first = entity.GetOrThrow<TFirst>();
			var second = entity.GetOrThrow<TSecond>();
			var third = entity.GetOrThrow<TThird>();
			return new EntityRef<TFirst, TSecond, TThird>(entity, first, second, third);
		}

		#region Implicit cast operators from / to EntityRef<>

		public static implicit operator EntityRef<TFirst, TSecond, TThird>(EntityRef<TFirst> @ref)
		{
			var second = @ref.Entity.GetOrThrow<TSecond>();
			var third = @ref.Entity.GetOrThrow<TThird>();
			return new EntityRef<TFirst, TSecond, TThird>(@ref, @ref.Component, second, third);
		}
		public static implicit operator EntityRef<TFirst, TSecond, TThird>(EntityRef<TSecond> @ref)
		{
			var first = @ref.Entity.GetOrThrow<TFirst>();
			var third = @ref.Entity.GetOrThrow<TThird>();
			return new EntityRef<TFirst, TSecond, TThird>(@ref, first, @ref.Component, third);
		}
		public static implicit operator EntityRef<TFirst, TSecond, TThird>(EntityRef<TThird> @ref)
		{
			var first = @ref.Entity.GetOrThrow<TFirst>();
			var second = @ref.Entity.GetOrThrow<TSecond>();
			return new EntityRef<TFirst, TSecond, TThird>(@ref, first, second, @ref.Component);
		}

		public static implicit operator EntityRef<TFirst>(EntityRef<TFirst, TSecond, TThird> @ref)
		{
			return new EntityRef<TFirst>(@ref.Entity, @ref.First);
		}
		public static implicit operator EntityRef<TSecond>(EntityRef<TFirst, TSecond, TThird> @ref)
		{
			return new EntityRef<TSecond>(@ref.Entity, @ref.Second);
		}
		public static implicit operator EntityRef<TThird>(EntityRef<TFirst, TSecond, TThird> @ref)
		{
			return new EntityRef<TThird>(@ref.Entity, @ref.Third);
		}

		#endregion

		#region Implicit cast operators from / to EntityRef<,>

		public static implicit operator EntityRef<TFirst, TSecond, TThird>(EntityRef<TFirst, TSecond> @ref)
		{
			var third = @ref.Entity.GetOrThrow<TThird>();
			return new EntityRef<TFirst, TSecond, TThird>(@ref, @ref.First, @ref.Second, third);
		}
		public static implicit operator EntityRef<TFirst, TSecond, TThird>(EntityRef<TFirst, TThird> @ref)
		{
			var second = @ref.Entity.GetOrThrow<TSecond>();
			return new EntityRef<TFirst, TSecond, TThird>(@ref, @ref.First, second, @ref.Second);
		}
		public static implicit operator EntityRef<TFirst, TSecond, TThird>(EntityRef<TSecond, TThird> @ref)
		{
			var first = @ref.Entity.GetOrThrow<TFirst>();
			return new EntityRef<TFirst, TSecond, TThird>(@ref, first, @ref.First, @ref.Second);
		}

		public static implicit operator EntityRef<TFirst, TSecond, TThird>(EntityRef<TSecond, TFirst> @ref)
		{
			var third = @ref.Entity.GetOrThrow<TThird>();
			return new EntityRef<TFirst, TSecond, TThird>(@ref, @ref.Second, @ref.First, third);
		}
		public static implicit operator EntityRef<TFirst, TSecond, TThird>(EntityRef<TThird, TFirst> @ref)
		{
			var second = @ref.Entity.GetOrThrow<TSecond>();
			return new EntityRef<TFirst, TSecond, TThird>(@ref, @ref.Second, second, @ref.First);
		}
		public static implicit operator EntityRef<TFirst, TSecond, TThird>(EntityRef<TThird, TSecond> @ref)
		{
			var first = @ref.Entity.GetOrThrow<TFirst>();
			return new EntityRef<TFirst, TSecond, TThird>(@ref, first, @ref.Second, @ref.First);
		}


		public static implicit operator EntityRef<TFirst, TSecond>(EntityRef<TFirst, TSecond, TThird> @ref)
		{
			return new EntityRef<TFirst, TSecond>(@ref.Entity, @ref.First, @ref.Second);
		}
		public static implicit operator EntityRef<TFirst, TThird>(EntityRef<TFirst, TSecond, TThird> @ref)
		{
			return new EntityRef<TFirst, TThird>(@ref.Entity, @ref.First, @ref.Third);
		}
		public static implicit operator EntityRef<TSecond, TThird>(EntityRef<TFirst, TSecond, TThird> @ref)
		{
			return new EntityRef<TSecond, TThird>(@ref.Entity, @ref.Second, @ref.Third);
		}

		public static implicit operator EntityRef<TSecond, TFirst>(EntityRef<TFirst, TSecond, TThird> @ref)
		{
			return new EntityRef<TSecond, TFirst>(@ref.Entity, @ref.Second, @ref.First);
		}
		public static implicit operator EntityRef<TThird, TFirst>(EntityRef<TFirst, TSecond, TThird> @ref)
		{
			return new EntityRef<TThird, TFirst>(@ref.Entity, @ref.Third, @ref.First);
		}
		public static implicit operator EntityRef<TThird, TSecond>(EntityRef<TFirst, TSecond, TThird> @ref)
		{
			return new EntityRef<TThird, TSecond>(@ref.Entity, @ref.Third, @ref.Second);
		}

		#endregion
	}

	#endregion


	public static class EntityRefExtensions
	{
		/// <summary>
		/// Adds a component to an entity and
		/// returns it as an EntityRef&lt;TComponent&gt;.
		/// </summary>
		public static EntityRef<TComponent> AddRef<TComponent>(
			this Entity entity, TComponent component)
			where TComponent : Component
		{
			if (entity == null) throw new ArgumentNullException("entity");
			ComponentCheckAndAdd<TComponent>(entity, component, "component");
			return new EntityRef<TComponent>(entity, component);
		}

		/// <summary>
		/// Adds two components to an entity and returns
		/// it as an EntityRef&lt;TFirst, TSecond&gt;.
		/// </summary>
		public static EntityRef<TFirst, TSecond> AddRef<TFirst, TSecond>(
			this Entity entity, TFirst first, TSecond second)
			where TFirst : Component
			where TSecond : Component
		{
			if (entity == null) throw new ArgumentNullException("entity");
			ComponentCheckAndAdd<TFirst>(entity, first, "first");
			ComponentCheckAndAdd<TSecond>(entity, second, "second");
			return new EntityRef<TFirst, TSecond>(entity, first, second);
		}

		/// <summary>
		/// Adds three components to an entity and returns
		/// it as an EntityRef&lt;TFirst, TSecond, TThird&gt;.
		/// </summary>
		public static EntityRef<TFirst, TSecond, TThird> AddRef<TFirst, TSecond, TThird>(
			this Entity entity, TFirst first, TSecond second, TThird third)
			where TFirst : Component
			where TSecond : Component
			where TThird : Component
		{
			if (entity == null) throw new ArgumentNullException("entity");
			ComponentCheckAndAdd<TFirst>(entity, first, "first");
			ComponentCheckAndAdd<TSecond>(entity, second, "second");
			ComponentCheckAndAdd<TThird>(entity, third, "third");
			return new EntityRef<TFirst, TSecond, TThird>(entity, first, second, third);
		}


		static void ComponentCheckAndAdd<TComponent>(Entity entity, TComponent component, string paramName)
			where TComponent : Component
		{
			if (component == null)
				throw new ArgumentNullException(paramName);
			if (typeof(TComponent) != component.GetType())
				throw new ArgumentException(string.Format(
					"{0} isn't the same type as {1}",
					Component.ToString(typeof(TComponent)), component),
					"T" + char.ToUpper(paramName[0]) + paramName.Substring(1));
			entity.Add(component);
		}
	}
}

