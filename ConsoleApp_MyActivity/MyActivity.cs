using System;
using System.Threading;

namespace ConsoleApp_MyActivity
{
    public class MyActivity : IDisposable
    {
        private static readonly AsyncLocal<MyActivity> CurrentActivity = new AsyncLocal<MyActivity>();
        private readonly string _id;
        private readonly string _parentId;
        private readonly string _operationName;

        public MyActivity(string operationName, string parentId = null)
        {
            _id = Guid.NewGuid().ToString();
            _parentId = parentId ?? Current?.Id;
            _operationName = operationName;
        }

        public string Id => _id;
        public string TraceId => _id;
        public string SpanId => _id;
        public string ParentId => _parentId;
        public string OperationName => _operationName;

        public static MyActivity Current => CurrentActivity.Value;


        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            CurrentActivity.Value = this;
        }

        public void Stop()
        {
            CurrentActivity.Value = null;
        }
    }
}
