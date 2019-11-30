using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Front.Models
{
    public class HostSettings : IHostSettings
    {
        public string DireccionHost { get; set; }
    }
    public interface IHostSettings
    {
        public string DireccionHost { get; set; }
    }
}
