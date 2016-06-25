using System.Globalization;
using PropertyChanged;

namespace BD2.Model
{
    [ImplementPropertyChanged]
    public class Child
    {
        public Person child { get; set; }
        public decimal Count { get; set; }

        public override string ToString()
        {
            if (child?.BirthDate == "01/01/0001" && child?.DeathDate == "01/01/0001")
            {
                return $"{child?.Name}" + $", {child?.Gender}";
            }
            else if (child?.DeathDate == "01/01/0001")
            {
                return $"{child?.Name}" + $", {child?.Gender}" + " Ur: " + $"{child?.BirthDate}";
            }

            return $"{child?.Name}" + $", {child?.Gender}" + " Ur: " + $"{child?.BirthDate}" + " Zm: " + $"{child?.DeathDate}";
        }
    }
}
