﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using InnerCore.Api.DeConz.Converters;
using InnerCore.Api.DeConz.Models.Schedule;
using Newtonsoft.Json;

namespace InnerCore.Api.DeConz.Models.Rules
{
    [DataContract]
    public class Rule
    {
        public string Id { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "Lasttriggered")]
        [JsonConverter(typeof(NullableDateTimeConverter))]
        public DateTime? LastTriggered { get; set; }

        [DataMember(Name = "creationtime")]
        [JsonConverter(typeof(NullableDateTimeConverter))]
        public DateTime? CreationTime { get; set; }

        [DataMember(Name = "timestriggered")]
        public int TimesTriggered { get; set; }

        [DataMember(Name = "owner")]
        public string Owner { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "conditions")]
        public List<RuleCondition> Conditions { get; set; }

        [DataMember(Name = "actions")]
        public List<InternalBridgeCommand> Actions { get; set; }
    }
}
