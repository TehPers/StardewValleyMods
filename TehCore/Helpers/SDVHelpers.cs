using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TehCore.Enums;

namespace TehCore.Helpers {
    public static class SDVHelpers {
        public static Season? ToSeason(string s) {
            switch (s.ToLower()) {
                case "spring":
                    return Season.Spring;
                case "summer":
                    return Season.Summer;
                case "fall":
                    return Season.Fall;
                case "winter":
                    return Season.Winter;
                default:
                    return null;
            }
        }

        public static Weather ToWeather(bool raining) => raining ? Weather.Rainy : Weather.Sunny;

        public static WaterType? ToWaterType(int type) {
            switch (type) {
                case -1:
                    return WaterType.Both;
                case 0:
                    return WaterType.River;
                case 1:
                    return WaterType.Lake;
                default:
                    return null;
            }
        }

        public static int ToInt(this WaterType type) => type == WaterType.Both ? -1 : (type == WaterType.River ? 0 : 1);
    }
}
