using UnityEngine;
using UnityEngine.UI;

public class PlayerBlockPipUI : MonoBehaviour
{
    [Header("References")]
    public PlayerBlock playerBlock;
    public GameObject rootObject;

    [Header("Pips")]
    public Image pip1;
    public Image pip2;
    public Image pip3;

    void Update()
    {
        if (playerBlock == null || rootObject == null || pip1 == null || pip2 == null || pip3 == null)
            return;

        bool show = playerBlock.IsBlocking();
        rootObject.SetActive(show);

        if (!show)
            return;

        int hp = playerBlock.GetCurrentBlockHp();

        pip1.enabled = hp >= 1;
        pip2.enabled = hp >= 2;
        pip3.enabled = hp >= 3;
    }
}