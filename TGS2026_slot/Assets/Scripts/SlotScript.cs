#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

public class SlotScript : MonoBehaviour
{
    // スロットの絵柄の画像
    [SerializeField] private Sprite image_slot_7;
    [SerializeField] private Sprite image_slot_bar;
    [SerializeField] private Sprite image_slot_replay;
    [SerializeField] private Sprite image_slot_cherry;
    [SerializeField] private Sprite image_slot_bell;
    [SerializeField] private Sprite image_slot_suika;

    // スロットの絵柄が表示される場所
    [SerializeField] private SpriteRenderer slot_left_up;
    [SerializeField] private SpriteRenderer slot_left_center;
    [SerializeField] private SpriteRenderer slot_left_down;
    [SerializeField] private SpriteRenderer slot_center_up;
    [SerializeField] private SpriteRenderer slot_center_center;
    [SerializeField] private SpriteRenderer slot_center_down;
    [SerializeField] private SpriteRenderer slot_right_up;
    [SerializeField] private SpriteRenderer slot_right_center;
    [SerializeField] private SpriteRenderer slot_right_down;

    // 表示されるスロットの絵柄のサイズ
    private float frameWidth = 3.6f;
    private float frameHeight = 2.7f;

    // スロットプレイ中かどうかのフラグ
    private bool isSlotPlaying = false;
    // スロットの各列が回転中がどうかのフラグ
    private bool isLeftSlotSpinning = false;
    private bool isCenterSlotSpinning = false;
    private bool isRightSlotSpinning = false;

    // ハズレが続いた回数
    private int slot_unhit_count = 0;
    [SerializeField, Tooltip("最大何回以内に惜しい盤面を出すか")] private int slot_hit_count_max = 10;
    [SerializeField] private int slot_7_weight = 1;
    [SerializeField] private int slot_bar_weight = 1;
    [SerializeField] private int slot_replay_weight = 3;
    [SerializeField] private int slot_cherry_weight = 8;
    [SerializeField] private int slot_bell_weight = 5;
    [SerializeField] private int slot_suika_weight = 5;

    // スロットの各列にどの絵柄が表示されるかを表す変数（0: 未定, 1: 7, 2: bar, 3: replay, 4: cherry, 5: bell, 6: suika）
    private int slot_left_result = 0;
    private int slot_center_result = 0;
    private int slot_right_result = 0;

