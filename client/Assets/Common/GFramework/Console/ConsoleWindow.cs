// V.M12.D11.2010.R1
/************************************************************************
* DebugConsole.cs
* Copyright 2008-2010 By: Jeremy Hollingsworth
* (http://www.ennanzus-interactive.com)
*
* Licensed for commercial, non-commercial, and educational use. 
* 
* THIS PRODUCT IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND. THE 
* LICENSOR MAKES NO WARRANTY REGARDING THE PRODUCT, EXPRESS OR IMPLIED. 
* THE LICENSOR EXPRESSLY DISCLAIMS AND THE LICENSEE HEREBY WAIVES ALL 
* WARRANTIES, EXPRESS OR IMPLIED, INCLUDING, BUT NOT LIMITED TO, ALL 
* IMPLIED WARRANTIES OF MERCHANTABILITY AND ALL IMPLIED WARRANTIES OF 
* FITNESS FOR A PARTICULAR PURPOSE.
* ************************************************************************/
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using GFramework;

/// <summary>
/// Provides a game-mode, multi-line console with command binding, logging and watch vars.
/// 
/// ==== Installation ====
/// Just drop this script into your project. To use from JavaScript(UnityScript), just make sure
/// you place this script in a folder such as "Plugins" so that it is compiled before your js code.
///
/// See the following Unity docs page for more info on this: 
/// http://unity3d.com/support/documentation/ScriptReference/index.Script_compilation_28Advanced29.html
/// 
/// ==== Usage (Logging) ====
/// 
/// To use, you only need to access the desired static Log functions. So, for example, to log a simple
/// message you would do the following:
/// 
/// \code
/// DebugConsole.Log( "Hello World!");
/// 
/// // Now open it
/// DebugConsole.IsOpen = true;
/// \endcode
/// 
/// Those static methods will automatically ensure that the console has been set up in your scene for you,
/// so no need to worry about attaching this script to anything.
/// 
/// See the comments for the other static functions below for details on their use.
/// 
/// ==== Usage (DebugCommand Binding) ====
/// 
/// To use command binding, you create a function to handle the command, then you register that function
/// along with the string used to invoke it with the console.
/// 
/// So, for example, if you want to have a command called "ShowFPS", you would first create the handler like 
/// this:
/// 
/// \code
/// // JavaScript
/// function ShowFPSCommand(args)
/// {
///     //...
/// }
/// 
/// // C#
/// public void ShowFPSCommand(params string[] args)
/// {
///     //...
/// }
/// \endcode
/// 
/// Then, to register the command with the console to be run when "ShowFPS" is typed, you would do the following:
/// 
/// \code
/// DebugConsole.RegisterCommand( "ShowFPS", ShowFPSCommand);
/// \endcode
/// 
/// That's it! Now when the user types "ShowFPS" in the console and hits enter, your function will be run.
/// 
/// If you wish to capture input entered after the command text, the args array will contain every space-separated
/// block of text the user entered after the command. "SetFOV 90" would pass the string "90" to the SetFOV command.
///  
/// Note: Typing "/?" followed by enter will show the list of currently-registered commands.
/// 
/// ==== Usage (Watch Vars) ===
/// 
/// For the Watch Vars feature, you need to use one of the provided (or your own subclass of WatchVar) to store
/// the value of your variable in your project. You then register that WatchVar with the console for tracking.
/// 
/// Example:
/// \code
/// // JavaScript
/// var myWatchInt = new WatchInt("PowerupCount");
/// 
/// myWatchInt.Value = 23;
/// 
/// DebugConsole.RegisterWatchVar(myWatchInt.Name, myWatchInt);
/// \endcode
/// 
/// As you use that WatchInt to store your value through the project, its live value will be shown in the console.
/// 
/// If you subclass WatchVar, you can create your own WatchVars to represent more types than are currently built-in.
/// </summary>


public class ConsoleWindow : SingletonMono<ConsoleWindow>
{
	public const string DEFAULT = "Default";
	public const string SYSTEM = "System";

