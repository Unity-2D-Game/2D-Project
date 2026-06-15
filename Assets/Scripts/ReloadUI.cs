using UnityEngine;
using TMPro;

public class ReloadUI : MonoBehaviour
{
    public GunScript gun;
    public TextMeshProUGUI reloadText;

    void Update()
    {
        if (Time.time < gun.NextFireTime)
        {
            reloadText.text = "RELOADING...";
        }
        else
        {
            reloadText.text = "";
        }
    }
}
