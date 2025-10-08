using System;

[System.Serializable]
public class BrightSDKConfig
{
    [System.Serializable]
    public class Versions
    {
        public string android;
        public string apple;

        public string this[string key]
        {
            get
            {
                switch (key)
                {
                    case "android": return android;
                    case "apple": return apple;
                    default: return null;
                }
            }
        }
    }

    public Versions versions;
}