	// This Enum holds the message types used to easily control the formatting and display of a message.
	public enum MessageTypes
	{
		Normal,
		Error,
		Warning,
		Command,
		System
	};

	// Display modes for the console.
	public enum ContentMode
	{
		Cmd = 0,
		Log,
		Watch
	};

	private static readonly string[] contentModeNames = new string[] { "Comd", "Log", "Watch" };

	public enum ShowMode
	{
		Hide = 0,
		Full,
		Compact,
	}

	// My Ip address
	public const string GM_IP = "123.30.135.67";

	private string ipAddress;

	// GUI skin
	public GUISkin guiSkin;

	// Background transparent
	public float bgTranparent = 0.8f;

	private bool showOptionDialog = false;

	public Rect optWindowRect = new Rect(0, 0, 100, 100);

	// Default color of the standard display text.
	public Color DefaultTextColor = Color.white;

	// Current window rect
	public Rect windowRect;
	private Vector2 minWindowSize = new Vector2(200, 30);

	// Content mode
	private ContentMode contentMode = ContentMode.Cmd;

	// Watch var
	private Hashtable watchVarTable;

	// Builder
	private StringBuilder displayString;

	// Input string
	private string inputString = string.Empty;

	// History
	//--------------------------------------------------------
	// How many command in history
	public int histotySize = 20;
	private List<string> historyList = new List<string>();
	private int historyCursor = 0;

	// Current show mode
	public ShowMode showMode { get; private set; }

	// GUI states
	//--------------------------------------------------------
	// Current display group
	private bool showGroupSelection;
	private int currentGroupIdx;
	private string[] groupNames;
	// Resize
	private bool isResizing = false;
	private Rect windowResizeStart = new Rect();
	// Current resizing rect
	private Rect resizingRect;
	// Scroll poss
	private Vector2 logScrollPos = Vector2.zero;
	private Vector2 watchVarsScrollPos = Vector2.zero;

	/// <summary>
	/// Represents a single message, with formatting options.
	/// </summary>
	private class Message
	{
		public string group;
		public string message;
		public MessageTypes messageType;
		public Color displayColor;
		public bool useCustomColor;

		public Message()
		{
			this.group = DEFAULT;
			this.message = "";
			this.messageType = MessageTypes.Normal;
			this.displayColor = new Color();
			this.useCustomColor = false;
		}
	}

	private SortedDictionary<string, LinkedList<Message>> _messages = new SortedDictionary<string, LinkedList<Message>>();

	/// <summary>
	/// Awakes this instance.
	/// </summary>
	void Awake()
	{
		watchVarTable = new Hashtable();
		displayString = new StringBuilder();


		AddMessage(new Message()
			{
				message = "Welcome to Debug Console.",
				messageType = MessageTypes.System
			});

		//StartCoroutine(RequestMyIP());
		DontDestroyOnLoad(gameObject);
	}

	/// <summary>
	/// Adds the message.
	/// </summary>
	/// <param name="message">The message.</param>
	private void AddMessage(Message message)
	{
		LinkedList<Message> list;
		if (!_messages.TryGetValue(message.group, out list))
		{
			list = new LinkedList<Message>();
			_messages.Add(message.group, list);

			groupNames = _messages.Keys.ToArray();
		}

		list.AddLast(message);
	}

	/// <summary>
	/// Checks the message limit.
	/// </summary>
	/// <param name="group">The group.</param>
	private void CheckMessageLimit(string group)
	{
		LinkedList<Message> list;
		if (_messages.TryGetValue(group, out list))
		{
			if (list.Count > histotySize)
				list.RemoveFirst();
		}
	}

