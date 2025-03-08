using UnityEngine;
using System;
using Amazon.Polly;
using Amazon.Runtime;
using Amazon;
using System.IO;
using Amazon.Polly.Model;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TwitchLib.PubSub.Enums;

public class MyTTS : MonoBehaviour
{
    public AudioSource audioSource;

    [SerializeField] private AudioSource _lowPitchAudioSource;
    [SerializeField] private AudioSource _regularPitchAudioSource;
    [SerializeField] private AudioSource _highPitchAudioSource;

    public static MyTTS inst;

    [SerializeField] private int max_TTS_string_length = 400;


    [SerializeField] private string textToSpeak;
    [SerializeField] private bool speakButton;
    [SerializeField] private bool testUsePolly;
    [SerializeField] private string voiceId = "Joey";
    [SerializeField] private AudioPitch audioPitch;

    private Queue<(AudioClip clip, float pitch)> audioQ = new Queue<(AudioClip clip, float pitch)>();

    private AmazonPollyClient client;

    private List<Voice> _voiceOptions;

    private int audioFileCycler = 0;

    public enum AudioPitch { Low, Reg, High };

    private StringBuilder _sb = new StringBuilder();

    private Dictionary<string, SubGifter> _giftedSubs = new Dictionary<string, SubGifter>();
    [SerializeField] private float _aggregateGiftsDuration = 2f;

    public void Start()
    {
        inst = this;

        if (!string.IsNullOrEmpty(AppConfig.inst.GetS("AWS_ACCESS_KEY")))
        {
            string accessKey = AppConfig.inst.GetS("AWS_ACCESS_KEY");
            string secretKey = AppConfig.inst.GetS("AWS_SECRET_KEY");
            var credentials = new BasicAWSCredentials(accessKey, secretKey);

            client = new AmazonPollyClient(credentials, RegionEndpoint.USEast2);

            _ = GetVoiceOptions();
        }

        _lowPitchAudioSource.pitch = 0.5f;
        _regularPitchAudioSource.pitch = 1;
        _highPitchAudioSource.pitch = 1.5f; 
    }

    private async Task GetVoiceOptions()
    {
        var response = await client.DescribeVoicesAsync(new DescribeVoicesRequest() { Engine = Engine.Standard });

        if(response.HttpStatusCode != System.Net.HttpStatusCode.OK)
        {
            Debug.LogError("FAILED TO FETCH VOICE OPTIONS: httpstatuscode: " + response.ToString());
            return;
        }

        CLDebug.Inst.Log($"Successfully fetched {response.Voices.Count} voice options.");


        _voiceOptions = response.Voices;
    }

    private void OnValidate()
    {
        if (speakButton)
        {
            speakButton = false;
            if (testUsePolly)
                SpeechMaster(textToSpeak, VoiceId.FindValue(voiceId), audioPitch, false);
        }
    }

    private void Update()
    {
        if(_giftedSubs.Count > 0)
        {
            string[] keys = _giftedSubs.Keys.ToArray();
            foreach(string key in keys)
            {
                SubGifter gifter = _giftedSubs[key];
                gifter.timer -= Time.deltaTime;
                _giftedSubs[key] = gifter;

                if(gifter.timer <= 0)
                {
                    _giftedSubs.Remove(key);
                    Announce($"{gifter.username} gifted {gifter.count} {gifter.tier} sub{((gifter.count > 1) ? "s" : "")}. What a bro.");
                }
            }
        }

        if (audioQ.Count <= 0)
            return;

        if (audioSource.isPlaying)
            return;

        var tts = audioQ.Dequeue();
        audioSource.pitch = tts.pitch;
        audioSource.clip = tts.clip;
        audioSource.PlayOneShot(tts.clip); 
    }

