using Newtonsoft.Json;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiple_Power_Generator
{
    public abstract class SingletonOptions<T> where T : class, new()
    {
        // Token: 0x17000001 RID: 1
        // (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
        // (set) Token: 0x06000002 RID: 2 RVA: 0x0000209C File Offset: 0x0000029C
        public static T Instance
        {
            get
            {
                bool flag = SingletonOptions<T>.instance == null;
                if (flag)
                {
                    T t;
                    bool flag2 = (t = POptions.ReadSettings<T>()) == null;
                    if (flag2)
                    {
                        t = Activator.CreateInstance<T>();
                    }
                    SingletonOptions<T>.instance = t;
                }
                return SingletonOptions<T>.instance;
            }
            protected set
            {
                bool flag = value != null;
                if (flag)
                {
                    SingletonOptions<T>.instance = value;
                }
            }
        }

        // Token: 0x04000001 RID: 1
        protected static T instance;
    }

    [JsonObject(MemberSerialization.OptIn)]
    [ModInfo("", null, false)]
    [ConfigFile("config.json", true, true)]
    [RestartRequired]
    internal class Options : SingletonOptions<Options>
    {


        [JsonProperty]
        [Option("发电机倍率", "Increase the multiple of the generator", null)]
        [Limit(0.1,100)]
        public float PowerRatio { get; set; }

        // Token: 0x17000189 RID: 393
        // (get) Token: 0x06000643 RID: 1603 RVA: 0x00019728 File Offset: 0x00017928
        // (set) Token: 0x06000644 RID: 1604 RVA: 0x00019730 File Offset: 0x00017930
        [JsonProperty]
        [Option("电线倍率", "WireRatio default is 100.", null)]
        [Limit(0.1, 1000)]
        public float WireRatio { get; set; }

        // Token: 0x1700018A RID: 394
        // (get) Token: 0x06000645 RID: 1605 RVA: 0x00019739 File Offset: 0x00017939
        // (set) Token: 0x06000646 RID: 1606 RVA: 0x00019741 File Offset: 0x00017941
        [JsonProperty]
        [Option("电池倍率", "BatteryRatio default is 10.", null)]
        [Limit(0.1, 100)]
        public float BatteryRatio { get; set; }

        // Token: 0x06000647 RID: 1607 RVA: 0x0001974A File Offset: 0x0001794A
        public Options()
        {
            this.PowerRatio = 10f;
            this.WireRatio = 100f;
            this.BatteryRatio = 10f;
        }






    }
}
