using System.Collections.Generic;

namespace CrestronLoggingDemo
{
    public partial class ControlSystem
    {
        private class Position
        {
            public int Lat { get; set; }
            public int Long { get; set; }
            public int Coins { get; set; }
            public string[] Sides { get; set; } = { "Heads", "Tails" };
        }
        public class BadClass
        {
            //This class should have initialized nullList. This will trigger a Null Reference Exception to classes that try to add to this list
            public List<string> NullList { get; set; }
        }
    }
}