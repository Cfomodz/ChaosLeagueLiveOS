using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldShopEntry : ShopEntry
{
    [SerializeField] private SpriteRenderer _bubbleContainer;
    [SerializeField] private TextMeshPro _bubbleText;
    [SerializeField] private LineRenderer _bubbleStalk;

    [SerializeField] public List<string> BuyCommands;

    private Color _purchaseColor;
    private int valuechoice;

    public void InitEntry(int tier, bool isGolden, float goldenDiscount)
    {

        Color color;
        int randomPrice; 
        if(tier == 1)
        {
            color = Color.HSVToRGB(0f, 1f, 0f); //dark 
            randomPrice = 1_000;
            valuechoice = 100000;
        }
        else if(tier == 2)
        {
            color = Color.HSVToRGB(0f, 1f, 0f); //dark 
            randomPrice = 5_000;
            valuechoice = 500000;
        }
        else
        {
            color = Color.HSVToRGB(0f, 1f, 0f); //dark 
            randomPrice = 10_000;
            valuechoice = 1000000;
        }


        if (isGolden)
            randomPrice = Mathf.RoundToInt(randomPrice * (1 - goldenDiscount));

        InitEntryBase(randomPrice, BuyCommands);

        _bubbleText.color = color;
        _purchaseColor = color;

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

        pb.Ph.pp.ShieldValue += valuechoice;

        StartCoroutine(PurchaseAnimation(pb));

        AudioController.inst.PlaySound(AudioController.inst.StorePurchase, 0.95f, 1.05f);
    }
}
