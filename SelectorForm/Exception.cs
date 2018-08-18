using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectorForm
{
    public class DataException : Exception
    {
        public DataException()
        {
        }
        public DataException(String msg)
            :base(msg)
        {

        }
    }
}
