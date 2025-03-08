using EZCameraShake;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Splines;

public class RebellionController : MonoBehaviour
{
    [SerializeField] private GameManager _gm;
    [SerializeField] private AutoPredictions _autoPredictions;
    [SerializeField] private HoldingPen _holdingPen;

    private List<SuperCapsule> _activeSuperCapsules = new List<SuperCapsule>();
    private List<MultiplierZone> _activeMultiplierZones = new List<MultiplierZone>();

    [SerializeField] private GameObject _superCapsulePrefab;
    [SerializeField] private GameObject _superchatZonePrefab;
    [SerializeField] private TextMeshPro _labelText;

    private ObjectPool<SuperCapsule> _superCapsulePool;
    private ObjectPool<MultiplierZone> _multiplierZonePool;

    [SerializeField] private Transform _capsuleSpawnPos;

    //[SerializeField] private ParticleSystem _particleHype;

    [SerializeField] private Texture defaultPfp;
    [SerializeField] private Transform _capsulesRoot;
    [SerializeField] private Transform _zonesRoot;
    [SerializeField] private Transform _capsuleStackStartPos;

    [SerializeField] private float _capsuleMoveSpeed = 0.1f;

    [SerializeField] private Vector3 _capsulePileSpacing;

    [SerializeField] private Vector2 _dollarColorMap;
    [SerializeField] private Gradient _dollarToColor;
    [SerializeField] private Gradient _dollarToTextColor;

    [SerializeField] private Vector2 _multiplierShakeMap;
    [SerializeField] private AnimationCurve _camShakeMagnitude;
    [SerializeField] private AnimationCurve _camShakeRoughness;
    [SerializeField] private AnimationCurve _camShakeFadeIn;
    [SerializeField] private AnimationCurve _camShakeFadeOut;
    [SerializeField] private AnimationCurve _capsuleImpactAudioPitch;

    [Space(10)]

    [SerializeField] private SplineContainer _capsuleSpawnSpline;
    [SerializeField] private AnimationCurve _capsuleSpawnSpeed;
    [SerializeField] private CleaningBarController _cleaningBarController;

    [SerializeField] private KingController _kingController;

    public static int CapsuleCount = 0;
    public static bool RoyalCelebration = false;
    public static bool WasCelebration = false;

    public float TISpeed = 0.1f;

    private Queue<SuperCapsule> _capsuleSpawnQ = new Queue<SuperCapsule>();

    private void Awake()
    {
        _superCapsulePool = new ObjectPool<SuperCapsule>(SuperCapsuleFactory, CapsuleTurnOn, CapsuleTurnOff);
        _multiplierZonePool = new ObjectPool<MultiplierZone>(SuperchatZoneFactory, SuperchatZoneTurnOn, SuperchatZoneTurnOff);
        StartCoroutine(CapsuleSpawner());
    }

    private SuperCapsule SuperCapsuleFactory()
    {
        return Instantiate(_superCapsulePrefab, _capsulesRoot).GetComponent<SuperCapsule>(); ;
    }

    private void CapsuleTurnOn(SuperCapsule sc)
    {
        sc.gameObject.SetActive(true);
    }
    private void CapsuleTurnOff(SuperCapsule sc)
    {
        sc.gameObject.SetActive(false);
    }

    private MultiplierZone SuperchatZoneFactory()
    {
        return Instantiate(_superchatZonePrefab, _zonesRoot).GetComponent<MultiplierZone>();
    }
    private void SuperchatZoneTurnOn(MultiplierZone sz)
    {
        sz.gameObject.SetActive(true);
        sz.transform.position = _holdingPen.transform.position;
    }
    private void SuperchatZoneTurnOff(MultiplierZone sz)
    {
        sz.gameObject.name = "pooledZone";
        //Can't turn off Zone parent gameobject, otherwise OnTriggerExit2D won't trigger on the PBEffectors
    }

