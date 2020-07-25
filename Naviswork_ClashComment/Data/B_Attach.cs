using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Naviswork_ClashComment.Data
{
    class data_image
    {
        public string src { get; set; }
        public string name { get; set; }
        public string messageId { get; set; }

    }

    class data_attach
    {
        public string messageId { get; set; }
        public List<data_image> images { get; set; }

    }
}
