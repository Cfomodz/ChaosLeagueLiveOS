using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingBackgroundShopEntry : ShopEntry
{
    [SerializeField] private Crown _crown; 
    [SerializeField] public List<string> BuyCommands;

    private List<Color> colors = new List<Color>();

    private int TotalBGOne = 48;
    private int TotalBGTwo = 98;
    private int TotalBGThree = 98;
    private int AnimationMode = 0;

    bool PlusUp = true;
    float PlusAnimation = 0;

    public void InitEntry(int tier)
    {
        InitEntryBase(GoldCost, BuyCommands);

        BackgroundCustomizations(SapphireCost);
    }

    public override void ReceivePlayer(PlayerBall pb)
    {
        AttemptPurchase(pb);
    }

    public void BackgroundCustomizations(int tier)
    {
        int Txtr1 = UnityEngine.Random.Range(0, TotalBGOne);
        int Txtr2 = UnityEngine.Random.Range(0, TotalBGTwo);
        int Txtr3 = UnityEngine.Random.Range(0, TotalBGThree);

        Debug.LogWarning($"{tier}");

        //If T1
        if (tier == 1)
        {
            Debug.LogWarning("Backgrounds T1");
            var BaseMaterials = DisplayBackground.materials;
            var DesiredMaterials = _crown.EnhancedMaterials;
            BaseMaterials[0] = DesiredMaterials[Txtr1];
            DisplayBackground.materials = BaseMaterials;
            EmeraldCost = Txtr1;
            return;
        }
        //If T2
        else if (tier == 2)
        {
            Debug.LogWarning("Backgrounds T2");
            var BaseMaterials = DisplayBackground.materials;
            var DesiredMaterials = _crown.EnhancedMaterials;
            BaseMaterials[0] = DesiredMaterials[Txtr2];
            DisplayBackground.materials = BaseMaterials;
            EmeraldCost = Txtr2;
            return;
        }
        //If T3
        else if (tier >= 3)
        {
            Debug.LogWarning("Backgrounds T3");
            var BaseMaterials = DisplayBackground.materials;
            var DesiredMaterials = _crown.EnhancedMaterials;
            BaseMaterials[0] = DesiredMaterials[Txtr3];
            DisplayBackground.materials = BaseMaterials;
            EmeraldCost = Txtr3;
            return;
        }
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

        pb.Ph.pp.KingBGTier = SapphireCost;
        pb.Ph.pp.KingBG = EmeraldCost;

        if (SapphireCost > 2)
            AnimationMode = 1;
        else
            AnimationMode = 0;

        if (pb.Ph.IsKing())
            pb.Ph.ReloadKingCosmetics(AnimationMode);

        StartCoroutine(PurchaseAnimation(pb));

        AudioController.inst.PlaySound(AudioController.inst.StorePurchase, 0.95f, 1.05f);
    }

    private void Update()
    {
        var BaseMaterials = DisplayBackground.materials;

        if (SapphireCost == 3)
        {
            BaseMaterials[0].SetFloat("Vector1_4200F1D7", PlusAnimation);

            if (PlusUp)
                PlusAnimation += 0.001f;
            else
                PlusAnimation -= 0.001f;

            if (PlusAnimation > 1.499f)
                PlusUp = false;

            if (PlusAnimation < 0.0001f)
                PlusUp = true;

            DisplayBackground.materials = BaseMaterials;
        }
    }
}
