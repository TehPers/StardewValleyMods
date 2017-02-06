using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewModdingAPI.Events;

namespace TehPers.Stardew.CombatOverhaul.Natures {
    public class NatureLusidity : Nature {

        public override bool activate(GameLocation location, int x, int y, int power, Farmer who) {
            int timeRemaining = 120;
            ModEntry.INSTANCE.updateEvents.Add(() => {
                keepFull(who);
                return --timeRemaining > 0;
            });
            return true;
        }

        public override string getName() {
            return "Lusidity";
        }

        protected override string getTextureName() {
            return "LusidityScepter";
        }

        public override string getDescription() {
            return "A refreshing aura eminates from this";
        }

        public void keepFull(Farmer who) {
            who.stamina = Math.Min(++who.stamina, who.maxStamina);
            who.health = Math.Min(++who.health, who.maxHealth);
        }
    }
}
