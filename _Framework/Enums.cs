using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.Framework {
    public enum WaterType : int {
        /** <summary>Game ID is 1</summary> **/
        LAKE = 1,

        /** <summary>Game ID is 0</summary> **/
        RIVER = 2,

        /** <summary>Game ID is -1</summary> **/
        BOTH = LAKE | RIVER
    }

    public enum Weather : int {
        SUNNY = 1,
        RAINY = 2,
        BOTH = SUNNY | RAINY
    }

    public enum Season : int {
        SPRING = 1,
        SUMMER = 2,
        FALL = 4,
        WINTER = 8,

        SPRINGSUMMER = SPRING | SUMMER,
        SPRINGFALL = SPRING | FALL,
        SUMMERFALL = SUMMER | FALL,
        SPRINGSUMMERFALL = SPRING | SUMMER | FALL,
        SPRINGWINTER = SPRING | WINTER,
        SUMMERWINTER = SUMMER | WINTER,
        SPRINGSUMMERWINTER = SPRING | SUMMER | WINTER,
        FALLWINTER = FALL | WINTER,
        SPRINGFALLWINTER = SPRING | FALL | WINTER,
        SUMMERFALLWINTER = SUMMER | FALL | WINTER,
        SPRINGSUMMERFALLWINTER = SPRING | SUMMER | FALL | WINTER
    }
}
