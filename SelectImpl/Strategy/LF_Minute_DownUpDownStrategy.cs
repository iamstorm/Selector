using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class LF_Minute_DownUpDownStrategy : LFMinuteBuyStrategy, IStrategy_LF_Minute
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "LF_Minute_DownUpDown";
        }
        #endregion

        const int startMinuteCount_ = 30;

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            IStrategy_LF_Minute stra = (IStrategy_LF_Minute)this;
            int nowMiniute = Utils.NowMinute();
            if (nowMiniute < 60) {
                return null;
            }
            if (dsh.stock_.name_.ToUpper().IndexOf("ST") != -1) {
                return null;
            }
            if (dsh.Ref(Info.ZF, dsh.HH(Info.ZF, 10)) > 0.095) {
                return null;
            }
            if (dsh.Ref(Info.ZF, dsh.LL(Info.ZF, 10)) < -0.095) {
                return null;
            }
            
            var lastDayC = dsh.Ref(Info.C, 1);
            var rsh = dsh.newRuntimeDsh();

            int globalLLIndex = rsh.LL(Info.C, -1);
            int globalHHIndex = rsh.HH(Info.C, -1);
            int iLLIndex = rsh.LL(Info.C, -1, startMinuteCount_);
            int iReverseHHIndex = rsh.ReverseHH(Info.C, -1, iLLIndex);

            var c = rsh.Ref(Info.C);
            var llc = rsh.Ref(Info.C, iLLIndex);

            if (c >= llc) {
                return null;
            }
            
            var reverseHHc = rsh.Ref(Info.C, iReverseHHIndex);
            var deltaUp = Utils.DeltaF(reverseHHc - llc, lastDayC);
            if (deltaUp < 0.02) {
                return null;
            }

            return EmptyRateItemButSel;
        }
    }
}
