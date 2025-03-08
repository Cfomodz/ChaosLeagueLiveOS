using Amazon.Runtime.Internal.Transform;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TwitchLib.PubSub.Models.Responses.Messages.Redemption;
using UnityEngine;
using UnityEngine.UIElements;

public class TileController : MonoBehaviour
{
    [SerializeField] private GameManager _gm; 
    [SerializeField] private KingController _kingController;
    [SerializeField] private AutoPredictions _autoPredictions;
    [SerializeField] public Podium Podium;
    [SerializeField] private TwitchClient _twitchClient;
    [SerializeField] private RebellionController _rebellionController;
    [SerializeField] private GoldDistributor _goldDistributor;
    [SerializeField] public NPCHandler _npcHandler;

 //   private static float _commonRarity;
    private static float _rareRarity;
    private static float _epicRarity;
    private static float _legendaryRarity;
    private static float _mythicRarity;
    private static float _etherealRarity;
    private static float _cosmicRarity;
    private static float _ExtraRarity1;
    private static float _ExtraRarity2;
    private static float _ExtraRarity3;
    private static float _ExtraRarity4;
    private static float _ExtraRarity5;
    private static float _ExtraRarity6;
    private static float _ExtraRarity7;

    [SerializeField] private List<GameTile> AllRarities; //Adds one to each rarity 
    [SerializeField] private List<GameTile> FirstRemove; //Common - Legendary
    [SerializeField] private List<GameTile> SecondRemove; //Rare - Mythic
    [SerializeField] private List<GameTile> CommonTiles; //69%      // 45%
    [SerializeField] private List<GameTile> RareTiles; // 25%       // 30%
    [SerializeField] private List<GameTile> EpicTiles; // 5%        // 18%
    [SerializeField] private List<GameTile> LegendaryTiles; // 1%   // 5%
    [SerializeField] private List<GameTile> MythicTiles; // X       // 1.5%
    [SerializeField] private List<GameTile> EtherealTiles; // X     // 0.3%
    [SerializeField] private List<GameTile> CosmicTiles; // X       // 0.2%

    public Transform HoldingPen;
    public Transform TilesRoot;

    public Transform LeftTileCenter;
    public Transform CenterTileCenter;
    public Transform RightTileCenter;

    [SerializeField] public BidHandler BidHandler;

    [SerializeField] public GameTile GameplayTile;
    [SerializeField] public GameTile CurrentBiddingTile; 
    [SerializeField] public GameTile NextBiddingTile;
    [SerializeField] private GameTile ForceThisTileNext;
    [SerializeField] private RarityType _forceThisRarity;

    [SerializeField] public bool _forceCurse;
    [SerializeField] public bool _forceGolden; 
    [SerializeField] public bool _forceRuby; 
    [SerializeField] public bool _forceMystery; 
    [SerializeField] public bool _forceNull; 
    
    [SerializeField] public bool _SpinningNow; 
    [SerializeField] public bool _TileRepeating; 

    private Dictionary<int, ObjectPool<GameTile>> _tilePools = new Dictionary<int, ObjectPool<GameTile>>(); 

    [SerializeField] private float _rotationDurationS = 10;
    [SerializeField] private int _animeTileCountMin;
    [SerializeField] private int _animeTileCountMax;
    [SerializeField] private float _animeRotateDegreesPerTile;

    [SerializeField] private AnimationCurve _animeSpeedCurve;

    [SerializeField] private List<Color> _pipeColors;
    [Space(5)]
    [SerializeField] private Color _commonStartColor;
    [SerializeField] private Color _commonEndColor;
    [SerializeField] private Color _commonTrimColor;
    [Space(5)]
    [SerializeField] private Color _rareStartColor;
    [SerializeField] private Color _rareEndColor;
    [SerializeField] private Color _rareTrimColor;
    [Space(5)]
    [SerializeField] private Color _epicStartColor;
    [SerializeField] private Color _epicEndColor;
    [SerializeField] private Color _epicTrimColor;
    [Space(5)]
    [SerializeField] private Color _legendaryStartColor;
    [SerializeField] private Color _legendaryEndColor;
    [SerializeField] private Color _legendaryTrimColor;
    [Space(5)]
    [SerializeField] private Color _mythicStartColor;
    [SerializeField] private Color _mythicEndColor;
    [SerializeField] private Color _mythicTrimColor;
    [Space(5)]
    [SerializeField] private Color _etherealStartColor;
    [SerializeField] private Color _etherealEndColor;
    [SerializeField] private Color _etherealTrimColor;
    [Space(5)]
    [SerializeField] private Color _cosmicStartColor;
    [SerializeField] private Color _cosmicEndColor;
    [SerializeField] private Color _cosmicTrimColor;

