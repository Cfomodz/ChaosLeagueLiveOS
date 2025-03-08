using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class VoiceShopEntry : ShopEntry
{
    [SerializeField] private SpriteRenderer _bubbleContainer;
    [SerializeField] private TextMeshPro _bubbleText;
    [SerializeField] private LineRenderer _bubbleStalk;
    [SerializeField] private int voiceID;

    [SerializeField] public List<string> BuyCommands;

    public int VoiceIndex;

    public void InitEntry(int tier, bool isGolden, float goldenDiscount)
    {
        int VoiceID;
        int randomPrice; 

        if(tier == 1)
        {
            VoiceID = UnityEngine.Random.Range(2, 15);
            randomPrice = 30000;
        }
        else if(tier == 2)
        {
            VoiceID = UnityEngine.Random.Range(15, 36);
            randomPrice = 300000;
        }
        else
        {
            VoiceID = UnityEngine.Random.Range(36, 60);
            randomPrice = 3000000;
        }

        _bubbleText.SetText(GetName(VoiceID));

        if (isGolden)
            randomPrice = Mathf.RoundToInt(randomPrice * (1 - goldenDiscount));

        InitEntryBase(randomPrice, BuyCommands);

       VoiceIndex = VoiceID;

    }

    public string GetName(int VoiceIndex)
    {
        string SpeakerName = "";

        switch (VoiceIndex)
        {
            default: //Default Male Name 0
                SpeakerName = "Joey";
                break;
            case 1: //Default Female Name 1
                SpeakerName = "Salli";
                break;
            case 2: // Beginning of Tier 1 Names 2-14
                SpeakerName = "Emma";
                break;
            case 3:
                SpeakerName = "Nicole";
                break;
            case 4:
                SpeakerName = "Russell";
                break;
            case 5:
                SpeakerName = "Amy";
                break;
            case 6:
                SpeakerName = "Brian";
                break;
            case 7:
                SpeakerName = "Aditi";
                break;
            case 8:
                SpeakerName = "Raveena";
                break;
            case 9:
                SpeakerName = "Ivy";
                break;
            case 10:
                SpeakerName = "Joanna";
                break;
            case 11:
                SpeakerName = "Kendra";
                break;
            case 12:
                SpeakerName = "Kimberly";
                break;
            case 13:
                SpeakerName = "Kevin";
                break;
            case 14:
                SpeakerName = "Geraint";
                break; // End of Tier 1 Names 2-14
            case 15: // Beginning of Tier 2 Names 15-35
                SpeakerName = "Celine";
                break;
            case 16:
                SpeakerName = "Lea";
                break;
            case 17:
                SpeakerName = "Mathieu";
                break;
            case 18:
                SpeakerName = "Chantal";
                break;
            case 19:
                SpeakerName = "Marlene";
                break;
            case 20:
                SpeakerName = "Vicki";
                break;
            case 21:
                SpeakerName = "Hans";
                break;
            case 22:
                SpeakerName = "Carla";
                break;
            case 23:
                SpeakerName = "Bianca";
                break;
            case 24:
                SpeakerName = "Giorgio";
                break;
            case 25:
                SpeakerName = "Camila";
                break;
            case 26:
                SpeakerName = "Vitoria";
                break;
            case 27:
                SpeakerName = "Ricardo";
                break;
            case 28:
                SpeakerName = "Ines";
                break;
            case 29:
                SpeakerName = "Cristiano";
                break;
            case 30:
                SpeakerName = "Conchita";
                break;
            case 31:
                SpeakerName = "Lucia";
                break;
            case 32:
                SpeakerName = "Enrique";
                break;
            case 33:
                SpeakerName = "Mia";
                break;
            case 34:
                SpeakerName = "Lupe";
                break;
            case 35:
                SpeakerName = "Penelope";
                break;
            case 36:
                SpeakerName = "Miguel";
                break; // End of Tier 2 Names 15-35
            case 37: // End of Tier 2 Names 36-59
                SpeakerName = "Zeina";
                break;
            case 38:
                SpeakerName = "Zhiyu";
                break;
            case 39:
                SpeakerName = "Naja";
                break;
            case 40:
                SpeakerName = "Mads";
                break;
            case 41:
                SpeakerName = "Lotte";
                break;
            case 42:
                SpeakerName = "Ruben";
                break;
            case 43:
                SpeakerName = "Aditi";
                break;
            case 44:
                SpeakerName = "Dora";
                break;
            case 45:
                SpeakerName = "Karl";
                break;
            case 46:
                SpeakerName = "Mizuki";
                break;
            case 47:
                SpeakerName = "Takumi";
                break;
            case 48:
                SpeakerName = "Seoyeon";
                break;
            case 49:
                SpeakerName = "Liv";
                break;
            case 50:
                SpeakerName = "Ewa";
                break;
            case 51:
                SpeakerName = "Maja";
                break;
            case 52:
                SpeakerName = "Jacek";
                break;
            case 53:
                SpeakerName = "Jan";
                break;
            case 54:
                SpeakerName = "Carmen";
                break;
            case 55:
                SpeakerName = "Tatyana";
                break;
            case 56:
                SpeakerName = "Maxim";
                break;
            case 57:
                SpeakerName = "Astrid";
                break;
            case 58:
                SpeakerName = "Filiz";
                break;
            case 59:
                SpeakerName = "Gwyneth";
                break; // End of Tier 2 Names 36-59               
        }

        return SpeakerName;
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

        pb.Ph.pp.VoiceID = VoiceIndex;

        StartCoroutine(PurchaseAnimation(pb));

        AudioController.inst.PlaySound(AudioController.inst.StorePurchase, 0.95f, 1.05f);
    }
}
