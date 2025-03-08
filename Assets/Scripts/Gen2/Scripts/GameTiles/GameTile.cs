using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;


public enum RarityType { Common, Rare, Epic, Legendary, Mythic, Ethereal, Cosmic, CommonPlus, RarePlus, EpicPlus, LegendaryPlus, MythicPlus, EtherealPlus, CosmicPlus, SuperCommon, SuperRare, SuperEpic, SuperLegendary, SuperMythic, SuperEthereal, SuperCosmic, Mystery }
public enum DurationTYpe { Timer, Manual }
public enum Side { Left, Center, Right}
public enum TileState { Inactive, LockedInPos, Bidding, Gameplay}

public class GameTile : MonoBehaviour
{
    private TileController _tc;
    public int TileIDNum;
    [SerializeField] private Game _game;
    [SerializeField] private SuddenDeath _suddenDeath;
    [SerializeField] private CleaningBarController _cleaningBarController;
    [SerializeField] private Transform _releaseBar;
    [SerializeField] private TextMeshPro _tileNameText;
    [SerializeField] public TextMeshPro _rarityText;
    [SerializeField] private TextMeshPro _timerText;
    [SerializeField] public TextMeshPro _ticketBonusAmountText;
    [SerializeField] public TextMeshPro _indicator1;
    [SerializeField] public TextMeshPro _indicator2;
    [SerializeField] public TextMeshPro _indicator3;
    [SerializeField] public TextMeshPro _indicator4;
    [SerializeField] public TextMeshPro _indicator5;
    public int TicketBonusAmount;

    public TileState TileState = TileState.Inactive;
    public RarityType RarityType;
    [SerializeField] public DurationTYpe DurationTYpe;
    [SerializeField] public bool IsGolden; //1% chance
    [SerializeField] public bool IsRuby; //0.01% chance
    [SerializeField] public bool IsCurse; //Special
    [SerializeField] public bool IsMystery; //Special
    [SerializeField] public bool IsNull; //Special
    [SerializeField] public bool IsShop;
    [SerializeField] public bool IsRisk;
    [SerializeField] public bool IsKing;
    [SerializeField] public bool HasBackground;
    [SerializeField] public bool BuyingActive;
    [SerializeField] public bool Sponsored;
    [SerializeField] private CycleMode _cycleMode;
    [SerializeField] private bool _updateCycleModeButton;
    [SerializeField] private ContactWarp _contactWarp;

    [SerializeField] private Transform _resetablesRoot;

    [SerializeField] public MeshRenderer _background;
    [SerializeField] private List<Material> _bgOptions;
    [SerializeField] private List<Material> _bgOptionsPlus;
    [SerializeField] private List<Material> _bgOptionsSuper;

    [SerializeField] public int SponsorshipPrice;
    [SerializeField] public string CurrentSponsor;
    public PlayerHandler SponsorHandler;


    [SerializeField] private int _tileDurationS;

    [Range(0, 8)]
    [SerializeField] public int MinAuctionSlots;
    [Range(0, 8)]
    [SerializeField] public int MaxAuctionSlots;
    [Range(0, 8)]
    [SerializeField] public int RaffleSlots;
    [SerializeField] public int AuctionDuration = 60;

    [SerializeField] private bool finishTileButton;
    [SerializeField] public PipeReleaser EntrancePipe;
    [SerializeField] private GoldenVisuals _goldenVisuals;
    [SerializeField] private List<MeshRenderer> _colorTrimsByRarity;

    [SerializeField] public int TileEffect;

    public List<PlayerHandler> ConveyorBelt = new List<PlayerHandler>();

    private bool _forceEndGameplay = false;
    private float _tileGameplayTimeElapsed = 0;
    private float _timer;

    static public int GoldenSpids;
    //public bool TileActive;


    private MaterialPropertyBlock _mpb;
    private MaterialPropertyBlock _trimMpb;

    private int MysteriousAnnouncer;

    [SerializeField] [HideInInspector] private Color _backgroundStartColor;
    [SerializeField] [HideInInspector] private Color _backgroundEndColor;
    [SerializeField] [HideInInspector] private Color _trimColor;

    public PBEffector[] Effectors;

    public Side CurrentSide;

    //public int TotalPlayers;
    public List<PlayerHandler> Players = new List<PlayerHandler>();
    public List<PlayerHandler> AlivePlayers = new List<PlayerHandler>();
    public List<PlayerHandler> EliminatedPlayers = new List<PlayerHandler>();

    [Header("Wait for All Players Released Before Starting Game")]
    [SerializeField] private bool _waitForAll = true;
    [SerializeField] private bool _waitForAllDead = false;

    //private int _playersReleased = 0;

    private DateTime _tileStartTime;
    private long _playerPointsSumStart;

    private void Awake()
    {
        _mpb = new MaterialPropertyBlock();
        _trimMpb = new MaterialPropertyBlock();

        //SetBackgroundShader(1);

        _background.GetPropertyBlock(_mpb);
        _mpb.SetColor("_StartColor", _backgroundStartColor);
        _mpb.SetColor("_EndColor", _backgroundEndColor);
        _background.SetPropertyBlock(_mpb);

        _trimMpb.SetColor("_BaseColor", _trimColor);
        foreach (var meshRenderer in _colorTrimsByRarity)
            meshRenderer.SetPropertyBlock(_trimMpb);

        SetBackground();

        /*        foreach (var resetable in _resetablesRoot.GetComponentsInChildren<IResetable>())
                    resetable.MyReset();*/

        Effectors = GetComponentsInChildren<PBEffector>();

        if(!IsKing)
        _tileNameText.SetText(gameObject.name.Replace("(Clone)", ""));
    }

