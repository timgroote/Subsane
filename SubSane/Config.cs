using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubSane
{
    public class Config
    {
        public bool PlayWavs { get; set; }
        private static Config _instance = null;

        private Config(bool playWavs)
        {
            PlayWavs = playWavs;
        }

        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Config(false);
                }
                return _instance;
            }
        }
    }
}