    public IEnumerator CreateRebellion(PlayerHandler ph, int bitsInMessage, string message)
    {
        CapsuleCount++;
        _autoPredictions.RebellionSignal();
        SuperCapsule capsule = _superCapsulePool.GetObject();
        capsule.transform.position = _capsuleSpawnPos.position;

        CoroutineResult<Texture> coResult = new CoroutineResult<Texture>();
        yield return ph.GetPfp(coResult);
        Texture pfp = coResult.Result;

        if (pfp == null)
            pfp = _gm.DefaultPFP;

        if (ph.IsKing())
            MyTTS.inst.Announce("New Rebellion from " + ph.pp.TwitchUsername);
        else if (RoyalCelebration)
            MyTTS.inst.Announce(ph.pp.TwitchUsername + "has joined the celebration! " + message);
        else
            MyTTS.inst.Announce("New Rebellion from " + ph.pp.TwitchUsername + ". " + message);

        int dollarAmount = Mathf.FloorToInt(bitsInMessage / 100f);

        //Prevent them from despawning for the duration of the superchat, if they do despawn, then the texture will be lost on the super capsule
        //int superCapsuleDuration = AppConfig.inst.GetI("CapsuleSecondsPerDollar") * dollarAmount;

        float t = dollarAmount / _dollarColorMap.y;
        Color headerBackgroundColor = _dollarToColor.Evaluate(t);

        Color.RGBToHSV(headerBackgroundColor, out float H, out float S, out float V);

        Color bodyBackgroundColor = Color.HSVToRGB(H, S, V - 0.5f);

        Color particlesColor = headerBackgroundColor;

        capsule.InitRebellion(this, ph, pfp, GetZoneFor(ph), dollarAmount, label: MyUtil.AbbreviateNum4Char(bitsInMessage), _dollarColorMap, bodyBackgroundColor, headerBackgroundColor, particlesColor);
        _capsuleSpawnQ.Enqueue(capsule);
    }

    public IEnumerator CreateRoyalCelebration(PlayerHandler ph, int bitsInMessage, string message)
    {        
        CapsuleCount++;
        RoyalCelebration = true;
        //_autoPredictions.RoyalCelebrationSignal();
        SuperCapsule capsule = _superCapsulePool.GetObject();
        capsule.transform.position = _capsuleSpawnPos.position;

        CoroutineResult<Texture> coResult = new CoroutineResult<Texture>();
        yield return ph.GetPfp(coResult);
        Texture pfp = coResult.Result;

        if (pfp == null)
            pfp = _gm.DefaultPFP;

        StartCoroutine(_cleaningBarController.RunCleaningBar());

        MyTTS.inst.Announce("Citizens will be rewarded with Gold from The Treasury during our Royal Celebration");
        _labelText.SetText("Cheer bits to Join the Celebration! (200 Minimum)");

        int dollarAmount = Mathf.FloorToInt(bitsInMessage / 100f);

        //Prevent them from despawning for the duration of the superchat, if they do despawn, then the texture will be lost on the super capsule
        //int superCapsuleDuration = AppConfig.inst.GetI("CapsuleSecondsPerDollar") * dollarAmount;

        float t = dollarAmount / _dollarColorMap.y;
        Color headerBackgroundColor = _dollarToColor.Evaluate(t);

        Color.RGBToHSV(headerBackgroundColor, out float H, out float S, out float V);

        Color bodyBackgroundColor = Color.HSVToRGB(H, S, V - 0.5f);

        Color particlesColor = headerBackgroundColor;

        capsule.InitRoyalCelebration(this, ph, pfp, GetRoyalZoneFor(ph), dollarAmount, label: MyUtil.AbbreviateNum4Char(bitsInMessage), _dollarColorMap, bodyBackgroundColor, headerBackgroundColor, particlesColor);
        _capsuleSpawnQ.Enqueue(capsule);

        if (_kingController.TollRate < 10)
            _kingController.UpdateTollRate(10);

        CheckCelebration();
    }

    public IEnumerator SpawnNewCapsule(SuperCapsule capsule)
    {
        AudioController.inst.PlaySound(AudioController.inst.SpawnNewSuperCapsule, 0.9f, 1.1f);

        yield return capsule.RunSpawnAnimation(_capsuleSpawnSpline.Spline, _capsuleSpawnSpeed, _capsuleStackStartPos);
        //yield return null; 

        float t = capsule.DollarEquivalent / _multiplierShakeMap.y;
        CameraShaker.Instance.ShakeOnce(_camShakeMagnitude.Evaluate(t), _camShakeRoughness.Evaluate(t), _camShakeFadeIn.Evaluate(t), _camShakeFadeOut.Evaluate(t));
        float pitch = _capsuleImpactAudioPitch.Evaluate(t);
        AudioController.inst.PlaySound(AudioController.inst.RebellionCapsuleImpact, pitch, pitch);

        _activeSuperCapsules.Add(capsule);

    }

