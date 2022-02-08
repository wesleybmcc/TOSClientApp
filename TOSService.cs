using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThinkOrSwim;

namespace TOSClientApp
{
    internal class TOSService
    {
        private static readonly TOSWorker tosWorker = new TOSWorker();

        public TOSService()
        {
        }

        public void Start()
        {
            new Task(tosWorker.Run).Start();
        }

        public void Stop()
        {
            tosWorker.Dispose();
        }
    }
}
