using UnityEngine;

public class SiteButton : MonoBehaviour
{ 
    public static void SendToSite()
    {
        Application.OpenURL("https://polimi-game-collective.itch.io/feed-me-quack");
    }
    
}