	/// <summary>
	/// Show the console
	/// </summary>
	public void Show(ShowMode mode)
	{
		if (showMode == mode)
			return;

		if (iTween.Count(gameObject) > 0)
			return;


		// Old mode
		if (showMode == ShowMode.Hide)
		{
			float height = Screen.height / 2;
			windowRect = new Rect(0.0F, -height, Screen.width, height);
		}

		// New mode
		Rect destRect;
		if (mode == ShowMode.Full)
		{
			destRect = new Rect(0.0F, 0.0F, Screen.width, Screen.height / 2);
		}
		else if (mode == ShowMode.Compact)
		{
			destRect = new Rect(0.0F, 0.0F, Screen.width, minWindowSize.y);
		}
		else // hide
		{
			destRect = windowRect;
			destRect.y = -windowRect.height;
		}

		if (mode == ShowMode.Hide)
		{
			iTween.ValueTo(gameObject, iTween.Hash(
				iT.ValueTo.from, windowRect,
				iT.ValueTo.to, destRect,
				iT.ValueTo.onupdate, "SetWindowSize",
				iT.ValueTo.speed, 1500f,
				iT.ValueTo.easetype, iTween.EaseType.easeOutExpo,
				iT.ValueTo.oncomplete, "OnHideComplete",
				iT.ValueTo.oncompletetarget, this));
		}
		else
		{
			iTween.ValueTo(gameObject, iTween.Hash(
				iT.ValueTo.from, windowRect,
				iT.ValueTo.to, destRect,
				iT.ValueTo.onupdate, "SetWindowSize",
				iT.ValueTo.speed, 1500f,
				iT.ValueTo.easetype, iTween.EaseType.easeOutExpo));

			showMode = mode;
		}
	}

	private void OnHideComplete()
	{
		showMode = ShowMode.Hide;
	}

	private void SetWindowSize(Rect rct)
	{
		resizingRect = rct;
		windowRect = rct;
	}

	public void ToogleShow(bool compactMode)
	{
		if (compactMode)
		{
			if (showMode != ShowMode.Compact)
				Show(ShowMode.Compact);
			else
				Show(ShowMode.Hide);
		}
		else
		{
			if (showMode != ShowMode.Full)
				Show(ShowMode.Full);
			else
				Show(ShowMode.Hide);
		}
	}

	public IEnumerator RequestMyIP()
	{
		WWW www = new WWW("http://checkip.dyndns.org");
		if (www != null)
		{
			yield return www;

			ipAddress = www.text;
			if (!string.IsNullOrEmpty(ipAddress))
				ipAddress = ipAddress.Substring(ipAddress.IndexOf(":") + 1).Trim();

			Debug.Log("My IP = " + ipAddress);
		}
	}

	void OnGUI()
	{
		GUI.skin = guiSkin;
		Color cl = Color.white;
		cl.a = bgTranparent;
		GUI.color = cl;

		if (showMode != ShowMode.Hide)
		{
			string title = "Debug Console - ";
			switch (contentMode)
			{
				case ContentMode.Cmd:
					title += "Command Line";
					break;

				case ContentMode.Log:
					title += "Log Only";
					break;

				case ContentMode.Watch:
					title += "Watch Variables";
					break;
			}

			windowRect = GUI.Window(-1111, windowRect, MainWindow, "", GUI.skin.box);
			GUI.BringWindowToFront(-1111);

			if (showOptionDialog)
			{
				float haftWidth = Screen.width / 2;
				float haftHeight = Screen.height / 2;
				GUI.Window(-1112, optWindowRect, OptionWindow, "", GUI.skin.box);
				GUI.BringWindowToFront(-1112);
			}
		}

		if (false && Event.current.isKey &&
			Event.current.type == EventType.KeyUp &&
			Event.current.keyCode == KeyCode.BackQuote)
		{
			ToogleShow((Event.current.modifiers & EventModifiers.Shift) == EventModifiers.Shift);

			/*if (showMode != ShowMode.Hide)
			{
				if ((Event.current.modifiers & EventModifiers.Shift) == EventModifiers.Shift)
				{
					if (showMode == ShowMode.Full)
						Show(ShowMode.Compact);
					else
						Show(ShowMode.Full);
				}
				else
					Show(ShowMode.Hide);
			}
			else
			{
				if ((Event.current.modifiers & EventModifiers.Shift) == EventModifiers.Shift)
					Show(ShowMode.Compact);
				else
					Show(ShowMode.Full);
			}*/

		}

		GUI.color = Color.white;
	}

