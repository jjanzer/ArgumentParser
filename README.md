# Simple C# Argument Parser

This is a really lightweight C# argument parser library in a single file. It is not designed to support every type of issue rather just when you want a quick and simple cli tool without  having to depend on 3rd party libraries. It is loosely modeled off *getopts* and *Go flag*.

## Usage

First instantiate the class object like so:
~~~C#
var parser = new ArgumentParser();
~~~

Next Add your argument types by calling `parser.AddXYZ`, there are 3 currently:
1. `AddBool`
2. `AddString`
3. `AddInt32`

Each method has 3 arguments.

1. Name - A short single word (no spaces) key such as "dryrun" or "count"
2. Help String - This will appear if the user issues a `--help` argument
3. Default Value (optional) - Overrides the default value if the user does not supply the argument

~~~C#
var dryrun = parser.AddBool("dryrun","If supplied will only report what would be done", false);
~~~

Then parse all the arguments by passing in the array from the Main (or similar)

~~~C#
parser.Parse(args);
~~~

You can now query the argument to check if it was supplied and it's current value

~~~C#
if(dryrun.supplied)
{
	// It was provided by the user
}
if(dryrun.value)
{
	// The value is true
}
~~~

Note ensure you do not query `supplied` or `value` until after you run `Parse`.

## Compiling

Simply include `ArgumentParser.cs` into your solution.

## Example

~~~C#
static int Main(string[] args)
{
	// Create a new object that houses all of our options
	var parser = new ArgumentParser();

	// Supply the options
	var dryrun = parser.AddBool("dryrun","If supplied will only report what would be done", false);
	var output = parser.AddString("output","The path to the output folder, must be supplied");

	parser.Parse(args);

	// Check if dryrun is enabled
	if(dryrun.value)
	{
		// It is ...
	}

	// Ensure output was supplied and has a value
	if(output.supplied && string.IsNullOrEmpty(output.value))
	{
		// Check if the folder exists and is writable ...
	} else 
	{
		Console.WriteLine("You must supply an ouptupt path");
		return 1;
	}

	return 0;
}
~~~

## Auto Help

This class always has a `--help` option running the argument against the above code generates:

~~~
$ ./test --help
  --dryrun                     If supplied will only report what would be done
  --output                     The path to the output folder, must be supplied
~~~