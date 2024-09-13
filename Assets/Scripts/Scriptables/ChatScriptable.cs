using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChatData", menuName = "Data/ChatData")]
public class ChatScriptable : ScriptableObject
{
    public Chat[] chats;
}

[System.Serializable]
public class Chat
{
    public string ID;
    public AudioClip Clip;
}
