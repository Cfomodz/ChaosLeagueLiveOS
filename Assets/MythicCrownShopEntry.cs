using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MythicCrownShopEntry : ShopEntry
{
    [SerializeField] private Crown _crown; 
    [SerializeField] public List<string> BuyCommands;

    private int CrownTier = 0;

    private List<Color> colors = new List<Color>();
    public void InitEntry(int tier, int goldCost)
    {
        InitEntryBase(goldCost, BuyCommands);
        CrownTier = tier;
        _crown.EnhancedCustomizations(tier); 
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

        string json = CrownSerializer.GetJSONFromColorList(colors); 
        Debug.Log($"Setting {pb.Ph.pp.TwitchUsername} player JSON: {json}");
        pb.Ph.pp.CrownJSON = json;
        
        pb.Ph.pp.CrownTexture1 = _crown.Texture1;
        pb.Ph.pp.CrownTexture2 = _crown.Texture2;
        pb.Ph.pp.EnhancedCrown = _crown.Enhanced;
        pb.Ph.pp.CrownTier = CrownTier;
        //pb.Ph.pp.CrownTexture2 = _crown.Texture3;
        //pb.Ph.pp.CrownTexture2 = _crown.Texture4;
        //pb.Ph.pp.CrownTexture2 = _crown.Texture5;
        //pb.Ph.pp.CrownTexture2 = _crown.Texture6;

        StartCoroutine(PurchaseAnimation(pb));

        if (pb.Ph.IsKing())
            pb.Ph.ReloadKingCosmetics(71717);

        AudioController.inst.PlaySound(AudioController.inst.StorePurchase, 0.95f, 1.05f);
    }

    private void Update()
    {
        //Don't spin while game isn't active to avoid drift
        //if(_buyCommandText.gameObject.activeSelf)
        _crown.transform.RotateAround(_crown.transform.position, _crown.transform.forward, 0.05f);
    }

}
