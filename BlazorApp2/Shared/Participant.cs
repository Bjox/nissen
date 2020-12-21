namespace BlazorApp2.Shared
{
    public class Participant
    {
        public string Name { get; }
        public int? OwnNumber { get; set; }
        public int? AssignedNumber { get; set; }
        public int? CandidateNumber { get; set; }
        public bool Revealed { get; set; }

        public Participant(string name)
        {
            this.Name = name;
        }

        public override string ToString()
        {
            return $"Name={Name}, OwnNumber={OwnNumber}, AssignedNumber={AssignedNumber}, CandidateNumber={CandidateNumber}, Revealed={Revealed}";
        }
    }
}
