using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomProperties : MonoBehaviour
{
    public static CustomProperties Instance;

    public bool isRed = false;
    public string MyCode;
    internal string playerName;
    internal int kills;
    internal float currentHealth;
    internal int currentAmmo;
    private const string alphanumericCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    public Vector3 LastPositon { get; internal set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            MyCode= GenerateRandomString(18);
        }
        else
        {
            Destroy(this);
        }
    }

    private string GenerateRandomString(int length)
    {
        char[] code = new char[length];

        for (int i = 0; i < length; i++)
        {
            code[i] = alphanumericCharacters[Random.Range(0, alphanumericCharacters.Length)];
        }

        return new string(code);
    }
}
