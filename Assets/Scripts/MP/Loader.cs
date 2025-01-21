using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader {


    public enum Scene {
        MainMenuScene,
        GameScene,
        LoadingScene,
        LobbyScene,
        CharacterSelectScene,
    }


    private static Scene targetScene;

    internal static string[] Gamescenes = new string[] { "Sk_park_tdm" }; //{ "Playground1", "Playground2", "Playground3", "Playground4" };


    public static void Load(Scene targetScene) {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }
    public static void Load(int i) {

        SceneManager.LoadScene(i);
    }

    [ServerRpc (RequireOwnership =false)]
    public static void LoadNetworkServerRpc(string targetScene) {

        NetworkManager.Singleton.SceneManager.LoadScene(targetScene, LoadSceneMode.Single);
    }

    public static void LoaderCallback() {
        SceneManager.LoadScene(targetScene.ToString());
    }

}