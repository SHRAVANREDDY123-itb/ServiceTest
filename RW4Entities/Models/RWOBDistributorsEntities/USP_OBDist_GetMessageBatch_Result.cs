﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RW4Entities.Models.RWOBDistributorsEntities
{
    public partial class USP_OBDist_GetMessageBatch_Result
    {
        public Guid conversation_handle { get; set; }
        public string service_name { get; set; }
        public string message_type_name { get; set; }
        public byte[] message_body { get; set; }
        public long message_sequence_number { get; set; }
    }
}