    public void DestroyCapsule(SuperCapsule capsule)
    {
        CapsuleCount--;
        AudioController.inst.PlaySound(AudioController.inst.RebellionEnd, 0.95f, 1.05f);
        capsule.Zone.DecrementMultiplier(capsule.DollarEquivalent);
        capsule.Zone = null;

        _activeSuperCapsules.Remove(capsule);
        _superCapsulePool.ReturnObject(capsule);

        CheckCelebration();
    }

    public void DestroySuperchatZone(MultiplierZone zone)
    {
        zone.DisableZone();
        _activeMultiplierZones.Remove(zone);
        _multiplierZonePool.ReturnObject(zone);

    }

    //Runs forever
    public IEnumerator CapsuleSpawner()
    {
        while (true)
        {
            while (_capsuleSpawnQ.Count > 0)
            {
                //Create text to speech announcing the superchat
                yield return SpawnNewCapsule(_capsuleSpawnQ.Dequeue());
            }
            yield return null;
        }
    }

    private void Update()
    {
        for (int i = 0; i < _activeSuperCapsules.Count; i++)
        {
            //Move each capsule towards its set index based on the number of super capsules
            SuperCapsule sc = _activeSuperCapsules[_activeSuperCapsules.Count - 1 - i];
            Vector3 nextPos = Vector3.MoveTowards(sc.transform.position, _capsuleStackStartPos.position + _capsulePileSpacing * i, _capsuleMoveSpeed * Time.deltaTime);
            sc.transform.position = nextPos;
        }        
    }

    private void CheckCelebration()
    {
        if (CapsuleCount == 0)
        {
            RoyalCelebration = false;
            Debug.Log("Celebration Off");
        }

        if (RoyalCelebration)
        {
            _zonesRoot.localScale = new Vector3(5.3f, 5.3f, 5.3f);
            Debug.Log("MakeBeeg");
            WasCelebration = true;
        }
        else
        {
            _zonesRoot.localScale = new Vector3(1, 1, 1);
            Debug.Log("MakeSmol");
            if (WasCelebration)
            {
                MyTTS.inst.Announce("The Royal Celebration has Concluded. Thank you for your participation.");
                _kingController.currentKing.Ph.SpeechBubble($"The Royal Celebration has Concluded. Thank you for your participation.");
                _labelText.SetText("Cheer bits to lead Rebellion! (200 Minimum)");
                WasCelebration = false;
            }
        }

    }

    public MultiplierZone GetZoneFor(PlayerHandler ph)
    {
        foreach (MultiplierZone z in _activeMultiplierZones)
        {
            if (z.Ph.pp.TwitchID == ph.pp.TwitchID)
                return z;
        }

        MultiplierZone zone = _multiplierZonePool.GetObject();
        zone.InitRebellion(this, ph, _dollarColorMap, _dollarToColor, _dollarToTextColor);
        zone.name = ph.pp.TwitchID + "_ZONE";       
        _activeMultiplierZones.Add(zone);
        return zone;
    }

    public MultiplierZone GetRoyalZoneFor(PlayerHandler ph)
    {
        foreach (MultiplierZone z in _activeMultiplierZones)
        {
            if (z.Ph.pp.TwitchID == ph.pp.TwitchID)
                return z;
        }

        MultiplierZone zone = _multiplierZonePool.GetObject();
        zone.InitRoyalCelebration(this, ph, _dollarColorMap, _dollarToColor, _dollarToTextColor);
        zone.name = ph.pp.TwitchID + "_ZONE";
        _activeMultiplierZones.Add(zone);
        return zone;
    }

    public bool DoesPlayerHaveActiveRebellion(PlayerHandler ph)
    {
        foreach(MultiplierZone z in _activeMultiplierZones)
        {
            if (ph.pp.TwitchID == z.Ph.pp.TwitchID)
                return true;
        }
        return false;
    }

    public int GetMultiplierByPos(Vector2 pos)
    {
        int multiplier = 0;
        foreach(MultiplierZone z in _activeMultiplierZones)
        {
            if(Vector2.Distance(z.transform.position, pos) <= z.GetRadius())
            {
                if (z.Multiplier >= 2)
                    multiplier += z.Multiplier;
            }
        }

        multiplier = Mathf.Max(1, multiplier);
        return multiplier;
    }

    public void OnNewTileSpin()
    {
        for (int i = _activeSuperCapsules.Count - 1; i >= 0; i--)
            _activeSuperCapsules[i].Decay(); 
    }
}
