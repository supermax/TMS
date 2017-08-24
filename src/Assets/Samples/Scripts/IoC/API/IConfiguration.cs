using System.Collections.Generic;

public interface IConfiguration
{
	string Name { get; }

	IDictionary<string, object> Values { get; }
}