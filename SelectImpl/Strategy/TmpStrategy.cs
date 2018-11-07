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

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (dsh.Ref(Info.OF) > 0.095)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 1) < 0.095)
            {
                return null;
            }
            int iSigIndex = -1;
            for (int i = 1; i < 22; ++i )
            {
                if (dsh.IsDownStopEveryDay(3, i))
                {
                    iSigIndex = i;
                    break;
                }
                if (i != 1 && dsh.Ref(Info.ZF, i) > 0.095)
                {
                    return null;
                }
            }
            if (iSigIndex == -1)
            {
                return null;
            }
            return EmptyRateItemButSel;
        }
   
    }
}
