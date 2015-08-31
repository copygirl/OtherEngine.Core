using System;
using System.Collections.Generic;
using System.Linq;
using OtherEngine.Core.Hierarchy;

namespace OtherEngine.Core.Managers
{
	public abstract class TypeContainerManager<TType> : ContainerManager<Type> where TType : class
	{
		protected override string ContainerType { get { return typeof(TType).Name; } }


		internal TypeContainerManager(Game game)
			: base(game) {  }


		protected abstract string ToString(Type type);


		#region Getting containers

		/// <summary>
		/// Gets the container for this type.
		/// Throws an error if the module containing the type was not loaded.
		/// </summary>
		public override Entity GetContainer(Type type)
		{
			if (type == null)
				throw new ArgumentException("type");
			if (!type.IsSubclassOf(typeof(TType)))
				throw new ArgumentException(string.Format(
					"{0} is not a {1}", type, typeof(TType).Name), "type");
			if (type.IsAbstract)
				throw new ArgumentException(string.Format(
					"{0} is abstract", type), "type");
			
			var container = base.GetContainer(type);
			if (container == null) {
				if (type.IsGenericType && !type.IsGenericTypeDefinition) {

					// Attempt to get the container of the generic type definition
					// of this type. Throws an exception if it's not been loaded.
					var typeDef = type.GetGenericTypeDefinition();
					var typeDefContainer = GetContainer(typeDef);

					// Create a new container for specific type.
					container = CreateContainer(type);

					// Add this container to typeDef container's hierarchy.
					typeDefContainer.Add(container);

				} else throw new InvalidOperationException(string.Format(
					"The module containing {0} has not been loaded", ToString(type)));
			}

			return container;
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

		#region Module loading

		/// <summary>
		/// Called when a module is being loaded.
		/// For the duration of this, events are delayed. See ModuleManager.
		/// </summary>
		internal virtual IEnumerable<Entity> OnModuleLoad(IEnumerable<Type> keys)
		{
			return keys.Select(CreateContainer).ToList();
		}

		/// <summary>
		/// Called after a module has been loaded.
		/// This happens after OnModuleLoad has been called for every manager.
		/// </summary>
		internal virtual void OnModuleLoaded(Entity moduleContainer)
		{
			var label = ContainerType + "s";
			var group = Game.Hierarchy.GetChild(label) ?? Game.Hierarchy.AddGroup(label);
			group.AddLinks(moduleContainer.GetChild(label).GetChildren());
		}

		#endregion
	}
}

