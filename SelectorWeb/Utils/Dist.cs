using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace SelectorWeb.Utils
{
    public class Dist
    {
        public static String binPath_;
        public static String scriptPath_;
        public static String dataPath_;
        public static String dayPath_;
        public static String assetPath_;
        public static String straPath_;
        public static String manualStraPath_;
        static Dist()
        {
            binPath_ = ConfigurationManager.AppSettings["BinPath"];
            if (!binPath_.EndsWith("\\"))
            {
                binPath_ += "\\";
            }
            scriptPath_ = binPath_ + "Script\\";
            dataPath_ = binPath_ + "data\\";
            dayPath_ = dataPath_ + "day\\";
            assetPath_ = binPath_ + "asset\\";
            straPath_ = assetPath_ + "strategy\\";
            manualStraPath_ = binPath_ + "manual\\";
        }
        public static String GlobalDBFile()
        {
            return binPath_ + "global.data";
        }
        public static String DayDBFile()
        {
            return dataPath_ + "day.data";
        }
    }
}