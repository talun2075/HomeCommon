﻿using DeconzInnerCore.API.Models.Lights;
using InnerCore.Api.DeConz.ColorConverters;
using InnerCore.Api.DeConz.ColorConverters.HSB.Extensions;
using System.Runtime.Serialization;

namespace InnerCore.Api.DeConz.Models.Lights
{
    [DataContract]
    public class Light
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "hascolor")]
        public bool HasColor { get; set; }

        [DataMember(Name = "colorcapabilities")]
        public int ColorCapabilities { get; set; }

        [DataMember(Name = "ctmax")]
        public int CtMax { get; set; }

        [DataMember(Name = "ctmin")]
        public int CtMin { get; set; }

        [DataMember(Name = "state")]
        public State State { get; set; }

        public string HexColor => State.ToHex();

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "modelid")]
        public string ModelId { get; set; }

        /// <summary>
        /// Unique id of the device. The MAC address of the device with a unique endpoint id in the form: AA:BB:CC:DD:EE:FF:00:11-XX
        /// </summary>
        [DataMember(Name = "uniqueid")]
        public string UniqueId { get; set; }

        [DataMember(Name = "manufacturername")]
        public string ManufacturerName { get; set; }

        [DataMember(Name = "swversion")]
        public string SoftwareVersion { get; set; }

        #region DeConz specific
        [DataMember(Name = "config")]
        public Config  Config { get; set; }
        /// <summary>
        /// HTTP etag which changes on any action to the group.
        /// </summary>
        [DataMember(Name = "etag")]
        public string Etag { get; set; }

        #endregion
    }
}
