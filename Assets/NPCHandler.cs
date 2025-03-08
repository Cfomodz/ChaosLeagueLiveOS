using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.Users.GetUsers;
using TwitchLib.Client.Models;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using TMPro;

public class NPCHandler : MonoBehaviour
{
    [SerializeField] private GameManager _gm;
    [SerializeField] private TwitchClient _twitchClient;
    [SerializeField] private TwitchApi _twitchAPI;
    [SerializeField] private TwitchPubSub _twitchPubSub;
    [SerializeField] private GoldDistributor _goldDistributor;
    [SerializeField] private InvitePromo _invitePromo;
    [SerializeField] public BidHandler _bidHandler;

    [SerializeField] public MeshRenderer _KingTexture;
    [SerializeField] public GameTile _KingTile;

    [SerializeField] private TileController _tileController;
    [SerializeField] private MyHttpClient _myHttpClient;
    [SerializeField] private KingController _kingController;
    [SerializeField] private RebellionController _rebellionController;
    [SerializeField] private SQLiteServiceAsync _sqliteServiceAsync; 
    [SerializeField] private UnitTesting _unitTesting; 
    [SerializeField] private AutoNgrokService _ang; 

    [SerializeField] public Texture DefaultPFP;

    public int EventCountdown = 5;
    public int RefreshCountdown = 250;
    public int NPC = 0;
    public bool firstNPC = true;
    public int RepeaterCount1 = 0;
    public int RepeaterCount2 = 0;
    public int UpgraderCount1 = 0;
    public int UpgraderCount2 = 0;
    public int GoldCount1 = 0;
    public int GoldCount2 = 0;
    public int RubyCount1 = 0;
    public int RubyCount2 = 0;
    public int CurseCount1 = 0;
    public int CurseCount2 = 0;
    public int MysteryCount1 = 0;
    public int MysteryCount2 = 0;
    public int NullCount1 = 0;
    public int NullCount2 = 0;

    private int maxNPC = 154;
    private int maxMode = 23;

    void Awake()
    {
        
    }

    public void Update()
    {

    }

