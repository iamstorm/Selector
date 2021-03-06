﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class DownUpStrategy : CloseBuyStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "DownUp";
        }
        #endregion
        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (zf > 0.03 || zf < 0.02)
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
            var of = dsh.Ref(Info.OF);
            if (of > 0.02 || of < -0.04)
            {
                return null;
            }
            if ((dsh.Ref(Info.H) - dsh.Ref(Info.H, 1)) / dsh.Ref(Info.C, 1) < -0.03)
            {
                return null;
            }
            int nOccurCount = 0;
            bool bSearchDown = true;
            int iMaxSearchIndex = 1;
            float maxDownVol = float.MinValue;
            float maxUpVol = float.MinValue;
            for (int i = 1; i < 100; ++i )
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
                if (curZF < -0.095)
                {
                    break;
                }
                if (!dsh.IsReal(i))
                {
                    return null;
                }
                if (bSearchDown && curZF > 0)
                {
                    break;
                }
                if (!bSearchDown && curZF < 0)
                {
                    break;
                }
                if (dsh.Ref(Info.CO, i) / dsh.Ref(Info.C, 1) < 0.01)
                {
                    break;
                }
                if (dsh.IsReal(i) && curZF < -0.01 && 
                    dsh.IsReal(i+1) && dsh.Ref(Info.ZF, i + 1) > 0.01 &&
                    dsh.IsReal(i+2) && dsh.Ref(Info.ZF, i + 2) < -0.01)
                {
                    ++nOccurCount;
                    iMaxSearchIndex = i;
                }
                bSearchDown = !bSearchDown;
                if (curZF > 0)
                {
                    maxUpVol = Math.Max(maxUpVol, dsh.Ref(Info.V, i));
                }
                else
                {
                    maxDownVol = Math.Max(maxDownVol, dsh.Ref(Info.V, i));
                }
            }
            if (nOccurCount != 2)
            {
                return null;
            }
            if (dsh.Ref(Info.V) > Math.Max(maxDownVol, maxUpVol)*1.5)
            {
                return null;
            }
            if (maxUpVol > maxDownVol * 2)
            {
                return null;
            }

            var szZF = dsh.SZRef(Info.ZF);
            if (szZF > 0 && szZF < 0.011 && dsh.SZAcc(Info.ZF, 7, 1) > 0.03)
            {
                return null;
            }
            if (!selectBySZ(dsh))
            {
                return null;
            }
            return EmptyRateItemButSel;
        }
    }
}