    public void SetRarity(RarityType rarityType, Color backgroundStartColor, Color backgroundEndColor, Color trimColor)
    {
        RarityType = rarityType;

        _rarityText.SetText(System.Enum.GetName(typeof(RarityType), rarityType));

        _backgroundStartColor = backgroundStartColor;
        _backgroundEndColor = backgroundEndColor;
        _trimColor = trimColor;

    }

    public void NewSponsor(PlayerHandler ph, string playername)
    {
        Debug.LogWarning("DidNewSponsor");
        
        SponsorHandler = ph;
        CurrentSponsor = playername;

        ph.pp.Sapphires -= SponsorshipPrice;
        SponsorshipPrice += 5;
        Sponsored = true;
    }

    public void SetBackground()
    {
        var BaseMaterials = _background.materials;
        var DesiredMaterials = _bgOptions;
        var MaterialsPlus = _bgOptionsPlus;
        var SuperMaterials = _bgOptionsSuper;

        if (!IsKing)
        {
            switch (GetRarity())
            {
                case RarityType.Common:
                    BaseMaterials[0] = DesiredMaterials[0];
                    break;
                case RarityType.Rare:
                    BaseMaterials[0] = DesiredMaterials[1];
                    break;
                case RarityType.Epic:
                    BaseMaterials[0] = DesiredMaterials[2];
                    break;
                case RarityType.Legendary:
                    BaseMaterials[0] = DesiredMaterials[3];
                    break;
                case RarityType.Mythic:
                    BaseMaterials[0] = DesiredMaterials[4];
                    break;
                case RarityType.Ethereal:
                    BaseMaterials[0] = DesiredMaterials[5];
                    break;
                case RarityType.Cosmic:
                    BaseMaterials[0] = DesiredMaterials[6];
                    break;
                case RarityType.CommonPlus:
                    BaseMaterials[0] = MaterialsPlus[0];
                    break;
                case RarityType.RarePlus:
                    BaseMaterials[0] = MaterialsPlus[1];
                    break;
                case RarityType.EpicPlus:
                    BaseMaterials[0] = MaterialsPlus[2];
                    break;
                case RarityType.LegendaryPlus:
                    BaseMaterials[0] = MaterialsPlus[3];
                    break;
                case RarityType.MythicPlus:
                    BaseMaterials[0] = MaterialsPlus[4];
                    break;
                case RarityType.EtherealPlus:
                    BaseMaterials[0] = MaterialsPlus[5];
                    break;
                case RarityType.CosmicPlus:
                    BaseMaterials[0] = MaterialsPlus[6];
                    break;
                case RarityType.SuperCommon:
                    BaseMaterials[0] = SuperMaterials[0];
                    break;
                case RarityType.SuperRare:
                    BaseMaterials[0] = SuperMaterials[1];
                    break;
                case RarityType.SuperEpic:
                    BaseMaterials[0] = SuperMaterials[2];
                    break;
                case RarityType.SuperLegendary:
                    BaseMaterials[0] = SuperMaterials[3];
                    break;
                case RarityType.SuperMythic:
                    BaseMaterials[0] = SuperMaterials[4];
                    break;
                case RarityType.SuperEthereal:
                    BaseMaterials[0] = SuperMaterials[5];
                    break;
                case RarityType.SuperCosmic:
                    BaseMaterials[0] = SuperMaterials[6];
                    break;
            }
        }
        _background.materials = BaseMaterials;
    }

    public RarityType GetRarity()
    {
        return RarityType; 
    }

    private void OnValidate()
    {
        if (finishTileButton) 
        {
            finishTileButton = false;
            FinishTile();
        }
        if (_updateCycleModeButton)
        {
            _updateCycleModeButton = false;
            InitCycleMode(); 
        }
    }

    private void InitCycleMode()
    {
        _contactWarp.targetMode = _cycleMode;

        if (_cycleMode == CycleMode.receiver)
        {
            EntrancePipe.gameObject.SetActive(true); 
            _contactWarp.playerReceiver = EntrancePipe;
            EntrancePipe.transform.localPosition = new Vector3(0, EntrancePipe.transform.localPosition.y, EntrancePipe.transform.localPosition.z);
            EntrancePipe.transform.localEulerAngles = new Vector3(0, 0, 180); 
        }
        else if(_cycleMode == CycleMode.verticalWrap)
        {
            EntrancePipe.gameObject.SetActive(false);
            int side = (CurrentSide == Side.Left) ? 1 : -1;
            EntrancePipe.transform.localPosition = new Vector3((_background.transform.localScale.x / 2.1f) * side, EntrancePipe.transform.localPosition.y, EntrancePipe.transform.localPosition.z); //Extra .1 to get past wall barrier
            EntrancePipe.transform.localEulerAngles = new Vector3(0, 0, 90 * side); 
        }
    }