    public void PlayerSpeech(string textToSpeak, int VoiceIndex)
    {
        VoiceId SpeakerName;

        switch (VoiceIndex)
        {
            default: //Default Male Name 0
                SpeakerName = Amazon.Polly.VoiceId.Joey;
                break;
            case 1: //Default Female Name 1
                SpeakerName = Amazon.Polly.VoiceId.Salli;
                break;
            case 2: // Beginning of Tier 1 Names 2-14
                SpeakerName = Amazon.Polly.VoiceId.Emma;
                break;
            case 3:
                SpeakerName = Amazon.Polly.VoiceId.Nicole;
                break;
            case 4:
                SpeakerName = Amazon.Polly.VoiceId.Russell;
                break;
            case 5:
                SpeakerName = Amazon.Polly.VoiceId.Amy;
                break;
            case 6:
                SpeakerName = Amazon.Polly.VoiceId.Brian;
                break;
            case 7:
                SpeakerName = Amazon.Polly.VoiceId.Aditi;
                break;
            case 8:
                SpeakerName = Amazon.Polly.VoiceId.Raveena;
                break;
            case 9:
                SpeakerName = Amazon.Polly.VoiceId.Ivy;
                break;
            case 10:
                SpeakerName = Amazon.Polly.VoiceId.Joanna;
                break;
            case 11:
                SpeakerName = Amazon.Polly.VoiceId.Kendra;
                break;
            case 12:
                SpeakerName = Amazon.Polly.VoiceId.Kimberly;
                break;
            case 13:
                SpeakerName = Amazon.Polly.VoiceId.Kevin;
                break;
            case 14:
                SpeakerName = Amazon.Polly.VoiceId.Geraint;
                break; // End of Tier 1 Names 2-14
            case 15: // Beginning of Tier 2 Names 15-35
                SpeakerName = Amazon.Polly.VoiceId.Celine;
                break;
            case 16:
                SpeakerName = Amazon.Polly.VoiceId.Lea;
                break;
            case 17:
                SpeakerName = Amazon.Polly.VoiceId.Mathieu;
                break;
            case 18:
                SpeakerName = Amazon.Polly.VoiceId.Chantal;
                break;
            case 19:
                SpeakerName = Amazon.Polly.VoiceId.Marlene;
                break;
            case 20:
                SpeakerName = Amazon.Polly.VoiceId.Vicki;
                break;
            case 21:
                SpeakerName = Amazon.Polly.VoiceId.Hans;
                break;
            case 22:
                SpeakerName = Amazon.Polly.VoiceId.Carla;
                break;
            case 23:
                SpeakerName = Amazon.Polly.VoiceId.Bianca;
                break;
            case 24:
                SpeakerName = Amazon.Polly.VoiceId.Giorgio;
                break;
            case 25:
                SpeakerName = Amazon.Polly.VoiceId.Camila;
                break;
            case 26:
                SpeakerName = Amazon.Polly.VoiceId.Vitoria;
                break;
            case 27:
                SpeakerName = Amazon.Polly.VoiceId.Ricardo;
                break;
            case 28:
                SpeakerName = Amazon.Polly.VoiceId.Ines;
                break;
            case 29:
                SpeakerName = Amazon.Polly.VoiceId.Cristiano;
                break;
            case 30:
                SpeakerName = Amazon.Polly.VoiceId.Conchita;
                break;
            case 31:
                SpeakerName = Amazon.Polly.VoiceId.Lucia;
                break;
            case 32:
                SpeakerName = Amazon.Polly.VoiceId.Enrique;
                break;
            case 33:
                SpeakerName = Amazon.Polly.VoiceId.Mia;
                break;
            case 34:
                SpeakerName = Amazon.Polly.VoiceId.Lupe;
                break;
            case 35:
                SpeakerName = Amazon.Polly.VoiceId.Penelope;
                break;
            case 36:
                SpeakerName = Amazon.Polly.VoiceId.Miguel;
                break; // End of Tier 2 Names 15-35
            case 37: // End of Tier 2 Names 36-59
                SpeakerName = Amazon.Polly.VoiceId.Zeina;
                break;
            case 38:
                SpeakerName = Amazon.Polly.VoiceId.Zhiyu;
                break;
            case 39:
                SpeakerName = Amazon.Polly.VoiceId.Naja;
                break;
            case 40:
                SpeakerName = Amazon.Polly.VoiceId.Mads;
                break;
            case 41:
                SpeakerName = Amazon.Polly.VoiceId.Lotte;
                break;
            case 42:
                SpeakerName = Amazon.Polly.VoiceId.Ruben;
                break;
            case 43:
                SpeakerName = Amazon.Polly.VoiceId.Aditi;
                break;
            case 44:
                SpeakerName = Amazon.Polly.VoiceId.Dora;
                break;
            case 45:
                SpeakerName = Amazon.Polly.VoiceId.Karl;
                break;
            case 46:
                SpeakerName = Amazon.Polly.VoiceId.Mizuki;
                break;
            case 47:
                SpeakerName = Amazon.Polly.VoiceId.Takumi;
                break;
            case 48:
                SpeakerName = Amazon.Polly.VoiceId.Seoyeon;
                break;
            case 49:
                SpeakerName = Amazon.Polly.VoiceId.Liv;
                break;
            case 50:
                SpeakerName = Amazon.Polly.VoiceId.Ewa;
                break;
            case 51:
                SpeakerName = Amazon.Polly.VoiceId.Maja;
                break;
            case 52:
                SpeakerName = Amazon.Polly.VoiceId.Jacek;
                break;
            case 53:
                SpeakerName = Amazon.Polly.VoiceId.Jan;
                break;
            case 54:
                SpeakerName = Amazon.Polly.VoiceId.Carmen;
                break;
            case 55:
                SpeakerName = Amazon.Polly.VoiceId.Tatyana;
                break;
            case 56:
                SpeakerName = Amazon.Polly.VoiceId.Maxim;
                break;
            case 57:
                SpeakerName = Amazon.Polly.VoiceId.Astrid;
                break;
            case 58:
                SpeakerName = Amazon.Polly.VoiceId.Filiz;
                break;
            case 59:
                SpeakerName = Amazon.Polly.VoiceId.Gwyneth;
                break; // End of Tier 2 Names 36-59

        }

        SpeechMaster(textToSpeak, SpeakerName, AudioPitch.Reg, addToQ: false);
    }

