using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class EnvUpStrategy : CloseBuyStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "EnvUp";
        }
        #endregion
        
        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (zf < 0.01)
            {
                return null;
            }
            if (dsh.IsLikeSTStop())
            {
                return null;
            }
            if (dsh.SZRef(Info.ZF) < 0.01)
            {
                return null;
            }
            if (dsh.SZRef(Info.ZF, dsh.SZHH(Info.ZF, 10, 1)) < 0.005)
            {
                return null;
            }
            if (!dsh.IsReal())
            {
                return null;
            }
            if (dsh.UpShadow() > 0.04)
            {
                return null;
            }
            if (dsh.DownShadow() < -0.04)
            {
                return null;
            }
            var of = dsh.Ref(Info.OF);
            if (of > 0.02 || of < -0.04)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 1) > -0.02 || dsh.Ref(Info.ZF, 1) < -0.095)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 2) < -0.02)
            {
                return null;
            }
            if (zf + dsh.Ref(Info.ZF, 1) < 0)
            {
                return null;
            }
            if (dsh.Ref(Info.V) > dsh.Ref(Info.V, 1) * 1.5)
            {
                return null;
            }
            for (int i = 1; i < 8; ++i)
            {
                var curZF = dsh.Ref(Info.ZF, i);
                var curOf = dsh.Ref(Info.OF, i);
                if (curOf > 0.04 && curZF < 0)
                {
                    return null;
                }
                if (curOf < -0.02 && curZF < 0)
                {
                    return null;
                }
            }
            if (dsh.AccZF(8, 1) < -0.1)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, dsh.HH(Info.CO, 10)) > 0)
            {
                return null;
            }
            int iMaxVolIndex = dsh.HH(Info.V, 10);
            if (dsh.Ref(Info.C, iMaxVolIndex) > dsh.Ref(Info.O, iMaxVolIndex))
            {
                return null;
            }
            if ((dsh.Ref(Info.H, dsh.HH(Info.H, 8)) - dsh.Ref(Info.L, dsh.LL(Info.L, 8))) / dsh.Ref(Info.C, 1) > 0.2)
            {
                return null;
            }
            return EmptyRateItemButSel;
        }
    }
}
