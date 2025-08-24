using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SkyVuEngine.Core.Console
{
	public class ConsoleManager
	{
		private static ConsoleManager _instance;

		private Dictionary<string, Command> _commands;

		public List<Command> Commands
		{
			get
			{
				return _commands.Values.ToList();
			}
		}

		public static ConsoleManager Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ConsoleManager();
				}
				return _instance;
			}
		}

		private ConsoleManager()
		{
			_commands = new Dictionary<string, Command>();
			Command command = new Command
			{
				Name = "list",
				Info = "Prints a list of registered commands and the info of any command provided as a parameter.",
				ExecuteCommand = List
			};
			Command command2 = new Command
			{
				Name = "set_time_scale",
				Info = "Usage: set_time_scale [scale]\n\t\t\t\t\t\tSets the scale the game is running at.",
				ExecuteCommand = SetScale
			};
			_commands.Add(command.Name, command);
			_commands.Add(command2.Name, command2);
		}

		public bool HasCommand(string commandName)
		{
			return _commands.ContainsKey(commandName);
		}

		public Command GetCommand(string commandName)
		{
			if (_commands.ContainsKey(commandName))
			{
				return _commands[commandName];
			}
			return null;
		}

		public string ExecuteCommand(string commandName)
		{
			return ExecuteCommand(commandName, null);
		}

		public string ExecuteCommand(string commandName, string[] options)
		{
			if (_commands.ContainsKey(commandName))
			{
				return _commands[commandName].ExecuteCommand(options);
			}
			return "Error: " + commandName + " is not a registered command.\n";
		}

		public string GetCommandInfo(string commandName)
		{
			if (_commands.ContainsKey(commandName))
			{
				return _commands[commandName].Info;
			}
			return "Error: " + commandName + " is not a registered command.\n";
		}

		public bool RegisterCommand(Command command)
		{
			if (!_commands.ContainsKey(command.Name))
			{
				_commands.Add(command.Name, command);
				return true;
			}
			return false;
		}

		private string List(string[] options)
		{
			string text = "";
			if (options == null || options.Length == 0)
			{
				for (int i = 0; i < _commands.Count; i++)
				{
					text = text + Commands[i].Name + "\n";
				}
			}
			else
			{
				for (int j = 0; j < options.Length; j++)
				{
					Command command = GetCommand(options[j]);
					text = ((command == null) ? (text + "Error: " + options[j] + " is not a registered command.\n") : (text + command.Info + "\n"));
				}
			}
			return text;
		}

		private string SetScale(string[] options)
		{
			float result = 1f;
			if (float.TryParse(options[0], out result))
			{
				Time.timeScale = result;
				return "Timescale is " + Time.timeScale + ".";
			}
			return "That is not a valid scale";
		}
	}
}
