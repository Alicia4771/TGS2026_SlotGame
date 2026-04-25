#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

public class SlotScript : MonoBehaviour
{
    // スロットの絵柄の画像
    [SerializeField] private Sprite image_slot_7;
    [SerializeField] private Sprite image_slot_cherry;
    [SerializeField] private Sprite image_slot_bell;
    [SerializeField] private Sprite image_slot_bar;
    [SerializeField] private Sprite image_slot_replay;
    [SerializeField] private Sprite image_slot_suika;

    // スロットの絵柄が表示される場所
    [SerializeField] private SpriteRenderer slot_left;
    [SerializeField] private SpriteRenderer slot_center;
    [SerializeField] private SpriteRenderer slot_right;

    void Start()
    {
        if (
            image_slot_7 == null ||
            image_slot_cherry == null ||
            image_slot_bell == null ||
            image_slot_bar == null ||
            image_slot_replay == null ||
            image_slot_suika == null ||
            slot_left == null ||
            slot_center == null ||
            slot_right == null
        )
        {
            Debug.LogError("必要なスロット画像が設定されていません。実行を終了します。");

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            StopSlotLeft();
        } else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            StopSlotCenter();
        } else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            StopSlotRight();
        }
    }

    private void StopSlotLeft()
    {
        slot_left.sprite = image_slot_7;
    }

    private void StopSlotCenter()
    {
        // code;
    }

    private void StopSlotRight()
    {
        // code;
    }
}
