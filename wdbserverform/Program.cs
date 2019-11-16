using Ding.Agent;

namespace wdbserverform
{
    class Program
    {
      
            static void Main(string[] args) => MyService.ServiceMain();
        

      

    }
    class MyService : AgentServiceBase<MyService>
    {
        public MyService()
        {
            ServiceName = "WeavingDB";
        }

        public override string DisplayName => "WeavingDB";

        public override string Description => "雷达-遥感-等值面图数据库";

        protected override void StartWork(string reason)
        {
            new WeavingDB.Logical.DBcontrol();
            base.StartWork(reason);
        }

        protected override void StopWork(string reason)
        {
            base.StopWork(reason);
        }
    }
}
