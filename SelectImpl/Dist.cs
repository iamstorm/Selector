using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace SelectImpl
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
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);

            binPath_ = Path.GetDirectoryName(path).TrimEnd(new char[] { '\\', '/' }) + "\\";
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
        public static void Setup()
        {
            if (!Directory.Exists(assetPath_))
            {
                Directory.CreateDirectory(assetPath_);
            }
            if (!Directory.Exists(straPath_))
            {
                Directory.CreateDirectory(straPath_);
            }
            if (!Directory.Exists(manualStraPath_))
            {
                Directory.CreateDirectory(manualStraPath_);
            }
        }
    }
}