    public void TogglePhysics(bool toggle)
    {
        foreach (var rb in GetComponentsInChildren<Rigidbody2D>())
            rb.simulated = toggle;

        foreach(var col in GetComponentsInChildren<Collider2D>())
            col.enabled = toggle;

        foreach (var oscillators in GetComponentsInChildren<OscillatorV2>())
            oscillators.ToggleOnOff(toggle); 
    }
    public void PreInitTile(TileController tc, bool insideGolden, bool insideRuby, bool insideCurse, bool insideMystery, bool insideNull)
    {
        IsCurse = false;
        IsGolden = false;
        IsRuby = false;
        IsMystery = false;
        IsNull = false;
        TileEffect = 0;

        _goldenVisuals._coverObj.gameObject.SetActive(false);
        _indicator3.gameObject.SetActive(false);  

        if (_mpb == null)
            _mpb = new MaterialPropertyBlock();
        
        _tc = tc;

        InitCycleMode(); 

        Effectors = GetComponentsInChildren<PBEffector>();

        _timer = 0;
        UpdateTileTimer();
        ResetTicketBonus();

        EntrancePipe.LockIcon.enabled = true;   

        foreach (var resetable in _resetablesRoot.GetComponentsInChildren<IResetable>())
            resetable.MyReset();

        int Mysteries = UnityEngine.Random.Range(1, 23);

        foreach (var effector in Effectors)
        {
            effector.ResetEffector();

            effector.MultiplyCurrValue(AppConfig.GetMult(RarityType));

            if (insideMystery)
            {
                switch (Mysteries)
                {
                    case 1:
                        effector.MultiplyCurrValue(0);
                        _indicator3.SetText("Null");
                        break;
                    case 2:
                        effector.MultiplyCurrValue(-1);
                        _indicator3.SetText("Cursed");
                        break;
                    case 3:
                        effector.MultiplyCurrValue(10);
                        _indicator3.SetText("10x");
                        break;
                    case 4:
                        effector.MultiplyCurrValue(50);
                        _indicator3.SetText("50x");
                        break;
                    case 5:
                        effector.MultiplyCurrValue(100);
                        _indicator3.SetText("100x");
                        break;
                    case 6:
                        effector.MultiplyCurrValue(1000);
                        _indicator3.SetText("1000x");
                        break;
                    case 7: //Left to Right
                        TileEffect = 1;
                        _indicator3.SetText("Sway");
                        break;
                    case 8: //Elevator
                        TileEffect = 2;
                        _indicator3.SetText("Elevator");
                        break;
                    case 9: //7+8
                        TileEffect = 3;
                        _indicator3.SetText("Slaunchwise");
                        break;
                    case 10: //Flip
                        TileEffect = 4;
                        _indicator3.SetText("Flip");
                        break;
                    case 11: //Flop
                        TileEffect = 5;
                        _indicator3.SetText("Flop");
                        break;
                    case 12: //Florp
                        TileEffect = 6;
                        _indicator3.SetText("Florp");
                        break;
                    case 13: //Space Training
                        TileEffect = 7;
                        _indicator3.SetText("Space Training");
                        break;
                    case 14: //Hyperspace Training
                        TileEffect = 8;
                        _indicator3.SetText("Hyperspace Training");
                        break;
                    case 15: //Wide
                        TileEffect = 9;
                        _indicator3.SetText("Wide");
                        break;
                    case 16: //Tall
                        TileEffect = 10;
                        _indicator3.SetText("Tall");
                        break;
                    case 17: //Embiggen
                        TileEffect = 11;
                        _indicator3.SetText("Embiggen");
                        break;
                    case 18: //Squishy
                        TileEffect = 12;
                        _indicator3.SetText("Squishy");
                        break;
                    case 19: //Squishy Slaunch
                        TileEffect = 13;
                        _indicator3.SetText("Squishy Slaunch");
                        break;
                    case 20: //Squishy Space Training
                        TileEffect = 14;
                        _indicator3.SetText("Squishy Space Training");
                        break;
                    case 21: //Slaunchwise Squish Training
                        TileEffect = 15;
                        _indicator3.SetText("HyperSquish Training");
                        break;
                    case 22:
                        effector.MultiplyCurrValue(500);
                        _indicator3.SetText("500x");
                        break;
                }
            }
            else if (insideRuby)
            {
                effector.MultiplyCurrValue(50);
            }
            else if (insideGolden)
            {
                effector.MultiplyCurrValue(10);
            }
            else if (insideCurse)
            {
                effector.MultiplyCurrValue(-1);
            }
            else if (insideNull)
            {
                effector.MultiplyCurrValue(0);
            }
        }       

        EntrancePipe.SetTollCost(tc.GetGameManager().GetKingController().TollRate * AppConfig.GetMult(RarityType));

        if (_game != null)
        {
            _game.DoneWithGameplay = false;
            _game.IsGameStarted = false;
            _game.OnTilePreInit();
        }

        if (_suddenDeath.gameObject.activeSelf)
            _suddenDeath.OnTilePreInit();

        if (IsShop)
        {
            _indicator3.SetText("Shop");
            insideCurse = false;
            insideRuby = false;
            insideGolden = false;
            insideNull = false;
            Mysteries = 0;
        }

        MysteriousAnnouncer = Mysteries;
                
        if (insideMystery)
        {
            _indicator3.gameObject.SetActive(true);
            _goldenVisuals.gameObject.SetActive(true);
            _goldenVisuals._coverObj.gameObject.SetActive(true);
            IsMystery = true;
            GoldenSpids = 4;
            _goldenVisuals.UpdateSettings(4);
        }
        else if (insideNull)
        {
            _goldenVisuals.gameObject.SetActive(true);
            IsNull = true;
            GoldenSpids = 5;
            _goldenVisuals.UpdateSettings(5);
        }
        else if (insideCurse)
        {            
            _goldenVisuals.gameObject.SetActive(true);
            IsCurse = true;
            GoldenSpids = 3;
            _goldenVisuals.UpdateSettings(3);
        }
        else if (insideRuby)
        {
            _goldenVisuals.gameObject.SetActive(true);
            IsRuby = true;
            GoldenSpids = 2;
            _goldenVisuals.UpdateSettings(2);
        }
        else if (insideGolden)
        {
            _goldenVisuals.gameObject.SetActive(true);
            IsGolden = true;
            GoldenSpids = 1;
            _goldenVisuals.UpdateSettings(1);
        }        
        else
        {
            _goldenVisuals.UpdateSettings(0);
            _goldenVisuals.gameObject.SetActive(false);
            _goldenVisuals._coverObj.gameObject.SetActive(false);
            GoldenSpids = 0;
        }

        _mpb.SetColor("_StartColor", _backgroundStartColor);
        _mpb.SetColor("_EndColor", _backgroundEndColor);
        _background.SetPropertyBlock(_mpb);

        _trimMpb.SetColor("_BaseColor", _trimColor);
        foreach (var meshRenderer in _colorTrimsByRarity)
            meshRenderer.SetPropertyBlock(_trimMpb);

        SetBackground();
        //SetBackgroundShader(0);
        UpdateTileTimer();

    }

