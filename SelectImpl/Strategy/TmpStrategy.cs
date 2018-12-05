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
        #endregion
        Dictionary<String, String> selectFor(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (dsh.IsLikeSTStop())
            {
                return null;
            }
            if (zf > 0)
            {
                return null;
            }
            var c = dsh.Ref(Info.C);
            int iSigIndex = -1;
            for (int i = 1; i < 15; ++i )
            {
                var curZF = dsh.Ref(Info.ZF, i);
                if (curZF > 0.095)
                {
                    for (int j = 1; j < 8; ++j )
                    {
                        if (dsh.Ref(Info.ZF, j+i) > 0.095)
                        {
                            return null;
                        }
                    }

                    iSigIndex = i;
                    break;
                }
                if (c > dsh.Ref(Info.L, i))
                {
                    return null;
                }
            }
            if (iSigIndex == -1)
            {
                return null;
            }
            if (Math.Min(dsh.Ref(Info.L, iSigIndex), dsh.Ref(Info.C, iSigIndex+1)) < c)
            {
                return null;
            }
            return EmptyRateItemButSel;
        }

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (zf > 0.02 && zf < 0.025)
            {
                return EmptyRateItemButSel;
            }
            return null;
            if (dsh.IsLikeSTStop())
            {
                return null;
            }
            if (dsh.IsGreen())
            {
                return null;
            }
            int iIndex = 1;
            do 
            {
                if (dsh.IsRed(iIndex))
                {
                    return null;
                }
                var ret = selectFor(dsh.newDsh(iIndex), selectMode, ref sigInfo);
                if (ret != null)
                {
                    return EmptyRateItemButSel;
                }
                ++iIndex;
            } while (iIndex < 40);

            return null;
        }
    }
}
