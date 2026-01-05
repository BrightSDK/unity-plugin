using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AOT;
using UnityEngine;

namespace Brdsdk
{
    public class BrdsdkBridgeWin
    {
        public enum Choice : int
        {
            None = 0,
            Peer = 1,
            NotPeer = 2,
        }
        public enum ServiceStatus : int
        {
            None = 0,
            NotInstalled = 1,
            Installed = 2,
            NotRunning = 3,
            Running = 4,
            Disconnected = 5,
            Blocked = 6,
            Connected = 7,
            Peer = 8,
        }

        public delegate void ChoiceChangeCallback(BrdsdkBridgeWin.Choice choice);
        public delegate void ServiceStatusChangeCallback(BrdsdkBridgeWin.ServiceStatus status);

        public static Choice choice => (Choice)_NativeImportsWin.brd_sdk_get_consent_choice_c();

        public static void Init(bool skipConsent)
        {
            _NativeImportsWin.brd_sdk_set_skip_consent_on_init_c(skipConsent);
            _NativeImportsWin.brd_sdk_set_choice_change_cb_c(sdkChoiceCallback);
            _NativeImportsWin.brd_sdk_set_service_status_change_cb_c(sdkStatusChangeCallback);
            _NativeImportsWin.brd_sdk_init_c();
        }

        public static void ShowConsent() 
        {
            _NativeImportsWin.brd_sdk_show_consent_c();
        }

        public static void OptOut()
        {
            _NativeImportsWin.brd_sdk_opt_out_c();
        }

        public static void FixService()
        {
            _NativeImportsWin.brd_sdk_fix_service_status_c();
        }

        public static void Deinit()
        {
            _NativeImportsWin.brd_sdk_close_c();
        }

        public static void SetChoiceChangeCallback(BrdsdkBridgeWin.ChoiceChangeCallback callback)
        {
            onChoiceChange = callback;
        }
        private static BrdsdkBridgeWin.ChoiceChangeCallback onChoiceChange;
        [MonoPInvokeCallback(typeof(BrdsdkBridgeWin.ChoiceChangeCallback))]
        private static void sdkChoiceCallback(BrdsdkBridgeWin.Choice choice)
        {
            if (onChoiceChange != null)
                onChoiceChange(choice);
        }

        public static void SetStatusChangeCallback(BrdsdkBridgeWin.ServiceStatusChangeCallback callback)
        {
            onStatusChange = callback;
        }
        private static BrdsdkBridgeWin.ServiceStatusChangeCallback onStatusChange;
        [MonoPInvokeCallback(typeof(BrdsdkBridgeWin.ServiceStatusChangeCallback))]
        private static void sdkStatusChangeCallback(BrdsdkBridgeWin.ServiceStatus status)
        {
            if (onStatusChange != null)
                onStatusChange(status);
        }
    }

#if UNITY_STANDALONE_WIN
    static class _NativeImportsWin
    {
#if UNITY_64
        private const string lumDllName = "lum_sdk64";
#else
        private const string lumDllName = "lum_sdk32";
#endif
        [DllImport(lumDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void brd_sdk_set_skip_consent_on_init_c(bool skip);
        [DllImport(lumDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void brd_sdk_init_c();
        [DllImport(lumDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void brd_sdk_show_consent_c();
        [DllImport(lumDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int brd_sdk_get_consent_choice_c();
        [DllImport(lumDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int brd_sdk_opt_out_c();
        [DllImport(lumDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int brd_sdk_close_c();
        [DllImport(lumDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern int brd_sdk_fix_service_status_c();
        
        [DllImport(lumDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void brd_sdk_set_choice_change_cb_c(BrdsdkBridgeWin.ChoiceChangeCallback callback);
        [DllImport(lumDllName, CallingConvention = CallingConvention.StdCall)]
        public static extern void brd_sdk_set_service_status_change_cb_c(BrdsdkBridgeWin.ServiceStatusChangeCallback callback);
    }
#endif
}