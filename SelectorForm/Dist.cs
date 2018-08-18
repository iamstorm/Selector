using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SelectorForm
{
    class Dist
    {
        public static String binPath_;
        public static String scriptPath_;
        public static String dataPath_;
        public static String dayPath_;
        public static String regressPath_;
        public static String assetPath_;
        static Dist()
        {
            binPath_ = Path.GetDirectoryName(Application.ExecutablePath).TrimEnd(new char[] { '\\', '/' }) + "\\";
            scriptPath_ = binPath_ + "Script\\";
            dataPath_ = binPath_ + "data\\";
            dayPath_ = dataPath_ + "day\\";
            regressPath_ = binPath_ + "regress\\";
            assetPath_ = binPath_ + "asset\\";
        }
        public static String GlobalDBFile()
        {
            return binPath_ + "global.data";
        }
        public static String YearDayFile(String year)
        {
            return String.Join("\\", binPath_, "data", year+".day");
        }
        public static void Setup()
        {
            if (!Directory.Exists(regressPath_))
            {
                Directory.CreateDirectory(regressPath_);
            }
            if (!Directory.Exists(assetPath_))
            {
                Directory.CreateDirectory(assetPath_);
            }
        }
    }
}