    private int _tileCounter = 0; 
    private int _promptIndex = 0;

    public bool IsRisky;

    private void Start()
    {

        //Set all the tile ID numbers
        int tileID = 0;
        foreach (GameTile tile in AllRarities)
        {
            tile.TileIDNum = tileID++;
        }
        foreach (GameTile tile in FirstRemove)
        {
            tile.TileIDNum = tileID++;
        }
        foreach (GameTile tile in SecondRemove)
        {
            tile.TileIDNum = tileID++;
        }
        foreach (GameTile tile in CommonTiles)
        {
            tile.TileIDNum = tileID++;
            tile.SetRarity(RarityType.Common, _commonStartColor, _commonEndColor, _commonTrimColor); 
        }
        foreach (GameTile tile in RareTiles)
        {
            tile.TileIDNum = tileID++;
            tile.SetRarity(RarityType.Rare, _rareStartColor, _rareEndColor, _rareTrimColor);
        }
        foreach (GameTile tile in EpicTiles)
        {
            tile.TileIDNum = tileID++;
            tile.SetRarity(RarityType.Epic, _epicStartColor, _epicEndColor, _epicTrimColor);
        }
        foreach (GameTile tile in LegendaryTiles)
        {
            tile.TileIDNum = tileID++;
            tile.SetRarity(RarityType.Legendary, _legendaryStartColor, _legendaryEndColor, _legendaryTrimColor);
        }
        foreach (GameTile tile in MythicTiles)
        {
            tile.TileIDNum = tileID++;
            tile.SetRarity(RarityType.Mythic, _mythicStartColor, _mythicEndColor, _mythicTrimColor);
        }
        foreach (GameTile tile in EtherealTiles)
        {
            tile.TileIDNum = tileID++;
            tile.SetRarity(RarityType.Ethereal, _etherealStartColor, _etherealEndColor, _etherealTrimColor);
        }
        foreach (GameTile tile in CosmicTiles)
        {
            tile.TileIDNum = tileID++;
            tile.SetRarity(RarityType.Cosmic, _cosmicStartColor, _cosmicEndColor, _cosmicTrimColor);
        }
        List<GameTile> tilePossibilities = new List<GameTile>();
        tilePossibilities = tilePossibilities.Concat(AllRarities).Concat(FirstRemove).Concat(SecondRemove).Concat(CommonTiles).Concat(RareTiles).Concat(EpicTiles).Concat(LegendaryTiles).Concat(MythicTiles).Concat(EtherealTiles).Concat(CosmicTiles).ToList(); 

        //Create a pool for each tile type
        foreach (GameTile tile in tilePossibilities)
            _tilePools.Add(tile.TileIDNum, new ObjectPool<GameTile>(() => {
                GameTile newTile = Instantiate(tile.gameObject).GetComponent<GameTile>();
                newTile.gameObject.name = "tile" + _tileCounter++;
                newTile.TileState = TileState.Inactive;
                return newTile; 
            }, 
            PoolTurnOnGT, 
            PoolTurnOffGT));


        CurrentBiddingTile = SpawnOriginTile(LeftTileCenter.position, Side.Left, false);
        CurrentBiddingTile.PreInitTile(this, _forceGolden, _forceRuby, _forceCurse, _forceMystery, _forceNull);
        CurrentBiddingTile.InitTileInPos();

        StartCoroutine(BidHandler.RunBiddingOn(CurrentBiddingTile)); 
        
        GameplayTile = SpawnOriginTile(RightTileCenter.position, Side.Right, false);
        GameplayTile.PreInitTile(this, _forceGolden, _forceRuby, _forceCurse, _forceMystery, _forceNull); 
        GameplayTile.InitTileInPos();
        StartCoroutine(GameplayTile.RunTile());
    }

    public void PoolTurnOnGT(GameTile tile)
    {
        tile.gameObject.SetActive(true);

        tile.TogglePhysics(false);
    }

