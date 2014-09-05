## LogHelper

A toolkit for writing out to log files with little to no configuration.

### Documentation


You can download the Nuget package [here](https://www.nuget.org/packages/LogHelper)

### Installation

   Install-Package LogHelper

### Quick How To
or, how to start logging with LogHelper... it's so easy ;-)
```csharp
namespace Demo
{
  class Program
  {
    static void Main(string[] args)
    {
		var foo = new Foo();

		foo.Bar();
    }
  }

  public class Foo
  {
	public void Bar()
	{
		this.Log(LogLevel.Trace, "Logged it!");
	}
  }
}
```