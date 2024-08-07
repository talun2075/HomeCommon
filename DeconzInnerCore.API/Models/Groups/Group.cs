﻿using InnerCore.Api.DeConz.Converters;
using InnerCore.Api.DeConz.Models.Scenes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace InnerCore.Api.DeConz.Models.Groups
{
    [DataContract]
    public class Group
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember]
        public string Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Luminaire / Lightsource / LightGroup
        /// </summary>
        [JsonConverter(typeof(StringNullableEnumConverter))]
        [DataMember(Name = "type")]
        public GroupType? Type { get; set; }

        /// <summary>
        /// The IDs of the lights that are in the group.
        /// </summary>
        [DataMember(Name = "lights")]
        public List<string> Lights { get; set; }

        /// <summary>
        /// The light state of one of the lamps in the group.
        /// </summary>
        [JsonProperty("action", NullValueHandling = NullValueHandling.Ignore)]
        [DataMember(Name = "action")]
        public State Action { get; set; }

        [DataMember(Name = "state")]
        public GroupState State { get; set; }

        [DataMember(Name = "scenes")]
        public List<Scene> Scenes { get; set; }

        #region DeConz specific

        /// <summary>
        /// A list of device ids (sensors) if this group was created by a device.
        /// </summary>
        [DataMember(Name = "devicemembership")]
        public List<string> DeviceMembership { get; set; }

        /// <summary>
        /// HTTP etag which changes on any action to the group.
        /// </summary>
        [DataMember(Name = "etag")]
        public string Etag { get; set; }

        /// <summary>
        /// Indicates the hidden status of the group. Has no effect at the gateway but apps can uses this to hide groups.
        /// </summary>
        [DataMember(Name = "hidden")]
        public bool Hidden { get; set; }

        /// <summary>
        /// A list of light ids of this group that can be sorted by the user. Need not to contain all light ids of this group.
        /// </summary>
        [DataMember(Name = "lightsequence")]
        public List<string> LightSequence { get; set; }

        /// <summary>
        /// A list of light ids of this group that are subsequent ids from multidvices with multiple endpoints like the FLS-PP.
        /// </summary>
        [DataMember(Name = "mulitdeviceids")]
        public List<string> MulitDeviceIds { get; set; }


        #endregion
    }
}