    public void PoolTurnOffGT(GameTile tile)
    {
        //tile.transform.position = _tc.HoldingPen.position;
        tile.gameObject.SetActive(false);
        tile.transform.position = new Vector3(100, 0, 0);

    }
    public void ProcessMsgInTiles(PlayerHandler ph)
    {
/*        //Receive the stream event in each of the tiles 
        foreach (GameTile tile in _activeTiles)
            tile.ProcessPBMsg(ph, se);*/
    }

    public string getGameplayTile()
    {
        if (GameplayTile == null)
            return "Okay";
        else
            return "NotOkay";
    }

    public string getNextForcedTile()
    {
        if (ForceThisTileNext == null)
            return "Okay";
        else
            return "NotOkay";
    }

    public void doRepeatTile()
    {
        if (GameplayTile == null)
        {
            ForceThisTileNext = CurrentBiddingTile;
            _forceThisRarity = CurrentBiddingTile.GetRarity();
            CurrentBiddingTile._indicator2.SetText("🔁");
        }
        else
        {
            ForceThisTileNext = GameplayTile;
            _forceThisRarity = GameplayTile.GetRarity();
            GameplayTile._indicator2.SetText("🔁");
        }

        _TileRepeating = true;
    }

    public void doUpgradeTile()
    {
        RarityType rarity = RarityType.Common;
        int UpgradeChance = Random.Range(1, 101);

        if (UpgradeChance < 2)
        {
            if (GameplayTile == null)
            {
                rarity = CurrentBiddingTile.GetRarity();
                CurrentBiddingTile._indicator2.SetText("↓↓↓");
            }
            else
            {
                rarity = GameplayTile.GetRarity();
                GameplayTile._indicator2.SetText("↓↓↓");
            }

            rarity = RarityType.Common;

            _forceThisRarity = rarity;
        }
        else if (UpgradeChance < 11)
        {
            if (GameplayTile == null)
            {
                rarity = CurrentBiddingTile.GetRarity();
                CurrentBiddingTile._indicator2.SetText("↑↑");
            }
            else
            {
                rarity = GameplayTile.GetRarity();
                GameplayTile._indicator2.SetText("↑↑");
            }

            if (rarity == RarityType.Common)
                rarity = RarityType.CommonPlus;
            else if (rarity == RarityType.Rare)
                rarity = RarityType.RarePlus;
            else if (rarity == RarityType.Epic)
                rarity = RarityType.EpicPlus;
            else if (rarity == RarityType.Legendary)
                rarity = RarityType.LegendaryPlus;
            else if (rarity == RarityType.Mythic)
                rarity = RarityType.MythicPlus;
            else if (rarity == RarityType.Ethereal)
                rarity = RarityType.EtherealPlus;
            else if (rarity == RarityType.Cosmic)
                rarity = RarityType.CosmicPlus;
            else if (rarity == RarityType.CommonPlus)
                rarity = RarityType.SuperCommon;
            else if (rarity == RarityType.RarePlus)
                rarity = RarityType.SuperRare;
            else if (rarity == RarityType.EpicPlus)
                rarity = RarityType.SuperEpic;
            else if (rarity == RarityType.LegendaryPlus)
                rarity = RarityType.SuperLegendary;
            else if (rarity == RarityType.MythicPlus)
                rarity = RarityType.SuperMythic;
            else if (rarity == RarityType.EtherealPlus)
                rarity = RarityType.SuperEthereal;
            else if (rarity == RarityType.CosmicPlus)
                rarity = RarityType.SuperCosmic;
            else if (rarity == RarityType.SuperCommon)
                rarity = RarityType.SuperRare;
            else if (rarity == RarityType.SuperRare)
                rarity = RarityType.SuperEpic;
            else if (rarity == RarityType.SuperEpic)
                rarity = RarityType.SuperLegendary;
            else if (rarity == RarityType.SuperLegendary)
                rarity = RarityType.SuperMythic;
            else if (rarity == RarityType.SuperMythic)
                rarity = RarityType.SuperEthereal;
            else if (rarity == RarityType.SuperEthereal)
                rarity = RarityType.SuperCosmic;

            _forceThisRarity = rarity;
        }
        else
        {

            if (GameplayTile == null)
            {
                rarity = CurrentBiddingTile.GetRarity();
                CurrentBiddingTile._indicator2.SetText("↑");
            }
            else
            {
                rarity = GameplayTile.GetRarity();
                GameplayTile._indicator2.SetText("↑");
            }

            if (rarity == RarityType.Common)
                rarity = RarityType.Rare;
            else if (rarity == RarityType.Rare)
                rarity = RarityType.Epic;
            else if (rarity == RarityType.Epic)
                rarity = RarityType.Legendary;
            else if (rarity == RarityType.Legendary)
                rarity = RarityType.Mythic;
            else if (rarity == RarityType.Mythic)
                rarity = RarityType.Ethereal;
            else if (rarity == RarityType.Ethereal)
                rarity = RarityType.Cosmic;
            else if (rarity == RarityType.Cosmic)
                rarity = RarityType.CommonPlus;
            else if (rarity == RarityType.CommonPlus)
                rarity = RarityType.RarePlus;
            else if (rarity == RarityType.RarePlus)
                rarity = RarityType.EpicPlus;
            else if (rarity == RarityType.EpicPlus)
                rarity = RarityType.LegendaryPlus;
            else if (rarity == RarityType.LegendaryPlus)
                rarity = RarityType.MythicPlus;
            else if (rarity == RarityType.MythicPlus)
                rarity = RarityType.EtherealPlus;
            else if (rarity == RarityType.EtherealPlus)
                rarity = RarityType.CosmicPlus;
            else if (rarity == RarityType.CosmicPlus)
                rarity = RarityType.SuperCommon;
            else if (rarity == RarityType.SuperCommon)
                rarity = RarityType.SuperRare;
            else if (rarity == RarityType.SuperRare)
                rarity = RarityType.SuperEpic;
            else if (rarity == RarityType.SuperEpic)
                rarity = RarityType.SuperLegendary;
            else if (rarity == RarityType.SuperLegendary)
                rarity = RarityType.SuperMythic;
            else if (rarity == RarityType.SuperMythic)
                rarity = RarityType.SuperEthereal;
            else if (rarity == RarityType.SuperEthereal)
                rarity = RarityType.SuperCosmic;

            _forceThisRarity = rarity;
        }
    }

