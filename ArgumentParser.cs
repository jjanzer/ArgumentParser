// MIT License
//
// Copyright (c) 2022 Jesse Janzer
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/// <summary>
/// A one file class for parsing unix style arguments
/// </summary>
public class ArgumentParser 
{

	public enum ArgumentType
	{
		Unknown = 0,
		Int32 = 1,
		String = 2,
		Bool = 3,
	}

	public abstract class Argument
	{
		public abstract string name { get; set; }
		public abstract string help { get; set; }
		public abstract bool supplied { get; set; }
		public abstract ArgumentType argumentType { get; set; }
	}


	public class Argument<T> : Argument
	{
		/// <summary>
		/// Used to map something like "-i" or "--ignore"
		/// </summary>
		public override string name { get; set; }

		/// <summary>
		/// Shows up if we specify --help or -h
		/// </summary>
		public override string help { get; set; }
		
		public override ArgumentType argumentType { get; set; }

		/// <summary>
		/// Boxed object that can be of any type
		/// </summary>
		public T defaultValue;

		/// <summary>
		/// The given value
		/// </summary>
		public T value;

		/// <summary>
		/// True if found in the argument set supplied to the program
		/// </summary>
		public override bool supplied { get; set; }
	}

	/// <summary>
	/// A list of supplied potential arguments supplied by the developer
	/// </summary>
	public List<Argument> options = new List<Argument>();


	public Argument<System.Int32> AddInt32(string name, string help, System.Int32 defaultValue = 0)
	{
		var arg = new Argument<System.Int32>()
		{
			name = name,
			help = help,
			defaultValue = defaultValue,
			argumentType = ArgumentType.Int32,
		};

		options.Add(arg);

		return arg;
	}

	public Argument<string> AddString(string name, string help, string defaultValue = "")
	{
		var arg = new Argument<string>()
		{
			name = name,
			help = help,
			defaultValue = defaultValue,
			argumentType = ArgumentType.String,
		};

		options.Add(arg);

		return arg;
	}

	public Argument<bool> AddBool(string name, string help, bool defaultValue = false)
	{
		var arg = new Argument<bool>()
		{
			name = name,
			help = help,
			defaultValue = defaultValue,
			argumentType = ArgumentType.Bool,

		};

		options.Add(arg);

		return arg;
	}

	/// <summary>
	/// Prints the possible arguments to the console
	/// </summary>
	public void PrintHelp()
	{
		int maxLen = 0;
		foreach(var arg in options)
		{
			if(arg.name.Length > maxLen)
			{
				maxLen = arg.name.Length;
			}
		}

		maxLen += 5; //5 == "  -- "

		if(maxLen < 30)
		{
			maxLen = 30;
		}

		foreach(var arg in options)
		{
			var line = "  --" + arg.name;
			var len = line.Length;
			line = line.PadRight(maxLen,' ');
			line += " " + arg.help;

			Console.WriteLine(line);
		}
	}

	/// <summary>
	/// Outputs all options and tells the user if they are set and their value
	/// </summary>
	public void PrintOptions()
	{
		foreach(var option in options)
		{
			//Console.WriteLine()
			if(option.argumentType == ArgumentType.Int32)
			{
				var real = option as Argument<System.Int32>;
				Console.WriteLine("  " + real.name + " supplied: " + (real.supplied ? "yes" : "no") + " => " + real.value.ToString());
			}
			else if(option.argumentType == ArgumentType.String)
			{
				var real = option as Argument<string>;
				Console.WriteLine("  " + real.name + " supplied: " + (real.supplied ? "yes" : "no") + " => " + (real.value != null ? real.value.ToString() : ""));
			}
			else if(option.argumentType == ArgumentType.Bool)
			{
				var real = option as Argument<bool>;
				Console.WriteLine("  " + real.name + " supplied: " + (real.supplied ? "yes" : "no") + " => " + real.value.ToString());
			}
		}
	}

