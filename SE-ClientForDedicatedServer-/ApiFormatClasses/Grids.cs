using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE_ClientForDedicatedServer_.ApiFormatClasses
{
    public class _Grids
    {
        public string DisplayName { get; set; }
        public long EntityId { get; set; }
        public string GridSize { get; set; }
        public int BlocksCount { get; set; }
        public float Mass { get; set; }
        public _Position Position { get; set; }
        public double LinearSpeed { get; set; }
        public double DistanceToPlayer { get; set; }
        public long OwnerSteamId { get; set; }
        public string OwnerDisplayName { get; set; }
        public bool IsPowered { get; set; }
        public int PCU { get; set; }


        public class _Position
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }

            public string Get => $"X: {X}, Y: {Y}, Z: {Z}";

        }
    }
}
