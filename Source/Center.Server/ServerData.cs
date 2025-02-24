namespace Center.Server
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.Serialization;

    [DataContract]
    public class ServerData
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Ip { get; set; }

        [DataMember]
        public int LowestLevel { get; set; }

        [DataMember]
        public int MustLevel { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int Online { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public int State { get; set; }
    }
}

