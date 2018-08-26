using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public interface IBuyDesider
    {
        String name();
        SelectItem makeDeside(List<SelectItem> selItems);
    }
}
