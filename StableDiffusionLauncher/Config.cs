using DotNetJsonConfig.Atrtributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StableDiffusionLauncher
{
    public class Config : DotNetJsonConfig.Config<Config>
    {
        [FileBrowser]
        [JsonProperty]
        public string StableDiffusionBinary { get; set; }

        [JsonProperty]
        public string Arguments { get; set; }

        protected override Config TypedRef => this;
        protected override string File => "appsettings.conf";
    }
}
