using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class StrategyData
    {
        public int selectCount_;
        public int sucCount_;
        public int failCount
        {
            get
            {
                return selectCount_ - sucCount_;
            }
        }
        public int bonus_;
    }
}
