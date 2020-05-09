// ================================================================================================================================
// File:        Log.cs
// Description:	Allows printing messages to the UI message window
// Author:	    Harley Laurie https://www.github.com/Swaelo/
// ================================================================================================================================

using UnityEngine;
using UnityEngine.UI;

public class Log : MonoBehaviour
{
    //Singleton instance
    public static Log Instance = null;
    private void Awake() { Instance = this; }

    //Array of lines where each message is displayed in the message window
    public Text[] MessageLines;

    public static void Print(string Message)
    {
        //Move all the previous message back 1 line
        for (int i = 7; i > 0; i--)
            Log.Instance.MessageLines[i].text = Log.Instance.MessageLines[i - 1].text;
        //Display the new message on the first line
        Log.Instance.MessageLines[0].text = Message;
    }
}
