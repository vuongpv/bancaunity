using UnityEngine;

public class UIGoldPackDragDropItem : MonoBehaviour
{
    public UISprite sprite;
    public UILabel coinLabel;

    public Vector3 simpleModePos;

    Transform mTrans;
    Transform mParent;

    UIRoot root;
    float pixelAdjustment;
    
    Vector3 origPos;

    FHPlayerMultiController player;
    UIShareGoldPacksMenu container;
    int coinValue;

    public void Setup(FHPlayerMultiController _player, UIShareGoldPacksMenu _container, int _coinValue)
    {
        player = _player;
        container = _container;
        coinValue = _coinValue;

        if (coinValue <= player.gold)
        {
            collider.enabled = true;
            sprite.spriteName = "multi_coinpack";
        }
        else
        {
            collider.enabled = false;
            sprite.spriteName = "multi_coinpack_dis";
        }

        sprite.MakePixelPerfect();

        coinLabel.text = coinValue.ToString();
    }

    void Awake()
    {
        mTrans = transform;

        player = null;
        
        UIRoot root = NGUITools.FindInParents<UIRoot>(GuiManager.instance.gameObject);
        pixelAdjustment = root.GetPixelSizeAdjustment(Screen.height);
    }

    void OnDrag(Vector2 delta)
    {
        if (FHMultiPlayerManager.instance.sharingCoinObj != gameObject)
            return;

        mTrans.localPosition += (Vector3)delta * pixelAdjustment;
        FHTouchZoneManager.instance.CheckHover();
    }

    void OnPress(bool isPressed)
    {
        if (isPressed)
        {
            if (FHMultiPlayerManager.instance.sharingCoinObj != null)
                return;

            FHMultiPlayerManager.instance.sharingCoinObj = gameObject;

            container.DisableAllColliders();
            FHTouchZoneManager.instance.EnableColliders(player);

            mParent = mTrans.parent;
            origPos = mTrans.localPosition;

			mTrans.parent = UIDragDropRoot.root;
            Vector3 pos = mTrans.localPosition;
            pos.z = 0f;
            mTrans.localPosition = pos;

            NGUITools.MarkParentAsChanged(gameObject);
        }
        else
        {
            FHMultiPlayerManager.instance.ShareCoin(player, UICamera.hoveredObject, mTrans.position, coinValue);

            mTrans.parent = mParent;
            mTrans.localPosition = origPos;
            NGUITools.MarkParentAsChanged(gameObject);

            container.Reload();
            FHTouchZoneManager.instance.Reset();

            FHMultiPlayerManager.instance.sharingCoinObj = null;
        }
    }
}