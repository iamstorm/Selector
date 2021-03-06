﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class UpDownUpStrategy : CloseBuyStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "UpDownUp";
        }
        #endregion

        public virtual Dictionary<String, String> selectFor(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (zf < 0.02)
            {
                return null;
            }
            if (dsh.IsLikeSTStop())
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 2) < 0.03)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 1) > -0.01)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 3) > 0)
            {
                return null;
            }
            if (dsh.AccZF(3) < 0.05)
            {
                return null;
            }
            if (dsh.AccZF(3, 3) > 0)
            {
                return null;
            }
            if (dsh.Ref(Info.V) > dsh.Ref(Info.V, 1) * 2)
            {
                return null;
            }
            for (int i = 0; i < 4; ++i)
            {
                if (dsh.DownShadow(i) < -0.04)
                {
                    return null;
                }
                if ((dsh.Ref(Info.CO, i) / dsh.Ref(Info.C, i + 1)) < 0.005)
                {
                    return null;
                }
                if (dsh.Ref(Info.OF, i) > 0.02 && dsh.Ref(Info.ZF, i) < -0.02)
                {
                    return null;
                }
            }
            var hhCO = dsh.HH(Info.CO, 8);
            if (dsh.Ref(Info.ZF, hhCO) > 0)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, dsh.LL(Info.ZF, 8)) < -0.095)
            {
                return null;
            }
            if (dsh.HH(Info.V, 6) == 0)
            {
                return null;
            }

            return EmptyRateItemButSel;
        }

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            return selectFor(dsh, selectMode, ref sigInfo);
        }

 
    }
}