    public GameTile SpawnOriginTile(Vector3 pos, Side side, bool spinNew) 
    {
        GameTile gt; 
        if (ForceThisTileNext != null)
        {
            gt = GetTileByTileID(ForceThisTileNext.TileIDNum, _forceThisRarity, side);
            ForceThisTileNext = null;
        }
        else
            gt = GetRandomTile(null, side);

        gt.transform.position = pos; 
        if(spinNew)
            return SpinNewTile(gt);

        return gt;
    }

    //Have to start the coroutine here. If I start it in the gametile, when the tile is disabled, it will stop the coroutine
    public GameTile SpinNewTile(GameTile gt)
    {
        //Don't allow the same tile to spin, or a match for any other active file
        List<int> blacklistedTiles = new List<int>
            {
                gt.TileIDNum,
                CurrentBiddingTile.TileIDNum
            };
        if (GameplayTile != null)
        {
            blacklistedTiles.Add(GameplayTile.TileIDNum);
            GameplayTile._indicator1.SetText("");
            GameplayTile._indicator2.SetText("");
        }

        GameTile nextTile;
        if(ForceThisTileNext != null)
        {
            nextTile = GetTileByTileID(ForceThisTileNext.TileIDNum, _forceThisRarity, gt.CurrentSide);
            ForceThisTileNext = null; 
        } 
        else
             nextTile = GetRandomTile(blacklistedTiles, gt.CurrentSide);
        NextBiddingTile = nextTile;
        GameplayTile = null; 
        StartCoroutine(CSpinNewTile(gt, nextTile));

        _SpinningNow = true;

        return nextTile; 
    }


