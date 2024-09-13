using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeirdBrothers.ThirdPersonController;
using TMPro;

public class ChatScript : MonoBehaviour
{
    [SerializeField] ChatScriptable chatData;
    [SerializeField] GameObject ButtonPrefab;
    [SerializeField] Transform Master;
    [SerializeField] GameObject Chats;
    [SerializeField] TMP_InputField ChatInput;
    

    [Space(5)]
    [Header("Message Panel")]
    public GameObject MessageMaster;
    public Messagerow[] MessageRows;
    public int index = 0;
    


    private void Start()
    {
        SetData();
        WBUIActions.SetMessage += ShowMessage;
    }

    public void SendChatInput()
    {
        string message = ChatInput.text;
        ChatInput.text = "";
        WBUIActions.SendChat?.Invoke(message);
        ChatInput.gameObject.SetActive(false);
    }

    private void SetData()
    {
        SetRaw(ButtonPrefab, chatData.chats[0]);
        ButtonPrefab.SetActive(true);
        for (int i = 1; i < chatData.chats.Length; i++)
        {
            SetRaw(Instantiate(ButtonPrefab,Master),chatData.chats[i]);
        }

    }

    void SetRaw(GameObject Object,Chat chat)
    {
        Object.GetComponent<Button>().onClick.AddListener(() => { 
            WBUIActions.PlayClip?.Invoke(chat);
            Chats.SetActive(false);
        });
        Object.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = chat.ID;
    }

    public void OpenChatPanel()
    {
        Chats.SetActive(!Chats.activeSelf);
    }
    public void OpenChatInput()
    {
        ChatInput.gameObject.SetActive(!ChatInput.gameObject.activeSelf);
    }

    public void ShowMessage(string message,string sender)
    {
        MessageMaster.SetActive(true);
        MessageRows[index].SetMessage(message, sender);
        var t = index;
        StartCoroutine(MessageRows[t].DisableBox());
        CancelInvoke(nameof(CloseMasterBox));
        Invoke(nameof(CloseMasterBox),1.5f);
        index++;
        if (index >= MessageRows.Length) index = 0;
    }


    void CloseMasterBox()
    {
        MessageMaster.SetActive(false);
    }



    private void OnDestroy()
    {
        WBUIActions.SetMessage -= ShowMessage;
    }

}

[System.Serializable]
public class Messagerow
{
    public GameObject Row;
    public TMP_Text Name, Message;

    internal void SetMessage(string message, string sender)
    {
        Row.SetActive(true);
        Name.text = sender;
        Message.text = message;
        
    }

    internal IEnumerator DisableBox()
    {
        yield return new WaitForSeconds(1.5f);
        Row.SetActive(false);
    }

}