    public void TestNPCs()
    {
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC)); 
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC)); 
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC)); 
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC)); 
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
        InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
    }

    public IEnumerator CheckWaiter()
    {
        _gm.PauseForEffect = true;
        yield return new WaitForSeconds(1);
        _gm.PauseForEffect = false;
        CheckCountdown();
    }


    public void ReignsCombo(int biscuits)
    {
        Debug.LogWarning($"{biscuits}");
        
        string[] keys = _gm.PlayerHandlers.Keys.ToArray();
        foreach (string key in keys)
        {
            if (_gm.PlayerHandlers[key].IsKing())
            {
                MyTTS.inst.PlayerSpeech($"I've reached {biscuits} reigns!", _gm.PlayerHandlers[key].pp.VoiceID);
                if (biscuits >= 500)
                    StartCoroutine(_gm._twitchPubSub._rebellionController.CreateRoyalCelebration(_gm.PlayerHandlers[key], biscuits, ""));
            }
            else
            {
                if (biscuits >= 200)
                    StartCoroutine(_gm._twitchPubSub._rebellionController.CreateRebellion(_gm.PlayerHandlers[key], biscuits, ""));
            }

            
        }
    }

    public void CheckCountdown()
    {
        
        CheckNPCs();
        if (firstNPC == true)
        {
            InitializeNPCs(0);
            NPC = UnityEngine.Random.Range(0, maxNPC);
            firstNPC = false;
        }

        if (RefreshCountdown < 1)
        {
            InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
            InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
            InitializeNPCs(UnityEngine.Random.Range(0, maxNPC));
            _gm._twitchAPI.AskForBotAuthorization();
            StartCoroutine(_ang.RestartNgrokTunnel());
            RefreshCountdown = 250;
        }

        if (EventCountdown < 1)
        {                
            InitializeNPCs(NPC);
            NPC = UnityEngine.Random.Range(1, maxNPC);
            EventCountdown = UnityEngine.Random.Range(17, 50);
            if (AppConfig.Monday)
                EventCountdown = 17;
        }

        Debug.Log($"{EventCountdown}");
        EventCountdown -= 1;
        RefreshCountdown -= 1;

        if (_kingController.TollRate > 10)
            _kingController.TollRate -= 1;

    }

    public void InitializeNPCs(int ID)
    {
        switch (ID)
        {
            case 0:
                _unitTesting.NPCReward("GameMaster", "GameMaster", "bid", 1);
                StartCoroutine(UpdateWaiter("GameMaster"));
                break;
            case 1:
                _unitTesting.NPCReward("TinyDefender", "TinyDefender", "bid", 1);
                StartCoroutine(UpdateWaiter("TinyDefender"));
                break;
            case 2:
                _unitTesting.NPCReward("SmallDefender", "SmallDefender", "bid", 1);
                StartCoroutine(UpdateWaiter("SmallDefender"));
                break;
            case 3:
                _unitTesting.NPCReward("LittleDefender", "LittleDefender", "bid", 1);
                StartCoroutine(UpdateWaiter("LittleDefender"));
                break;
            case 4:
                _unitTesting.NPCReward("MinorDefender", "MinorDefender", "bid", 1);
                StartCoroutine(UpdateWaiter("MinorDefender"));
                break;
            case 5:
                _unitTesting.NPCReward("ModerateDefender", "ModerateDefender", "bid", 1);
                StartCoroutine(UpdateWaiter("ModerateDefender"));
                break;
            case 6:
                _unitTesting.NPCReward("AverageDefender", "AverageDefender", "bid", 1);
                StartCoroutine(UpdateWaiter("AverageDefender"));
                break;
            case 7:
                _unitTesting.NPCReward("Defender", "Defender", "bid", 1);
                StartCoroutine(UpdateWaiter("Defender"));
                break;
            case 8:
                _unitTesting.NPCReward("LargeDefender", "LargeDefender", "bid", 1);
                StartCoroutine(UpdateWaiter("LargeDefender"));
                break;
            case 9:
                _unitTesting.NPCReward("MajorDefender", "MajorDefender", "bid", 1);
                StartCoroutine(UpdateWaiter("MajorDefender"));
                break;
            case 10:
                _unitTesting.NPCReward("HugeDefender", "HugeDefender", "bid", 1);
                StartCoroutine(UpdateWaiter("HugeDefender"));
                break;
            case 11:
                _unitTesting.NPCReward("MassiveDefender", "MassiveDefender", "bid", 1);
                StartCoroutine(UpdateWaiter("MassiveDefender"));
                break;
            case 12:
                _unitTesting.NPCReward("GiganticDefender", "GiganticDefender", "bid", 1);
                StartCoroutine(UpdateWaiter("GiganticDefender"));
                break;
            case 13:
                _unitTesting.NPCReward("EnormousDefender", "EnormousDefender", "bid", 1);
                StartCoroutine(UpdateWaiter("EnormousDefender"));
                break;
            case 14:
                _unitTesting.NPCReward("TinyAttacker", "TinyAttacker", "bid", 1);
                StartCoroutine(UpdateWaiter("TinyAttacker"));
                break;
            case 15:
                _unitTesting.NPCReward("SmallAttacker", "SmallAttacker", "bid", 1);
                StartCoroutine(UpdateWaiter("SmallAttacker"));
                break;
            case 16:
                _unitTesting.NPCReward("LittleAttacker", "LittleAttacker", "bid", 1);
                StartCoroutine(UpdateWaiter("LittleAttacker"));
                break;
            case 17:
                _unitTesting.NPCReward("MinorAttacker", "MinorAttacker", "bid", 1);
                StartCoroutine(UpdateWaiter("MinorAttacker"));
                break;
            case 18:
                _unitTesting.NPCReward("ModerateAttacker", "ModerateAttacker", "bid", 1);
                StartCoroutine(UpdateWaiter("ModerateAttacker"));
                break;
            case 19:
                _unitTesting.NPCReward("AverageAttacker", "AverageAttacker", "bid", 1);
                StartCoroutine(UpdateWaiter("AverageAttacker"));
                break;
            case 20:
                _unitTesting.NPCReward("Attacker", "Attacker", "bid", 1);
                StartCoroutine(UpdateWaiter("Attacker"));
                break;
            case 21:
                _unitTesting.NPCReward("LargeAttacker", "LargeAttacker", "bid", 1);
                StartCoroutine(UpdateWaiter("LargeAttacker"));
                break;
            case 22:
                _unitTesting.NPCReward("MajorAttacker", "MajorAttacker", "bid", 1);
                StartCoroutine(UpdateWaiter("MajorAttacker"));
                break;
            case 23:
                _unitTesting.NPCReward("HugeAttacker", "HugeAttacker", "bid", 1);
                StartCoroutine(UpdateWaiter("HugeAttacker"));
                break;
            case 24:
                _unitTesting.NPCReward("MassiveAttacker", "MassiveAttacker", "bid", 1);
                StartCoroutine(UpdateWaiter("MassiveAttacker"));
                break;
            case 25:
                _unitTesting.NPCReward("GiganticAttacker", "GiganticAttacker", "bid", 1);
                StartCoroutine(UpdateWaiter("GiganticAttacker"));
                break;
            case 26:
                _unitTesting.NPCReward("EnormousAttacker", "EnormousAttacker", "bid", 1);
                StartCoroutine(UpdateWaiter("EnormousAttacker"));
                break;
            case 27: 
                _unitTesting.NPCReward("TinyRebel", "TinyRebel", "bid", 1);
                StartCoroutine(UpdateWaiter("TinyRebel"));
                break;
            case 28:
                _unitTesting.NPCReward("SmallRebel", "SmallRebel", "bid", 1);
                StartCoroutine(UpdateWaiter("SmallRebel"));
                break;
            case 29:
                _unitTesting.NPCReward("LittleRebel", "LittleRebel", "bid", 1);
                StartCoroutine(UpdateWaiter("LittleRebel"));
                break;
            case 30:
                _unitTesting.NPCReward("ModerateRebel", "ModerateRebel", "bid", 1);
                StartCoroutine(UpdateWaiter("ModerateRebel"));
                break;
            case 31:
                _unitTesting.NPCReward("AverageRebel", "AverageRebel", "bid", 1);
                StartCoroutine(UpdateWaiter("AverageRebel"));
                break;
            case 32:
                _unitTesting.NPCReward("Rebel", "Rebel", "bid", 1);
                StartCoroutine(UpdateWaiter("Rebel"));
                break;
            case 33:
                _unitTesting.NPCReward("LargeRebel", "LargeRebel", "bid", 1);
                StartCoroutine(UpdateWaiter("LargeRebel"));
                break;
            case 34:
                _unitTesting.NPCReward("HugeRebel", "HugeRebel", "bid", 1);
                StartCoroutine(UpdateWaiter("HugeRebel"));
                break;
            case 35:
                _unitTesting.NPCReward("MassiveRebel", "MassiveRebel", "bid", 1);
                StartCoroutine(UpdateWaiter("MassiveRebel"));
                break;
            case 36:
                _unitTesting.NPCReward("EnormousRebel", "EnormousRebel", "bid", 1);
                StartCoroutine(UpdateWaiter("EnormousRebel"));
                break;
            case 37:
                _unitTesting.NPCReward("Celebrant", "Celebrant", "bid", 1);
                StartCoroutine(UpdateWaiter("Celebrant"));
                break;
            case 38:
                _unitTesting.NPCReward("LargeCelebrant", "LargeCelebrant", "bid", 1);
                StartCoroutine(UpdateWaiter("LargeCelebrant"));
                break;
            case 39:
                _unitTesting.NPCReward("MassiveCelebrant", "MassiveCelebrant", "bid", 1);
                StartCoroutine(UpdateWaiter("MassiveCelebrant"));
                break;
            case 40:
                _unitTesting.NPCReward("MomentaryRepeater", "MomentaryRepeater", "bid", 1);
                StartCoroutine(UpdateWaiter("MomentaryRepeater"));
                break;
            case 41:
                _unitTesting.NPCReward("BriefRepeater", "BriefRepeater", "bid", 1);
                StartCoroutine(UpdateWaiter("BriefRepeater"));
                break;
            case 42:
                _unitTesting.NPCReward("ShortRepeater", "ShortRepeater", "bid", 1);
                StartCoroutine(UpdateWaiter("ShortRepeater"));
                break;
            case 43:
                _unitTesting.NPCReward("TemporaryRepeater", "TemporaryRepeater", "bid", 1);
                StartCoroutine(UpdateWaiter("TemporaryRepeater"));
                break;
            case 44:
                _unitTesting.NPCReward("PassingRepeater", "PassingRepeater", "bid", 1);
                StartCoroutine(UpdateWaiter("PassingRepeater"));
                break;
            case 45:
                _unitTesting.NPCReward("InterimRepeater", "InterimRepeater", "bid", 1);
                StartCoroutine(UpdateWaiter("InterimRepeater"));
                break;
            case 46:
                _unitTesting.NPCReward("Repeater", "Repeater", "bid", 1);
                StartCoroutine(UpdateWaiter("Repeater"));
                break;
            case 47:
                _unitTesting.NPCReward("ExtendedRepeater", "ExtendedRepeater", "bid", 1);
                StartCoroutine(UpdateWaiter("ExtendedRepeater"));
                break;
            case 48:
                _unitTesting.NPCReward("ProlongedRepeater", "ProlongedRepeater", "bid", 1);
                StartCoroutine(UpdateWaiter("ProlongedRepeater"));
                break;
            case 49:
                _unitTesting.NPCReward("LastingRepeater", "LastingRepeater", "bid", 1);
                StartCoroutine(UpdateWaiter("LastingRepeater"));
                break;
            case 50:
                _unitTesting.NPCReward("PermanentRepeater", "PermanentRepeater", "bid", 1);
                StartCoroutine(UpdateWaiter("PermanentRepeater"));
                break;
            case 51:
                _unitTesting.NPCReward("EternalRepeater", "EternalRepeater", "bid", 1);
                StartCoroutine(UpdateWaiter("EternalRepeater"));
                break;
            case 52:
                _unitTesting.NPCReward("MomentaryUpgrader", "MomentaryUpgrader", "bid", 1);
                StartCoroutine(UpdateWaiter("MomentaryUpgrader"));
                break;
            case 53:
                _unitTesting.NPCReward("BriefUpgrader", "BriefUpgrader", "bid", 1);
                StartCoroutine(UpdateWaiter("BriefUpgrader"));
                break;
            case 54:
                _unitTesting.NPCReward("ShortUpgrader", "ShortUpgrader", "bid", 1);
                StartCoroutine(UpdateWaiter("ShortUpgrader"));
                break;
            case 55:
                _unitTesting.NPCReward("TemporaryUpgrader", "TemporaryUpgrader", "bid", 1);
                StartCoroutine(UpdateWaiter("TemporaryUpgrader"));
                break;
            case 56:
                _unitTesting.NPCReward("PassingUpgrader", "PassingUpgrader", "bid", 1);
                StartCoroutine(UpdateWaiter("PassingUpgrader"));
                break;
            case 57:
                _unitTesting.NPCReward("InterimUpgrader", "InterimUpgrader", "bid", 1);
                StartCoroutine(UpdateWaiter("InterimUpgrader"));
                break;
            case 58:
                _unitTesting.NPCReward("Upgrader", "Upgrader", "bid", 1);
                StartCoroutine(UpdateWaiter("Upgrader"));
                break;
            case 59:
                _unitTesting.NPCReward("ExtendedUpgrader", "ExtendedUpgrader", "bid", 1);
                StartCoroutine(UpdateWaiter("ExtendedUpgrader"));
                break;
            case 60:
                _unitTesting.NPCReward("ProlongedUpgrader", "ProlongedUpgrader", "bid", 1);
                StartCoroutine(UpdateWaiter("ProlongedUpgrader"));
                break;
            case 61:
                _unitTesting.NPCReward("LastingUpgrader", "LastingUpgrader", "bid", 1);
                StartCoroutine(UpdateWaiter("LastingUpgrader"));
                break;
            case 62:
                _unitTesting.NPCReward("PermanentUpgrader", "PermanentUpgrader", "bid", 1);
                StartCoroutine(UpdateWaiter("PermanentUpgrader"));
                break;
            case 63:
                _unitTesting.NPCReward("EternalUpgrader", "EternalUpgrader", "bid", 1);
                StartCoroutine(UpdateWaiter("EternalUpgrader"));
                break;
            case 64:
                _unitTesting.NPCReward("MomentaryGold", "MomentaryGold", "bid", 1);
                StartCoroutine(UpdateWaiter("MomentaryGold"));
                break;
            case 65:
                _unitTesting.NPCReward("BriefGold", "BriefGold", "bid", 1);
                StartCoroutine(UpdateWaiter("BriefGold"));
                break;
            case 66:
                _unitTesting.NPCReward("ShortGold", "ShortGold", "bid", 1);
                StartCoroutine(UpdateWaiter("ShortGold"));
                break;
            case 67:
                _unitTesting.NPCReward("TemporaryGold", "TemporaryGold", "bid", 1);
                StartCoroutine(UpdateWaiter("TemporaryGold"));
                break;
            case 68:
                _unitTesting.NPCReward("PassingGold", "PassingGold", "bid", 1);
                StartCoroutine(UpdateWaiter("PassingGold"));
                break;
            case 69:
                _unitTesting.NPCReward("InterimGold", "InterimGold", "bid", 1);
                StartCoroutine(UpdateWaiter("InterimGold"));
                break;
            case 70:
                _unitTesting.NPCReward("Gold", "Gold", "bid", 1);
                StartCoroutine(UpdateWaiter("Gold"));
                break;
            case 71:
                _unitTesting.NPCReward("ExtendedGold", "ExtendedGold", "bid", 1);
                StartCoroutine(UpdateWaiter("ExtendedGold"));
                break;
            case 72:
                _unitTesting.NPCReward("ProlongedGold", "ProlongedGold", "bid", 1);
                StartCoroutine(UpdateWaiter("ProlongedGold"));
                break;
            case 73:
                _unitTesting.NPCReward("LastingGold", "LastingGold", "bid", 1);
                StartCoroutine(UpdateWaiter("LastingGold"));
                break;
            case 74:
                _unitTesting.NPCReward("PermanentGold", "PermanentGold", "bid", 1);
                StartCoroutine(UpdateWaiter("PermanentGold"));
                break;
            case 75:
                _unitTesting.NPCReward("EternalGold", "EternalGold", "bid", 1);
                StartCoroutine(UpdateWaiter("EternalGold"));
                break;
            case 76:
                _unitTesting.NPCReward("MomentaryRuby", "MomentaryRuby", "bid", 1);
                StartCoroutine(UpdateWaiter("MomentaryRuby"));
                break;
            case 77:
                _unitTesting.NPCReward("BriefRuby", "BriefRuby", "bid", 1);
                StartCoroutine(UpdateWaiter("BriefRuby"));
                break;
            case 78:
                _unitTesting.NPCReward("ShortRuby", "ShortRuby", "bid", 1);
                StartCoroutine(UpdateWaiter("ShortRuby"));
                break;
            case 79:
                _unitTesting.NPCReward("TemporaryRuby", "TemporaryRuby", "bid", 1);
                StartCoroutine(UpdateWaiter("TemporaryRuby"));
                break;
            case 80:
                _unitTesting.NPCReward("PassingRuby", "PassingRuby", "bid", 1);
                StartCoroutine(UpdateWaiter("PassingRuby"));
                break;
            case 81:
                _unitTesting.NPCReward("InterimRuby", "InterimRuby", "bid", 1);
                StartCoroutine(UpdateWaiter("InterimRuby"));
                break;
            case 82:
                _unitTesting.NPCReward("Ruby", "Ruby", "bid", 1);
                StartCoroutine(UpdateWaiter("Ruby"));
                break;
            case 83:
                _unitTesting.NPCReward("ExtendedRuby", "ExtendedRuby", "bid", 1);
                StartCoroutine(UpdateWaiter("ExtendedRuby"));
                break;
            case 84:
                _unitTesting.NPCReward("ProlongedRuby", "ProlongedRuby", "bid", 1);
                StartCoroutine(UpdateWaiter("ProlongedRuby"));
                break;
            case 85:
                _unitTesting.NPCReward("LastingRuby", "LastingRuby", "bid", 1);
                StartCoroutine(UpdateWaiter("LastingRuby"));
                break;
            case 86:
                _unitTesting.NPCReward("PermanentRuby", "PermanentRuby", "bid", 1);
                StartCoroutine(UpdateWaiter("PermanentRuby"));
                break;
            case 87:
                _unitTesting.NPCReward("EternalRuby", "EternalRuby", "bid", 1);
                StartCoroutine(UpdateWaiter("EternalRuby"));
                break;
            case 88:
                _unitTesting.NPCReward("ReverseToll", "ReverseToll", "bid", 1);
                StartCoroutine(UpdateWaiter("ReverseToll"));
                break;
            case 89:
                _unitTesting.NPCReward("NoToll", "NoToll", "bid", 1);
                StartCoroutine(UpdateWaiter("NoToll"));
                break;
            case 90:
                _unitTesting.NPCReward("TinyToll", "TinyToll", "bid", 1);
                StartCoroutine(UpdateWaiter("TinyToll"));
                break;
            case 91:
                _unitTesting.NPCReward("SmallToll", "SmallToll", "bid", 1);
                StartCoroutine(UpdateWaiter("SmallToll"));
                break;
            case 92:
                _unitTesting.NPCReward("LittleToll", "LittleToll", "bid", 1);
                StartCoroutine(UpdateWaiter("LittleToll"));
                break;
            case 93:
                _unitTesting.NPCReward("ModerateToll", "ModerateToll", "bid", 1);
                StartCoroutine(UpdateWaiter("ModerateToll"));
                break;
            case 94:
                _unitTesting.NPCReward("AverageToll", "AverageToll", "bid", 1);
                StartCoroutine(UpdateWaiter("AverageToll"));
                break;
            case 95:
                _unitTesting.NPCReward("Toll", "Toll", "bid", 1);
                StartCoroutine(UpdateWaiter("Toll"));
                break;
            case 96:
                _unitTesting.NPCReward("LargeToll", "LargeToll", "bid", 1);
                StartCoroutine(UpdateWaiter("LargeToll"));
                break;
            case 97:
                _unitTesting.NPCReward("HugeToll", "HugeToll", "bid", 1);
                StartCoroutine(UpdateWaiter("HugeToll"));
                break;
            case 98:
                _unitTesting.NPCReward("MassiveToll", "MassiveToll", "bid", 1);
                StartCoroutine(UpdateWaiter("MassiveToll"));
                break;
            case 99:
                _unitTesting.NPCReward("EnormousToll", "EnormousToll", "bid", 1);
                StartCoroutine(UpdateWaiter("EnormousToll"));
                break;
            case 100:
                _unitTesting.NPCReward("ObsceneToll", "ObsceneToll", "bid", 1);
                StartCoroutine(UpdateWaiter("ObsceneToll"));
                break;
            case 101:
                _unitTesting.NPCReward("MomentaryCurse", "MomentaryCurse", "bid", 1);
                StartCoroutine(UpdateWaiter("MomentaryCurse"));
                break;
            case 102:
                _unitTesting.NPCReward("BriefCurse", "BriefCurse", "bid", 1);
                StartCoroutine(UpdateWaiter("BriefCurse"));
                break;
            case 103:
                _unitTesting.NPCReward("ShortCurse", "ShortCurse", "bid", 1);
                StartCoroutine(UpdateWaiter("ShortCurse"));
                break;
            case 104:
                _unitTesting.NPCReward("TemporaryCurse", "TemporaryCurse", "bid", 1);
                StartCoroutine(UpdateWaiter("TemporaryCurse"));
                break;
            case 105:
                _unitTesting.NPCReward("PassingCurse", "PassingCurse", "bid", 1);
                StartCoroutine(UpdateWaiter("PassingCurse"));
                break;
            case 106:
                _unitTesting.NPCReward("InterimCurse", "InterimCurse", "bid", 1);
                StartCoroutine(UpdateWaiter("InterimCurse"));
                break;
            case 107:
                _unitTesting.NPCReward("Curse", "Curse", "bid", 1);
                StartCoroutine(UpdateWaiter("Curse"));
                break;
            case 108:
                _unitTesting.NPCReward("ExtendedCurse", "ExtendedCurse", "bid", 1);
                StartCoroutine(UpdateWaiter("ExtendedCurse"));
                break;
            case 109:
                _unitTesting.NPCReward("ProlongedCurse", "ProlongedCurse", "bid", 1);
                StartCoroutine(UpdateWaiter("ProlongedCurse"));
                break;
            case 110:
                _unitTesting.NPCReward("LastingCurse", "LastingCurse", "bid", 1);
                StartCoroutine(UpdateWaiter("LastingCurse"));
                break;
            case 111:
                _unitTesting.NPCReward("PermanentCurse", "PermanentCurse", "bid", 1);
                StartCoroutine(UpdateWaiter("PermanentCurse"));
                break;
            case 112:
                _unitTesting.NPCReward("EternalCurse", "EternalCurse", "bid", 1);
                StartCoroutine(UpdateWaiter("EternalCurse"));
                break;
            case 113:
                _unitTesting.NPCReward("MomentaryMystery", "MomentaryMystery", "bid", 1);
                StartCoroutine(UpdateWaiter("MomentaryMystery"));
                break;
            case 114:
                _unitTesting.NPCReward("BriefMystery", "BriefMystery", "bid", 1);
                StartCoroutine(UpdateWaiter("BriefMystery"));
                break;
            case 115:
                _unitTesting.NPCReward("ShortMystery", "ShortMystery", "bid", 1);
                StartCoroutine(UpdateWaiter("ShortMystery"));
                break;
            case 116:
                _unitTesting.NPCReward("TemporaryMystery", "TemporaryMystery", "bid", 1);
                StartCoroutine(UpdateWaiter("TemporaryMystery"));
                break;
            case 117:
                _unitTesting.NPCReward("PassingMystery", "PassingMystery", "bid", 1);
                StartCoroutine(UpdateWaiter("PassingMystery"));
                break;
            case 118:
                _unitTesting.NPCReward("InterimMystery", "InterimMystery", "bid", 1);
                StartCoroutine(UpdateWaiter("InterimMystery"));
                break;
            case 119:
                _unitTesting.NPCReward("Mystery", "Mystery", "bid", 1);
                StartCoroutine(UpdateWaiter("Mystery"));
                break;
            case 120:
                _unitTesting.NPCReward("ExtendedMystery", "ExtendedMystery", "bid", 1);
                StartCoroutine(UpdateWaiter("ExtendedMystery"));
                break;
            case 121:
                _unitTesting.NPCReward("ProlongedMystery", "ProlongedMystery", "bid", 1);
                StartCoroutine(UpdateWaiter("ProlongedMystery"));
                break;
            case 122:
                _unitTesting.NPCReward("LastingMystery", "LastingMystery", "bid", 1);
                StartCoroutine(UpdateWaiter("LastingMystery"));
                break;
            case 123:
                _unitTesting.NPCReward("PermanentMystery", "PermanentMystery", "bid", 1);
                StartCoroutine(UpdateWaiter("PermanentMystery"));
                break;
            case 124:
                _unitTesting.NPCReward("EternalMystery", "EternalMystery", "bid", 1);
                StartCoroutine(UpdateWaiter("EternalMystery"));
                break;
            case 125:
                _unitTesting.NPCReward("Lava", "Lava", "bid", 1);
                StartCoroutine(UpdateWaiter("Lava"));
                break;
            case 126:
                _unitTesting.NPCReward("Water", "Water", "bid", 1);
                StartCoroutine(UpdateWaiter("Water"));
                break;
            case 127:
                _unitTesting.NPCReward("MomentaryNull", "MomentaryNull", "bid", 1);
                StartCoroutine(UpdateWaiter("MomentaryNull"));
                break;
            case 128:
                _unitTesting.NPCReward("BriefNull", "BriefNull", "bid", 1);
                StartCoroutine(UpdateWaiter("BriefNull"));
                break;
            case 129:
                _unitTesting.NPCReward("ShortNull", "ShortNull", "bid", 1);
                StartCoroutine(UpdateWaiter("ShortNull"));
                break;
            case 130:
                _unitTesting.NPCReward("TemporaryNull", "TemporaryNull", "bid", 1);
                StartCoroutine(UpdateWaiter("TemporaryNull"));
                break;
            case 131:
                _unitTesting.NPCReward("PassingNull", "PassingNull", "bid", 1);
                StartCoroutine(UpdateWaiter("PassingNull"));
                break;
            case 132:
                _unitTesting.NPCReward("InterimNull", "InterimNull", "bid", 1);
                StartCoroutine(UpdateWaiter("InterimNull"));
                break;
            case 133:
                _unitTesting.NPCReward("Null", "Null", "bid", 1);
                StartCoroutine(UpdateWaiter("Null"));
                break;
            case 134:
                _unitTesting.NPCReward("ExtendedNull", "ExtendedNull", "bid", 1);
                StartCoroutine(UpdateWaiter("ExtendedNull"));
                break;
            case 135:
                _unitTesting.NPCReward("ProlongedNull", "ProlongedNull", "bid", 1);
                StartCoroutine(UpdateWaiter("ProlongedNull"));
                break;
            case 136:
                _unitTesting.NPCReward("LastingNull", "LastingNull", "bid", 1);
                StartCoroutine(UpdateWaiter("LastingNull"));
                break;
            case 137:
                _unitTesting.NPCReward("PermanentNull", "PermanentNull", "bid", 1);
                StartCoroutine(UpdateWaiter("PermanentNull"));
                break;
            case 138:
                _unitTesting.NPCReward("EternalNull", "EternalNull", "bid", 1);
                StartCoroutine(UpdateWaiter("EternalNull"));
                break;
            case 139:
                _unitTesting.NPCReward("Sway", "Sway", "bid", 1);
                StartCoroutine(UpdateWaiter("Sway"));
                break;
            case 140:
                _unitTesting.NPCReward("Elevator", "Elevator", "bid", 1);
                StartCoroutine(UpdateWaiter("Elevator"));
                break;
            case 141:
                _unitTesting.NPCReward("Slaunchwise", "Slaunchwise", "bid", 1);
                StartCoroutine(UpdateWaiter("Slaunchwise"));
                break;
            case 142:
                _unitTesting.NPCReward("Flip", "Flip", "bid", 1);
                StartCoroutine(UpdateWaiter("Flip"));
                break;
            case 143:
                _unitTesting.NPCReward("Flop", "Flop", "bid", 1);
                StartCoroutine(UpdateWaiter("Flop"));
                break;
            case 144:
                _unitTesting.NPCReward("Florp", "Florp", "bid", 1);
                StartCoroutine(UpdateWaiter("Florp"));
                break;
            case 145:
                _unitTesting.NPCReward("SpaceTraining", "SpaceTraining", "bid", 1);
                StartCoroutine(UpdateWaiter("SpaceTraining"));
                break;
            case 146:
                _unitTesting.NPCReward("HyperspaceTraining", "HyperspaceTraining", "bid", 1);
                StartCoroutine(UpdateWaiter("HyperspaceTraining"));
                break;
            case 147:
                _unitTesting.NPCReward("Wide", "Wide", "bid", 1);
                StartCoroutine(UpdateWaiter("Wide"));
                break;
            case 148:
                _unitTesting.NPCReward("Tall", "Tall", "bid", 1);
                StartCoroutine(UpdateWaiter("Tall"));
                break;
            case 149:
                _unitTesting.NPCReward("Embiggen", "Embiggen", "bid", 1);
                StartCoroutine(UpdateWaiter("Embiggen"));
                break;
            case 150:
                _unitTesting.NPCReward("Squishy", "Squishy", "bid", 1);
                StartCoroutine(UpdateWaiter("Squishy"));
                break;
            case 151:
                _unitTesting.NPCReward("SquishySlaunch", "SquishySlaunch", "bid", 1);
                StartCoroutine(UpdateWaiter("SquishySlaunch"));
                break;
            case 152:
                _unitTesting.NPCReward("SquishySpaceTraining", "SquishySpaceTraining", "bid", 1);
                StartCoroutine(UpdateWaiter("SquishySpaceTraining"));
                break;
            case 153:
                _unitTesting.NPCReward("HyperSquishTraining", "HyperSquishTraining", "bid", 1);
                StartCoroutine(UpdateWaiter("HyperSquishTraining"));
                break;
        }
                
    }

    private IEnumerator UpdateWaiter(string ID)
    {
        yield return new WaitForSeconds(1);
        
        InitializePlayerHandler(ID);
    }

    public void InitializePlayerHandler(string ID)
    {
        switch (ID)
        {
            case "GameMaster":
                SetNPCMode(0, ID, "#5DADE2");
                NPCAttack(_gm.PlayerHandlers[ID]);
                break;
            case "TinyDefender":
                SetNPCMode(1, ID, "#2E86C1");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 1);
                break;
            case "SmallDefender":
                SetNPCMode(1, ID, "#2E86C1");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 10);
                break;
            case "LittleDefender":
                SetNPCMode(1, ID, "#2E86C1");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 100);
                break;
            case "MinorDefender":
                SetNPCMode(1, ID, "#2E86C1");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 1000);
                break;
            case "ModerateDefender":
                SetNPCMode(1, ID, "#2E86C1");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 10000);
                break;
            case "AverageDefender":
                SetNPCMode(1, ID, "#2E86C1");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 100000);
                break;
            case "Defender":
                SetNPCMode(1, ID, "#2E86C1");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 1000000);
                break;
            case "LargeDefender":
                SetNPCMode(1, ID, "#2E86C1");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 10000000);
                break;
            case "MajorDefender":
                SetNPCMode(1, ID, "#2E86C1");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 100000000);
                break;
            case "HugeDefender":
                SetNPCMode(1, ID, "#2E86C1");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 1000000000);
                break;
            case "MassiveDefender":
                SetNPCMode(1, ID, "#2E86C1");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 10000000000);
                break;
            case "GiganticDefender":
                SetNPCMode(1, ID, "#2E86C1");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 100000000000);
                break;
            case "EnormousDefender":
                SetNPCMode(1, ID, "#2E86C1");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 1000000000000);
                break;
            case "TinyAttacker":
                SetNPCMode(2, ID, "#E74C3C");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 1000);
                break;
            case "SmallAttacker":
                SetNPCMode(2, ID, "#E74C3C");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 10000);
                break;
            case "LittleAttacker":
                SetNPCMode(2, ID, "#E74C3C");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 100000);
                break;
            case "MinorAttacker":
                SetNPCMode(2, ID, "#E74C3C");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 1000000);
                break;
            case "ModerateAttacker":
                SetNPCMode(2, ID, "#E74C3C");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 10000000);
                break;
            case "AverageAttacker":
                SetNPCMode(2, ID, "#E74C3C");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 100000000);
                break;
            case "Attacker":
                SetNPCMode(2, ID, "#E74C3C");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 1000000000);
                break;
            case "LargeAttacker":
                SetNPCMode(2, ID, "#E74C3C");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 10000000000);
                break;
            case "MajorAttacker":
                SetNPCMode(2, ID, "#E74C3C");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 100000000000);
                break;
            case "HugeAttacker":
                SetNPCMode(2, ID, "#E74C3C");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 1000000000000);
                break;
            case "MassiveAttacker":
                SetNPCMode(2, ID, "#E74C3C");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 10000000000000);
                break;
            case "GiganticAttacker":
                SetNPCMode(2, ID, "#E74C3C");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 100000000000000);
                break;
            case "EnormousAttacker":
                SetNPCMode(2, ID, "#E74C3C");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 1000000000000000);
                break;
            case "TinyRebel":
                SetNPCMode(3, ID, "#E4E03F");
                _unitTesting.NPCBits(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, 200);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid A", 15);
                break;
            case "SmallRebel":
                SetNPCMode(3, ID, "#E4E03F");
                _unitTesting.NPCBits(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, 300);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid A", 15);
                break;
            case "LittleRebel":
                SetNPCMode(3, ID, "#E4E03F");
                _unitTesting.NPCBits(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, 400);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid A", 15);
                break;
            case "ModerateRebel":
                SetNPCMode(3, ID, "#E4E03F");
                _unitTesting.NPCBits(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, 500);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid A", 15);
                break;
            case "AverageRebel":
                SetNPCMode(3, ID, "#E4E03F");
                _unitTesting.NPCBits(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, 1000);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid A", 15);
                break;
            case "Rebel":
                SetNPCMode(3, ID, "#E4E03F");
                _unitTesting.NPCBits(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, 1700);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid D", 25);
                break;
            case "LargeRebel":
                SetNPCMode(3, ID, "#E4E03F");
                _unitTesting.NPCBits(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, 2500);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid D", 25);
                break;
            case "HugeRebel":
                SetNPCMode(3, ID, "#E4E03F");
                _unitTesting.NPCBits(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, 5000);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid D", 25);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid D", 25);
                break;
            case "MassiveRebel":
                SetNPCMode(3, ID, "#E4E03F");
                _unitTesting.NPCBits(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, 10000);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid D", 25);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid D", 25);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid D", 25);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid D", 25);
                break;
            case "EnormousRebel":
                SetNPCMode(3, ID, "#E4E03F");
                _unitTesting.NPCBits(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, 25000);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid B", 175);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid D", 25);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid D", 25);
                _unitTesting.NPCReward(_gm.PlayerHandlers[ID].pp.TwitchID, _gm.PlayerHandlers[ID].pp.TwitchUsername, "AutoBid D", 25);
                break;
            case "Celebrant":
                SetNPCMode(4, ID, "#82E0AA");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 10000000);
                break;
            case "LargeCelebrant":
                SetNPCMode(4, ID, "#82E0AA");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 10000000);
                break;
            case "MassiveCelebrant":
                SetNPCMode(4, ID, "#82E0AA");
                NPCGivePoints(_gm.PlayerHandlers[ID].pp.TwitchUsername, 10000000);
                break;
            case "MomentaryRepeater":
                SetNPCMode(5, ID);
                RepeaterCount1 += 1;
                break;
            case "BriefRepeater":
                SetNPCMode(5, ID);
                RepeaterCount1 += 2;
                break;
            case "ShortRepeater":
                SetNPCMode(5, ID);
                RepeaterCount1 += 3;
                break;
            case "TemporaryRepeater":
                SetNPCMode(5, ID);
                RepeaterCount1 += 5;
                break;
            case "PassingRepeater":
                SetNPCMode(5, ID);
                RepeaterCount1 += 7;
                break;
            case "InterimRepeater":
                SetNPCMode(5, ID);
                RepeaterCount1 += 10;
                break;
            case "Repeater":
                SetNPCMode(5, ID);
                RepeaterCount1 += 17;
                break;            
            case "ExtendedRepeater":
                SetNPCMode(6, ID);
                RepeaterCount1 += 3;
                RepeaterCount2 += 5;
                break;
            case "ProlongedRepeater":
                SetNPCMode(6, ID);
                RepeaterCount1 += 5;
                RepeaterCount2 += 7;
                break;
            case "LastingRepeater":
                SetNPCMode(6, ID);
                RepeaterCount1 += 7;
                RepeaterCount2 += 11;
                break;
            case "PermanentRepeater":
                SetNPCMode(6, ID);
                RepeaterCount1 += 11;
                RepeaterCount2 += 13;
                break;
            case "EternalRepeater":
                SetNPCMode(6, ID);
                RepeaterCount1 += 13;
                RepeaterCount2 += 17;
                break;
            case "MomentaryUpgrader":
                SetNPCMode(7, ID);
                UpgraderCount1 += 1;
                break;
            case "BriefUpgrader":
                SetNPCMode(7, ID);
                UpgraderCount1 += 2;
                break;
            case "ShortUpgrader":
                SetNPCMode(7, ID);
                UpgraderCount1 += 3;
                break;
            case "TemporaryUpgrader":
                SetNPCMode(7, ID);
                UpgraderCount1 += 5;
                break;
            case "PassingUpgrader":
                SetNPCMode(7, ID);
                UpgraderCount1 += 7;
                break;
            case "InterimUpgrader":
                SetNPCMode(7, ID);
                UpgraderCount1 += 10;
                break;
            case "Upgrader":
                SetNPCMode(7, ID);
                UpgraderCount1 += 17;
                break;
            case "ExtendedUpgrader":
                SetNPCMode(8, ID);
                SetNPCMode(8, ID);
                UpgraderCount1 += 3;
                UpgraderCount2 += 5;
                break;
            case "ProlongedUpgrader":
                SetNPCMode(8, ID);
                UpgraderCount1 += 5;
                UpgraderCount2 += 7;
                break;
            case "LastingUpgrader":
                SetNPCMode(8, ID);
                UpgraderCount1 += 7;
                UpgraderCount2 += 11;
                break;
            case "PermanentUpgrader":
                SetNPCMode(8, ID);
                UpgraderCount1 += 11;
                UpgraderCount2 += 13;
                break;
            case "EternalUpgrader":
                SetNPCMode(8, ID);
                UpgraderCount1 += 13;
                UpgraderCount2 += 17;
                break;
            case "MomentaryGold":
                SetNPCMode(9, ID, "#FFCC00");
                GoldCount1 += 1;
                break;
            case "BriefGold":
                SetNPCMode(9, ID, "#FFCC00");
                GoldCount1 += 2;
                break;
            case "ShortGold":
                SetNPCMode(9, ID, "#FFCC00");
                GoldCount1 += 3;
                break;
            case "TemporaryGold":
                SetNPCMode(9, ID, "#FFCC00");
                GoldCount1 += 5;
                break;
            case "PassingGold":
                SetNPCMode(9, ID, "#FFCC00");
                GoldCount1 += 7;
                break;
            case "InterimGold":
                SetNPCMode(9, ID, "#FFCC00");
                GoldCount1 += 10;
                break;
            case "Gold":
                SetNPCMode(9, ID, "#FFCC00");
                GoldCount1 += 17;
                break;
            case "ExtendedGold":
                SetNPCMode(10, ID, "#FFCC00");
                GoldCount1 += 3;
                GoldCount2 += 5;
                break;
            case "ProlongedGold":
                SetNPCMode(10, ID, "#FFCC00");
                GoldCount1 += 5;
                GoldCount2 += 7;
                break;
            case "LastingGold":
                SetNPCMode(10, ID, "#FFCC00");
                GoldCount1 += 7;
                GoldCount2 += 11;
                break;
            case "PermanentGold":
                SetNPCMode(10, ID, "#FFCC00");
                GoldCount1 += 11;
                GoldCount2 += 13;
                break;
            case "EternalGold":
                SetNPCMode(10, ID, "#FFCC00");
                GoldCount1 += 13;
                GoldCount2 += 17;
                break;
            case "MomentaryRuby":
                SetNPCMode(11, ID, "#FF0000");
                RubyCount1 += 1;
                break;
            case "BriefRuby":
                SetNPCMode(11, ID, "#FF0000");
                RubyCount1 += 2;
                break;
            case "ShortRuby":
                SetNPCMode(11, ID, "#FF0000");
                RubyCount1 += 3;
                break;
            case "TemporaryRuby":
                SetNPCMode(11, ID, "#FF0000");
                RubyCount1 += 5;
                break;
            case "PassingRuby":
                SetNPCMode(11, ID, "#FF0000");
                RubyCount1 += 7;
                break;
            case "InterimRuby":
                SetNPCMode(11, ID, "#FF0000");
                RubyCount1 += 10;
                break;
            case "Ruby":
                SetNPCMode(11, ID, "#FF0000");
                RubyCount1 += 17;
                break;
            case "ExtendedRuby":
                SetNPCMode(12, ID, "#FF0000");
                RubyCount1 += 3;
                RubyCount2 += 5;
                break;
            case "ProlongedRuby":
                SetNPCMode(12, ID, "#FF0000");
                RubyCount1 += 5;
                RubyCount2 += 7;
                break;
            case "LastingRuby":
                SetNPCMode(12, ID, "#FF0000");
                RubyCount1 += 7;
                RubyCount2 += 11;
                break;
            case "PermanentRuby":
                SetNPCMode(12, ID, "#FF0000");
                RubyCount1 += 11;
                RubyCount2 += 13;
                break;
            case "EternalRuby":
                SetNPCMode(12, ID, "#FF0000");
                RubyCount1 += 13;
                RubyCount2 += 17;
                break;
            case "ReverseToll":
                SetNPCMode(13, ID, "#DDDDDD", -10);
                break;
            case "NoToll":
                SetNPCMode(13, ID, "#DDDDDD", 0);
                break;
            case "TinyToll":
                SetNPCMode(13, ID, "#DDDDDD", 1);
                break;
            case "SmallToll":
                SetNPCMode(13, ID, "#DDDDDD", 2);
                break;
            case "LittleToll":
                SetNPCMode(13, ID, "#DDDDDD", 3);
                break;
            case "ModerateToll":
                SetNPCMode(13, ID, "#DDDDDD", 5);
                break;
            case "AverageToll":
                SetNPCMode(13, ID, "#DDDDDD", 7);
                break;
            case "Toll":
                SetNPCMode(13, ID, "#DDDDDD", 10);
                break;
            case "LargeToll":
                SetNPCMode(13, ID, "#DDDDDD", 17);
                break;
            case "HugeToll":
                SetNPCMode(13, ID, "#DDDDDD", 26);
                break;
            case "MassiveToll":
                SetNPCMode(13, ID, "#DDDDDD", 51);
                break;
            case "EnormousToll":
                SetNPCMode(13, ID, "#DDDDDD", 101);
                break;
            case "ObsceneToll":
                SetNPCMode(13, ID, "#DDDDDD", 251);
                break;
            case "MomentaryCurse":
                SetNPCMode(14, ID, "#00FF00");
                CurseCount1 += 1;
                break;
            case "BriefCurse":
                SetNPCMode(14, ID, "#00FF00");
                CurseCount1 += 2;
                break;
            case "ShortCurse":
                SetNPCMode(14, ID, "#00FF00");
                CurseCount1 += 3;
                break;
            case "TemporaryCurse":
                SetNPCMode(14, ID, "#00FF00");
                CurseCount1 += 5;
                break;
            case "PassingCurse":
                SetNPCMode(14, ID, "#00FF00");
                CurseCount1 += 7;
                break;
            case "InterimCurse":
                SetNPCMode(14, ID, "#00FF00");
                CurseCount1 += 10;
                break;
            case "Curse":
                SetNPCMode(14, ID, "#00FF00");
                CurseCount1 += 17;
                break;
            case "ExtendedCurse":
                SetNPCMode(15, ID, "#00FF00");
                CurseCount1 += 3;
                CurseCount2 += 5;
                break;
            case "ProlongedCurse":
                SetNPCMode(15, ID, "#00FF00");
                CurseCount1 += 5;
                CurseCount2 += 7;
                break;
            case "LastingCurse":
                SetNPCMode(15, ID, "#00FF00");
                CurseCount1 += 7;
                CurseCount2 += 11;
                break;
            case "PermanentCurse":
                SetNPCMode(15, ID, "#00FF00");
                CurseCount1 += 11;
                CurseCount2 += 13;
                break;
            case "EternalCurse":
                SetNPCMode(15, ID, "#00FF00");
                CurseCount1 += 13;
                CurseCount2 += 17;
                break;
            case "MomentaryMystery":
                SetNPCMode(16, ID, "#8800FF");
                MysteryCount1 += 1;
                break;
            case "BriefMystery":
                SetNPCMode(16, ID, "#8800FF");
                MysteryCount1 += 2;
                break;
            case "ShortMystery":
                SetNPCMode(16, ID, "#8800FF");
                MysteryCount1 += 3;
                break;
            case "TemporaryMystery":
                SetNPCMode(16, ID, "#8800FF");
                MysteryCount1 += 5;
                break;
            case "PassingMystery":
                SetNPCMode(16, ID, "#8800FF");
                MysteryCount1 += 7;
                break;
            case "InterimMystery":
                SetNPCMode(16, ID, "#8800FF");
                MysteryCount1 += 10;
                break;
            case "Mystery":
                SetNPCMode(16, ID, "#8800FF");
                MysteryCount1 += 17;
                break;
            case "ExtendedMystery":
                SetNPCMode(17, ID, "#8800FF");
                MysteryCount1 += 3;
                MysteryCount2 += 5;
                break;
            case "ProlongedMystery":
                SetNPCMode(17, ID, "#8800FF");
                MysteryCount1 += 5;
                MysteryCount2 += 7;
                break;
            case "LastingMystery":
                SetNPCMode(17, ID, "#8800FF");
                MysteryCount1 += 7;
                MysteryCount2 += 11;
                break;
            case "PermanentMystery":
                SetNPCMode(17, ID, "#8800FF");
                MysteryCount1 += 11;
                MysteryCount2 += 13;
                break;
            case "EternalMystery":
                SetNPCMode(17, ID, "#8800FF");
                MysteryCount1 += 13;
                MysteryCount2 += 17;
                break;
            case "Lava":
                SetNPCMode(18, ID, "#FF2222");
                break;
            case "Water":
                SetNPCMode(19, ID, "#2222FF");
                break;
            case "MomentaryNull":
                SetNPCMode(20, ID, "#808080");
                NullCount1 += 1;
                break;
            case "BriefNull":
                SetNPCMode(20, ID, "#808080");
                NullCount1 += 2;
                break;
            case "ShortNull":
                SetNPCMode(20, ID, "#808080");
                NullCount1 += 3;
                break;
            case "TemporaryNull":
                SetNPCMode(20, ID, "#808080");
                NullCount1 += 5;
                break;
            case "PassingNull":
                SetNPCMode(20, ID, "#808080");
                NullCount1 += 7;
                break;
            case "InterimNull":
                SetNPCMode(20, ID, "#808080");
                NullCount1 += 10;
                break;
            case "Null":
                SetNPCMode(20, ID, "#808080");
                NullCount1 += 17;
                break;
            case "ExtendedNull":
                SetNPCMode(21, ID, "#808080");
                NullCount1 += 3;
                NullCount2 += 5;
                break;
            case "ProlongedNull":
                SetNPCMode(21, ID, "#808080");
                NullCount1 += 5;
                NullCount2 += 7;
                break;
            case "LastingNull":
                SetNPCMode(21, ID, "#808080");
                NullCount1 += 7;
                NullCount2 += 11;
                break;
            case "PermanentNull":
                SetNPCMode(21, ID, "#808080");
                NullCount1 += 11;
                NullCount2 += 13;
                break;
            case "EternalNull":
                SetNPCMode(21, ID, "#808080");
                NullCount1 += 13;
                NullCount2 += 17;
                break;
            case "Sway":
                SetNPCMode(22, ID, "#FF00FF", 0, 1);
                break;
            case "Elevator":
                SetNPCMode(22, ID, "#FF00FF", 0, 2);
                break;
            case "Slaunchwise":
                SetNPCMode(22, ID, "#FF00FF", 0, 3);
                break;
            case "Flip":
                SetNPCMode(22, ID, "#FF00FF", 0, 4);
                break;
            case "Flop":
                SetNPCMode(22, ID, "#FF00FF", 0, 5);
                break;
            case "Florp":
                SetNPCMode(22, ID, "#FF00FF", 0, 6);
                break;
            case "SpaceTraining":
                SetNPCMode(22, ID, "#FF00FF", 0, 7);
                break;
            case "HyperspaceTraining":
                SetNPCMode(22, ID, "#FF00FF", 0, 8);
                break;
            case "Wide":
                SetNPCMode(22, ID, "#FF00FF", 0, 9);
                break;
            case "Tall":
                SetNPCMode(22, ID, "#FF00FF", 0, 10);
                break;
            case "Embiggen":
                SetNPCMode(22, ID, "#FF00FF", 0, 11);
                break;
            case "Squishy":
                SetNPCMode(22, ID, "#FF00FF", 0, 12);
                break;
            case "SquishySlaunch":
                SetNPCMode(22, ID, "#FF00FF", 0, 13);
                break;
            case "SquishySpaceTraining":
                SetNPCMode(22, ID, "#FF00FF", 0, 14);
                break;
            case "HyperSquishTraining":
                SetNPCMode(22, ID, "#FF00FF", 0, 15);
                break;
        }
    
    }

    public void SetNPCMode(int mode, string key, string nameColorHex = "#FFFFFF", int state = 0, int data1 = 0)
    {
        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
        _gm.PlayerHandlers[key].pp.NameColorHex = nameColorHex;
        _gm.PlayerHandlers[key].pp.IsNPC = true;
        _gm.PlayerHandlers[key].pp.ModeNPC = mode;
        _gm.PlayerHandlers[key].pp.StateNPC = state;
        _gm.PlayerHandlers[key].pp.Data1NPC = data1;
        _gm.PlayerHandlers[key].SetCustomizationsFromPP();
    }

    public void CheckNPCs()
    {                
        string[] keys = _gm.PlayerHandlers.Keys.ToArray();
        foreach (string key in keys)
        {
            if (_gm.PlayerHandlers[key].pp.IsNPC == true)
            {
                if (_gm.PlayerHandlers[key].pp.ModeNPC == 0) //GameMaster
                {
                    if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                        NPCTradeUp(_gm.PlayerHandlers[key]);

                    _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 1) //Defenders
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Haha! Come and get me!");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Haha! Come and get me!");
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "AutoBid A", 15);
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (!_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            NPCMessage(_gm.PlayerHandlers[key], "Oh No, My Gold!");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                        }
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 2) //Attackers
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Fear Not. I have Rescued you all.");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Fear Not. I have Rescued you all.");
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "AutoBid A", 15);
                        }
                        else
                        {
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                            {
                                _gm.PlayerHandlers[key].pp.StateNPC = 0;
                                NPCAttack(_gm.PlayerHandlers[key]);
                            }
                            else
                                _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);

                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (!_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            NPCMessage(_gm.PlayerHandlers[key], "Time to Give Back to the community.");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                        }
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 3) //Rebels
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                            NPCTradeUp(_gm.PlayerHandlers[key]);
                        _gm.PlayerHandlers[key].pp.StateNPC = 1;
                    }
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                        if (_gm.PlayerHandlers[key].pp.AutoBidRemainder > 0 && _gm.PlayerHandlers[key].pp.CurrentBid == 0)
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 4) //Celebrants
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Preparing the Celebration!");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);

                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;

                            switch (key)
                            {
                                case "Celebrant":
                                    _unitTesting.NPCBits(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, 500);
                                    break;
                                case "LargeCelebrant":
                                    _unitTesting.NPCBits(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, 2500);
                                    break;
                                case "MassiveCelebrant":
                                    _unitTesting.NPCBits(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, 5000);
                                    break;
                            }
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "AutoBid A", 15);
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (!_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            NPCMessage(_gm.PlayerHandlers[key], "I hope you enjoyed the celebration!");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                        }
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 5) //Single Repeaters
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Alright! Let's repeat some Tiles!");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);

                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Alright! Let's repeat some Tiles!");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (RepeaterCount1 > 0)
                            {
                                NPCRepeatTile();
                                RepeaterCount1 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "I'm all out of repeats! Feel free to take the throne.");
                                _gm.PlayerHandlers[key].pp.StateNPC = 4;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "My time has come, again.");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 4;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 6) //Double Repeaters
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Alright! Let's repeat some Tiles!");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Alright! Let's repeat some Tiles!");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (RepeaterCount2 > 0)
                            {
                                NPCRepeatTile();
                                RepeaterCount2 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "Halfway through my repeats!");
                                _gm.PlayerHandlers[key].pp.StateNPC = 0;
                                _gm.PlayerHandlers[key].pp.ModeNPC = 5;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "My time has come, again.");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                            _gm.PlayerHandlers[key].pp.ModeNPC = 5;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 7) //Single Upgraders
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Alright! Let's Upgrade some Tiles!");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Alright! Let's Upgrade some Tiles!");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (UpgraderCount1 > 0)
                            {
                                NPCUpgradeTile();
                                UpgraderCount1 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "I'm all out of Upgrades! Feel free to take the throne.");
                                _gm.PlayerHandlers[key].pp.StateNPC = 4;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "I guess I'll come up with the gold.");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 4;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 8) //Double Upgraders
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Alright! Let's Upgrade some Tiles!");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Alright! Let's Upgrade some Tiles!");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (UpgraderCount2 > 0)
                            {
                                NPCUpgradeTile();
                                UpgraderCount2 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "Halfway through my Upgrades!");
                                _gm.PlayerHandlers[key].pp.StateNPC = 0;
                                _gm.PlayerHandlers[key].pp.ModeNPC = 7;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "I guess I'll come up with the gold.");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                            _gm.PlayerHandlers[key].pp.ModeNPC = 7;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 9) //Single Golds
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Alright! Let's Gild some Tiles!");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Alright! Let's gild some Tiles!");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (GoldCount1 > 0)
                            {
                                NPCGoldenTile();
                                GoldCount1 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "I'm all out of Golden Tiles! Feel free to take the throne.");
                                _gm.PlayerHandlers[key].pp.StateNPC = 4;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "Stay Golden, Folks");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 4;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 10) //Double Golds
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Alright! Let's Gild some Tiles!");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Alright! Let's Gild some Tiles!");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (GoldCount2 > 0)
                            {
                                NPCGoldenTile();
                                GoldCount2 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "Halfway through my Golden Tiles!");
                                _gm.PlayerHandlers[key].pp.StateNPC = 0;
                                _gm.PlayerHandlers[key].pp.ModeNPC = 9;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "Stay Golden, Folks!.");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                            _gm.PlayerHandlers[key].pp.ModeNPC = 9;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 11) //Single Rubies
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Time for the Ruby Treatment.");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Time for the Ruby Treatment.");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (RubyCount1 > 0)
                            {
                                NPCRubyTile();
                                RubyCount1 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "I'm all out of Ruby Tiles! Feel free to take the throne.");
                                _gm.PlayerHandlers[key].pp.StateNPC = 4;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "Stay Clever, Darlings.");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 4;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 12) //Double Rubies
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Time for the Ruby Treatment.");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Time for the Ruby Treatment.");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (RubyCount2 > 0)
                            {
                                NPCRubyTile();
                                RubyCount2 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "Stay Clever, Darlings.");
                                _gm.PlayerHandlers[key].pp.StateNPC = 0;
                                _gm.PlayerHandlers[key].pp.ModeNPC = 11;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "Stay Clever, Darlings.");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                            _gm.PlayerHandlers[key].pp.ModeNPC = 11;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 13) //Tolls
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC != 71717)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            if (_gm.PlayerHandlers[key].pp.StateNPC > 10)
                                NPCMessage(_gm.PlayerHandlers[key], $"I decree a new toll: {_gm.PlayerHandlers[key].pp.StateNPC - 1}");
                            else
                                NPCMessage(_gm.PlayerHandlers[key], $"I decree a new toll: {_gm.PlayerHandlers[key].pp.StateNPC}");

                            NPCToll(_gm.PlayerHandlers[key].pp.StateNPC);
                            _gm.PlayerHandlers[key].pp.StateNPC = 71717;
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                        }
                        else
                        {
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000)
                            {
                                NPCAttack(_gm.PlayerHandlers[key]);
                            }
                            else
                                _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 14) //Single Curses
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Arrr. These Tiles be Accurse-ed");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Arrr. These Tiles be Accurse-ed");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (CurseCount1 > 0)
                            {
                                NPCCursedTile();
                                CurseCount1 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "Curses! Me Stash-o-Curses has run dry!");
                                _gm.PlayerHandlers[key].pp.StateNPC = 4;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "Properly warned ye be, says I!");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 4;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 15) //Double Curses
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Arrr. These Tiles be Accurse-ed");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Arrr. These Tiles be Accurse-ed");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (CurseCount2 > 0)
                            {
                                NPCCursedTile();
                                CurseCount2 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "Halfway through my stash-o-curses! Arrr!");
                                _gm.PlayerHandlers[key].pp.StateNPC = 0;
                                _gm.PlayerHandlers[key].pp.ModeNPC = 14;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "Properly warned ye be, says I!");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                            _gm.PlayerHandlers[key].pp.ModeNPC = 14;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 16) //Single Mystery
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "We're on the trail!");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "We're on the trail!");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (MysteryCount1 > 0)
                            {
                                NPCMysteryTile();
                                MysteryCount1 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "Mystery Solved!");
                                _gm.PlayerHandlers[key].pp.StateNPC = 4;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "Hmmm... How very mysterious.");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 4;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 17) //Double Mystery
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "We're on the trail!");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "We're on the trail!");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (MysteryCount2 > 0)
                            {
                                NPCMysteryTile();
                                MysteryCount2 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "I need a break from all these mysteries.");
                                _gm.PlayerHandlers[key].pp.StateNPC = 0;
                                _gm.PlayerHandlers[key].pp.ModeNPC = 16;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "Hmmm... How very mysterious.");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                            _gm.PlayerHandlers[key].pp.ModeNPC = 16;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 18) //Lava
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Haha! Can't breach these defenses!");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "Haha! Can't breach these defenses!");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "Activate Lava", 1);
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "Awww, you got me.");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                        }
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 19) //Water
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC < 17)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                            NPCTradeUp(_gm.PlayerHandlers[key]);

                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                        _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                        _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "Activate Water", 1);
                        _gm.PlayerHandlers[key].pp.StateNPC += 1;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 20) //Single Null
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "...");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "...");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (MysteryCount1 > 0)
                            {
                                NPCNullTile();
                                MysteryCount1 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "....");
                                _gm.PlayerHandlers[key].pp.StateNPC = 4;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], ".....");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 4;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 21) //Double Null
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "...");
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCMessage(_gm.PlayerHandlers[key], "...");
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            if (MysteryCount2 > 0)
                            {
                                NPCNullTile();
                                MysteryCount2 -= 1;
                                _gm.PlayerHandlers[key].pp.StateNPC = 3;
                            }
                            else
                            {
                                NPCMessage(_gm.PlayerHandlers[key], "....");
                                _gm.PlayerHandlers[key].pp.StateNPC = 0;
                                _gm.PlayerHandlers[key].pp.ModeNPC = 20;
                            }
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], ".....");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                            _gm.PlayerHandlers[key].pp.ModeNPC = 20;
                        }
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 3)
                    {
                        _gm.PlayerHandlers[key].pp.StateNPC = 2;
                    }
                }
                else if (_gm.PlayerHandlers[key].pp.ModeNPC == 22) //TileEffects
                {
                    if (_gm.PlayerHandlers[key].pp.StateNPC == 0)
                    {
                        
                        if (_gm.PlayerHandlers[key].pp.SessionScore > 10000)
                        {
                            NPCAttack(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                        }
                        else if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 1;
                            NPCSetTileEffect(_gm.PlayerHandlers[key].pp.Data1NPC);
                        }
                        else
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);

                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 1)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.StateNPC = 2;
                            NPCSetTileEffect(_gm.PlayerHandlers[key].pp.Data1NPC);
                        }
                        else
                        {
                            _unitTesting.NPCReward(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "bid", 1);
                            _gm.PlayerHandlers[key].pp.StateNPC = 0;
                        }
                        _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                    }
                    else if (_gm.PlayerHandlers[key].pp.StateNPC == 2)
                    {
                        if (_gm.PlayerHandlers[key].IsKing())
                        {
                            _gm.PlayerHandlers[key].pp.LastInteraction = DateTime.Now;
                            NPCSetTileEffect(_gm.PlayerHandlers[key].pp.Data1NPC);
                        }
                        else
                        {
                            NPCMessage(_gm.PlayerHandlers[key], "Very Effective.");
                            _gm.PlayerHandlers[key].pp.Gold = _gm.PlayerHandlers[key].pp.Gold / 2;
                            _goldDistributor.SpawnGoldFromEvent(_gm.PlayerHandlers[key].pp.Gold);
                            if (_gm.PlayerHandlers[key].pp.SessionScore > 1000000000)
                                NPCTradeUp(_gm.PlayerHandlers[key]);
                            _gm.PlayerHandlers[key].pp.StateNPC = 3;
                        }
                    }
                }
            }
        }
    }

    public void PerformShopPurchases()
    {
        string[] keys = _gm.PlayerHandlers.Keys.ToArray();
        foreach (string key in keys)
        {
            if (_gm.PlayerHandlers[key].pp.IsNPC == true)
            {
                switch (UnityEngine.Random.Range(1, 4))
                {
                    case 1:
                        _unitTesting.NPCCommand(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "!buy1");
                        break;
                    case 2:
                        _unitTesting.NPCCommand(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "!buy2");
                        break;
                    case 3:
                        _unitTesting.NPCCommand(_gm.PlayerHandlers[key].pp.TwitchID, _gm.PlayerHandlers[key].pp.TwitchUsername, "!buy3");
                        break;
                }
            }
        }
    }

    public void NPCMessage(PlayerHandler ph, string msg)
    {
        ph.SpeechBubble(msg);
        if (ph.IsKing())
        {
            MyTTS.inst.PlayerSpeech(msg, ph.pp.VoiceID);
        }
    }

    public void NPCToll(int toll)
    {
        _kingController.TollRate = toll;
    }

    public void NPCTradeUp(PlayerHandler ph)
    {
        ph.TradeUp();
    }

    public void NPCRepeatTile()
    {
        if (_tileController.getNextForcedTile() == "NotOkay")
        {
            return;
        }

        if (_tileController.GameplayTile != null)
        {
            if (_tileController.GameplayTile.IsRuby)
            {
                Debug.Log("Ruby Tiles can neither be upgraded nor Repeated.");
                return;
            }
            else if (_tileController.GameplayTile.IsGolden)
            {
                Debug.Log("Golden Tiles cannot be Repeated.");
                return;
            }
        }
        else
        {
            if (_tileController.CurrentBiddingTile.IsRuby)
            {
                Debug.Log("Ruby Tiles can neither be upgraded nor Repeated.");
                return;
            }
            else if (_tileController.CurrentBiddingTile.IsGolden)
            {
                Debug.Log("Golden Tiles cannot be Repeated.");
                return;
            }
        }

        _tileController.doRepeatTile();
    }

    public void NPCUpgradeTile()
    {
        if (_tileController.getNextForcedTile() == "NotOkay")
        {
            return;
        }

        if (_tileController.GameplayTile == null)
        {
            if (_tileController.CurrentBiddingTile.IsRuby)
            {
                Debug.Log("Ruby Tiles can neither be upgraded nor Repeated.");
                return;
            }
            if (_tileController.CurrentBiddingTile.IsShop == true)
                return;
            if (_tileController.CurrentBiddingTile.GetRarity() == RarityType.SuperCosmic)
                return;
        }
        else
        {
            if (_tileController.GameplayTile.IsRuby)
            {
                Debug.Log("Ruby Tiles can neither be upgraded nor Repeated.");
                return;
            }
            if (_tileController.GameplayTile.IsShop == true)
                return;
            if (_tileController.GameplayTile.GetRarity() == RarityType.SuperCosmic)
                return;
        }

        _tileController.doRepeatTile();
        _tileController.doUpgradeTile();
    }

    public void NPCNullTile()
    {
        if (_tileController._forceGolden == true || _tileController._forceRuby == true || _tileController._forceMystery == true)
        {
            Debug.Log("The upcoming tile is already Golden or Ruby. Please wait until the reel spins to try again.");
            return;
        }

        if (_tileController.GameplayTile == null)
        {
            if (_tileController.CurrentBiddingTile.IsNull == true)
                return;
            else
                _tileController.CurrentBiddingTile._indicator1.SetText("⚠");
        }
        else
        {
            if (_tileController.GameplayTile.IsNull == true)
                return;
            else
                _tileController.GameplayTile._indicator1.SetText("⚠");
        }

        _tileController._forceCurse = false;
        _tileController._forceNull = true;

    }

    public void NPCGoldenTile()
    {
        if (_tileController._forceGolden == true || _tileController._forceRuby == true)
        {
            Debug.Log("The upcoming tile is already Golden or Ruby. Please wait until the reel spins to try again.");
            return;
        }

        if (_tileController.GameplayTile == null)
        {
            if (_tileController.CurrentBiddingTile.IsShop == true)
                return;
            else
                _tileController.CurrentBiddingTile._indicator1.SetText("💛");
        }
        else
        {
            if (_tileController.GameplayTile.IsShop == true)
                return;
            else
                _tileController.GameplayTile._indicator1.SetText("💛");
        }

        _tileController._forceCurse = false;
        _tileController._forceGolden = true;
        
    }

    public void NPCRubyTile()
    {
        if (_tileController._forceRuby == true)
        {
            Debug.Log("The upcoming tile is already Ruby. Please wait until the reel spins to try again.");
            return;
        }        

        if (_tileController.GameplayTile == null)
        {
            if (_tileController.CurrentBiddingTile.IsShop == true)
                return;
            else
                _tileController.CurrentBiddingTile._indicator1.SetText("🔴");
        }
        else
        {
            if (_tileController.GameplayTile.IsShop == true)
                return;
            else
                _tileController.GameplayTile._indicator1.SetText("🔴");
        }

        _tileController._forceCurse = false;
        _tileController._forceGolden = false;
        _tileController._forceRuby = true;
    }

    public void NPCCursedTile()
    {
        if (_tileController._forceRuby == true || _tileController._forceGolden == true || _tileController._forceNull)
        {
            Debug.Log("The upcoming tile is already Golden, Ruby, or Null. Please wait until the reel spins to try again.");
            return;
        }

        if (_tileController.GameplayTile == null)
        {
            if (_tileController.CurrentBiddingTile.IsShop == true)
                return;
            else
                _tileController.CurrentBiddingTile._indicator1.SetText("💚");
        }
        else
        {
            if (_tileController.GameplayTile.IsShop == true)
                return;
            else
                _tileController.GameplayTile._indicator1.SetText("💚");
        }

        _tileController._forceCurse = true;
    }

    public void NPCMysteryTile()
    {
        if (_tileController._forceCurse == true || _tileController._forceGolden == true || _tileController._forceRuby == true || _tileController.getNextForcedTile() == "NotOkay")
        {
            Debug.Log("The upcoming tile is already Cursed, Golden, Ruby, Repeated, or Upgraded. Please wait until the reel spins to try again.");
            return;
        }

        if (_tileController.GameplayTile == null)
        {
            if (_tileController.CurrentBiddingTile.IsShop == true)
                return;
            else
                _tileController.CurrentBiddingTile._indicator1.SetText("❔");
        }
        else
        {
            if (_tileController.GameplayTile.IsShop == true)
                return;
            else
                _tileController.GameplayTile._indicator1.SetText("❔");
        }

        _tileController._forceMystery = true;
    }

    public void NPCGivePoints(string targetUsername, long desiredPointsToGive)
    {
  
        TextPopupMaster.Inst.CreateTravelingIndicator(MyUtil.AbbreviateNum4Char(desiredPointsToGive), desiredPointsToGive, _gm.PlayerHandlers["GameMaster"], _gm.PlayerHandlers[targetUsername], 0.1f, Color.white, _gm.PlayerHandlers[targetUsername].PfpTexture, TI_Type.GivePoints);

    }

    public void NPCAttack(PlayerHandler ph)
    {
        if (ph.pb != null)
        {
            return;
        }
        if (ph.GetState() == PlayerHandlerState.BiddingQ)
        {
            return;
        }
        if (ph.GetState() == PlayerHandlerState.Gameplay)
        {
            return;
        }
        if (RebellionController.RoyalCelebration)
        {
            return;
        }

        PlayerBall pb = ph.GetPlayerBall();
        ph.SetState(PlayerHandlerState.Gameplay); //Prevent bug where players could enter bidding Q while king if timed correctly
        ph.ReceivableTarget = null; //Prevent bug where players would move to raffle after attacking and get stuck
        _twitchClient._attackPipe.ReceivePlayer(pb);

    }

    public void NPCSetTileEffect(int effect)
    {
        _tileController.CurrentBiddingTile.TileEffect = effect;
    }

    public void ChangeKingBackground()
    {
        if (_tileController.GameplayTile != null)
        {
            _KingTexture.materials = _tileController.GameplayTile._background.materials;
            _KingTile.RarityType = _tileController.GameplayTile.GetRarity();           

        }
        else
        {
            _KingTexture.materials = _tileController.CurrentBiddingTile._background.materials;
            _KingTile.RarityType = _tileController.CurrentBiddingTile.GetRarity();            
        }

        switch (_KingTile.GetRarity())
        {
            case RarityType.CommonPlus:
                goto case RarityType.CosmicPlus;
            case RarityType.RarePlus:
                goto case RarityType.CosmicPlus;
            case RarityType.EpicPlus:
                goto case RarityType.CosmicPlus;
            case RarityType.LegendaryPlus:
                goto case RarityType.CosmicPlus;
            case RarityType.MythicPlus:
                goto case RarityType.CosmicPlus;
            case RarityType.EtherealPlus:
                goto case RarityType.CosmicPlus;
            case RarityType.SuperCommon:
                goto case RarityType.CosmicPlus;
            case RarityType.SuperRare:
                goto case RarityType.CosmicPlus;
            case RarityType.SuperEpic:
                goto case RarityType.CosmicPlus;
            case RarityType.SuperLegendary:
                goto case RarityType.CosmicPlus;
            case RarityType.SuperMythic:
                goto case RarityType.CosmicPlus;
            case RarityType.SuperEthereal:
                goto case RarityType.CosmicPlus;
            case RarityType.SuperCosmic:
                goto case RarityType.CosmicPlus;
            case RarityType.CosmicPlus:
                _KingTile.HasBackground = true;
                break;
            default:
                _KingTile.HasBackground = false;
                break;
        }

    }

}
