using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Configuration;

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
        public static String runtimePath_;
        static Dist()
        {

            String sBinPathConfig = ConfigurationManager.AppSettings["BinPath"].ToString().Trim();
            if (sBinPathConfig == "")
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);

                binPath_ = Path.GetDirectoryName(path).TrimEnd(new char[] { '\\', '/' }) + "\\";
            }
            else
            {
                if (!sBinPathConfig.EndsWith("\\") && !sBinPathConfig.EndsWith("/"))
                {
                    binPath_ = sBinPathConfig + "\\";
                }
                else
                {
                    binPath_ = sBinPathConfig;
                }
            }

            scriptPath_ = binPath_ + "Script\\";
            dataPath_ = binPath_ + "data\\";
            dayPath_ = dataPath_ + "day\\";
            assetPath_ = binPath_ + "asset\\";
            straPath_ = assetPath_ + "strategy\\";
            manualStraPath_ = binPath_ + "manual\\";
            runtimePath_ = binPath_ + "runtime\\";
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
            if (Directory.Exists(runtimePath_))
            {
                var now = DateTime.Now;
                if (now.Hour < 9 || (now.Hour == 9 && now.Minute < 30)) {
                    Directory.Delete(runtimePath_);
                }
            }
            if (!Directory.Exists(runtimePath_))  {
                Directory.CreateDirectory(runtimePath_);
            }
        }
    }
}
