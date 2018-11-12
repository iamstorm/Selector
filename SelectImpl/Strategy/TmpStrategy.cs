using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class TmpStrategy : CloseBuyStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "Tmp";
        }
        public override float bonusLimit()
        {
            return 0.052f;
        }
        public override float firstDayBonusLimit()
        {
            return 0.095f;
        }
        public override int buySpan()
        {
            return 1;
        }
        #endregion

        bool isSigDate(DataStoreHelper dsh, int iIndex)
        {
            if (dsh.Ref(Info.ZF, iIndex) < 0)
            {
                return false;
            }
            if (dsh.AccZF(8, iIndex) < 0.15)
            {
                return false;
            }
            for (int i = 0; i < 8; ++i )
            {
                if (dsh.Ref(Info.ZF, i+iIndex) > 0.095)
                {
                    return false;
                }
            }

            return true;
        }
        bool IsNearRange(DataStoreHelper dsh, int iLhsIndex, int iRhsIndex)
        {
            if (Math.Abs((dsh.Ref(Info.H, iLhsIndex) - dsh.Ref(Info.H, iRhsIndex)) / dsh.Ref(Info.C, iLhsIndex)) > 0.01)
            {
                return false;
            }

            if (Math.Abs((dsh.Ref(Info.L, iLhsIndex) - dsh.Ref(Info.L, iRhsIndex)) / dsh.Ref(Info.C, iLhsIndex)) > 0.01)
            {
                return false;
            }
            return true;
        }

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (zf > 0)
            {
                return null;
            }
            if (dsh.Ref(Info.HL) / dsh.Ref(Info.C, 1) < 0.04)
            {
                return null;
            }
            if (dsh.Ref(Info.HL, 1) / dsh.Ref(Info.C, 2) < 0.04)
            {
                return null;
            }
            if (dsh.Ref(Info.HL, 2) / dsh.Ref(Info.C, 3) < 0.04)
            {
                return null;
            }
            if (dsh.Ref(Info.HL, 3) / dsh.Ref(Info.C, 4) < 0.04)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 3) > 0 && dsh.Ref(Info.ZF, 2) < 0 && dsh.Ref(Info.ZF, 1) > 0)
            {
            }
            else
            {
                return null;
            }
            if (!IsNearRange(dsh, 3, 2) || !IsNearRange(dsh, 2, 1) || !IsNearRange(dsh, 1, 0))
            {
                return null;
            }
            return EmptyRateItemButSel;
        }
   
    }
}
