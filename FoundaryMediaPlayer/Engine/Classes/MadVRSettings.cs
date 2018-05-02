using System.Diagnostics.CodeAnalysis;
using System.Text;
using FoundaryMediaPlayer.Interfaces;

namespace FoundaryMediaPlayer.Engine.Classes
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class MadVRSettings
    {
        private IMadVRSettings _MadVR { get; }

        public bool bIsValid => _MadVR != null;

        public MadVRSettings(object madVR)
        {
            _MadVR = madVR as IMadVRSettings;
        }

        public bool SetString(string setting, string value)
        {
            if (bIsValid)
            {
                return _MadVR.SettingsSetString(setting, value);
            }

            return false;
        }

        public string GetString(string setting)
        {
            string retVal = string.Empty;

            if (bIsValid)
            {
                int sbLen = 100;
                StringBuilder smMode = new StringBuilder(sbLen);

                bool success = _MadVR.SettingsGetString(setting, smMode, ref sbLen);
                if (sbLen > smMode.Capacity)
                {
                    smMode = new StringBuilder(sbLen);
                    success = _MadVR.SettingsGetString(setting, smMode, ref sbLen);
                }
                if (success)
                    retVal = smMode.ToString();
            }

            return retVal;
        }

        public bool SetBool(string setting, bool value)
        {
            bool retVal = false;

            if (bIsValid)
            {
                retVal = _MadVR.SettingsSetBoolean(setting, value);
            }

            return retVal;
        }

        public bool GetBool(string setting)
        {
            bool retVal = false;

            if (bIsValid)
            {
                bool success = _MadVR.SettingsGetBoolean(setting, ref retVal);
            }

            return retVal;
        }

        public bool SetInt(string setting, int value)
        {
            bool retVal = false;

            if (bIsValid)
            {
                retVal = _MadVR.SettingsSetInteger(setting, value);
            }

            return retVal;
        }

        public int GetInt(string setting)
        {
            int retVal = -1;

            if (bIsValid)
            {
                bool success = _MadVR.SettingsGetInteger(setting, ref retVal);
            }

            return retVal;
        }
    }
}