    public void InitTileInPos()
    {

        _indicator4.gameObject.SetActive(true);
        _indicator5.gameObject.SetActive(true);
        _indicator4.SetText($"Current Sponsor: {CurrentSponsor}");
        _indicator5.SetText($"{SponsorshipPrice} Sapphires to Sponsor!");

        TileState = TileState.LockedInPos;
        //TileActive = true;
        _timer = 0;
        //_playersReleased = 0;

        Players.Clear();
        AlivePlayers.Clear();
        EliminatedPlayers.Clear();

        if (_releaseBar != null)
            _releaseBar.gameObject.SetActive(true);

        if (_game != null)
            _game.OnTileInitInPos();

        if (IsMystery)
        {
            StartCoroutine(PlaySoundSequence("Mystery"));
        }
        else if (IsRuby)
        {
            AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.4f, 0.5f);
        }
        else if (IsGolden)
        {
            AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.2f, 0.3f);
        }
        else if (IsCurse)
        {
            StartCoroutine(PlaySoundSequence("Curse"));
        }
        else if (IsNull)
        {
            AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.015f, 0.15f);
        }
        else if (IsShop)
        {
            AudioController.inst.PlaySound(AudioController.inst.BattlePerchEarn, 0.4f, 0.5f);
        }

        _tc._forceGolden = false;
        _tc._forceRuby = false;
        _tc._forceCurse = false;
        _tc._forceMystery = false;
        _tc._forceNull = false;

        TogglePhysics(true);

        UpdateTileTimer();

    }

    public void OnPipeReleasePlayer(PlayerBall pb)
    {
        //_playersReleased++; 
    }

    public IEnumerator PlaySoundSequence(string Which)
    {
        switch (Which)
        {
            case "Mystery":
                AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.4f, 0.8f);
                yield return new WaitForSeconds(0.1f);
                AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.35f, 0.7f);
                yield return new WaitForSeconds(0.1f);
                AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.3f, 0.6f);
                yield return new WaitForSeconds(0.1f);
                AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.25f, 0.5f);
                yield return new WaitForSeconds(0.1f);
                AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.2f, 0.4f);
                yield return new WaitForSeconds(0.1f);
                AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.15f, 0.3f);
                break;
            case "Curse":
                AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.06f, 0.08f);
                yield return new WaitForSeconds(0.5f);
                AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.05f, 0.07f);
                yield return new WaitForSeconds(0.5f);
                AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.04f, 0.06f);
                yield return new WaitForSeconds(0.5f);
                AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.03f, 0.05f);
                yield return new WaitForSeconds(0.5f);
                AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.02f, 0.04f);
                yield return new WaitForSeconds(0.5f);
                AudioController.inst.PlaySound(AudioController.inst.TileStatus, 0.01f, 0.03f);
                break;
        }
    }
    public IEnumerator RunTile()
    {
        // TileEffect = 15; // TESTING ONLY!! COMMENT OUT BEFORE BUILDING!

        _indicator4.gameObject.SetActive(false);
        _indicator5.gameObject.SetActive(false);

        float textFade = -1.15f;
        float positionX = 0f;
        float positionY = 0f;
        float positionZ = 0f;
        float rotationX = 0f;
        float rotationY = 0f;
        float rotationZ = 0f;
        float scaleX = 0f;
        float scaleY = 0f;
        float scaleZ = 0f;
        bool boundary1 = false;
        bool boundary2 = false;
        int timer1 = 0;
        Vector3 StartPosition = gameObject.transform.position;
        Quaternion StartRotation = gameObject.transform.rotation;
        Vector3 StartScale = gameObject.transform.localScale;

        TileState = TileState.Gameplay; 
        EntrancePipe.LockIcon.enabled = false;
        _tileStartTime = DateTime.Now;

        _goldenVisuals._coverObj.gameObject.SetActive(false);
        
        if (IsMystery)
        AnnounceMystery(MysteriousAnnouncer);

        _playerPointsSumStart = 0;
        foreach (var player in Players)
        {
            player.TilePointsROI = 0; 
            _playerPointsSumStart += player.pp.SessionScore;
        }

        if (_waitForAll)
        {
            //wait until all the players are off the conveyor belt
            while (ConveyorBelt.Count > 0)
                yield return null;
        }

        if (_suddenDeath.gameObject.activeSelf)
            _suddenDeath.StartSuddenDeath();

        if (_releaseBar != null)
            _releaseBar.gameObject.SetActive(false); 
        
        if(_game != null)
        {
            _game.StartGame();
            _game.IsGameStarted = true;
        }

        _forceEndGameplay = false;
        //pre-Gameplay Settings Go here
        switch (TileEffect)
        {
            default:
                break;
            case 1:
                if (CurrentSide == Side.Right)
                    boundary1 = true;
                else
                    boundary1 = false;
                break;
            case 2:
                gameObject.transform.position = transform.position + new Vector3(0, -4.5f, 0);
                break;
            case 3:
                if (CurrentSide == Side.Right)
                    boundary1 = true;
                else
                    boundary1 = false;
                gameObject.transform.position = transform.position + new Vector3(0, -4.5f, 0);
                break;
            case 4:
                goto case 1;
            case 5:
                goto case 1;
            case 6:
                goto case 1;
            case 7:
                goto case 1;
            case 8:
                goto case 3;
            case 9:
                if (CurrentSide == Side.Right)
                    boundary1 = true;
                else
                    boundary1 = false;

                timer1 = 2000;
                break;
            case 10:
                goto case 9;
            case 11:
                goto case 9;
            case 12:
                goto case 9;
            case 13:
                if (CurrentSide == Side.Right)
                    boundary1 = true;
                else
                    boundary1 = false;
                
                gameObject.transform.position = transform.position + new Vector3(0, -4.5f, 0);

                timer1 = 2000;
                break;
            case 14:
                goto case 9;
            case 15:
                goto case 13;
        }

        _tileGameplayTimeElapsed = 0; 
        //Run the gameplay until we either get a signal from the game that it's done, or there is only one player left alive
        while (true)
        {
            //Stop if the game signals it's done
            if (_game != null && _game.DoneWithGameplay)
                break;

            //Stop if the timer runs out
            if (_timer > _tileDurationS)
                break;

            // This is the Indicator for Mystery Tiles
            if (textFade < 5)
            {
                textFade += 0.01f;
                _indicator3.gameObject.transform.position = transform.position + new Vector3(0, 0, textFade);
            }

            //This is the TileEffect Handler
            if (ConveyorBelt.Count == 0)
            {
                switch (TileEffect)
                {
                    case 1:
                        if (boundary1)
                            positionX -= 0.0001f;
                        else
                            positionX += 0.0001f;

                        if (positionX > 0.025)
                            boundary1 = true;
                        if (positionX < -0.025)
                            boundary1 = false;

                        gameObject.transform.position = transform.position + new Vector3(positionX, 0, 0);
                        break;
                    case 2:
                        if (boundary1)
                            positionY -= 0.001f;
                        else
                            positionY += 0.001f;

                        if (positionY > 0.1)
                            boundary1 = true;
                        if (positionY < -0.1)
                            boundary1 = false;

                        gameObject.transform.position = transform.position + new Vector3(0, positionY, 0);
                        break;
                    case 3:
                        if (boundary1)
                            positionX -= 0.0001f;
                        else
                            positionX += 0.0001f;

                        if (positionX > 0.025)
                            boundary1 = true;
                        if (positionX < -0.025)
                            boundary1 = false;

                        if (boundary2)
                            positionY -= 0.001f;
                        else
                            positionY += 0.001f;

                        if (positionY > 0.1)
                            boundary2 = true;
                        if (positionY < -0.1)
                            boundary2 = false;

                        gameObject.transform.position = transform.position + new Vector3(positionX, positionY, 0);
                        break;
                    case 4:
                        if (boundary1)
                        {
                            rotationX = -.1f;
                        }
                        else
                        {
                            rotationX = .1f;
                        }

                        gameObject.transform.Rotate(rotationX, 0, 0, Space.Self);
                        break;
                    case 5:
                        if (boundary1)
                        {
                            rotationY = -.2f;
                        }
                        else
                        {
                            rotationY = .2f;
                        }

                        gameObject.transform.Rotate(0, rotationY, 0, Space.Self);
                        break;
                    case 6:
                        if (boundary1)
                        {
                            rotationZ = -.3f;
                        }
                        else
                        {
                            rotationZ = .3f;
                        }

                        gameObject.transform.Rotate(0, 0, rotationZ, Space.Self);
                        break;
                    case 7:
                        if (boundary1)
                        {
                            rotationX = -.1f;
                            rotationY = -.2f;
                            rotationZ = -.3f;
                        }
                        else
                        {
                            rotationX = .1f;
                            rotationY = .2f;
                            rotationZ = .3f;
                        }

                        gameObject.transform.Rotate(rotationX, rotationY, rotationZ, Space.Self);
                        break;
                    case 8:
                        if (boundary1)
                        {
                            rotationX = -.1f;
                            rotationY = -.2f;
                            rotationZ = -.3f;
                            positionX -= 0.0001f;
                        }
                        else
                        {
                            rotationX = .15f;
                            rotationY = .25f;
                            rotationZ = .35f;
                            positionX += 0.0001f;
                        }

                        if (boundary2)
                            positionY -= 0.001f;
                        else
                            positionY += 0.001f;

                        if (positionX > 0.025)
                            boundary1 = true;
                        if (positionX < -0.025)
                            boundary1 = false;

                        if (positionY > 0.1)
                            boundary2 = true;
                        if (positionY < -0.1)
                            boundary2 = false;

                        gameObject.transform.Rotate(rotationX, rotationY, rotationZ, Space.Self);
                        gameObject.transform.position = transform.position + new Vector3(positionX, positionY, 0);
                        break;
                    case 9:
                        if (boundary1)
                            scaleX = -0.001f;
                        else
                            scaleX = 0.001f;

                        timer1 += 1;

                        if (timer1 > 4000)
                            if (boundary1)
                            {
                                timer1 = 0;
                                boundary1 = false;
                            }
                            else
                            {
                                timer1 = 0;
                                boundary1 = true;
                            }

                        gameObject.transform.localScale = transform.localScale + new Vector3(scaleX, 0, 0);
                        break;
                    case 10:
                        if (boundary1)
                            scaleY = -0.001f;
                        else
                            scaleY = 0.001f;

                        timer1 += 1;

                        if (timer1 > 4000)
                            if (boundary1)
                            {
                                timer1 = 0;
                                boundary1 = false;
                            }
                            else
                            {
                                timer1 = 0;
                                boundary1 = true;
                            }

                        gameObject.transform.localScale = transform.localScale + new Vector3(0, scaleY, 0);
                        break;
                    case 11:
                        if (boundary1)
                        {
                            scaleX = -0.001f;
                            scaleY = -0.001f;
                        }
                        else
                        {
                            scaleX = 0.001f;
                            scaleY = 0.001f;
                        }
                        timer1 += 1;

                        if (timer1 > 4000)
                            if (boundary1)
                            {
                                timer1 = 0;
                                boundary1 = false;
                            }
                            else
                            {
                                timer1 = 0;
                                boundary1 = true;
                            }

                        gameObject.transform.localScale = transform.localScale + new Vector3(scaleX, scaleY, 0);
                        break;
                    case 12:
                        if (boundary1)
                        {
                            scaleX = -0.001f;
                            scaleY = 0.001f;
                        }
                        else
                        {
                            scaleX = 0.001f;
                            scaleY = -0.001f;
                        }
                        timer1 += 1;

                        if (timer1 > 4000)
                            if (boundary1)
                            {
                                timer1 = 0;
                                boundary1 = false;
                            }
                            else
                            {
                                timer1 = 0;
                                boundary1 = true;
                            }

                        gameObject.transform.localScale = transform.localScale + new Vector3(scaleX, scaleY, 0);
                        break;
                    case 13:
                        if (boundary1)
                        {
                            positionX -= 0.0001f;
                            scaleX = -0.001f;
                            scaleY = 0.001f;
                        }
                        else
                        {
                            positionX += 0.0001f;
                            scaleX = 0.001f;
                            scaleY = -0.001f;
                        }
                        if (positionX > 0.025)
                            boundary1 = true;
                        if (positionX < -0.025)
                            boundary1 = false;

                        if (boundary2)
                            positionY -= 0.001f;
                        else
                            positionY += 0.001f;

                        if (positionY > 0.1)
                            boundary2 = true;
                        if (positionY < -0.1)
                            boundary2 = false;

                        gameObject.transform.localScale = transform.localScale + new Vector3(scaleX, scaleY, 0);
                        gameObject.transform.position = transform.position + new Vector3(positionX, positionY, 0);
                        break;
                    case 14:
                        if (boundary1)
                        {
                            rotationX = -.1f;
                            rotationY = -.2f;
                            rotationZ = -.3f;
                            scaleX = -0.001f;
                            scaleY = 0.001f;
                        }
                        else
                        {
                            rotationX = .1f;
                            rotationY = .2f;
                            rotationZ = .3f;
                            scaleX = 0.001f;
                            scaleY = -0.001f;
                        }
                        timer1 += 1;

                        if (timer1 > 4000)
                            if (boundary1)
                            {
                                timer1 = 0;
                                boundary1 = false;
                            }
                            else
                            {
                                timer1 = 0;
                                boundary1 = true;
                            }

                        gameObject.transform.localScale = transform.localScale + new Vector3(scaleX, scaleY, 0);
                        gameObject.transform.Rotate(rotationX, rotationY, rotationZ, Space.Self);
                        break;
                    case 15:
                        if (boundary1)
                        {
                            rotationX = -.1f;
                            rotationY = -.2f;
                            rotationZ = -.3f;
                            positionX -= 0.0001f;
                            scaleX = -0.001f;
                            scaleY = 0.001f;
                        }
                        else
                        {
                            rotationX = .1f;
                            rotationY = .2f;
                            rotationZ = .3f;
                            positionX += 0.0001f;
                            scaleX = 0.001f;
                            scaleY = -0.001f;
                        }
                        timer1 += 1;

                        if (boundary2)
                            positionY -= 0.001f;
                        else
                            positionY += 0.001f;

                        if (positionX > 0.025)
                            boundary1 = true;
                        if (positionX < -0.025)
                            boundary1 = false;

                        if (positionY > 0.1)
                            boundary2 = true;
                        if (positionY < -0.1)
                            boundary2 = false;

                        gameObject.transform.localScale = transform.localScale + new Vector3(scaleX, scaleY, 0);
                        gameObject.transform.Rotate(rotationX, rotationY, rotationZ, Space.Self);
                        gameObject.transform.position = transform.position + new Vector3(positionX, positionY, 0);
                        break;
                }
            }
            //Stop if there is only one player left alive and none on the belt
            if (!IsShop && AlivePlayers.Count <= ((_waitForAllDead) ? 0 : 1) && ConveyorBelt.Count <= 0)
                break;

            if (IsShop && AlivePlayers.Count <= 0 && ConveyorBelt.Count <= 0)
                break;

            if (_forceEndGameplay)
                break;

            if(_tileGameplayTimeElapsed > 240)
            {
                Debug.LogError("Finishing tile due exciting gameplay duration limit"); 
                break;
            }

            _tileGameplayTimeElapsed += Time.deltaTime;
            yield return null;
        }

        if (TileEffect != 0)
        {
            gameObject.transform.position = StartPosition;
            gameObject.transform.rotation = StartRotation;
            gameObject.transform.localScale = StartScale;
        }

        TileEffect = 0;        

        Debug.Log($"about to start podium: tileTimeElapsed: {_timer} tileDuration:{_tileDurationS} alivePlayers:{AlivePlayers.Count} ConveyorBelt:{ConveyorBelt.Count}");

        //Freeze the physics on any winning survivors so that they don't die while the podium is going up
        foreach(var ph in AlivePlayers)
        {
            if (ph.pb != null)
                ph.pb.EnableKinematicMode(); 
        }

        long _playerPointsSumFinish = 0;
        foreach (var player in Players)
            _playerPointsSumFinish += player.pp.SessionScore;

        if(Players.Count > 0)
            Debug.Log($"Tile {name} Rarity {RarityType} Gameplay finished in {(DateTime.Now - _tileStartTime).ToString(@"hh\:mm\:ss")}\n" +
                      $"Total Points Distributed: {(_playerPointsSumFinish - _playerPointsSumStart)}\n" + 
                      $"Average Points Gained Per Player: {(_playerPointsSumFinish - _playerPointsSumStart) / Players.Count}");


        if (!IsShop && Players.Count > 0)
            yield return _tc.Podium.RunPodium(this, Players);

        yield return null; //Wait one frame to allow time for bid handler to switch in case the tile has no bidders and instantly spins after countdown
        FinishTile(); 
    }

    private void AnnounceMystery(int Mystery)
    {
        switch (Mystery)
        {
            case 0:
                MyTTS.inst.PlayerSpeech("Shop Tile", 2);
                break;
            case 1:
                MyTTS.inst.PlayerSpeech("Null Tile", 2);
                break;
            case 2:
                MyTTS.inst.PlayerSpeech("Cursed Tile", 2);
                break;
            case 3:
                MyTTS.inst.PlayerSpeech("10x Multiplier", 2);
                break;
            case 4:
                MyTTS.inst.PlayerSpeech("50x Multiplier", 2);
                break;
            case 5:
                MyTTS.inst.PlayerSpeech("100x Multiplier", 2);
                break;
            case 6:
                MyTTS.inst.PlayerSpeech("1000x Multiplier", 2);
                break;
            case 7:
                MyTTS.inst.PlayerSpeech("Sway", 2);
                break;
            case 8:
                MyTTS.inst.PlayerSpeech("Elevator", 2);
                break;
            case 9:
                MyTTS.inst.PlayerSpeech("Slaunchwise", 2);
                break;
            case 10:
                MyTTS.inst.PlayerSpeech("Flip", 2);
                break;
            case 11:
                MyTTS.inst.PlayerSpeech("Flop", 2);
                break;
            case 12:
                MyTTS.inst.PlayerSpeech("Florp", 2);
                break;
            case 13:
                MyTTS.inst.PlayerSpeech("Space Training", 2);
                break;
            case 14:
                MyTTS.inst.PlayerSpeech("Hyperspace Training", 2);
                break;
            case 15:
                MyTTS.inst.PlayerSpeech("Wide", 2);
                break;
            case 16:
                MyTTS.inst.PlayerSpeech("Tall", 2);
                break;
            case 17:
                MyTTS.inst.PlayerSpeech("Embiggen", 2);
                break;
            case 18:
                MyTTS.inst.PlayerSpeech("Squishy", 2);
                break;
            case 19:
                MyTTS.inst.PlayerSpeech("Squishy Slaunch", 2);
                break;
            case 20:
                MyTTS.inst.PlayerSpeech("Squishy Space Training", 2);
                break;
            case 21:
                MyTTS.inst.PlayerSpeech("Slaunchwise Squish Training", 2);
                break;
            case 22:
                MyTTS.inst.PlayerSpeech("500x", 2);
                break;

        }
    }

    public void EliminatePlayer(PlayerHandler ph, bool setRankScoreByElimOrder)
    {
        int rankScore = -1;
        if(setRankScoreByElimOrder)
            rankScore = EliminatedPlayers.Count;

        EliminatePlayer(ph, rankScore); 
    }

    public void EliminatePlayer(PlayerHandler ph, int rankScoreOverride = -1)
    {
        ph.SetState(PlayerHandlerState.Idle);

        //You can only be eliminated once
        if (!EliminatedPlayers.Contains(ph))
            EliminatedPlayers.Add(ph);

        if (rankScoreOverride != -1)
            ph.SetRankScore(rankScoreOverride);

        AlivePlayers.Remove(ph);
        ConveyorBelt.Remove(ph); //In case the player somehow died on the belt due to bug

        ph.GetPlayerBall().ExplodeBall();

    }

    public void EliminatePlayers(List<PlayerHandler> phs, bool setRankScoreByElimOrder)
    {
        int rankScore = -1;
        if(setRankScoreByElimOrder)
            rankScore = Players.Count - AlivePlayers.Count;

        foreach(var ph in phs)
            EliminatePlayer(ph, rankScore); 
    }

    public void ProcessGameplayCommand(string messageId, TwitchClient twitchClient, PlayerHandler ph, string msg, string rawEmotesRemoved)
    {
        if (_game == null)
            return;

        //If the game is not started, don't process any gameplay commands
        if (!_game.IsGameStarted)
            return;

        _game.ProcessGameplayCommand(messageId, twitchClient, ph, msg, rawEmotesRemoved);
    }


    //TODO: Backup timer, if all but one player isn't eliminated by the time this timer runs out, make the remaining players tie for first
    public IEnumerator RunTimer(int durationS)
    {
        _timer = 0;
        while (_timer < durationS)
        {
            float t = _timer / durationS;
            SetBackgroundShader(1 - t);
            _timerText.SetText( MyUtil.GetMinuteSecString((int)(durationS - _timer)));

            yield return new WaitForSeconds(1);

            _timer++;
        }
        SetBackgroundShader(0);
        _timerText.SetText(MyUtil.GetMinuteSecString(0));

    }

    private void UpdateTileTimer()
    {
        float t = _timer / _tileDurationS;

        if(t >= 1)
        {
            FinishTile();
            return;
        }

        SetBackgroundShader(1 - t); 
    }

    public void SetBackgroundShader(float t)
    {
        _background.GetPropertyBlock(_mpb);
        _mpb.SetFloat("_FillAmount", t);
        _background.SetPropertyBlock(_mpb);
    }

    public PlayerHandler GetAlivePlayerViaUsername(string twitchUsername)
    {
        //Check if the usernames match, ignore upper vs lowercase
        return AlivePlayers.Find(ph => string.Equals(ph.pp.TwitchUsername, twitchUsername, System.StringComparison.OrdinalIgnoreCase));
    }

    public void FinishTile()
    {
        IsCurse = false;
        IsGolden = false;
        IsRuby = false;
        IsMystery = false;
        IsNull = false;
        _goldenVisuals._coverObj.gameObject.SetActive(false);

        //TileActive = false;
        TileState = TileState.Inactive; 
        if (_game != null)
        {
            _game.CleanUpGame();
            _game.IsGameStarted = false;
        }

        _suddenDeath.CleanUp();

        Players.Clear();
        AlivePlayers.Clear();
        EliminatedPlayers.Clear();        

        //Once the gameplay tile finishes, spin it to a new tile
        _tc.SpinNewTile(this);        
    }

    public void SetTicketBonus(int count)
    {
        if (IsShop)
            return;

        TicketBonusAmount = count;
        UpdateTicketBonusText();
        _ticketBonusAmountText.enabled = true; 
    }

    private void ResetTicketBonus()
    {
        TicketBonusAmount = 0;
        _ticketBonusAmountText.enabled = false;
        UpdateTicketBonusText(); 
    }

    private void UpdateTicketBonusText()
    {
        _ticketBonusAmountText.SetText("Win Prize: " + MyUtil.AbbreviateNum4Char(TicketBonusAmount)); 
    }

    public Vector3 GetTicketBonusAmountPos()
    {
        return _ticketBonusAmountText.transform.position;
    }
    public TileController GetTileController()
    {
        return _tc;
    }
    public Game GetGame()
    {
        return _game;
    }
    public SuddenDeath GetSuddenDeath()
    {
        return _suddenDeath;
    }

    public void ForceEndGameplay()
    {
        _forceEndGameplay = true; 
    }

    public Vector3 GetGoldSpawnPos()
    {
        return _rarityText.transform.position;
    }
}
