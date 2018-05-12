using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehCore.Enums {
    public enum Alignment {
        LEFT,
        CENTER,
        RIGHT,
        TOP = Alignment.LEFT,
        MIDDLE = Alignment.CENTER,
        BOTTOM = Alignment.RIGHT,
    }
}
