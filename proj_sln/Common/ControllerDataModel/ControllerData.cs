using Common.ControllerDataModel;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Common
{
  /*
   *Svaki kontroler ima lokalni in memory bafer koji se sastoji recnika <ime_uredjaja,lista promena odnosno lista tipa tuple<long,double> 
   * Klasa Controller Data sluzi za slanje podataka sa kontrolera na ams i takodje ako dodje do otkaza kontrolera ova klasa se serijalizuje u xml fajl
   * A kada se kontroler opet pokrene ova klasa se deserijallizuje i kontroler se inicijalizuje podacima iz nje odnosno vracaju se podaci u lokalni bafer
   * kao i ime lokalnog kontrolera
   */


    [DataContract]
    [Serializable]
    public class ControllerData
    {
        [DataMember]
        public int LocalControllerCode { get; set; }

        [DataMember]
        public long UnixTimestamp { get; set; }

        [DataMember]
        public List<DeviceDataListNode> DevicesDataList { get; set; }

        public ControllerData(int dcode,long timestamp,List<DeviceDataListNode> datalist)
        {
            if (datalist == null)
                throw new ArgumentNullException("Lista uredjaja i njihovih promena ne sme biti null!");

            this.DevicesDataList = datalist;
            this.LocalControllerCode = dcode;
            this.UnixTimestamp = timestamp;
        }

        public ControllerData() { }

    }
}