    //Can pass in null for the forceThisTileNext if you don't want to 
    private IEnumerator CSpinNewTile(GameTile gt, GameTile nextTile)
    {        
        _rebellionController.OnNewTileSpin(); 
        //Choose amount of random tiles and put them in a list
        List<GameTile> animeTiles = new List<GameTile>{ gt };//Add the current tile to the animatino

        int _animeTileCount = Random.Range(_animeTileCountMin, _animeTileCountMax);
        for(int i = 0; i < _animeTileCount; i++)
            animeTiles.Add(GetRandomTile(null, gt.CurrentSide));

        animeTiles.Add(nextTile);

        //Init all the tiles visually except for the first one (that just finished gameplay)
        for (int i = 1; i < animeTiles.Count; i++)
        {
            GameTile tile = animeTiles[i];
            bool isGolden = false;
            bool isRuby = false;
            bool isCurse = false;
            bool isMystery = false;
            bool isNull = false;
            float random = Random.Range(0f, 100f);

            if (random <= 0.001f)
                isMystery = true;
            else if (random <= 0.05f)
                isRuby = true;
            else if (random <= 0.25f)
                isCurse = true;
            else if (random <= 1f)
                isGolden = true;

            if (_forceGolden)
                isGolden = true;

            if (_forceRuby)
                isRuby = true;

            if (_forceCurse)
                isCurse = true;

            if (_forceMystery)
                isMystery = true;

            if (_forceNull)
                isNull = true;

            if (gt.IsShop)
            {
                isGolden = false;
                isRuby = false;
                isCurse = false; 
                isNull = false;
            }

            tile.PreInitTile(this, isGolden, isRuby, isCurse, isMystery, isNull);
        }

        Vector3 finalTilePos = gt.transform.position;
        Quaternion finalTileRot = gt.transform.rotation;

        Vector3 rotatePoint = finalTilePos + Vector3.forward * 50;

        //Play the spin animation
        yield return SpinAnimation(animeTiles, rotatePoint, finalTilePos);        

        //Move all the random animation tiles back to the holding pen, and init the selected one
        for (int i = 0; i < animeTiles.Count; i++)
        {
            GameTile tile = animeTiles[i];

            if (i == animeTiles.Count - 1) //If the last tile in the animation
            {
                tile.transform.SetPositionAndRotation(finalTilePos, finalTileRot);
                tile.InitTileInPos();
                tile.Sponsored = false;
                _goldDistributor.SpawnGoldFromTileRarity(tile); 
                _goldDistributor.SpawnSapphiresFromTileRarity(tile, tile.SponsorHandler); 
                if (tile.RarityType == RarityType.Legendary)
                    _autoPredictions.LegendarySignal(); 
                else if (tile.RarityType == RarityType.Mythic)
                    _autoPredictions.LegendarySignal();
                else if (tile.RarityType == RarityType.Ethereal)
                    _autoPredictions.LegendarySignal();
                else if (tile.RarityType == RarityType.Cosmic)
                    _autoPredictions.LegendarySignal();
                continue;
            }
            _tilePools[tile.TileIDNum].ReturnObject(tile);
        }

        _SpinningNow = false;        
        _TileRepeating = false;       

        StartCoroutine(_npcHandler.CheckWaiter());
    }

    public IEnumerator SpinAnimation(List<GameTile> animeTiles, Vector3 rotatePoint, Vector3 finalTilePos)
    {

        float rotationTimer = 0;
        float totalRotation = (animeTiles.Count - 1) * _animeRotateDegreesPerTile;
        float t = 0;

        AudioController.inst.PlaySound(AudioController.inst.TileSpinStart, 0.95f, 1.05f);

        while (rotationTimer < _rotationDurationS)
        {
            for (int i = 0; i < animeTiles.Count; i++)
            {
                GameTile tile = animeTiles[i];
                tile.transform.SetPositionAndRotation(finalTilePos, Quaternion.identity);
                float degrees = i * _animeRotateDegreesPerTile - (totalRotation * t);

                //Cull if out of view
                if (degrees > -25 && degrees < 25)
                    tile.gameObject.SetActive(true);
                else
                    tile.gameObject.SetActive(false);

                tile.transform.RotateAround(rotatePoint, Vector3.right, degrees);
            }

            rotationTimer += Time.deltaTime;

            t = _animeSpeedCurve.Evaluate(rotationTimer / _rotationDurationS);

            yield return null;
        }
        //Debug.Log("DONE WITH spin animation with animeTiles count: " + animeTiles.Count + "names: " + debugString);

        if (_gm.GetKingController().currentKing != null)
            _gm.GetKingController().UpdateGoldText();
    }

