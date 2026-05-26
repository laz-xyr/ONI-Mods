using Newtonsoft.Json;
using PeterHan.PLib.Options;

namespace GeoTuner_mod
{

    [ConfigFile("config.json", true, true)]
    [RestartRequired] 
    internal class Options : SingletonOptions<Options>
    {


        [JsonProperty]
        [Option("UI.UISIDESCREENS.OPTION.MAX_TUNING.TITLE", "UI.UISIDESCREENS.OPTION.MAX_TUNING.TOOLTIP", null)]
        [Limit(5,10)]
        public int  Maxcount { get; set; }




        [JsonProperty]
        [Option("UI.UISIDESCREENS.OPTION.GEOTUNERS_RATIO.TITLE", "UI.UISIDESCREENS.OPTION.GEOTUNERS_RATIO.TOOLTIP", null)]
        [Limit(0.1, 10)]
        public float Geotuners_Ratio { get; set; }


        [JsonProperty]
        [Option("UI.UISIDESCREENS.OPTION.GEYSER_RATIO.TITLE", "UI.UISIDESCREENS.OPTION.GEYSER_RATIO.TOOLTIP", null)]
        [Limit(0.1, 10)]
        public float Geyser_Ratio { get; set; }

        [JsonProperty]
        [Option("UI.UISIDESCREENS.OPTION.BROKER_VANILLA.TITLE", "UI.UISIDESCREENS.OPTION.BROKER_VANILLA.TOOLTIP", null)]
        public bool Broker_Vanilla { get; set; }

        [JsonProperty]
        [Option("UI.UISIDESCREENS.OPTION.ENERGYCONSUMER.TITLE", "UI.UISIDESCREENS.OPTION.ENERGYCONSUMER.TOOLTIP", null)]
        public bool energyConsumer { get; set; }


        public Options()
        {
            this.Maxcount = 5;
            this.Geotuners_Ratio = 1;
            this.Geyser_Ratio = 1;
            this.Broker_Vanilla = false;
            this.energyConsumer= true;

        }






    }
}