    public void Announce(string textToSpeak)
    {
        SpeechMaster(textToSpeak, VoiceId.Brian, AudioPitch.Reg, addToQ:true);
    }

    public void AggregateSubGift(string gifterUsername, int multiMonthDuration, SubscriptionPlan tier)
    {
        SubGifter sg;
        if(!_giftedSubs.TryGetValue(gifterUsername, out sg))
            sg = new SubGifter() { username = gifterUsername, count = 0, multimonthduration = multiMonthDuration, tier = tier};
        
        sg.count++;
        sg.timer = _aggregateGiftsDuration;
        _giftedSubs[gifterUsername] = sg;

    }

    public void SpeechMaster(string textToSpeak, VoiceId voiceID, AudioPitch pitch, bool addToQ)
    {
        if(textToSpeak.Length > max_TTS_string_length)
        {
            CLDebug.Inst.Log($"String length {textToSpeak.Length} is greater than max allowed {max_TTS_string_length}. Skipping TTS in speech Master.");
            return;
        }

        if (!string.IsNullOrEmpty(AppConfig.inst.GetS("AWS_ACCESS_KEY")))
            _ = SpeechPolly(textToSpeak, voiceID, pitch, addToQ);
        else
            SpeechLocal(textToSpeak, voiceID, pitch, addToQ);
    }

    private async Task SpeechPolly(string textToSpeak, VoiceId voiceId, AudioPitch pitch, bool addToQ)
    {
        try
        {
            var request = new SynthesizeSpeechRequest()
            {
                Text = textToSpeak, //$"<speak> <prosody pitch=\"{pitch}%\"> <amazon:effect vocal-tract-length=\"{vocal_tract_length_percent}%\">{textToSpeak} </amazon:effect> </prosody> </speak>",
                Engine = Engine.Standard,
                VoiceId = VoiceId.FindValue(voiceId),
                OutputFormat = OutputFormat.Mp3,
                TextType = TextType.Text //TextType.Ssml
            };

            var response = await client.SynthesizeSpeechAsync(request);

            audioFileCycler = (audioFileCycler + 1) % 10;
            string _polyAudioFilePath = $"{Application.persistentDataPath}/audio{audioFileCycler}.mp3";

            Debug.Log("_polyAudioFilepath: " + _polyAudioFilePath);
            WriteIntoFile(response.AudioStream, _polyAudioFilePath);


            using (var www = UnityWebRequestMultimedia.GetAudioClip(_polyAudioFilePath, AudioType.MPEG))
            {
                var result = www.SendWebRequest();

                while (!result.isDone) await Task.Yield();
                   
                if(www.result == UnityWebRequest.Result.ConnectionError || www.responseCode != 200)
                {
                    www.Dispose();
                    Debug.Log("error Inside of MyTTS unity web request multimedia. Cancelling SpeechPolly"); 
                    return;
                }

                var clip = DownloadHandlerAudioClip.GetContent(www);

                if(clip.length > 30)
                {
                    CLDebug.Inst.Log("AWS Polly audio clip over 30 seconds long. Not playing on audio source to avoid annoyance. Length: " + clip.length);
                    return;
                }

                float pitchVal = 1;
                if (pitch == AudioPitch.Low)
                    pitchVal = 0.5f;
                else if (pitch == AudioPitch.Reg)
                    pitchVal = 1f;
                else if (pitch == AudioPitch.High)
                    pitchVal = 1.5f;

                if (addToQ)
                    audioQ.Enqueue((clip, pitchVal));
                else
                {
                    if (pitch == AudioPitch.Low)
                        _lowPitchAudioSource.PlayOneShot(clip); 
                    else if(pitch == AudioPitch.Reg)
                        _regularPitchAudioSource.PlayOneShot(clip);
                    else
                        _highPitchAudioSource.PlayOneShot(clip);
                }
            }

        }
        catch (Exception e)
        {
            CLDebug.Inst.LogError("found error in MyTTS: " + e);
        }
    }

    private void SpeechLocal(string textToSpeak, VoiceId voiceId, AudioPitch pitch, bool addToQ)
    {
    /*    float pitchVal = 1;
        if (pitch == AudioPitch.Low)
            pitchVal = 0.5f;
        else if (pitch == AudioPitch.Reg)
            pitchVal = 1f;
        else if (pitch == AudioPitch.High)
            pitchVal = 1.5f; */

        //Speaker.Instance.Speak(textToSpeak, pitch: pitchVal); //TODO, need open source local TTS
        Debug.Log("TODO: Need local open source TTS");
    }

    private void WriteIntoFile(Stream stream, string filePath)
    {


        using (var fileStream = new FileStream(path: filePath, FileMode.Create))
        {
            byte[] buffer = new byte[8 * 1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, bytesRead);
            }
        }
    }

}

public struct SubGifter
{
    public float timer; 
    public string username;
    public int count;
    public int multimonthduration;
    public SubscriptionPlan tier; 
}