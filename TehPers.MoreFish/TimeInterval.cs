namespace TehPers.MoreFish {
    public class TimeInterval {
        public int Start { get; set; }
        
        public int Finish { get; set; }

        public TimeInterval() { }

        public TimeInterval(int start, int finish) {
            this.Start = start;
            this.Finish = finish;
        }
    }
}