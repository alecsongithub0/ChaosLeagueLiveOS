using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BubbleColorShopEntry : ShopEntry
{
    [SerializeField] private SpriteRenderer _bubbleContainer;
    [SerializeField] private TextMeshPro _bubbleText;
    [SerializeField] private LineRenderer _bubbleStalk; 

    [SerializeField] public List<string> BuyCommands;

    private Color _purchaseColor;

    public void InitEntry(int tier, bool isGolden, float goldenDiscount)
    {

        Color color;
        int randomPrice;
        if (tier == 1)
        {
            color = Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 0.5f)); //dark 
            randomPrice = Mathf.RoundToInt(Random.Range(3, 7) * 1000);
        }
        else if (tier == 2)
        {
            color = Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f)); //can be anything
            randomPrice = Mathf.RoundToInt(Random.Range(7, 20) * 1000);
        }
        else
        {
            color = Color.HSVToRGB(Random.Range(0f, 1f), Random.Range(0.9f, 1f), Random.Range(0.9f, 1f)); //bright colors only
            randomPrice = Mathf.RoundToInt(Random.Range(20, 50) * 1000);
        }

        if (isGolden)
            randomPrice = Mathf.RoundToInt(randomPrice * (1 - goldenDiscount));

        InitEntryBase(randomPrice, BuyCommands);

        color = MyUtil.SetColorSaveAlpha(color, _bubbleContainer.color);

        _purchaseColor = color;

        _bubbleContainer.color = color;
        _bubbleText.SetText("");

        color.a = 0;
        _bubbleStalk.startColor = color;
        color.a = 1; 
        _bubbleStalk.endColor = color;
    }

    public override void ReceivePlayer(PlayerBall pb)
    {
        AttemptPurchase(pb);
    }

    public void AttemptPurchase(PlayerBall pb)
    {
        if (pb.Ph.pp.Gold < GoldCost)
        {
            Debug.Log("Player doesn't have enough gold");
            return;
        }

        pb.Ph.pp.Gold -= GoldCost;
        //Gold subtraction popup
        TextPopupMaster.Inst.CreateTextPopup(pb.GetPosition(), Vector3.up, $"-{MyUtil.AbbreviateNum4Char(GoldCost)} GOLD", MyColors.Gold);

        string hexString = MyUtil.ColorToHexString(_purchaseColor);
        pb.Ph.pp.SpeechBubbleFillHex = hexString;

        pb.Ph.SetCustomizationsFromPP();

        StartCoroutine(PurchaseAnimation(pb));

        AudioController.inst.PlaySound(AudioController.inst.StorePurchase, 0.95f, 1.05f);
    }
}
