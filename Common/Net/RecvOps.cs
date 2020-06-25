using System;
using System.Collections.Generic;

namespace NineToFive.ReceiveOps {
    public class RecvOps {
        public Dictionary<short, Type> Events { get; private set; }
        public RecvOps() {
            Events = new Dictionary<short, Type>(35);
        }
    }

    public enum CLogin : short {
        OnPasswordCheck = 1
    }
}
