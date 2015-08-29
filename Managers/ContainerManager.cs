using System;
using System.Collections.Generic;
using System.Linq;
using OtherEngine.Core.Components;
using OtherEngine.Core.Controllers;

namespace OtherEngine.Core.Managers
{
	public abstract class ContainerManager<TKey> where TKey : class
	{
		readonly Dictionary<TKey, Entity> _containers = new Dictionary<TKey, Entity>();


		protected Game Game { get; private set; }

		protected virtual string ContainerType { get { return typeof(TKey).Name; } }


		/// <summary>
		/// Gets the container entities for this manager.
		/// </summary>
		public IEnumerable<Entity> Containers { get { return _containers.Values.Select(c => c); } }


		internal ContainerManager(Game game)
		{
			Game = game;
		}

		/// <summary>
		/// Gets the container for this key.
		/// </summary>
		public virtual Entity GetContainer(TKey key)
		{
			if (key == null)
				throw new ArgumentNullException("key");

			Entity container;
			return (_containers.TryGetValue(key, out container) ? container : null);
		}


		protected virtual Entity CreateContainer(TKey key)
		{
			var container = new Entity(Game) {
				new TypeComponent { Value = ContainerType } };

			_containers.Add(key, container);

			return container;
		}
	}
}

