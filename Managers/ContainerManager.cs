using System;
using System.Collections.Generic;
using OtherEngine.Core.Components;

namespace OtherEngine.Core.Managers
{
	/// <summary>
	/// Base class for other managers which are supposed to keep track
	/// of container entities storing information about their types.
	/// </summary>
	public abstract class ContainerManager<TType> : Manager
	{
		protected readonly Dictionary<Type, Entity> Containers
			= new Dictionary<Type, Entity>();


		internal ContainerManager(Game game) : base(game) {  }


		#region Getting containers

		/// <summary>
		/// Gets the container for this type.
		/// Throws an error if the module containing the type was not loaded.
		/// </summary>
		public Entity GetContainer(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (!type.IsSubclassOf(typeof(TType)))
				throw new ArgumentException(string.Format(
					"{0} is not a {1}", type, typeof(TType).Name), "type");
			if (type.IsAbstract)
				throw new ArgumentException(string.Format(
					"{0} is abstract", type), "type");

			Entity entity;
			if (!Containers.TryGetValue(type, out entity)) {
				if (type.IsGenericType && !type.IsGenericTypeDefinition) {
					
					// Verify that the generic type definition
					// of this type was previously loaded.
					var typeDef = type.GetGenericTypeDefinition();
					GetContainer(typeDef);

					// Create a new container for specific type.
					entity = CreateContainer(type);

				} else throw new InvalidOperationException(string.Format(
					"The module containing {0} has not been loaded", type));
			}

			return entity;
		}

		/// <summary>
		/// Gets the container for the type of this object.
		/// Throws an error if the module containing the type was not loaded.
		/// </summary>
		public Entity GetContainer(TType instance)
		{
			if (instance == null)
				throw new ArgumentNullException("instance");
			return GetContainer(instance.GetType());
		}

		/// <summary>
		/// Gets the container for the generic type.
		/// Throws an error if the module containing the type was not loaded.
		/// </summary>
		public Entity GetContainer<T>() where T : TType
		{
			return GetContainer(typeof(T));
		}

		#endregion


		protected virtual Entity CreateContainer(Type type)
		{
			var container = new Entity(Game);
			Containers.Add(type, container);
			container.Add(new TypeComponent{ Value = typeof(TType).Name });
			return container;
		}

		internal virtual void OnModuleLoaded(IEnumerable<Type> types)
		{
			foreach (var type in types)
				CreateContainer(type);
		}
	}
}

