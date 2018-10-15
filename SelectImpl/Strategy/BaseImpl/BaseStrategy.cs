using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace SelectImpl
{
    public abstract class BaseStrategyImpl
    {
        public string dbKey()
        {
            IStrategy stra = (IStrategy)this;
            return "$stra_" + stra.name();
        }
        public SQLiteHelper sh()
        {
            return DB.connDict_[dbKey()];
        }
        virtual public FocusOn focusOn()
        {
            return FocusOn.FO_All;
        }
        virtual public float bonusLimit()
        {
            return 0.095f;
        }
        public static Dictionary<String, String> EmptyRateItemButSel = new Dictionary<string, string>();

        public bool selectBySZ(DataStoreHelper dsh)
        {
            {
                var szzf = dsh.SZRef(Info.ZF);
                if (dsh.SZRef(Info.ZF, dsh.SZLL(Info.ZF, 3)) < -0.03 &&
                    szzf < 0.005 && szzf > -0.005 &&
        dsh.SZUpShadow() > 0.005 && dsh.SZDownShadow() < -0.005)
                {
                    return false;
                }
            }

            float minSZZF = float.MaxValue;
            float maxSZZF = float.MinValue;
            int iSigIndex = -1;
            for (int i = 1; i <= 3; ++i)
            {
                var szzf = dsh.SZRef(Info.ZF, i);
                if (minSZZF > szzf)
                {
                    minSZZF = szzf;
                    if (minSZZF < -0.02)
                    {
                        iSigIndex = i;
                        break;
                    }
                }
                maxSZZF = Math.Max(maxSZZF, szzf);
            }
            if (iSigIndex != -1 && maxSZZF < 0.01)
            {
                var acc = dsh.SZAcc(Info.ZF, 3, iSigIndex + 1);
                if (acc > 0.02)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
