// SPDX-License-Identifier: MIT
// Copyright © 2021 Oscar Björhn, Petter Löfgren and contributors

namespace Daf.Core.Sdk
{
	public interface IPlugin
	{
		string Name { get; }
		string Description { get; }
		string Version { get; }
		string TimeStamp { get; }

		int Execute();
	}
}
