using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KingBackgroundShop : Game
{
    [SerializeField] private List<KingBackgroundShopEntry> _entries;

    [SerializeField] private int _buyTime = 30;
    [SerializeField] private int _modeOne;
    [SerializeField] private int _modeTwo;
    [SerializeField] private int _modeThree;
    [SerializeField] private int _priceOne;
    [SerializeField] private int _priceTwo;
    [SerializeField] private int _priceThree;

    public override void OnTilePreInit()
    {
        _entries[0].InitEntry(_modeOne);
        _entries[1].InitEntry(_modeTwo);
        _entries[2].InitEntry(_modeThree);

        foreach (var entry in _entries)
            entry.HideCommandText();
    }

    public override void StartGame()
    {
        StartCoroutine(_gt.RunTimer(_buyTime));
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