    public (int, RarityType) GetRandomIDandRarity(List<int> blacklistedTiles)
    {
        GameTile tile;
        RarityType rarity;
        CheckRarityEvent();
        do
        {
            //Select the tile
            float t = Random.Range(0f, 1f);
            if (t <= _ExtraRarity7)
            {
                int index = Random.Range(0, AllRarities.Count);
                tile = AllRarities[index];
                rarity = RarityType.CosmicPlus;
            }
            else if (t <= _ExtraRarity6 + _ExtraRarity7)
            {
                int index = Random.Range(0, AllRarities.Count);
                tile = AllRarities[index];
                rarity = RarityType.EtherealPlus;
            }
            else if (t <= _ExtraRarity5 + _ExtraRarity6 + _ExtraRarity7)
            {
                int index = Random.Range(0, AllRarities.Count);
                tile = AllRarities[index];
                rarity = RarityType.MythicPlus;
            }
            else if (t <= _ExtraRarity4 + _ExtraRarity5 + _ExtraRarity6 + _ExtraRarity7)
            {
                int index = Random.Range(0, AllRarities.Count);
                tile = AllRarities[index];
                rarity = RarityType.LegendaryPlus;
            }
            else if (t <= _ExtraRarity3 + _ExtraRarity4 + _ExtraRarity5 + _ExtraRarity6 + _ExtraRarity7)
            {
                int index = Random.Range(0, AllRarities.Count);
                tile = AllRarities[index];
                rarity = RarityType.EpicPlus;
            }
            else if (t <= _ExtraRarity2 + _ExtraRarity3 + _ExtraRarity4 + _ExtraRarity5 + _ExtraRarity6 + _ExtraRarity7)
            {
                int index = Random.Range(0, AllRarities.Count);
                tile = AllRarities[index];
                rarity = RarityType.SuperCommon;
            }
            else if (t <= _ExtraRarity1 + _ExtraRarity2 + _ExtraRarity3 + _ExtraRarity4 + _ExtraRarity5 + _ExtraRarity6 + _ExtraRarity7)
            {
                int index = Random.Range(0, AllRarities.Count);
                tile = AllRarities[index];
                rarity = RarityType.CommonPlus;
            }
            else if (t <= _cosmicRarity + _ExtraRarity1 + _ExtraRarity2 + _ExtraRarity3 + _ExtraRarity4 + _ExtraRarity5 + _ExtraRarity6 + _ExtraRarity7)
            {
                int index = Random.Range(0, CosmicTiles.Count + AllRarities.Count);
                if (index < CosmicTiles.Count)
                {
                    tile = CosmicTiles[index];
                }
                else
                {
                    tile = AllRarities[index - CosmicTiles.Count];
                }
                rarity = RarityType.Cosmic;
            }
            else if (t <= _cosmicRarity + _etherealRarity + _ExtraRarity1 + _ExtraRarity2 + _ExtraRarity3 + _ExtraRarity4 + _ExtraRarity5 + _ExtraRarity6 + _ExtraRarity7)
            {
                int index = Random.Range(0, EtherealTiles.Count + AllRarities.Count);
                if (index < EtherealTiles.Count)
                {
                    tile = EtherealTiles[index];
                }
                else
                {
                    tile = AllRarities[index - EtherealTiles.Count];
                }
                rarity = RarityType.Ethereal;
            }
            else if (t <= _cosmicRarity + _etherealRarity + _mythicRarity + _ExtraRarity1 + _ExtraRarity2 + _ExtraRarity3 + _ExtraRarity4 + _ExtraRarity5 + _ExtraRarity6 + _ExtraRarity7)
            {
                int index = Random.Range(0, MythicTiles.Count + AllRarities.Count + SecondRemove.Count);
                if (index < MythicTiles.Count)
                {
                    tile = MythicTiles[index];
                }
                else if (index < MythicTiles.Count + AllRarities.Count)
                {
                    tile = AllRarities[index - MythicTiles.Count];
                }
                else
                {
                    tile = SecondRemove[index - MythicTiles.Count - AllRarities.Count];
                }
                rarity = RarityType.Mythic;
            }
            else if (t <= _cosmicRarity + _etherealRarity + _mythicRarity + _legendaryRarity + _ExtraRarity1 + _ExtraRarity2 + _ExtraRarity3 + _ExtraRarity4 + _ExtraRarity5 + _ExtraRarity6 + _ExtraRarity7)
            {
                int index = Random.Range(0, LegendaryTiles.Count + AllRarities.Count + FirstRemove.Count + SecondRemove.Count);
                if (index < LegendaryTiles.Count)
                {
                    tile = LegendaryTiles[index];
                }
                else if (index < LegendaryTiles.Count + AllRarities.Count)
                {
                    tile = AllRarities[index - LegendaryTiles.Count];
                }
                else if (index < LegendaryTiles.Count + AllRarities.Count + FirstRemove.Count)
                {
                    tile = FirstRemove[index - LegendaryTiles.Count - AllRarities.Count];
                }
                else
                {
                    tile = SecondRemove[index - LegendaryTiles.Count - AllRarities.Count - FirstRemove.Count];
                }
                rarity = RarityType.Legendary;
            }
            else if (t <= _cosmicRarity + _etherealRarity + _mythicRarity + _legendaryRarity + _epicRarity + _ExtraRarity1 + _ExtraRarity2 + _ExtraRarity3 + _ExtraRarity4 + _ExtraRarity5 + _ExtraRarity6 + _ExtraRarity7)
            {
                int index = Random.Range(0, EpicTiles.Count + AllRarities.Count + FirstRemove.Count + SecondRemove.Count);
                if (index < EpicTiles.Count)
                {
                    tile = EpicTiles[index];
                }
                else if (index < EpicTiles.Count + AllRarities.Count)
                {
                    tile = AllRarities[index - EpicTiles.Count];
                }
                else if (index < EpicTiles.Count + AllRarities.Count + FirstRemove.Count)
                {
                    tile = FirstRemove[index - EpicTiles.Count - AllRarities.Count];
                }
                else
                {
                    tile = SecondRemove[index - EpicTiles.Count - AllRarities.Count - FirstRemove.Count];
                }
                rarity = RarityType.Epic;
            }
            else if (t <= _cosmicRarity + _etherealRarity + _mythicRarity + _legendaryRarity + _epicRarity + _rareRarity + _ExtraRarity1 + _ExtraRarity2 + _ExtraRarity3 + _ExtraRarity4 + _ExtraRarity5 + _ExtraRarity6 + _ExtraRarity7)
            {
                int index = Random.Range(0, RareTiles.Count + AllRarities.Count + FirstRemove.Count + SecondRemove.Count);
                if (index < RareTiles.Count)
                {
                    tile = RareTiles[index];
                }
                else if (index < RareTiles.Count + AllRarities.Count)
                {
                    tile = AllRarities[index - RareTiles.Count];
                }
                else if (index < RareTiles.Count + AllRarities.Count + FirstRemove.Count)
                {
                    tile = FirstRemove[index - RareTiles.Count - AllRarities.Count];
                }
                else
                {
                    tile = SecondRemove[index - RareTiles.Count - AllRarities.Count - FirstRemove.Count];
                }
                rarity = RarityType.Rare;
            }
            else
            {
                int index = Random.Range(0, CommonTiles.Count + AllRarities.Count + FirstRemove.Count);
                if (index < CommonTiles.Count)
                {
                    tile = CommonTiles[index];
                }
                else if (index < CommonTiles.Count + AllRarities.Count)
                {
                    tile = AllRarities[index - CommonTiles.Count];
                }
                else
                {
                    tile = FirstRemove[index - CommonTiles.Count - AllRarities.Count];
                }
                rarity = RarityType.Common;
            }
        } while (blacklistedTiles != null && blacklistedTiles.Any(t => t == tile.TileIDNum));

        return (tile.TileIDNum, rarity);
    }

