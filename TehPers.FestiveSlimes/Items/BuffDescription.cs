using System;

namespace TehPers.FestiveSlimes.Items {
    public class BuffDescription {
        public int Duration { get; }
        public TimeSpan TimeDuration => TimeSpan.FromMinutes(this.Duration * 0.7 / 60);

        public int Farming { get; set; } = 0;
        public int Fishing { get; set; } = 0;
        public int Mining { get; set; } = 0;
        [Obsolete("Unimplemented")]
        public int Digging { get; set; } = 0;
        public int Luck { get; set; } = 0;
        public int Foraging { get; set; } = 0;
        [Obsolete("Unimplemented")]
        public int Crafting { get; set; } = 0;
        public int MaxEnergy { get; set; } = 0;
        public int Magnetism { get; set; } = 0;
        public int Speed { get; set; } = 0;
        public int Defense { get; set; } = 0;
        public int Attack { get; set; } = 0;

        public BuffDescription(TimeSpan duration) : this((int) (duration.TotalMinutes / 0.7 * 60)) { }
        public BuffDescription(int duration) {
            this.Duration = duration;
        }

        public string GetRawBuffInformation() {
            return string.Join(" ", new[] {
                this.Farming,
                this.Fishing,
                this.Mining,
                this.Digging,
                this.Luck,
                this.Foraging,
                this.Crafting,
                this.MaxEnergy,
                this.Magnetism,
                this.Speed,
                this.Defense,
                this.Attack
            });
        }
    }
}