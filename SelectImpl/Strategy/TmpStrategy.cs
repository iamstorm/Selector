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
            if (zf < 0.02 || zf > 0.03)
            {
                return null;
            }
            if (dsh.IsLikeSTStop())
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 1) > -0.02)
            {
                return null;
            }
            if (dsh.UpShadow() > 0.04)
            {
                return null;
            }
            if (dsh.AccZF(8) > -0.1)
            {
                return null;
            }
            if (dsh.HH(Info.V, 8) == 0)
            {
                return null;
            }
            if (dsh.DownShadow() < -0.04)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 2) < -0.02)
            {
                return null;
            }
            if (!dsh.IsReal())
            {
                return null;
            }
            if (dsh.AccZF(2) > 0)
            {
                return null;
            }
            
            
            for (int i = 1; i < 8; ++i )
            {
                var curZF = dsh.Ref(Info.ZF, i);
                var curOF = dsh.Ref(Info.OF, i);
                if (curZF < -0.095 || curZF > 0.095)
                {
                    return null;
                }
                if (curOF > 0.04 && dsh.IsGreen(i))
                {
                    return null;
                }
            }

            return EmptyRateItemButSel;
        }
   
    }
}