    public static void CheckRarityEvent()
    {

        if (AppConfig.Wednesday)
        {
        //    _commonRarity = .09f;
            _rareRarity = .15f;
            _epicRarity = .15f;
            _legendaryRarity = .15f;
            _mythicRarity = .15f;
            _etherealRarity = .15f;
            _cosmicRarity = .15f;
            _ExtraRarity1 = .003f;
            _ExtraRarity2 = .002f;
            _ExtraRarity3 = .001f;
            _ExtraRarity4 = .001f;
            _ExtraRarity5 = .001f;
            _ExtraRarity6 = .001f;
            _ExtraRarity7 = .001f;
        }
        else
        {
      //      _commonRarity = .449f;
            _rareRarity = .3f;
            _epicRarity = .18f;
            _legendaryRarity = .05f;
            _mythicRarity = .015f;
            _etherealRarity = .003f;
            _cosmicRarity = .002f;
            _ExtraRarity1 = .0003f;
            _ExtraRarity2 = .0002f;
            _ExtraRarity3 = .0001f;
            _ExtraRarity4 = .0001f;
            _ExtraRarity5 = .0001f;
            _ExtraRarity6 = .0001f;
            _ExtraRarity7 = .0001f;
        }
    }

    public GameTile GetRandomTile(List<int> blacklistedTiles, Side side)
    {
        (int TileIDNum, RarityType rarity) = GetRandomIDandRarity(blacklistedTiles);

        GameTile tile = GetTileByTileID(TileIDNum, rarity, side);

        IsRisky = tile.IsRisk;

        return tile;
    }

