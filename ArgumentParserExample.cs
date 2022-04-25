using System;

// You can compile this with mono or similar like:
// mcs -sdk:4.5 -target:exe -out:ArgumentParserExample.exe ArgumentParserExample.cs ArgumentParser.cs

/// <summary>
/// A simple class for testing the ArgumentParser and showing an example of usage
/// </summary>
class ArgumentParserExample
{
	static int Main(string[] args)
	{

		// Create a new object that houses all of our options
	var parser2 = new ArgumentParser();

	// Supply the options
	var dryrun = parser2.AddBool("dryrun","If supplied will only report what would be done", true);
	var output = parser2.AddString("output","The path to the output folder, must be supplied");

	parser2.Parse(args);

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



		// Create a new object that houses all of our options
		var parser = new ArgumentParser();

		// Setup test/examples

		var parsedBool0 = parser.AddBool("bool0","Sample boolean, no default, should be false");
		var parsedBool1 = parser.AddBool("bool1","Sample boolean, defaults false", false);
		var parsedBool2 = parser.AddBool("bool2","Sample boolean, defaults true", true);

		var parsedInt0 = parser.AddInt32("int0","Sample integer, no default, should be 0");
		var parsedInt1 = parser.AddInt32("int1","Sample integer, defaults to 1",1);

		var parsedString0 = parser.AddString("string0","Sample string, no default, should be empty");
		var parsedString1 = parser.AddString("string1","Sample string, default null",null);
		var parsedString2 = parser.AddString("string2","Sample string, defaults to hello","hello");
		
		// After calling this everything will be parsed and we can use our values
		// NOTE: do NOT query the parsed values until after Parse or they will not be set
		parser.Parse(args);

		if(!parsedBool0.supplied && parsedBool0.value != false)
		{
			Console.WriteLine("Unexepcted value for parsedBool0: " + parsedBool0.value.ToString());
			return 1;
		}
		if(!parsedBool1.supplied && parsedBool1.value != false)
		{
			Console.WriteLine("Unexepcted value for parsedBool1: " + parsedBool1.value.ToString());
			return 1;
		}
		if(!parsedBool2.supplied && parsedBool2.value != true)
		{
			Console.WriteLine("Unexepcted value for parsedBool2: " + parsedBool2.value.ToString());
			return 1;
		}
		
		if(!parsedInt0.supplied && parsedInt0.value != 0)
		{
			Console.WriteLine("Unexepcted value for parsedInt0: " + parsedInt0.value.ToString());
			return 1;
		}
		if(!parsedInt1.supplied && parsedInt1.value != 1)
		{
			Console.WriteLine("Unexepcted value for parsedInt1: " + parsedInt1.value.ToString());
			return 1;
		}

		if(!parsedString0.supplied && parsedString0.value != "")
		{
			Console.WriteLine("Unexepcted value for parsedString0: " + parsedString0.value.ToString());
			return 1;
		}
		if(!parsedString1.supplied && parsedString1.value != null)
		{
			Console.WriteLine("Unexepcted value for parsedString1: " + parsedString1.value.ToString());
			return 1;
		}
		if(!parsedString2.supplied && parsedString2.value != "hello")
		{
			Console.WriteLine("Unexepcted value for parsedString2: " + parsedString2.value.ToString());
			return 1;
		}

		// Reports all the values and if the user supplied them
		parser.PrintOptions();

		return 0;
	}
}