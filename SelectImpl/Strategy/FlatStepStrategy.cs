using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class FlatStepStrategy : CloseBuyStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "FlatStep";
        }
        public override float bounusLimit()
        {
            return 0.095f;
        }
        #endregion

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);
            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (zf > 0.002 || zf < -0.002)
            {
                return null;
            }
//             if (Math.Abs(dsh.Ref(Info.C) - dsh.Ref(Info.C, 1)) > 0.02*Setting.NormalizeRate)
//             {
//                 return null;
//             }
            if (dsh.Ref(Info.HF) < 0.012)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 2) < 0.04 || dsh.Ref(Info.ZF, 2) > 0.095)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 1) < 0.01 || dsh.Ref(Info.ZF, 1) > 0.095)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 3) < -0.095)
            {
                return null;
            }
            if (dsh.Ref(Info.V) > dsh.Ref(Info.V, 1) * 1.2 && dsh.Ref(Info.V) > dsh.Ref(Info.V, 2) * 1.2)
            {
                return null;
            }
            if (dsh.Ref(Info.OF) < -0.002 || dsh.Ref(Info.OF) > 0.005)
            {
                return null;
            }
            if (dsh.AccZF(8, 3) < -0.15)
            {
                return null;
            }
            if (dsh.AccZF(3, 3) > 0.095)
            {
                return null;
            }
            if (dsh.Ref(Info.OF, 1) > 0.03)
            {
                return null;
            }
            if (dsh.Ref(Info.OF, 2) > 0.03)
            {
                return null;
            }
            if (dsh.Ref(Info.HF, 1) > 0.095)
            {
                return null;
            }
            int iMaxVolIndex = dsh.HH(Info.V, 20, 3);
            if (dsh.Ref(Info.C, iMaxVolIndex) < dsh.Ref(Info.O, iMaxVolIndex))
            {
                return null;
            }
            else
            {
                if (dsh.Ref(Info.HF, iMaxVolIndex-1) <= 0 &&
                    dsh.Ref(Info.L, iMaxVolIndex - 1) <= dsh.Ref(Info.L, iMaxVolIndex))
                {
                    return null;
                }
                if (dsh.UpShadow(iMaxVolIndex) > 0.06)
                {
                    return null;
                }
            }
            for (int i = 0; i < 8; ++i )
            {
                if (dsh.UpShadow(i) > 0.04)
                {
                    return null;
                }
            }
            int iMaxCOIndex = dsh.HH(Info.CO, 30, 1);
            if (dsh.Ref(Info.C, iMaxCOIndex) > dsh.Ref(Info.O, iMaxCOIndex))
            {
                return null;
            }
            var ret = new Dictionary<String, String>();
            ret[String.Format("of/{0}", dsh.Ref(Info.OF) > 0 ? "1" : "-1")] = "";
            ret[String.Format("hf/{0}", dsh.Ref(Info.HF) > 0.02 ? "1" : "-1")] = "";
            ret[String.Format("zf/{0}", Math.Abs(dsh.Ref(Info.ZF)) > 0.001 ? "1" : "-1")] = "";
            return ret;
        }
    }
}