    private GameTile GetTileByTileID(int tileID, RarityType rarity, Side side)
    {
        //Get or create it from the pool
        _tilePools.TryGetValue(tileID, out ObjectPool<GameTile> pool);

        if (pool == null)
            Debug.LogError($"Unable to find pool with ID {tileID}");
        

        GameTile tile = pool.GetObject();
        tile.transform.SetParent(TilesRoot);
        tile.CurrentSide = side;

        if (rarity == RarityType.Common)
            tile.SetRarity(RarityType.Common, _commonStartColor, _commonEndColor, _commonTrimColor);
        else if(rarity == RarityType.Rare)
            tile.SetRarity(RarityType.Rare, _rareStartColor, _rareEndColor, _rareTrimColor);
        else if(rarity == RarityType.Epic)
            tile.SetRarity(RarityType.Epic, _epicStartColor, _epicEndColor, _epicTrimColor);
        else if (rarity == RarityType.Legendary)
            tile.SetRarity(RarityType.Legendary, _legendaryStartColor, _legendaryEndColor, _legendaryTrimColor);
        else if (rarity == RarityType.Mythic)
            tile.SetRarity(RarityType.Mythic, _mythicStartColor, _mythicEndColor, _mythicTrimColor);
        else if (rarity == RarityType.Ethereal)
            tile.SetRarity(RarityType.Ethereal, _etherealStartColor, _etherealEndColor, _etherealTrimColor);
        else if (rarity == RarityType.Cosmic)
            tile.SetRarity(RarityType.Cosmic, _cosmicStartColor, _cosmicEndColor, _cosmicTrimColor);
        else if (rarity == RarityType.CommonPlus)
            tile.SetRarity(RarityType.CommonPlus, _commonStartColor, _commonEndColor, _commonTrimColor);
        else if (rarity == RarityType.RarePlus)
            tile.SetRarity(RarityType.RarePlus, _rareStartColor, _rareEndColor, _rareTrimColor);
        else if (rarity == RarityType.EpicPlus)
            tile.SetRarity(RarityType.EpicPlus, _epicStartColor, _epicEndColor, _epicTrimColor);
        else if (rarity == RarityType.LegendaryPlus)
            tile.SetRarity(RarityType.LegendaryPlus, _legendaryStartColor, _legendaryEndColor, _legendaryTrimColor);
        else if (rarity == RarityType.MythicPlus)
            tile.SetRarity(RarityType.MythicPlus, _mythicStartColor, _mythicEndColor, _mythicTrimColor);
        else if (rarity == RarityType.EtherealPlus)
            tile.SetRarity(RarityType.EtherealPlus, _etherealStartColor, _etherealEndColor, _etherealTrimColor);
        else if (rarity == RarityType.CosmicPlus)
            tile.SetRarity(RarityType.CosmicPlus, _cosmicStartColor, _cosmicEndColor, _cosmicTrimColor);
        else if (rarity == RarityType.SuperCommon)
            tile.SetRarity(RarityType.SuperCommon, _commonStartColor, _commonEndColor, _commonTrimColor);
        else if (rarity == RarityType.SuperRare)
            tile.SetRarity(RarityType.SuperRare, _rareStartColor, _rareEndColor, _rareTrimColor);
        else if (rarity == RarityType.SuperEpic)
            tile.SetRarity(RarityType.SuperEpic, _epicStartColor, _epicEndColor, _epicTrimColor);
        else if (rarity == RarityType.SuperLegendary)
            tile.SetRarity(RarityType.SuperLegendary, _legendaryStartColor, _legendaryEndColor, _legendaryTrimColor);
        else if (rarity == RarityType.SuperMythic)
            tile.SetRarity(RarityType.SuperMythic, _mythicStartColor, _mythicEndColor, _mythicTrimColor);
        else if (rarity == RarityType.SuperEthereal)
            tile.SetRarity(RarityType.SuperEthereal, _etherealStartColor, _etherealEndColor, _etherealTrimColor);
        else if (rarity == RarityType.SuperCosmic)
            tile.SetRarity(RarityType.SuperCosmic, _cosmicStartColor, _cosmicEndColor, _cosmicTrimColor);

        return tile;
    }

    public int GetNextPromptIndex()
    {
        int nextIndex = _promptIndex;
        _promptIndex = (_promptIndex + 1) % AppConfig.QuipBattleQuestions.list.Count; 
        return nextIndex;
    }

    public GameManager GetGameManager() { return _gm; }

}
