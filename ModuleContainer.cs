using System;
using System.Collections.Generic;
using System.Reflection;
using OtherEngine.Core.Attributes;
using OtherEngine.ES;
using OtherEngine.Utility;
using OtherEngine.Utility.Attributes;

namespace OtherEngine.Core
{
	public class ModuleContainer
	{
		#region Public properties

		/// <summary> Gets the game instance this module was created for. </summary>
		public Game Game { get; private set; }

		/// <summary> Gets the assembly the module was loaded from. </summary>
		public Assembly Assembly { get; private set; }


		/// <summary> Gets the name of the module. </summary>
		public string Name { get; private set; }

		/// <summary> Gets the version of the module. </summary>
		public Version Version { get; private set; }


		/// <summary> Gets the authors that made this module. </summary>
		public IReadOnlyCollection<string> Authors { get; private set; }

		/// <summary> Gets an additional credits string for mentioning additional
		///           people, oranizations or technology that helped with this module. </summary>
		public string Credits { get; private set; }

		/// <summary> Gets an URL where more information about the module
		///           can be found, such as its website or repository. </summary>
		public string URL { get; private set; }

		/// <summary> Gets the URL to this module's source repository. </summary>
		public string SourceURL { get; private set; }


		/// <summary> Gets a collection of component types defined by this module. </summary>
		public IReadOnlyCollection<Type> Components { get; private set; }

		/// <summary> Gets a collection of processors that were defined by this module. </summary>
		public IReadOnlyCollection<ProcessorContainer> Processors { get; private set; }

		#endregion


		internal ModuleContainer(Game game, Assembly assembly)
		{
			Game = game;
			Assembly = assembly;
			ValidatedAttribute.ValidateAssembly(assembly);

			GetModuleMetadata();

			List<Type> components;
			List<Type> processors;
			FindEngineTypes(out components, out processors);

			// TODO: Implement proper logging.
			if (components.Count + processors.Count == 0)
				Console.WriteLine("Warning: {0} doesn't contain any engine types", this);

			Components = components.AsReadOnly();
			Processors = processors.CollectionSelect(type => new ProcessorContainer(this, type)).ToReadOnly();
		}


		/// <summary> Collects metadata information for this module, such
		///           as name, version and other data from attributes. </summary>
		void GetModuleMetadata()
		{
			var assemblyName = Assembly.GetName();

			Name = assemblyName.Name;
			Version = assemblyName.Version;

			// TODO: This causes a mono compiler exception, replace when fixed.
			// Authors = (Assembly.GetAttribute<ModuleAuthorsAttribute>()?.Authors ?? new string[0]).AsReadOnly();
			Authors = Assembly.GetAttribute<ModuleAuthorsAttribute>()?.Authors.AsReadOnly() ?? (new string[0]).AsReadOnly();
			Credits = Assembly.GetAttribute<ModuleCreditsAttribute>()?.Credits ?? "";
			URL = Assembly.GetAttribute<ModuleURLAttribute>()?.URL ?? "";
			SourceURL = Assembly.GetAttribute<ModuleSourceAttribute>()?.SourceURL ?? "";
		}

		/// <summary> Collects all component and processor types. </summary>
		void FindEngineTypes(out List<Type> components, out List<Type> processors)
		{
			components = new List<Type>();
			processors = new List<Type>();

			foreach (var type in Assembly.GetTypes()) {
				if (ValidateComponentType(type))
					components.Add(type);
				if (ValidateProcessorType(type))
					processors.Add(type);
			}
		}


		/// <summary> Returns if the specified type is a component type.
		///           Throws an exception if the component type is invalid. </summary>
		static bool ValidateComponentType(Type type)
		{
			if (!typeof(IComponent).IsAssignableFrom(type) || type.IsInterface) return false;

			if (!type.Name.EndsWith("Component", StringComparison.Ordinal))
				throw new InvalidOperationException(string.Format(
					"{0} (IComponent) doesn't end with 'Component' suffix", type));
			if (!type.IsValueType)
				throw new InvalidOperationException(string.Format(
					"{0} is not a struct", type));

			return true;
		}

		/// <summary> Returns if the specified type is a processor type.
		///           Throws an exception if the processor type is invalid. </summary>
		static bool ValidateProcessorType(Type type)
		{
			if (!typeof(IProcessor).IsAssignableFrom(type) || type.IsAbstract) return false;

			if (!type.Name.EndsWith("Processor", StringComparison.Ordinal))
				throw new InvalidOperationException(string.Format(
					"{0} (Processor) doesn't end with 'Processor' suffix", type));
			if (!type.IsClass)
				throw new InvalidOperationException(string.Format(
					"{0} is not a class", type));
			if (type.IsGenericType)
				throw new InvalidOperationException(string.Format(
					"{0} is a generic class", type));
			if (type.GetConstructor(Type.EmptyTypes) == null)
				throw new InvalidOperationException(string.Format(
					"{0} doesn't have an empty public constructor", type));

			return true;
		}


		public override string ToString()
		{
			return string.Format("[{0} ({1})]", Name, Version);
		}
	}
}

