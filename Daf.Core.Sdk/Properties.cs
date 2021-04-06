// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

using System;
using System.Collections.Generic;

namespace Daf.Core.Sdk
{
	public class Properties
	{
		private static Properties? _instance;

		private Properties()
		{
			OtherProperties = new Dictionary<string, string>();
		}

		public static Properties Instance
		{
			get
			{
				if (_instance == null)
					_instance = new Properties();

				return _instance;
			}
		}

		public string? OutputDirectory { get; set; }

		public string? FilePath { get; set; }

		public string? ProjectDirectory { get; set; }

		public string? CommandLine { get; set; }

		public Dictionary<string, string> OtherProperties { get; private set; }

		public void SetOtherProperties(IEnumerable<string> args)
		{
			if (args == null)
				throw new ArgumentNullException(nameof(args));

			foreach (string property in args)
			{
				string[] tokens = property.Split('=');

				OtherProperties[tokens[0]] = tokens[1];
			}
		}
	}
}