    void Start()
    {
        // 必要なスロットの枠と絵柄の画像が設定されているかの確認
        if (
            image_slot_7 == null ||
            image_slot_cherry == null ||
            image_slot_bell == null ||
            image_slot_bar == null ||
            image_slot_replay == null ||
            image_slot_suika == null ||
            slot_left_up == null ||
            slot_left_center == null ||
            slot_left_down == null ||
            slot_center_up == null ||
            slot_center_center == null ||
            slot_center_down == null ||
            slot_right_up == null ||
            slot_right_center == null ||
            slot_right_down == null
        )
        {
            Debug.LogError("必要なスロット画像が設定されていません。実行を終了します。");

#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // スロットの絵柄の画像のサイズが全て同じであるかの確認
        float imageWidth = image_slot_7.bounds.size.x;
        float imageHeight = image_slot_7.bounds.size.y;
        if (Mathf.Abs(image_slot_cherry.bounds.size.x - imageWidth) > 0 ||
            Mathf.Abs(image_slot_cherry.bounds.size.y - imageHeight) > 0 ||
            Mathf.Abs(image_slot_bell.bounds.size.x - imageWidth) > 0 ||
            Mathf.Abs(image_slot_bell.bounds.size.y - imageHeight) > 0 ||
            Mathf.Abs(image_slot_bar.bounds.size.x - imageWidth) > 0 ||
            Mathf.Abs(image_slot_bar.bounds.size.y - imageHeight) > 0 ||
            Mathf.Abs(image_slot_replay.bounds.size.x - imageWidth) > 0 ||
            Mathf.Abs(image_slot_replay.bounds.size.y - imageHeight) > 0 ||
            Mathf.Abs(image_slot_suika.bounds.size.x - imageWidth) > 0 ||
            Mathf.Abs(image_slot_suika.bounds.size.y - imageHeight) > 0)
        {
            Debug.LogError("スロットの絵柄の画像のサイズが全て同じではありません。実行を終了します。");
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // 設定されているスロットの枠が4:3になっているかの確認
        if (Mathf.Abs((frameWidth / frameHeight) - (4.0f / 3.0f)) > 0.01f)
        {
            Debug.LogError("スロットの枠のサイズが4:3になっていません。実行を終了します。");
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // スロットの枠のサイズを設定
        float scaleX = frameWidth / imageWidth;
        float scaleY = frameHeight / imageHeight;
        slot_left_up.transform.localScale = new Vector3(scaleX, scaleY, 1);
        slot_left_center.transform.localScale = new Vector3(scaleX, scaleY, 1);
        slot_left_down.transform.localScale = new Vector3(scaleX, scaleY, 1);
        slot_center_up.transform.localScale = new Vector3(scaleX, scaleY, 1);
        slot_center_center.transform.localScale = new Vector3(scaleX, scaleY, 1);
        slot_center_down.transform.localScale = new Vector3(scaleX, scaleY, 1);
        slot_right_up.transform.localScale = new Vector3(scaleX, scaleY, 1);
        slot_right_center.transform.localScale = new Vector3(scaleX, scaleY, 1);
        slot_right_down.transform.localScale = new Vector3(scaleX, scaleY, 1);

        // スロットの絵柄を初期化
        slot_left_up.sprite = null;
        slot_left_center.sprite = null;
        slot_left_down.sprite = null;
        slot_center_up.sprite = null;
        slot_center_center.sprite = null;
        slot_center_down.sprite = null;
        slot_right_up.sprite = null;
        slot_right_center.sprite = null;
        slot_right_down.sprite = null;

        // スロットプレイ中のフラグを初期化
        isSlotPlaying = false;

        // スロットの各列が回転中かどうかのフラグを初期化
        isLeftSlotSpinning = false;
        isCenterSlotSpinning = false;
        isRightSlotSpinning = false;
    }

    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            StopSlot(0);
        } else if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            StopSlot(1);
        } else if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            StopSlot(2);
        } else  if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            SlotPlay();
        }
    }


    private void StopSlot(int slotNumber)
    {
        if (isSlotPlaying)
        {
            switch (slotNumber)
            {
                case 0:
                    if (isLeftSlotSpinning) {
                        if (slot_unhit_count >= slot_hit_count_max)
                        {
                            slot_left_result = 1;
                        } else
                        {
                            slot_right_result = GetSlotResult();
                        }
                        slot_left_center.sprite = ReturnSlotSprite(slot_left_result);
                        isLeftSlotSpinning = false;
                    }
                    break;
                case 1:
                    if (isCenterSlotSpinning) {
                        if (slot_unhit_count >= slot_hit_count_max)
                        {
                            slot_left_result = 1;
                        } else
                        {
                            slot_right_result = GetSlotResult();
                        }
                        slot_center_center.sprite = ReturnSlotSprite(slot_center_result);
                        isCenterSlotSpinning = false;
                    }
                    break;
                case 2:
                    if (isRightSlotSpinning) { 
                        if (slot_unhit_count >= slot_hit_count_max)
                        {
                            slot_left_result = 1;
                        } else
                        {
                            slot_right_result = GetSlotResult();
                        }
                        slot_right_center.sprite = ReturnSlotSprite(slot_right_result);
                        isRightSlotSpinning = false;
                    }
                    break;
                default:
                    Debug.LogError("無効なスロット番号: " + slotNumber);
                    break;
            }

            if (!isLeftSlotSpinning && !isCenterSlotSpinning && !isRightSlotSpinning)
            {
                slot_unhit_count++;
                
                isSlotPlaying = false;
                Debug.Log(slot_left_result + ", " + slot_center_result + ", " + slot_right_result);
            }
        }
    }

    private int GetSlotResult()
    {
        int result_numer = 0;
        int result_random_number = Random.Range(1, slot_7_weight + slot_bar_weight + slot_replay_weight + slot_cherry_weight + slot_bell_weight + slot_suika_weight + 1);
        Debug.Log("result_random_number: " + result_random_number);
        if (result_random_number <= slot_7_weight)
        {
            result_numer = 1;
        } else if (result_random_number <= slot_7_weight + slot_bar_weight)
        {
            result_numer = 2;
        } else if (result_random_number <= slot_7_weight + slot_bar_weight + slot_replay_weight)
        {
            result_numer = 3;
        } else if (result_random_number <= slot_7_weight + slot_bar_weight + slot_replay_weight + slot_cherry_weight)
        {
            result_numer = 4;
        } else if (result_random_number <= slot_7_weight + slot_bar_weight + slot_replay_weight + slot_cherry_weight + slot_bell_weight)
        {
            result_numer = 5;
        } else if (result_random_number <= slot_7_weight + slot_bar_weight + slot_replay_weight + slot_cherry_weight + slot_bell_weight + slot_suika_weight)
        {
            result_numer = 6;
        }
        Debug.Log("result_numer: " + result_numer);
        return result_numer;
    }

    private Sprite ReturnSlotSprite(int slotResultNum)
    {
        switch (slotResultNum)
        {
            case 1:
                return image_slot_7;
            case 2:
                return image_slot_bar;
            case 3:
                return image_slot_replay;
            case 4:
                return image_slot_cherry;
            case 5:
                return image_slot_bell;
            case 6:
                return image_slot_suika;
            default:
                Debug.LogError("無効なスロット結果番号: " + slotResultNum);
                return null;
        }
    }

    private void SlotPlay()
    {
        if (isSlotPlaying)
        {
            // すでにスロットプレイ中の場合は何もしない
            return;
        }

        slot_left_up.sprite = null;
        slot_left_center.sprite = null;
        slot_left_down.sprite = null;
        slot_center_up.sprite = null;
        slot_center_center.sprite = null;
        slot_center_down.sprite = null;
        slot_right_up.sprite = null;
        slot_right_center.sprite = null;
        slot_right_down.sprite = null;

        // スロットプレイ中のフラグを設定
        isSlotPlaying = true;
        // スロットの各列が回転中のフラグを設定
        isLeftSlotSpinning = true;
        isCenterSlotSpinning = true;
        isRightSlotSpinning = true;
        // スロットの各列の結果を初期化
        slot_left_result = 0;
        slot_center_result = 0;
        slot_right_result = 0;
    }
}
