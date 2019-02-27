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
        
        Dictionary<String, String> selectForRed(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
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
            if (dsh.Ref(Info.LF) < 0)
            {
                return null;
            }
            if (dsh.IsGreen())
            {
                return null;
            }
            if (dsh.UpShadow()  > 0.03)
            {
                return null;
            }
            return EmptyRateItemButSel;
        }
        Dictionary<String, String> selectForGreen(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
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
            if (dsh.Ref(Info.HF) > 0)
            {
                return null;
            }
            if (dsh.IsRed())
            {
                return null;
            }
            if (dsh.DownShadow() < -0.03)
            {
                return null;
            }
            return EmptyRateItemButSel;
        }
        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            if (null == selectForRed(dsh.newDsh(1), selectMode, ref sigInfo))
            {
                return null;
            }
            if (null == selectForGreen(dsh, selectMode, ref sigInfo))
            {
                return null;
            }
            return EmptyRateItemButSel;
        }
    }
}
