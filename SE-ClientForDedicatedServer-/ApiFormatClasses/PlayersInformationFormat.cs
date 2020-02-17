using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SE_ClientForDedicatedServer_.ApiFormatClasses
{
    class PlayersInformationFormat
    {
        public string Name { get; set; }
        public long UserId { get; set; }
        public DateTime LastLoginTime { get; set; }
        public bool IsProtected { get; set; }
        public int PCU { get; set; }
        public int GridAmount { get; set; }
        public double PCUPercents { get; set; }

        public ObservableCollection<_Grids> grids;

        public PlayersInformationFormat()
        {
            grids = new ObservableCollection<_Grids>();
        }

    }
}