	/// <summary>
	/// Loops through all options and any option not set (supplied==false)
	/// has a default value set in it's value property
	/// </summary>
	protected void SetAnyDefaults()
	{
		foreach(var option in options)
		{
			//Console.WriteLine()
			if(option.argumentType == ArgumentType.Int32)
			{
				var real = option as Argument<System.Int32>;
				if(!real.supplied)
				{
					real.value = real.defaultValue;
				}
			}
			else if(option.argumentType == ArgumentType.String)
			{
				var real = option as Argument<string>;
				if(!real.supplied)
				{
					real.value = real.defaultValue;
				}
			}
			else if(option.argumentType == ArgumentType.Bool)
			{
				var real = option as Argument<bool>;
				if(!real.supplied)
				{
					real.value = real.defaultValue;
				}
			}
		}
	}

	protected void CloseOption(Argument option, string value = "", bool valueIsProvided = false)
	{
		option.supplied = true;
		if(option.argumentType == ArgumentType.Int32)
		{
			var real = option as Argument<System.Int32>;
			if(valueIsProvided)
			{
				//real.value = value;
				System.Int32 tmp;
				if(System.Int32.TryParse(value, out tmp))
				{
					real.value = tmp;
				} else {
					Console.WriteLine("Unable to parse number for arg: " + option.name);
					throw new System.Exception("Invalid value provided");
				}
			} else 
			{
				real.value = real.defaultValue;
			}
		}
		else if(option.argumentType == ArgumentType.String)
		{
			var real = option as Argument<string>;
			if(valueIsProvided)
			{
				real.value = value;
			} else 
			{
				real.value = real.defaultValue;
			}
		}
		else if(option.argumentType == ArgumentType.Bool)
		{
			var real = option as Argument<bool>;
			if(valueIsProvided)
			{
				value = value.ToLower();
				if(value == "" || value == "1" || value == "yes" || value == "y" || value == "on" || value == "true" || value == "t" || value == "enable" || value == "enabled" || value == "active")
				{
					real.value = true;
				} else 
				{
					real.value = false;
				}
			}
			else
			{
				real.value = real.defaultValue;
			}
		}
	}

	/// <summary>
	/// Parses the list of arguments passed in from main against the supplied options, you must use the AddXYZ functions above first
	/// </summary>
	/// <param name="args"></param>
	public void Parse(string[] args)
	{
		// This is based a bit on `getopts` from unix land
		// syntax that's supported is:
		// -test
		// --test
		// -test=x
		// -test="x"
		// -test x
		// -test "x"
		// --test=x
		// --test="x"
		// --test x
		// --test "x"

		// when a argument is preceded by a flag it "belongs" to that flag, provided the flag accepts an arg

		// You could use something arguments to the program like:
		//  ./foo.exe -a -b 1 -foo --bar --car=dar --ear far --input foo --output="test foo bar" --test=foo

		Argument openOption = null;

		const string argPattern = @"-+([^ =]+)";
		//var regex = new Regex(attributePattern, RegexOptions.IgnoreCase);
		var regex = new Regex(argPattern);

		foreach(var arg in args)
		{
			if(arg == "-h" || arg == "--help" || arg == "-?" || arg == "/?")
			{
				PrintHelp();
				System.Environment.Exit(0);
			}

			if(arg.StartsWith("-"))
			{
				if(openOption != null)
				{
					CloseOption(openOption,"",true);
					openOption = null;
				}

				var match = regex.Match(arg);
				if(!match.Success)
				{
					continue;
				}

				var word = match.Groups[1].ToString();

				bool mapped = false;

				foreach(var option in options)
				{
					if(option.name != word)
					{
						continue;
					}
					
					mapped = true;

					openOption = option;

					var joined = option.name + "=";
					var len = joined.Length;

					var pos = arg.IndexOf(joined);
					if(pos >= 0)
					{
						//it's a joined name/value like --foo=bar and not --foo bar
						var value = arg.Substring(pos + len);
						CloseOption(openOption,value,true);
						openOption = null;
					}
				}

				if(!mapped)
				{
					Console.WriteLine("Unknown argument: " + word);
					System.Environment.Exit(0);
				}
			}
			else
			{
				if(openOption != null)
				{
					CloseOption(openOption,arg,true);
					openOption = null;
				}
			}
		}

		if(openOption != null)
		{
			CloseOption(openOption,"",true);
			openOption = null;
		}

		SetAnyDefaults();
	}
}