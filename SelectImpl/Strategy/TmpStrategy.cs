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
   
            return EmptyRateItemButSel;
        }
   
    }
}
