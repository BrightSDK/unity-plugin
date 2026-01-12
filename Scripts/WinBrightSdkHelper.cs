using System;
using UnityEngine;
using UnityEngine.Events;
using Brdsdk;

public class WinBrightSDKHelper : BrightSDKHelper
{
#if UNITY_STANDALONE_WIN
    void Awake()
    {
        BrdsdkBridgeWin.SetChoiceChangeCallback(choiceChanged);
        BrdsdkBridgeWin.SetStatusChangeCallback(statusChanged);
        BrdsdkBridgeWin.Init(skipConsent);
    }

    public override void ShowConsent()
    {
        BrdsdkBridgeWin.ShowConsent();
    }

    public override bool IsEnabled()
    {
        return BrdsdkBridgeWin.choice == BrdsdkBridgeWin.Choice.Peer;
    }

    public override void OptOut()
    {
        BrdsdkBridgeWin.OptOut();
    }

    private void choiceChanged(BrdsdkBridgeWin.Choice choice)
    {
        bool enabled = choice == BrdsdkBridgeWin.Choice.Peer;
        if (onStatusChangeCallback != null)
            onStatusChangeCallback.Invoke(enabled);
    }

    private void statusChanged(BrdsdkBridgeWin.ServiceStatus status)
    {
        if (status == BrdsdkBridgeWin.ServiceStatus.NotInstalled
            || status == BrdsdkBridgeWin.ServiceStatus.NotRunning)
        {
            BrdsdkBridgeWin.FixService();
        }
    }
#endif
}