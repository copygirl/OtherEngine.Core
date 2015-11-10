using System;
using System.Linq;
using OtherEngine.Utility;

namespace OtherEngine.Core.Attributes
{
	/// <summary> Specifies which authors have developed or created assets for this module.
	///           If omitted, tries to find and use the AssemblyCopyright attribute instead. </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class ModuleAuthorsAttribute : Attribute
	{
		public string[] Authors { get; private set; }

		[CLSCompliant(false)]
		public ModuleAuthorsAttribute(params string[] authors)
		{
			if (authors == null)
				throw new ArgumentNullException("authors");
			if (authors.Contains(null))
				throw new ArgumentElementNullException(authors, "authors");
			Authors = authors;
		}

		public ModuleAuthorsAttribute(string authors)
			: this(authors.Split(',')) {  }
	}

	/// <summary> Specifies additional credits for mentioning people, organizations
	///           or technology that have helped with the development of this module. </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class ModuleCreditsAttribute : Attribute
	{
		public string Credits { get; private set; }

		public ModuleCreditsAttribute(string credits)
		{
			if (credits == null)
				throw new ArgumentNullException("credits");
			Credits = credits;
		}
	}

	/// <summary> Specifies a URL where more information about
	///           the module can be found, such as its website. </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class ModuleURLAttribute : Attribute
	{
		public string URL { get; private set; }

		public ModuleURLAttribute(string url)
		{
			if (url == null)
				throw new ArgumentNullException("url");
			URL = url;
		}
	}

	/// <summary> Specifies a URL of the modules repository. </summary>
	[AttributeUsage(AttributeTargets.Assembly)]
	public class ModuleSourceAttribute : Attribute
	{
		public string SourceURL { get; private set; }

		public ModuleSourceAttribute(string sourceUrl)
		{
			if (sourceUrl == null)
				throw new ArgumentNullException("sourceUrl");
			SourceURL = sourceUrl;
		}
	}
}

