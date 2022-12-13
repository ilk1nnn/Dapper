using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Entities
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Score { get; set; }
        public bool IsStar { get; set; }
    }
}