	#region StaticAccessors

	/// <summary>
	/// Clears all console output.
	/// </summary>
	public static void Clear()
	{
		ConsoleWindow.instance.ClearLog(null);
	}

	/// <summary>
	/// Registers a named "watch var" for monitoring.
	/// </summary>
	/// <param name="name">Name of the watch var to be shown in the console.</param>
	/// <param name="watchVar">The WatchVar instance you want to monitor.</param>
	public static void RegisterWatchVar(string name, WatchVar watchVar)
	{
		ConsoleWindow.instance.AddWatchVarToTable(name, watchVar);
	}

	/// <summary>
	/// Removes a previously-registered watch var.
	/// </summary>
	/// <param name="name">Name of the watch var you wish to remove.</param>
	public static void UnRegisterWatchVar(string name)
	{
		ConsoleWindow.instance.RemoveWatchVarFromTable(name);
	}

	#endregion

	#region InternalFunctionality

	private void OpListItem(string strItemName, ContentMode mode)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(10);
		if (GUILayout.Toggle(contentMode == mode, strItemName))
			contentMode = mode;
		GUILayout.EndHorizontal();
	}

	private void OpListBox()
	{
		GUILayout.BeginVertical();
		for (int i = 0; i < contentModeNames.Length; i++)
		{
			OpListItem(contentModeNames[i], (ContentMode)i);
		}
		GUILayout.EndVertical();
	}

	private void OptionWindow(int windowID)
	{
		GUILayout.BeginVertical();
		//contentMode = (ContentMode)GUILayout.SelectionGrid((int)contentMode, contentModeNames, 1);//
		OpListBox();

		GUILayout.Space(10);

		bgTranparent = GUILayout.HorizontalSlider(bgTranparent, 0.2f, 1);
		GUILayout.EndVertical();
	}

	private void MainWindow(int windowID)
	{
		GUILayout.BeginVertical();
		//GUILayout.Space(5.0F);

		if (windowRect.height > minWindowSize.y + 30)
		{
			if (contentMode == ContentMode.Cmd)
			{
				logScrollPos = GUILayout.BeginScrollView(logScrollPos);
				DisplayNormalLog(groupNames.Length > 0 ? groupNames[currentGroupIdx] : DEFAULT);
				GUILayout.EndScrollView();

				//GUILayout.Space(4.0F);
			}
			else if (contentMode == ContentMode.Log)
			{
				logScrollPos = GUILayout.BeginScrollView(logScrollPos);
				BuildDisplayString(groupNames.Length > 0 ? groupNames[currentGroupIdx] : DEFAULT);
				GUILayout.TextArea(displayString.ToString(), GUI.skin.label, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
				GUILayout.EndScrollView();
			}
			else if (contentMode == ContentMode.Watch)
			{
				watchVarsScrollPos = GUILayout.BeginScrollView(watchVarsScrollPos);
				foreach (string key in watchVarTable.Keys)
				{
					GUILayout.Space(2.0F);

					GUILayout.BeginHorizontal();
					GUILayout.Label(((WatchVar)watchVarTable[key]).Name + ": ");
					GUILayout.FlexibleSpace();
					GUILayout.Label(((WatchVar)watchVarTable[key]).GetValue().ToString());
					GUILayout.Space(2.0F);
					GUILayout.EndHorizontal();
				}
				GUILayout.EndScrollView();
			}
		}
		else
		{
			GUILayout.FlexibleSpace();
		}

		currentGroupIdx = Mathf.Clamp(currentGroupIdx, 0, groupNames.Length);

		// Show group selection
		if (showGroupSelection && groupNames.Length > 0)
		{
			int newGroupIdx = GUILayout.SelectionGrid(currentGroupIdx, groupNames, 4, GUILayout.Height(30));
			if (newGroupIdx != currentGroupIdx)
			{
				currentGroupIdx = newGroupIdx;
				showGroupSelection = false;
			}
		}

		GUILayout.BeginHorizontal();
		GUI.SetNextControlName("input_field");
		inputString = GUILayout.TextField(inputString, GUILayout.ExpandWidth(true));

		//int textFieldID = GUIUtility.GetControlID( new GUIContent("input_field"), FocusType.Keyboard);
		//if (GUIUtility.keyboardControl == textFieldID )

		if (Event.current.type == EventType.KeyUp &&
			Event.current.isKey &&
			GUI.GetNameOfFocusedControl() == "input_field")
		{
			if (Event.current.keyCode == KeyCode.Return && inputString.Trim() != string.Empty)
			{
				// Remove all later cmd after cursor
				for (int i = historyCursor; i < historyList.Count; i++)
					historyList.RemoveRange(historyCursor, historyList.Count - historyCursor);

				// Insert to cursor location
				historyList.Add(inputString);
				if (historyList.Count > histotySize)
					historyList.RemoveAt(0);

				historyCursor = historyList.Count;

				if (inputString == "?")
				{
					LogMessage(
						"\n------------- Console Help ------------- \n" +
						"CTRL + LeftMouse to resize the console\n" +
						"Useful commands:\n" +
						"- close: close this console\n" +
						"- clear: clear this console content\n" +
						"- help or ?: display a list of available commands\n" +
						"------------- Console Help -------------", SYSTEM);
				}
				else
				{
					string sRet = GFramework.ConsoleCommands.ExecuteCommand(inputString);
					if (sRet != string.Empty)
						LogMessage(sRet, DEFAULT, MessageTypes.Command);
				}
				inputString = "";
			}
			else if (Event.current.keyCode == KeyCode.UpArrow)
			{
				historyCursor--;
				if (historyCursor < 0)
					historyCursor = 0;

				inputString = historyList[historyCursor];
			}
			else if (Event.current.keyCode == KeyCode.DownArrow)
			{
				historyCursor++;
				if (historyCursor >= historyList.Count)
					historyCursor = historyList.Count - 1;

				inputString = historyList[historyCursor];
			}
		}

		//GUILayout.Space(4.0F);

		showGroupSelection = GUILayout.Toggle(showGroupSelection, "Groups", guiSkin.button, GUILayout.ExpandWidth(false));

		if (GUILayout.Button("Clear", GUILayout.ExpandWidth(false)))
		{
			Clear();
		}
		// Buttons group
		/*if (GUILayout.Button(contentMode.ToString(), GUILayout.Width(60f), GUILayout.Height(20f)))
		{
			contentMode++;
			if ((int)contentMode > (int)ContentMode.Watch)
				contentMode = ContentMode.Cmd;
		}*/

		//GUILayout.Box("", GUILayout.Width(20f), GUILayout.Height(20f));

		// Option button
		if (GUILayout.Button("O", GUILayout.Width(24f)))
			showOptionDialog = !showOptionDialog;

		if (Event.current.type == EventType.Repaint)
		{
			Rect btnOptRect = GUILayoutUtility.GetLastRect();
			optWindowRect.x = btnOptRect.x + btnOptRect.width - optWindowRect.width;
			optWindowRect.y = btnOptRect.y - optWindowRect.height;
		}

		GUILayout.EndHorizontal();
		GUILayout.EndVertical();

		// Resize window
		resizingRect = ResizeWindow(resizingRect, ref isResizing, ref windowResizeStart, minWindowSize);

		// Only resize in repaint event
		if (Event.current.type == EventType.repaint)
		{
			windowRect = resizingRect;
		}

		//if (!isResizing)
		//	GUI.DragWindow();
	}

	public Rect ResizeWindow(Rect windowRect, ref bool isResizing, ref Rect resizeStart, Vector2 minWindowSize)
	{
		/*GUIContent content = new GUIContent("", "drag to resize");
		//Rect r = GUILayoutUtility.GetRect(content, styleWindowResize);
		//GUI.Button(r, content);//, styleWindowResize);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUI.SetNextControlName("resizer");
		GUILayout.Button(content, GUILayout.Width(15f), GUILayout.Height(15f));
		GUILayout.EndHorizontal();
		*/

		//if (Event.current.type == EventType.Layout)
		//	return windowRect;

		//if (Event.current.type == EventType.Used)
		//	Debug.Log(r + " " + Event.current.type);

		Vector2 mouse = GUIUtility.ScreenToGUIPoint(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y));
		if (Event.current.type == EventType.mouseDown /*&&
			(Event.current.modifiers & EventModifiers.Control) == EventModifiers.Control*/
																						  )
		{
			isResizing = true;
			resizeStart = new Rect(mouse.x, mouse.y, windowRect.width, windowRect.height);
			//Event.current.Use();  // the GUI.Button below will eat the event, and this way it will show its active state
		}
		else if (Event.current.type == EventType.mouseUp && isResizing)
		{
			isResizing = false;
		}
		else if (!Input.GetMouseButton(0))
		{
			// if the mouse is over some other window we won't get an event, this just kind of circumvents that by checking the button state directly
			isResizing = false;
		}
		else if (isResizing)
		{
			//windowRect.width = Mathf.Max(minWindowSize.x, resizeStart.width + (mouse.x - resizeStart.x));
			windowRect.height = Mathf.Max(minWindowSize.y, resizeStart.height + (mouse.y - resizeStart.y));
			//windowRect.xMax = Mathf.Min(Screen.width, windowRect.xMax);  // modifying xMax affects width, not x
			windowRect.yMax = Mathf.Min(Screen.height, windowRect.yMax);  // modifying yMax affects height, not y
		}

		return windowRect;
	}



	//--- Local version. Use the static version above instead.
	public void LogMessage(string text, string group)
	{
		LogMessage(text, group, MessageTypes.Normal, Color.white, false);
	}

	//--- Local version. Use the static version above instead.
	public void LogMessage(string text, string group, MessageTypes messageType)
	{
		LogMessage(text, group, messageType, Color.white, false);
	}

	//--- Local version. Use the static version above instead.
	public void LogMessage(string text, string group, Color displayColor)
	{
		LogMessage(text, group, MessageTypes.Normal, displayColor, true);
	}

	//--- Local version. Use the static version above instead.
	public void LogMessage(string text, string group, MessageTypes messageType, Color displayColor, bool useCustomColor)
	{
		if (string.IsNullOrEmpty(group))
			group = DEFAULT;

		if (_messages != null)
		{
			if (useCustomColor)
			{
				AddMessage(new Message()
					{
						message = text,
						group = group,
						messageType = messageType,
						displayColor = displayColor,
					});
			}
			else
			{
				AddMessage(new Message()
					{
						message = text,
						group = group,
						messageType = messageType,
					});
			}

			CheckMessageLimit(group);

			logScrollPos = new Vector2(logScrollPos.x, 50000.0F);
		}
	}

	//--- Local version. Use the static version above instead.
	public void ClearLog(string group)
	{
		if (string.IsNullOrEmpty(group))
			_messages.Clear();
		else
		{
			LinkedList<Message> list;
			if (_messages.TryGetValue(group, out list))
				list.Clear();
		}
	}

	//--- Local version. Use the static version above instead.
	public void AddWatchVarToTable(string name, WatchVar watchVar)
	{
		watchVarTable.Add(name, watchVar);
	}

	//--- Local version. Use the static version above instead.
	public void RemoveWatchVarFromTable(string name)
	{
		if (watchVarTable.ContainsKey(name))
		{
			watchVarTable.Remove(name);
		}
	}

	/// <summary>
	/// Displays the normal log.
	/// </summary>
	/// <param name="group">The group.</param>
	private void DisplayNormalLog(string group)
	{
		if( string.IsNullOrEmpty(group) )
			group = DEFAULT;

		LinkedList<Message> list;
		if (!_messages.TryGetValue(group, out list) || list.Count == 0)
			return;

		foreach (Message m in list)
		{
			// Default text color                
			Color displayColor = DefaultTextColor;

			if (m.useCustomColor)
			{
				displayColor = m.displayColor;
			}
			else
			{
				switch (m.messageType)
				{
					case MessageTypes.Error:
						displayColor = Color.red;
						break;

					case MessageTypes.Warning:
						displayColor = Color.yellow;
						break;

					case MessageTypes.System:
						displayColor = Color.green;
						break;

					case MessageTypes.Command:
						displayColor = Color.magenta;
						break;
				}
			}

			Color oldColor = GUI.color;
			GUI.color = displayColor;
			GUILayout.Label("> " + m.message);
			GUI.color = oldColor;
		}
	}

	/// <summary>
	/// Builds the display string.
	/// </summary>
	/// <param name="group">The group.</param>
	private void BuildDisplayString(string group)
	{
		if (string.IsNullOrEmpty(group))
			group = DEFAULT;

		LinkedList<Message> list;
		if (!_messages.TryGetValue(group, out list) || list.Count == 0)
			return;

		displayString = new StringBuilder();

		foreach (Message m in list)
		{
			if (string.IsNullOrEmpty(group) && m.group != group)
				continue;

			string messageTypeString = "";

			if (m.useCustomColor == false)
			{
				switch (m.messageType)
				{
					case MessageTypes.Error:
						messageTypeString = "error";
						break;

					case MessageTypes.Warning:
						messageTypeString = "warning";
						break;

					case MessageTypes.System:
						messageTypeString = "system";
						break;

					case MessageTypes.Command:
						messageTypeString = "command";
						break;
				}
			}
			else
			{
				messageTypeString = "customColor(" + m.displayColor.ToString() + ")";
			}

			if (!string.IsNullOrEmpty(messageTypeString))
			{
				displayString.AppendLine(">> [" + messageTypeString + "]" + m.message + "[/" + messageTypeString + "]");
			}
			else
			{
				displayString.AppendLine(">> " + m.message);
			}
		}
	}

	#endregion
}

