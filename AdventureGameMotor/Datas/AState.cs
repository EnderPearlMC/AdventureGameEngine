using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvG
{
    class AState
    {

        public string CurrentSceneId { get; set; }
        public Dictionary<string, object> ScenesState { get; set; }

        public AState()
        {
            ScenesState = new Dictionary<string, object>();
        }

    }
}
