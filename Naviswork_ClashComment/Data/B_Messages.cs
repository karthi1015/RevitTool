using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naviswork_ClashComment.Data
{
    class data_messages
    {
        public string id { get; set; }
        public string message { get; set; }
        public string time { get; set; }
        public string userId { get; set; }
        public bool edited { get; set; }
        public bool loading { get; set; }
        public bool error { get; set; }
        public bool mark { get; set; }
        public List<object> seenBy { get; set; }
    }
}
