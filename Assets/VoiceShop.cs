using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VoiceShop : Game
{
    [SerializeField] private List<VoiceShopEntry> _entries;

    [SerializeField] private int _buyTime = 30;

    [SerializeField] private float _goldenDiscount = 0.25f;

    private NPCHandler _npcHandler;

    public override void OnTilePreInit()
    {

        _entries[0].InitEntry(1, _gt.IsGolden, _goldenDiscount);
        _entries[1].InitEntry(2, _gt.IsGolden, _goldenDiscount);
        _entries[2].InitEntry(3, _gt.IsGolden, _goldenDiscount); 

        foreach (var entry in _entries)
            entry.HideCommandText();
    }

    public override void StartGame()
    {
        StartCoroutine(_gt.RunTimer(_buyTime));
        StartCoroutine(VoiceLoop());
        StartCoroutine(KillAllAfterDelay(_buyTime));

        if (_gt.Players.Count <= 0)
            return;

        foreach (var entry in _entries)
            entry.ShowCommandText();

        _gt.BuyingActive = true;
    }

    public IEnumerator KillAllAfterDelay(int secDelay)
    {
        yield return new WaitForSeconds(secDelay);
        _gt.EliminatePlayers(_gt.AlivePlayers.ToList(), false);
    }

    public IEnumerator VoiceLoop()
    {
        MyTTS.inst.PlayerSpeech($"This is {_entries[0].GetName(_entries[0].VoiceIndex)} Speaking.", _entries[0].VoiceIndex);
        yield return new WaitForSeconds(2.5f);
        MyTTS.inst.PlayerSpeech($"This is {_entries[1].GetName(_entries[1].VoiceIndex)} Speaking.", _entries[1].VoiceIndex);
        yield return new WaitForSeconds(2.5f);
        MyTTS.inst.PlayerSpeech($"This is {_entries[2].GetName(_entries[2].VoiceIndex)} Speaking.", _entries[2].VoiceIndex);
        yield return new WaitForSeconds(2.5f);
        MyTTS.inst.PlayerSpeech($"{_entries[0].GetName(_entries[0].VoiceIndex)}, Speaking Now.", _entries[0].VoiceIndex);
        yield return new WaitForSeconds(2.5f);
        MyTTS.inst.PlayerSpeech($"{_entries[1].GetName(_entries[1].VoiceIndex)}, Speaking Now.", _entries[1].VoiceIndex);
        yield return new WaitForSeconds(2.5f);
        MyTTS.inst.PlayerSpeech($"{_entries[2].GetName(_entries[2].VoiceIndex)}, Speaking Now.", _entries[2].VoiceIndex);
        yield return new WaitForSeconds(2.5f);
        MyTTS.inst.PlayerSpeech($"{_entries[0].GetName(_entries[0].VoiceIndex)} is prepared to take the Throne.", _entries[0].VoiceIndex);
        yield return new WaitForSeconds(2.5f);
        MyTTS.inst.PlayerSpeech($"{_entries[0].GetName(_entries[1].VoiceIndex)} is prepared to take the Throne.", _entries[1].VoiceIndex);
        yield return new WaitForSeconds(2.5f);
        MyTTS.inst.PlayerSpeech($"{_entries[0].GetName(_entries[2].VoiceIndex)} is prepared to take the Throne.", _entries[2].VoiceIndex);
    }    

    public override void CleanUpGame()
    {
        foreach (var entry in _entries)
            entry.HideCommandText();
    }

    public override void ProcessGameplayCommand(string messageId, TwitchClient twitchClient, PlayerHandler ph, string msg, string rawEmotesRemoved)
    {
        foreach (var entry in _entries)
        {
            foreach (string buyCommand in entry.BuyCommands)
            {
                if (msg.ToLower().StartsWith(buyCommand))
                {
                    if (ph.pp.Gold < entry.GoldCost)
                    {
                        twitchClient.ReplyToPlayer(messageId, ph.pp.TwitchUsername, $"You don't have enough gold! Your current gold: {ph.pp.Gold}");
                        return;
                    }

                    //If the player handler is king, allow them to attempt the purchase without moving
                    if (ph.IsKing())
                        entry.AttemptPurchase(ph.pb);
                    else //If the player is a normal ball, move them to the entry before making the purchase
                        ph.ReceivableTarget = entry;

                    break;
                }
            }

        }

    }
}
