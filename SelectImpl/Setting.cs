using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public static class Setting
    {
        public static int NormalizeRate = 10000;
        public static Dictionary<String, String> AllreadyBanedCodeDict = new Dictionary<string, string>();
        static Setting()
        {
            AllreadyBanedCodeDict["600656"] = "";
            AllreadyBanedCodeDict["601268"] = "";
            AllreadyBanedCodeDict["600087"] = "";
        }
        public static bool DebugMode
        {
            get
            {
                return Utils.GetSysInfo(DB.Global(), "DebugMode", "0") == "1";
            }
        }
        public static bool IsAcceptableRuntimeCode(String code)
        {
            if (code.StartsWith("900"))
            {//B股
                return false;
            }
            else if (Setting.AllreadyBanedCodeDict.ContainsKey(code))
            {//退市
                return false;
            }
            else
            {
                return true;
            }
        }
        public static float MyGoodSampleEnvBounsMin = -0.005f;
        public static float MyGoodSampleEnvBounsMax = 0.005f;
        public static float MyGoodSampleUpRateThreshold = 0.5f;
        public static float MyBuyRateLimit = 0.7f;
        public static float MyNewStockLimit = 100;
    }
}