/// <summary>
/// Base class for all WatchVars. Provides base functionality.
/// </summary>
public abstract class WatchVar
{
	/// <summary>
	/// Name of the WatchVar.
	/// </summary>
	public string Name
	{
		get { return _name; }
		set { _name = value; }
	}

	private string _name = "Default WatchVar";

	public WatchVar(string name)
	{
		this._name = name;
		ConsoleWindow.RegisterWatchVar(_name, this);
	}

	public abstract object GetValue();
}

/// <summary>
/// A WatchVar designed to monitor a float type variable.
/// </summary>
public class WatchFloat : WatchVar
{
	/// <summary>
	/// Gets or sets the value of this WatchFloat.
	/// </summary>
	public float Value
	{
		get { return _value; }
		set { _value = value; }
	}

	private float _value;

	public WatchFloat(string name)
		: base(name)
	{
		//...
	}

	public override object GetValue()
	{
		return (object)_value;
	}
}

/// <summary>
/// A WatchVar designed to monitor an int type variable.
/// </summary>
public class WatchInt : WatchVar
{
	/// <summary>
	/// Gets or sets the value of this WatchInt.
	/// </summary>
	public int Value
	{
		get { return _value; }
		set { _value = value; }
	}

	private int _value;

	public WatchInt(string name)
		: base(name)
	{
		//...
	}

	public override object GetValue()
	{
		return (object)_value;
	}
}

/// <summary>
/// A WatchVar designed to monitor a boolean type variable.
/// </summary>
public class WatchBool : WatchVar
{
	/// <summary>
	/// Gets or sets the value of this WatchBool.
	/// </summary>
	public bool Value
	{
		get { return _value; }
		set { _value = value; }
	}

	private bool _value;

	public WatchBool(string name)
		: base(name)
	{
		//...
	}

	public override object GetValue()
	{
		return (object)_value;
	}
}
